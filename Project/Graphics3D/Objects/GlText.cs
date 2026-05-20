using System.Drawing;

using Microvision.Geometry;
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

        private readonly string _text;
        private readonly Font _font;

        private Point3D _position;
        private float _extrusion;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlText(string text, Point3D pos, string fontName, float fontSize, FontStyle fontStyle)
        {
            _font = new Font(fontName, fontSize, fontStyle);
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
            if (isExplicit) _font.Dispose();

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