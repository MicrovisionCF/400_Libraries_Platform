using System.Runtime.InteropServices;

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
        // ***************************************************************************************************

        public delegate int AVISaveCallback(int npercent);

        // ***************************************************************************************************

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AviCompressOptions
        {
            public AVIStreamType FCCType;
            public uint fccHandler;
            public uint dwKeyFrameEvery;
            public uint dwQuality;
            public uint dwBytesPerSecond;
            public uint dwFlags;
            public IntPtr lpFormat;
            public uint cbFormat;
            public IntPtr lpParms;
            public uint cbParms;
            public uint dwInterleaveEvery;

            // ---------------------------------------------------
            // Propriétés
            // ---------------------------------------------------

            public string Compressor => zStringizeLong((int)fccHandler);

            public bool IsDefined => fccHandler != 0;

            public int KeyFramePeriod
            {
                get => (int)dwKeyFrameEvery;

                set
                {
                    dwKeyFrameEvery = (uint)value;
                }
            }

            public int Quality
            {
                get => (int)(dwQuality / 100);

                set
                {
                    dwQuality = (uint)(value * 100);
                }
            }

            public bool UseKeyFrames
            {
                get => (dwFlags & (long)AVICompressFlags.AVICOMPRESSF_KEYFRAMES) != 0;

                set
                {
                    if (value)
                        dwFlags = (dwFlags | (uint)AVICompressFlags.AVICOMPRESSF_KEYFRAMES);
                    else
                        dwFlags = (dwFlags & ~(uint)AVICompressFlags.AVICOMPRESSF_KEYFRAMES);
                }
            }

            // ---------------------------------------------------
            // Méthodes
            // ---------------------------------------------------

            // ---------------------------------------------------
            // Shared
            // ---------------------------------------------------

            public static AviCompressOptions Empty()
            {
                AviCompressOptions copts = new AviCompressOptions();
                copts.FCCType = AVIStreamType.streamtypeVIDEO;

                return copts;
            }

            public static int GetBytes(AviCompressOptions copts, out Bytes bf)
            {
                bf = new Bytes(MarshShop.SizeOf(copts));

                return MarshShop.StructToBuffer(copts, bf, 0);
            }

            public static AviCompressOptions MSVC(int qual)
            {
                AviCompressOptions copts = new AviCompressOptions();
                zSetDefault(ref copts, "MSVC");
                copts.dwQuality = (uint)(qual * 100);

                return copts;
            }

            public static int SetBytes(ref Bytes bf, out AviCompressOptions copts)
            {
                return MarshShop.BufferToStruct(bf, 0, out copts);
            }

            // ---------------------------------------------------
            // Privées
            // ---------------------------------------------------

            private static void zSetDefault(ref AviCompressOptions opts, string fourcc)
            {
                // -- valeurs pompées chez ChB, Win2000, le 13.11.01
                // -- et apparemment toujours valables chez ChB, WinSeven, le 17.03.10

                switch (fourcc ?? "")
                {
                    case "IV50": // -- "Indeo® Video 5.10"
                        opts.FCCType = AVIStreamType.streamtypeVIDEO;
                        opts.fccHandler = (uint)AVILib.mmioStringToFOURCC("IV50", 0);
                        opts.dwKeyFrameEvery = 0U;
                        opts.dwQuality = 8500U;
                        opts.dwBytesPerSecond = 0U;
                        opts.dwFlags = (uint)AVICompressFlags.AVICOMPRESSF_VALID;
                        opts.lpFormat = (IntPtr)0;
                        opts.cbFormat = 0U;
                        opts.lpParms = (IntPtr)0;
                        opts.cbParms = 48U;
                        opts.dwInterleaveEvery = 0U;
                        break;

                    case "MSVC": // -- "Microsoft Video 1"
                        opts.FCCType = AVIStreamType.streamtypeVIDEO;
                        opts.fccHandler = (uint)AVILib.mmioStringToFOURCC("MSVC", 0);
                        opts.dwKeyFrameEvery = 0U;
                        opts.dwQuality = 7500U;
                        opts.dwBytesPerSecond = 0U;
                        opts.dwFlags = (uint)AVICompressFlags.AVICOMPRESSF_VALID;
                        opts.lpFormat = (IntPtr)0;
                        opts.cbFormat = 0U;
                        opts.lpParms = (IntPtr)0;
                        opts.cbParms = 4U;
                        opts.dwInterleaveEvery = 0U;
                        break;

                    case "CVID": // -- "Codec Cinepak de Radius"
                        opts.FCCType = AVIStreamType.streamtypeVIDEO;
                        opts.fccHandler = (uint)AVILib.mmioStringToFOURCC("CVID", 0);
                        opts.dwKeyFrameEvery = 0U;
                        opts.dwQuality = 10000U;
                        opts.dwBytesPerSecond = 0U;
                        opts.dwFlags = (uint)AVICompressFlags.AVICOMPRESSF_VALID;
                        opts.lpFormat = (IntPtr)0;
                        opts.cbFormat = 0U;
                        opts.lpParms = (IntPtr)0;
                        opts.cbParms = 4U;
                        opts.dwInterleaveEvery = 0U;
                        break;

                    case "MRLE": // -- "Microsoft RLE"
                        break;

                    case "DIB ": // -- "Trames complètes (non compressé)"
                        break;
                }
            }

            private static string zStringizeLong(int v)
            {
                string ch = "";
                if (v != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        ch += Convert.ToChar(v & 0xFF);
                        v /= 0x100;
                    }
                }

                return ch;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct AVIFileInfo
        {
            public int dwMaxBytesPerSec;
            public int dwFlags;
            public int dwCaps;
            public int dwStreams;
            public int dwSuggestedBufferSize;
            public int dwWidth;
            public int dwHeight;
            public int dwScale;
            public int dwRate;
            public int dwLength;
            public int dwEditCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            private char[] szFileType; // -- [64] en C

            // ---------------------------------------------------
            // Propriétés
            // ---------------------------------------------------

            public bool AllKeyFrames => (dwCaps & (int)AVIFileCaps.AVIFILECAPS_ALLKEYFRAMES) != 0;

            public string FileType => new string(szFileType);

            public string Flags => dwFlags.ToString("X") + ", " + dwCaps.ToString("X");

            public int StreamsCount => dwStreams;

            public bool UseCompression => !((dwCaps & (int)AVIFileCaps.AVIFILECAPS_NOCOMPRESSION) != 0);

            // ---------------------------------------------------
            // Shared
            // ---------------------------------------------------

            public static AVIFileInfo Empty()
            {
                AVIFileInfo inf = new AVIFileInfo();
                inf.szFileType = new char[64];

                return inf;
            }
        }

        private struct AVIRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct AVIStreamInfo
        {
            public int FCCType;
            public int fccHandler;
            public int dwFlags;
            public int dwCaps;
            public short wPriority;
            public short wLanguage;
            public int dwScale;
            public int dwRate;
            public int dwStart;
            public int dwLength;
            public int dwInitialFrames;
            public int dwSuggestedBufferSize;
            public int dwQuality;
            public int dwSampleSize;
            private AVIRect rcFrame;
            public int dwEditCount;
            public int dwFormatChangeCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            private char[] szName; // -- [64] en C

            public AVIStreamInfo(Size siz, float freq)
            {
                FCCType = 0;
                fccHandler = 0;
                dwFlags = 0;
                dwCaps = 0;
                wPriority = 0;
                wLanguage = 0;
                dwScale = 0;
                dwRate = 0;
                dwStart = 0;
                dwLength = 0;
                dwInitialFrames = 0;
                dwSuggestedBufferSize = 0;
                dwQuality = 0;
                dwSampleSize = 0;
                rcFrame = new AVIRect();
                dwEditCount = 0;
                dwFormatChangeCount = 0;
                szName = new char[64];

                FCCTypeString = "vids";
                Size = siz;
                Frequency = freq;
                SampleStart = 0;
            }

            // ---------------------------------------------------
            // Propriétés
            // ---------------------------------------------------

            public string Compressor => zStringizeLong(fccHandler);

            public string FCCTypeString
            {
                get => zStringizeLong(FCCType);

                set
                {
                    FCCType = AVILib.mmioStringToFOURCC(value, 0);
                }
            }

            public float Frequency
            {
                get => (dwRate / (float)dwScale);

                set
                {
                    dwRate = (value * 1000).ToRoundInt();
                    dwScale = 1000;
                }
            }

            public string Name => new string(szName);

            public int SampleCount
            {
                get => dwLength;

                set
                {
                    if (value != dwLength)
                    {
                        dwLength = value;
                    }
                }
            }

            public int SampleStart
            {
                get => dwStart;

                set
                {
                    if (value != dwStart)
                    {
                        dwStart = value;
                    }
                }
            }

            public Size Size
            {
                get => new Size(rcFrame.Right - rcFrame.Left, rcFrame.Bottom - rcFrame.Top);

                set
                {
                    rcFrame.Left = 0;
                    rcFrame.Top = 0;
                    rcFrame.Right = rcFrame.Left + value.Width;
                    rcFrame.Bottom = rcFrame.Top + value.Height;
                }
            }

            // ---------------------------------------------------
            // Shared
            // ---------------------------------------------------

            public static AVIStreamInfo Empty()
            {
                AVIStreamInfo inf = new AVIStreamInfo();
                inf.szName = new char[64];

                return inf;
            }

            // ---------------------------------------------------
            // Privées
            // ---------------------------------------------------

            private static string zStringizeLong(int v)
            {
                string output = "";
                if (v != 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        output += Convert.ToChar(v & 0xFF);
                        v /= 0x100;
                    }
                }

                return output;
            }
        }


        public enum AVICompressFlags
        {
            AVICOMPRESSF_INTERLEAVE = 0x1,              // interleave
            AVICOMPRESSF_DATARATE = 0x2,                // use a data rate
            AVICOMPRESSF_KEYFRAMES = 0x4,               // use keyframes
            AVICOMPRESSF_VALID = 0x8                   // has valid data?
        }

        public enum AVIError
        {
            AVIERR_OK = 0,
            AviErrPrefix = int.MinValue + 0x00044000,
            AVIERR_UNSUPPORTED = AviErrPrefix | 101,
            AVIERR_BADFORMAT = AviErrPrefix | 102,
            AVIERR_MEMORY = AviErrPrefix | 103,
            AVIERR_INTERNAL = AviErrPrefix | 104,
            AVIERR_BADFLAGS = AviErrPrefix | 105,
            AVIERR_BADPARAM = AviErrPrefix | 106,
            AVIERR_BADSIZE = AviErrPrefix | 107,
            AVIERR_BADHANDLE = AviErrPrefix | 108,
            AVIERR_FILEREAD = AviErrPrefix | 109,
            AVIERR_FILEWRITE = AviErrPrefix | 110,
            AVIERR_FILEOPEN = AviErrPrefix | 111,
            AVIERR_COMPRESSOR = AviErrPrefix | 112,
            AVIERR_NOCOMPRESSOR = AviErrPrefix | 113,
            AVIERR_READONLY = AviErrPrefix | 114,
            AVIERR_NODATA = AviErrPrefix | 115,
            AVIERR_BUFFERTOOSMALL = AviErrPrefix | 116,
            AVIERR_CANTCOMPRESS = AviErrPrefix | 117,
            AVIERR_USERABORT = AviErrPrefix | 198,
            AVIERR_ERROR = AviErrPrefix | 199
        }

        private enum AVIFileFlags
        {
            AVIFILEINFO_HASINDEX = 0x10,                // The avi file has an index at the end of the file. For good performance, all avi files should contain an index.
            AVIFILEINFO_MUSTUSEINDEX = 0x20,            // The file index contains the playback order for the chunks in the file. Use the index rather than the physical ordering of the chunks when playing back the data. This could be used for creating a list of frames for editing.
            AVIFILEINFO_ISINTERLEAVED = 0x100,          // The avi file is interleaved.
            AVIFILEINFO_WASCAPTUREFILE = 0x10000,        // The avi file is a specially allocated file used for capturing real-time video. Applications should warn the user before writing over a file with this flag set because the user probably defragmented this file.
            AVIFILEINFO_COPYRIGHTED = 0x20000           // The avi file contains copyrighted data and software. When this flag is used, software should not permit the data to be duplicated.
        }

        private enum AVIFileCaps
        {
            AVIFILECAPS_CANREAD = 0x1,                  // An application can open the avi file with with the read privilege.
            AVIFILECAPS_CANWRITE = 0x2,                 // An application can open the avi file with the write privilege.
            AVIFILECAPS_ALLKEYFRAMES = 0x10,            // Every frame in the avi file is a key frame.
            AVIFILECAPS_NOCOMPRESSION = 0x20           // The avi file does not use a compression method.
        }

        private enum AVIFindFlags
        {
            FIND_ANY = 0x20, // Finds a nonempty frame. This flag supersedes the SEARCH_ANY flag.
            FIND_KEY = 0x10, // Finds a key frame. This flag supersedes the SEARCH_KEY flag.
            FIND_FORMAT = 0x40, // Finds a format change.
            FIND_NEXT = 0x1, // Finds nearest sample, frame, or format change searching forward. The current sample is included in the search. Use this flag with the FIND_ANY, FIND_KEY, or FIND_FORMAT flag. This flag supersedes the SEARCH_FORWARD flag.
            FIND_PREV = 0x4, // Finds nearest sample, frame, or format change searching backward. The current sample is included in the search. Use this flag with the FIND_ANY, FIND_KEY, or FIND_FORMAT flag. This flag supersedes the SEARCH_NEAREST and SEARCH_BACKWARD flags.
            FIND_FROM_START = 0x8 // Finds first sample, frame, or format change beginning from the start of the stream. Use this flag with the FIND_ANY, FIND_KEY, or FIND_FORMAT flag.
        }

        public enum AVIStreamType
        {
            streamtypeAUDIO = 0x73647561,                // = "auds"   Indicates an audio stream.
            streamtypeMIDI = 0x7364696D,                 // = "mids"   Indicates a MIDI stream.
            streamtypeTEXT = 0x73747874,                 // = "txts"   Indicates a text stream.
            streamtypeVIDEO = 0x73646976                // = "vids"   Indicates a video stream.
        }


        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIFileCreateStreamW(IntPtr pfile, ref IntPtr ppAvi, IntPtr psi);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIFileGetStream(IntPtr pfile, ref IntPtr ppAvi, AVIStreamType FCCType, int lParam);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIFileInfoW(IntPtr pfile, IntPtr pfi, int lSize);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIFileOpenW(ref IntPtr ppFile, string szFile, int mode, int pclsidHandler);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIMakeCompressedStream(ref IntPtr ppsCompressed, IntPtr psSource, IntPtr lpOptions, IntPtr pclsidHandler);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVISaveOptions(IntPtr hwnd, int uiFlags, int nStreams, ref IntPtr ppavi, ref IntPtr ppoptions);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVISaveOptionsFree(int nStreams, ref IntPtr ppOptions);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVISaveVW(string szFile, IntPtr pclsidHandler, AVISaveCallback lpfnCallback, int nStreams, ref IntPtr ppaviStream, ref IntPtr ppCompOptions);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamFindSample(IntPtr pavi, int lPos, int lFlags);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamGetFrameClose(IntPtr pget);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamInfoW(IntPtr pavi, IntPtr psi, int lSize);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamLength(IntPtr pavi);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamRead(IntPtr pavi, int lStart, int lSamples, IntPtr lpBufferPtr, int cbBuffer, ref int plBytes, ref int plSamples);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamReadFormat(IntPtr pavi, int lPos, IntPtr lpFormatAddress, ref int lpcbFormat);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamRelease(IntPtr pavi);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamSetFormat(IntPtr pavi, int lPos, IntPtr lpFormat, int cbFormat);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamStart(IntPtr pavi);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int AVIStreamWrite(IntPtr pavi, int lStart, int lSamples, IntPtr lpBufferPtr, int cbBuffer, int dwFlags, ref int plSampWritten, ref int plBytesWritten);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern int EditStreamSetInfoW(IntPtr pavi, IntPtr lpInfo, int cbInfo);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr AVIStreamGetFrame(IntPtr pgf, int lPos);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr AVIStreamGetFrameOpen(IntPtr pavi, IntPtr lpbiWanted);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern uint AVIFileRelease(IntPtr ppFile);
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern void AVIFileExit();
        [DllImport("avifil32.dll", CharSet = CharSet.Unicode)] private static extern void AVIFileInit();
        [DllImport("avifil32.dll", EntryPoint = "CreateEditableStream", CharSet = CharSet.Unicode)] private static extern int CreateEditableStreamAPI(ref IntPtr ppsEditable, IntPtr psSource);
        [DllImport("winmm.dll", EntryPoint = "mmioStringToFOURCCA")] private static extern int mmioStringToFOURCC(string sz, int uFlags); // returns fourcc


        private const int AVISTREAMINFO_DISABLED = 0x1;
        private const int AVISTREAMINFO_FORMATCHANGES = 0x10000;

        private const short AVIIF_KEYFRAME = 16;               // // this frame is a key frame.
        private const short AVIGETFRAMEF_BESTDISPLAYFMT = 1;


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
            AVIFileRelease(hFile);
        }

        public AVIError CloseFrame(IntPtr hFrm)
        {
            return (AVIError)AVIStreamGetFrameClose(hFrm);
        }

        public void CloseLibrary()
        {
            AVIFileExit();
        }

        public AVIError CloseStream(IntPtr hStream)
        {
            return (AVIError)AVIStreamRelease(hStream);
        }

        public AVIError CreateCompressedStream(IntPtr hSrc, AviCompressOptions opts, ref IntPtr hStream)
        {
            IntPtr hopts = MarshShop.LockStruct(opts);
            AVIError erc = (AVIError)AVIMakeCompressedStream(ref hStream, hSrc, hopts, IntPtr.Zero);
            MarshShop.UnlockStruct<AviCompressOptions>(hopts);

            return erc;
        }

        public AVIError CreateEditableStream(IntPtr hSrc, ref IntPtr hStream)
        {
            return (AVIError)CreateEditableStreamAPI(ref hStream, hSrc);
        }

        public AVIError CreateStream(IntPtr hFile, AVIStreamInfo inf, ref IntPtr hStream)
        {
            IntPtr hinf = MarshShop.LockStruct(inf);
            AVIError erc = (AVIError)AVIFileCreateStreamW(hFile, ref hStream, hinf);
            MarshShop.UnlockStruct<AVIStreamInfo>(hinf);

            return erc;
        }

        public int FindSampleKey(IntPtr hStream, int samno)
        {
            return AVIStreamFindSample(hStream, samno, (int)(AVIFindFlags.FIND_PREV | AVIFindFlags.FIND_KEY));
        }

        public int GetStreamLength(IntPtr hStream)
        {
            return AVIStreamLength(hStream);
        }

        public int GetStreamStart(IntPtr hStream)
        {
            return AVIStreamStart(hStream);
        }

        public AVIError OpenFile(ref IntPtr hFile, string fnam, int ofmode)
        {
            return (AVIError)AVILib.AVIFileOpenW(ref hFile, fnam, ofmode, 0);
        }

        public IntPtr OpenFrame(IntPtr hStream)
        {
            return AVIStreamGetFrameOpen(hStream, (IntPtr)0);
        }

        public IntPtr OpenFrame(IntPtr hStream, NativeMethods.Gdi32.BITMAPINFOHEADER bmih)
        {
            IntPtr hbmih = MarshShop.LockStruct(bmih);
            IntPtr hfrm = AVIStreamGetFrameOpen(hStream, hbmih);
            MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFOHEADER>(hbmih);

            return hfrm;
        }

        public void OpenLibrary()
        {
            AVIFileInit();
        }

        public AVIError OpenStream(IntPtr hFile, AVIStreamType fcc, int noInType, ref IntPtr hStream)
        {
            return (AVIError)AVIFileGetStream(hFile, ref hStream, fcc, noInType);
        }

        public AVIError ReadFileInfo(IntPtr hFile, out AVIFileInfo inf)
        {
            inf = default;
            IntPtr hinf = MarshShop.LockStruct(AVIFileInfo.Empty());
            AVIError erc = (AVIError)AVIFileInfoW(hFile, hinf, MarshShop.SizeOf(inf));
            inf = MarshShop.UnlockStruct<AVIFileInfo>(hinf);

            return erc;
        }

        public IntPtr ReadFrame(IntPtr hFrm, int samno)
        {
            return AVIStreamGetFrame(hFrm, samno);
        }

        public AVIError ReadStreamInfo(IntPtr hStream, ref AVIStreamInfo inf)
        {
            IntPtr hinf = MarshShop.LockStruct(AVIStreamInfo.Empty());
            AVIError erc = (AVIError)AVIStreamInfoW(hStream, hinf, MarshShop.SizeOf(inf));
            inf = MarshShop.UnlockStruct<AVIStreamInfo>(hinf);

            return erc;
        }

        public AVIError ReadVideoFormat(IntPtr hStream, ref NativeMethods.Gdi32.BITMAPINFO bmi)
        {
            int lng = 0;
            bmi.bmiColors = new int[256];
            AVIError erc = (AVIError)AVIStreamReadFormat(hStream, 0, IntPtr.Zero, ref lng);

            if (erc == 0 && MarshShop.SizeOf(bmi) >= lng)
            {
                IntPtr hbmi = MarshShop.LockStruct(bmi);
                erc = (AVIError)AVIStreamReadFormat(hStream, 0, hbmi, ref lng);
                bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);
            }

            return erc;
        }

        public AVIError ReadVideoSample(IntPtr hStream, int samno, AVIImage img)
        {
            int plSamples = 0, lng = 0, donesams = 0, donelg = 0;
            AVIError erc = (AVIError)AVIStreamRead(hStream, samno, 1, IntPtr.Zero, 0, ref lng, ref plSamples);
            if (erc == 0)
            {
                img.DataLength = lng;        // -- n'est plus traité en interne...
                IntPtr hdata = img.LockBits(false);
                erc = (AVIError)AVIStreamRead(hStream, samno, 1, hdata, lng, ref donelg, ref donesams);
                img.UnlockBits(hdata);
            }

            return erc;
        }

        public AVIError SaveStream(IntPtr hStream, string fnam, AVISaveCallback prghdlr)
        {
            // -- "This function creates a file, copies stream data into the file, closes the file, and releases the resources used by the new file. "
            IntPtr zero = IntPtr.Zero;
            return (AVIError)AVILib.AVISaveVW(fnam, IntPtr.Zero, prghdlr, 1, ref hStream, ref zero);
        }

        public AVIError SaveStream(IntPtr hStream, string fnam, AVISaveCallback prghdlr, ref AviCompressOptions opts)
        {
            // -- "This function creates a file, copies stream data into the file, closes the file, and releases the resources used by the new file. "
            // -- 11.02.14 : MSVC fonctionne, mais pas MRLE (sur PC ChB, Seven/64)

            IntPtr hopts = MarshShop.LockStruct(opts);
            AVIError erc;
            try
            {
                erc = (AVIError)AVILib.AVISaveVW(fnam, IntPtr.Zero, prghdlr, 1, ref hStream, ref hopts);
            }
            catch
            {
                erc = AVIError.AVIERR_CANTCOMPRESS;
            }

            opts = MarshShop.UnlockStruct<AviCompressOptions>(hopts);
            return erc;
        }

        public AVIError SetVideoFormat(IntPtr hStream, ref NativeMethods.Gdi32.BITMAPINFO bmi)
        {
            IntPtr hbmi = MarshShop.LockStruct(bmi);
            AVIError erc = (AVIError)AVIStreamSetFormat(hStream, 0, hbmi, bmi.InfoLength());
            bmi = MarshShop.UnlockStruct<NativeMethods.Gdi32.BITMAPINFO>(hbmi);

            return erc;
        }

        public bool ShowSaveOptionsDlg(IWin32Window prnt, IntPtr hStream, ref AviCompressOptions opts)
        {
            opts.FCCType = AVIStreamType.streamtypeAUDIO;
            IntPtr hopts = MarshShop.LockStruct(opts);

            int ok = AVISaveOptions(prnt.Handle, 0, 1, ref hStream, ref hopts);
            AVISaveOptionsFree(1, ref hopts);
            opts = MarshShop.UnlockStruct<AviCompressOptions>(hopts);

            return ok != 0;
        }

        public AVIError WriteStreamInfo(IntPtr hStream, AVIStreamInfo inf)
        {
            // -- le stream doit être éditable...

            IntPtr hinf = MarshShop.LockStruct(inf);
            AVIError erc = (AVIError)EditStreamSetInfoW(hStream, hinf, MarshShop.SizeOf(inf));
            MarshShop.UnlockStruct<AVIStreamInfo>(hinf);

            return erc;
        }

        public AVIError WriteVideoSample(IntPtr hStream, int samno, AVIImage img, bool iskey)
        {
            // -- Rem : en vidéo, seule l'écriture en fin de stream est possible...

            int bytedone = 0, samdone = 0;

            IntPtr hdata = img.LockBits(true);
            AVIError erc = (AVIError)AVIStreamWrite(hStream, samno, 1, hdata, img.DataLength, iskey ? AVIIF_KEYFRAME : 0, ref samdone, ref bytedone);
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