using System;
using System.Drawing;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlCone : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, objet 3D conique
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private Point3D _spikePosition;
        private float _diameter;
        private float _height;
        private int _resolution;
        private bool _closed;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlCone(Point3D spikePosition, float diameter, float height, bool closed) : this(spikePosition, diameter, height, closed, Color.WhiteSmoke)
        {
        }

        public GlCone(Point3D spikePosition, float diameter, float height, bool closed, HColor color) : base(color)
        {
            _spikePosition = spikePosition;
            _diameter = diameter;
            _height = height;

            _resolution = 15;
            _closed = closed;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public Point3D Center => new Point3D(_spikePosition.X, _spikePosition.Y, _spikePosition.Z - _height / 2);

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
            gl.Translate(_spikePosition);
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Fill);

            gl.Cylinder(obj, 0, _diameter / 2, _height, _resolution, 1);

            if (_closed && _diameter > 0)
            {
                gl.Translate(0, 0, _height);
                gl.Disk(obj, 0, _diameter / 2, _resolution, 1);
            }

            gl.DeleteQuadric(obj);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Silhouette);

            gl.Cylinder(obj, 0, _diameter / 2, _height, _resolution, 1);

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