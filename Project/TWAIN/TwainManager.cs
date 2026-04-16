using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    public interface ITwainImageReceiver : IMessageFilter
    {
        // ***************************************************************************************************
        // 16.03.23 : Création
        // ***************************************************************************************************

        delegate void ImageReceivedEventHandler(Bitmap bmp, bool userCancel);

        event ImageReceivedEventHandler ImageReceived;
    }

    public class TwainManager : Citizen, ITwainImageReceiver
    {
        // ***************************************************************************************************
        // 08.03.23 : Création
        // 09.05.23 : Ajout de zIsLibraryInstalled pour éviter une exception lorsque les bibliothèques TWAIN
        //            ne sont pas installées sur le poste.
        // 20.03.24 : Suppression de l'instanciation spécialisée des DataSources.
        // ***************************************************************************************************

        public event ITwainImageReceiver.ImageReceivedEventHandler ImageReceived;

        // ***************************************************************************************************

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")] private extern static int zFreeLibrary(IntPtr module);
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode)] private extern static IntPtr zLoadLibrary(string fileName);


        private const TWAIN.TWCY KCountry = TWAIN.TWCY.USA;
        private const TWAIN.TWLG KLanguage = TWAIN.TWLG.ENGLISH_USA;


        private TwainDataSources _dataSources;
        private TwainThread _thread;
        private IntPtr _hWnd;
        private TWAIN _dsm;


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
            _dsm.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref _hWnd);

            _dsm.Dispose();
            _dsm = null;
        }

        public void EnumerateDS()
        {
            _dataSources.Clear();

            TWAIN.TW_IDENTITY id = default;
            for (TWAIN.STS sts = _dsm.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETFIRST, ref id); sts == TWAIN.STS.SUCCESS; sts = _dsm.DatIdentity(TWAIN.DG.CONTROL, TWAIN.MSG.GETNEXT, ref id))
            {
                TwainDataSource ds = new TwainDataSource(id);

                if (ds.Open(_dsm, _thread, this))
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

        public TwainDataSource OpenDS(int no)
        {
            return _dataSources.Open(no, _dsm, _thread, this);
        }

        public bool OpenDSM()
        {
            bool ok = zIsLibraryInstalled();

            if (ok)
            {
                // Attention les chaines sont sur 32 char max sinon crash

                _dsm = new TWAIN("Microvision",
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
                    null,
                    zScanCallback,
                    _thread.RunInUIThread,
                    _hWnd);

                ok = _dsm.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.OPENDSM, ref _hWnd) == TWAIN.STS.SUCCESS;
                if (ok)
                {
                    TWAIN.TW_ENTRYPOINT entryPoint = default;
                    entryPoint.Size = (uint)Marshal.SizeOf(entryPoint);
                    ok = (_dsm.DatEntrypoint(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref entryPoint) == TWAIN.STS.SUCCESS);

                    if (!ok) _dsm.DatParent(TWAIN.DG.CONTROL, TWAIN.MSG.CLOSEDSM, ref _hWnd);
                }

                if (!ok)
                {
                    _dsm.Dispose();
                    _dsm = null;
                }
            }

            return ok;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_dsm is not null)
            {
                if (isExplicit) _dsm.Dispose();
                _dsm = null;
            }

            _hWnd = IntPtr.Zero;

            if (_thread is not null)
            {
                if (isExplicit) _thread.Dispose();
                _thread = null;
            }

            if (_dataSources is not null)
            {
                if (isExplicit) _dataSources.Dispose();
                _dataSources = null;
            }

            base.oDispose(isExplicit);
        }

        protected void oOnImageReceived(Bitmap bmp, bool userCancel)
        {
            this.ImageReceived?.Invoke(bmp, userCancel);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static void zFillBitmap(in TWAIN.TW_IMAGEINFO info, in TWAIN.TW_IMAGEMEMXFER xfer, IntPtr src, Bitmap bmp)
        {
            int bytesPerRow = (int)xfer.Columns * info.BitsPerPixel / 8;    // peut-être différent de xfer.BytesPerRow pour des raisons d'alignement.

            // La structure TWAIN.TW_IMAGEMEMXFER contient une partie de l'image
            // qui peut être une "strip" ou une "tile".
            // Notre scanner EPSON Perfection V850 Pro nous renvoie des "strips".
            // Le test ci-dessous permet de détecter une "strip".
            // cf. TWAIN 2.5 page 336 / 766.
            if ((xfer.XOffset == 0) && (xfer.Columns == info.ImageWidth))
            {
                BitmapData data = bmp.LockBits(new Rectangle(0, (int)xfer.YOffset, bmp.Width, (int)xfer.Rows), ImageLockMode.WriteOnly, bmp.PixelFormat);

                for (int row = 0; row < xfer.Rows; row++)
                {
                    if (info.PixelType == (short)TWAIN.TWPT.RGB)
                        QuickShop.RGBToBGR(src + row * (int)xfer.BytesPerRow, data.Scan0 + row * data.Stride, 3, (int)xfer.Columns);
                    else
                        KernelShop.Copy(src + row * (int)xfer.BytesPerRow, data.Scan0 + row * data.Stride, (int)xfer.Columns);
                }

                bmp.UnlockBits(data);
            }
        }

        private static bool zIsLibraryInstalled()
        {
            IntPtr module = zLoadLibrary("twaindsm.dll");
            bool ok = (module != IntPtr.Zero);

            if (ok) zFreeLibrary(module);

            return ok;
        }

        private static bool zMakeBitmap(in TWAIN.TW_IMAGEINFO info, out Bitmap bmp)
        {
            bmp = null;

            if (info.PixelType == (short)TWAIN.TWPT.RGB)
            {
                try
                {
                    bmp = new Bitmap(info.ImageWidth, info.ImageLength, PixelFormat.Format24bppRgb);
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                    bmp = null;
                }
            }
            else if (info.PixelType == (short)TWAIN.TWPT.GRAY)
            {
                try
                {
                    bmp = new Bitmap(info.ImageWidth, info.ImageLength, PixelFormat.Format8bppIndexed);
                }
                catch (ArgumentException)    // survient lorsque l'image est trop lourde.
                {
                    bmp = null;
                }

                if (bmp is not null)
                {
                    ColorPalette palette = bmp.Palette;
                    Enumerable.Range(0, palette.Entries.Length).ToList().ForEach(o => palette.Entries[o] = Color.FromArgb(o, o, o));
                    bmp.Palette = palette;
                }
            }

            bmp?.SetResolution(info.XResolution.Get(), info.YResolution.Get());

            return bmp is not null;
        }

        private TWAIN.STS zScanCallback(bool closing)
        {
            bool ok = true;
            bool userCancel = false;
            Bitmap bmp = null;
            TWAIN.TW_IMAGEINFO info = default;
            TWAIN.TW_IMAGELAYOUT layout = default;
            TWAIN.TW_SETUPMEMXFER setup = default;
            ok = ok && (_dsm.DatImageinfo(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref info) == TWAIN.STS.SUCCESS);
            ok = ok && (_dsm.DatImagelayout(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref layout) == TWAIN.STS.SUCCESS);
            ok = ok && (_dsm.DatSetupmemxfer(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref setup) == TWAIN.STS.SUCCESS);

            if (ok)
            {
                IntPtr memHandle = _dsm.DsmMemAlloc(setup.Preferred);
                ok = (memHandle != IntPtr.Zero);
                if (ok)
                {
                    IntPtr memPointer = _dsm.DsmMemLock(memHandle);

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
                        status = _dsm.DatImagememxfer(TWAIN.DG.IMAGE, TWAIN.MSG.GET, ref xfer);
                        userCancel = (status == TWAIN.STS.CANCEL);

                        // Le test ci-dessous n'est pas décrit dans la spec.
                        // Il est devenu nécessaire avec le scanner ESPON Perfection V850 Pro,
                        // pour éviter une boucle folle lorsque l'utilisateur appuie sur "Annuler"
                        // alors que le scanner est en train d'acquérir une image
                        // (le problème survient après que la tête du scanner ait atteint le début de l'image à acquérir.)
                        if ((status == TWAIN.STS.SUCCESS) && (xfer.BytesWritten == 0))
                        {
                            TWAIN.TW_PENDINGXFERS p = default;
                            _dsm.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref p);
                        }

                        if (((status == TWAIN.STS.SUCCESS) || (status == TWAIN.STS.XFERDONE)) && (xfer.BytesWritten > 0))
                        {
                            ok = (bmp is not null) || zMakeBitmap(info, out bmp);

                            if (ok) zFillBitmap(info, xfer, memPointer, bmp);
                        }
                    } while (ok && (status == TWAIN.STS.SUCCESS));

                    ok = ok && (status == TWAIN.STS.XFERDONE);

                    _dsm.DsmMemUnlock(memHandle);
                    _dsm.DsmMemFree(ref memHandle);
                }
            }

            TWAIN.TW_PENDINGXFERS pending = default;
            _dsm.DatPendingxfers(TWAIN.DG.CONTROL, TWAIN.MSG.ENDXFER, ref pending);

            oOnImageReceived(ok ? bmp : null, userCancel);

            if (bmp is not null)
            {
                bmp.Dispose();
                bmp = null;
            }

            return ok ? TWAIN.STS.SUCCESS : TWAIN.STS.FAILURE;
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

        bool IMessageFilter.PreFilterMessage(ref Message m) => _dsm.PreFilterMessage(m.HWnd, m.Msg, m.WParam, m.LParam);


    }
}