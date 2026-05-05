using Microvision.Types;

namespace Microvision.Avi
{
    public class AVIFile : Citizen
    {
        // ***************************************************************************************************
        // 01.10.01 : (ChB) structure vaguement définie dans AVIFile.dll, identifiée par un handle, contenant
        //            une en-tête (AVIFileInfo) et des Streams (AVIStream), qui peuvent être vidéo, audio ou
        //            autro.
        // 08.03.10 : traduction VBNet.
        // 14.06.11 : libs 1.8
        // 05.02.14 : libs 2.0, intégration à µV.Platform.
        // 25.03.15 : typages plus rigoureux, à la suite de AVILib (_handle as Intptr, _lastError as AVIError)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public enum OpenFileMode
        {
            OF_CREATE = 0x1000,             // Creates a new file. If the file already exists, it is truncated to zero length.
            OF_SHARE_DENY_NONE = 0x40,      // Opens the file nonexclusively. Other processes can open the file with read or write access. AVIFileOpen fails if another process has opened the file in compatibility mode.
            OF_SHARE_DENY_READ = 0x30,      // Opens the file nonexclusively. Other processes can open the file with write access. AVIFileOpen fails if another process has opened the file in compatibility mode or has read access to it.
            OF_SHARE_DENY_WRITE = 0x20,     // Opens the file nonexclusively. Other processes can open the file with read access. AVIFileOpen fails if another process has opened the file in compatibility mode or has write access to it.
            OF_SHARE_EXCLUSIVE = 0x10,      // Opens the file and denies other processes any access to it. AVIFileOpen fails if any other process has opened the file.
            OF_READ = 0x0,                  // Opens the file for reading.
            OF_READWRITE = 0x2,             // Opens the file for reading and writing.
            OF_WRITE = 0x1                  // Opens the file for writing.
        }


        private readonly AVILib _lib;

        private IntPtr _handle;
        private AVILib.AVIFileInfo _info;
        private AVILib.AVIError _lastError;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AVIFile() : base()
        {
            _lib = new AVILib();
            _lib.OpenLibrary();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public AVILib.AVIFileInfo FileInfo => _info;

        public int StreamsCount => _info.StreamsCount;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void CloseFile()
        {
            if (_handle != IntPtr.Zero)
            {
                _lib.CloseFile(_handle);
                _handle = IntPtr.Zero;
            }
        }

        public AVIStream? CreateStream(AVILib.AVIStreamInfo info)
        {
            IntPtr hStream = IntPtr.Zero;
            _lastError = _lib.CreateStream(_handle, info, ref hStream);

            AVIStream? output = _lastError == AVILib.AVIError.AVIERR_OK ? new AVIStream(_lib, hStream) : null;

            return output;
        }

        public bool OpenFile(string fileName, OpenFileMode access)
        {
            IntPtr hdl = IntPtr.Zero;
            _lastError = _lib.OpenFile(ref hdl, fileName, (int)access);

            if (_lastError == AVILib.AVIError.AVIERR_OK)
            {
                if (_lib.ReadFileInfo(hdl, out AVILib.AVIFileInfo inf) == AVILib.AVIError.AVIERR_OK)
                {
                    _handle = hdl;
                    _info = inf;
                }
                else
                {
                    _lib.CloseFile(hdl);
                }
            }

            return _handle != IntPtr.Zero;
        }

        public AVIStream? OpenStream(AVILib.AVIStreamType streamtype, int noInType)
        {
            IntPtr hStream = IntPtr.Zero;
            _lastError = _lib.OpenStream(_handle, streamtype, noInType, ref hStream);

            AVIStream? output = _lastError == AVILib.AVIError.AVIERR_OK ? new AVIStream(_lib, hStream) : null;

            return output;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _lib.Dispose();

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