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
        // 02.06.26 : (libs 4.0)
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

        public static List<WIA.DeviceCommand> ToList(this WIA.DeviceCommands commands)
        {
            List<WIA.DeviceCommand> lst = [];

            for (int i = 0; i < commands.Count; i++)
                lst.Add(commands[1 + i]);

            return lst;
        }

        public static List<WIA.DeviceEvent> ToList(this WIA.DeviceEvents events)
        {
            List<WIA.DeviceEvent> lst = [];

            for (int i = 0; i < events.Count; i++)
                lst.Add(events[1 + i]);

            return lst;
        }

        public static List<WIA.DeviceInfo> ToList(this WIA.DeviceInfos infos)
        {
            List<WIA.DeviceInfo> lst = [];

            for (int i = 0; i < infos.Count; i++)
                lst.Add(infos[1 + i]);

            return lst;
        }

        public static List<string> ToList(this WIA.Formats formats)
        {
            List<string> lst = [];

            for (int i = 0; i < formats.Count; i++)
                lst.Add(formats[1 + i]);

            return lst;
        }

        public static List<WIA.Item> ToList(this WIA.Items items)
        {
            List<WIA.Item> lst = [];

            for (int i = 0; i < items.Count; i++)
                lst.Add(items[1 + i]);

            return lst;
        }

        public static List<WIA.Property> ToList(this WIA.Properties properties)
        {
            List<WIA.Property> lst = [];

            for (int i = 0; i < properties.Count; i++)
                lst.Add(properties[1 + i]);

            return lst;
        }

        public static List<T> ToList<T>(this WIA.Vector vector)
        {
            List<T> lst = [];

            for (int i = 0; i < vector.Count; i++)
                lst.Add((T)vector.get_Item(1 + i));

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