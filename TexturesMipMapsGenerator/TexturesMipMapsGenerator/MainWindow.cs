using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace TexturesMipMapsGenerator
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string GetTempPath() 
        {
            return Path.Combine(Application.StartupPath, "temp");
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Color 
        {
            public byte b;
            public byte g;
            public byte r;
            public byte a;
            public byte GetIntense() 
            {
                return (byte)(((int)r + (int)g + (int)b + (int)a) >> 2);
            }
        }

        private unsafe void SaveMipmap(Bitmap bitmap, string targetPath, string fileName) 
        {
            Application.DoEvents();

            var width = bitmap.Width;
            var height = bitmap.Height;
            var newFileName = fileName + "_" + width + "x" + height + ".png";

            bitmap.Save(Path.Combine(targetPath, newFileName), ImageFormat.Png);

            if ((width == 1) || (height == 1)) return;

            var mipmapWidth = bitmap.Width >> 1;
            var mipmapHeight = bitmap.Height >> 1;
            using (var mipmap = new Bitmap(mipmapWidth, mipmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) 
            {
                var sourceBits = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var targetBits = mipmap.LockBits(new Rectangle(0, 0, mipmapWidth, mipmapHeight), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var sourceScan = (Color*)sourceBits.Scan0;
                var targetScan = (Color*)targetBits.Scan0;
                int x, y;

                Color* sourceRow;
                Color* sourceRow1;

                Color result;
                Color source1;
                Color source2;
                Color source3;
                Color source4;
                for (y = 0; y < mipmapHeight; y++)
                {
                    sourceRow = sourceScan;
                    sourceRow1 = sourceRow + width;
                    for (x = 0; x < mipmapWidth; x++)
                    {
                        source1 = (*sourceRow);
                        source2 = (*sourceRow1);
                        sourceRow ++;
                        sourceRow1 ++;
                        source3 = (*sourceRow);
                        source4 = (*sourceRow);                        
                        sourceRow++;
                        sourceRow1++;
                        result.r = (byte)(((int)source1.r + (int)source2.r + (int)source3.r + (int)source4.r) >> 2);
                        result.g = (byte)(((int)source1.g + (int)source2.g + (int)source3.g + (int)source4.g) >> 2);
                        result.b = (byte)(((int)source1.b + (int)source2.b + (int)source3.b + (int)source4.b) >> 2);
                        result.a = (byte)(((int)source1.a + (int)source2.a + (int)source3.a + (int)source4.a) >> 2);
                        
                        (*targetScan) = result;
                        targetScan++;
                    }
                    sourceScan = sourceRow1;
                }
                
                bitmap.UnlockBits(sourceBits);
                mipmap.UnlockBits(targetBits);

                SaveMipmap(mipmap, targetPath, fileName);

            }
        }

        private unsafe Bitmap CreateNormalMap(Bitmap bitmap) 
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            var heightMap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var sourceBits = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var targetBits = heightMap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var sourceScan = (Color*)sourceBits.Scan0;
            var targetScan = (Color*)targetBits.Scan0;
            int x, y;

            Color* sourceRow;
            Color* sourceRow1;

            Color result;
            Color source;
            Color source1;
            Color source2;
            Color source3;
            Color source4;

            double h;
            double hxp;
            double hxm;
            double hyp;
            double hym;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    h = (*(sourceScan + y * width + x)).GetIntense();
                    if (x == 0)
                    {
                        hxp = (*(sourceScan + y * width + x + 1)).GetIntense();
                        hxm = (*(sourceScan + y * width + width - 1)).GetIntense();
                    }
                    else if (x == (width - 1))
                    {
                        hxp = (*(sourceScan + y * width + 0)).GetIntense();
                        hxm = (*(sourceScan + y * width + x - 1)).GetIntense();
                    }
                    else
                    {
                        hxp = (*(sourceScan + y * width + x + 1)).GetIntense();
                        hxm = (*(sourceScan + y * width + x - 1)).GetIntense();
                    }
                    if (y == 0) 
                    {
                        hyp = (*(sourceScan + (y + 1) * width + x)).GetIntense();
                        hym = (*(sourceScan + (height - 1) * width + x)).GetIntense();
                    }
                    else if (y == (height - 1))
                    {
                        hyp = (*(sourceScan + (0) * width + x)).GetIntense();
                        hym = (*(sourceScan + (y - 1) * width + x)).GetIntense();
                    }
                    else
                    {
                        hyp = (*(sourceScan + (y + 1) * width + x)).GetIntense();
                        hym = (*(sourceScan + (y - 1) * width + x)).GetIntense();
                    }

                    var resultX = (hxm - h) + (h - hxp);
                    var resultY = (hym - h) + (h - hyp);
                    var resultZ = 1.0;

                    var l = Math.Sqrt(resultX * resultX + resultY * resultY + resultZ * resultZ);
                    resultX /= l;
                    resultY /= l;
                    resultZ /= l;

                    result.r = (byte)((resultX + 1) * 0.5 * 255);
                    result.g = (byte)((resultY + 1) * 0.5 * 255);
                    result.b = (byte)((resultZ + 1) * 0.5 * 255);
                    result.a = 255;

                    (*(targetScan + y * width + x)) = result;
                }
            }

            bitmap.UnlockBits(sourceBits);
            heightMap.UnlockBits(targetBits);

            return heightMap;
        }

        class Matrix2 
        {
            public float[] values = new float[4];

            public static Matrix2 Identity = new Matrix2(
                    1, 0,
                    0, 1);

            public Matrix2() {

            }

            public Matrix2(
                    float xx, float xy,
                    float yx, float yy) {
                values[0] = xx;
                values[1] = xy;
                values[2] = yx;
                values[3] = yy;
            }

            public Vertex transform(Vertex v){
                return new Vertex(
                        values[0] * v.get(0) + values[1] * v.get(1),
                        values[2] * v.get(0) + values[3] * v.get(1));
            }

            public float get(int row, int column) {
                return values[row * 2 + column];
            }

            public void set(int row, int column, float value) {
                values[row * 2 + column] = value;
            }

            public float determinant(){
                return values[0] * values[3] - values[1] * values[2];
            }

            public Matrix2 createInverse() {
                float dt = 1.0f / determinant();
                return new Matrix2(
                        values[3] * dt,  -values[1] * dt,
                       -values[2] * dt,   values[0] * dt);
            }

            public Matrix2 createTranspose() {
                return new Matrix2(
                        values[0], values[2],
                        values[1], values[3]);
            }

            public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2, Matrix2 m3, Matrix2 m4, Matrix2 m5, Matrix2 m6){
                return  Multiply(Multiply(m1, m2, m3, m4, m5), m6);
            }

            public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2, Matrix2 m3, Matrix2 m4, Matrix2 m5){
                return  Multiply(Multiply(m1, m2, m3, m4), m5);
            }

            public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2, Matrix2 m3, Matrix2 m4){
                return  Multiply(Multiply(m1, m2, m3), m4);
            }

            public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2, Matrix2 m3){
                return  Multiply(Multiply(m1, m2), m3);
            }

            public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2){
                Matrix2 r = new Matrix2(0, 0, 0, 0);
                int i, j, k;

                for (i=0; i<2; ++i)
                    for (j=0; j<2; ++j)  {
                        for (k=0; k<2; ++k)
                            r.set(i, j, r.get(i, j) + m1.get(i, k) * m2.get(k, j));
                    }
                return r;
            }

            public float getXX() {
                return values[0];
            }
            public float getXY() {
                return values[1];
            }

            public float getYX() {
                return values[2];
            }
            public float getYY() {
                return values[3];
            }
        }

        class Vertex 
        {
            public float x;
            public float y;
            public float z;

            public Vertex()
            {
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
            }

            public Vertex(float x, float y)
            {
                this.x = x;
                this.y = y;
                z = 0;
            }

            public Vertex(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public float get(int i)
            {
                if (i == 0)
                    return x;
                if (i == 1)
                    return y;
                if (i == 2)
                    return z;
                return 0;
            }

            public static Vertex operator *(Vertex a, Vertex b)
            {
                return new Vertex
                {
                    x = a.x * b.x,
                    y = a.y * b.y,
                    z = a.z * b.z,
                };
            }

            public static Vertex operator *(Vertex a, float b)
            {
                return new Vertex
                {
                    x = a.x * b,
                    y = a.y * b,
                    z = a.z * b,
                };
            }

            public static Vertex operator +(Vertex a, Vertex b)
            {
                return new Vertex
                    {
                        x = a.x + b.x,
                        y = a.y + b.y,
                        z = a.z + b.z,
                    };
            }

            public static Vertex operator -(Vertex a, Vertex b)
            {
                return new Vertex
                {
                    x = a.x - b.x,
                    y = a.y - b.y,
                    z = a.z - b.z,
                };
            }

            public static Vertex CrossProduct(Vertex u, Vertex v)
            {
                return new Vertex
                {
                    x = u.y*v.z - u.z*v.y,
                    y = u.z*v.x - u.x*v.z,
                    z = u.x*v.y - u.y*v.x
                };
            }

            public void Write(BinaryWriter writer, bool bigEndain) 
            {
                if (bigEndain)
                {
                    writer.Write(CompoundVertex.InverseBytes(x));
                    writer.Write(CompoundVertex.InverseBytes(y));
                    writer.Write(CompoundVertex.InverseBytes(z));
                }
                else
                {
                    writer.Write(x);
                    writer.Write(y);
                    writer.Write(z);
                }
            }
            public static Vertex FromString(string s)
            {
                var a = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                s = s.Replace('.', a);
                var split = s.Split(' ');
                return new Vertex
                {
                    x = float.Parse(split[0]),
                    y = float.Parse(split[1]),
                    z = float.Parse(split[2])
                };
            }

            public void NormalizeSelf()
            {
                var l = Math.Sqrt(x*x + y*y + z*z);
                x = (float) (x/l);
                y = (float) (y/l);
                z = (float) (z/l);
            }
        }

        class CompoundVertex 
        {
            public int v;
            public int vt;
            public int vn;
            public string key;

            public int Index;
            public Vertex Position;
            public Vertex Normal;
            public Vertex BiNormal;
            public Vertex Tangent;
            public Vertex Texture;

            public List<Triangle> Triangles = new List<Triangle>();

            public void Build()
            {
                //Normal = new Vertex();
                Tangent = new Vertex();
                BiNormal = new Vertex();
                foreach (var t in Triangles)
                {
                    //Normal += t.Normal;
                    Tangent += t.Tangent;
                    BiNormal += t.BiNormal;
                }

                //Normal.NormalizeSelf();
                Tangent.NormalizeSelf();
                BiNormal.NormalizeSelf();            
            }

            public void Write(BinaryWriter writer) 
            {
                writer.Write(Index);
                Position.Write(writer, false);
                writer.Write(Texture.x);
                writer.Write(Texture.y);
                Normal.Write(writer, false);
                BiNormal.Write(writer, false);
                Tangent.Write(writer, false);
            }

            public static float InverseBytes(float v)
            {
                var bytes = BitConverter.GetBytes(v);
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
            
            public void WriteBatch(BinaryWriter writer, int meshIndex, bool bigEndain)
            {
                Position.Write(writer, bigEndain);
                Normal.Write(writer, bigEndain);
                BiNormal.Write(writer, bigEndain);
                Tangent.Write(writer, bigEndain);
                if (bigEndain)
                {
                    writer.Write(InverseBytes(Texture.x));
                    writer.Write(InverseBytes(Texture.y));
                    writer.Write(InverseBytes(meshIndex));
                }
                else
                {
                    writer.Write(Texture.x);
                    writer.Write(Texture.y);
                    writer.Write((float)meshIndex);
                }
            }

            public static CompoundVertex FromString(string s, Dictionary<string, CompoundVertex> compoundVertexes)
            {
                var split = s.Split('/');
                var result = new CompoundVertex
                {
                    v = int.Parse(split[0]),
                    vt = int.Parse(split[1]),
                    vn = int.Parse(split[2]),
                };
                result.key = result.v.ToString() + "/" + result.vn.ToString() + "/" + result.vt.ToString();
                if (compoundVertexes.ContainsKey(result.key))
                    return compoundVertexes[result.key];
                compoundVertexes.Add(result.key, result);
                return result;
            }
        }

        class Triangle 
        {
            public CompoundVertex a;
            public CompoundVertex b;
            public CompoundVertex c;

            public Vertex Normal;
            public Vertex BiNormal;
            public Vertex Tangent;

            public void Build()
            {
                Vertex ba = b.Position - a.Position; // Vertex.sub(getB().getPosition(), getA().getPosition());
                Vertex ca = c.Position - a.Position; // Vertex.sub(getC().getPosition(), getA().getPosition());

                Vertex tBA = b.Texture - a.Texture; // Vertex.sub(getB().getTexturePosition(), getA().getTexturePosition());
                Vertex tCA = c.Texture - a.Texture; // Vertex.sub(getC().getTexturePosition(), getA().getTexturePosition());


                Matrix2 tMatrix = new Matrix2(
                        tBA.x, tBA.y,
                        tCA.x, tCA.y).createInverse();

                Vertex ex = tMatrix.transform(new Vertex(1, 0));
                Vertex ey = tMatrix.transform(new Vertex(0, 1));

                Tangent = ba*ex.x + ca*ex.y;

                BiNormal = ba * ey.x + ca * ey.y;

                Normal = Vertex.CrossProduct(ba, ca) * -1.0f;

                a.Triangles.Add(this);
                b.Triangles.Add(this);
                c.Triangles.Add(this);
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(a.Index);
                writer.Write(b.Index);
                writer.Write(c.Index);
            }

            public short InverseBytes(short v)
            {
                var bytes = BitConverter.GetBytes((short)v);
                Array.Reverse(bytes); 
                return BitConverter.ToInt16(bytes, 0);
            }

            public void WriteBatch(BinaryWriter writer, int baseTndex, bool bigEndain)
            {
                if (bigEndain)
                {
                    writer.Write(InverseBytes((short)(baseTndex + a.Index)));
                    writer.Write(InverseBytes((short)(baseTndex + b.Index)));
                    writer.Write(InverseBytes((short)(baseTndex + c.Index)));
                }
                else
                {
                    writer.Write((short)(baseTndex + a.Index));
                    writer.Write((short)(baseTndex + b.Index));
                    writer.Write((short)(baseTndex + c.Index));
                }
            }

            public static Triangle FromString(string s, Dictionary<string, CompoundVertex> compoundVertexes)
            {
                var split = s.Split(' ');
                return new Triangle
                {
                    a = CompoundVertex.FromString(split[0], compoundVertexes),
                    b = CompoundVertex.FromString(split[1], compoundVertexes),
                    c = CompoundVertex.FromString(split[2], compoundVertexes)
                };
            }
        }

        private class Mesh
        {
            public float X;
            public float Y;
            public float Z;
            public List<Triangle> Triangles = new List<Triangle>();
            public List<CompoundVertex> Compounds = new List<CompoundVertex>();

            public Mesh(float x, float y, float z, List<CompoundVertex> compounds, List<Triangle> triangles)
            {
                X = x;
                Y = y;
                Z = z;
                Compounds = compounds;
                Triangles = triangles;
            }
        }

        private class MeshBatch
        {
            public List<Mesh> Meshes = new List<Mesh>();
            public MeshBatch(Mesh[] skins)
            {
                for (var i = 0; i < skins.Length; i++)
                {
                    var skinIndex = i;
                    var skin = skins[skinIndex];
                    Meshes.Add(skin);
                }
            }

            public void WriteVertices(BinaryWriter writer, bool bigEndain)
            {
                for (var i = 0; i < Meshes.Count; i++)
                {
                    var mesh = Meshes[i];
                    foreach (var vertex in mesh.Compounds)
                        vertex.WriteBatch(writer, i, bigEndain);
                }
            }

            public void WriteIndices(BinaryWriter writer, bool bigEndain)
            {
                int baseIndex = 0;
                for (var i = 0; i < Meshes.Count; i++)
                {
                    var mesh = Meshes[i];
                    foreach (var triangle in mesh.Triangles)
                        triangle.WriteBatch(writer, baseIndex, bigEndain);
                    baseIndex += mesh.Compounds.Count;
                }
            }

            public void WriteTransforms(BinaryWriter writer, bool bigEndain)
            {
                for (var i = 0; i < Meshes.Count; i++)
                {
                    var mesh = Meshes[i];
                    if (bigEndain)
                    {
                        writer.Write(CompoundVertex.InverseBytes(mesh.X));
                        writer.Write(CompoundVertex.InverseBytes(mesh.Y));
                        writer.Write(CompoundVertex.InverseBytes(mesh.Z));
                    }
                    else
                    {
                        writer.Write(mesh.X);
                        writer.Write(mesh.Y);
                        writer.Write(mesh.Z);
                    }
                }
            }
        }

        private class ConvertContext
        {
            public List<Vertex> vertices = new List<Vertex>();
            public List<Vertex> textures = new List<Vertex>();
            public List<Vertex> normals = new List<Vertex>();
            public List<Triangle> triangles = new List<Triangle>();
            public Dictionary<string, CompoundVertex> compounds = new Dictionary<string, CompoundVertex>();
        }

        private List<Mesh> ConvertObjToBinary(StreamReader stringReader, string tempPath, string name)
        {
            List<Mesh> result = new List<Mesh>();
            List<ConvertContext> contexts = new List<ConvertContext>();

            var context = new ConvertContext();
            //contexts.Add(context);
            
            //var vertices = new List<Vertex>();
            //var textures = new List<Vertex>();
            //var normals = new List<Vertex>();
            //var triangles = new List<Triangle>();

            //var compounds = new Dictionary<string, CompoundVertex>();

            while (!stringReader.EndOfStream)
            {
                var line = stringReader.ReadLine();
                if (line.StartsWith("# object"))
                {
                    context = new ConvertContext();
                    contexts.Add(context);
                }
                else if (line.StartsWith("vt"))
                {
                    /// read vertex texture position
                    line = line.Remove(0, 2).Trim();
                    var v = Vertex.FromString(line);
                    context.textures.Add(v);
                }
                else if (line.StartsWith("vn"))
                { 
                    /// read vertex normal direction
                    line = line.Remove(0, 2).Trim();
                    var v = Vertex.FromString(line);
                    context.normals.Add(v);
                }
                else if (line.StartsWith("v"))
                {
                    /// read vertex position
                    line = line.Remove(0, 1).Trim();
                    var v = Vertex.FromString(line);
                    context.vertices.Add(v);
                }
                else if (line.StartsWith("f"))
                {
                    /// read triangle
                    line = line.Remove(0, 1).Trim();
                    var triangle = Triangle.FromString(line, context.compounds);
                    context.triangles.Add(triangle);
                }
            }
            var offsetV = 0;
            var newOffsetV = 0;
            var offsetVT = 0;
            var newOffsetVT = 0;
            var offsetVN = 0;
            var newOffsetVN = 0;
            foreach (var c in contexts)
            {
                var i = 0;
                var compoundsList = new List<CompoundVertex>();
                foreach (var vertex in c.compounds)
                {
                    if (vertex.Value.v > newOffsetV)
                        newOffsetV = vertex.Value.v;
                    if (vertex.Value.vt > newOffsetVT)
                        newOffsetVT = vertex.Value.vt;
                    if (vertex.Value.vn > newOffsetVN)
                        newOffsetVN = vertex.Value.vn;
                    vertex.Value.Position = c.vertices[vertex.Value.v - 1 - offsetV];
                    vertex.Value.Texture = c.textures[vertex.Value.vt - 1 - offsetVT];
                    vertex.Value.Normal = c.normals[vertex.Value.vn - 1 - offsetVN];
                    vertex.Value.Index = i;
                    i++;
                    compoundsList.Add(vertex.Value);
                }

                offsetV = newOffsetV;
                offsetVT = newOffsetVT;
                offsetVN = newOffsetVN;

                float x = 0, y = 0, z = 0;
                foreach (var v in compoundsList)
                {
                    x += v.Position.x;
                    y += v.Position.y;
                    z += v.Position.z;
                }
                if (c.compounds.Count > 0)
                {
                    x /= c.compounds.Count;
                    y /= c.compounds.Count;
                    z /= c.compounds.Count;
                }
                foreach (var v in compoundsList)
                {
                    v.Position -= new Vertex(x, y, z);
                }

                foreach (var t in c.triangles)
                    t.Build();
                foreach (var v in c.compounds)
                {
                    v.Value.Build();
                }

                //using (var stream = File.Create(Path.Combine(tempPath, name + "_h.3d")))
                //using (var writer = new BinaryWriter(stream))
                //{
                //    writer.Write((int)c.compounds.Count);
                //    writer.Write((int)c.triangles.Count);
                //}

                //using (var stream = File.Create(Path.Combine(tempPath, name + "_vb.3d")))
                //using (var writer = new BinaryWriter(stream))
                //{
                //    foreach (var vertex in c.compounds)
                //        vertex.Value.Write(writer);
                //}

                //using (var stream = File.Create(Path.Combine(tempPath, name + "_ib.3d")))
                //using (var writer = new BinaryWriter(stream))
                //{
                //    foreach (var triangle in c.triangles)
                //        triangle.Write(writer);
                //}

                //using (var stream = File.Create(Path.Combine(tempPath, name + ".3d")))
                //using (var writer = new BinaryWriter(stream))
                //{
                //    writer.Write((int)c.compounds.Count);
                //    foreach (var vertex in c.compounds)
                //        vertex.Value.Write(writer);
                //    writer.Write((int)c.triangles.Count);
                //    foreach (var triangle in c.triangles)
                //        triangle.Write(writer);
                //}
                var mesh = new Mesh(x, y, z, compoundsList, c.triangles);
                result.Add(mesh);
            }
            return result;
        }

        private void Generate() 
        {
            var tempPath = GetTempPath();
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            var tempFiles = Directory.GetFiles(tempPath);
            foreach (var file in tempFiles) 
                File.Delete(file);

            var filesPng = Directory.GetFiles(_pngSource.Text, "*.png");
            var filesJpeg = Directory.GetFiles(_pngSource.Text, "*.jpeg");
            var filesJpg = Directory.GetFiles(_pngSource.Text, "*.jpg");
            var filesList = new List<string>();
            filesList.AddRange(filesPng);
            filesList.AddRange(filesJpeg);
            filesList.AddRange(filesJpg);
            var files = filesList.ToArray();
            foreach (var file in files) 
            {
                //var copy = Path.Combine(tempPath, Path.GetFileName(file));

                var name = Path.GetFileNameWithoutExtension(file);
                
                if (name.EndsWith("bump")) 
                {
                    using (var source = new Bitmap(file))
                    using (var bitmap = CreateNormalMap(source))
                        SaveMipmap(bitmap, tempPath, name);
                }
                else
                {
                    using (var bitmap = new Bitmap(file))
                        SaveMipmap(bitmap, tempPath, name);
                }
            }
            files = Directory.GetFiles(_pngSource.Text, "*.obj");
            var meshes = new List<Mesh>();
            foreach (var file in files) 
            {
                var name = Path.GetFileNameWithoutExtension(file);
                using (var stringReader = new StreamReader(file))
                {
                    var localMeshes = ConvertObjToBinary(stringReader, tempPath, name);
                    foreach (var localMesh in localMeshes)
                        meshes.Add(localMesh);
                }
            }

            var meshBatch = new MeshBatch(meshes.ToArray());
            using (var stream = File.Create(Path.Combine(tempPath, "batch_vb_big.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteVertices(writer, true);
            using (var stream = File.Create(Path.Combine(tempPath, "batch_vb_lit.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteVertices(writer, false);
            
            using (var stream = File.Create(Path.Combine(tempPath, "batch_ib_big.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteIndices(writer, true);
            using (var stream = File.Create(Path.Combine(tempPath, "batch_ib_lit.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteIndices(writer, false);

            using (var stream = File.Create(Path.Combine(tempPath, "batch_transforms_big.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteTransforms(writer, true);
            using (var stream = File.Create(Path.Combine(tempPath, "batch_transforms_lit.3d")))
            using (var writer = new BinaryWriter(stream))
                meshBatch.WriteTransforms(writer, false);

            files = Directory.GetFiles(tempPath, "*.png");
            foreach (var file in files)
            {
                if (!ExecuteCommand(_etc1Tool.Text, "\"" + file + "\"")) 
                    return;
                Application.DoEvents();
            }

            files = Directory.GetFiles(_targetFolder.Text, "*.pkm");
            foreach (var file in files)
            {
                File.Delete(file);
                Application.DoEvents();
            }

            files = Directory.GetFiles(_targetFolder.Text, "*.3d");
            foreach (var file in files)
            {
                File.Delete(file);
                Application.DoEvents();
            }

            files = Directory.GetFiles(tempPath, "*.pkm");
            foreach (var file in files)
            {
                var copy = Path.Combine(_targetFolder.Text, Path.GetFileName(file));
                File.Copy(file, copy, true);
                Application.DoEvents();
            }
            
            files = Directory.GetFiles(tempPath, "*.3d");
            foreach (var file in files)
            {
                var copy = Path.Combine(_targetFolder.Text, Path.GetFileName(file));
                File.Copy(file, copy, true);
                Application.DoEvents();
            }
        }

        private bool ExecuteCommand(string command, string parameters) 
        {
            try
            {
                string workDir = Path.GetDirectoryName(command);
                string fileCmd = Path.GetFileName(command);
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + fileCmd + " " + parameters);
                procStartInfo.WorkingDirectory = workDir;
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();
                _status.Text = command + " " + parameters;
                return true;
            }
            catch (Exception objException)
            {
                _status.Text = objException.Message;
                return false;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            _pngSource.Text = Properties.Settings.Default.SourcePath;
            _etc1Tool.Text = Properties.Settings.Default.Etc1Tool;
            _targetFolder.Text = Properties.Settings.Default.TergetPath;
        }

        private void _generate_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SourcePath = _pngSource.Text;
            Properties.Settings.Default.Etc1Tool = _etc1Tool.Text;
            Properties.Settings.Default.TergetPath = _targetFolder.Text;
            Properties.Settings.Default.Save();
            _generate.Enabled = false;
            Generate();
            _generate.Enabled = true;
        }

        private void _browsePngSource_Click(object sender, EventArgs e)
        {
            if (_bowsePngSourceDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            _pngSource.Text = _bowsePngSourceDialog.SelectedPath;
        }

        private void _browseTargetFolder_Click(object sender, EventArgs e)
        {
            if (_bowseTargetFolderDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            _targetFolder.Text = _bowseTargetFolderDialog.SelectedPath;
        }

        private void _browseEtc1Tool_Click(object sender, EventArgs e)
        {
            if (_openEtc1ToolDialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK) return;
            _etc1Tool.Text = _openEtc1ToolDialog.FileName;
        }
    }
}
