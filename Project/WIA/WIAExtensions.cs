using System.Collections.Generic;

namespace Microvision.Scanners
{
    internal static class WIAExtensions
    {
        // ***************************************************************************************************
        // 19.09.16 : (création) transformation des pseudos listes de WIA (avec index de base 1) en vraies List(of)
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
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

        public static List<WIA.DeviceCommand> ToList(this WIA.DeviceCommands cmds)
        {
            List<WIA.DeviceCommand> lst = new List<WIA.DeviceCommand>();

            for (int i = 0; i < cmds.Count; i++)
                lst.Add(cmds[1 + i]);

            return lst;
        }

        public static List<WIA.DeviceEvent> ToList(this WIA.DeviceEvents evts)
        {
            List<WIA.DeviceEvent> lst = new List<WIA.DeviceEvent>();

            for (int i = 0; i < evts.Count; i++)
                lst.Add(evts[1 + i]);

            return lst;
        }

        public static List<WIA.DeviceInfo> ToList(this WIA.DeviceInfos infs)
        {
            List<WIA.DeviceInfo> lst = new List<WIA.DeviceInfo>();

            for (int i = 0; i < infs.Count; i++)
                lst.Add(infs[1 + i]);

            return lst;
        }

        public static List<string> ToList(this WIA.Formats fmts)
        {
            List<string> lst = new List<string>();

            for (int i = 0; i < fmts.Count; i++)
                lst.Add(fmts[1 + i]);
            
            return lst;
        }

        public static List<WIA.Item> ToList(this WIA.Items itms)
        {
            List<WIA.Item> lst = new List<WIA.Item>();

            for (int i = 0; i < itms.Count; i++)
                lst.Add(itms[1 + i]);

            return lst;
        }

        public static List<WIA.Property> ToList(this WIA.Properties prps)
        {
            List<WIA.Property> lst = new List<WIA.Property>();
            
            for (int i = 0; i < prps.Count; i++)
                lst.Add(prps[1 + i]);

            return lst;
        }

        public static List<T> ToList<T>(this WIA.Vector vct)
        {
            List<T> lst = new List<T>();
            
            for (int i = 0; i < vct.Count; i++)
                lst.Add((T)vct.get_Item(1 + i));
            
            return lst;
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