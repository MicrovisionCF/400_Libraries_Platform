using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlLine : GlObject
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, ligne entre 2 points
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private float _width;
        private Point3D _fromPt, _toPt;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlLine(Point3D fromPt, Point3D toPt)
        {
            _width = 1;
            _fromPt = fromPt;
            _toPt = toPt;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public float Width
        {
            get => _width;

            set
            {
                if (_width != value)
                {
                    _width = value;
                }
            }
        }


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
            gl.LineWidth(_width);

            gl.Begin(BeginMode.Lines);
            gl.Vertices(_fromPt, _toPt);
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