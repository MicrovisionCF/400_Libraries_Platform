using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microvision.Geometry;
using Microvision.NativeMethods;
using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    public interface ITwainImageReceiver : IMessageFilter
    {
        // ***************************************************************************************************
        // 16.03.23 : Création
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        delegate void ImageReceivedEventHandler(Bitmap? image, bool userCancel);

        event ImageReceivedEventHandler? ImageReceived;
    }

    public class TwainManager : Citizen, ITwainImageReceiver
    {
        // ***************************************************************************************************
        // 08.03.23 : Création
        // 09.05.23 : Ajout de zIsLibraryInstalled pour éviter une exception lorsque les bibliothèques TWAIN
        //            ne sont pas installées sur le poste.
        // 20.03.24 : Suppression de l'instanciation spécialisée des DataSources.
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        public event ITwainImageReceiver.ImageReceivedEventHandler? ImageReceived;

        // ***************************************************************************************************


        private const TWAIN.TWCY KCountry = TWAIN.TWCY.USA;
        private const TWAIN.TWLG KLanguage = TWAIN.TWLG.ENGLISH_USA;


        private readonly TwainDataSources _dataSources;
        private readonly TwainThread _thread;

        private IntPtr _hWnd;
        private TWAIN? _dataSourceManager;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainManager() : base()
        {
            _dataSources = new TwainDataSources();
            _thread = new TwainThread();
            _hWnd = _thread.HWnd;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int DevicesCount => _dataSources.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void CloseDSM()
        {
            _dataSourceManager.ThrowIfNull();

            _dataSourceManager.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref _hWnd);

            _dataSourceManager.Dispose();
            _dataSourceManager = null;
        }

        public void EnumerateDS()
        {
            _dataSourceManager.ThrowIfNull();

            _dataSources.Clear();

            TWAIN.TW_IDENTITY id = default;
            for (TWAIN.STS sts = _dataSourceManager.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETFIRST, ref id); sts == TWAIN.STS.SUCCESS; sts = _dataSourceManager.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETNEXT, ref id))
            {
                TwainDataSource ds = new TwainDataSource(id);

                if (ds.Open(_dataSourceManager, _thread, this))
                {
                    _dataSources.Add(new TwainDataSource(id).GiveLife());

                    ds.Close();
                }

                ds.Dispose();
            }
        }

        public string GetDeviceProductName(int no)
        {
            return _dataSources.GetProductName(no);
        }

        public TwainDataSource? OpenDS(int no)
        {
            _dataSourceManager.ThrowIfNull();

            return _dataSources.Open(no, _dataSourceManager, _thread, this);
        }

        public bool OpenDSM()
        {
            _dataSourceManager.ThrowIfNotNull();

            bool ok = zIsLibraryInstalled();

            if (ok)
            {
                // Attention les chaines sont sur 32 char max sinon crash

                _dataSourceManager = new TWAIN("Microvision",
                    MakinShop.GetAppName(),
                    MakinShop.GetAppName(),
                    (ushort)TWAIN.TWON_PROTOCOL.MAJOR,
                    (ushort)TWAIN.TWON_PROTOCOL.MINOR,
                    (uint)(TWAIN.DG.APP2 | TWAIN.DG.IMAGE | TWAIN.DG.CONTROL),
                    KCountry,
                    MakinShop.GetAppName() + " " + MakinShop.GetAppVersion(),
                    KLanguage,
                    ushort.Parse(MakinShop.GetAppVersion().Split('.')[0]),
                    ushort.Parse(MakinShop.GetAppVersion().Split('.')[1]),
                    false,
                    true,
                    null, // TODONEXTGEN à résoudre
                    oScanCallback,
                    _thread.RunInUIThread,
                    _hWnd);

                ok = _dataSourceManager.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDSM, ref _hWnd) == TWAIN.STS.SUCCESS;
                if (ok)
                {
                    TWAIN.TW_ENTRYPOINT entryPoint = default;
                    entryPoint.Size = (uint)Marshal.SizeOf(entryPoint);
                    ok = (_dataSourceManager.DatEntrypoint(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref entryPoint) == TWAIN.STS.SUCCESS);

                    if (!ok) _dataSourceManager.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref _hWnd);
                }

                if (!ok)
                {
                    _dataSourceManager.Dispose();
                    _dataSourceManager = null;
                }
            }

            return ok;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_dataSourceManager is not null)
            {
                if (isExplicit) _dataSourceManager.Dispose();
                _dataSourceManager = null;
            }

            _hWnd = IntPtr.Zero;

            if (isExplicit) _thread.Dispose();

            if (isExplicit) _dataSources.Dispose();

            base.oDispose(isExplicit);
        }

        protected void oOnImageReceived(Bitmap? bmp, bool userCancel)
        {
            this.ImageReceived?.Invoke(bmp, userCancel);
        }

        protected TWAIN.STS oScanCallback(bool closing)
        {
            _dataSourceManager.ThrowIfNull();

            bool ok = true;
            bool userCancel = false;
            Bitmap? bmp = null;
            TWAIN.TW_IMAGEINFO info = default;
            TWAIN.TW_IMAGELAYOUT layout = default;
            TWAIN.TW_SETUPMEMXFER setup = default;
            ok = ok && (_dataSourceManager.DatImageinfo(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref info) == TWAIN.STS.SUCCESS);
            ok = ok && (_dataSourceManager.DatImagelayout(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref layout) == TWAIN.STS.SUCCESS);
            ok = ok && (_dataSourceManager.DatSetupmemxfer(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref setup) == TWAIN.STS.SUCCESS);

            if (ok)
            {
                IntPtr memHandle = _dataSourceManager.DsmMemAlloc(setup.Preferred);
                ok = (memHandle != IntPtr.Zero);
                if (ok)
                {
                    IntPtr memPointer = _dataSourceManager.DsmMemLock(memHandle);

                    TWAIN.STS status;
                    do
                    {
                        TWAIN.TW_IMAGEMEMXFER xfer;
                        xfer.Compression = TWAIN.TWON_DONTCARE16;
                        xfer.BytesPerRow = TWAIN.TWON_DONTCARE32;
                        xfer.Columns = TWAIN.TWON_DONTCARE32;
                        xfer.Rows = TWAIN.TWON_DONTCARE32;
                        xfer.XOffset = TWAIN.TWON_DONTCARE32;
                        xfer.YOffset = TWAIN.TWON_DONTCARE32;
                        xfer.BytesWritten = TWAIN.TWON_DONTCARE32;
                        xfer.Memory.Flags = (uint)(TWAIN.TWMF.APPOWNS | TWAIN.TWMF.POINTER);
                        xfer.Memory.Length = setup.Preferred;
                        xfer.Memory.TheMem = memPointer;
                        status = _dataSourceManager.DatImagememxfer(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref xfer);
                        userCancel = (status == TWAIN.STS.CANCEL);

                        // Le test ci-dessous n'est pas décrit dans la spec.
                        // Il est devenu nécessaire avec le scanner ESPON Perfection V850 Pro,
                        // pour éviter une boucle folle lorsque l'utilisateur appuie sur "Annuler"
                        // alors que le scanner est en train d'acquérir une image
                        // (le problème survient après que la tête du scanner ait atteint le début de l'image à acquérir.)
                        if ((status == TWAIN.STS.SUCCESS) && (xfer.BytesWritten == 0))
                        {
                            TWAIN.TW_PENDINGXFERS p = default;
                            _dataSourceManager.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref p);
                        }

                        if (((status == TWAIN.STS.SUCCESS) || (status == TWAIN.STS.XFERDONE)) && (xfer.BytesWritten > 0))
                        {
                            if ((bmp is not null) || zMakeBitmap(info, out bmp))
                            {
                                zFillBitmap(info, xfer, memPointer, bmp);
                                ok = true;
                            }
                            else
                            {
                                ok = false;
                            }
                        }
                    } while (ok && (status == TWAIN.STS.SUCCESS));

                    ok = ok && (status == TWAIN.STS.XFERDONE);

                    _dataSourceManager.DsmMemUnlock(memHandle);
                    _dataSourceManager.DsmMemFree(ref memHandle);
                }
            }

            TWAIN.TW_PENDINGXFERS pending = default;
            _dataSourceManager.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref pending);

            oOnImageReceived(ok ? bmp : null, userCancel);

            bmp?.Dispose();

            return ok ? TWAIN.STS.SUCCESS : TWAIN.STS.FAILURE;
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static void zFillBitmap(in TWAIN.TW_IMAGEINFO info, in TWAIN.TW_IMAGEMEMXFER xfer, IntPtr src, Bitmap image)
        {
            // La structure TWAIN.TW_IMAGEMEMXFER contient une partie de l'image
            // qui peut être une "strip" ou une "tile".
            // Notre scanner EPSON Perfection V850 Pro nous renvoie des "strips".
            // Le test ci-dessous permet de détecter une "strip".
            // cf. TWAIN 2.5 page 336 / 766.
            if ((xfer.XOffset == 0) && (xfer.Columns == info.ImageWidth))
            {
                BitmapData data = image.LockBits(new RectI(0, (int)xfer.YOffset, image.Width, (int)xfer.Rows), ImageLockMode.WriteOnly, image.PixelFormat);

                for (int row = 0; row < xfer.Rows; row++)
                {
                    if (info.PixelType == (short)TWAIN.TWPT.RGB)
                        QuickShop.RGBToBGR(src + row * (int)xfer.BytesPerRow, data.Scan0 + row * data.Stride, 3, (int)xfer.Columns);
                    else
                        KernelShop.Copy(src + row * (int)xfer.BytesPerRow, data.Scan0 + row * data.Stride, (int)xfer.Columns);
                }

                image.UnlockBits(data);
            }
        }

        private static bool zIsLibraryInstalled()
        {
            IntPtr module = Kernel32.LoadLibraryW("twaindsm.dll");
            bool ok = (module != IntPtr.Zero);

            if (ok) Kernel32.FreeLibrary(module);

            return ok;
        }

        private static bool zMakeBitmap(in TWAIN.TW_IMAGEINFO info, [NotNullWhen(true)] out Bitmap? image)
        {
            image = null;

            if (info.PixelType == (short)TWAIN.TWPT.RGB)
            {
                try
                {
                    image = new Bitmap(info.ImageWidth, info.ImageLength, PixelFormat.Format24bppRgb);
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                    image = null;
                }
            }
            else if (info.PixelType == (short)TWAIN.TWPT.GRAY)
            {
                try
                {
                    image = new Bitmap(info.ImageWidth, info.ImageLength, PixelFormat.Format8bppIndexed);
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                    image = null;
                }

                if (image is not null)
                {
                    ColorPalette palette = image.Palette;
                    Enumerable.Range(0, palette.Entries.Length).ToList().ForEach(o => palette.Entries[o] = Color.FromArgb(o, o, o));
                    image.Palette = palette;
                }
            }

            image?.SetResolution(info.XResolution.Get(), info.YResolution.Get());

            return image is not null;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

        // ####################################
        // As IMessageFilter
        // ####################################

        bool IMessageFilter.PreFilterMessage(ref Message m) => _dataSourceManager.ThrowIfNull().PreFilterMessage(m.HWnd, m.Msg, m.WParam, m.LParam);


    }
}