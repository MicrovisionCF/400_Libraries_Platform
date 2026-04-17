using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    public class TwainDataSource : Citizen
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // 20.03.24 : Ajout de l'écriture du gamma par défaut à l'ouverture du scanner pour compenser
        //            l'image saturée que produisent les scanners EPSON.
        // 13.05.24 : Synchronisation des surfaces à scanner indépendamment du mode de fonctionnement
        //            opaque/transparent (en interne, les scanners EPSON semblent avoir 2 espaces mémoires
        //            différents où ils stockent cette surface.)
        // ***************************************************************************************************

        public delegate void PhysicalSizeChangedEventHandler(float width, float height);

        public event PhysicalSizeChangedEventHandler PhysicalSizeChanged;

        // ***************************************************************************************************

        [Flags]
        public enum LightPath
        {
            Reflective = 1,
            Transmissive = 2,
        }

        [Flags]
        public enum PixelType
        {
            Gray = 1,
            RGB = 2,
        }


        private TWAIN.TW_IDENTITY _id;
        private TWAIN _dsm;

        private TwainThread _thread;
        private ITwainImageReceiver _imageReceiver;
        private Bitmap _imageReceived;
        private bool _receiptCanceled;
        private Semaphore _receiptComplete;

        private TwainCapabilities _capabilities;
        private RectangleF _lastFrameSet;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainDataSource(in TWAIN.TW_IDENTITY id) : base()
        {
            _id = id;
            _receiptComplete = new Semaphore(0, 1);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool HasResolutionRange => _capabilities.HasResolutionRange;

        public string ProductName => _id.ProductName.Get();


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool Acquire(out Bitmap bmp, out bool canceled)
        {
            bmp = null;
            canceled = false;

            _imageReceiver_Attach(true);

            TWAIN.TW_USERINTERFACE ui;
            ui.ShowUI = 0;
            ui.ModalUI = 0;
            ui.hParent = _thread.HWnd;

            if (_dsm.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.ENABLEDS, ref ui) == TWAIN.STS.SUCCESS)
            {
                _receiptComplete.WaitOne();
                canceled = _receiptCanceled;
                bmp = _imageReceived;
                _imageReceived = null;

                _dsm.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref ui);
            }

            _imageReceiver_Attach(false);

            return bmp is not null;
        }

        public void Close()
        {
            _capabilities_Attach(false);
            _capabilities.Dispose();
            _capabilities = null;

            _dsm.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref _id);

            _imageReceiver = null;
            _thread = null;
            _dsm = null;
        }

        public float GetDefaultGamma()
        {
            return oGetDefaultGamma();
        }

        public RectangleF GetFrame()
        {
            return _capabilities.GetFrame();
        }

        public float GetPhysicalHeight()
        {
            return _capabilities.PhysicalHeight;
        }

        public float GetPhysicalWidth()
        {
            return _capabilities.PhysicalWidth;
        }

        public PixelType GetPixelType()
        {
            TWAIN.TWPT pixelType = _capabilities.GetPixelType();

            PixelType output = pixelType switch
            {
                TWAIN.TWPT.GRAY => PixelType.Gray,
                _ => PixelType.RGB
            };

            return output;
        }

        public (float resX, float resY) GetResolution()
        {
            return _capabilities.GetResolution();
        }

        public (float minX, float maxX, float minY, float maxY) GetResolutionRange()
        {
            return _capabilities.GetResolutionRange();
        }

        public LightPath GetSupportedLightPaths()
        {
            List<TWAIN.TWLP> lightPaths = _capabilities.GetSupportedLightPaths();

            LightPath supported = 0;
            if (lightPaths.Contains(TWAIN.TWLP.REFLECTIVE)) supported |= LightPath.Reflective;
            if (lightPaths.Contains(TWAIN.TWLP.TRANSMISSIVE)) supported |= LightPath.Transmissive;

            return supported;
        }

        public PixelType GetSupportedPixelTypes()
        {
            List<TWAIN.TWPT> pixelTypes = _capabilities.GetSupportedPixelTypes();

            PixelType supported = 0;
            if (pixelTypes.Contains(TWAIN.TWPT.GRAY)) supported |= PixelType.Gray;
            if (pixelTypes.Contains(TWAIN.TWPT.RGB)) supported |= PixelType.RGB;

            return supported;
        }

        public bool Open(TWAIN dsm, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            return oOpen(dsm, thread, imageReceiver);
        }

        public void SetFrame(RectangleF frame)
        {
            oSetFrame(frame);
        }

        public void SetGamma(float gamma)
        {
            oSetGamma(gamma);
        }

        public void SetLightPath(LightPath lightPath)
        {
            if (lightPath.HasFlag(LightPath.Reflective)) _capabilities.SetLightPath(TWAIN.TWLP.REFLECTIVE);
            else if (lightPath.HasFlag(LightPath.Transmissive)) _capabilities.SetLightPath(TWAIN.TWLP.TRANSMISSIVE);

            // Scanners EPSON Perfection V850 Pro et EPSON Expression 13000 XL:
            // Modifier la Frame (surface à scanner) pendant que le scanner est en mode opaque
            // ne modifie pas la Frame du mode transparent (et vice-versa.)
            // Les lignes ci-dessous appliquent notre Frame à celle du mode qui vient d'être appliqué.
            if (_lastFrameSet != _capabilities.GetFrame())
            {
                oSetFrame(_lastFrameSet);
            }
        }

        public void SetPixelType(PixelType pixelType)
        {
            if (pixelType.HasFlag(PixelType.RGB)) _capabilities.SetPixelType(TWAIN.TWPT.RGB);
            else if (pixelType.HasFlag(PixelType.Gray)) _capabilities.SetPixelType(TWAIN.TWPT.GRAY);
        }

        public void SetResolution(float resX, float resY)
        {
            _capabilities.SetResolution(resX, resY);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _imageReceiver = null;
            _thread = null;
            _dsm = null;

            if (_receiptComplete is not null)
            {
                if (isExplicit) _receiptComplete.Dispose();
                _receiptComplete = null;
            }

            if (_imageReceived is not null)
            {
                if (isExplicit) _imageReceived.Dispose();
                _imageReceived = null;
            }

            if (_capabilities is not null)
            {
                _capabilities_Attach(false);
                if (isExplicit) _capabilities.Dispose();
                _capabilities = null;
            }

            base.oDispose(isExplicit);
        }

        protected float oGetDefaultGamma()
        {
            return _capabilities.GetDefaultGamma();
        }

        protected virtual bool oOpen(TWAIN dsm, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            bool ok = false;

            _dsm = dsm;
            _thread = thread;
            _imageReceiver = imageReceiver;

            if (_dsm.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDS, ref _id) == TWAIN.STS.SUCCESS)
            {
                if (TwainCapabilities.CheckSupportedCaps(_dsm))
                {
                    _capabilities = new TwainCapabilities(_dsm);
                    _capabilities_Attach(true);

                    _lastFrameSet = _capabilities.GetFrame();

                    // Sans la ligne ci-dessous, les images que retournent les scanners
                    // EPSON Perfection V850 Pro et EPSON Expression 13000 XL sont saturées.
                    // Les tests réalisés font sérieusement penser à un bug des DLLs EPSON.
                    // Le code ci-dessous permet de le contourner en ré-écrivant la valeur par défaut.
                    if (_capabilities.HasGamma) oSetGamma(oGetDefaultGamma(), true);

                    ok = true;

                    if (!ok)
                    {
                        _capabilities_Attach(false);
                        _capabilities.Dispose();
                        _capabilities = null;
                    }
                }

                if (!ok) _dsm.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref _id);
            }

            if (!ok)
            {
                _imageReceiver = null;
                _thread = null;
                _dsm = null;
            }

            return ok;
        }

        protected void oSetFrame(RectangleF frame)
        {
            _lastFrameSet = frame;
            _capabilities.SetFrame(_lastFrameSet);
        }

        protected void oSetGamma(float gamma, bool force = false)
        {
            _capabilities.SetGamma(gamma, force);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _capabilities_Attach(bool attach)
        {
            if (attach)
            {
                _capabilities.PhysicalSizeChanged += _capabilities_PhysicalSizeChanged;
            }
            else
            {
                _capabilities.PhysicalSizeChanged -= _capabilities_PhysicalSizeChanged;
            }
        }

        private void _capabilities_PhysicalSizeChanged(float width, float height)
        {
            this.PhysicalSizeChanged?.Invoke(width, height);
        }

        private void _imageReceiver_Attach(bool attach)
        {
            if (attach)
            {
                _imageReceiver.ImageReceived += _imageReceiver_ImageReceived;
                _thread.SetMessageFilter(true, _imageReceiver);
            }
            else
            {
                _thread.SetMessageFilter(false, _imageReceiver);
                _imageReceiver.ImageReceived -= _imageReceiver_ImageReceived;
            }
        }

        private void _imageReceiver_ImageReceived(Bitmap bmp, bool userCancel)
        {
            _imageReceived?.Dispose();
            _imageReceived = null;

            if (bmp is not null)
            {
                try
                {
                    _imageReceived = (Bitmap)bmp.Clone();
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                }
            }

            _receiptCanceled = userCancel;
            _receiptComplete.Release();
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}