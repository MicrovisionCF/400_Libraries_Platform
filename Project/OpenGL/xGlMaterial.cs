using Microvision.Graphic;

namespace Microvision.OpenGL
{
    public struct xGlMaterial
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, contient les paramètres de couleur et de réflection aux différents éclairages
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        public bool valid;

        public HColor color;
        public float ambientRatio, diffuseRatio, emissionRatio, specularRatio;
        public float specularIntensity;

        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public xGlMaterial(HColor color) : this(color, 0.5f, 0.75f, 0, 0.3f, 0.3f)
        {
        }

        public xGlMaterial(HColor color, float ambientRatio, float diffuseRatio, float emissionRatio, float specularRatio, float specularIntensity)
        {
            this.color = color;

            this.ambientRatio = ambientRatio;
            this.diffuseRatio = diffuseRatio;
            this.emissionRatio = emissionRatio;
            this.specularRatio = specularRatio;
            this.specularIntensity = specularIntensity;

            valid = true;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public readonly bool IsTransparent => color.Alpha < 255;

        public readonly bool IsValid => valid;

        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public readonly void Apply(OpenGLContext gl)
        {
            gl.MaterialGlobal(color, ambientRatio, diffuseRatio, emissionRatio, specularRatio, specularIntensity);
        }

        public static xGlMaterial Invalid()
        {
            return new xGlMaterial();
        }

        public static xGlMaterial Flat(HColor color)
        {
            return new xGlMaterial(color, 1, 0, 0, 0, 0);
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