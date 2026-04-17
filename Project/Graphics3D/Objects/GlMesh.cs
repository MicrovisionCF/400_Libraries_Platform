using Microvision.Collections;
using Microvision.Graphic;
using Microvision.OpenGL;

namespace Microvision.Graphics3D
{
    public class GlMesh : GlObjectLineable
    {
        // ***************************************************************************************************
        // 24.04.19 : Création, représentation d'une surface quadrillée dont chaque point a une altitude différente
        // 21.11.19 : (libs 2.2)
        // ***************************************************************************************************

        private GlTexture _texture;

        private float _maxZ, _minZ;
        private Array2D<float> _depth;
        private float[] _vertices, _textCoords, _normal, _colors;
        private uint[] _indices;
        private uint[] _indicesLines;

        private Point3D _origin;
        private PointG _center;
        private SizeG _size;
        private List<float> _xPositions, _yPositions;
        private float _zFactor;
        private bool _showNormals;

        private float _colorOpacity;
        private float _colorFade;
        private bool _colorInverted;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public GlMesh()
        {
            oSetMaterial(new xGlMaterial(Color.White, 0.5f, 1, 0, 0.65f, 0.5f));
            _depth = new Array2D<float>(0, 0);
            _zFactor = 1;
            _origin = new Point3D();
            _showNormals = false;

            _colorOpacity = 0;
            _colorFade = 0;
            _colorInverted = false;

            _xPositions = null;
            _yPositions = null;
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public float AutoColorFade
        {
            get => _colorFade;

            set
            {
                if (_colorFade != value)
                {
                    _colorFade = value;
                    _colors = zChangeOpacity(zCalcColors(_depth, _colorInverted, _colorFade), _colorOpacity);
                }
            }
        }

        public bool AutoColorInverted
        {
            get => _colorInverted;

            set
            {
                if (_colorInverted != value)
                {
                    _colorInverted = value;
                    if (_colors is not null) _colors = zChangeOpacity(_colors, _colorOpacity);
                }
            }
        }

        public float AutoColorOpacity
        {
            get => _colorOpacity;

            set
            {
                if (_colorOpacity != value)
                {
                    _colorOpacity = value;
                    if (_colors is not null) _colors = zChangeOpacity(_colors, _colorOpacity);
                }
            }
        }

        public PointG Center => _center;

        public float MaxZ => _maxZ * _zFactor;

        public float MinZ => _minZ * _zFactor;

        public SizeG Size => _size;


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public void SetDepth(float[,] depth, SizeG size)
        {
            _xPositions = null;
            _yPositions = null;
            _size = size;
            oSetDepth(depth);
        }

        public void SetMesh(Point3D origin, float[,] depth, SizeG size)
        {
            _xPositions = null;
            _yPositions = null;
            _origin = origin;
            _size = size;
            oSetDepth(depth);
        }

        public void SetMesh(Point3D origin, float[,] depth, List<float> xSteps, List<float> ySteps)
        {
            _xPositions = xSteps;
            _yPositions = ySteps;
            _origin = origin;
            oSetDepth(depth);
        }

        public void SetTexture(GlTexture texture)
        {
            _texture?.Dispose();
            _texture = texture;
            _texture?.AddLife();
        }

        public void SetZFactor(float factor)
        {
            if (_zFactor != factor)
            {
                oSetZFactor(factor);
            }
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            _depth = default;
            _vertices = null;
            _textCoords = null;
            _normal = null;
            _indices = null;

            if (_texture is not null)
            {
                if (isExplicit) _texture.Dispose();
                _texture = null;
            }

            base.oDispose(isExplicit);
        }

        protected override void oRender(OpenGLContext gl)
        {
            if (_indices is not null)
            {
                if (_texture is not null && _colorOpacity < 1)
                {
                    gl.Enable(EnableTarget.Texture2D);

                    gl.EnableClientState(EnableClientTarget.VertexArray);
                    gl.EnableClientState(EnableClientTarget.TextureCoordArray);
                    gl.EnableClientState(EnableClientTarget.NormalArray);

                    gl.VertexPointer(3, 0, _vertices);
                    gl.TexCoordPointer(2, (TexCoordType)DataType.Float, 0, _textCoords);
                    gl.NormalPointer((NormalType)DataType.Float, 0, _normal);

                    gl.DrawElements((DrawElementsMode)BeginMode.Triangles, _indices.Length, _indices);

                    gl.DisableClientState(EnableClientTarget.NormalArray);
                    gl.DisableClientState(EnableClientTarget.TextureCoordArray);
                    gl.DisableClientState(EnableClientTarget.VertexArray);

                    gl.Disable(EnableTarget.Texture2D);
                    gl.Flush();
                }

                if (_colorOpacity == 0 && _texture is null)
                {
                    gl.EnableClientState(EnableClientTarget.VertexArray);
                    gl.EnableClientState(EnableClientTarget.NormalArray);

                    gl.VertexPointer(3, 0, _vertices);
                    gl.NormalPointer((NormalType)DataType.Float, 0, _normal);

                    gl.DrawElements((DrawElementsMode)BeginMode.Triangles, _indices.Length, _indices);

                    gl.DisableClientState(EnableClientTarget.NormalArray);
                    gl.DisableClientState(EnableClientTarget.VertexArray);
                    gl.Flush();
                }

                if (_colorOpacity > 0)
                {
                    gl.Enable(EnableTarget.ColorMaterial);

                    gl.EnableClientState(EnableClientTarget.VertexArray);
                    gl.EnableClientState(EnableClientTarget.NormalArray);
                    gl.EnableClientState(EnableClientTarget.ColorArray);

                    gl.VertexPointer(3, 0, _vertices);
                    gl.ColorPointer(4, PixelType.Float, 0, _colors);
                    gl.NormalPointer((NormalType)DataType.Float, 0, _normal);

                    gl.DrawElements((DrawElementsMode)BeginMode.Triangles, _indices.Length, _indices);

                    gl.DisableClientState(EnableClientTarget.ColorArray);
                    gl.DisableClientState(EnableClientTarget.NormalArray);
                    gl.DisableClientState(EnableClientTarget.VertexArray);

                    gl.Disable(EnableTarget.ColorMaterial);
                    gl.Flush();
                }

                if (_showNormals)
                {
                    gl.Begin(BeginMode.Lines);

                    float normalSize = (_size.w + _size.h) / 50;
                    for (int j = 0; j < _depth.rowsnb; j++)
                    {
                        for (int i = 0; i < _depth.colsnb; i++)
                        {
                            gl.Vertex(_vertices[j * 3 * _depth.colsnb + i * 3 + 0],
                                        _vertices[j * 3 * _depth.colsnb + i * 3 + 1],
                                        _vertices[j * 3 * _depth.colsnb + i * 3 + 2]);
                            gl.Vertex(_vertices[j * 3 * _depth.colsnb + i * 3 + 0] + _normal[j * 3 * _depth.colsnb + i * 3 + 0] * normalSize,
                                        _vertices[j * 3 * _depth.colsnb + i * 3 + 1] + _normal[j * 3 * _depth.colsnb + i * 3 + 1] * normalSize,
                                        _vertices[j * 3 * _depth.colsnb + i * 3 + 2] + _normal[j * 3 * _depth.colsnb + i * 3 + 2] * normalSize);
                        }
                    }

                    gl.End();
                }
            }
        }

        protected override void oRenderLines(OpenGLContext gl)
        {
            if (_indicesLines is not null)
            {
                gl.EnableClientState(EnableClientTarget.VertexArray);

                // TODO3D : Dessin des lignes 0.1 au dessus pour s'affranchir des collisions avec le remplissage et bien voir les lignes
                // La méthode reste à améliorer parce que "0.1" n'est surement pas une réponse universelle...
                gl.VertexPointer(3, 0, zOffset(_vertices, 0.1f));
                gl.DrawElements((DrawElementsMode)BeginMode.Lines, _indicesLines.Length, _indicesLines);

                gl.VertexPointer(3, 0, zOffset(_vertices, -0.1f));
                gl.DrawElements((DrawElementsMode)BeginMode.Lines, _indicesLines.Length, _indicesLines);

                gl.DisableClientState(EnableClientTarget.VertexArray);
                gl.Flush();
            }
        }

        protected void oSetDepth(float[,] depth)
        {
            if (depth is not null)
            {
                _depth = new Array2D<float>(depth);
                _maxZ = zMax(_depth);
                _minZ = zMin(_depth);

                int w = _depth.colsnb;
                int h = _depth.rowsnb;

                if (_xPositions is not null)
                {
                    _vertices = zCalcVertices(_origin, _depth, _xPositions, _yPositions, _zFactor);
                    _textCoords = zCalcTextureCoordinates(w, h, _xPositions, _yPositions);
                }
                else
                {
                    _vertices = zCalcVertices(_origin, _depth, _size.w / (w - 1), _size.h / (h - 1), _zFactor);
                    _textCoords = zCalcTextureCoordinates(w, h);
                }

                _indices = zCalcIndices(w, h);
                _indicesLines = zCalcIndicesLines(w, h);
                _normal = zCalcNormal(_vertices, w, h);
                _colors = zChangeOpacity(zCalcColors(_depth, _colorInverted, _colorFade), _colorOpacity);

                _center = new PointG(_size.w / 2, _size.h / 2);
            }
            else
            {
                _depth = default;
                _indices = null;
                _indicesLines = null;
                _normal = null;
                _colors = null;
                _center = new PointG();
                _maxZ = 0;
                _minZ = 0;
            }
        }

        protected void oSetZFactor(float value)
        {
            _zFactor = value;
            _vertices = zCalcVertices(_origin, _depth, _size.w / _depth.colsnb, _size.h / _depth.rowsnb, _zFactor);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static float[] zCalcColors(Array2D<float> depth, bool inverted, float fade)
        {
            float min = zMin(depth);
            float max = zMax(depth);

            List<Color> listColors = new[] { Color.Blue, Color.Cyan, Color.Lime, Color.Yellow, Color.Red }.ToList();
            if (fade > 0) listColors = listColors.Select(o => (Color)HColor.Lighter(o, fade)).ToList();
            if (inverted) listColors.Reverse();

            float[] colors = new float[depth.colsnb * depth.rowsnb * 4];

            for (int j = 0; j < depth.rowsnb; j++)
            {
                for (int i = 0; i < depth.colsnb; i++)
                {
                    HColor col = zzColorFromScale(min, max, depth[i, j], listColors);
                    colors[(i + j * depth.colsnb) * 4 + 0] = (col.red / 255f / 2f);
                    colors[(i + j * depth.colsnb) * 4 + 1] = (col.green / 255f / 2f);
                    colors[(i + j * depth.colsnb) * 4 + 2] = (col.blue / 255f / 2f);
                    colors[(i + j * depth.colsnb) * 4 + 3] = 1;
                }
            }

            return colors;
        }

        private static uint[] zCalcIndices(int w, int h)
        {
            uint[] indices = new uint[(w - 1) * (h - 1) * 6];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (x < w - 1 && y < h - 1)
                    {
                        indices[(x + (w - 1) * y) * 6 + 0] = (uint)(x + 0 + (y + 0) * w);
                        indices[(x + (w - 1) * y) * 6 + 1] = (uint)(x + 1 + (y + 0) * w);
                        indices[(x + (w - 1) * y) * 6 + 2] = (uint)(x + 1 + (y + 1) * w);

                        indices[(x + (w - 1) * y) * 6 + 3] = (uint)(x + 1 + (y + 1) * w);
                        indices[(x + (w - 1) * y) * 6 + 4] = (uint)(x + 0 + (y + 1) * w);
                        indices[(x + (w - 1) * y) * 6 + 5] = (uint)(x + 0 + (y + 0) * w);
                    }
                }
            }

            return indices;
        }

