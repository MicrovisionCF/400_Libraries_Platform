using System;
using System.Drawing;

using Microvision.Graphic;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlDisk : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, un disque 3D, potentiellement partiellement ouvert
        // 21.11.19 : (libs 2.2)(libs 3.0)
        // ***************************************************************************************************

        private Point3D _center;
        private float _innerDiameter, _outerDiameter;

        private int _resolution;

        private float _partialAngleStart, _partialAngle;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlDisk(Point3D center, float outerDiameter) : this(center, outerDiameter, Color.WhiteSmoke)
        {
        }

        public GlDisk(Point3D center, float outerDiameter, HColor col) : base(col)
        {
            _center = center;
            _outerDiameter = outerDiameter;

            _innerDiameter = 0;
            _resolution = 15;

            _partialAngleStart = 0;
            _partialAngle = 360;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public Point3D Center
        {
            get => _center;

            set
            {
                if (_center != value)
                {
                    _center = value;
                }
            }
        }

        public float InnerDiameter
        {
            get => _innerDiameter;

            set
            {
                if (_innerDiameter != value)
                {
                    _innerDiameter = value;
                }
            }
        }

        public float PartialAngle
        {
            get => _partialAngle;

            set
            {
                if (_partialAngle != value)
                {
                    _partialAngle = value;
                }
            }
        }

        public float PartialAngleStart
        {
            get => _partialAngleStart;

            set
            {
                if (_partialAngleStart != value)
                {
                    _partialAngleStart = value;
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
            gl.Translate(_center);
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Fill);

            if (_partialAngle < 360)
                gl.PartialDisk(obj, _innerDiameter / 2, _outerDiameter / 2, ((float)_resolution / 360 * _partialAngle).ToRoundInt(), 1, _partialAngleStart, _partialAngle);
            else
                gl.Disk(obj, _innerDiameter / 2, _outerDiameter / 2, _resolution, 1);

            gl.DeleteQuadric(obj);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();

            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Line);

            if (_partialAngle < 360)
                gl.PartialDisk(obj, _innerDiameter / 2, _outerDiameter / 2, ((float)_resolution / 360 * _partialAngle).ToRoundInt(), 1, _partialAngleStart, _partialAngle);
            else
                gl.Disk(obj, _innerDiameter / 2, _outerDiameter / 2, _resolution, 1);

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