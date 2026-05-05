using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlFace : GlObjectLineable
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, plan à 4 sommets avec une texture associée (permet de projeter une image dans une scène)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private readonly List<Point3D> _points;

        private GlTexture? _texture;



        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlFace(List<Point3D> pts) : base(new xGlMaterial(Color.White, 1, 1, 0, 0, 0))
        {
            if (pts.Count != 4) throw new InvalidOperationException("Une face doit être constituée de 4 points");

            _points = pts;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void SetTexture(GlTexture texture)
        {
            _texture?.Dispose();
            _texture = texture;
            _texture?.AddLife();
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_texture is not null)
            {
                if (isExplicit) _texture.Dispose();
                _texture = null;
            }

            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            if (_texture is not null)
            {
                gl.Enable(EnableTarget.Texture2D);
                _texture.Bind(gl);
            }

            gl.Begin(BeginMode.Quads);
            // TODO3D elles sont ou les normales ?
            gl.TexCoord(0, 0);
            gl.Vertex(_points[0]);
            gl.TexCoord(1, 0);
            gl.Vertex(_points[1]);
            gl.TexCoord(1, 1);
            gl.Vertex(_points[2]);
            gl.TexCoord(0, 1);
            gl.Vertex(_points[3]);

            gl.End();

            gl.Disable(EnableTarget.Texture2D);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            gl.Begin(BeginMode.LineLoop);
            gl.Vertices([.. _points]);
            gl.End();
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Vect3D zCalcNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            return Vect3D.VectorProduct(p2 - p1, p3 - p2);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}