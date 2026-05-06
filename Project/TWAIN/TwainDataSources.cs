using Microvision.Collections;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    public class TwainDataSources : BaseList<TwainDataSource>
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainDataSources() : base()
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string GetProductName(int no)
        {
            return _items[no].ProductName;
        }

        public TwainDataSource? Open(int no, TWAIN dsm, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            bool ok = _items[no].Open(dsm, thread, imageReceiver);

            return ok ? _items[no].AddLife() : null;
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