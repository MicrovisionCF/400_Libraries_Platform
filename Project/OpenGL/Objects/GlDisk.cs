using System;
using System.Drawing;

using Microvision.Geometry;
using Microvision.Graphic;

namespace Microvision.OpenGL
{
    public class GlDisk : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, un disque 3D, potentiellement partiellement ouvert
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly Point3D _center;
        
        private readonly float _innerDiameter;
        private readonly float _outerDiameter;
        
        private readonly float _partialAngleStart;
        private readonly float _partialAngle;

        private int _resolution;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlDisk(Point3D center, float outerDiameter) : this(center, outerDiameter, Color.WhiteSmoke)
        {
        }

        public GlDisk(Point3D center, float outerDiameter, HColor color) : base(color)
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