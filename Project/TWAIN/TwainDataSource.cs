using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;

using Microvision.Geometry;
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
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public delegate void PhysicalSizeChangedEventHandler(float width, float height);

        public event PhysicalSizeChangedEventHandler? PhysicalSizeChanged;

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


        private readonly Semaphore _receiptCompleteLock;

        private TWAIN.TW_IDENTITY _id;
        private TWAIN? _dataSourceManager;

        private TwainThread? _thread;
        private ITwainImageReceiver? _imageReceiver;
        private Bitmap? _imageReceived;
        private bool _receiptCanceled;

        private TwainCapabilities? _capabilities;
        private RectG _lastFrameSet;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainDataSource(in TWAIN.TW_IDENTITY id) : base()
        {
            _id = id;
            _receiptCompleteLock = new Semaphore(0, 1);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool HasResolutionRange => _capabilities.ThrowIfNull().HasResolutionRange;

        public string ProductName => _id.ProductName.Get();


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool Acquire([NotNullWhen(true)] out Bitmap? image, out bool canceled)
        {
            oThrowIfNotOpened();

            image = null;
            canceled = false;

            _imageReceiver_Attach(true);

            TWAIN.TW_USERINTERFACE ui;
            ui.ShowUI = 0;
            ui.ModalUI = 0;
            ui.hParent = _thread.HWnd;

            if (_dataSourceManager.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.ENABLEDS, ref ui) == TWAIN.STS.SUCCESS)
            {
                _receiptCompleteLock.WaitOne();
                canceled = _receiptCanceled;
                image = _imageReceived;
                _imageReceived = null;

                _dataSourceManager.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.DISABLEDS, ref ui);
            }

            _imageReceiver_Attach(false);

            return image is not null;
        }

        public void Close()
        {
            oThrowIfNotOpened();

            _capabilities_Attach(false);
            _capabilities.Dispose();
            _capabilities = null;

            _dataSourceManager.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref _id);

            _imageReceiver = null;
            _thread = null;
            _dataSourceManager = null;
        }

        public float GetDefaultGamma()
        {
            return oGetDefaultGamma();
        }

        public RectG GetFrame()
        {
            oThrowIfNotOpened();

            return _capabilities.GetFrame();
        }

        public float GetPhysicalHeight()
        {
            oThrowIfNotOpened();

            return _capabilities.PhysicalHeight;
        }

        public float GetPhysicalWidth()
        {
            oThrowIfNotOpened();

            return _capabilities.PhysicalWidth;
        }

        public PixelType GetPixelType()
        {
            oThrowIfNotOpened();

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
            oThrowIfNotOpened();

            return _capabilities.GetResolution();
        }

        public (float minX, float maxX, float minY, float maxY) GetResolutionRange()
        {
            oThrowIfNotOpened();

            return _capabilities.GetResolutionRange();
        }

        public LightPath GetSupportedLightPaths()
        {
            oThrowIfNotOpened();

            List<TWAIN.TWLP> lightPaths = _capabilities.GetSupportedLightPaths();

            LightPath supported = 0;
            if (lightPaths.Contains(TWAIN.TWLP.REFLECTIVE)) supported |= LightPath.Reflective;
            if (lightPaths.Contains(TWAIN.TWLP.TRANSMISSIVE)) supported |= LightPath.Transmissive;

            return supported;
        }

        public PixelType GetSupportedPixelTypes()
        {
            oThrowIfNotOpened();

            List<TWAIN.TWPT> pixelTypes = _capabilities.GetSupportedPixelTypes();

            PixelType supported = 0;
            if (pixelTypes.Contains(TWAIN.TWPT.GRAY)) supported |= PixelType.Gray;
            if (pixelTypes.Contains(TWAIN.TWPT.RGB)) supported |= PixelType.RGB;

            return supported;
        }

        public bool Open(TWAIN dataSourceManager, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            return oOpen(dataSourceManager, thread, imageReceiver);
        }

        public void SetFrame(RectG frame)
        {
            oSetFrame(frame);
        }

        public void SetGamma(float gamma)
        {
            oSetGamma(gamma);
        }

        public void SetLightPath(LightPath lightPath)
        {
            oThrowIfNotOpened();

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
            oThrowIfNotOpened();

            if (pixelType.HasFlag(PixelType.RGB)) _capabilities.SetPixelType(TWAIN.TWPT.RGB);
            else if (pixelType.HasFlag(PixelType.Gray)) _capabilities.SetPixelType(TWAIN.TWPT.GRAY);
        }

        public void SetResolution(float resX, float resY)
        {
            oThrowIfNotOpened();

            _capabilities.SetResolution(resX, resY);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _imageReceiver = null;
            _thread = null;
            _dataSourceManager = null;

            if (isExplicit) _receiptCompleteLock.Dispose();

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
            oThrowIfNotOpened();

            return _capabilities.GetDefaultGamma();
        }

        [MemberNotNull(nameof(_dataSourceManager), nameof(_capabilities), nameof(_imageReceiver), nameof(_thread))]
        protected void oThrowIfNotOpened()
        {
            _dataSourceManager.ThrowIfNull();
            _capabilities.ThrowIfNull();
            _imageReceiver.ThrowIfNull();
            _thread.ThrowIfNull();
        }

        protected virtual bool oOpen(TWAIN dsm, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            bool ok = false;

            _dataSourceManager = dsm;
            _thread = thread;
            _imageReceiver = imageReceiver;

            if (_dataSourceManager.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDS, ref _id) == TWAIN.STS.SUCCESS)
            {
                if (TwainCapabilities.CheckSupportedCaps(_dataSourceManager))
                {
                    _capabilities = new TwainCapabilities(_dataSourceManager);
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

                if (!ok) _dataSourceManager.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDS, ref _id);
            }

            if (!ok)
            {
                _imageReceiver = null;
                _thread = null;
                _dataSourceManager = null;
            }

            return ok;
        }

        protected void oSetFrame(RectG frame)
        {
            oThrowIfNotOpened();

            _lastFrameSet = frame;
            _capabilities.SetFrame(_lastFrameSet);
        }

        protected void oSetGamma(float gamma, bool force = false)
        {
            oThrowIfNotOpened();

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
            _capabilities.ThrowIfNull();

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
            _imageReceiver.ThrowIfNull();
            _thread.ThrowIfNull();

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

        private void _imageReceiver_ImageReceived(Bitmap? image, bool userCanceled)
        {
            _imageReceived?.Dispose();
            _imageReceived = null;

            if (image is not null)
            {
                try
                {
                    _imageReceived = (Bitmap)image.Clone();
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                }
            }

            _receiptCanceled = userCanceled;
            _receiptCompleteLock.Release();
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}