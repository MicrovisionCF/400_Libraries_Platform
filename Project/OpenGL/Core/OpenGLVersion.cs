namespace Microvision.OpenGL
{
    internal enum OpenGLVersion
    {
        [Version(1, 1)]
        OpenGL1_1,
        [Version(1, 2)]
        OpenGL1_2,
        [Version(1, 3)]
        OpenGL1_3,
        [Version(1, 4)]
        OpenGL1_4,
        [Version(1, 5)]
        OpenGL1_5,
        [Version(2, 0)]
        OpenGL2_0,
        [Version(2, 1)]
        OpenGL2_1,
        [Version(3, 0)]
        OpenGL3_0,
        [Version(3, 1)]
        OpenGL3_1,
        [Version(3, 2)]
        OpenGL3_2,
        [Version(3, 3)]
        OpenGL3_3,
        [Version(4, 0)]
        OpenGL4_0,
        [Version(4, 1)]
        OpenGL4_1,
        [Version(4, 2)]
        OpenGL4_2,
        [Version(4, 3)]
        OpenGL4_3,
        [Version(4, 4)]
        OpenGL4_4
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal class VersionAttribute : Attribute
    {
        // ***************************************************************************************************
        // 15.05.19 : Création, représente une version d'OpenGL
        // 21.11.19 : (libs 2.2) Correction Minor et Major (qui bouclaient...)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private int _major;
        private int _minor;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public VersionAttribute(int major, int minor)
        {
            _major = major;
            _minor = minor;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public int Major => _major;

        public int Minor => _minor;


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static VersionAttribute GetVersionAttribute<TEnum>(TEnum enumeration) where TEnum : struct, Enum
        {
            return enumeration.GetType().GetMember(enumeration.ToNameString()).Single().GetCustomAttributes(typeof(VersionAttribute), false).OfType<VersionAttribute>().FirstOrDefault();
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public bool IsAtLeastVersion(int major, int minor)
        {
            return (_major > major) || (_major == major && _minor > minor);
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