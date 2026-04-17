using Microvision.Types;

namespace Microvision.Avi
{
    public class AVIStream : Citizen
    {
        // ***************************************************************************************************
        // 01.10.01 : (ChB) sous-ensemble d'un AVIFile, de type video ou audio ou autro, identifié par un
        //            handle, contenant une en-tête (AVIStreamInfo) et des samples (iAVISample).
        //            Dans le cas de la video, iAVISample est implémentée par AVIImage. Toujours dans le
        //            cas de la vidéo, la librairie prévoit en plus laconversion directe d'un sample en Dib.
        // 05.03.10 : traduction VBNet. Pour des questions de marshaling, suppression de iAVISample.
        // 14.06.11 : libs 1.8
        // 05.02.14 : libs 2.0, intégration à µV.Platform.
        // 25.03.15 : typages plus rigoureux, à la suite de AVILib (_handle as Intptr, _lastError as AVIError).
        //            Changement de patchouille dans zReadFrame car la précédente ne marche plus.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 25.01.21 : Correction d'un pointeur en int
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private AVILib _lib;

        private IntPtr _handle;
        private AVILib.AVIStreamInfo _info;
        private AVILib.AVIError _lastError;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AVIStream(AVILib avilib, IntPtr hdl) : base()
        {
            _lib = avilib;
            _handle = hdl;
            _lastError = _lib.ReadStreamInfo(_handle, ref _info);
        }

        public AVIStream(AVILib avilib, IntPtr hdl, AVILib.AVIStreamInfo stinf) : this(avilib, hdl)
        {
            // -- le stream doit être éditable...

            _lastError = _lib.WriteStreamInfo(_handle, stinf);
            _lastError = _lib.ReadStreamInfo(_handle, ref _info);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public float Frequency
        {
            get => _info.Frequency;
            set => _info.Frequency = value;
        }

        public IntPtr Handle => _handle;

        public AVILib.AVIStreamInfo Info => _info;

        public int LastError => (int)_lastError & 0xFFF;

        public int SamplesCount => _lib.GetStreamLength(_handle);

        public Size SampleSize => _info.Size;

        public int SampleStart => _lib.GetStreamStart(_handle);


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public AVIStream CreateCompressedStream(AVILib.AviCompressOptions opts)
        {
            IntPtr hdl = IntPtr.Zero;
            AVIStream output = null;

            // -- à déplacer vers AVIFile ?

            _lastError = _lib.CreateCompressedStream(_handle, opts, ref hdl);
            if (_lastError == AVILib.AVIError.AVIERR_OK)
                output = new AVIStream(_lib, hdl);

            return output;
        }

        public AVIStream CreateEditableStream()
        {
            IntPtr hdl = IntPtr.Zero;
            AVIStream output = null;

            // -- à déplacer vers AVIFile ?

            _lastError = _lib.CreateEditableStream(_handle, ref hdl);
            if (_lastError == AVILib.AVIError.AVIERR_OK)
                output = new AVIStream(_lib, hdl, _info);

            return output;
        }

        public int FindPreviousKeyFrame(int sampleNo)
        {
            return _lib.FindSampleKey(_handle, sampleNo);
        }

        public AVIImage GetFormat()
        {
            AVIImage img = null;
            NativeMethods.Gdi32.BITMAPINFO bmi = default;

            switch (_info.FCCTypeString.ToLower())
            {
                case "vids":
                    _lastError = _lib.ReadVideoFormat(_handle, ref bmi);
                    if (_lastError == AVILib.AVIError.AVIERR_OK) img = new AVIImage(ref bmi);
                    break;
            }

            return img;
        }

        public bool IsKey(int sampleNo)
        {
            return _lib.FindSampleKey(_handle, sampleNo) == sampleNo;
        }

        public AVIImage ReadSample(AVIImage img, int sampleNo)
        {
            AVIImage output = null;

            _lastError = _lib.ReadVideoSample(_handle, sampleNo, img);
            if (_lastError == AVILib.AVIError.AVIERR_OK) output = img;

            return output;
        }

        public BasicDibApi ReadSampleDib(BasicDibApi dib, int sampleNo)
        {
            bool ok = false;
            IntPtr hfrm = _lib.OpenFrame(_handle);

            if (hfrm != IntPtr.Zero)
            {
                ok = zReadFrame(_lib, hfrm, sampleNo, dib);
                _lib.CloseFrame(hfrm);
            }

            if (!ok) dib = null;

            return dib;
        }

        public bool Save(string fname, AVILib.AVISaveCallback prghdlr, AVILib.AviCompressOptions copt)
        {
            bool ok = false;

            if (copt.IsDefined)
                _lastError = _lib.SaveStream(_handle, fname, prghdlr, ref copt);
            else
                _lastError = _lib.SaveStream(_handle, fname, prghdlr);

            switch (_lastError)
            {
                case AVILib.AVIError.AVIERR_OK:
                    ok = true;
                    break;

                case AVILib.AVIError.AVIERR_MEMORY:
                    // -- erreur qui se produit quasi à chaque coup avec certaines caméras,
                    // jamais avec d'autres, et qu'apparemment c'est pas grave d'ignorer.
                    ok = true;
                    break;

                case AVILib.AVIError.AVIERR_FILEREAD:
                    // -- erreur constatée assez systématiquement avec certaines tailles d'image et 
                    // sans compression, bien que le fichier soit valide.
                    FileInfo info = new FileInfo(fname);
                    if (info.Exists) ok = info.Length > 0;
                    break;
            }

            return ok;
        }

        public bool SaveOptionsDlg(IWin32Window prnt, ref AVILib.AviCompressOptions opts)
        {
            return _lib.ShowSaveOptionsDlg(prnt, _handle, ref opts);
        }

        public bool SetFormat(AVIImage img)
        {
            NativeMethods.Gdi32.BITMAPINFO info = img.Header;
            _lastError = _lib.SetVideoFormat(_handle, ref info);

            return _lastError == AVILib.AVIError.AVIERR_OK;
        }

        public bool WriteSample(int sampleNo, AVIImage img, bool iskey)
        {
            // -- Rem : en vidéo, seule l'écriture en fin de stream est possible...

            _lastError = _lib.WriteVideoSample(_handle, sampleNo, img, iskey);
            return _lastError == AVILib.AVIError.AVIERR_OK;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _info = default;
            if (_lib is not null)
            {
                if (_handle != IntPtr.Zero)
                {
                    _lib.CloseStream(_handle);
                    _handle = IntPtr.Zero;
                }

                _lib = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static bool zReadFrame(AVILib lb, IntPtr hfrm, int sampleNo, BasicDibApi dst)
        {
            // -- AVIStreamGetFrame ne marche pas toujours, notamment avec le dernier échantillon. En 32 bits, 
            // une seconde lecture suffisait, ce n'est plus le cas. Par contre lire un autre échantillon 
            // semble débloquer la situation...

            bool ok = false;
            IntPtr srcad = lb.ReadFrame(hfrm, sampleNo);

            if (srcad == IntPtr.Zero && sampleNo > 0)
            {
                srcad = lb.ReadFrame(hfrm, sampleNo - 1);
                srcad = lb.ReadFrame(hfrm, sampleNo);
            }

            if (srcad != IntPtr.Zero)
            {
                dst.SetCompactHandle(srcad);
                ok = true;
            }

            return ok;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}