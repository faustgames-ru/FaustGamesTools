using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using FbxConverter.VertexConverters;

namespace FbxConverter
{
    public class ConverterFbx
    {
        public static void ParseMesh(ConverterContext context, List<VertexConverter> converters, Mesh mesh, int meshIndex, ref int baseIndex)
        {
            context.Mesh = mesh;
            context.MeshIndex = meshIndex;
            for (var j = 0; j < context.Mesh.VertexCount; j++)
            {
                context.VertexIndex = j;
                context.ValuesList = new List<float>();
                context.AllValues.Add(context.ValuesList);
                foreach (var converter in converters)
                {
                    converter.Convert(context);
                }
                context.VertexSize = context.ValuesList.Count;
            }
            for (var j = 0; j < context.Mesh.FaceCount; j++)
            {
                var face = context.Mesh.Faces[j];
                switch (face.IndexCount)
                {
                    case 1:
                    case 2:
                        // todo: support points & edges ?
                        break;
                    case 3:
                        for (int k = 0; k < face.IndexCount; k++)
                        {
                            var index = face.Indices[k];
                            context.AddIndex(baseIndex + index);
                        }
                        break;
                    default:
                        var index0 = baseIndex + face.Indices[0];
                        for (var k = 1; k < (face.IndexCount - 1); k++)
                        {
                            var index1 = baseIndex + face.Indices[k];
                            var index2 = baseIndex + face.Indices[k + 1];
                            context.AddIndex(index0);
                            context.AddIndex(index1);
                            context.AddIndex(index2);
                        }
                        break;
                }

            }
            baseIndex = context.AllValues.Count;
        }

        public static Matrix4x4 GetFullNodeTransform(Node node)
        {
            var result = Matrix4x4.Identity;
            while (node != null)
            {
                result = result * node.Transform;
                node = node.Parent;
            }
            return result;
        }

        public static void ParseNode(ConverterContext context, List<VertexConverter> converters, Node node, Scene model, ref int baseIndex, MeshTransform[] matrixes)
        {
            for (var i = 0; i < node.MeshCount; i++)
            {
                var meshIndex = node.MeshIndices[i];
                matrixes[meshIndex].Transform = GetFullNodeTransform(node);

                float r = 0.0f;
                var min = new Vector3D(float.MaxValue);
                var max = new Vector3D(-float.MaxValue);
                for (var j = 0; j < model.Meshes[meshIndex].VertexCount; j++)
                {
                    var v = model.Meshes[meshIndex].Vertices[j] = matrixes[meshIndex].Transform * model.Meshes[meshIndex].Vertices[j];
                    if (j < model.Meshes[meshIndex].Normals.Count)
                    {
                        model.Meshes[meshIndex].Normals[j] = new Matrix3x3(matrixes[meshIndex].Transform)*
                                                             model.Meshes[meshIndex].Normals[j];
                    }
                    if (j < model.Meshes[meshIndex].Tangents.Count)
                    {
                        model.Meshes[meshIndex].Tangents[j] = new Matrix3x3(matrixes[meshIndex].Transform)*
                                                              model.Meshes[meshIndex].Tangents[j];
                    }
                    if (j < model.Meshes[meshIndex].Tangents.Count)
                    {
                        model.Meshes[meshIndex].Tangents[j] = new Matrix3x3(matrixes[meshIndex].Transform) *
                                                                model.Meshes[meshIndex].BiTangents[j];
                    }
                    if (min.X > v.X) 
                        min.X = v.X;
                    if (min.Y > v.Y)
                        min.Y = v.Y;
                    if (min.Z > v.Z)
                        min.Z = v.Z;
                    if (max.X < v.X)
                        max.X = v.X;
                    if (max.Y < v.Y)
                        max.Y = v.Y;
                    if (max.Z < v.Z)
                        max.Z = v.Z;
                }
                var center = (max + min) * 0.5f;
                for (var j = 0; j < model.Meshes[meshIndex].VertexCount; j++)
                {
                    var v = model.Meshes[meshIndex].Vertices[j] = model.Meshes[meshIndex].Vertices[j];// - center;
                    var l = v.Length();
                    if (l > r)
                        r = l;
                }
                matrixes[meshIndex].Transform = Matrix4x4.FromTranslation(center);
                matrixes[meshIndex].Radius = r;
                ParseMesh(context, converters, model.Meshes[meshIndex], meshIndex, ref baseIndex);
            }
            for (var i = 0; i < node.ChildCount; i++)
            {
                var child = node.Children[i];
                ParseNode(context, converters, child, model, ref baseIndex, matrixes);
            }
        }

