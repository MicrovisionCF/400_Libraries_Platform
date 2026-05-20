using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlCylinder : GlObjectLineable
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, un cylindre 3D
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private float _height, _baseDiameter, _topDiameter;
        private Point3D _baseCenter;
        private bool _closed;

        private int _resolution;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlCylinder(Point3D baseCenter, float baseDiameter, float topDiameter, float height, bool closed) : this(baseCenter, baseDiameter, topDiameter, height, closed, Color.WhiteSmoke)
        {
        }

        public GlCylinder(Point3D baseCenter, float baseDiameter, float topDiameter, float height, bool closed, HColor col) : base(col)
        {
            _baseCenter = baseCenter;
            _baseDiameter = baseDiameter;
            _topDiameter = topDiameter;
            _height = height;
            _closed = closed;

            _resolution = 15;
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

            gl.Translate(_baseCenter);
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Fill);

            gl.Cylinder(obj, _baseDiameter / 2, _topDiameter / 2, _height, _resolution, 1);

            if (_closed)
            {
                if (_baseDiameter > 0)
                {
                    // Rotation pour avoir la normale dans le bon sens. Je ne sais pas pourquoi ça ne fonctionne pas avec glQuadricsNormal
                    // TODO : Attention ça ne fonctionne pas avec une resolution impaire...
                    gl.Rotate(180, 1, 0, 0);
                    gl.Disk(obj, 0, _baseDiameter / 2, _resolution, 1);
                    gl.Rotate(-180, 1, 0, 0);
                }

                if (_topDiameter > 0)
                {
                    gl.Translate(0, 0, _height);
                    gl.Disk(obj, 0, _topDiameter / 2, _resolution, 1);
                }
            }

            gl.DeleteQuadric(obj);
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            IntPtr obj = gl.NewQuadric();
            gl.QuadricDrawStyle(obj, QuadricDrawStyle.Silhouette);

            gl.Cylinder(obj, _baseDiameter / 2, _topDiameter / 2, _height, _resolution, 1);

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