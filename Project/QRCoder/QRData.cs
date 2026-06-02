using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microvision.Geometry;
using Microvision.Types;

namespace Microvision.QRCoder
{
    internal class QRData : Citizen
    {
        // ***************************************************************************************************
        // 13.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        private readonly xQRConfigInfos _info;
        private readonly List<BitArray> _matrix;
        private readonly List<BitArray> _matrixLocked;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRData(xQRConfigInfos info)
        {
            _info = info;
            int size = zGetSizeFromVersion(_info.version);

            _matrix = [];
            _matrixLocked = [];

            for (int i = 0; i < size; i++)
            {
                _matrix.Add(new BitArray(size));
                _matrixLocked.Add(new BitArray(size));
            }
        }

        public QRData(QRData other) : this(other._info)
        {
            for (int x = 0; x < _matrix.Count; x++)
                for (int y = 0; y < _matrix.Count; y++)
                {
                    _matrix[x][y] = other.GetPixel(x, y);
                    _matrixLocked[x][y] = other.IsLocked(x, y);
                }
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public PointIs AlignmentPositions => _info.alignementPositions;

        public xQRConfigInfos Infos => _info;

        public QRStrength Strength => _info.strength;

        public QRVersion Version => _info.version;

        public int Width => _matrix.Count;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void AddQuietZone()
        {
            bool[] quietLine = new bool[_matrix.Count + 8];
            bool[] quietPart = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                _matrix.Insert(0, new BitArray(quietLine));
                _matrix.Add(new BitArray(quietLine));
            }

            for (int i = 4; i < _matrix.Count - 4; i++)
            {
                bool[] tmpLine = [.. quietPart, .. _matrix[i].Cast<bool>(), .. quietPart];
                _matrix[i] = new BitArray(tmpLine);
            }

            // On a plus rien à modifier après la quiet zone, comme ça ça petera
            _matrixLocked.Clear();
            _matrixLocked.TrimExcess();
        }

        public bool GetPixel(int x, int y)
        {
            return _matrix[x][y];
        }

        public bool GetPixelToMask(int x, int y)
        {
            return _matrix[x][y];
        }

        public bool IsLocked(int x, int y)
        {
            return _matrixLocked[x][y];
        }

        public void SetPixel(int x, int y, bool value)
        {
            _matrix[x][y] = value;
            _matrixLocked[x][y] = true;
        }

        public void SetPixelToMask(int x, int y, bool value)
        {
            _matrix[x][y] = value;
            _matrixLocked[x][y] = false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < _matrix.Count; x++)
            {
                for (int y = 0; y < _matrix.Count; y++)
                {
                    if (_matrix[x][y])
                    {
                        if (_matrixLocked[x][y])
                            sb.Append("x ");
                        else
                            sb.Append("# ");
                    }
                    else if (_matrixLocked[x][y])
                        sb.Append(". ");
                    else
                        sb.Append("  ");
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
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

        private static int zGetSizeFromVersion(QRVersion version)
        {
            return 21 + ((int)version - 1) * 4;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}