        public static void Convert(string sourseFile, string targetDirectory, Configuration configuration)
        {
            var importer = new AssimpContext();
            //importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var model = importer.ImportFile(sourseFile, PostProcessPreset.TargetRealTimeQuality|PostProcessPreset.ConvertToLeftHanded);
            var context = new ConverterContext();
            var converters = configuration.VertexDeclarations.Select(converter => VertexConverterFactory.CreteConverter(converter.Type)).ToList();
            var baseIndex = 0;
            var matrices = new MeshTransform[model.Meshes.Count];
            for (var i = 0; i < matrices.Length; i++ )
                matrices[i] = new MeshTransform();
            ParseNode(context, converters, model.RootNode, model, ref baseIndex, matrices);
            context.Transforms = matrices;
            SaveVerticesAndIndicesToCCharpCode(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "Resources.cs"));
            //SaveVerticesToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_v.v"));
            //SaveIndicesToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_i.i"));
            //SaveTransformsToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_t.t"));
        }

        public static void SaveVerticesAndIndicesToCCharpCode(ConverterContext context, string fileName)
        {
            // todo: use formaters
            using (var writer = new StreamWriter(File.Create(fileName)))
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                writer.WriteLine("using Moon.Framework.Engine.Graphics.GraphicsCanvas;");
                writer.WriteLine("using Moon.Framework.Mathematics;");
                writer.WriteLine("");
                writer.WriteLine("namespace LogoResources");
                writer.WriteLine("{");
                writer.WriteLine("\tpublic static class Resource");
                writer.WriteLine("\t{");
                writer.WriteLine("\t\tpublic static SolidVertex[] Vertices = ");
                writer.WriteLine("\t\t{");
                foreach (var vertex in context.AllValues)
                {
                    writer.WriteLine("\t\t\tnew SolidVertex(new Vector({0:0.000}f, {1:0.000}f, 0.0f), new Color(0x{2:X})),", vertex[0], vertex[2], Color.FromArgb(
                        (byte)(vertex[3] * 255),
                        (byte)(vertex[4] * 255),
                        (byte)(vertex[5] * 255), 
                        (byte)(vertex[6] * 255)).ToArgb());
                }
                writer.WriteLine("\t\t};");
                writer.WriteLine("");
                writer.WriteLine("\t\tpublic static short[] Indices = ");
                writer.WriteLine("\t\t{");
                var counter = 0;
                writer.Write("\t\t\t");
                foreach (var index in context.Indices)
                {
                    writer.Write("{0}, ", index);
                    counter++;
                    if (counter > 18)
                    {
                        writer.WriteLine();
                        writer.Write("\t\t\t");
                        counter = 0;
                    }
                }
                if (counter != 0)
                    writer.WriteLine();
                writer.WriteLine("\t\t};");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }
        }

        public static void SaveVerticesToBinaryFile(ConverterContext context, string fileName)
        {
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                foreach (var vertex in context.AllValues)
                    foreach (var value in vertex)
                        writer.Write(value);
            }
        }

        public static void SaveIndicesToBinaryFile(ConverterContext context, string fileName)
        {
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                foreach (var index in context.Indices)
                    writer.Write(index);
            }
        }

        public static void SaveTransformsToBinaryFile(ConverterContext context, string fileName)
        {
            using (var writer = new BinaryWriter(File.Create(fileName)))
            {
                foreach (var transform in context.Transforms)
                {
                    writer.Write(transform.Radius);
                    for (var j = 1; j <= 4; j++)
                    {
                        for (var i = 1; i <= 4; i++)
                        {
                            writer.Write(transform.Transform[i, j]);
                        }
                    }
                }
            }
        }

        public static Task ConvertAsync(string sourseFile, string targetDirectory, Configuration configuration)
        {
            var result = new Task(() => Convert(sourseFile, targetDirectory, configuration));
            result.Start();
            return result;
        }
    }
}
