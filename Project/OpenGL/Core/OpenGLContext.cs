using System;
using System.Collections.Generic;
using System.Linq;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.OpenGL
{
    public partial class OpenGLContext
    {
        // ***************************************************************************************************
        // 15.05.19 : Création, importation des fonctions d'opengl32.dll qui nous sont utiles
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        internal void Begin(BeginMode mode)
        {
            OpenGl32.glBegin((uint)mode);
        }

        internal void BindTexture(BindTextureTarget target, uint texture)
        {
            OpenGl32.glBindTexture((uint)target, texture);
        }

        internal void BlendFunc(BlendingSourceFactor sourceFactor, BlendingDestinationFactor destinationFactor)
        {
            OpenGl32.glBlendFunc((uint)sourceFactor, (uint)destinationFactor);
        }

        internal void CallLists(DataType dataType, int cnt, IEnumerable<byte> lists)
        {
            OpenGl32.glCallLists(cnt, (uint)dataType, [.. lists]);
        }

        internal void Clear(uint mask)
        {
            OpenGl32.glClear(mask);
        }

        internal void ClearColor(float red, float green, float blue, float alpha)
        {
            OpenGl32.glClearColor(red, green, blue, alpha);
        }

        internal void ClearDepth(double depth)
        {
            OpenGl32.glClearDepth(depth);
        }

        internal void Color(float red, float green, float blue)
        {
            OpenGl32.glColor3f(red, green, blue);
        }

        internal void ColorPointer(int size, PixelType type, int stride, float[] pointer)
        {
            OpenGl32.glColorPointer(size, (uint)type, stride, pointer);
        }

        internal void Cylinder(IntPtr qobj, double baseRadius, double topRadius, double height, int slices, int stacks)
        {
            Glu32.gluCylinder(qobj, baseRadius, topRadius, height, slices, stacks);
        }

        internal void DeleteQuadric(IntPtr quadric)
        {
            Glu32.gluDeleteQuadric(quadric);
        }

        internal void DeleteTexture(uint texture)
        {
            DeleteTextures([texture]);
        }

        internal void DeleteTextures(IEnumerable<uint> textures)
        {
            try
            {
                OpenGl32.glDeleteTextures(textures.Count(), [.. textures]);
            }
            catch (Exception ex)
            {
                throw new AccessViolationException("Impossible to delete openGL texture. Perhaps an object miss an explicit Dispose call.", ex);
            }
        }

        internal void DepthFunc(DepthFunction function)
        {
            OpenGl32.glDepthFunc((uint)function);
        }

        internal void Disable(EnableTarget cap)
        {
            OpenGl32.glDisable((uint)cap);
        }

        internal void DisableClientState(EnableClientTarget array)
        {
            OpenGl32.glDisableClientState((uint)array);
        }

        internal void Disk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops)
        {
            Glu32.gluDisk(qobj, innerRadius, outerRadius, slices, loops);
        }

        internal void DrawElements(DrawElementsMode mode, int count, uint[] indices)
        {
            OpenGl32.glDrawElements((uint)mode, count, (uint)DataType.UnsignedInt, indices);
        }

        internal void Enable(EnableTarget cap)
        {
            OpenGl32.glEnable((uint)cap);
        }

        internal void EnableClientState(EnableClientTarget array)
        {
            OpenGl32.glEnableClientState((uint)array);
        }

        internal void End()
        {
            OpenGl32.glEnd();
        }

        internal void Flush()
        {
            OpenGl32.glFlush();
        }

        internal uint GenLists(int range)
        {
            return OpenGl32.glGenLists(range);
        }

        internal uint GenTexture()
        {
            return GenTextures(1)[0];
        }

        internal uint[] GenTextures(int n)
        {
            uint[] textures = new uint[n];
            OpenGl32.glGenTextures(n, textures);

            return textures;
        }

        internal void GetDouble(GetTarget pname, double[] parameters)
        {
            OpenGl32.glGetDoublev((uint)pname, parameters);
        }

        internal ErrorCode GetErrorCode()
        {
            return (ErrorCode)OpenGl32.glGetError();
        }

        internal float GetFloatValue(GetTarget pname)
        {
            float[] v = [0];
            OpenGl32.glGetFloatv((uint)pname, v);

            return v[0];
        }

        internal void GetFloatValues(GetTarget pname, float[] parameters)
        {
            OpenGl32.glGetFloatv((uint)pname, parameters);
        }

        internal int GetIntegerValue(GetTarget pname)
        {
            int[] v = [0];
            OpenGl32.glGetIntegerv((uint)pname, v);

            return v[0];
        }

        internal void GetIntegerValues(GetTarget pname, int[] parameters)
        {
            OpenGl32.glGetIntegerv((uint)pname, parameters);
        }

        internal string GetString(StringTarget pname)
        {
            return new string(MarshShop.PointerToStringAnsi(OpenGl32.glGetString((uint)pname)).ToCharArray());
        }

        internal void Hint(HintTarget target, HintMode mode)
        {
            OpenGl32.glHint((uint)target, (uint)mode);
        }

        internal void Light(LightName light, LightParameter pname, float param)
        {
            OpenGl32.glLightf((uint)light, (uint)pname, param);
        }

        internal void Light(LightName light, LightParameter pname, float[] parameters)
        {
            OpenGl32.glLightfv((uint)light, (uint)pname, parameters);
        }

        internal void LineWidth(float width)
        {
            OpenGl32.glLineWidth(width);
        }

        internal void ListBase(uint listbase)
        {
            OpenGl32.glListBase(listbase);
        }

        internal void LoadIdentity()
        {
            OpenGl32.glLoadIdentity();
        }

        internal void LoadMatrixf(float[] m)
        {
            OpenGl32.glLoadMatrixf(m);
        }

        internal void LookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz)
        {
            Glu32.gluLookAt(eyex, eyey, eyez, centerx, centery, centerz, upx, upy, upz);
        }

        internal void Material(FaceMode face, MaterialParameter pname, float param)
        {
            OpenGl32.glMaterialf((uint)face, (uint)pname, param);
        }

        internal void Material(FaceMode face, MaterialParameter pname, float[] parameters)
        {
            OpenGl32.glMaterialfv((uint)face, (uint)pname, parameters);
        }

        internal void MaterialGlobal(HColor col, float ambient, float diffuse, float emission, float specular, float specularIntensity)
        {
            float r = col.Red / 255.0f;
            float g = col.Green / 255.0f;
            float b = col.Blue / 255.0f;
            float a = col.Alpha / 255.0f;

            OpenGl32.glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Ambient, [r * ambient, g * ambient, b * ambient, a]);
            OpenGl32.glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Diffuse, [r * diffuse, g * diffuse, b * diffuse, a]);
            OpenGl32.glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Emission, [r * emission, g * emission, b * emission, a]);
            OpenGl32.glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Specular, [specular, specular, specular, a]); // Je veux toujours un reflet spéculaire de la couleur de la lumière
            OpenGl32.glMaterialf((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Shininess, 100 - specularIntensity * 100);
        }

        internal void MatrixMode(MatrixMode mode)
        {
            OpenGl32.glMatrixMode((uint)mode);
        }

        internal IntPtr NewQuadric()
        {
            return Glu32.gluNewQuadric();
        }

        internal void Normal(float nx, float ny, float nz)
        {
            OpenGl32.glNormal3f(nx, ny, nz);
        }

        internal void Normal(Vect3D v)
        {
            OpenGl32.glNormal3f(v.X, v.Y, v.Z);
        }

        internal void NormalPointer(NormalType type, int stride, float[] pointer)
        {
            OpenGl32.glNormalPointer((uint)type, stride, pointer);
        }

        internal void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            OpenGl32.glOrtho(left, right, bottom, top, zNear, zFar);
        }

        internal void PartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle)
        {
            Glu32.gluPartialDisk(qobj, innerRadius, outerRadius, slices, loops, startAngle, sweepAngle);
        }

        internal void Perspective(double fovyDegrees, double aspect, double zNear, double zFar)
        {
            Glu32.gluPerspective(fovyDegrees, aspect, zNear, zFar);
        }

        internal void PopAttrib()
        {
            OpenGl32.glPopAttrib();
        }

        internal void PopMatrix()
        {
            OpenGl32.glPopMatrix();
        }

        internal void PushAttrib(AttributeMask mask)
        {
            OpenGl32.glPushAttrib((uint)mask);
        }

        internal void PushMatrix()
        {
            OpenGl32.glPushMatrix();
        }

        internal void QuadricDrawStyle(IntPtr quadObject, QuadricDrawStyle drawStyle)
        {
            Glu32.gluQuadricDrawStyle(quadObject, (uint)drawStyle);
        }

        internal void QuadricNormals(IntPtr quadricObject, QuadricNormal normals)
        {
            Glu32.gluQuadricNormals(quadricObject, (uint)normals);
        }

        internal void QuadricOrientation(IntPtr quadricObject, QuadricOrientation orientation)
        {
            Glu32.gluQuadricOrientation(quadricObject, (int)orientation);
        }

        internal void QuadricTexture(IntPtr quadricObject, Bool textureCoords)
        {
            Glu32.gluQuadricTexture(quadricObject, (int)textureCoords);
        }

        internal void RasterPos(int x, int y)
        {
            OpenGl32.glRasterPos2i(x, y);
        }

        internal void ReadBuffer(ReadBufferMode mode)
        {
            OpenGl32.glReadBuffer((uint)mode);
        }

        internal void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, byte[] pixels)
        {
            OpenGl32.glReadPixels(x, y, width, height, (uint)format, (uint)type, pixels);
        }

        internal void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
        {
            OpenGl32.glReadPixels(x, y, width, height, (uint)format, (uint)type, pixels);
        }

        internal void Rotate(float angle, float axisX, float axisY, float axisZ)
        {
            OpenGl32.glRotatef(angle, axisX, axisY, axisZ);
        }

        internal void Rotate(float anglex, float angley, float anglez)
        {
            OpenGl32.glRotatef(anglex, 1, 0, 0);
            OpenGl32.glRotatef(angley, 0, 1, 0);
            OpenGl32.glRotatef(anglez, 0, 0, 1);
        }

        internal void Scale(float x, float y, float z)
        {
            OpenGl32.glScalef(x, y, z);
        }

        internal void ShadeModel(ShadeModel mode)
        {
            OpenGl32.glShadeModel((uint)mode);
        }

        internal void Sphere(IntPtr qobj, double radius, int slices, int stacks)
        {
            Glu32.gluSphere(qobj, radius, slices, stacks);
        }

        internal void TexCoord(float s, float t)
        {
            OpenGl32.glTexCoord2f(s, t);
        }

        internal void TexCoordPointer(int size, TexCoordType type, int stride, float[] pointer)
        {
            OpenGl32.glTexCoordPointer(size, (uint)type, stride, pointer);
        }

        internal void TexImage2D(TextureImageTarget target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels)
        {
            OpenGl32.glTexImage2D((uint)target, level, internalformat, width, height, border, format, type, pixels);
        }

        internal void TexImage2D(TextureImageTarget target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels)
        {
            OpenGl32.glTexImage2D((uint)target, level, internalformat, width, height, border, format, type, pixels);
        }

        internal void TexParameter(TextureTarget target, TextureParameter pname, float param)
        {
            OpenGl32.glTexParameterf((uint)target, (uint)pname, param);
        }

        internal void TexParameter(TextureTarget target, TextureParameter pname, float[] parameters)
        {
            OpenGl32.glTexParameterfv((uint)target, (uint)pname, parameters);
        }

        internal void Translate(float x, float y, float z)
        {
            OpenGl32.glTranslatef(x, y, z);
        }

        internal void Translate(Vect3D v)
        {
            OpenGl32.glTranslatef(v.X, v.Y, v.Z);
        }

        internal double[] UnProject(double winx, double winy, double winz)
        {
            double[] modelView = new double[16];
            double[] projection = new double[16];
            int[] viewport = new int[4];

            GetDouble(GetTarget.ModelviewMatix, modelView);
            GetDouble(GetTarget.ProjectionMatrix, projection);
            GetIntegerValues(GetTarget.Viewport, viewport);

            double[] result = new double[3];
            Glu32.gluUnProject(winx, winy, winz, modelView, projection, viewport, ref result[0], ref result[1], ref result[2]);

            return result;
        }

        internal void Vertex(float x, float y, float z)
        {
            OpenGl32.glVertex3f(x, y, z);
        }

        internal void Vertex(Point3D pt)
        {
            OpenGl32.glVertex3f(pt.X, pt.Y, pt.Z);
        }

        internal void VertexPointer(int size, int stride, float[] pointer)
        {
            OpenGl32.glVertexPointer(size, (uint)DataType.Float, stride, pointer);
        }

        internal void Vertices(params Point3D[] pts)
        {
            pts.ToList().ForEach(o => OpenGl32.glVertex3f(o.X, o.Y, o.Z));
        }

        internal void Vertices(IEnumerable<Point3D> pts)
        {
            pts.ToList().ForEach(o => OpenGl32.glVertex3f(o.X, o.Y, o.Z));
        }

        internal void Viewport(int x, int y, int width, int height)
        {
            OpenGl32.glViewport(x, y, width, height);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}