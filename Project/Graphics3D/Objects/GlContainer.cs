using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlContainer : GlObject
    {
        // ***************************************************************************************************
        // 29.04.19 : Création, objet 3D encapsuleur d'une collection d'objets 3D
        // 21.11.19 : (libs 2.2)
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private readonly List<GlObject> _children;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlContainer()
        {
            _children = [];
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void AddSubItem(GlObject obj)
        {
            oAddSubItem(obj);
        }

        public void Clear()
        {
            oClear();
        }

        public void RemoveSubItem(GlObject obj)
        {
            oRemoveSubItem(obj);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected virtual void oAddSubItem(GlObject obj)
        {
            obj.AddLife();

            if (_children.Count > 0 && _children[_children.Count - 1].IsTransparent)
                _children.Insert(_children.Count - 1, obj);
            else
                _children.Add(obj);
        }

        protected virtual void oClear()
        {
            _children.ForEach(o => o.Dispose());
            _children.Clear();
            _children.TrimExcess();
        }

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _children.ForEach(o => o.Dispose());
            
            base.oDispose(isExplicit);
        }

        protected override bool oIsTransparent()
        {
            return _children.Exists(o => o.IsTransparent);
        }

        protected virtual void oRemoveSubItem(GlObject obj)
        {
            if (_children.Remove(obj)) obj.Dispose();
        }

        protected override void oRender(OpenGLContext gl)
        {
            _children.ForEach(o => o.Render(gl));
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