using System;

using Microvision.Geometry;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlCamera : Citizen
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, décrit la façon dont on regarde une scène
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test contexte existant
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected Point3D _position;
        protected Point3D _observation;
        protected Vect3D _upDirection;

        private float _fovX;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlCamera()
        {
            _position = new Point3D(-1, -1, 1);
            _observation = new Point3D(0, 0, 0);
            _upDirection = new Vect3D(0, 0, 1);

            _fovX = zFOV(36, 50); // -- objectif "normal" en 24x36
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public float FieldOfView
        {
            get => _fovX;

            set
            {
                if (_fovX != value)
                {
                    _fovX = value;
                }
            }
        }

        public Point3D LookAt => _observation;

        public Point3D Position => _position;

        public Vect3D UpDirection => _upDirection;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Apply(OpenGLContext gl)
        {
            gl?.LookAt(_position.X, _position.Y, _position.Z,
                _observation.X, _observation.Y, _observation.Z,
                _upDirection.X, _upDirection.Y, _upDirection.Z);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected void oSetObservation(Point3D pos)
        {
            _observation = pos;
        }

        protected void oSetPosition(Point3D pos)
        {
            _position = pos;
        }

        protected void oSetUpDirection(Vect3D dir)
        {
            _upDirection = dir;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static float zFOV(float sensorW, float foc)
        {
            float th = MathF.Atan2(sensorW / 2, foc);

            return 2 * th;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}