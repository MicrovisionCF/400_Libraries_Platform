using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Microvision.Graphic;
using Microvision.Types;

namespace Microvision.OpenGL
{
    public partial class OpenGLContext
    {
        // ***************************************************************************************************
        // 15.05.19 : Création, importation des fonctions d'opengl32.dll qui nous sont utiles
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        [DllImport("Glu32.dll")] private static extern IntPtr gluNewNurbsRenderer();
        [DllImport("Glu32.dll")] private static extern IntPtr gluNewQuadric();
        [DllImport("Glu32.dll")] private static extern IntPtr gluNewTess();
        [DllImport("Glu32.dll")] private static extern void gluBeginCurve(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluBeginSurface(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluBeginTrim(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluBuild1DMipmaps(uint target, uint components, int width, uint format, uint type, IntPtr data);
        [DllImport("Glu32.dll")] private static extern void gluBuild2DMipmaps(uint target, uint components, int width, int height, uint format, uint type, IntPtr data);
        [DllImport("Glu32.dll")] private static extern void gluCylinder(IntPtr qobj, double baseRadius, double topRadius, double height, int slices, int stacks);
        [DllImport("Glu32.dll")] private static extern void gluDeleteNurbsRenderer(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluDeleteQuadric(IntPtr state);
        [DllImport("Glu32.dll")] private static extern void gluDeleteTess(IntPtr tess);
        [DllImport("Glu32.dll")] private static extern void gluDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops);
        [DllImport("Glu32.dll")] private static extern void gluEndCurve(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluEndSurface(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluEndTrim(IntPtr nobj);
        [DllImport("Glu32.dll")] private static extern void gluGetNurbsProperty(IntPtr nobj, int property, float value);
        [DllImport("Glu32.dll")] private static extern void gluGetTessProperty(IntPtr tess, int which, double value);
        [DllImport("Glu32.dll")] private static extern void gluLoadSamplingMatrices(IntPtr nobj, float[] modelMatrix, float[] projMatrix, int[] viewport);
        [DllImport("Glu32.dll")] private static extern void gluLookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz);
        [DllImport("Glu32.dll")] private static extern void gluNurbsCurve(IntPtr nobj, int nknots, float[] knot, int stride, float[] ctlarray, int order, uint type);
        [DllImport("Glu32.dll")] private static extern void gluNurbsProperty(IntPtr nobj, int property, float value);
        [DllImport("Glu32.dll")] private static extern void gluNurbsSurface(IntPtr nobj, int sknot_count, float[] sknot, int tknot_count, float[] tknot, int s_stride, int t_stride, float[] ctlarray, int sorder, int torder, uint type);
        [DllImport("Glu32.dll")] private static extern void gluOrtho2D(double left, double right, double bottom, double top);
        [DllImport("Glu32.dll")] private static extern void gluPartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle);
        [DllImport("Glu32.dll")] private static extern void gluPerspective(double fovy, double aspect, double zNear, double zFar);
        [DllImport("Glu32.dll")] private static extern void gluPickMatrix(double x, double y, double width, double height, int[] viewport);
        [DllImport("Glu32.dll")] private static extern void gluProject(double objx, double objy, double objz, double[] modelMatrix, double[] projMatrix, int[] viewport, double[] winx, double[] winy, double[] winz);
        [DllImport("Glu32.dll")] private static extern void gluPwlCurve(IntPtr nobj, int count, float array, int stride, uint type);
        [DllImport("Glu32.dll")] private static extern void gluQuadricDrawStyle(IntPtr quadObject, uint drawStyle);
        [DllImport("Glu32.dll")] private static extern void gluQuadricNormals(IntPtr quadObject, uint normals);
        [DllImport("Glu32.dll")] private static extern void gluQuadricOrientation(IntPtr quadObject, int orientation);
        [DllImport("Glu32.dll")] private static extern void gluQuadricTexture(IntPtr quadObject, int textureCoords);
        [DllImport("Glu32.dll")] private static extern void gluScaleImage(int format, int widthin, int heightin, int typein, int[] datain, int widthout, int heightout, int typeout, int[] dataout);
        [DllImport("Glu32.dll")] private static extern void gluSphere(IntPtr qobj, double radius, int slices, int stacks);
        [DllImport("Glu32.dll")] private static extern void gluTessBeginContour(IntPtr tess);
        [DllImport("Glu32.dll")] private static extern void gluTessBeginPolygon(IntPtr tess, IntPtr polygonData);
        [DllImport("Glu32.dll")] private static extern void gluTessEndContour(IntPtr tess);
        [DllImport("Glu32.dll")] private static extern void gluTessEndPolygon(IntPtr tess);
        [DllImport("Glu32.dll")] private static extern void gluTessNormal(IntPtr tess, double x, double y, double z);
        [DllImport("Glu32.dll")] private static extern void gluTessProperty(IntPtr tess, int which, double value);
        [DllImport("Glu32.dll")] private static extern void gluTessVertex(IntPtr tess, double[] coords, double[] data);
        [DllImport("Glu32.dll")] private static extern void gluUnProject(double winx, double winy, double winz, double[] modelMatrix, double[] projMatrix, int[] viewport, ref double objx, ref double objy, ref double objz);
        [DllImport("opengl32.dll")] private static extern IntPtr glGetString(uint name);
        [DllImport("opengl32.dll")] private static extern uint glGenLists(int range);
        [DllImport("opengl32.dll")] private static extern uint glGetError();
        [DllImport("opengl32.dll")] private static extern void glBegin(uint mode);
        [DllImport("opengl32.dll")] private static extern void glBindTexture(uint target, uint texture);
        [DllImport("opengl32.dll")] private static extern void glBlendFunc(uint sfactor, uint dfactor);
        [DllImport("opengl32.dll")] private static extern void glCallLists(int n, uint type, byte[] lists);
        [DllImport("opengl32.dll")] private static extern void glClear(uint mask);
        [DllImport("opengl32.dll")] private static extern void glClearColor(float red, float green, float blue, float alpha);
        [DllImport("opengl32.dll")] private static extern void glClearDepth(double depth);
        [DllImport("opengl32.dll")] private static extern void glColor3f(float red, float green, float blue);
        [DllImport("opengl32.dll")] private static extern void glColorPointer(int size, uint type, int stride, float[] pointer);
        [DllImport("opengl32.dll")] private static extern void glDeleteTextures(int n, uint[] textures);
        [DllImport("opengl32.dll")] private static extern void glDepthFunc(uint func);
        [DllImport("opengl32.dll")] private static extern void glDisable(uint cap);
        [DllImport("opengl32.dll")] private static extern void glDisableClientState(uint array);
        [DllImport("opengl32.dll")] private static extern void glDrawElements(uint mode, int count, uint type, uint[] indices);
        [DllImport("opengl32.dll")] private static extern void glEnable(uint cap);
        [DllImport("opengl32.dll")] private static extern void glEnableClientState(uint array);
        [DllImport("opengl32.dll")] private static extern void glEnd();
        [DllImport("opengl32.dll")] private static extern void glFlush();
        [DllImport("opengl32.dll")] private static extern void glGenTextures(int n, uint[] textures);
        [DllImport("opengl32.dll")] private static extern void glGetDoublev(uint pname, double[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glGetFloatv(uint pname, float[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glGetIntegerv(uint pname, int[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glHint(uint target, uint mode);
        [DllImport("opengl32.dll")] private static extern void glLightf(uint light, uint pname, float param);
        [DllImport("opengl32.dll")] private static extern void glLightfv(uint light, uint pname, float[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glLineStipple(int factor, ushort pattern);
        [DllImport("opengl32.dll")] private static extern void glLineWidth(float width);
        [DllImport("opengl32.dll")] private static extern void glListBase(uint base_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glLoadIdentity();
        [DllImport("opengl32.dll")] private static extern void glLoadMatrixf(float[] m);
        [DllImport("opengl32.dll")] private static extern void glMaterialf(uint face, uint pname, float param);
        [DllImport("opengl32.dll")] private static extern void glMaterialfv(uint face, uint pname, float[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glMatrixMode(uint mode);
        [DllImport("opengl32.dll")] private static extern void glNormal3f(float nx, float ny, float nz);
        [DllImport("opengl32.dll")] private static extern void glNormalPointer(uint type, int stride, float[] pointer);
        [DllImport("opengl32.dll")] private static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);
        [DllImport("opengl32.dll")] private static extern void glPopAttrib();
        [DllImport("opengl32.dll")] private static extern void glPopMatrix();
        [DllImport("opengl32.dll")] private static extern void glPushAttrib(uint mask);
        [DllImport("opengl32.dll")] private static extern void glPushMatrix();
        [DllImport("opengl32.dll")] private static extern void glRasterPos2i(int x, int y);
        [DllImport("opengl32.dll")] private static extern void glReadBuffer(uint mode);
        [DllImport("opengl32.dll")] private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);
        [DllImport("opengl32.dll")] private static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, IntPtr pixels);
        [DllImport("opengl32.dll")] private static extern void glRotatef(float angle, float x, float y, float z);
        [DllImport("opengl32.dll")] private static extern void glScalef(float x, float y, float z);
        [DllImport("opengl32.dll")] private static extern void glShadeModel(uint mode);
        [DllImport("opengl32.dll")] private static extern void glTexCoord2f(float s, float t);
        [DllImport("opengl32.dll")] private static extern void glTexCoordPointer(int size, uint type, int stride, float[] pointer);
        [DllImport("opengl32.dll")] private static extern void glTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels);
        [DllImport("opengl32.dll")] private static extern void glTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);
        [DllImport("opengl32.dll")] private static extern void glTexParameterf(uint target, uint pname, float param);
        [DllImport("opengl32.dll")] private static extern void glTexParameterfv(uint target, uint pname, float[] params_notkeyword);
        [DllImport("opengl32.dll")] private static extern void glTranslatef(float x, float y, float z);
        [DllImport("opengl32.dll")] private static extern void glVertex3f(float x, float y, float z);
        [DllImport("opengl32.dll")] private static extern void glVertexPointer(int size, uint type, int stride, float[] pointer);
        [DllImport("opengl32.dll")] private static extern void glViewport(int x, int y, int width, int height);


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
            glBegin((uint)mode);
        }

        internal void BindTexture(BindTextureTarget target, uint texture)
        {
            glBindTexture((uint)target, texture);
        }

        internal void BlendFunc(BlendingSourceFactor sourceFactor, BlendingDestinationFactor destinationFactor)
        {
            glBlendFunc((uint)sourceFactor, (uint)destinationFactor);
        }

        internal void CallLists(DataType dataType, int cnt, IEnumerable<byte> lists)
        {
            glCallLists(cnt, (uint)dataType, lists.ToArray());
        }

        internal void Clear(uint mask)
        {
            glClear(mask);
        }

        internal void ClearColor(float red, float green, float blue, float alpha)
        {
            glClearColor(red, green, blue, alpha);
        }

        internal void ClearDepth(double depth)
        {
            glClearDepth(depth);
        }

        internal void Color(float red, float green, float blue)
        {
            glColor3f(red, green, blue);
        }

        internal void ColorPointer(int size, PixelType type, int stride, float[] pointer)
        {
            glColorPointer(size, (uint)type, stride, pointer);
        }

        internal void Cylinder(IntPtr qobj, double baseRadius, double topRadius, double height, int slices, int stacks)
        {
            gluCylinder(qobj, baseRadius, topRadius, height, slices, stacks);
        }

        internal void DeleteQuadric(IntPtr quadric)
        {
            gluDeleteQuadric(quadric);
        }

        internal void DeleteTexture(uint texture)
        {
            DeleteTextures(new[] { texture });
        }

        internal void DeleteTextures(IEnumerable<uint> textures)
        {
            try
            {
                glDeleteTextures(textures.Count(), textures.ToArray());
            }
            catch (Exception ex)
            {
                throw new AccessViolationException("Impossible to delete openGL texture. Perhaps an object miss an explicit Dispose call.", ex);
            }
        }

        internal void DepthFunc(DepthFunction function)
        {
            glDepthFunc((uint)function);
        }

        internal void Disable(EnableTarget cap)
        {
            glDisable((uint)cap);
        }

        internal void DisableClientState(EnableClientTarget array)
        {
            glDisableClientState((uint)array);
        }

        internal void Disk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops)
        {
            gluDisk(qobj, innerRadius, outerRadius, slices, loops);
        }

        internal void DrawElements(DrawElementsMode mode, int count, uint[] indices)
        {
            glDrawElements((uint)mode, count, (uint)DataType.UnsignedInt, indices);
        }

        internal void Enable(EnableTarget cap)
        {
            glEnable((uint)cap);
        }

        internal void EnableClientState(EnableClientTarget array)
        {
            glEnableClientState((uint)array);
        }

        internal void End()
        {
            glEnd();
        }

        internal void Flush()
        {
            glFlush();
        }

        internal uint GenLists(int range)
        {
            return glGenLists(range);
        }

        internal uint GenTexture()
        {
            return GenTextures(1)[0];
        }

        internal uint[] GenTextures(int n)
        {
            uint[] textures = new uint[n];
            glGenTextures(n, textures);

            return textures;
        }

        internal void GetDouble(GetTarget pname, double[] parameters)
        {
            glGetDoublev((uint)pname, parameters);
        }

        internal ErrorCode GetErrorCode()
        {
            return (ErrorCode)glGetError();
        }

        internal float GetFloatValue(GetTarget pname)
        {
            float[] v = new float[] { 0 };
            glGetFloatv((uint)pname, v);

            return v[0];
        }

        internal void GetFloatValues(GetTarget pname, float[] parameters)
        {
            glGetFloatv((uint)pname, parameters);
        }

        internal int GetIntegerValue(GetTarget pname)
        {
            int[] v = new[] { 0 };
            glGetIntegerv((uint)pname, v);

            return v[0];
        }

        internal void GetIntegerValues(GetTarget pname, int[] parameters)
        {
            glGetIntegerv((uint)pname, parameters);
        }

        internal string GetString(StringTarget pname)
        {
            return new string(MarshShop.PointerToStringAnsi(glGetString((uint)pname)).ToCharArray());
        }

        internal void Hint(HintTarget target, HintMode mode)
        {
            glHint((uint)target, (uint)mode);
        }

        internal void Light(LightName light, LightParameter pname, float param)
        {
            glLightf((uint)light, (uint)pname, param);
        }

        internal void Light(LightName light, LightParameter pname, float[] parameters)
        {
            glLightfv((uint)light, (uint)pname, parameters);
        }

        internal void LineWidth(float width)
        {
            glLineWidth(width);
        }

        internal void ListBase(uint listbase)
        {
            glListBase(listbase);
        }

        internal void LoadIdentity()
        {
            glLoadIdentity();
        }

        internal void LoadMatrixf(float[] m)
        {
            glLoadMatrixf(m);
        }

        internal void LookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz)
        {
            gluLookAt(eyex, eyey, eyez, centerx, centery, centerz, upx, upy, upz);
        }

        internal void Material(FaceMode face, MaterialParameter pname, float param)
        {
            glMaterialf((uint)face, (uint)pname, param);
        }

        internal void Material(FaceMode face, MaterialParameter pname, float[] parameters)
        {
            glMaterialfv((uint)face, (uint)pname, parameters);
        }

        internal void MaterialGlobal(HColor col, float ambient, float diffuse, float emission, float specular, float specularIntensity)
        {
            float r = col.red / 255.0f;
            float g = col.green / 255.0f;
            float b = col.blue / 255.0f;
            float a = col.alpha / 255.0f;

            glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Ambient, new[] { r * ambient, g * ambient, b * ambient, a });
            glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Diffuse, new[] { r * diffuse, g * diffuse, b * diffuse, a });
            glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Emission, new[] { r * emission, g * emission, b * emission, a });
            glMaterialfv((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Specular, new[] { specular, specular, specular, a }); // Je veux toujours un reflet spéculaire de la couleur de la lumière
            glMaterialf((uint)FaceMode.FrontAndBack, (uint)MaterialParameter.Shininess, 100 - specularIntensity * 100);
        }

        internal void MatrixMode(MatrixMode mode)
        {
            glMatrixMode((uint)mode);
        }

        internal IntPtr NewQuadric()
        {
            return gluNewQuadric();
        }

        internal void Normal(float nx, float ny, float nz)
        {
            glNormal3f(nx, ny, nz);
        }

        internal void Normal(Vect3D v)
        {
            glNormal3f(v.x, v.y, v.z);
        }

        internal void NormalPointer(NormalType type, int stride, float[] pointer)
        {
            glNormalPointer((uint)type, stride, pointer);
        }

        internal void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            glOrtho(left, right, bottom, top, zNear, zFar);
        }

        internal void PartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle)
        {
            gluPartialDisk(qobj, innerRadius, outerRadius, slices, loops, startAngle, sweepAngle);
        }

        internal void Perspective(double fovyDegrees, double aspect, double zNear, double zFar)
        {
            gluPerspective(fovyDegrees, aspect, zNear, zFar);
        }

        internal void PopAttrib()
        {
            glPopAttrib();
        }

        internal void PopMatrix()
        {
            glPopMatrix();
        }

        internal void PushAttrib(AttributeMask mask)
        {
            glPushAttrib((uint)mask);
        }

        internal void PushMatrix()
        {
            glPushMatrix();
        }

        internal void QuadricDrawStyle(IntPtr quadObject, QuadricDrawStyle drawStyle)
        {
            gluQuadricDrawStyle(quadObject, (uint)drawStyle);
        }

        internal void QuadricNormals(IntPtr quadricObject, QuadricNormal normals)
        {
            gluQuadricNormals(quadricObject, (uint)normals);
        }

        internal void QuadricOrientation(IntPtr quadricObject, QuadricOrientation orientation)
        {
            gluQuadricOrientation(quadricObject, (int)orientation);
        }

        internal void QuadricTexture(IntPtr quadricObject, Bool textureCoords)
        {
            gluQuadricTexture(quadricObject, (int)textureCoords);
        }

        internal void RasterPos(int x, int y)
        {
            glRasterPos2i(x, y);
        }

        internal void ReadBuffer(ReadBufferMode mode)
        {
            glReadBuffer((uint)mode);
        }

        internal void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, byte[] pixels)
        {
            glReadPixels(x, y, width, height, (uint)format, (uint)type, pixels);
        }

        internal void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
        {
            glReadPixels(x, y, width, height, (uint)format, (uint)type, pixels);
        }

        internal void Rotate(float angle, float axisX, float axisY, float axisZ)
        {
            glRotatef(angle, axisX, axisY, axisZ);
        }

        internal void Rotate(float anglex, float angley, float anglez)
        {
            glRotatef(anglex, 1, 0, 0);
            glRotatef(angley, 0, 1, 0);
            glRotatef(anglez, 0, 0, 1);
        }

        internal void Scale(float x, float y, float z)
        {
            glScalef(x, y, z);
        }

        internal void ShadeModel(ShadeModel mode)
        {
            glShadeModel((uint)mode);
        }

        internal void Sphere(IntPtr qobj, double radius, int slices, int stacks)
        {
            gluSphere(qobj, radius, slices, stacks);
        }

        internal void TexCoord(float s, float t)
        {
            glTexCoord2f(s, t);
        }

        internal void TexCoordPointer(int size, TexCoordType type, int stride, float[] pointer)
        {
            glTexCoordPointer(size, (uint)type, stride, pointer);
        }

        internal void TexImage2D(TextureImageTarget target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels)
        {
            glTexImage2D((uint)target, level, internalformat, width, height, border, format, type, pixels);
        }

        internal void TexImage2D(TextureImageTarget target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels)
        {
            glTexImage2D((uint)target, level, internalformat, width, height, border, format, type, pixels);
        }

        internal void TexParameter(TextureTarget target, TextureParameter pname, float param)
        {
            glTexParameterf((uint)target, (uint)pname, param);
        }

        internal void TexParameter(TextureTarget target, TextureParameter pname, float[] parameters)
        {
            glTexParameterfv((uint)target, (uint)pname, parameters);
        }

        internal void Translate(float x, float y, float z)
        {
            glTranslatef(x, y, z);
        }

        internal void Translate(Vect3D v)
        {
            glTranslatef(v.x, v.y, v.z);
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
            gluUnProject(winx, winy, winz, modelView, projection, viewport, ref result[0], ref result[1], ref result[2]);

            return result;
        }

        internal void Vertex(float x, float y, float z)
        {
            glVertex3f(x, y, z);
        }

        internal void Vertex(Point3D pt)
        {
            glVertex3f(pt.x, pt.y, pt.z);
        }

        internal void VertexPointer(int size, int stride, float[] pointer)
        {
            glVertexPointer(size, (uint)DataType.Float, stride, pointer);
        }

        internal void Vertices(params Point3D[] pts)
        {
            pts.ToList().ForEach(o => glVertex3f(o.x, o.y, o.z));
        }

        internal void Vertices(IEnumerable<Point3D> pts)
        {
            pts.ToList().ForEach(o => glVertex3f(o.x, o.y, o.z));
        }

        internal void Viewport(int x, int y, int width, int height)
        {
            glViewport(x, y, width, height);
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