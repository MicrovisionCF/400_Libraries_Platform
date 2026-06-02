using Microvision.Geometry;

namespace Microvision.OpenGL
{
    public class GlCameraFree : GlCamera
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, camera sans contraintes
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // ----------------------------------------
        // Classe
        // ----------------------------------------


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void SetObservation(Point3D pos)
        {
            oSetObservation(pos);
        }

        public void SetPosition(Point3D pos)
        {
            oSetPosition(pos);
        }

        public void SetUpDirection(Vect3D dir)
        {
            oSetUpDirection(dir);
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