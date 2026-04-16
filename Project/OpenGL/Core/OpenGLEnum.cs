using System;

namespace Microvision.OpenGL
{
    internal enum AccumOperation : uint
    {
        Accum = OpenGLConst.GL_ACCUM,
        Load = OpenGLConst.GL_LOAD,
        Return = OpenGLConst.GL_RETURN,
        Multiple = OpenGLConst.GL_MULT,
        Add = OpenGLConst.GL_ADD
    }

    internal enum AlphaTestFunction : uint
    {
        Never = OpenGLConst.GL_NEVER,
        Less = OpenGLConst.GL_LESS,
        Equal = OpenGLConst.GL_EQUAL,
        LessThanOrEqual = OpenGLConst.GL_LEQUAL,
        Great = OpenGLConst.GL_GREATER,
        NotEqual = OpenGLConst.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGLConst.GL_GEQUAL,
        Always = OpenGLConst.GL_ALWAYS
    }

    [Flags]
    internal enum AttributeMask : uint
    {
        None = 0U,
        Current = OpenGLConst.GL_CURRENT_BIT,
        Point = OpenGLConst.GL_POINT_BIT,
        Line = OpenGLConst.GL_LINE_BIT,
        Polygon = OpenGLConst.GL_POLYGON_BIT,
        PolygonStipple = OpenGLConst.GL_POLYGON_STIPPLE_BIT,
        PixelMode = OpenGLConst.GL_PIXEL_MODE_BIT,
        Lighting = OpenGLConst.GL_LIGHTING_BIT,
        Fog = OpenGLConst.GL_FOG_BIT,
        DepthBuffer = OpenGLConst.GL_DEPTH_BUFFER_BIT,
        AccumBuffer = OpenGLConst.GL_ACCUM_BUFFER_BIT,
        StencilBuffer = OpenGLConst.GL_STENCIL_BUFFER_BIT,
        Viewport = OpenGLConst.GL_VIEWPORT_BIT,
        Transform = OpenGLConst.GL_TRANSFORM_BIT,
        Enable = OpenGLConst.GL_ENABLE_BIT,
        ColorBuffer = OpenGLConst.GL_COLOR_BUFFER_BIT,
        Hint = OpenGLConst.GL_HINT_BIT,
        Eval = OpenGLConst.GL_EVAL_BIT,
        List = OpenGLConst.GL_LIST_BIT,
        Texture = OpenGLConst.GL_TEXTURE_BIT,
        Scissor = OpenGLConst.GL_SCISSOR_BIT,
        All = OpenGLConst.GL_ALL_ATTRIB_BITS
    }

    internal enum BeginMode : uint
    {
        Points = OpenGLConst.GL_POINTS,
        Lines = OpenGLConst.GL_LINES,
        LineLoop = OpenGLConst.GL_LINE_LOOP,
        LineStrip = OpenGLConst.GL_LINE_STRIP,
        Triangles = OpenGLConst.GL_TRIANGLES,
        TriangleString = OpenGLConst.GL_TRIANGLE_STRIP,
        TriangleFan = OpenGLConst.GL_TRIANGLE_FAN,
        Quads = OpenGLConst.GL_QUADS,
        QuadStrip = OpenGLConst.GL_QUAD_STRIP,
        Polygon = OpenGLConst.GL_POLYGON
    }

    internal enum DrawElementsMode : uint
    {
        Points = OpenGLConst.GL_POINTS,
        LineStrip = OpenGLConst.GL_LINE_STRIP,
        LineLoop = OpenGLConst.GL_LINE_LOOP,
        Lines = OpenGLConst.GL_LINES,
        LineStripAdjacency = OpenGLConst.GL_LINE_STRIP_ADJACENCY,
        LinesAdjency = OpenGLConst.GL_LINES_ADJACENCY,
        TrianglesStrip = OpenGLConst.GL_TRIANGLE_STRIP,
        TriangleFan = OpenGLConst.GL_TRIANGLE_FAN,
        Triangles = OpenGLConst.GL_TRIANGLES,
        TrianglesStripAdjacency = OpenGLConst.GL_TRIANGLE_STRIP_ADJACENCY,
        TrianglesAdjacency = OpenGLConst.GL_TRIANGLES_ADJACENCY
        // Patches = GlConst.GL_PATCHES
    }

    internal enum BlendingDestinationFactor : uint
    {
        Zero = OpenGLConst.GL_ZERO,
        One = OpenGLConst.GL_ONE,
        SourceColor = OpenGLConst.GL_SRC_COLOR,
        OneMinusSourceColor = OpenGLConst.GL_ONE_MINUS_SRC_COLOR,
        SourceAlpha = OpenGLConst.GL_SRC_ALPHA,
        OneMinusSourceAlpha = OpenGLConst.GL_ONE_MINUS_SRC_ALPHA,
        DestinationAlpha = OpenGLConst.GL_DST_ALPHA,
        OneMinusDestinationAlpha = OpenGLConst.GL_ONE_MINUS_DST_ALPHA
    }

    internal enum BlendingSourceFactor : uint
    {
        DestinationColor = OpenGLConst.GL_DST_COLOR,
        OneMinusDestinationColor = OpenGLConst.GL_ONE_MINUS_DST_COLOR,
        SourceAlphaSaturate = OpenGLConst.GL_SRC_ALPHA_SATURATE,
        SourceAlpha = OpenGLConst.GL_SRC_ALPHA
    }

    internal enum ClipPlaneName : uint
    {
        ClipPlane0 = OpenGLConst.GL_CLIP_PLANE0,
        ClipPlane1 = OpenGLConst.GL_CLIP_PLANE1,
        ClipPlane2 = OpenGLConst.GL_CLIP_PLANE2,
        ClipPlane3 = OpenGLConst.GL_CLIP_PLANE3,
        ClipPlane4 = OpenGLConst.GL_CLIP_PLANE4,
        ClipPlane5 = OpenGLConst.GL_CLIP_PLANE5
    }

    internal enum FaceMode : uint
    {
        Front = OpenGLConst.GL_FRONT,
        FrontAndBack = OpenGLConst.GL_FRONT_AND_BACK,
        Back = OpenGLConst.GL_BACK
    }

    internal enum DataType : uint
    {
        Byte = OpenGLConst.GL_BYTE,
        UnsignedByte = OpenGLConst.GL_UNSIGNED_BYTE,
        Short = OpenGLConst.GL_SHORT,
        UnsignedShort = OpenGLConst.GL_UNSIGNED_SHORT,
        Int = OpenGLConst.GL_INT,
        UnsignedInt = OpenGLConst.GL_UNSIGNED_INT,
        Float = OpenGLConst.GL_FLOAT,
        TwoBytes = OpenGLConst.GL_2_BYTES,
        ThreeBytes = OpenGLConst.GL_3_BYTES,
        FourBytes = OpenGLConst.GL_4_BYTES,
        Double = OpenGLConst.GL_DOUBLE
    }

