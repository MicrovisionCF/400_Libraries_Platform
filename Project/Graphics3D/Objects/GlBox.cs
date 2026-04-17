using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlBox : GlObject
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, objet représentant les arretes d'un cube sans sommet
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private HColor _color;
        private Rect3D _rect;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlBox(Rect3D rct)
        {
            _color = Color.Black;
            _rect = rct;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


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
            Point3D ptLowTopleft = new Point3D(_rect.x, _rect.y + _rect.h, _rect.z);
            Point3D ptLowTopRight = new Point3D(_rect.x + _rect.w, _rect.y + _rect.h, _rect.z);
            Point3D ptLowBottomRight = new Point3D(_rect.x + _rect.w, _rect.y, _rect.z);
            Point3D ptLowBottomLeft = new Point3D(_rect.x, _rect.y, _rect.z);

            Point3D ptHighTopleft = new Point3D(_rect.x, _rect.y + _rect.h, _rect.z + _rect.d);
            Point3D ptHighTopRight = new Point3D(_rect.x + _rect.w, _rect.y + _rect.h, _rect.z + _rect.d);
            Point3D ptHighBottomRight = new Point3D(_rect.x + _rect.w, _rect.y, _rect.z + _rect.d);
            Point3D ptHighBottomLeft = new Point3D(_rect.x, _rect.y, _rect.z + _rect.d);

            gl.LineWidth(1);
            gl.MaterialGlobal(_color, 0, 0, 0, 0, 0);

            gl.Begin(BeginMode.Lines);
            gl.Vertices(ptLowBottomLeft, ptHighBottomLeft);
            gl.Vertices(ptLowBottomRight, ptHighBottomRight);
            gl.Vertices(ptLowTopRight, ptHighTopRight);
            gl.Vertices(ptLowTopleft, ptHighTopleft);
            gl.End();

            gl.Begin(BeginMode.LineLoop);
            gl.Vertices(ptLowBottomLeft, ptLowTopleft, ptLowTopRight, ptLowBottomRight);
            gl.End();
        }


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