using System.Collections.Generic;
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

        public static void ParseNode(ConverterContext context, List<VertexConverter> converters, Node node, Scene model, ref int baseIndex, Matrix4x4[] matrixes)
        {
            for (var i = 0; i < node.MeshCount; i++)
            {
                var meshIndex = node.MeshIndices[i];
                matrixes[meshIndex] = GetFullNodeTransform(node);
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
            var model = importer.ImportFile(sourseFile, PostProcessPreset.TargetRealTimeQuality);
            var context = new ConverterContext();
            var converters = configuration.VertexDeclarations.Select(converter => VertexConverterFactory.CreteConverter(converter.Type)).ToList();
            var baseIndex = 0;
            var matrices = new Matrix4x4[model.Meshes.Count];
            for (var i = 0; i < matrices.Length; i++ )
                matrices[i] = Matrix4x4.Identity;
            ParseNode(context, converters, model.RootNode, model, ref baseIndex, matrices);
            context.Transforms = matrices;
            SaveVerticesToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_v.v"));
            SaveIndicesToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_i.i"));
            SaveTransformsToBinaryFile(context, Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(sourseFile) + "_t.t"));
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
                    for (var j = 1; j <= 4; j++)
                    {
                        for (var i = 1; i <= 4; i++)
                        {
                            writer.Write(transform[i, j]);
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