    internal enum TexCoordType : uint
    {
        Short = OpenGLConst.GL_SHORT,
        Int = OpenGLConst.GL_INT,
        Float = OpenGLConst.GL_FLOAT,
        Double = OpenGLConst.GL_DOUBLE
    }

    internal enum NormalType : uint
    {
        Byte = OpenGLConst.GL_BYTE,
        Short = OpenGLConst.GL_SHORT,
        Int = OpenGLConst.GL_INT,
        Float = OpenGLConst.GL_FLOAT,
        Double = OpenGLConst.GL_DOUBLE
    }

    internal enum ColorType : uint
    {
        Byte = OpenGLConst.GL_BYTE,
        UnsignedByte = OpenGLConst.GL_UNSIGNED_BYTE,
        Short = OpenGLConst.GL_SHORT,
        UnsignedShort = OpenGLConst.GL_UNSIGNED_SHORT,
        Int = OpenGLConst.GL_INT,
        UnsignedInt = OpenGLConst.GL_UNSIGNED_INT,
        Float = OpenGLConst.GL_FLOAT,
        Double = OpenGLConst.GL_DOUBLE
    }

    internal enum DepthFunction : uint
    {
        Never = OpenGLConst.GL_NEVER,
        Less = OpenGLConst.GL_LESS,
        Equal = OpenGLConst.GL_EQUAL,
        LessThanOrEqual = OpenGLConst.GL_LEQUAL,
        Great = OpenGLConst.GL_GREATER,
        NotEqual = OpenGLConst.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGLConst.GL_GEQUAL,
        Always = OpenGLConst.GL_ALWAYS
    }

    internal enum DrawBufferMode : uint
    {
        None = OpenGLConst.GL_NONE,
        FrontLeft = OpenGLConst.GL_FRONT_LEFT,
        FrontRight = OpenGLConst.GL_FRONT_RIGHT,
        BackLeft = OpenGLConst.GL_BACK_LEFT,
        BackRight = OpenGLConst.GL_BACK_RIGHT,
        Front = OpenGLConst.GL_FRONT,
        Back = OpenGLConst.GL_BACK,
        Left = OpenGLConst.GL_LEFT,
        Right = OpenGLConst.GL_RIGHT,
        FrontAndBack = OpenGLConst.GL_FRONT_AND_BACK,
        Auxilliary0 = OpenGLConst.GL_AUX0,
        Auxilliary1 = OpenGLConst.GL_AUX1,
        Auxilliary2 = OpenGLConst.GL_AUX2,
        Auxilliary3 = OpenGLConst.GL_AUX3
    }

    internal enum ReadBufferMode : uint
    {
        FrontLeft = OpenGLConst.GL_FRONT_LEFT,
        FrontRight = OpenGLConst.GL_FRONT_RIGHT,
        BackLeft = OpenGLConst.GL_BACK_LEFT,
        BackRight = OpenGLConst.GL_BACK_RIGHT,
        Front = OpenGLConst.GL_FRONT,
        Back = OpenGLConst.GL_BACK,
        Left = OpenGLConst.GL_LEFT,
        Right = OpenGLConst.GL_RIGHT
        // ColorAttachmentX = GlConst.GL_COLOR_ATTACHMENT0
        
    }

    internal enum ErrorCode : uint
    {
        NoError = OpenGLConst.GL_NO_ERROR,
        InvalidEnum = OpenGLConst.GL_INVALID_ENUM,
        InvalidFramebufferOperation = OpenGLConst.GL_INVALID_FRAMEBUFFER_OPERATION,
        InvalidValue = OpenGLConst.GL_INVALID_VALUE,
        InvalidOperation = OpenGLConst.GL_INVALID_OPERATION,
        StackOverflow = OpenGLConst.GL_STACK_OVERFLOW,
        StackUnderflow = OpenGLConst.GL_STACK_UNDERFLOW,
        OutOfMemory = OpenGLConst.GL_OUT_OF_MEMORY
    }

    internal enum FeedbackMode : uint
    {
        TwoD = OpenGLConst.GL_2D,
        ThreeD = OpenGLConst.GL_3D,
        FourD = OpenGLConst.GL_4D_COLOR,
        ThreeDColorTexture = OpenGLConst.GL_3D_COLOR_TEXTURE,
        FourDColorTexture = OpenGLConst.GL_4D_COLOR_TEXTURE
    }

    internal enum FeedbackToken : uint
    {
        PassThroughToken = OpenGLConst.GL_PASS_THROUGH_TOKEN,
        PointToken = OpenGLConst.GL_POINT_TOKEN,
        LineToken = OpenGLConst.GL_LINE_TOKEN,
        PolygonToken = OpenGLConst.GL_POLYGON_TOKEN,
        BitmapToken = OpenGLConst.GL_BITMAP_TOKEN,
        DrawPixelToken = OpenGLConst.GL_DRAW_PIXEL_TOKEN,
        CopyPixelToken = OpenGLConst.GL_COPY_PIXEL_TOKEN,
        LineResetToken = OpenGLConst.GL_LINE_RESET_TOKEN
    }

    internal enum FogMode : uint
    {
        Exp = OpenGLConst.GL_EXP,
        Exp2 = OpenGLConst.GL_EXP2
    }

    internal enum GetMapTarget : uint
    {
        Coeff = OpenGLConst.GL_COEFF,
        Order = OpenGLConst.GL_ORDER,
        Domain = OpenGLConst.GL_DOMAIN
    }