        private static uint[] zCalcIndicesLines(int w, int h)
        {
            uint[] indices = new uint[((w - 1) * h + w * (h - 1)) * 2];
            int cpt = 0;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y <= h - 2; y++)
                {
                    indices[cpt] = (uint)(x + y * w);
                    cpt++;
                    indices[cpt] = (uint)(x + (y + 1) * w);
                    cpt++;
                }
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x <= w - 2; x++)
                {
                    indices[cpt] = (uint)(x + y * w);
                    cpt++;
                    indices[cpt] = (uint)(x + 1 + y * w);
                    cpt++;
                }
            }

            return indices;
        }

        private static float[] zCalcNormal(float[] vertices, int w, int h)
        {
            float[] normalsV = new float[w * h * 3];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (x > 0 && y > 0 && x < w - 1 && y < h - 1)
                    {
                        Point3D p0 = zGetPoint(vertices, w, x + 0, y + 0);
                        Point3D p1 = zGetPoint(vertices, w, x + 0, y - 1);
                        Point3D p2 = zGetPoint(vertices, w, x + 1, y + 0);
                        Point3D p3 = zGetPoint(vertices, w, x + 0, y + 1);
                        Point3D p4 = zGetPoint(vertices, w, x - 1, y + 0);

                        Vect3D n1 = zCalcNormal(p2, p0, p1);
                        Vect3D n2 = zCalcNormal(p3, p0, p2);
                        Vect3D n3 = zCalcNormal(p4, p0, p3);
                        Vect3D n4 = zCalcNormal(p1, p0, p4);

                        normalsV[(x + w * y) * 3 + 0] = (n1.x + n2.x + n3.x + n4.x) / 4;
                        normalsV[(x + w * y) * 3 + 1] = (n1.y + n2.y + n3.y + n4.y) / 4;
                        normalsV[(x + w * y) * 3 + 2] = (n1.z + n2.z + n3.z + n4.z) / 4;
                    }
                    else
                    {
                        normalsV[(x + w * y) * 3 + 0] = 0;
                        normalsV[(x + w * y) * 3 + 1] = 0;
                        normalsV[(x + w * y) * 3 + 2] = 1;
                    }
                }
            }

            return normalsV;
        }

        private static Vect3D zCalcNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            Vect3D normal = Vect3D.VectorProduct(p2 - p1, p3 - p2);
            normal.Normalize();

            return normal;
        }

        private static float[] zCalcTextureCoordinates(int w, int h)
        {
            float[] tex = new float[w * h * 2];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    tex[(x + w * y) * 2 + 0] = ((float)x / (w - 1));
                    tex[(x + w * y) * 2 + 1] = ((float)y / (h - 1));
                }
            }

            return tex;
        }

        private static float[] zCalcTextureCoordinates(int w, int h, List<float> xPositions, List<float> yPositions)
        {
            float totalW = xPositions[xPositions.Count - 1] - xPositions[0];
            float totalH = yPositions[yPositions.Count - 1] - yPositions[0];

            float[] tex = new float[w * h * 2];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    tex[(x + w * y) * 2 + 0] = (xPositions[x] - xPositions[0]) / (totalW - 1);
                    tex[(x + w * y) * 2 + 1] = (yPositions[y] - yPositions[0]) / (totalH - 1);
                }
            }

            return tex;
        }

        private static float[] zCalcVertices(Point3D origin, Array2D<float> depth, float xf, float yf, float zf)
        {
            int w = depth.colsnb;
            int h = depth.rowsnb;

            float[] vertices = new float[w * h * 3];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    vertices[(x + w * y) * 3 + 0] = origin.x + x * xf;
                    vertices[(x + w * y) * 3 + 1] = origin.y + y * yf;
                    vertices[(x + w * y) * 3 + 2] = origin.z + depth[x, y] * zf;
                }
            }

            return vertices;
        }

        private static float[] zCalcVertices(Point3D origin, Array2D<float> depth, List<float> xPositions, List<float> yPositions, float zf)
        {
            int w = depth.colsnb;
            int h = depth.rowsnb;

            float[] vertices = new float[w * h * 3];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    vertices[(x + w * y) * 3 + 0] = origin.x + xPositions[x];
                    vertices[(x + w * y) * 3 + 1] = origin.y + yPositions[y];
                    vertices[(x + w * y) * 3 + 2] = origin.z + depth[x, y] * zf;
                }
            }

            return vertices;
        }

        private static float[] zChangeOpacity(float[] cols, float opacity)
        {
            for (int i = 3; i < cols.Length; i += 4)
                cols[i] = opacity;

            return cols;
        }

        private static Point3D zGetPoint(float[] vertices, int w, int x, int y)
        {
            return new Point3D(vertices[(x + 0 + w * (y + 0)) * 3 + 0],
                               vertices[(x + 0 + w * (y + 0)) * 3 + 1],
                               vertices[(x + 0 + w * (y + 0)) * 3 + 2]);
        }

        private static float zMax(Array2D<float> mat)
        {
            int w = mat.colsnb;
            int h = mat.rowsnb;
            float max = float.MinValue;

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    max = Math.Max(max, mat[x, y]);

            return max;
        }

        private static float zMin(Array2D<float> mat)
        {
            int w = mat.colsnb;
            int h = mat.rowsnb;
            float min = float.MaxValue;

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    min = Math.Min(min, mat[x, y]);

            return min;
        }

        private static float[] zOffset(float[] pt, float offset)
        {
            float[] output = new float[pt.Length];
            Array.Copy(pt, output, pt.Length);

            for (int i = 2; i < pt.Length; i += 3)
                output[i] += offset;

            return output;
        }

        private static Color zzColorFromScale(float min, float max, float val, List<Color> colors)
        {
            Color output = colors[0];
            float colorRange = (max - min) / (colors.Count - 1);
            float normalized = val - min;
            float inRangeFactor = normalized % colorRange / colorRange;

            if (colorRange > 0)
            {
                Color c1 = colors[(normalized / colorRange).ToFloorInt()];
                Color c2 = colors[(normalized / colorRange).ToCeilingInt()];

                output = Color.FromArgb((c1.R * (1 - inRangeFactor) + c2.R * inRangeFactor).ToRoundInt(),
                                        (c1.G * (1 - inRangeFactor) + c2.G * inRangeFactor).ToRoundInt(),
                                        (c1.B * (1 - inRangeFactor) + c2.B * inRangeFactor).ToRoundInt());
            }

            return output;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}