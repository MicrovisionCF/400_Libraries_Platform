using System;
using System.Runtime.InteropServices;

namespace Microvision.NativeMethods
{
    internal static partial class OpenGl32
    {
        [DllImport(nameof(OpenGl32))]
        public static extern IntPtr glGetString(uint name);

        [DllImport(nameof(OpenGl32))]
        public static extern uint glGenLists(int range);

        [DllImport(nameof(OpenGl32))]
        public static extern uint glGetError();

        [DllImport(nameof(OpenGl32))]
        public static extern void glBegin(uint mode);

        [DllImport(nameof(OpenGl32))]
        public static extern void glBindTexture(uint target, uint texture);

        [DllImport(nameof(OpenGl32))]
        public static extern void glBlendFunc(uint sfactor, uint dfactor);

        [DllImport(nameof(OpenGl32))]
        public static extern void glCallLists(int n, uint type, byte[] lists);

        [DllImport(nameof(OpenGl32))]
        public static extern void glClear(uint mask);

        [DllImport(nameof(OpenGl32))]
        public static extern void glClearColor(float red, float green, float blue, float alpha);

        [DllImport(nameof(OpenGl32))]
        public static extern void glClearDepth(double depth);

        [DllImport(nameof(OpenGl32))]
        public static extern void glColor3f(float red, float green, float blue);

        [DllImport(nameof(OpenGl32))]
        public static extern void glColorPointer(int size, uint type, int stride, float[] pointer);

        [DllImport(nameof(OpenGl32))]
        public static extern void glDeleteTextures(int n, uint[] textures);

        [DllImport(nameof(OpenGl32))]
        public static extern void glDepthFunc(uint func);

        [DllImport(nameof(OpenGl32))]
        public static extern void glDisable(uint cap);

        [DllImport(nameof(OpenGl32))]
        public static extern void glDisableClientState(uint array);

        [DllImport(nameof(OpenGl32))]
        public static extern void glDrawElements(uint mode, int count, uint type, uint[] indices);

        [DllImport(nameof(OpenGl32))]
        public static extern void glEnable(uint cap);

        [DllImport(nameof(OpenGl32))]
        public static extern void glEnableClientState(uint array);

        [DllImport(nameof(OpenGl32))]
        public static extern void glEnd();

        [DllImport(nameof(OpenGl32))]
        public static extern void glFlush();

        [DllImport(nameof(OpenGl32))]
        public static extern void glGenTextures(int n, uint[] textures);

        [DllImport(nameof(OpenGl32))]
        public static extern void glGetDoublev(uint pname, double[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glGetFloatv(uint pname, float[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glGetIntegerv(uint pname, int[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glHint(uint target, uint mode);

        [DllImport(nameof(OpenGl32))]
        public static extern void glLightf(uint light, uint pname, float param);

        [DllImport(nameof(OpenGl32))]
        public static extern void glLightfv(uint light, uint pname, float[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glLineStipple(int factor, ushort pattern);

        [DllImport(nameof(OpenGl32))]
        public static extern void glLineWidth(float width);

        [DllImport(nameof(OpenGl32))]
        public static extern void glListBase(uint base_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glLoadIdentity();

        [DllImport(nameof(OpenGl32))]
        public static extern void glLoadMatrixf(float[] m);

        [DllImport(nameof(OpenGl32))]
        public static extern void glMaterialf(uint face, uint pname, float param);

        [DllImport(nameof(OpenGl32))]
        public static extern void glMaterialfv(uint face, uint pname, float[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glMatrixMode(uint mode);

        [DllImport(nameof(OpenGl32))]
        public static extern void glNormal3f(float nx, float ny, float nz);

        [DllImport(nameof(OpenGl32))]
        public static extern void glNormalPointer(uint type, int stride, float[] pointer);

        [DllImport(nameof(OpenGl32))]
        public static extern void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);

        [DllImport(nameof(OpenGl32))]
        public static extern void glPopAttrib();

        [DllImport(nameof(OpenGl32))]
        public static extern void glPopMatrix();

        [DllImport(nameof(OpenGl32))]
        public static extern void glPushAttrib(uint mask);

        [DllImport(nameof(OpenGl32))]
        public static extern void glPushMatrix();

        [DllImport(nameof(OpenGl32))]
        public static extern void glRasterPos2i(int x, int y);

        [DllImport(nameof(OpenGl32))]
        public static extern void glReadBuffer(uint mode);

        [DllImport(nameof(OpenGl32))]
        public static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);

        [DllImport(nameof(OpenGl32))]
        public static extern void glReadPixels(int x, int y, int width, int height, uint format, uint type, IntPtr pixels);

        [DllImport(nameof(OpenGl32))]
        public static extern void glRotatef(float angle, float x, float y, float z);

        [DllImport(nameof(OpenGl32))]
        public static extern void glScalef(float x, float y, float z);

        [DllImport(nameof(OpenGl32))]
        public static extern void glShadeModel(uint mode);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexCoord2f(float s, float t);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexCoordPointer(int size, uint type, int stride, float[] pointer);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexParameterf(uint target, uint pname, float param);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTexParameterfv(uint target, uint pname, float[] params_notkeyword);

        [DllImport(nameof(OpenGl32))]
        public static extern void glTranslatef(float x, float y, float z);

        [DllImport(nameof(OpenGl32))]
        public static extern void glVertex3f(float x, float y, float z);

        [DllImport(nameof(OpenGl32))]
        public static extern void glVertexPointer(int size, uint type, int stride, float[] pointer);

        [DllImport(nameof(OpenGl32))]
        public static extern void glViewport(int x, int y, int width, int height);
    }
}
