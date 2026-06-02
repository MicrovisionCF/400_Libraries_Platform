using Microvision.Collections;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    public class TwainDataSources : BaseList<TwainDataSource>
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // 02.06.26 : (libs 4.0)
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

        public TwainDataSource? Open(int no, TWAIN dataSourceManager, TwainThread thread, ITwainImageReceiver imageReceiver)
        {
            bool ok = _items[no].Open(dataSourceManager, thread, imageReceiver);

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