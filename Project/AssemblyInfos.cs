using System;
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

        public string AssemblyName => _assembly.GetName().Name ?? throw new InvalidOperationException("Assembly name is null.");

        public Version AssemblyVersion => _assembly.GetName().Version ?? throw new InvalidOperationException("Assembly version is null.");

        public string Company => zGetAttribute<AssemblyCompanyAttribute>(_assembly).Company;

        public string Copyright => zGetAttribute<AssemblyCopyrightAttribute>(_assembly).Copyright;

        public string Description => zGetAttribute<AssemblyDescriptionAttribute>(_assembly).Description;

        public Version FileVersion => new Version(zGetAttribute<AssemblyFileVersionAttribute>(_assembly).Version);

        public string Product => zGetAttribute<AssemblyProductAttribute>(_assembly).Product;

        public string ProductTitle => zGetAttribute<AssemblyTitleAttribute>(_assembly).Title;

        public string Trademark => zGetAttribute<AssemblyTrademarkAttribute>(_assembly).Trademark;


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