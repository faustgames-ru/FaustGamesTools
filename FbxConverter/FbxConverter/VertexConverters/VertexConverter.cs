using System;
using System.Collections.Generic;
using Assimp;

namespace FbxConverter.VertexConverters
{
    public class ConverterContext
    {
        public List<float> ValuesList = new List<float>();
        public List<List<float>> AllValues = new List<List<float>>();
        public List<ushort> Indices = new List<ushort>();
        public MeshTransform[] Transforms;
        public int MeshIndex;
        public Mesh Mesh;
        public int VertexIndex;
        public int VertexSize;

        public void AddIndex(int index)
        {
            if ((index < ushort.MinValue) || (index > ushort.MaxValue))
                throw new IndexOutOfRangeException(string.Format("index buffer value mus be in range [{0}..{1}]", ushort.MinValue, ushort.MaxValue));
            Indices.Add((ushort)index);
        }
    }

    public class MeshTransform 
    {
        public Matrix4x4 Transform = Matrix4x4.Identity;
        public float Radius;
    }

    public static class VertexConverterFactory
    {
        public static VertexConverter CreteConverter(VertexDeclarationTypes type)
        {
            switch (type)
            {
                case VertexDeclarationTypes.Position:
                    return new VertexConverterPosition();
                case VertexDeclarationTypes.Normal:
                    return new VertexConverterNormal();
                case VertexDeclarationTypes.Tangent:
                    return new VertexConverterTangent();
                case VertexDeclarationTypes.BiTangent:
                    return new VertexConverterBiTangent();
                case VertexDeclarationTypes.VertexColor:
                    return new VertexConverterColor();
                case VertexDeclarationTypes.TexturePosition2D:
                    return new VertexConverterTexturePosition2D();
                case VertexDeclarationTypes.MeshIndex:
                    return new VertexConverterMeshIndex();
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public abstract class VertexConverter
    {
        public abstract void Convert(ConverterContext context);
    }
    public class VertexConverterPosition : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            context.ValuesList.Add(context.Mesh.Vertices[context.VertexIndex].X);
            context.ValuesList.Add(context.Mesh.Vertices[context.VertexIndex].Y);
            context.ValuesList.Add(context.Mesh.Vertices[context.VertexIndex].Z);
        }
    }

    public class VertexConverterNormal : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            context.ValuesList.Add(context.Mesh.Normals[context.VertexIndex].X);
            context.ValuesList.Add(context.Mesh.Normals[context.VertexIndex].Y);
            context.ValuesList.Add(context.Mesh.Normals[context.VertexIndex].Z);
        }
    }

    public class VertexConverterTangent : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            context.ValuesList.Add(context.Mesh.Tangents[context.VertexIndex].X);
            context.ValuesList.Add(context.Mesh.Tangents[context.VertexIndex].Y);
            context.ValuesList.Add(context.Mesh.Tangents[context.VertexIndex].Z);
        }
    }

    public class VertexConverterBiTangent : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            context.ValuesList.Add(context.Mesh.BiTangents[context.VertexIndex].X);
            context.ValuesList.Add(context.Mesh.BiTangents[context.VertexIndex].Y);
            context.ValuesList.Add(context.Mesh.BiTangents[context.VertexIndex].Z);
        }
    }

    public class VertexConverterColor : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            var colors = context.Mesh.VertexColorChannels[0];
            var color = new Color4D(1, 1, 1, 1);
            if (context.VertexIndex < colors.Count)
                color = colors[context.VertexIndex];
            context.ValuesList.Add(color.R);
            context.ValuesList.Add(color.G);
            context.ValuesList.Add(color.B);
            context.ValuesList.Add(color.A);
        }
    }

    public class VertexConverterTexturePosition2D : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            var texChanel = context.Mesh.TextureCoordinateChannels[0];
            var texCoord = new Vector3D(0, 0, 0);
            if (context.VertexIndex < texChanel.Count)
                texCoord = texChanel[context.VertexIndex];
            context.ValuesList.Add(texCoord.X);
            context.ValuesList.Add(texCoord.Y);
        }
    }

    public class VertexConverterMeshIndex : VertexConverter
    {
        public override void Convert(ConverterContext context)
        {
            context.ValuesList.Add(context.MeshIndex);
        }
    }
}