    internal enum GetTarget : uint
    {
        CurrentColor = OpenGLConst.GL_CURRENT_COLOR,
        CurrentIndex = OpenGLConst.GL_CURRENT_INDEX,
        CurrentNormal = OpenGLConst.GL_CURRENT_NORMAL,
        CurrentTextureCoords = OpenGLConst.GL_CURRENT_TEXTURE_COORDS,
        CurrentRasterColor = OpenGLConst.GL_CURRENT_RASTER_COLOR,
        CurrentRasterIndex = OpenGLConst.GL_CURRENT_RASTER_INDEX,
        CurrentRasterTextureCoords = OpenGLConst.GL_CURRENT_RASTER_TEXTURE_COORDS,
        CurrentRasterPosition = OpenGLConst.GL_CURRENT_RASTER_POSITION,
        CurrentRasterPositionValid = OpenGLConst.GL_CURRENT_RASTER_POSITION_VALID,
        CurrentRasterDistance = OpenGLConst.GL_CURRENT_RASTER_DISTANCE,
        PointSmooth = OpenGLConst.GL_POINT_SMOOTH,
        PointSize = OpenGLConst.GL_POINT_SIZE,
        PointSizeRange = OpenGLConst.GL_POINT_SIZE_RANGE,
        PointSizeGranularity = OpenGLConst.GL_POINT_SIZE_GRANULARITY,
        LineSmooth = OpenGLConst.GL_LINE_SMOOTH,
        LineWidth = OpenGLConst.GL_LINE_WIDTH,
        LineWidthRange = OpenGLConst.GL_LINE_WIDTH_RANGE,
        LineWidthGranularity = OpenGLConst.GL_LINE_WIDTH_GRANULARITY,
        LineStipple = OpenGLConst.GL_LINE_STIPPLE,
        LineStipplePattern = OpenGLConst.GL_LINE_STIPPLE_PATTERN,
        LineStippleRepeat = OpenGLConst.GL_LINE_STIPPLE_REPEAT,
        ListMode = OpenGLConst.GL_LIST_MODE,
        MaxListNesting = OpenGLConst.GL_MAX_LIST_NESTING,
        ListBase = OpenGLConst.GL_LIST_BASE,
        ListIndex = OpenGLConst.GL_LIST_INDEX,
        PolygonMode = OpenGLConst.GL_POLYGON_MODE,
        PolygonSmooth = OpenGLConst.GL_POLYGON_SMOOTH,
        PolygonStipple = OpenGLConst.GL_POLYGON_STIPPLE,
        EdgeFlag = OpenGLConst.GL_EDGE_FLAG,
        CullFace = OpenGLConst.GL_CULL_FACE,
        CullFaceMode = OpenGLConst.GL_CULL_FACE_MODE,
        FrontFace = OpenGLConst.GL_FRONT_FACE,
        Lighting = OpenGLConst.GL_LIGHTING,
        LightModelLocalViewer = OpenGLConst.GL_LIGHT_MODEL_LOCAL_VIEWER,
        LightModelTwoSide = OpenGLConst.GL_LIGHT_MODEL_TWO_SIDE,
        LightModelAmbient = OpenGLConst.GL_LIGHT_MODEL_AMBIENT,
        ShadeModel = OpenGLConst.GL_SHADE_MODEL,
        ColorMaterialFace = OpenGLConst.GL_COLOR_MATERIAL_FACE,
        ColorMaterialParameter = OpenGLConst.GL_COLOR_MATERIAL_PARAMETER,
        ColorMaterial = OpenGLConst.GL_COLOR_MATERIAL,
        Fog = OpenGLConst.GL_FOG,
        FogIndex = OpenGLConst.GL_FOG_INDEX,
        FogDensity = OpenGLConst.GL_FOG_DENSITY,
        FogStart = OpenGLConst.GL_FOG_START,
        FogEnd = OpenGLConst.GL_FOG_END,
        FogMode = OpenGLConst.GL_FOG_MODE,
        FogColor = OpenGLConst.GL_FOG_COLOR,
        DepthRange = OpenGLConst.GL_DEPTH_RANGE,
        DepthTest = OpenGLConst.GL_DEPTH_TEST,
        DepthWritemask = OpenGLConst.GL_DEPTH_WRITEMASK,
        DepthClearValue = OpenGLConst.GL_DEPTH_CLEAR_VALUE,
        DepthFunc = OpenGLConst.GL_DEPTH_FUNC,
        AccumClearValue = OpenGLConst.GL_ACCUM_CLEAR_VALUE,
        StencilTest = OpenGLConst.GL_STENCIL_TEST,
        StencilClearValue = OpenGLConst.GL_STENCIL_CLEAR_VALUE,
        StencilFunc = OpenGLConst.GL_STENCIL_FUNC,
        StencilValueMask = OpenGLConst.GL_STENCIL_VALUE_MASK,
        StencilFail = OpenGLConst.GL_STENCIL_FAIL,
        StencilPassDepthFail = OpenGLConst.GL_STENCIL_PASS_DEPTH_FAIL,
        StencilPassDepthPass = OpenGLConst.GL_STENCIL_PASS_DEPTH_PASS,
        StencilRef = OpenGLConst.GL_STENCIL_REF,
        StencilWritemask = OpenGLConst.GL_STENCIL_WRITEMASK,
        MatrixMode = OpenGLConst.GL_MATRIX_MODE,
        Normalize = OpenGLConst.GL_NORMALIZE,
        Viewport = OpenGLConst.GL_VIEWPORT,
        ModelviewStackDepth = OpenGLConst.GL_MODELVIEW_STACK_DEPTH,
        ProjectionStackDepth = OpenGLConst.GL_PROJECTION_STACK_DEPTH,
        TextureStackDepth = OpenGLConst.GL_TEXTURE_STACK_DEPTH,
        ModelviewMatix = OpenGLConst.GL_MODELVIEW_MATRIX,
        ProjectionMatrix = OpenGLConst.GL_PROJECTION_MATRIX,
        TextureMatrix = OpenGLConst.GL_TEXTURE_MATRIX,
        AttribStackDepth = OpenGLConst.GL_ATTRIB_STACK_DEPTH,
        ClientAttribStackDepth = OpenGLConst.GL_CLIENT_ATTRIB_STACK_DEPTH,
        AlphaTest = OpenGLConst.GL_ALPHA_TEST,
        AlphaTestFunc = OpenGLConst.GL_ALPHA_TEST_FUNC,
        AlphaTestRef = OpenGLConst.GL_ALPHA_TEST_REF,
        Dither = OpenGLConst.GL_DITHER,
        BlendDst = OpenGLConst.GL_BLEND_DST,
        BlendSrc = OpenGLConst.GL_BLEND_SRC,
        Blend = OpenGLConst.GL_BLEND,
        LogicOpMode = OpenGLConst.GL_LOGIC_OP_MODE,
        IndexLogicOp = OpenGLConst.GL_INDEX_LOGIC_OP,
        ColorLogicOp = OpenGLConst.GL_COLOR_LOGIC_OP,
        AuxBuffers = OpenGLConst.GL_AUX_BUFFERS,
        DrawBuffer = OpenGLConst.GL_DRAW_BUFFER,
        ReadBuffer = OpenGLConst.GL_READ_BUFFER,
        ScissorBox = OpenGLConst.GL_SCISSOR_BOX,
        ScissorTest = OpenGLConst.GL_SCISSOR_TEST,
        IndexClearValue = OpenGLConst.GL_INDEX_CLEAR_VALUE,
        IndexWritemask = OpenGLConst.GL_INDEX_WRITEMASK,
        ColorClearValue = OpenGLConst.GL_COLOR_CLEAR_VALUE,
        ColorWritemask = OpenGLConst.GL_COLOR_WRITEMASK,
        IndexMode = OpenGLConst.GL_INDEX_MODE,
        RgbaMode = OpenGLConst.GL_RGBA_MODE,
        DoubleBuffer = OpenGLConst.GL_DOUBLEBUFFER,
        Stereo = OpenGLConst.GL_STEREO,
        RenderMode = OpenGLConst.GL_RENDER_MODE,
        PerspectiveCorrectionHint = OpenGLConst.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmoothHint = OpenGLConst.GL_POINT_SMOOTH_HINT,
        LineSmoothHint = OpenGLConst.GL_LINE_SMOOTH_HINT,
        PolygonSmoothHint = OpenGLConst.GL_POLYGON_SMOOTH_HINT,
        FogHint = OpenGLConst.GL_FOG_HINT,
        TextureGenS = OpenGLConst.GL_TEXTURE_GEN_S,
        TextureGenT = OpenGLConst.GL_TEXTURE_GEN_T,
        TextureGenR = OpenGLConst.GL_TEXTURE_GEN_R,
        TextureGenQ = OpenGLConst.GL_TEXTURE_GEN_Q,
        PixelMapItoI = OpenGLConst.GL_PIXEL_MAP_I_TO_I,
        PixelMapStoS = OpenGLConst.GL_PIXEL_MAP_S_TO_S,
        PixelMapItoR = OpenGLConst.GL_PIXEL_MAP_I_TO_R,
        PixelMapItoG = OpenGLConst.GL_PIXEL_MAP_I_TO_G,
        PixelMapItoB = OpenGLConst.GL_PIXEL_MAP_I_TO_B,
        PixelMapItoA = OpenGLConst.GL_PIXEL_MAP_I_TO_A,
        PixelMapRtoR = OpenGLConst.GL_PIXEL_MAP_R_TO_R,
        PixelMapGtoG = OpenGLConst.GL_PIXEL_MAP_G_TO_G,
        PixelMapBtoB = OpenGLConst.GL_PIXEL_MAP_B_TO_B,
        PixelMapAtoA = OpenGLConst.GL_PIXEL_MAP_A_TO_A,
        PixelMapItoISize = OpenGLConst.GL_PIXEL_MAP_I_TO_I_SIZE,
        PixelMapStoSSize = OpenGLConst.GL_PIXEL_MAP_S_TO_S_SIZE,
        PixelMapItoRSize = OpenGLConst.GL_PIXEL_MAP_I_TO_R_SIZE,
        PixelMapItoGSize = OpenGLConst.GL_PIXEL_MAP_I_TO_G_SIZE,
        PixelMapItoBSize = OpenGLConst.GL_PIXEL_MAP_I_TO_B_SIZE,
        PixelMapItoASize = OpenGLConst.GL_PIXEL_MAP_I_TO_A_SIZE,
        PixelMapRtoRSize = OpenGLConst.GL_PIXEL_MAP_R_TO_R_SIZE,
        PixelMapGtoGSize = OpenGLConst.GL_PIXEL_MAP_G_TO_G_SIZE,
        PixelMapBtoBSize = OpenGLConst.GL_PIXEL_MAP_B_TO_B_SIZE,
        PixelMapAtoASize = OpenGLConst.GL_PIXEL_MAP_A_TO_A_SIZE,
        UnpackSwapBytes = OpenGLConst.GL_UNPACK_SWAP_BYTES,
        LsbFirst = OpenGLConst.GL_UNPACK_LSB_FIRST,
        UnpackRowLength = OpenGLConst.GL_UNPACK_ROW_LENGTH,
        UnpackSkipRows = OpenGLConst.GL_UNPACK_SKIP_ROWS,
        UnpackSkipPixels = OpenGLConst.GL_UNPACK_SKIP_PIXELS,
        UnpackAlignment = OpenGLConst.GL_UNPACK_ALIGNMENT,
        PackSwapBytes = OpenGLConst.GL_PACK_SWAP_BYTES,
        PackLsbFirst = OpenGLConst.GL_PACK_LSB_FIRST,
        PackRowLength = OpenGLConst.GL_PACK_ROW_LENGTH,
        PackSkipRows = OpenGLConst.GL_PACK_SKIP_ROWS,
        PackSkipPixels = OpenGLConst.GL_PACK_SKIP_PIXELS,
        PackAlignment = OpenGLConst.GL_PACK_ALIGNMENT,
        MapColor = OpenGLConst.GL_MAP_COLOR,
        MapStencil = OpenGLConst.GL_MAP_STENCIL,
        IndexShift = OpenGLConst.GL_INDEX_SHIFT,
        IndexOffset = OpenGLConst.GL_INDEX_OFFSET,
        RedScale = OpenGLConst.GL_RED_SCALE,
        RedBias = OpenGLConst.GL_RED_BIAS,
        ZoomX = OpenGLConst.GL_ZOOM_X,
        ZoomY = OpenGLConst.GL_ZOOM_Y,
        GreenScale = OpenGLConst.GL_GREEN_SCALE,
        GreenBias = OpenGLConst.GL_GREEN_BIAS,
        BlueScale = OpenGLConst.GL_BLUE_SCALE,
        BlueBias = OpenGLConst.GL_BLUE_BIAS,
        AlphaScale = OpenGLConst.GL_ALPHA_SCALE,
        AlphaBias = OpenGLConst.GL_ALPHA_BIAS,
        DepthScale = OpenGLConst.GL_DEPTH_SCALE,
        DepthBias = OpenGLConst.GL_DEPTH_BIAS,
        MapEvalOrder = OpenGLConst.GL_MAX_EVAL_ORDER,
        MaxLights = OpenGLConst.GL_MAX_LIGHTS,
        MaxClipPlanes = OpenGLConst.GL_MAX_CLIP_PLANES,
        MaxTextureSize = OpenGLConst.GL_MAX_TEXTURE_SIZE,
        MapPixelMapTable = OpenGLConst.GL_MAX_PIXEL_MAP_TABLE,
        MaxAttribStackDepth = OpenGLConst.GL_MAX_ATTRIB_STACK_DEPTH,
        MaxModelviewStackDepth = OpenGLConst.GL_MAX_MODELVIEW_STACK_DEPTH,
        MaxNameStackDepth = OpenGLConst.GL_MAX_NAME_STACK_DEPTH,
        MaxProjectionStackDepth = OpenGLConst.GL_MAX_PROJECTION_STACK_DEPTH,
        MaxTextureStackDepth = OpenGLConst.GL_MAX_TEXTURE_STACK_DEPTH,
        MaxViewportDims = OpenGLConst.GL_MAX_VIEWPORT_DIMS,
        MaxClientAttribStackDepth = OpenGLConst.GL_MAX_CLIENT_ATTRIB_STACK_DEPTH,
        SubpixelBits = OpenGLConst.GL_SUBPIXEL_BITS,
        IndexBits = OpenGLConst.GL_INDEX_BITS,
        RedBits = OpenGLConst.GL_RED_BITS,
        GreenBits = OpenGLConst.GL_GREEN_BITS,
        BlueBits = OpenGLConst.GL_BLUE_BITS,
        AlphaBits = OpenGLConst.GL_ALPHA_BITS,
        DepthBits = OpenGLConst.GL_DEPTH_BITS,
        StencilBits = OpenGLConst.GL_STENCIL_BITS,
        AccumRedBits = OpenGLConst.GL_ACCUM_RED_BITS,
        AccumGreenBits = OpenGLConst.GL_ACCUM_GREEN_BITS,
        AccumBlueBits = OpenGLConst.GL_ACCUM_BLUE_BITS,
        AccumAlphaBits = OpenGLConst.GL_ACCUM_ALPHA_BITS,
        NameStackDepth = OpenGLConst.GL_NAME_STACK_DEPTH,
        AutoNormal = OpenGLConst.GL_AUTO_NORMAL,
        Map1Color4 = OpenGLConst.GL_MAP1_COLOR_4,
        Map1Index = OpenGLConst.GL_MAP1_INDEX,
        Map1Normal = OpenGLConst.GL_MAP1_NORMAL,
        Map1TextureCoord1 = OpenGLConst.GL_MAP1_TEXTURE_COORD_1,
        Map1TextureCoord2 = OpenGLConst.GL_MAP1_TEXTURE_COORD_2,
        Map1TextureCoord3 = OpenGLConst.GL_MAP1_TEXTURE_COORD_3,
        Map1TextureCoord4 = OpenGLConst.GL_MAP1_TEXTURE_COORD_4,
        Map1Vertex3 = OpenGLConst.GL_MAP1_VERTEX_3,
        Map1Vertex4 = OpenGLConst.GL_MAP1_VERTEX_4,
        Map2Color4 = OpenGLConst.GL_MAP2_COLOR_4,
        Map2Index = OpenGLConst.GL_MAP2_INDEX,
        Map2Normal = OpenGLConst.GL_MAP2_NORMAL,
        Map2TextureCoord1 = OpenGLConst.GL_MAP2_TEXTURE_COORD_1,
        Map2TextureCoord2 = OpenGLConst.GL_MAP2_TEXTURE_COORD_2,
        Map2TextureCoord3 = OpenGLConst.GL_MAP2_TEXTURE_COORD_3,
        Map2TextureCoord4 = OpenGLConst.GL_MAP2_TEXTURE_COORD_4,
        Map2Vertex3 = OpenGLConst.GL_MAP2_VERTEX_3,
        Map2Vertex4 = OpenGLConst.GL_MAP2_VERTEX_4,
        Map1GridDomain = OpenGLConst.GL_MAP1_GRID_DOMAIN,
        Map1GridSegments = OpenGLConst.GL_MAP1_GRID_SEGMENTS,
        Map2GridDomain = OpenGLConst.GL_MAP2_GRID_DOMAIN,
        Map2GridSegments = OpenGLConst.GL_MAP2_GRID_SEGMENTS,
        Texture1D = OpenGLConst.GL_TEXTURE_1D,
        Texture2D = OpenGLConst.GL_TEXTURE_2D,
        FeedbackBufferPointer = OpenGLConst.GL_FEEDBACK_BUFFER_POINTER,
        FeedbackBufferSize = OpenGLConst.GL_FEEDBACK_BUFFER_SIZE,
        FeedbackBufferType = OpenGLConst.GL_FEEDBACK_BUFFER_TYPE,
        SelectionBufferPointer = OpenGLConst.GL_SELECTION_BUFFER_POINTER,
        SelectionBufferSize = OpenGLConst.GL_SELECTION_BUFFER_SIZE
    }

