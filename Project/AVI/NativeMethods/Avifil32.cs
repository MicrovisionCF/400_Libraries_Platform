using System;
using System.Drawing;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.NativeMethods
{
    public class Avifil32
    {
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
                        opts.fccHandler = (uint)Winmm.mmioStringToFOURCC("IV50", 0);
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
                        opts.fccHandler = (uint)Winmm.mmioStringToFOURCC("MSVC", 0);
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
                        opts.fccHandler = (uint)Winmm.mmioStringToFOURCC("CVID", 0);
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
                    FCCType = Winmm.mmioStringToFOURCC(value, 0);
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

        public enum AVIFileFlags
        {
            AVIFILEINFO_HASINDEX = 0x10,                // The avi file has an index at the end of the file. For good performance, all avi files should contain an index.
            AVIFILEINFO_MUSTUSEINDEX = 0x20,            // The file index contains the playback order for the chunks in the file. Use the index rather than the physical ordering of the chunks when playing back the data. This could be used for creating a list of frames for editing.
            AVIFILEINFO_ISINTERLEAVED = 0x100,          // The avi file is interleaved.
            AVIFILEINFO_WASCAPTUREFILE = 0x10000,        // The avi file is a specially allocated file used for capturing real-time video. Applications should warn the user before writing over a file with this flag set because the user probably defragmented this file.
            AVIFILEINFO_COPYRIGHTED = 0x20000           // The avi file contains copyrighted data and software. When this flag is used, software should not permit the data to be duplicated.
        }

        public enum AVIFileCaps
        {
            AVIFILECAPS_CANREAD = 0x1,                  // An application can open the avi file with with the read privilege.
            AVIFILECAPS_CANWRITE = 0x2,                 // An application can open the avi file with the write privilege.
            AVIFILECAPS_ALLKEYFRAMES = 0x10,            // Every frame in the avi file is a key frame.
            AVIFILECAPS_NOCOMPRESSION = 0x20           // The avi file does not use a compression method.
        }

        public enum AVIFindFlags
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





        public const int AVISTREAMINFO_DISABLED = 0x1;
        public const int AVISTREAMINFO_FORMATCHANGES = 0x10000;

        public const short AVIIF_KEYFRAME = 16;               // // this frame is a key frame.
        public const short AVIGETFRAMEF_BESTDISPLAYFMT = 1;



        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIFileCreateStreamW(IntPtr pfile, ref IntPtr ppAvi, IntPtr psi);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIFileGetStream(IntPtr pfile, ref IntPtr ppAvi, AVIStreamType FCCType, int lParam);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIFileInfoW(IntPtr pfile, IntPtr pfi, int lSize);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIFileOpenW(ref IntPtr ppFile, string szFile, int mode, int pclsidHandler);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIMakeCompressedStream(ref IntPtr ppsCompressed, IntPtr psSource, IntPtr lpOptions, IntPtr pclsidHandler);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVISaveOptions(IntPtr hwnd, int uiFlags, int nStreams, ref IntPtr ppavi, ref IntPtr ppoptions);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVISaveOptionsFree(int nStreams, ref IntPtr ppOptions);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVISaveVW(string szFile, IntPtr pclsidHandler, AVISaveCallback lpfnCallback, int nStreams, ref IntPtr ppaviStream, ref IntPtr ppCompOptions);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamFindSample(IntPtr pavi, int lPos, int lFlags);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamGetFrameClose(IntPtr pget);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamInfoW(IntPtr pavi, IntPtr psi, int lSize);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamLength(IntPtr pavi);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamRead(IntPtr pavi, int lStart, int lSamples, IntPtr lpBufferPtr, int cbBuffer, ref int plBytes, ref int plSamples);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamReadFormat(IntPtr pavi, int lPos, IntPtr lpFormatAddress, ref int lpcbFormat);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamRelease(IntPtr pavi);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamSetFormat(IntPtr pavi, int lPos, IntPtr lpFormat, int cbFormat);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamStart(IntPtr pavi);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int AVIStreamWrite(IntPtr pavi, int lStart, int lSamples, IntPtr lpBufferPtr, int cbBuffer, int dwFlags, ref int plSampWritten, ref int plBytesWritten);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern int EditStreamSetInfoW(IntPtr pavi, IntPtr lpInfo, int cbInfo);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern IntPtr AVIStreamGetFrame(IntPtr pgf, int lPos);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern IntPtr AVIStreamGetFrameOpen(IntPtr pavi, IntPtr lpbiWanted);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern uint AVIFileRelease(IntPtr ppFile);

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern void AVIFileExit();

        [DllImport(nameof(Avifil32), CharSet = CharSet.Unicode)]
        public static extern void AVIFileInit();

        [DllImport(nameof(Avifil32), EntryPoint = "CreateEditableStream", CharSet = CharSet.Unicode)]
        public static extern int CreateEditableStreamAPI(ref IntPtr ppsEditable, IntPtr psSource);
    }
}
