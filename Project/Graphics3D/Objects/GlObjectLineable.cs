using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public abstract class GlObjectLineable : GlObject
    {
        // ***************************************************************************************************
        // 02.05.19 : Création, objet 3D de base mais dont on peut afficher les contours
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected bool _isFill;

        protected bool _linesVisible;
        protected HColor _linesColor;
        protected xGlMaterial _linesMaterial;
        protected float _linesWidth;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected GlObjectLineable() : this(Color.WhiteSmoke)
        {
        }

        protected GlObjectLineable(HColor color) : base(color)
        {
            _isFill = true;
            _linesVisible = false;

            _linesMaterial = xGlMaterial.Flat(Color.Black);
            _linesWidth = 1;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool IsFill
        {
            get => _isFill;

            set
            {
                if (_isFill != value)
                {
                    oSetIsFill(value);
                }
            }
        }

        public HColor LinesColor
        {
            get => _linesColor;

            set
            {
                if (_linesColor != value)
                {
                    oSetLinesColor(value);
                }
            }
        }

        public bool LinesVisible
        {
            get => _linesVisible;

            set
            {
                if (_linesVisible != value)
                {
                    oSetLinesVisible(value);
                }
            }
        }

        public float LinesWidth
        {
            get => _linesWidth;

            set
            {
                if (_linesWidth != value)
                {
                    oSetLinesWidth(value);
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

        protected abstract void oRenderLines(OpenGLContext gl);

        protected override void oRenderSpecif(OpenGLContext gl)
        {
            if (_linesVisible)
            {
                _linesMaterial.Apply(gl);
                gl.LineWidth(_linesWidth);
                oRenderLines(gl);
            }
        }

        protected void oSetIsFill(bool value)
        {
            _isFill = value;
        }

        protected void oSetLinesColor(HColor value)
        {
            _linesColor = value;
            _linesMaterial = xGlMaterial.Flat(_linesColor);
        }

        protected void oSetLinesVisible(bool value)
        {
            _linesVisible = value;
        }

        protected void oSetLinesWidth(float value)
        {
            _linesWidth = value;
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