using System;
using System.Drawing;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlSphere : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, une sphère 3D
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private int _resolution;
        private float _diameter;
        private Point3D _position;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlSphere(Point3D position, float diameter) : this(position, diameter, Color.WhiteSmoke)
        {
        }

        public GlSphere(Point3D position, float diameter, HColor col)
        {
            _diameter = diameter;
            _position = position;

            _resolution = 20;

            _material = new xGlMaterial(col);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public Point3D Center
        {
            get => _position;

            set
            {
                if (_position != value)
                {
                    _position = value;
                }
            }
        }

        public float Diameter
        {
            get => _diameter;

            set
            {
                if (_diameter != value)
                {
                    _diameter = value;
                }
            }
        }

        public int Resolution
        {
            get => _resolution;

            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
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
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Fill);

            gl.Sphere(obj, _diameter / 2, _resolution, _resolution / 2);

            gl.DeleteQuadric(obj);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Silhouette);

            gl.Sphere(obj, _diameter / 2, _resolution, _resolution / 2);

            gl.DeleteQuadric(obj);
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