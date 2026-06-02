using System;
using System.Windows.Forms;

using Microvision.NativeMethods;
using Microvision.Types;

namespace Microvision.Avi
{
    public class AVILib : Citizen
    {
        // ***************************************************************************************************
        // 05.03.10 : création, pour rangement des types à Bill implémentées en classes VB6 et maintenant
        //            en structures VBNet.
        // 14.06.11 : libs 1.8
        // 05.02.14 : libs 2.0, intégration à µV.Platform.
        // 25.03.15 : corrections des accès à l'API (Unicode), typages plus rigoureux.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AVILib() : base()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void CloseFile(IntPtr hFile)
        {
            Avifil32.AVIFileRelease(hFile);
        }

        public Avifil32.AVIError CloseFrame(IntPtr hFrm)
        {
            return (Avifil32.AVIError)Avifil32.AVIStreamGetFrameClose(hFrm);
        }

        public void CloseLibrary()
        {
            Avifil32.AVIFileExit();
        }

        public Avifil32.AVIError CloseStream(IntPtr hStream)
        {
            return (Avifil32.AVIError)Avifil32.AVIStreamRelease(hStream);
        }

        public Avifil32.AVIError CreateCompressedStream(IntPtr hSrc, Avifil32.AviCompressOptions opts, ref IntPtr hStream)
        {
            IntPtr hopts = MarshShop.LockStruct(opts);
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIMakeCompressedStream(ref hStream, hSrc, hopts, IntPtr.Zero);
            MarshShop.UnlockStruct<Avifil32.AviCompressOptions>(hopts);

            return erc;
        }

        public Avifil32.AVIError CreateEditableStream(IntPtr hSrc, ref IntPtr hStream)
        {
            return (Avifil32.AVIError)Avifil32.CreateEditableStreamAPI(ref hStream, hSrc);
        }

        public Avifil32.AVIError CreateStream(IntPtr hFile, Avifil32.AVIStreamInfo inf, ref IntPtr hStream)
        {
            IntPtr hinf = MarshShop.LockStruct(inf);
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIFileCreateStreamW(hFile, ref hStream, hinf);
            MarshShop.UnlockStruct<Avifil32.AVIStreamInfo>(hinf);

            return erc;
        }

        public int FindSampleKey(IntPtr hStream, int samno)
        {
            return Avifil32.AVIStreamFindSample(hStream, samno, (int)(Avifil32.AVIFindFlags.FIND_PREV | Avifil32.AVIFindFlags.FIND_KEY));
        }

        public int GetStreamLength(IntPtr hStream)
        {
            return Avifil32.AVIStreamLength(hStream);
        }

        public int GetStreamStart(IntPtr hStream)
        {
            return Avifil32.AVIStreamStart(hStream);
        }

        public Avifil32.AVIError OpenFile(ref IntPtr hFile, string fnam, int ofmode)
        {
            return (Avifil32.AVIError)Avifil32.AVIFileOpenW(ref hFile, fnam, ofmode, 0);
        }

        public IntPtr OpenFrame(IntPtr hStream)
        {
            return Avifil32.AVIStreamGetFrameOpen(hStream, (IntPtr)0);
        }

        public IntPtr OpenFrame(IntPtr hStream, NativeMethods.Gdi32.BITMAPINFOHEADER bmih)
        {
            IntPtr hbmih = MarshShop.LockStruct(bmih);
            IntPtr hfrm = Avifil32.AVIStreamGetFrameOpen(hStream, hbmih);
            MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFOHEADER>(hbmih);

            return hfrm;
        }

        public void OpenLibrary()
        {
            Avifil32.AVIFileInit();
        }

        public Avifil32.AVIError OpenStream(IntPtr hFile, Avifil32.AVIStreamType fcc, int noInType, ref IntPtr hStream)
        {
            return (Avifil32.AVIError)Avifil32.AVIFileGetStream(hFile, ref hStream, fcc, noInType);
        }

        public Avifil32.AVIError ReadFileInfo(IntPtr hFile, out Avifil32.AVIFileInfo inf)
        {
            inf = default;
            IntPtr hinf = MarshShop.LockStruct(Avifil32.AVIFileInfo.Empty());
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIFileInfoW(hFile, hinf, MarshShop.SizeOf(inf));
            inf = MarshShop.UnlockStruct<Avifil32.AVIFileInfo>(hinf);

            return erc;
        }

        public IntPtr ReadFrame(IntPtr hFrm, int samno)
        {
            return Avifil32.AVIStreamGetFrame(hFrm, samno);
        }

        public Avifil32.AVIError ReadStreamInfo(IntPtr hStream, ref Avifil32.AVIStreamInfo inf)
        {
            IntPtr hinf = MarshShop.LockStruct(Avifil32.AVIStreamInfo.Empty());
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIStreamInfoW(hStream, hinf, MarshShop.SizeOf(inf));
            inf = MarshShop.UnlockStruct<Avifil32.AVIStreamInfo>(hinf);

            return erc;
        }