    internal enum StringTarget : uint
    {
        Vendor = OpenGLConst.GL_VENDOR,
        Renderer = OpenGLConst.GL_RENDERER,
        Version = OpenGLConst.GL_VERSION,
        Shading = OpenGLConst.GL_SHADING_LANGUAGE_VERSION
    }

    internal enum FrontFaceMode : uint
    {
        ClockWise = OpenGLConst.GL_CW,
        CounterClockWise = OpenGLConst.GL_CCW
    }

    internal enum HintMode : uint
    {
        DontCare = OpenGLConst.GL_DONT_CARE,
        Fastest = OpenGLConst.GL_FASTEST,
        Nicest = OpenGLConst.GL_NICEST
    }

    internal enum HintTarget : uint
    {
        PerspectiveCorrection = OpenGLConst.GL_PERSPECTIVE_CORRECTION_HINT,
        PointSmooth = OpenGLConst.GL_POINT_SMOOTH_HINT,
        LineSmooth = OpenGLConst.GL_LINE_SMOOTH_HINT,
        PolygonSmooth = OpenGLConst.GL_POLYGON_SMOOTH_HINT,
        Fog = OpenGLConst.GL_FOG_HINT
    }

    internal enum LightName : uint
    {
        Light0 = OpenGLConst.GL_LIGHT0,
        Light1 = OpenGLConst.GL_LIGHT1,
        Light2 = OpenGLConst.GL_LIGHT2,
        Light3 = OpenGLConst.GL_LIGHT3,
        Light4 = OpenGLConst.GL_LIGHT4,
        Light5 = OpenGLConst.GL_LIGHT5,
        Light6 = OpenGLConst.GL_LIGHT6,
        Light7 = OpenGLConst.GL_LIGHT7
    }

