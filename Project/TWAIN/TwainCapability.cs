using System;
using System.Collections.Generic;
using System.Linq;

using Microvision.Geometry;
using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    internal class TwainCapability<T> : Citizen where T : struct
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        internal delegate void ValueChangedEventHandler();

        internal event ValueChangedEventHandler? ValueChanged;

        // ***************************************************************************************************

        private readonly TWAIN _dataSourceManager;
        private readonly TWAIN.CAP _capabilities;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapability(TWAIN dataSourceManager, TWAIN.CAP capabilities) : base()
        {
            _dataSourceManager = dataSourceManager;
            _capabilities = capabilities;
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
            List<T>? values = null;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                values = capability.GetArray<T>(_dataSourceManager);

                _dataSourceManager.DsmMemFree(ref capability.hContainer);
            }

            return values ?? [];
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
            List<T>? values = null;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                values = capability.GetEnumeration<T>(_dataSourceManager);

                _dataSourceManager.DsmMemFree(ref capability.hContainer);
            }

            return values ?? [];
        }

        public T GetOneValue()
        {
            return oGetAnyOneValue(TWAIN.MSG.GET);
        }

        public (T minValue, T maxValue, T stepValue, T defaultValue, T currentValue) GetRange()
        {
            T minValue = default;
            T maxValue = default;
            T stepValue = default;
            T defaultValue = default;
            T currentValue = default;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GET, ref capability) == TWAIN.STS.SUCCESS)
            {
                (minValue, maxValue, stepValue, defaultValue, currentValue) = capability.GetRange<T>(_dataSourceManager);

                _dataSourceManager.DsmMemFree(ref capability.hContainer);
            }

            return (minValue, maxValue, stepValue, defaultValue, currentValue);
        }

        public void SetOneValue(T value, bool force = false)
        {
            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = TWAIN.TWON.ONEVALUE;
            capability.hContainer = IntPtr.Zero;

            // La spéc. préconise de lire la valeur courante
            // et de ne la modifier que si elle est différente.
            // (cf. TWAIN 2.5 page 422/766 § Best Practices for Applications.)

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GETCURRENT, ref capability) == TWAIN.STS.SUCCESS)
            {
                T current = capability.GetOneValue<T>(_dataSourceManager);

                if (force || !value.Equals(current))
                {
                    capability.SetOneValue(_dataSourceManager, value);
                    oOnValueChanged();
                }

                _dataSourceManager.DsmMemFree(ref capability.hContainer);
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            base.oDispose(isExplicit);
        }

        protected T oGetAnyOneValue(TWAIN.MSG message)
        {
            T value = default;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, message, ref capability) == TWAIN.STS.SUCCESS)
            {
                value = capability.GetOneValue<T>(_dataSourceManager);

                _dataSourceManager.DsmMemFree(ref capability.hContainer);
            }

            return value;
        }

        protected TWAIN.TWON oGetContainerType(TWAIN.MSG message)
        {
            TWAIN.TW_CAPABILITY capability;
            capability.Cap = _capabilities;
            capability.ConType = (TWAIN.TWON)TWAIN.TWON_DONTCARE16;
            capability.hContainer = IntPtr.Zero;

            if (_dataSourceManager.DatCapability(TWAIN.DG.CONTROL, message, ref capability) == TWAIN.STS.SUCCESS)
            {
                _dataSourceManager.DsmMemFree(ref capability.hContainer);
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

    internal class TwainCapabilityEnum<T1, T2> : TwainCapability<T2> where T1 : Enum where T2 : struct
    {
        // ***************************************************************************************************
        // 15.03.23 : Création
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityEnum(TWAIN dataSourceManager, TWAIN.CAP capabilities) : base(dataSourceManager, capabilities)
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

            return [.. lst.Select(o => (T1)Convert.ChangeType(o, Enum.GetUnderlyingType(typeof(T1))))];
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
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityFloat(TWAIN dataSourceManager, TWAIN.CAP capabilities) : base(dataSourceManager, capabilities)
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

        public new (float minValue, float maxValue, float stepValue, float defaultValue, float currentValue) GetRange()
        {
            (TWAIN.TW_FIX32 minValue, TWAIN.TW_FIX32 maxValue, TWAIN.TW_FIX32 stepValue, TWAIN.TW_FIX32 defaultValue, TWAIN.TW_FIX32 currentValue) = base.GetRange();
            return (minValue.Get(), maxValue.Get(), stepValue.Get(), defaultValue.Get(), currentValue.Get());
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
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilityRectangleF(TWAIN dataSourceManager, TWAIN.CAP capabilities) : base(dataSourceManager, capabilities)
        {
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public new RectG GetCurrentOneValue()
        {
            return base.GetCurrentOneValue().Get();
        }

        public void SetOneValue(RectG value, bool force = false)
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