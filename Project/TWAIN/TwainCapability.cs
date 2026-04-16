using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    internal class TwainCapability<T> : Citizen
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // ***************************************************************************************************

        internal delegate void ValueChangedEventHandler();

        internal event ValueChangedEventHandler ValueChanged;

        // ***************************************************************************************************

        private TWAIN _dsm;
        private TWAIN.CAP _cap;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapability(TWAIN dsm, TWAIN.CAP cap) : base()
        {
            _dsm = dsm;
            _cap = cap;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool IsGetArray => oGetContainerType(TWAIN.MSG.GET) == TWAIN.TWON.ARRAY;

        public bool IsGetEnumeration => oGetContainerType(TWAIN.MSG.GET) == TWAIN.TWON.ENUMERATION;

        public bool IsGetOneValue => oGetContainerType(TWAIN.MSG.GET) == TWAIN.TWON.ONEVALUE;

        public bool IsGetRange => oGetContainerType(TWAIN.MSG.GET) == TWAIN.TWON.RANGE;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public List<T> GetArray()
        {
            List<T> values = new List<T>();

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                values = capability.GetArray<T>(_dsm);

                _dsm.DsmMemFree(ref capability.hContainer);
            }

            return values;
        }

        public T GetCurrentOneValue()
        {
            return oGetAnyOneValue(TWAIN.MSG.GETCURRENT);
        }

        public T GetDefaultOneValue()
        {
            return oGetAnyOneValue(TWAIN.MSG.GETDEFAULT);
        }

        public List<T> GetEnumeration()
        {
            List<T> values = new List<T>();

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                values = capability.GetEnumeration<T>(_dsm);

                _dsm.DsmMemFree(ref capability.hContainer);
            }

            return values;
        }

        public T GetOneValue()
        {
            return oGetAnyOneValue(TWAIN.MSG.GET);
        }

        public (T min, T max, T step, T def, T cur) GetRange()
        {
            T min = default;
            T max = default;
            T step = default;
            T def = default;
            T cur = default;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                (min, max, step, def, cur) = capability.GetRange<T>(_dsm);

                _dsm.DsmMemFree(ref capability.hContainer);
            }

            return (min, max, step, def, cur);
        }

        public void SetOneValue(T value, bool force = false)
        {
            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = TWAIN.TWON.ONEVALUE;
            capability.hContainer = IntPtr.Zero;

            // La spéc. préconise de lire la valeur courante
            // et de ne la modifier que si elle est différente.
            // (cf. TWAIN 2.5 page 422/766 § Best Practices for Applications.)

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GETCURRENT, ref capability) == TWAIN.STS.SUCCESS)
            {
                T current = capability.GetOneValue<T>(_dsm);

                if (force || !value.Equals(current))
                {
                    capability.SetOneValue(_dsm, value);
                    oOnValueChanged();
                }

                _dsm.DsmMemFree(ref capability.hContainer);
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _dsm = null;

            base.oDispose(isExplicit);
        }

        protected T oGetAnyOneValue(TWAIN.MSG msg)
        {
            T value = default;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, msg, ref capability) == TWAIN.STS.SUCCESS)
            {
                value = capability.GetOneValue<T>(_dsm);

                _dsm.DsmMemFree(ref capability.hContainer);
            }

            return value;
        }

        protected TWAIN.TWON oGetContainerType(TWAIN.MSG msg)
        {
            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _cap;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dsm.DatCapability(TWAIN.DG.CONTROL, msg, ref capability) == TWAIN.STS.SUCCESS)
            {
                _dsm.DsmMemFree(ref capability.hContainer);
            }

            return capability.ConType;
        }

        protected void oOnValueChanged()
        {
            this.ValueChanged?.Invoke();
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

    internal class TwainCapabilityEnum<T1, T2> : TwainCapability<T2> where T1 : Enum
    {
        // ***************************************************************************************************
        // 15.03.23 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityEnum(TWAIN dsm, TWAIN.CAP cap) : base(dsm, cap)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public new T1 GetCurrentOneValue()
        {
            return (T1)Convert.ChangeType(base.GetCurrentOneValue(), Enum.GetUnderlyingType(typeof(T1)));
        }

        public new List<T1> GetEnumeration()
        {
            List<T2> lst = base.GetEnumeration();

            return lst.Select(o => (T1)Convert.ChangeType(o, Enum.GetUnderlyingType(typeof(T1)))).ToList();
        }

        public new T1 GetOneValue()
        {
            return (T1)Convert.ChangeType(base.GetOneValue(), Enum.GetUnderlyingType(typeof(T1)));
        }

        public void SetOneValue(T1 value, bool force = false)
        {
            T2 value2 = (T2)Convert.ChangeType(value, typeof(T2));

            base.SetOneValue(value2, force);
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

    internal class TwainCapabilityFloat : TwainCapability<TWAIN.TW_FIX32>
    {
        // ***************************************************************************************************
        // 15.03.23 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityFloat(TWAIN dsm, TWAIN.CAP cap) : base(dsm, cap)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public new float GetCurrentOneValue()
        {
            return base.GetCurrentOneValue().Get();
        }

        public new float GetDefaultOneValue()
        {
            return base.GetDefaultOneValue().Get();
        }

        public new float GetOneValue()
        {
            return base.GetOneValue().Get();
        }

        public new (float min, float max, float step, float def, float cur) GetRange()
        {
            (TWAIN.TW_FIX32 min, TWAIN.TW_FIX32 max, TWAIN.TW_FIX32 step, TWAIN.TW_FIX32 def, TWAIN.TW_FIX32 cur) = base.GetRange();
            return (min.Get(), max.Get(), step.Get(), def.Get(), cur.Get());
        }

        public void SetOneValue(float value, bool force = false)
        {
            TWAIN.TW_FIX32 fix32 = default;
            fix32.Set(value);

            base.SetOneValue(fix32, force);
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

    internal class TwainCapabilityRectangleF : TwainCapability<TWAIN.TW_FRAME>
    {
        // ***************************************************************************************************
        // 15.03.23 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityRectangleF(TWAIN dsm, TWAIN.CAP cap) : base(dsm, cap)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public new RectangleF GetCurrentOneValue()
        {
            return base.GetCurrentOneValue().Get();
        }

        public void SetOneValue(RectangleF value, bool force = false)
        {
            TWAIN.TW_FRAME frame = default;
            frame.Set(value);

            base.SetOneValue(frame, force);
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