    internal enum LightParameter : uint
    {
        Ambient = OpenGLConst.GL_AMBIENT,
        Diffuse = OpenGLConst.GL_DIFFUSE,
        Specular = OpenGLConst.GL_SPECULAR,
        Position = OpenGLConst.GL_POSITION,
        SpotDirection = OpenGLConst.GL_SPOT_DIRECTION,
        SpotExponent = OpenGLConst.GL_SPOT_EXPONENT,
        SpotCutoff = OpenGLConst.GL_SPOT_CUTOFF,
        ConstantAttenuatio = OpenGLConst.GL_CONSTANT_ATTENUATION,
        LinearAttenuation = OpenGLConst.GL_LINEAR_ATTENUATION,
        QuadraticAttenuation = OpenGLConst.GL_QUADRATIC_ATTENUATION
    }

    internal enum MaterialParameter : uint
    {
        Ambient = OpenGLConst.GL_AMBIENT,
        Diffuse = OpenGLConst.GL_DIFFUSE,
        Specular = OpenGLConst.GL_SPECULAR,
        Emission = OpenGLConst.GL_EMISSION,
        Shininess = OpenGLConst.GL_SHININESS,
        AmbientAndDiffuse = OpenGLConst.GL_AMBIENT_AND_DIFFUSE,
        ColorIndexes = OpenGLConst.GL_COLOR_INDEXES
    }

