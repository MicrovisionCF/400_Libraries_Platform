using System.Timers;

using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlCameraSphere : GlCamera
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, camera regardant un point fixe et pouvant tourner autour
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public delegate void PositionChangedEventHandler();

        public event PositionChangedEventHandler PositionChanged;

        // ***************************************************************************************************

        protected float _thetaX, _thetaY;
        protected float _distance;

        protected int _animInterval;
        protected float _animThetaX;
        protected float _animThetaY;
        private Timer _timer;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlCameraSphere()
        {
            oSetObservation(new Point3D());
            oSetUpDirection(new Vect3D(0, 0, 1));

            _distance = 10;
            _animInterval = 50;
            _animThetaX = 5 * MathF.DegToRad;
            _animThetaY = 0;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool AnimationRunning => (_timer is not null);

        public Point3D Center
        {
            get => _observation;

            set
            {
                if (_observation != value)
                {
                    oSetObservation(value);
                    oSetPosition(zCalcPosition(_observation, _distance, _thetaX, _thetaY));
                }
            }
        }

        public float Distance
        {
            get => _distance;

            set
            {
                if (_distance != value)
                {
                    oSetDistance(value);
                    oSetPosition(zCalcPosition(_observation, _distance, _thetaX, _thetaY));
                }
            }
        }

        public float ThetaX
        {
            get => _thetaX;

            set
            {
                if (_thetaX != value)
                {
                    oSetThetaX(value);
                    oSetPosition(zCalcPosition(_observation, _distance, _thetaX, _thetaY));
                }
            }
        }

        public float ThetaY
        {
            get => _thetaY;

            set
            {
                if (_thetaY != value)
                {
                    oSetThetaY(value);
                    oSetPosition(zCalcPosition(_observation, _distance, _thetaX, _thetaY));
                    oSetUpDirection(zCalcUpDirection(_thetaY));
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void SetAnim(int interval, float thetaX, float thetaY)
        {
            _animInterval = interval;
            _animThetaX = thetaX;
            _animThetaY = thetaY;
        }

        public void StartAnimation()
        {
            if (_timer is null)
            {
                _timer = new Timer(_animInterval);
                _timer_Attach(true);
                _timer.Start();
            }
        }

        public void StopAnimation()
        {
            if (_timer is not null)
            {
                _timer_Attach(false);
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            StopAnimation();

            base.oDispose(isExplicit);
        }

        protected void oOnPositionChanged()
        {
            PositionChanged?.Invoke();
        }

        protected void oSetDistance(float distance)
        {
            _distance = distance;
        }

        protected void oSetThetaX(float thetaX)
        {
            _thetaX = thetaX;

            while (_thetaX < 0)
                _thetaX += MathF.PI * 2;

            _thetaX = _thetaX % (MathF.PI * 2);
        }

        protected void oSetThetaY(float thetaY)
        {
            _thetaY = thetaY;

            while (_thetaY < 0)
                _thetaY += MathF.PI * 2;

            _thetaY = _thetaY % (MathF.PI * 2);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static Point3D zCalcPosition(Point3D center, float distance, float thetaX, float thetaY)
        {
            return new Point3D(center.x + distance * MathF.Sin(thetaX) * MathF.Cos(thetaY),
                                center.y + distance * MathF.Cos(thetaX) * MathF.Cos(thetaY),
                                center.z + distance * MathF.Sin(thetaY));
        }

        private static Vect3D zCalcUpDirection(float thetaY)
        {
            // On cherche seulement si on a la tête en bas ou en haut

            float th = thetaY;
            Vect3D upVect;

            while (th < 0)
                th += MathF.PI * 2;
            while (th > MathF.PI * 2)
                th -= MathF.PI * 2;

            if (th < MathF.PI / 2 * 3 && th > MathF.PI / 2)
                upVect = new Vect3D(0, 0, -1);
            else
                upVect = new Vect3D(0, 0, 1);

            return upVect;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _timer_Attach(bool attach)
        {
            if (attach)
            {
                _timer.Elapsed += _timer_Elapsed;
            }
            else
            {
                _timer.Elapsed -= _timer_Elapsed;
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.ThetaX += _animThetaX;
            this.ThetaY += _animThetaY;

            oOnPositionChanged();
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}