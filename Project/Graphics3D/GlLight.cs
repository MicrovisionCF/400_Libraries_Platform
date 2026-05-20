using System;
using System.Collections.Generic;
using System.Linq;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlLight : Citizen
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, une source lumineuse pour une scène OpenGL
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test contexte existant
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private static List<int> _availableLights;


        private Point3D _position;
        private bool _ponctual;
        private HColor _color;

        private bool _sunVisible;
        private float _sunDiameter;

        private LightName _lightNo;

        private float _ambientCompo, _diffuseCompo, _specularCompo;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        static GlLight()
        {
            _availableLights = Enumerable.Range((int)LightName.Light0, 8).ToList();
        }

        public GlLight()
        {
            _position = new Point3D();
            _ponctual = false;
            _color = System.Drawing.Color.White;

            _ambientCompo = 1;
            _diffuseCompo = 1;
            _specularCompo = 1;

            lock (_availableLights)
            {
                if (_availableLights.Count == 0)
                {
                    // Actuellement on ne peut utiliser que 8 lumières en tout alors que techniquement c'est 8 lumières par contexte.
                    // Si quelqu'un se heurte à cette limite, il faudra envisager d'allouer les numeros de lumière par contexte.
                    throw new Exception("Too much lights for OpenGL context");
                }

                _lightNo = (LightName)_availableLights[0];
                _availableLights.RemoveAt(0);
            }
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public HColor Color
        {
            get => _color;

            set
            {
                if (_color != value)
                {
                    _color = value;
                }
            }
        }

        public float CompoAmbient
        {
            get => _ambientCompo;

            set
            {
                if (_ambientCompo != value)
                {
                    _ambientCompo = value;
                }
            }
        }

        public float CompoDiffuse
        {
            get => _diffuseCompo;

            set
            {
                if (_diffuseCompo != value)
                {
                    _diffuseCompo = value;
                }
            }
        }

        public float CompoSpecular
        {
            get => _specularCompo;

            set
            {
                if (_specularCompo != value)
                {
                    _specularCompo = value;
                }
            }
        }

        public bool IsPonctual
        {
            get => _ponctual;

            set
            {
                if (_ponctual != value)
                {
                    _ponctual = value;
                }
            }
        }

        public Point3D Position
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

        public float SunDiameter
        {
            get => _sunDiameter;

            set
            {
                if (_sunDiameter != value)
                {
                    _sunDiameter = value;
                }
            }
        }

        public bool SunVisible
        {
            get => _sunVisible;

            set
            {
                if (_sunVisible != value)
                {
                    _sunVisible = value;
                }
            }
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Render(OpenGLContext gl, HColor col)
        {
            if (gl is not null && _sunVisible)
            {
                float sz1 = _sunDiameter / 2 * 3;
                float sz2 = MathF.Sqrt(sz1 * sz1 / 2);

                gl.MatrixMode(MatrixMode.Modelview);

                gl.PushMatrix();
                gl.Translate(_position.X, _position.Y, _position.Z);
                gl.MaterialGlobal(col, 0, 0, 1, 0, 0);

                IntPtr obj = gl.NewQuadric();
                gl.Sphere(obj, _sunDiameter / 2, 20, 20);
                gl.DeleteQuadric(obj);

                gl.LineWidth(1f);
                gl.Begin(BeginMode.Lines);

                gl.Vertex(sz1, 0, 0);
                gl.Vertex(-sz1, 0, 0);
                gl.Vertex(0, sz1, 0);
                gl.Vertex(0, -sz1, 0);
                gl.Vertex(0, 0, sz1);
                gl.Vertex(0, 0, -sz1);

                gl.Vertex(sz2, sz2, 0);
                gl.Vertex(-sz2, -sz2, 0);
                gl.Vertex(sz2, -sz2, 0);
                gl.Vertex(-sz2, sz2, 0);

                gl.Vertex(0, sz2, sz2);
                gl.Vertex(0, -sz2, -sz2);
                gl.Vertex(0, sz2, -sz2);
                gl.Vertex(0, -sz2, sz2);

                gl.Vertex(sz2, 0, sz2);
                gl.Vertex(-sz2, 0, -sz2);
                gl.Vertex(sz2, 0, -sz2);
                gl.Vertex(-sz2, 0, sz2);

                gl.End();

                gl.PopMatrix();
            }
        }

        public void TurnOff(OpenGLContext gl)
        {
            gl.Disable((EnableTarget)_lightNo);
        }

        public void TurnOn(OpenGLContext gl)
        {
            if (gl is not null)
            {
                gl.Enable((EnableTarget)_lightNo);
                gl.Light(_lightNo, LightParameter.Position, new[] { _position.X, _position.Y, _position.Z, _ponctual ? 1 : 0 });

                float r = _color.Red / 255f;
                float g = _color.Green / 255f;
                float b = _color.Blue / 255f;

                gl.Light(_lightNo, LightParameter.Ambient, new[] { _ambientCompo * r, _ambientCompo * g, _ambientCompo * b, 1 });
                gl.Light(_lightNo, LightParameter.Diffuse, new[] { _diffuseCompo * r, _diffuseCompo * g, _diffuseCompo * b, 1 });
                gl.Light(_lightNo, LightParameter.Specular, new[] { _specularCompo * r, _specularCompo * g, _specularCompo * b, 1 });
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            lock (_availableLights)
            {
                _availableLights.Add((int)_lightNo);
            }

            base.oDispose(isExplicit);
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