    internal enum LightModelParameter : uint
    {
        LocalViewer = OpenGLConst.GL_LIGHT_MODEL_LOCAL_VIEWER,
        TwoSide = OpenGLConst.GL_LIGHT_MODEL_TWO_SIDE,
        Ambient = OpenGLConst.GL_LIGHT_MODEL_AMBIENT
    }

    internal enum LogicOp : uint
    {
        Clear = OpenGLConst.GL_CLEAR,
        And = OpenGLConst.GL_AND,
        AndReverse = OpenGLConst.GL_AND_REVERSE,
        Copy = OpenGLConst.GL_COPY,
        AndInverted = OpenGLConst.GL_AND_INVERTED,
        NoOp = OpenGLConst.GL_NOOP,
        Xor = OpenGLConst.GL_XOR,
        Or = OpenGLConst.GL_OR,
        Nor = OpenGLConst.GL_NOR,
        Equiv = OpenGLConst.GL_EQUIV,
        Invert = OpenGLConst.GL_INVERT,
        OrReverse = OpenGLConst.GL_OR_REVERSE,
        CopyInverted = OpenGLConst.GL_COPY_INVERTED,
        OrInverted = OpenGLConst.GL_OR_INVERTED,
        NAnd = OpenGLConst.GL_NAND,
        Set = OpenGLConst.GL_SET
    }

    internal enum MatrixMode : uint
    {
        Modelview = OpenGLConst.GL_MODELVIEW,
        Projection = OpenGLConst.GL_PROJECTION,
        Texture = OpenGLConst.GL_TEXTURE
    }

    internal enum PixelTransferParameterName : uint
    {
        MapColor = OpenGLConst.GL_MAP_COLOR,
        MapStencil = OpenGLConst.GL_MAP_STENCIL,
        IndexShift = OpenGLConst.GL_INDEX_SHIFT,
        IndexOffset = OpenGLConst.GL_INDEX_OFFSET,
        RedScale = OpenGLConst.GL_RED_SCALE,
        RedBias = OpenGLConst.GL_RED_BIAS,
        ZoomX = OpenGLConst.GL_ZOOM_X,
        ZoomY = OpenGLConst.GL_ZOOM_Y,
        GreenScale = OpenGLConst.GL_GREEN_SCALE,
        GreenBias = OpenGLConst.GL_GREEN_BIAS,
        BlueScale = OpenGLConst.GL_BLUE_SCALE,
        BlueBias = OpenGLConst.GL_BLUE_BIAS,
        AlphaScale = OpenGLConst.GL_ALPHA_SCALE,
        AlphaBias = OpenGLConst.GL_ALPHA_BIAS,
        DepthScale = OpenGLConst.GL_DEPTH_SCALE,
        DepthBias = OpenGLConst.GL_DEPTH_BIAS
    }

    internal enum PolygonMode : uint
    {
        Points = OpenGLConst.GL_POINT,
        Lines = OpenGLConst.GL_LINE,
        Filled = OpenGLConst.GL_FILL
    }

    internal enum RenderingMode : uint
    {
        Render = OpenGLConst.GL_RENDER,
        Feedback = OpenGLConst.GL_FEEDBACK,
        Select = OpenGLConst.GL_SELECT
    }

    internal enum ShadeModel : uint
    {
        Flat = OpenGLConst.GL_FLAT,
        Smooth = OpenGLConst.GL_SMOOTH
    }

    internal enum StencilFunction : uint
    {
        Never = OpenGLConst.GL_NEVER,
        Less = OpenGLConst.GL_LESS,
        Equal = OpenGLConst.GL_EQUAL,
        LessThanOrEqual = OpenGLConst.GL_LEQUAL,
        Great = OpenGLConst.GL_GREATER,
        NotEqual = OpenGLConst.GL_NOTEQUAL,
        GreaterThanOrEqual = OpenGLConst.GL_GEQUAL,
        Always = OpenGLConst.GL_ALWAYS
    }

    internal enum StencilOperation : uint
    {
        Keep = OpenGLConst.GL_KEEP,
        Replace = OpenGLConst.GL_REPLACE,
        Increase = OpenGLConst.GL_INCR,
        Decrease = OpenGLConst.GL_DECR,
        Zero = OpenGLConst.GL_ZERO,
        IncreaseWrap = OpenGLConst.GL_INCR_WRAP,
        DecreaseWrap = OpenGLConst.GL_DECR_WRAP,
        Invert = OpenGLConst.GL_INVERT
    }

    internal enum TextureParameter : uint
    {
        TextureWidth = OpenGLConst.GL_TEXTURE_WIDTH,
        TextureHeight = OpenGLConst.GL_TEXTURE_HEIGHT,
        TextureInternalFormat = OpenGLConst.GL_TEXTURE_INTERNAL_FORMAT,
        TextureBorderColor = OpenGLConst.GL_TEXTURE_BORDER_COLOR,
        TextureBorder = OpenGLConst.GL_TEXTURE_BORDER,
        TextureMinFilter = OpenGLConst.GL_TEXTURE_MIN_FILTER,
        TextureMagFilter = OpenGLConst.GL_TEXTURE_MAG_FILTER
    }

    internal enum TextureTarget : uint
    {
        Texture1D = OpenGLConst.GL_TEXTURE_1D,
        Texture2D = OpenGLConst.GL_TEXTURE_2D,
        Texture3D = OpenGLConst.GL_TEXTURE_3D
    }

