using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlText : GlObjectLineable
    {
        // ***************************************************************************************************
        // 17.06.19 : Création, un texte en 3D
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private string _text;
        private Font _font;
        private Point3D _position;
        private float _extrusion;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlText(string text, Point3D pos, float fontSize)
        {
            _font = new Font("Courier New", fontSize);
            _text = text;
            _position = pos;
            _extrusion = 0.2f;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public float Extrusion
        {
            get => _extrusion;

            set
            {
                if (_extrusion != value)
                {
                    _extrusion = value;
                }
            }
        }

        public string FontName
        {
            get => _font.Name;

            set
            {
                if (_font.Name != value)
                {
                    Font tmp = _font;
                    _font = new Font(value, tmp.Size, tmp.Style);
                    tmp.Dispose();
                }
            }
        }

        public float FontSize
        {
            get => _font.Size;

            set
            {
                if (_font.Size != value)
                {
                    Font tmp = _font;
                    _font = new Font(tmp.FontFamily, value, tmp.Style);
                    tmp.Dispose();
                }
            }
        }

        public FontStyle FontStyle
        {
            get => _font.Style;

            set
            {
                if (_font.Style != value)
                {
                    Font tmp = _font;
                    _font = new Font(tmp.FontFamily, tmp.Size, value);
                    tmp.Dispose();
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oBeginRender(OpenGLContext gl)
        {
            base.oBeginRender(gl);
            gl.Translate(_position);

            // C'est bien plus efficace de faire un scale que de changer de taille de police :
            // - Déjà les tailles de police ça a pas l'air de bien marcher à toutes les échelles (police taille 800 ?!)...
            // - Ca evite de créer des pointeurs sur des polices pour chaque taille de police qu'on veut utiliser
            // - On ne maitrise la dispo/pas dispo de chaque taille pour chaque police
            gl.Scale(_font.Size * 1.6f, _font.Size * 1.6f, _font.Size * 1.6f);
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_font is not null)
            {
                if (isExplicit) _font.Dispose();
                _font = null;
            }

            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            gl.DrawText(_text, _font.Name, _extrusion, true);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            gl.DrawText(_text, _font.Name, _extrusion, false);
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