using System;
using System.Drawing;
using System.Linq;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlOrigin : GlObject
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, objet représentant l'axe XYZ en 3D du repère OpenGL
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private Vect3D _size;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlOrigin(float sz)
        {
            _size = new Vect3D(sz, sz, sz);
        }

        public GlOrigin(Vect3D sz)
        {
            _size = sz;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public Vect3D Size
        {
            get => _size;

            set
            {
                if (_size != value)
                {
                    _size = value;
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
            float maxSize = new[] { _size.X, _size.Y, _size.Z }.Max();

            IntPtr obj = gl.NewQuadric();

            gl.MaterialGlobal(Color.DarkGray, 0.3f, 0.5f, 0, 0.7f, 0.8f);
            gl.Sphere(obj, maxSize / 60, 30, 30);

            gl.Rotate(0, 90, 0);
            zArrow(gl, maxSize, _size.X, Color.Red, "X");
            gl.Rotate(0, -90, 0);
            gl.Rotate(-90, 0, 0);
            zArrow(gl, maxSize, _size.Y, Color.Lime, "Y");
            gl.Rotate(90, 0, 0);
            gl.Rotate(0, 0, -90);
            zArrow(gl, maxSize, _size.Z, Color.Blue, "Z");
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static void zArrow(OpenGLContext gl, float size, float lenght, HColor color, string letter)
        {
            gl.MaterialGlobal(color, 0.2f, 0.6f, 0, 1, 0.2f);

            IntPtr obj = gl.NewQuadric();
            gl.PushMatrix();

            gl.Cylinder(obj, size / 150, size / 150, lenght, 15, 1);
            gl.Translate(0, 0, lenght);
            gl.Cylinder(obj, size / 35, 0, size / 20, 15, 1);

            gl.Translate(size / 50, size / 28, size / 10);
            gl.Rotate(90, -135, 0);

            GlText text = new GlText(letter, new Point3D(), "Courier New", size / 10, FontStyle.Regular);
            text.Material = new xGlMaterial(color, 0.2f, 0.6f, 0, 1, 0.2f);
            text.LinesVisible = true;
            text.Extrusion = 0.1f;
            text.Render(gl);
            text.Dispose();

            gl.PopMatrix();
            gl.DeleteQuadric(obj);
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}