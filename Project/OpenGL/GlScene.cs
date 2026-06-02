using System;
using System.Collections.Generic;
using System.Drawing;

using Microvision.Geometry;
using Microvision.Graphic;
using Microvision.Types;

namespace Microvision.OpenGL
{
    public class GlScene : Citizen
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, une scène est un ensemble d'objets, de lumières et une camera pour observer
        //            tout ça avec une taille de rendu.
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test création du contexte
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0) OpenGLContext non nullable. CreateScene statique pour tester la création
        //            du contexte avant de créer la scène. PaintErrorImage statique pour afficher un message
        //            d'erreur dans le cas où la création du contexte échoue.
        // ***************************************************************************************************

        private readonly IntPtr _hdc;
        private readonly OpenGLContext _gl;
        private readonly GlContainer _objects;
        private readonly GlLight _defaultLight;
        private readonly List<GlLight> _lights;

        private SizeI _size;
        private GlCamera _camera;

        private HColor _backColor;
        private float _nearestDistance, _farestDistance;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        protected GlScene(IntPtr winHdc, OpenGLContext gl)
        {
            if (winHdc != IntPtr.Zero) _hdc = winHdc;

            _backColor = Color.White;
            _gl = gl;

            _objects = new GlContainer();

            _camera = new GlCamera();
            _lights = [];
            _defaultLight = new GlLight();

            _nearestDistance = 1;
            _farestDistance = 1000000;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public HColor BackColor
        {
            get => _backColor;

            set
            {
                if (_backColor != value)
                {
                    oSetBackColor(value);
                }
            }
        }

        public string GLGraphicCard => _gl.GetString(StringTarget.Renderer);

        public string GLVersion => _gl.GetString(StringTarget.Version);

        public SizeI ViewPortSize
        {
            get => _size;

            set
            {
                if (_size != value)
                {
                    _size = value;
                    oSetRenderDimensions(_size, (float)_camera.FieldOfView);
                }
            }
        }


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static GlScene? CreateScene(IntPtr hdc = 0)
        {
            GlScene? scene;
            OpenGLContext? gl = new OpenGLContext();

            if (gl.CreateInMemory())
            {
                gl.Enable(EnableTarget.DepthTest);
                gl.Enable(EnableTarget.Blend);
                gl.Enable(EnableTarget.LineSmooth);
                gl.Enable(EnableTarget.PolygonSmooth);
                gl.Enable(EnableTarget.AutoNormal);
                gl.Enable(EnableTarget.Normalize);
                gl.Enable(EnableTarget.Lighting);
                // _gL.Enable(OpenGL.GL_MULTISAMPLE)

                gl.ClearDepth(1d);
                gl.ShadeModel(ShadeModel.Smooth);
                gl.DepthFunc(DepthFunction.LessThanOrEqual);
                gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);
                gl.Hint(HintTarget.LineSmooth, HintMode.Nicest);
                gl.Hint(HintTarget.PolygonSmooth, HintMode.Nicest);
                gl.Hint(HintTarget.PerspectiveCorrection, HintMode.Nicest);

                gl.ClearColor(1, 1, 1, 0);
                scene = new GlScene(hdc, gl);
            }
            else
            {
                gl.Dispose();
                scene = null;
            }

            return scene;
        }

        public static void PaintErrorImage(Graphics g, SizeI size)
        {
            using StdGraphic dc = new StdGraphic(g);
            dc.FillRect(RectI.FromSize(size), Brushes.Black);
            dc.SetFont("Arial", 22, FontStyle.Regular);
            dc.PrintIn("3D View unavailable." + Environment.NewLine + "OpenGL not installed or outdated.", RectI.FromSize(size), Color.Red, ContentAlignment.MiddleCenter);
            dc.ResetFont();
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void AddLight(GlLight light)
        {
            bool hadLamps = _lights.Count > 0;

            _lights.Add(light.AddLife());

            // La lampe par défaut sert uniquement si on n'ajoute aucune lumière
            if (!hadLamps) _defaultLight.TurnOff(_gl);
        }

        public void AddObject(GlObject obj)
        {
            _objects.AddSubItem(obj);
        }

        public GlTexture CreateTexture(Bitmap img)
        {
            return new GlTexture(_gl, img);
        }

        public void RemoveObject(GlObject obj)
        {
            _objects.RemoveSubItem(obj);
        }

        public Bitmap RenderBitmap()
        {
            Bitmap bmp = new Bitmap(_size.Width, _size.Height);
            using Graphics g = Graphics.FromImage(bmp);
            oRenderGraphics(g);

            return bmp;
        }

        public void RenderGraphics(Graphics g)
        {
            oRenderGraphics(g);
        }

        public void RenderHdc()
        {
            oRenderHdc(_hdc);
        }

        public void SetCamera(GlCamera cam)
        {
            _camera.Dispose();
            _camera = cam.AddLife();

            oSetRenderDimensions(_size, (float)_camera.FieldOfView);
        }

        public void SetDistanceLimits(float nearest, float farest)
        {
            _nearestDistance = nearest;
            _farestDistance = farest;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _objects.Dispose();

            if (isExplicit) _defaultLight.Dispose();

            if (isExplicit) _lights.ForEach(o => o.Dispose());

            if (isExplicit) _camera.Dispose();

            if (isExplicit) _gl.Dispose();

            base.oDispose(isExplicit);
        }

        protected void oRenderGraphics(Graphics g)
        {
            IntPtr hdc = g.GetHdc();
            oRenderHdc(hdc);
            g.ReleaseHdc(hdc);
        }

        protected void oRenderHdc(IntPtr hdc)
        {
            _gl.MakeCurrent();

            _gl.Clear((uint)(AttributeMask.ColorBuffer | AttributeMask.DepthBuffer));
            _gl.MatrixMode(MatrixMode.Modelview);
            _gl.LoadIdentity();

            _camera.Apply(_gl);

            if (_lights.Count > 0)
            {
                _lights.ForEach(o =>
                {
                    o.TurnOn(_gl);
                    o.Render(_gl, Color.Orange);
                });
            }
            else
            {
                _defaultLight.TurnOn(_gl);
                _defaultLight.Render(_gl, Color.Orange);
            }

            _objects.Render(_gl);

            _gl.Blit(hdc);
        }

        protected void oSetBackColor(HColor color)
        {
            _backColor = color;
            _gl.ClearColor(_backColor.Red / 255f, _backColor.Green / 255f, _backColor.Blue / 255f, 0);
        }

        protected void oSetRenderDimensions(SizeI sz, float fovx)
        {
            _gl.SetDimensions(sz.Width, sz.Height);
            _gl.Viewport(0, 0, sz.Width, sz.Height);
            _gl.MatrixMode(MatrixMode.Projection);
            _gl.LoadIdentity();
            // OpenGL veut un fov en Y, c'est pas très pratique je trouve alors on le garde en X et on lui donne en Y
            _gl.Perspective(float.RadiansToDegrees(zFovXToFovY(fovx, sz)), sz.Width / (double)sz.Height, _nearestDistance, _farestDistance);
            _gl.MatrixMode(MatrixMode.Modelview);
            _gl.LoadIdentity();
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static float zFovXToFovY(float fovX, SizeI viewportSize)
        {
            float dist = viewportSize.Width / MathF.Tan(fovX / 2) / 2;
            float alpha = MathF.Atan(viewportSize.Height / dist / 2) * 2;

            return alpha;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}