        public Avifil32.AVIError ReadVideoFormat(IntPtr hStream, ref NativeMethods.Gdi32.BITMAPINFO bmi)
        {
            int lng = 0;
            bmi.bmiColors = new int[256];
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIStreamReadFormat(hStream, 0, IntPtr.Zero, ref lng);

            if (erc == 0 && MarshShop.SizeOf(bmi) >= lng)
            {
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                erc = (Avifil32.AVIError)Avifil32.AVIStreamReadFormat(hStream, 0, hbmi, ref lng);
                bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);
            }

            return erc;
        }

        public Avifil32.AVIError ReadVideoSample(IntPtr hStream, int samno, AVIImage img)
        {
            int plSamples = 0, lng = 0, donesams = 0, donelg = 0;
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIStreamRead(hStream, samno, 1, IntPtr.Zero, 0, ref lng, ref plSamples);
            if (erc == 0)
            {
                img.DataLength = lng;        // -- n'est plus traité en interne...
                IntPtr hdata = img.LockBits(false);
                erc = (Avifil32.AVIError)Avifil32.AVIStreamRead(hStream, samno, 1, hdata, lng, ref donelg, ref donesams);
                img.UnlockBits(hdata);
            }

            return erc;
        }

        public Avifil32.AVIError SaveStream(IntPtr hStream, string fnam, Avifil32.AVISaveCallback prghdlr)
        {
            // -- "This function creates a file, copies stream data into the file, closes the file, and releases the resources used by the new file. "
            IntPtr zero = IntPtr.Zero;
            return (Avifil32.AVIError)Avifil32.AVISaveVW(fnam, IntPtr.Zero, prghdlr, 1, ref hStream, ref zero);
        }

        public Avifil32.AVIError SaveStream(IntPtr hStream, string fnam, Avifil32.AVISaveCallback prghdlr, ref Avifil32.AviCompressOptions opts)
        {
            // -- "This function creates a file, copies stream data into the file, closes the file, and releases the resources used by the new file. "
            // -- 11.02.14 : MSVC fonctionne, mais pas MRLE (sur PC ChB, Seven/64)

            IntPtr hopts = MarshShop.LockStruct(opts);
            Avifil32.AVIError erc;
            try
            {
                erc = (Avifil32.AVIError)Avifil32.AVISaveVW(fnam, IntPtr.Zero, prghdlr, 1, ref hStream, ref hopts);
            }
            catch
            {
                erc = Avifil32.AVIError.AVIERR_CANTCOMPRESS;
            }

            opts = MarshShop.UnlockStruct<Avifil32.AviCompressOptions>(hopts);
            return erc;
        }

        public Avifil32.AVIError SetVideoFormat(IntPtr hStream, ref NativeMethods.Gdi32.BITMAPINFO bmi)
        {
            IntPtr hbmi = MarshShop.LockStruct(bmi);
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIStreamSetFormat(hStream, 0, hbmi, bmi.InfoLength());
            bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);

            return erc;
        }

        public bool ShowSaveOptionsDlg(IWin32Window prnt, IntPtr hStream, ref Avifil32.AviCompressOptions opts)
        {
            opts.FCCType = Avifil32.AVIStreamType.streamtypeAUDIO;
            IntPtr hopts = MarshShop.LockStruct(opts);

            int ok = Avifil32.AVISaveOptions(prnt.Handle, 0, 1, ref hStream, ref hopts);
            Avifil32.AVISaveOptionsFree(1, ref hopts);
            opts = MarshShop.UnlockStruct<Avifil32.AviCompressOptions>(hopts);

            return ok != 0;
        }

        public Avifil32.AVIError WriteStreamInfo(IntPtr hStream, Avifil32.AVIStreamInfo inf)
        {
            // -- le stream doit être éditable...

            IntPtr hinf = MarshShop.LockStruct(inf);
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.EditStreamSetInfoW(hStream, hinf, MarshShop.SizeOf(inf));
            MarshShop.UnlockStruct<Avifil32.AVIStreamInfo>(hinf);

            return erc;
        }

        public Avifil32.AVIError WriteVideoSample(IntPtr hStream, int samno, AVIImage img, bool iskey)
        {
            // -- Rem : en vidéo, seule l'écriture en fin de stream est possible...

            int bytedone = 0, samdone = 0;

            IntPtr hdata = img.LockBits(true);
            Avifil32.AVIError erc = (Avifil32.AVIError)Avifil32.AVIStreamWrite(hStream, samno, 1, hdata, img.DataLength, iskey ? Avifil32.AVIIF_KEYFRAME : 0, ref samdone, ref bytedone);
            img.UnlockBits(hdata);

            return erc;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
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