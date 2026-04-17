using Microvision.Graphic;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlScene : Citizen
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, une scène est un ensemble d'objets, de lumières et une camera pour observer
        //            tout ça avec une taille de rendu.
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test création du contexte
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private OpenGLContext _gl;
        private SizeI _size;

        private IntPtr _hdc;

        private List<GlLight> _lights;
        private GlLight _defaultLight;
        private GlCamera _camera;
        private HColor _backColor;

        private GlContainer _objects;
        private float _nearestDistance, _farestDistance;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlScene() : this((IntPtr)0)
        {
        }

        public GlScene(IntPtr winHdc)
        {
            if (winHdc != (IntPtr)0) _hdc = winHdc;

            oInitGL();
            oSetBackColor(Color.White);

            _objects = new GlContainer();

            _camera = new GlCamera();
            _lights = new List<GlLight>();
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

        public bool CanRender => _gl is not null;

        public string GLGraphicCard => _gl?.GetString(StringTarget.Renderer) ?? "OpenGL Error";

        public string GLVersion => _gl?.GetString(StringTarget.Version) ?? "OpenGL Error";

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
        // Méthodes
        // ----------------------------------------

        public void AddLight(GlLight light)
        {
            _lights.Add(light.AddLife());

            if (_defaultLight is not null)
            {
                // La lampe par défaut sert uniquement si on n'ajoute aucune lumière
                _defaultLight.TurnOff(_gl);
                _defaultLight.Dispose();
                _defaultLight = null;
            }
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
            Bitmap bmp;
            Graphics g;
            bmp = new Bitmap(_size.w, _size.h);
            g = Graphics.FromImage(bmp);
            oRenderGraphics(g);
            g.Dispose();
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
            if (_camera is not null)
            {
                _camera.Dispose();
            }

            _camera = cam;
            if (_camera is not null)
            {
                _camera.AddLife();
                oSetRenderDimensions(_size, (float)_camera.FieldOfView);
            }
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
            if (_objects is not null)
            {
                if (isExplicit) _objects.Dispose();
                _objects = null;
            }

            if (_lights is not null)
            {
                if (isExplicit) _lights.ForEach(o => o.Dispose());
                _lights = null;
            }

            if (_defaultLight is not null)
            {
                if (isExplicit) _defaultLight.Dispose();
                _defaultLight = null;
            }

            if (_camera is not null)
            {
                if (isExplicit) _camera.Dispose();
                _camera = null;
            }

            if (_gl is not null)
            {
                if (isExplicit) _gl.Dispose();
                _gl = null;
            }

            base.oDispose(isExplicit);
        }

        protected bool oInitGL()
        {
            _gl = new OpenGLContext();

            if (_gl.CreateInMemory())
            {
                _gl.Enable(EnableTarget.DepthTest);
                _gl.Enable(EnableTarget.Blend);
                _gl.Enable(EnableTarget.LineSmooth);
                _gl.Enable(EnableTarget.PolygonSmooth);
                _gl.Enable(EnableTarget.AutoNormal);
                _gl.Enable(EnableTarget.Normalize);
                _gl.Enable(EnableTarget.Lighting);
                // _gL.Enable(OpenGL.GL_MULTISAMPLE)

                _gl.ClearDepth(1d);
                _gl.ShadeModel(ShadeModel.Smooth);
                _gl.DepthFunc(DepthFunction.LessThanOrEqual);
                _gl.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);
                _gl.Hint(HintTarget.LineSmooth, HintMode.Nicest);
                _gl.Hint(HintTarget.PolygonSmooth, HintMode.Nicest);
                _gl.Hint(HintTarget.PerspectiveCorrection, HintMode.Nicest);
            }
            else
            {
                _gl.Dispose();
                _gl = null;
            }

            return _gl is not null;
        }

        protected void oRenderGraphics(Graphics g)
        {
            if (_gl is not null)
            {
                IntPtr hdc = g.GetHdc();
                oRenderHdc(hdc);
                g.ReleaseHdc(hdc);
            }
            else
            {
                zPaintErrorImage(g, _size);
            }
        }

        protected void oRenderHdc(IntPtr hdc)
        {
            if (_gl is not null)
            {
                _gl.MakeCurrent();

                _gl.Clear((uint)(AttributeMask.ColorBuffer | AttributeMask.DepthBuffer));
                _gl.MatrixMode(MatrixMode.Modelview);
                _gl.LoadIdentity();

                _camera.Apply(_gl);

                if (_defaultLight is not null)
                {
                    _defaultLight.TurnOn(_gl);
                    _defaultLight.Render(_gl, Color.Orange);
                }

                _lights.ForEach(o =>
                {
                    o.TurnOn(_gl);
                    o.Render(_gl, Color.Orange);
                });

                _objects.Render(_gl);

                _gl.Blit(hdc);
            }
            else
            {
                // Rien...
            }
        }

        protected void oSetBackColor(HColor color)
        {
            _backColor = color;
            _gl?.ClearColor(_backColor.red / 255f, _backColor.green / 255f, _backColor.blue / 255f, 0);
        }

        protected void oSetRenderDimensions(SizeI sz, float fovx)
        {
            if (_gl is not null)
            {
                _gl.SetDimensions(sz.w, sz.h);
                _gl.Viewport(0, 0, sz.w, sz.h);
                _gl.MatrixMode(MatrixMode.Projection);
                _gl.LoadIdentity();
                // OpenGL veut un fov en Y, c'est pas très pratique je trouve alors on le garde en X et on lui donne en Y
                _gl.Perspective(zFovXToFovY(fovx, sz) * MathF.RadToDeg, sz.w / (double)sz.h, _nearestDistance, _farestDistance);
                _gl.MatrixMode(MatrixMode.Modelview);
                _gl.LoadIdentity();
            }
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static float zFovXToFovY(float fovX, SizeI viewportSize)
        {
            float dist = viewportSize.w / MathF.Tan(fovX / 2) / 2;
            float alpha = MathF.Atan(viewportSize.h / dist / 2) * 2;

            return alpha;
        }

        private void zPaintErrorImage(Graphics g, SizeI sz)
        {
            StdGraphic stdG = new StdGraphic(g);
            stdG.FillRect(RectI.FromSize(sz), Brushes.Black);
            stdG.SetFont("Arial", 22, FontStyle.Regular);
            stdG.PrintIn("3D View unavailable." + Environment.NewLine + "OpenGL not installed or outdated.", RectI.FromSize(sz), Color.Red, ContentAlignment.MiddleCenter);
            stdG.ResetFont();
            stdG.Dispose();
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}