    internal enum TextureImageTarget : uint
    {
        Texture2D = OpenGLConst.GL_TEXTURE_2D,
        ProxyTexture2D = OpenGLConst.GL_PROXY_TEXTURE_2D,
        Texture1DArray = OpenGLConst.GL_TEXTURE_1D_ARRAY,
        ProxyTexture1DArray = OpenGLConst.GL_PROXY_TEXTURE_1D_ARRAY,
        TextureRectangle = OpenGLConst.GL_TEXTURE_RECTANGLE,
        ProxyTextureRectangle = OpenGLConst.GL_PROXY_TEXTURE_RECTANGLE,
        TextureCubeMapPositiveX = OpenGLConst.GL_TEXTURE_CUBE_MAP_POSITIVE_X,
        TextureCubeMapNegativeX = OpenGLConst.GL_TEXTURE_CUBE_MAP_NEGATIVE_X,
        TextureCubeMapPositiveY = OpenGLConst.GL_TEXTURE_CUBE_MAP_POSITIVE_Y,
        TextureCubeMapNegativeY = OpenGLConst.GL_TEXTURE_CUBE_MAP_NEGATIVE_Y,
        TextureCubeMapPositiveZ = OpenGLConst.GL_TEXTURE_CUBE_MAP_POSITIVE_Z,
        TextureCubeMapNegativeZ = OpenGLConst.GL_TEXTURE_CUBE_MAP_NEGATIVE_Z,
        ProxyTextureCubeMap = OpenGLConst.GL_PROXY_TEXTURE_CUBE_MAP
    }

    internal enum BindTextureTarget : uint
    {
        Texture1D = OpenGLConst.GL_TEXTURE_1D,
        Texture2D = OpenGLConst.GL_TEXTURE_2D,
        Texture3D = OpenGLConst.GL_TEXTURE_3D,
        Texture1DArray = OpenGLConst.GL_TEXTURE_1D_ARRAY,
        Texture2DArray = OpenGLConst.GL_TEXTURE_2D_ARRAY,
        TextureRectangle = OpenGLConst.GL_TEXTURE_RECTANGLE,
        TextureCubeMap = OpenGLConst.GL_TEXTURE_CUBE_MAP,
        TextureCubeMapArray = OpenGLConst.GL_TEXTURE_CUBE_MAP_ARRAY,
        TextureBuffer = OpenGLConst.GL_TEXTURE_BUFFER
        // Texture2DMultisample = GlConst.GL_TEXTURE_2D_MULTISAMPLE
        // Texture2DMultisampleArray = GlConst.GL_TEXTURE_2D_MULTISAMPLE_ARRAY
    }

    internal enum EnableTarget : uint
    {
        
        // Les capacitées commentées dépendent de ARB_imaging (cf doc glEnable)
        
        AlphaTest = OpenGLConst.GL_ALPHA_TEST,
        AutoNormal = OpenGLConst.GL_AUTO_NORMAL,
        Blend = OpenGLConst.GL_BLEND,
        ClipPlane0 = OpenGLConst.GL_CLIP_PLANE0,
        ColorLogipOp = OpenGLConst.GL_COLOR_LOGIC_OP,
        ColorMaterial = OpenGLConst.GL_COLOR_MATERIAL,
        // ColorSum = GlConst.GL_COLOR_SUM
        // ColorTable = GlConst.GL_COLOR_TABLE
        // Convolution1D = GlConst.GL_CONVOLUTION_1D
        // Convolution2D = GlConst.GL_CONVOLUTION_2D
        CullFace = OpenGLConst.GL_CULL_FACE,
        DepthTest = OpenGLConst.GL_DEPTH_TEST,
        Dither = OpenGLConst.GL_DITHER,
        Fog = OpenGLConst.GL_FOG,
        // Histogram = GlConst.GL_HISTOGRAM
        IndexLogicOp = OpenGLConst.GL_INDEX_LOGIC_OP,
        Light0 = OpenGLConst.GL_LIGHT0,
        Lighting = OpenGLConst.GL_LIGHTING,
        LineSmooth = OpenGLConst.GL_LINE_SMOOTH,
        LineStipple = OpenGLConst.GL_LINE_STIPPLE,
        Map1Color4 = OpenGLConst.GL_MAP1_COLOR_4,
        Map1Index = OpenGLConst.GL_MAP1_INDEX,
        Map1Normal = OpenGLConst.GL_MAP1_NORMAL,
        Map1TextureCoord1 = OpenGLConst.GL_MAP1_TEXTURE_COORD_1,
        Map1TextureCoord2 = OpenGLConst.GL_MAP1_TEXTURE_COORD_2,
        Map1TextureCoord3 = OpenGLConst.GL_MAP1_TEXTURE_COORD_3,
        Map1TextureCoord4 = OpenGLConst.GL_MAP1_TEXTURE_COORD_4,
        Map1Vertex3 = OpenGLConst.GL_MAP1_VERTEX_3,
        Map1Vertex4 = OpenGLConst.GL_MAP1_VERTEX_4,
        Map2Color4 = OpenGLConst.GL_MAP2_COLOR_4,
        Map2Index = OpenGLConst.GL_MAP2_INDEX,
        Map2Normal = OpenGLConst.GL_MAP2_NORMAL,
        Map2TextureCoord1 = OpenGLConst.GL_MAP2_TEXTURE_COORD_1,
        Map2TextureCoord2 = OpenGLConst.GL_MAP2_TEXTURE_COORD_2,
        Map2TextureCoord3 = OpenGLConst.GL_MAP2_TEXTURE_COORD_3,
        Map2TextureCoord4 = OpenGLConst.GL_MAP2_TEXTURE_COORD_4,
        Map2Vertex3 = OpenGLConst.GL_MAP2_VERTEX_3,
        Map2Vertex4 = OpenGLConst.GL_MAP2_VERTEX_4,
        // Minmax = GlConst.GL_MINMAX
        MultiSample = OpenGLConst.GL_MULTISAMPLE,
        Normalize = OpenGLConst.GL_NORMALIZE,
        PointSmooth = OpenGLConst.GL_POINT_SMOOTH,
        // PointSprite = GlConst.GL_POINT_SPRITE
        PolygonOffsetFill = OpenGLConst.GL_POLYGON_OFFSET_FILL,
        Fill = OpenGLConst.GL_FILL,
        PolygonOffsetLine = OpenGLConst.GL_POLYGON_OFFSET_LINE,
        Line = OpenGLConst.GL_LINE,
        PolygonOffsetPoint = OpenGLConst.GL_POLYGON_OFFSET_POINT,
        Point = OpenGLConst.GL_POINT,
        PolygonSmooth = OpenGLConst.GL_POLYGON_SMOOTH,
        PolygonStipple = OpenGLConst.GL_POLYGON_STIPPLE,
        // PostColorMatrixColorTable = GlConst.GL_POST_COLOR_MATRIX_COLOR_TABLE
        // PostConvolutionCOlotTable = GlConst.GL_POST_CONVOLUTION_COLOR_TABLE
        // RescaleNormal = GlConst.GL_RESCALE_NORMAL
        SampleAlphaToCoverage = OpenGLConst.GL_SAMPLE_ALPHA_TO_COVERAGE,
        SampleAlphaToOne = OpenGLConst.GL_SAMPLE_ALPHA_TO_ONE,
        SampleCoverage = OpenGLConst.GL_SAMPLE_COVERAGE,
        SampleCoverageInvert = OpenGLConst.GL_SAMPLE_COVERAGE_INVERT,
        // Separable2D = GlConst.GL_SEPARABLE_2D
        ScissorTest = OpenGLConst.GL_SCISSOR_TEST,
        StencilTest = OpenGLConst.GL_STENCIL_TEST,
        Texture1D = OpenGLConst.GL_TEXTURE_1D,
        Texture2D = OpenGLConst.GL_TEXTURE_2D,
        Texture3D = OpenGLConst.GL_TEXTURE_3D,
        TextureCubeMap = OpenGLConst.GL_TEXTURE_CUBE_MAP,
        TextureGenQ = OpenGLConst.GL_TEXTURE_GEN_Q,
        TextureGenR = OpenGLConst.GL_TEXTURE_GEN_R,
        TextureGenS = OpenGLConst.GL_TEXTURE_GEN_S,
        TextureGenT = OpenGLConst.GL_TEXTURE_GEN_T,
        VertexProgramPointSize = OpenGLConst.GL_VERTEX_PROGRAM_POINT_SIZE
        // VertexProgramTwoSide = GlConst.GL_VERTEX_PROGRAM_TWO_SIDE
        
    }

