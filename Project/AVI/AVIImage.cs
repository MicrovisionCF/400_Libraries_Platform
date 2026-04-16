using System.Diagnostics;
using System.Drawing;

using Microvision.Types;

namespace Microvision.Avi
{
    public class AVIImage : BasicDibApi
    {
        // ***************************************************************************************************
        // 01.10.01 : (ChB) un échantillon des AVIStreams vidéo, c'est à dire une image, c'est un dire un Dib.
        //            Cet objet a des capacités de dessins différentes de l'habitude, via la librairie DrawDib
        //            (en réalité VFW).
        // 06.04.06 : qq modifs pour éviter les buffers intermédiaires.
        // 08.03.10 : traduction VBNet.
        // 14.06.11 : libs 1.8
        // 05.02.14 : libs 2.0, héritage de BasicDibAPI et intégration à µV.Platform.
        // 12.05.17 : (libs 2.1) ImportDibHeader et ImportDibData supprimés car commentés depuis 2.0
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private DrawDibDC _drawDib;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AVIImage() : base()
        {
        }

        public AVIImage(ref BITMAPINFO bmi) : base(bmi)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int DataLength
        {
            get => _infos.DataLength();

            set
            {
                if (value > _infos.DataLength())
                {
                    Debug.Print("ya un gros problème dans AVIImage...");
                }
            }
        }

        public BITMAPINFO Header => _infos;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Draw(Point p)
        {
            // DDF_SAME_DRAW incompatible avec zoom
            _drawDib?.Draw(ref _infos, _bytes, p);
        }

        public void Draw(Rectangle rct)
        {
            // DDF_SAME_DRAW incompatible avec zoom
            _drawDib?.Draw(ref _infos, _bytes, rct);
        }

        public void DrawBegin(Graphics gf)
        {
            _drawDib = new DrawDibDC();
            _drawDib.BeginDraw(gf, ref _infos);
        }

        public void DrawBegin(Graphics gf, Size siz)
        {
            _drawDib = new DrawDibDC();
            _drawDib.BeginDraw(gf, ref _infos, siz);
        }

        public void DrawEnd(Graphics gf)
        {
            _drawDib.EndDraw(gf);
            _drawDib.Dispose();
            _drawDib = null;
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