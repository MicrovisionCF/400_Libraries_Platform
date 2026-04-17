using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlRectCube : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, un objet parallélépipède rectangle (un pavé...)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private Rect3D _bounds;

        private List<Point3D> _corners;
        private List<Point3D> _vertices;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlRectCube(Rect3D bounds) : this(bounds, Color.WhiteSmoke)
        {
        }

        public GlRectCube(Rect3D bounds, HColor color) : base(color)
        {
            _bounds = zNormalizeBound(bounds);

            _corners = zCalcCorners(_bounds);
            _vertices = zCalcVertices(_corners);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public Point3D Center => _bounds.Center;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            gl.Begin(BeginMode.Quads);

            for (int i = 0; i < _vertices.Count; i += 4)
            {
                gl.Normal(zCalcNormal(_vertices[i], _vertices[i + 1], _vertices[i + 2]));
                gl.Vertices(_vertices.GetRange(i, 4).ToArray());
            }

            gl.End();
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            gl.Begin(BeginMode.LineLoop);
            gl.Vertices(_corners[0], _corners[1], _corners[3], _corners[2]);
            gl.End();

            gl.Begin(BeginMode.LineLoop);
            gl.Vertices(_corners[4], _corners[5], _corners[7], _corners[6]);
            gl.End();

            gl.Begin(BeginMode.Lines);
            gl.Vertices(_corners[0], _corners[4]);
            gl.Vertices(_corners[1], _corners[5]);
            gl.Vertices(_corners[2], _corners[6]);
            gl.Vertices(_corners[3], _corners[7]);
            gl.End();
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static List<Point3D> zCalcCorners(Rect3D r)
        {
            return new List<Point3D> { new Point3D(r.X, r.Y, r.Z),
                                       new Point3D(r.X + r.Width, r.Y, r.Z),
                                       new Point3D(r.X, r.Y + r.Height, r.Z),
                                       new Point3D(r.X + r.Width, r.Y + r.Height, r.Z),
                                       new Point3D(r.X, r.Y, r.Z + r.Depth),
                                       new Point3D(r.X + r.Width, r.Y, r.Z + r.Depth),
                                       new Point3D(r.X, r.Y + r.Height, r.Z + r.Depth),
                                       new Point3D(r.X + r.Width, r.Y + r.Height, r.Z + r.Depth) };
        }

        private static Vect3D zCalcNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            return Vect3D.VectorProduct(p2 - p1, p3 - p2);
        }

        private static List<Point3D> zCalcVertices(List<Point3D> corners)
        {
            List<Point3D> v = new List<Point3D>();

            v.Add(corners[2]);
            v.Add(corners[3]);
            v.Add(corners[1]);
            v.Add(corners[0]);

            v.Add(corners[4]);
            v.Add(corners[6]);
            v.Add(corners[2]);
            v.Add(corners[0]);

            v.Add(corners[1]);
            v.Add(corners[5]);
            v.Add(corners[4]);
            v.Add(corners[0]);

            v.Add(corners[5]);
            v.Add(corners[7]);
            v.Add(corners[6]);
            v.Add(corners[4]);

            v.Add(corners[3]);
            v.Add(corners[7]);
            v.Add(corners[5]);
            v.Add(corners[1]);

            v.Add(corners[6]);
            v.Add(corners[7]);
            v.Add(corners[3]);
            v.Add(corners[2]);

            return v;
        }

        private static Rect3D zNormalizeBound(Rect3D bnds)
        {
            if (bnds.Width < 0)
            {
                bnds.Width = Math.Abs(bnds.Width);
                bnds.X -= bnds.Width;
            }

            if (bnds.Height < 0)
            {
                bnds.Height = Math.Abs(bnds.Height);
                bnds.Y -= bnds.Height;
            }

            if (bnds.Depth < 0)
            {
                bnds.Depth = Math.Abs(bnds.Depth);
                bnds.Z -= bnds.Depth;
            }

            return bnds;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}