    internal enum EnableClientTarget : uint
    {
        ColorArray = OpenGLConst.GL_COLOR_ARRAY,
        EdgeFlagArray = OpenGLConst.GL_EDGE_FLAG_ARRAY,
        // FogCoordArray = GlConst.GL_FOG_COORD_ARRAY
        IndexArray = OpenGLConst.GL_INDEX_ARRAY,
        NormalArray = OpenGLConst.GL_NORMAL_ARRAY,
        // SecondaryColorArray = GlConst.GL_SECONDARY_COLOR_ARRAY
        TextureCoordArray = OpenGLConst.GL_TEXTURE_COORD_ARRAY,
        VertexArray = OpenGLConst.GL_VERTEX_ARRAY
    }

    internal enum QuadricDrawStyle : uint
    {
        Point = OpenGLConst.GLU_POINT,
        Line = OpenGLConst.GLU_LINE,
        Fill = OpenGLConst.GLU_FILL,
        Silhouette = OpenGLConst.GLU_SILHOUETTE
    }

    internal enum QuadricNormal : uint
    {
        Smooth = OpenGLConst.GLU_SMOOTH,
        Flat = OpenGLConst.GLU_FLAT,
        None = OpenGLConst.GLU_NONE
    }

    internal enum QuadricOrientation : uint
    {
        Outside = OpenGLConst.GLU_OUTSIDE,
        Inside = OpenGLConst.GLU_INSIDE
    }

    internal enum Bool : uint
    {
        True = OpenGLConst.GL_TRUE,
        False = OpenGLConst.GL_FALSE
    }

    internal enum PixelFormat : uint
    {
        StencilIndex = OpenGLConst.GL_STENCIL_INDEX,
        DepthComponent = OpenGLConst.GL_DEPTH_COMPONENT,
        // DepthStencil = GlConst.GL_DEPTH_STENCIL
        Red = OpenGLConst.GL_RED,
        Green = OpenGLConst.GL_GREEN,
        Blue = OpenGLConst.GL_BLUE,
        Rgb = OpenGLConst.GL_RGB,
        Bgr = OpenGLConst.GL_BGR,
        Rgba = OpenGLConst.GL_RGBA,
        Bgra = OpenGLConst.GL_BGRA
    }

    internal enum PixelType : uint
    {
        UnsignedByte = OpenGLConst.GL_UNSIGNED_BYTE,
        Byte = OpenGLConst.GL_BYTE,
        UnsugnedShort = OpenGLConst.GL_UNSIGNED_SHORT,
        Short = OpenGLConst.GL_SHORT,
        UnsignedInt = OpenGLConst.GL_UNSIGNED_INT,
        Int = OpenGLConst.GL_INT,
        HalfFloat = OpenGLConst.GL_HALF_FLOAT,
        Float = OpenGLConst.GL_FLOAT,
        UnsignedByte_3_3_2 = OpenGLConst.GL_UNSIGNED_BYTE_3_3_2,
        UnsignedByte_2_3_3_REV = OpenGLConst.GL_UNSIGNED_BYTE_2_3_3_REV,
        UnsignedByte_5_6_5 = OpenGLConst.GL_UNSIGNED_SHORT_5_6_5,
        UnsignedByte_5_6_5_REV = OpenGLConst.GL_UNSIGNED_SHORT_5_6_5_REV,
        UnsignedByte_4_4_4_4 = OpenGLConst.GL_UNSIGNED_SHORT_4_4_4_4,
        UnsignedByte_4_4_4_4_REV = OpenGLConst.GL_UNSIGNED_SHORT_4_4_4_4_REV,
        UnsignedByte_5_5_5_1 = OpenGLConst.GL_UNSIGNED_SHORT_5_5_5_1,
        UnsignedByte_1_5_5_5_REV = OpenGLConst.GL_UNSIGNED_SHORT_1_5_5_5_REV,
        UnsignedInt_8_8_8_8 = OpenGLConst.GL_UNSIGNED_INT_8_8_8_8,
        UnsignedInt_8_8_8_8_REV = OpenGLConst.GL_UNSIGNED_INT_8_8_8_8_REV,
        UnsignedInt_10_10_10_2 = OpenGLConst.GL_UNSIGNED_INT_10_10_10_2,
        UnsignedInt_2_10_10_10_REV = OpenGLConst.GL_UNSIGNED_INT_2_10_10_10_REV,
        // UnsignedInt_24_8 = GlConst.GL_UNSIGNED_INT_24_8
        UnsignedInt_10F_11F_11F_REV = OpenGLConst.GL_UNSIGNED_INT_10F_11F_11F_REV,
        UnsignedInt_5_9_9_9_REV = OpenGLConst.GL_UNSIGNED_INT_5_9_9_9_REV
        // Float32UnsignedInt_24_8_REV = GlConst.GL_FLOAT_32_UNSIGNED_INT_24_8_REV
    }
}