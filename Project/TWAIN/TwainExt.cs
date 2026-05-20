using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    internal static class TWAIN_TW_FIX32_EXT
    {
        // ***************************************************************************************************
        // 15.03.23 : Création, méthodes d'extension pour la structure TWAIN.TW_FIX32.
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static float Get(this TWAIN.TW_FIX32 fix32)
        {
            // cf. TWAIN 2.5 page 327/766 § TW_FIX32
            return fix32.Whole + fix32.Frac / 65536.0f;
        }

        public static void Set(ref this TWAIN.TW_FIX32 fix32, float value)
        {
            // cf. TWAIN 2.5 page 327/766 § TW_FIX32
            int i32 = (value * 65536 + 0.5f).ToFloorInt();

            fix32.Whole = (short)(i32 >> 16);
            fix32.Frac = (ushort)(i32 & 0x0000ffff);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


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

    internal static class TWAIN_TW_FRAME_EXT
    {
        // ***************************************************************************************************
        // 16.03.23 : Création
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static RectangleF Get(this TWAIN.TW_FRAME frame)
        {
            float x = frame.Left.Get();
            float y = frame.Top.Get();
            float w = frame.Right.Get() - x;
            float h = frame.Bottom.Get() - y;

            return new RectangleF(x, y, w, h);
        }

        public static void Set(ref this TWAIN.TW_FRAME frame, RectangleF value)
        {
            frame.Left.Set(value.X);
            frame.Top.Set(value.Y);
            frame.Right.Set(value.X + value.Width);
            frame.Bottom.Set(value.Y + value.Height);
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


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

    internal static class TWAIN_TW_CAPABILITY_EXT
    {
        // ***************************************************************************************************
        // 15.03.23 : Création, méthodes d'extension pour la structure TWAIN.TW_CAPABILITY.
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static List<T> GetArray<T>(this TWAIN.TW_CAPABILITY capability, TWAIN dsm) where T : struct
        {
            IntPtr ptr = dsm.DsmMemLock(capability.hContainer);

            List<T> values = new List<T>();
            TWAIN.TW_ARRAY array = Marshal.PtrToStructure<TWAIN.TW_ARRAY>(ptr);
            ptr += Marshal.SizeOf(array);

            for (int i = 0; i < array.NumItems; i++)
            {
                T value = Marshal.PtrToStructure<T>(ptr);
                values.Add(value);
                ptr += Marshal.SizeOf(values[i]);
            }

            dsm.DsmMemUnlock(capability.hContainer);

            return values;
        }

        public static List<T> GetEnumeration<T>(this TWAIN.TW_CAPABILITY capability, TWAIN dsm) where T : struct
        {
            IntPtr ptr = dsm.DsmMemLock(capability.hContainer);

            List<T> values = new List<T>();
            TWAIN.TW_ENUMERATION enumeration = Marshal.PtrToStructure<TWAIN.TW_ENUMERATION>(ptr);
            ptr += Marshal.SizeOf(enumeration);

            for (int i = 0; i < enumeration.NumItems; i++)
            {
                values.Add(Marshal.PtrToStructure<T>(ptr));
                ptr += Marshal.SizeOf(values[i]);
            }

            dsm.DsmMemUnlock(capability.hContainer);

            return values;
        }

        public static T GetOneValue<T>(this TWAIN.TW_CAPABILITY capability, TWAIN dsm) where T : struct
        {
            IntPtr ptr = dsm.DsmMemLock(capability.hContainer);

            TWAIN.TW_ONEVALUE oneValue = Marshal.PtrToStructure<TWAIN.TW_ONEVALUE>(ptr);
            ptr += Marshal.SizeOf(oneValue);
            T output = Marshal.PtrToStructure<T>(ptr);

            dsm.DsmMemUnlock(capability.hContainer);

            return output;
        }

        public static (T min, T max, T step, T def, T cur) GetRange<T>(this TWAIN.TW_CAPABILITY capability, TWAIN dsm) where T : struct
        {
            IntPtr ptr = dsm.DsmMemLock(capability.hContainer);

            TWAIN.TW_RANGE range = Marshal.PtrToStructure<TWAIN.TW_RANGE>(ptr);
            T min = Marshal.PtrToStructure<T>(ptr + (int)Marshal.OffsetOf(typeof(TWAIN.TW_RANGE), "MinValue"));
            T max = Marshal.PtrToStructure<T>(ptr + (int)Marshal.OffsetOf(typeof(TWAIN.TW_RANGE), "MaxValue"));
            T step = Marshal.PtrToStructure<T>(ptr + (int)Marshal.OffsetOf(typeof(TWAIN.TW_RANGE), "StepSize"));
            T def = Marshal.PtrToStructure<T>(ptr + (int)Marshal.OffsetOf(typeof(TWAIN.TW_RANGE), "DefaultValue"));
            T cur = Marshal.PtrToStructure<T>(ptr + (int)Marshal.OffsetOf(typeof(TWAIN.TW_RANGE), "CurrentValue"));

            dsm.DsmMemUnlock(capability.hContainer);

            return (min, max, step, def, cur);
        }

        public static bool SetOneValue<T>(this TWAIN.TW_CAPABILITY capability, TWAIN dsm, T value) where T : struct
        {
            IntPtr ptr = dsm.DsmMemLock(capability.hContainer);

            TWAIN.TW_ONEVALUE oneValue = Marshal.PtrToStructure<TWAIN.TW_ONEVALUE>(ptr);
            ptr += Marshal.SizeOf(oneValue);
            Marshal.StructureToPtr(value, ptr, false);
            bool ok = dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.SET, ref capability) == TWAIN.STS.SUCCESS;

            dsm.DsmMemUnlock(capability.hContainer);

            return ok;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


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