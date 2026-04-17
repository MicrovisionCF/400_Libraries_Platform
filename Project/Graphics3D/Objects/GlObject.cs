using Microvision.Graphic;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public abstract class GlObject : Citizen
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, objet 3D de base
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        protected struct xRotation
        {
            public Vect3D axis;
            public Point3D center;
            public float theta;

            public xRotation(Point3D center, Vect3D axis, float theta)
            {
                this.center = center;
                this.axis = axis;
                this.theta = theta;
            }
        }


        protected xGlMaterial _material;
        protected bool _visible;

        protected List<xRotation> _rotations;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected GlObject() : this(Color.WhiteSmoke)
        {
        }

        public GlObject(HColor col)
        {
            _rotations = new List<xRotation>();
            _visible = true;
            _material = new xGlMaterial(col, 0.5f, 0.75f, 0, 0.3f, 0.3f);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool IsTransparent => oIsTransparent();

        public xGlMaterial Material
        {
            get => _material;

            set
            {
                if (!_material.Equals(value))
                {
                    oSetMaterial(value);
                }
            }
        }

        public bool Visible
        {
            get => _visible;

            set
            {
                if (_visible != value)
                {
                    oSetVisible(value);
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void AddRotation(Point3D center, Vect3D axisRotation, float theta)
        {
            // Toujours ajouter les rotations dans l'ordre inverse d'application, exemple :
            // Un planète tourne sur elle même et orbite autour du soleil :
            // 1/ Ajouter la rotation sur l'orbite
            // 2/ Ajouter la rotation sur son axe
            // En général, si il y a une rotation de l'objet sur son centre, c'est la dernière rotation qu'il faut ajouter

            _rotations.Add(new xRotation(center, axisRotation, theta));
        }

        public void ClearRotations()
        {
            _rotations.Clear();
        }

        public void Render(OpenGLContext gl)
        {
            if (_visible)
            {
                oBeginRender(gl);
                if (_material.IsValid) _material.Apply(gl);
                oRender(gl);
                oEndRender(gl);

                oBeginRender(gl);
                oRenderSpecif(gl);
                oEndRender(gl);
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected virtual void oBeginRender(OpenGLContext gl)
        {
            gl.PushMatrix();

            _rotations.ForEach(o =>
            {
                gl.Translate(o.center.X, o.center.Y, o.center.Z);
                gl.Rotate(o.theta / MathF.PI * 180, o.axis.X, o.axis.Y, o.axis.Z);
                gl.Translate(-o.center.X, -o.center.Y, -o.center.Z);
            });
        }

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected virtual void oEndRender(OpenGLContext gl)
        {
            gl.PopMatrix();
        }

        protected virtual bool oIsTransparent()
        {
            return _material.IsTransparent;
        }

        protected abstract void oRender(OpenGLContext gl);

        protected virtual void oRenderSpecif(OpenGLContext gl)
        {
            // Rien par défaut
        }

        protected virtual void oSetMaterial(xGlMaterial value)
        {
            _material = value;
        }

        protected virtual void oSetVisible(bool value)
        {
            _visible = value;
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