using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaProperty : Citizen
    {
        // ***************************************************************************************************
        // 08.02.13 : ébauche.
        //            Restent : subtype, readonly, vector, écriture.
        // 19.09.16 : introduction des listes WIA normalisées (via WiaExtensions)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public enum PropertyType
        {
            UnsupportedPropertyType = WIA.WiaPropertyType.UnsupportedPropertyType,
            BooleanPropertyType = WIA.WiaPropertyType.BooleanPropertyType,
            BytePropertyType = WIA.WiaPropertyType.BytePropertyType,
            IntegerPropertyType = WIA.WiaPropertyType.IntegerPropertyType,
            UnsignedIntegerPropertyType = WIA.WiaPropertyType.UnsignedIntegerPropertyType,
            LongPropertyType = WIA.WiaPropertyType.LongPropertyType,
            UnsignedLongPropertyType = WIA.WiaPropertyType.UnsignedLongPropertyType,
            ErrorCodePropertyType = WIA.WiaPropertyType.ErrorCodePropertyType,
            LargeIntegerPropertyType = WIA.WiaPropertyType.LargeIntegerPropertyType,
            UnsignedLargeIntegerPropertyType = WIA.WiaPropertyType.UnsignedLargeIntegerPropertyType,

            SinglePropertyType = WIA.WiaPropertyType.SinglePropertyType,
            DoublePropertyType = WIA.WiaPropertyType.DoublePropertyType,
            CurrencyPropertyType = WIA.WiaPropertyType.CurrencyPropertyType,
            DatePropertyType = WIA.WiaPropertyType.DatePropertyType,
            FileTimePropertyType = WIA.WiaPropertyType.FileTimePropertyType,
            ClassIDPropertyType = WIA.WiaPropertyType.ClassIDPropertyType,
            StringPropertyType = WIA.WiaPropertyType.StringPropertyType,
            ObjectPropertyType = WIA.WiaPropertyType.ObjectPropertyType,
            HandlePropertyType = WIA.WiaPropertyType.HandlePropertyType,
            VariantPropertyType = WIA.WiaPropertyType.VariantPropertyType,

            VectorOfBooleansPropertyType = WIA.WiaPropertyType.VectorOfBooleansPropertyType,
            VectorOfBytesPropertyType = WIA.WiaPropertyType.VectorOfBytesPropertyType,
            VectorOfIntegersPropertyType = WIA.WiaPropertyType.VectorOfIntegersPropertyType,
            VectorOfUnsignedIntegersPropertyType = WIA.WiaPropertyType.VectorOfUnsignedIntegersPropertyType,
            VectorOfLongsPropertyType = WIA.WiaPropertyType.VectorOfLongsPropertyType,
            VectorOfUnsignedLongsPropertyType = WIA.WiaPropertyType.VectorOfUnsignedLongsPropertyType,
            VectorOfErrorCodesPropertyType = WIA.WiaPropertyType.VectorOfErrorCodesPropertyType,
            VectorOfLargeIntegersPropertyType = WIA.WiaPropertyType.VectorOfLargeIntegersPropertyType,
            VectorOfUnsignedLargeIntegersPropertyType = WIA.WiaPropertyType.VectorOfUnsignedLargeIntegersPropertyType,

            VectorOfSinglesPropertyType = WIA.WiaPropertyType.VectorOfSinglesPropertyType,
            VectorOfDoublesPropertyType = WIA.WiaPropertyType.VectorOfDoublesPropertyType,
            VectorOfCurrenciesPropertyType = WIA.WiaPropertyType.VectorOfCurrenciesPropertyType,
            VectorOfDatesPropertyType = WIA.WiaPropertyType.VectorOfDatesPropertyType,
            VectorOfFileTimesPropertyType = WIA.WiaPropertyType.VectorOfFileTimesPropertyType,
            VectorOfClassIDsPropertyType = WIA.WiaPropertyType.VectorOfClassIDsPropertyType,
            VectorOfStringsPropertyType = WIA.WiaPropertyType.VectorOfStringsPropertyType,
            VectorOfVariantsPropertyType = WIA.WiaPropertyType.VectorOfVariantsPropertyType
        }

        public enum PropertySubType
        {
            UnspecifiedSubType = WIA.WiaSubType.UnspecifiedSubType,
            RangeSubType = WIA.WiaSubType.RangeSubType,
            ListSubType = WIA.WiaSubType.ListSubType,
            FlagSubtype = WIA.WiaSubType.FlagSubType                // -- map de bits
        }


        private WIA.Property _property;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        internal WiaProperty() : base()
        {
        }

        internal WiaProperty(WIA.Property prp) : this()
        {
            _property = prp;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public object DefaultValue
        {
            get
            {
                object output = null;

                if (_property.SubType != WIA.WiaSubType.UnspecifiedSubType)
                    output = _property.SubTypeDefault;

                return output;
            }
        }

        public bool IsReadOnly => _property.IsReadOnly;

        public bool IsVector => _property.IsVector;

        public string Name => _property.Name;

        public int PropertyID => _property.PropertyID;

        public PropertySubType SubType => (PropertySubType)_property.SubType;

        public PropertyType Type => (PropertyType)_property.Type;

        public object Value
        {
            get => _property.get_Value();
            set => _property.set_Value(value);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public string DebugString(string pfx)
        {
            return pfx + GetType().Name + " = " + zDebugProperty(this, pfx);
        }

        public int GetFlagMap()
        {
            int mp = 0;

            if (_property.SubType == WIA.WiaSubType.FlagSubType)
            {
                WIA.Vector vct = _property.SubTypeValues;
                List<int> lst = vct.ToList<int>();
                int nb = lst.Count;
                lst.ForEach(o => mp |= o);
            }

            return mp;
        }

        public void GetRange(out int minValue, out int maxValue, out int step)
        {
            if (_property.SubType == WIA.WiaSubType.RangeSubType)
            {
                minValue = _property.SubTypeMin;
                maxValue = _property.SubTypeMax;
                step = _property.SubTypeStep;
            }
            else
            {
                minValue = 0;
                maxValue = 0;
                step = 0;
            }
        }

        public List<T> GetTable<T>()
        {
            List<T> lst = null;
            if (_property.SubType == WIA.WiaSubType.FlagSubType || _property.SubType == WIA.WiaSubType.ListSubType)
            {
                WIA.Vector vct = _property.SubTypeValues;
                lst = vct.ToList<T>();
            }

            return lst;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_property is not null)
            {
                Marshal.ReleaseComObject(_property);
                _property = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static string zDebugProperty(WiaProperty prp, string pfx)
        {
            string ch = prp.Name + " (" + prp.Type.ToNameString() + ") = " + prp.Value.ToString();

            if (!prp.IsReadOnly && prp.SubType != PropertySubType.UnspecifiedSubType)
            {
                ch = ch + SpecialChars.NewLine + pfx + SpecialChars.Tab + prp.SubType.ToNameString() + ", " + prp.DefaultValue.ToString();

                if ((int)prp.SubType == (int)WIA.WiaSubType.RangeSubType)
                {
                    prp.GetRange(out int vmin, out int vmax, out int vstp);
                    ch = ch + ", [" + vmin.ToString() + ", " + vmax.ToString() + ", " + vstp.ToString() + "]";
                }
                else if ((int)prp.SubType == (int)WIA.WiaSubType.ListSubType || (int)prp.SubType == (int)WIA.WiaSubType.FlagSubType)
                {
                    List<object> vs = prp.GetTable<object>();
                    ch += ", {";
                    for (int i = 0; i < vs.Count; i++)
                    {
                        ch += vs[i].ToString();
                        if (i < vs.Count - 1)
                            ch += ", ";
                    }

                    ch += "}";
                }
            }

            return ch;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}