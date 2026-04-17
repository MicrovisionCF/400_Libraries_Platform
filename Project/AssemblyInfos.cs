using System.Reflection;

namespace Microvision.Platform
{
    public class AssemblyInfos
    {
        // ***************************************************************************************************
        // 05.01.22 : Création, encapsulation des informations de l'assembly
        // ***************************************************************************************************

        private Assembly _assembly;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public AssemblyInfos(Assembly assembly)
        {
            _assembly = assembly;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public String AssemblyName => _assembly.GetName().Name;

        public Version AssemblyVersion => _assembly.GetName().Version;

        public String Company => zGetAttribute<AssemblyCompanyAttribute>(_assembly).Company;

        public String Copyright => zGetAttribute<AssemblyCopyrightAttribute>(_assembly).Copyright;

        public String Description => zGetAttribute<AssemblyDescriptionAttribute>(_assembly).Description;

        public Version FileVersion => new Version(zGetAttribute<AssemblyFileVersionAttribute>(_assembly).Version);

        public String Product => zGetAttribute<AssemblyProductAttribute>(_assembly).Product;

        public String ProductTitle => zGetAttribute<AssemblyTitleAttribute>(_assembly).Title;

        public String Trademark => zGetAttribute<AssemblyTrademarkAttribute>(_assembly).Trademark;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static T zGetAttribute<T>(Assembly assembly) where T : Attribute
        {
            return (T)assembly.GetCustomAttributes(typeof(T), false)[0];
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}