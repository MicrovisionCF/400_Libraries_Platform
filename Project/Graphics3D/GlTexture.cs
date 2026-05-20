using System.Drawing.Imaging;

using Microvision.Geometry;
using Microvision.OpenGL;
using Microvision.Types;

namespace Microvision.Graphics3D
{
    public class GlTexture : Citizen
    {
        // ***************************************************************************************************
        // 25.04.19 : Création, image que l'on peut appliquer sur des objets
        // 21.11.19 : (libs 2.2)
        // 13.10.20 : Test contexte existant
        // 13.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private readonly OpenGLContext? _gl;

        private uint _glTextureID;
        private SizeI _size;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlTexture(OpenGLContext? gl, Bitmap img)
        {
            _gl = gl;

            _size = img.Size;

            if (_gl is not null) _glTextureID = _gl.GenTexture();

            oCheckImageSize(img);
            oSetImage(img);
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public SizeI Size => _size;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void Bind(OpenGLContext gl)
        {
            gl?.BindTexture(BindTextureTarget.Texture2D, _glTextureID);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected void oCheckImageSize(Image bmp)
        {
            if (_gl is not null)
            {
                int textureMaxSize = _gl.GetIntegerValue(GetTarget.MaxTextureSize);

                if (bmp.Width > textureMaxSize || bmp.Height > textureMaxSize)
                {
                    throw new ArgumentException("OpenGL Texture image size allowed = " + textureMaxSize + "x" + textureMaxSize + ", image provided size = " + bmp.Width + "x" + bmp.Height);
                }
            }
        }

        protected override void oDispose(bool isExplicit)
        {
            if (_gl is not null && _glTextureID != 0)
            {
                _gl.DeleteTexture(_glTextureID);
                _glTextureID = 0;
            }

            base.oDispose(isExplicit);
        }

        protected void oSetImage(Bitmap img)
        {
            if (_gl is not null)
            {
                BitmapData bmpData = img.LockBits(new RectI(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                _gl.BindTexture(BindTextureTarget.Texture2D, _glTextureID);
                _gl.TexImage2D(TextureImageTarget.Texture2D, 0, (uint)OpenGL.PixelFormat.Rgba, bmpData.Width, bmpData.Height, 0, (uint)OpenGL.PixelFormat.Bgra, (uint)DataType.UnsignedByte, bmpData.Scan0);
                img.UnlockBits(bmpData);

                _gl.TexParameter(TextureTarget.Texture2D, TextureParameter.TextureMinFilter, OpenGLConst.GL_LINEAR);
                _gl.TexParameter(TextureTarget.Texture2D, TextureParameter.TextureMagFilter, OpenGLConst.GL_LINEAR);
            }
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