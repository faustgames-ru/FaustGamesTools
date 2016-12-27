using Assimp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentBuilder
{
    public struct UV
    {
        public ushort U;
        public ushort V;

        public UV(ushort u, ushort v)
        {
            U = u;
            V = v;
        }
    }

    public class ConverterFbx
    {
        List<ushort> Faces = new List<ushort>();
        List<Vector3D> Positions = new List<Vector3D>();
        List<UV> UVs = new List<UV>();
        List<Vector3D> Normals = new List<Vector3D>();
        List<Vector3D> Tangents = new List<Vector3D>();
        List<Vector3D> BiTangents = new List<Vector3D>();

        public void Convert(string sourseFile, string targetFile)
        {
            Positions.Clear();
            Normals.Clear();
            Tangents.Clear();
            BiTangents.Clear();
            UVs.Clear();
            Faces.Clear();

            using (var importer = new AssimpContext())
            {
                var model = importer.ImportFile(sourseFile, PostProcessPreset.TargetRealTimeQuality | PostProcessPreset.ConvertToLeftHanded);
                ParseNode(model.RootNode, model);

                using (var writer = new BinaryWriter(File.Create(targetFile)))
                {
                    var diffuse = string.Empty;
                    var specularNormal = string.Empty;
                    if (model.HasMaterials)
                    {
                        var material = model.Materials[0];
                        if (material.HasTextureDiffuse)
                        {
                            diffuse = Path.GetFileName(material.TextureDiffuse.FilePath);
                        }
                        var diffuseSourceFile = Path.Combine(Path.GetDirectoryName(sourseFile), diffuse);
                        var diffuseTargetFile = Path.Combine(Path.GetDirectoryName(targetFile), diffuse);
                        if (File.Exists(diffuseSourceFile))
                        {
                            File.Copy(diffuseSourceFile, diffuseTargetFile, true);
                        }
                        // todo: unite specular and normal map                                                
                    }

                    WriteString(writer, diffuse);
                    WriteString(writer, specularNormal);

                    writer.Write(Faces.Count);
                    foreach (var i in Faces)
                    {
                        writer.Write(i);
                    }

                    writer.Write(Positions.Count);
                    foreach (var p in Positions)
                    {
                        WriteVector(writer, p);
                    }
                    foreach (var uv in UVs)
                    {
                        writer.Write(uv.U);
                        writer.Write(uv.V);
                    }
                    foreach (var n in Normals)
                    {
                        WriteVector(writer, n);
                    }
                    foreach (var t in Tangents)
                    {
                        WriteVector(writer, t);
                    }
                    foreach (var b in BiTangents)
                    {
                        WriteVector(writer, b);
                    }
                }
            }
        }

        public byte[] GetStringBytes(string value)
        {
            return Encoding.Convert(Encoding.Unicode, Encoding.ASCII, Encoding.Unicode.GetBytes(value));
        }

        void WriteVector(BinaryWriter writer, Vector3D value)
        {
            writer.Write(value.X);
            writer.Write(value.Y);
            writer.Write(value.Z);
        }

        void WriteString(BinaryWriter writer, string value)
        {
            var asciiBytes = GetStringBytes(value);
            writer.Write(asciiBytes.Length);
            foreach (var b in asciiBytes)
            {
                writer.Write(b);
            }
        }

        public void ParseNode(Node node, Scene model)
        {
            var transform = GetFullNodeTransform(node);
            var transform3 = new Matrix3x3(transform);
            for (var i = 0; i < node.MeshCount; i++)
            {
                var meshIndex = node.MeshIndices[i];
                /*
                for (var j = 0; j < model.Meshes[meshIndex].VertexCount; j++)
                {
                    var v = model.Meshes[meshIndex].Vertices[j] = transform * model.Meshes[meshIndex].Vertices[j];
                    if (j < model.Meshes[meshIndex].Normals.Count)
                    {
                        model.Meshes[meshIndex].Normals[j] = transform3 * model.Meshes[meshIndex].Normals[j];
                    }
                    if (j < model.Meshes[meshIndex].Tangents.Count)
                    {
                        model.Meshes[meshIndex].Tangents[j] = transform3 * model.Meshes[meshIndex].Tangents[j];
                    }
                    if (j < model.Meshes[meshIndex].Tangents.Count)
                    {
                        model.Meshes[meshIndex].Tangents[j] = transform3 * model.Meshes[meshIndex].BiTangents[j];
                    }
                }
                */
                ParseMesh(model.Meshes[meshIndex], transform, transform3);
            }
            for (var i = 0; i < node.ChildCount; i++)
            {
                var child = node.Children[i];
                ParseNode(child, model);
            }
        }

        public void ParseMesh(Mesh mesh, Matrix4x4 transform, Matrix4x4 transform3)
        {
            int baseIndex = Positions.Count();
            for (var j = 0; j < mesh.VertexCount; j++)
            {
                var v = transform * mesh.Vertices[j];
                var n = new Vector3D(0.0f, 0.0f, 0.0f);
                var t = new Vector3D(0.0f, 0.0f, 0.0f);
                var b = new Vector3D(0.0f, 0.0f, 0.0f);
                var uv = new UV(0, 0);
                if (mesh.HasNormals)
                {
                    n = transform3 * mesh.Normals[j];
                }
                if (mesh.HasTangentBasis)
                {
                    t = transform3 * mesh.Tangents[j];
                    b = transform3 * mesh.BiTangents[j];
                }
                if (mesh.HasTextureCoords(0))
                {
                    uv.U = (ushort)(mesh.TextureCoordinateChannels[0][j].X * ushort.MaxValue);
                    uv.V = (ushort)(mesh.TextureCoordinateChannels[0][j].Y * ushort.MaxValue);
                }

                Positions.Add(v);
                Normals.Add(n);
                Tangents.Add(t);
                BiTangents.Add(b);
                UVs.Add(uv);
            }


            for (var j = 0; j < mesh.FaceCount; j++)
            {
                var face = mesh.Faces[j];
                switch (face.IndexCount)
                {
                    case 1:
                    case 2:
                        // todo: support points & edges ?
                        break;
                    case 3:
                        for (int k = 0; k < face.IndexCount; k++)
                        {
                            var index = baseIndex + face.Indices[k];
                            Faces.Add((ushort)index);
                        }
                        break;
                    default:
                        var index0 = baseIndex + face.Indices[0];
                        for (var k = 1; k < (face.IndexCount - 1); k++)
                        {
                            var index1 = baseIndex + face.Indices[k];
                            var index2 = baseIndex + face.Indices[k + 1];
                            Faces.Add((ushort)index0);
                            Faces.Add((ushort)index1);
                            Faces.Add((ushort)index2);
                        }
                        break;
                }

            }
        }

        public Matrix4x4 GetFullNodeTransform(Node node)
        {
            var result = Matrix4x4.Identity;
            while (node != null)
            {
                result = result * node.Transform;
                node = node.Parent;
            }
            return result;
        }
    }
}
