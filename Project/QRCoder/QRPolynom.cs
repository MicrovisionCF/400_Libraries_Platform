using System.Collections.Generic;
using System.Text;

namespace Microvision.QRCoder
{
    internal class QRPolynom
    {
        // ***************************************************************************************************
        // 13.02.18 : Création
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public struct Item
        {
            public int coefficient;
            public int exponent;

            public Item(int coefficient, int exponent)
            {
                this.coefficient = coefficient;
                this.exponent = exponent;
            }
        }


        private List<Item> _items;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public QRPolynom()
        {
            this.PolyItems = new List<Item>();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public List<Item> PolyItems
        {
            get => _items;
            set => _items = value;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Item polyItem in PolyItems)
                sb.Append("a^" + polyItem.coefficient + "*x^" + polyItem.exponent + " + ");

            return sb.ToString().TrimEnd(new[] { ' ', '+' });
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


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