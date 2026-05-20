using System;
using System.Runtime.InteropServices;

namespace Microvision.NativeMethods
{
    internal class Glu32
    {
        [DllImport(nameof(Glu32))]
        public static extern IntPtr gluNewNurbsRenderer();

        [DllImport(nameof(Glu32))]
        public static extern IntPtr gluNewQuadric();

        [DllImport(nameof(Glu32))]
        public static extern IntPtr gluNewTess();

        [DllImport(nameof(Glu32))]
        public static extern void gluBeginCurve(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluBeginSurface(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluBeginTrim(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluBuild1DMipmaps(uint target, uint components, int width, uint format, uint type, IntPtr data);

        [DllImport(nameof(Glu32))]
        public static extern void gluBuild2DMipmaps(uint target, uint components, int width, int height, uint format, uint type, IntPtr data);

        [DllImport(nameof(Glu32))]
        public static extern void gluCylinder(IntPtr qobj, double baseRadius, double topRadius, double height, int slices, int stacks);

        [DllImport(nameof(Glu32))]
        public static extern void gluDeleteNurbsRenderer(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluDeleteQuadric(IntPtr state);

        [DllImport(nameof(Glu32))]
        public static extern void gluDeleteTess(IntPtr tess);

        [DllImport(nameof(Glu32))]
        public static extern void gluDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops);

        [DllImport(nameof(Glu32))]
        public static extern void gluEndCurve(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluEndSurface(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluEndTrim(IntPtr nobj);

        [DllImport(nameof(Glu32))]
        public static extern void gluGetNurbsProperty(IntPtr nobj, int property, float value);

        [DllImport(nameof(Glu32))]
        public static extern void gluGetTessProperty(IntPtr tess, int which, double value);

        [DllImport(nameof(Glu32))]
        public static extern void gluLoadSamplingMatrices(IntPtr nobj, float[] modelMatrix, float[] projMatrix, int[] viewport);

        [DllImport(nameof(Glu32))]
        public static extern void gluLookAt(double eyex, double eyey, double eyez, double centerx, double centery, double centerz, double upx, double upy, double upz);

        [DllImport(nameof(Glu32))]
        public static extern void gluNurbsCurve(IntPtr nobj, int nknots, float[] knot, int stride, float[] ctlarray, int order, uint type);

        [DllImport(nameof(Glu32))]
        public static extern void gluNurbsProperty(IntPtr nobj, int property, float value);

        [DllImport(nameof(Glu32))]
        public static extern void gluNurbsSurface(IntPtr nobj, int sknot_count, float[] sknot, int tknot_count, float[] tknot, int s_stride, int t_stride, float[] ctlarray, int sorder, int torder, uint type);

        [DllImport(nameof(Glu32))]
        public static extern void gluOrtho2D(double left, double right, double bottom, double top);

        [DllImport(nameof(Glu32))]
        public static extern void gluPartialDisk(IntPtr qobj, double innerRadius, double outerRadius, int slices, int loops, double startAngle, double sweepAngle);

        [DllImport(nameof(Glu32))]
        public static extern void gluPerspective(double fovy, double aspect, double zNear, double zFar);

        [DllImport(nameof(Glu32))]
        public static extern void gluPickMatrix(double x, double y, double width, double height, int[] viewport);

        [DllImport(nameof(Glu32))]
        public static extern void gluProject(double objx, double objy, double objz, double[] modelMatrix, double[] projMatrix, int[] viewport, double[] winx, double[] winy, double[] winz);

        [DllImport(nameof(Glu32))]
        public static extern void gluPwlCurve(IntPtr nobj, int count, float array, int stride, uint type);

        [DllImport(nameof(Glu32))]
        public static extern void gluQuadricDrawStyle(IntPtr quadObject, uint drawStyle);

        [DllImport(nameof(Glu32))]
        public static extern void gluQuadricNormals(IntPtr quadObject, uint normals);

        [DllImport(nameof(Glu32))]
        public static extern void gluQuadricOrientation(IntPtr quadObject, int orientation);

        [DllImport(nameof(Glu32))]
        public static extern void gluQuadricTexture(IntPtr quadObject, int textureCoords);

        [DllImport(nameof(Glu32))]
        public static extern void gluScaleImage(int format, int widthin, int heightin, int typein, int[] datain, int widthout, int heightout, int typeout, int[] dataout);

        [DllImport(nameof(Glu32))]
        public static extern void gluSphere(IntPtr qobj, double radius, int slices, int stacks);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessBeginContour(IntPtr tess);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessBeginPolygon(IntPtr tess, IntPtr polygonData);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessEndContour(IntPtr tess);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessEndPolygon(IntPtr tess);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessNormal(IntPtr tess, double x, double y, double z);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessProperty(IntPtr tess, int which, double value);

        [DllImport(nameof(Glu32))]
        public static extern void gluTessVertex(IntPtr tess, double[] coords, double[] data);

        [DllImport(nameof(Glu32))]
        public static extern void gluUnProject(double winx, double winy, double winz, double[] modelMatrix, double[] projMatrix, int[] viewport, ref double objx, ref double objy, ref double objz);

    }
}
