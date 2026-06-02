namespace Microvision.OpenGL
{
    internal static class OpenGLConst
    {
        // ***************************************************************************************************
        // 14.05.19 : Création, importation brute des constantes OpenGL
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // 02.06.26 : (libs 4.0)
        // ***************************************************************************************************

        // OpenGL Version Identifier
        public const uint GL_VERSION_1_1 = 1U;

        // AccumOp
        public const uint GL_ACCUM = 0x100U;
        public const uint GL_LOAD = 0x101U;
        public const uint GL_RETURN = 0x102U;
        public const uint GL_MULT = 0x103U;
        public const uint GL_ADD = 0x104U;

        // Alpha functions
        public const uint GL_NEVER = 0x200U;
        public const uint GL_LESS = 0x201U;
        public const uint GL_EQUAL = 0x202U;
        public const uint GL_LEQUAL = 0x203U;
        public const uint GL_GREATER = 0x204U;
        public const uint GL_NOTEQUAL = 0x205U;
        public const uint GL_GEQUAL = 0x206U;
        public const uint GL_ALWAYS = 0x207U;

        // AttribMask
        public const uint GL_CURRENT_BIT = 0x1U;
        public const uint GL_POINT_BIT = 0x2U;
        public const uint GL_LINE_BIT = 0x4U;
        public const uint GL_POLYGON_BIT = 0x8U;
        public const uint GL_POLYGON_STIPPLE_BIT = 0x10U;
        public const uint GL_PIXEL_MODE_BIT = 0x20U;
        public const uint GL_LIGHTING_BIT = 0x40U;
        public const uint GL_FOG_BIT = 0x80U;
        public const uint GL_DEPTH_BUFFER_BIT = 0x100U;
        public const uint GL_ACCUM_BUFFER_BIT = 0x200U;
        public const uint GL_STENCIL_BUFFER_BIT = 0x400U;
        public const uint GL_VIEWPORT_BIT = 0x800U;
        public const uint GL_TRANSFORM_BIT = 0x1000U;
        public const uint GL_ENABLE_BIT = 0x2000U;
        public const uint GL_COLOR_BUFFER_BIT = 0x4000U;
        public const uint GL_HINT_BIT = 0x8000U;
        public const uint GL_EVAL_BIT = 0x10000U;
        public const uint GL_LIST_BIT = 0x20000U;
        public const uint GL_TEXTURE_BIT = 0x40000U;
        public const uint GL_SCISSOR_BIT = 0x80000U;
        public const uint GL_ALL_ATTRIB_BITS = 0xFFFFFU;

        // BeginMode
        public const uint GL_POINTS = 0x0U;
        public const uint GL_LINES = 0x1U;
        public const uint GL_LINE_LOOP = 0x2U;
        public const uint GL_LINE_STRIP = 0x3U;
        public const uint GL_TRIANGLES = 0x4U;
        public const uint GL_TRIANGLE_STRIP = 0x5U;
        public const uint GL_TRIANGLE_FAN = 0x6U;
        public const uint GL_QUADS = 0x7U;
        public const uint GL_QUAD_STRIP = 0x8U;
        public const uint GL_POLYGON = 0x9U;

        // BlendingFactorDest
        public const uint GL_ZERO = 0U;
        public const uint GL_ONE = 1U;
        public const uint GL_SRC_COLOR = 0x300U;
        public const uint GL_ONE_MINUS_SRC_COLOR = 0x301U;
        public const uint GL_SRC_ALPHA = 0x302U;
        public const uint GL_ONE_MINUS_SRC_ALPHA = 0x303U;
        public const uint GL_DST_ALPHA = 0x304U;
        public const uint GL_ONE_MINUS_DST_ALPHA = 0x305U;

        // BlendingFactorSrc
        public const uint GL_DST_COLOR = 0x306U;
        public const uint GL_ONE_MINUS_DST_COLOR = 0x307U;
        public const uint GL_SRC_ALPHA_SATURATE = 0x308U;

        // Boolean
        public const uint GL_TRUE = 1U;
        public const uint GL_FALSE = 0U;

        // ClipPlaneName
        public const uint GL_CLIP_PLANE0 = 0x3000U;
        public const uint GL_CLIP_PLANE1 = 0x3001U;
        public const uint GL_CLIP_PLANE2 = 0x3002U;
        public const uint GL_CLIP_PLANE3 = 0x3003U;
        public const uint GL_CLIP_PLANE4 = 0x3004U;
        public const uint GL_CLIP_PLANE5 = 0x3005U;

        // DataType
        public const uint GL_BYTE = 0x1400U;
        public const uint GL_UNSIGNED_BYTE = 0x1401U;
        public const uint GL_SHORT = 0x1402U;
        public const uint GL_UNSIGNED_SHORT = 0x1403U;
        public const uint GL_INT = 0x1404U;
        public const uint GL_UNSIGNED_INT = 0x1405U;
        public const uint GL_FLOAT = 0x1406U;
        public const uint GL_2_BYTES = 0x1407U;
        public const uint GL_3_BYTES = 0x1408U;
        public const uint GL_4_BYTES = 0x1409U;
        public const uint GL_DOUBLE = 0x140AU;
        public const uint GL_HALF_FLOAT = 0x140BU;

        // DrawBufferMode
        public const uint GL_NONE = 0U;
        public const uint GL_FRONT_LEFT = 0x400U;
        public const uint GL_FRONT_RIGHT = 0x401U;
        public const uint GL_BACK_LEFT = 0x402U;
        public const uint GL_BACK_RIGHT = 0x403U;
        public const uint GL_FRONT = 0x404U;
        public const uint GL_BACK = 0x405U;
        public const uint GL_LEFT = 0x406U;
        public const uint GL_RIGHT = 0x407U;
        public const uint GL_FRONT_AND_BACK = 0x408U;
        public const uint GL_AUX0 = 0x409U;
        public const uint GL_AUX1 = 0x40AU;
        public const uint GL_AUX2 = 0x40BU;
        public const uint GL_AUX3 = 0x40CU;

        // ErrorCode
        public const uint GL_NO_ERROR = 0U;
        public const uint GL_INVALID_ENUM = 0x500U;
        public const uint GL_INVALID_VALUE = 0x501U;
        public const uint GL_INVALID_OPERATION = 0x502U;
        public const uint GL_STACK_OVERFLOW = 0x503U;
        public const uint GL_STACK_UNDERFLOW = 0x504U;
        public const uint GL_OUT_OF_MEMORY = 0x505U;
        public const uint GL_INVALID_FRAMEBUFFER_OPERATION = 0x506U;

        // FeedBackMode
        public const uint GL_2D = 0x600U;
        public const uint GL_3D = 0x601U;
        public const uint GL_4D_COLOR = 0x602U;
        public const uint GL_3D_COLOR_TEXTURE = 0x603U;
        public const uint GL_4D_COLOR_TEXTURE = 0x604U;

        // FeedBackToken
        public const uint GL_PASS_THROUGH_TOKEN = 0x700U;
        public const uint GL_POINT_TOKEN = 0x701U;
        public const uint GL_LINE_TOKEN = 0x702U;
        public const uint GL_POLYGON_TOKEN = 0x703U;
        public const uint GL_BITMAP_TOKEN = 0x704U;
        public const uint GL_DRAW_PIXEL_TOKEN = 0x705U;
        public const uint GL_COPY_PIXEL_TOKEN = 0x706U;
        public const uint GL_LINE_RESET_TOKEN = 0x707U;

        // FogMode
        public const uint GL_EXP = 0x800U;
        public const uint GL_EXP2 = 0x801U;

        // FrontFaceDirection
        public const uint GL_CW = 0x900U;
        public const uint GL_CCW = 0x901U;

        // GetMapTarget
        public const uint GL_COEFF = 0xA00U;
        public const uint GL_ORDER = 0xA01U;
        public const uint GL_DOMAIN = 0xA02U;

        // GetTarget
        public const uint GL_CURRENT_COLOR = 0xB00U;
        public const uint GL_CURRENT_INDEX = 0xB01U;
        public const uint GL_CURRENT_NORMAL = 0xB02U;
        public const uint GL_CURRENT_TEXTURE_COORDS = 0xB03U;
        public const uint GL_CURRENT_RASTER_COLOR = 0xB04U;
        public const uint GL_CURRENT_RASTER_INDEX = 0xB05U;
        public const uint GL_CURRENT_RASTER_TEXTURE_COORDS = 0xB06U;
        public const uint GL_CURRENT_RASTER_POSITION = 0xB07U;
        public const uint GL_CURRENT_RASTER_POSITION_VALID = 0xB08U;
        public const uint GL_CURRENT_RASTER_DISTANCE = 0xB09U;
        public const uint GL_POINT_SMOOTH = 0xB10U;
        public const uint GL_POINT_SIZE = 0xB11U;
        public const uint GL_POINT_SIZE_RANGE = 0xB12U;
        public const uint GL_POINT_SIZE_GRANULARITY = 0xB13U;
        public const uint GL_LINE_SMOOTH = 0xB20U;
        public const uint GL_LINE_WIDTH = 0xB21U;
        public const uint GL_LINE_WIDTH_RANGE = 0xB22U;
        public const uint GL_LINE_WIDTH_GRANULARITY = 0xB23U;
        public const uint GL_LINE_STIPPLE = 0xB24U;
        public const uint GL_LINE_STIPPLE_PATTERN = 0xB25U;
        public const uint GL_LINE_STIPPLE_REPEAT = 0xB26U;
        public const uint GL_LIST_MODE = 0xB30U;
        public const uint GL_MAX_LIST_NESTING = 0xB31U;
        public const uint GL_LIST_BASE = 0xB32U;
        public const uint GL_LIST_INDEX = 0xB33U;
        public const uint GL_POLYGON_MODE = 0xB40U;
        public const uint GL_POLYGON_SMOOTH = 0xB41U;
        public const uint GL_POLYGON_STIPPLE = 0xB42U;
        public const uint GL_EDGE_FLAG = 0xB43U;
        public const uint GL_CULL_FACE = 0xB44U;
        public const uint GL_CULL_FACE_MODE = 0xB45U;
        public const uint GL_FRONT_FACE = 0xB46U;
        public const uint GL_LIGHTING = 0xB50U;
        public const uint GL_LIGHT_MODEL_LOCAL_VIEWER = 0xB51U;
        public const uint GL_LIGHT_MODEL_TWO_SIDE = 0xB52U;
        public const uint GL_LIGHT_MODEL_AMBIENT = 0xB53U;
        public const uint GL_SHADE_MODEL = 0xB54U;
        public const uint GL_COLOR_MATERIAL_FACE = 0xB55U;
        public const uint GL_COLOR_MATERIAL_PARAMETER = 0xB56U;
        public const uint GL_COLOR_MATERIAL = 0xB57U;
        public const uint GL_FOG = 0xB60U;
        public const uint GL_FOG_INDEX = 0xB61U;
        public const uint GL_FOG_DENSITY = 0xB62U;
        public const uint GL_FOG_START = 0xB63U;
        public const uint GL_FOG_END = 0xB64U;
        public const uint GL_FOG_MODE = 0xB65U;
        public const uint GL_FOG_COLOR = 0xB66U;
        public const uint GL_DEPTH_RANGE = 0xB70U;
        public const uint GL_DEPTH_TEST = 0xB71U;
        public const uint GL_DEPTH_WRITEMASK = 0xB72U;
        public const uint GL_DEPTH_CLEAR_VALUE = 0xB73U;
        public const uint GL_DEPTH_FUNC = 0xB74U;
        public const uint GL_ACCUM_CLEAR_VALUE = 0xB80U;
        public const uint GL_STENCIL_TEST = 0xB90U;
        public const uint GL_STENCIL_CLEAR_VALUE = 0xB91U;
        public const uint GL_STENCIL_FUNC = 0xB92U;
        public const uint GL_STENCIL_VALUE_MASK = 0xB93U;
        public const uint GL_STENCIL_FAIL = 0xB94U;
        public const uint GL_STENCIL_PASS_DEPTH_FAIL = 0xB95U;
        public const uint GL_STENCIL_PASS_DEPTH_PASS = 0xB96U;
        public const uint GL_STENCIL_REF = 0xB97U;
        public const uint GL_STENCIL_WRITEMASK = 0xB98U;
        public const uint GL_MATRIX_MODE = 0xBA0U;
        public const uint GL_NORMALIZE = 0xBA1U;
        public const uint GL_VIEWPORT = 0xBA2U;
        public const uint GL_MODELVIEW_STACK_DEPTH = 0xBA3U;
        public const uint GL_PROJECTION_STACK_DEPTH = 0xBA4U;
        public const uint GL_TEXTURE_STACK_DEPTH = 0xBA5U;
        public const uint GL_MODELVIEW_MATRIX = 0xBA6U;
        public const uint GL_PROJECTION_MATRIX = 0xBA7U;
        public const uint GL_TEXTURE_MATRIX = 0xBA8U;
        public const uint GL_ATTRIB_STACK_DEPTH = 0xBB0U;
        public const uint GL_CLIENT_ATTRIB_STACK_DEPTH = 0xBB1U;
        public const uint GL_ALPHA_TEST = 0xBC0U;
        public const uint GL_ALPHA_TEST_FUNC = 0xBC1U;
        public const uint GL_ALPHA_TEST_REF = 0xBC2U;
        public const uint GL_DITHER = 0xBD0U;
        public const uint GL_BLEND_DST = 0xBE0U;
        public const uint GL_BLEND_SRC = 0xBE1U;
        public const uint GL_BLEND = 0xBE2U;
        public const uint GL_LOGIC_OP_MODE = 0xBF0U;
        public const uint GL_INDEX_LOGIC_OP = 0xBF1U;
        public const uint GL_COLOR_LOGIC_OP = 0xBF2U;
        public const uint GL_AUX_BUFFERS = 0xC00U;
        public const uint GL_DRAW_BUFFER = 0xC01U;
        public const uint GL_READ_BUFFER = 0xC02U;
        public const uint GL_SCISSOR_BOX = 0xC10U;
        public const uint GL_SCISSOR_TEST = 0xC11U;
        public const uint GL_INDEX_CLEAR_VALUE = 0xC20U;
        public const uint GL_INDEX_WRITEMASK = 0xC21U;
        public const uint GL_COLOR_CLEAR_VALUE = 0xC22U;
        public const uint GL_COLOR_WRITEMASK = 0xC23U;
        public const uint GL_INDEX_MODE = 0xC30U;
        public const uint GL_RGBA_MODE = 0xC31U;
        public const uint GL_DOUBLEBUFFER = 0xC32U;
        public const uint GL_STEREO = 0xC33U;
        public const uint GL_RENDER_MODE = 0xC40U;
        public const uint GL_PERSPECTIVE_CORRECTION_HINT = 0xC50U;
        public const uint GL_POINT_SMOOTH_HINT = 0xC51U;
        public const uint GL_LINE_SMOOTH_HINT = 0xC52U;
        public const uint GL_POLYGON_SMOOTH_HINT = 0xC53U;
        public const uint GL_FOG_HINT = 0xC54U;
        public const uint GL_TEXTURE_GEN_S = 0xC60U;
        public const uint GL_TEXTURE_GEN_T = 0xC61U;
        public const uint GL_TEXTURE_GEN_R = 0xC62U;
        public const uint GL_TEXTURE_GEN_Q = 0xC63U;
        public const uint GL_PIXEL_MAP_I_TO_I = 0xC70U;
        public const uint GL_PIXEL_MAP_S_TO_S = 0xC71U;
        public const uint GL_PIXEL_MAP_I_TO_R = 0xC72U;
        public const uint GL_PIXEL_MAP_I_TO_G = 0xC73U;
        public const uint GL_PIXEL_MAP_I_TO_B = 0xC74U;
        public const uint GL_PIXEL_MAP_I_TO_A = 0xC75U;
        public const uint GL_PIXEL_MAP_R_TO_R = 0xC76U;
        public const uint GL_PIXEL_MAP_G_TO_G = 0xC77U;
        public const uint GL_PIXEL_MAP_B_TO_B = 0xC78U;
        public const uint GL_PIXEL_MAP_A_TO_A = 0xC79U;
        public const uint GL_PIXEL_MAP_I_TO_I_SIZE = 0xCB0U;
        public const uint GL_PIXEL_MAP_S_TO_S_SIZE = 0xCB1U;
        public const uint GL_PIXEL_MAP_I_TO_R_SIZE = 0xCB2U;
        public const uint GL_PIXEL_MAP_I_TO_G_SIZE = 0xCB3U;
        public const uint GL_PIXEL_MAP_I_TO_B_SIZE = 0xCB4U;
        public const uint GL_PIXEL_MAP_I_TO_A_SIZE = 0xCB5U;
        public const uint GL_PIXEL_MAP_R_TO_R_SIZE = 0xCB6U;
        public const uint GL_PIXEL_MAP_G_TO_G_SIZE = 0xCB7U;
        public const uint GL_PIXEL_MAP_B_TO_B_SIZE = 0xCB8U;
        public const uint GL_PIXEL_MAP_A_TO_A_SIZE = 0xCB9U;
        public const uint GL_UNPACK_SWAP_BYTES = 0xCF0U;
        public const uint GL_UNPACK_LSB_FIRST = 0xCF1U;
        public const uint GL_UNPACK_ROW_LENGTH = 0xCF2U;
        public const uint GL_UNPACK_SKIP_ROWS = 0xCF3U;
        public const uint GL_UNPACK_SKIP_PIXELS = 0xCF4U;
        public const uint GL_UNPACK_ALIGNMENT = 0xCF5U;
        public const uint GL_PACK_SWAP_BYTES = 0xD00U;
        public const uint GL_PACK_LSB_FIRST = 0xD01U;
        public const uint GL_PACK_ROW_LENGTH = 0xD02U;
        public const uint GL_PACK_SKIP_ROWS = 0xD03U;
        public const uint GL_PACK_SKIP_PIXELS = 0xD04U;
        public const uint GL_PACK_ALIGNMENT = 0xD05U;
        public const uint GL_MAP_COLOR = 0xD10U;
        public const uint GL_MAP_STENCIL = 0xD11U;
        public const uint GL_INDEX_SHIFT = 0xD12U;
        public const uint GL_INDEX_OFFSET = 0xD13U;
        public const uint GL_RED_SCALE = 0xD14U;
        public const uint GL_RED_BIAS = 0xD15U;
        public const uint GL_ZOOM_X = 0xD16U;
        public const uint GL_ZOOM_Y = 0xD17U;
        public const uint GL_GREEN_SCALE = 0xD18U;
        public const uint GL_GREEN_BIAS = 0xD19U;
        public const uint GL_BLUE_SCALE = 0xD1AU;
        public const uint GL_BLUE_BIAS = 0xD1BU;
        public const uint GL_ALPHA_SCALE = 0xD1CU;
        public const uint GL_ALPHA_BIAS = 0xD1DU;
        public const uint GL_DEPTH_SCALE = 0xD1EU;
        public const uint GL_DEPTH_BIAS = 0xD1FU;
        public const uint GL_MAX_EVAL_ORDER = 0xD30U;
        public const uint GL_MAX_LIGHTS = 0xD31U;
        public const uint GL_MAX_CLIP_PLANES = 0xD32U;
        public const uint GL_MAX_TEXTURE_SIZE = 0xD33U;
        public const uint GL_MAX_PIXEL_MAP_TABLE = 0xD34U;
        public const uint GL_MAX_ATTRIB_STACK_DEPTH = 0xD35U;
        public const uint GL_MAX_MODELVIEW_STACK_DEPTH = 0xD36U;
        public const uint GL_MAX_NAME_STACK_DEPTH = 0xD37U;
        public const uint GL_MAX_PROJECTION_STACK_DEPTH = 0xD38U;
        public const uint GL_MAX_TEXTURE_STACK_DEPTH = 0xD39U;
        public const uint GL_MAX_VIEWPORT_DIMS = 0xD3AU;
        public const uint GL_MAX_CLIENT_ATTRIB_STACK_DEPTH = 0xD3BU;
        public const uint GL_SUBPIXEL_BITS = 0xD50U;
        public const uint GL_INDEX_BITS = 0xD51U;
        public const uint GL_RED_BITS = 0xD52U;
        public const uint GL_GREEN_BITS = 0xD53U;
        public const uint GL_BLUE_BITS = 0xD54U;
        public const uint GL_ALPHA_BITS = 0xD55U;
        public const uint GL_DEPTH_BITS = 0xD56U;
        public const uint GL_STENCIL_BITS = 0xD57U;
        public const uint GL_ACCUM_RED_BITS = 0xD58U;
        public const uint GL_ACCUM_GREEN_BITS = 0xD59U;
        public const uint GL_ACCUM_BLUE_BITS = 0xD5AU;
        public const uint GL_ACCUM_ALPHA_BITS = 0xD5BU;
        public const uint GL_NAME_STACK_DEPTH = 0xD70U;
        public const uint GL_AUTO_NORMAL = 0xD80U;
        public const uint GL_MAP1_COLOR_4 = 0xD90U;
        public const uint GL_MAP1_INDEX = 0xD91U;
        public const uint GL_MAP1_NORMAL = 0xD92U;
        public const uint GL_MAP1_TEXTURE_COORD_1 = 0xD93U;
        public const uint GL_MAP1_TEXTURE_COORD_2 = 0xD94U;
        public const uint GL_MAP1_TEXTURE_COORD_3 = 0xD95U;
        public const uint GL_MAP1_TEXTURE_COORD_4 = 0xD96U;
        public const uint GL_MAP1_VERTEX_3 = 0xD97U;
        public const uint GL_MAP1_VERTEX_4 = 0xD98U;
        public const uint GL_MAP2_COLOR_4 = 0xDB0U;
        public const uint GL_MAP2_INDEX = 0xDB1U;
        public const uint GL_MAP2_NORMAL = 0xDB2U;
        public const uint GL_MAP2_TEXTURE_COORD_1 = 0xDB3U;
        public const uint GL_MAP2_TEXTURE_COORD_2 = 0xDB4U;
        public const uint GL_MAP2_TEXTURE_COORD_3 = 0xDB5U;
        public const uint GL_MAP2_TEXTURE_COORD_4 = 0xDB6U;
        public const uint GL_MAP2_VERTEX_3 = 0xDB7U;
        public const uint GL_MAP2_VERTEX_4 = 0xDB8U;
        public const uint GL_MAP1_GRID_DOMAIN = 0xDD0U;
        public const uint GL_MAP1_GRID_SEGMENTS = 0xDD1U;
        public const uint GL_MAP2_GRID_DOMAIN = 0xDD2U;
        public const uint GL_MAP2_GRID_SEGMENTS = 0xDD3U;
        public const uint GL_TEXTURE_1D = 0xDE0U;
        public const uint GL_TEXTURE_2D = 0xDE1U;
        public const uint GL_FEEDBACK_BUFFER_POINTER = 0xDF0U;
        public const uint GL_FEEDBACK_BUFFER_SIZE = 0xDF1U;
        public const uint GL_FEEDBACK_BUFFER_TYPE = 0xDF2U;
        public const uint GL_SELECTION_BUFFER_POINTER = 0xDF3U;
        public const uint GL_SELECTION_BUFFER_SIZE = 0xDF4U;

        // GetTextureParameter
        public const uint GL_TEXTURE_WIDTH = 0x1000U;
        public const uint GL_TEXTURE_HEIGHT = 0x1001U;
        public const uint GL_TEXTURE_INTERNAL_FORMAT = 0x1003U;
        public const uint GL_TEXTURE_BORDER_COLOR = 0x1004U;
        public const uint GL_TEXTURE_BORDER = 0x1005U;

        // HintMode
        public const uint GL_DONT_CARE = 0x1100U;
        public const uint GL_FASTEST = 0x1101U;
        public const uint GL_NICEST = 0x1102U;

        // LightName
        public const uint GL_LIGHT0 = 0x4000U;
        public const uint GL_LIGHT1 = 0x4001U;
        public const uint GL_LIGHT2 = 0x4002U;
        public const uint GL_LIGHT3 = 0x4003U;
        public const uint GL_LIGHT4 = 0x4004U;
        public const uint GL_LIGHT5 = 0x4005U;
        public const uint GL_LIGHT6 = 0x4006U;
        public const uint GL_LIGHT7 = 0x4007U;

        // LightParameter
        public const uint GL_AMBIENT = 0x1200U;
        public const uint GL_DIFFUSE = 0x1201U;
        public const uint GL_SPECULAR = 0x1202U;
        public const uint GL_POSITION = 0x1203U;
        public const uint GL_SPOT_DIRECTION = 0x1204U;
        public const uint GL_SPOT_EXPONENT = 0x1205U;
        public const uint GL_SPOT_CUTOFF = 0x1206U;
        public const uint GL_CONSTANT_ATTENUATION = 0x1207U;
        public const uint GL_LINEAR_ATTENUATION = 0x1208U;
        public const uint GL_QUADRATIC_ATTENUATION = 0x1209U;

        // ListMode
        public const uint GL_COMPILE = 0x1300U;
        public const uint GL_COMPILE_AND_EXECUTE = 0x1301U;

        // LogicOp
        public const uint GL_CLEAR = 0x1500U;
        public const uint GL_AND = 0x1501U;
        public const uint GL_AND_REVERSE = 0x1502U;
        public const uint GL_COPY = 0x1503U;
        public const uint GL_AND_INVERTED = 0x1504U;
        public const uint GL_NOOP = 0x1505U;
        public const uint GL_XOR = 0x1506U;
        public const uint GL_OR = 0x1507U;
        public const uint GL_NOR = 0x1508U;
        public const uint GL_EQUIV = 0x1509U;
        public const uint GL_INVERT = 0x150AU;
        public const uint GL_OR_REVERSE = 0x150BU;
        public const uint GL_COPY_INVERTED = 0x150CU;
        public const uint GL_OR_INVERTED = 0x150DU;
        public const uint GL_NAND = 0x150EU;
        public const uint GL_SET = 0x150FU;

        // MaterialParameter
        public const uint GL_EMISSION = 0x1600U;
        public const uint GL_SHININESS = 0x1601U;
        public const uint GL_AMBIENT_AND_DIFFUSE = 0x1602U;
        public const uint GL_COLOR_INDEXES = 0x1603U;

        // MatrixMode
        public const uint GL_MODELVIEW = 0x1700U;
        public const uint GL_PROJECTION = 0x1701U;
        public const uint GL_TEXTURE = 0x1702U;

        // PixelCopyType
        public const uint GL_COLOR = 0x1800U;
        public const uint GL_DEPTH = 0x1801U;
        public const uint GL_STENCIL = 0x1802U;

        // PixelFormat
        public const uint GL_COLOR_INDEX = 0x1900U;
        public const uint GL_STENCIL_INDEX = 0x1901U;
        public const uint GL_DEPTH_COMPONENT = 0x1902U;
        public const uint GL_RED = 0x1903U;
        public const uint GL_GREEN = 0x1904U;
        public const uint GL_BLUE = 0x1905U;
        public const uint GL_ALPHA = 0x1906U;
        public const uint GL_RGB = 0x1907U;
        public const uint GL_RGBA = 0x1908U;
        public const uint GL_LUMINANCE = 0x1909U;
        public const uint GL_LUMINANCE_ALPHA = 0x190AU;

        // PixelType
        public const uint GL_BITMAP = 0x1A00U;

        // PolygonMode
        public const uint GL_POINT = 0x1B00U;
        public const uint GL_LINE = 0x1B01U;
        public const uint GL_FILL = 0x1B02U;

        // RenderingMode
        public const uint GL_RENDER = 0x1C00U;
        public const uint GL_FEEDBACK = 0x1C01U;
        public const uint GL_SELECT = 0x1C02U;

        // ShadingModel
        public const uint GL_FLAT = 0x1D00U;
        public const uint GL_SMOOTH = 0x1D01U;

        // StencilOp
        public const uint GL_KEEP = 0x1E00U;
        public const uint GL_REPLACE = 0x1E01U;
        public const uint GL_INCR = 0x1E02U;
        public const uint GL_DECR = 0x1E03U;

        // StringName
        public const uint GL_VENDOR = 0x1F00U;
        public const uint GL_RENDERER = 0x1F01U;
        public const uint GL_VERSION = 0x1F02U;
        public const uint GL_EXTENSIONS = 0x1F03U;

        // TextureCoordName
        public const uint GL_S = 0x2000U;
        public const uint GL_T = 0x2001U;
        public const uint GL_R = 0x2002U;
        public const uint GL_Q = 0x2003U;

        // TextureEnvMode
        public const uint GL_MODULATE = 0x2100U;
        public const uint GL_DECAL = 0x2101U;

        // TextureEnvParameter
        public const uint GL_TEXTURE_ENV_MODE = 0x2200U;
        public const uint GL_TEXTURE_ENV_COLOR = 0x2201U;

        // TextureEnvTarget
        public const uint GL_TEXTURE_ENV = 0x2300U;

        // TextureGenMode
        public const uint GL_EYE_LINEAR = 0x2400U;
        public const uint GL_OBJECT_LINEAR = 0x2401U;
        public const uint GL_SPHERE_MAP = 0x2402U;

        // TextureGenParameter
        public const uint GL_TEXTURE_GEN_MODE = 0x2500U;
        public const uint GL_OBJECT_PLANE = 0x2501U;
        public const uint GL_EYE_PLANE = 0x2502U;

        // TextureMagFilter
        public const uint GL_NEAREST = 0x2600U;
        public const uint GL_LINEAR = 0x2601U;

        // TextureMinFilter
        public const uint GL_NEAREST_MIPMAP_NEAREST = 0x2700U;
        public const uint GL_LINEAR_MIPMAP_NEAREST = 0x2701U;
        public const uint GL_NEAREST_MIPMAP_LINEAR = 0x2702U;
        public const uint GL_LINEAR_MIPMAP_LINEAR = 0x2703U;

        // TextureParameterName
        public const uint GL_TEXTURE_MAG_FILTER = 0x2800U;
        public const uint GL_TEXTURE_MIN_FILTER = 0x2801U;
        public const uint GL_TEXTURE_WRAP_S = 0x2802U;
        public const uint GL_TEXTURE_WRAP_T = 0x2803U;

        // TextureWrapMode
        public const uint GL_CLAMP = 0x2900U;
        public const uint GL_REPEAT = 0x2901U;

        // ClientAttribMask
        public const uint GL_CLIENT_PIXEL_STORE_BIT = 0x1U;
        public const uint GL_CLIENT_VERTEX_ARRAY_BIT = 0x2U;
        public const uint GL_CLIENT_ALL_ATTRIB_BITS = 0xFFFFFFFF;

        // Polygon Offset
        public const uint GL_POLYGON_OFFSET_FACTOR = 0x8038U;
        public const uint GL_POLYGON_OFFSET_UNITS = 0x2A00U;
        public const uint GL_POLYGON_OFFSET_POINT = 0x2A01U;
        public const uint GL_POLYGON_OFFSET_LINE = 0x2A02U;
        public const uint GL_POLYGON_OFFSET_FILL = 0x8037U;

        // Texture
        public const uint GL_ALPHA4 = 0x803BU;
        public const uint GL_ALPHA8 = 0x803CU;
        public const uint GL_ALPHA12 = 0x803DU;
        public const uint GL_ALPHA16 = 0x803EU;
        public const uint GL_LUMINANCE4 = 0x803FU;
        public const uint GL_LUMINANCE8 = 0x8040U;
        public const uint GL_LUMINANCE12 = 0x8041U;
        public const uint GL_LUMINANCE16 = 0x8042U;
        public const uint GL_LUMINANCE4_ALPHA4 = 0x8043U;
        public const uint GL_LUMINANCE6_ALPHA2 = 0x8044U;
        public const uint GL_LUMINANCE8_ALPHA8 = 0x8045U;
        public const uint GL_LUMINANCE12_ALPHA4 = 0x8046U;
        public const uint GL_LUMINANCE12_ALPHA12 = 0x8047U;
        public const uint GL_LUMINANCE16_ALPHA16 = 0x8048U;
        public const uint GL_INTENSITY = 0x8049U;
        public const uint GL_INTENSITY4 = 0x804AU;
        public const uint GL_INTENSITY8 = 0x804BU;
        public const uint GL_INTENSITY12 = 0x804CU;
        public const uint GL_INTENSITY16 = 0x804DU;
        public const uint GL_R3_G3_B2 = 0x2A10U;
        public const uint GL_RGB4 = 0x804FU;
        public const uint GL_RGB5 = 0x8050U;
        public const uint GL_RGB8 = 0x8051U;
        public const uint GL_RGB10 = 0x8052U;
        public const uint GL_RGB12 = 0x8053U;
        public const uint GL_RGB16 = 0x8054U;
        public const uint GL_RGBA2 = 0x8055U;
        public const uint GL_RGBA4 = 0x8056U;
        public const uint GL_RGB5_A1 = 0x8057U;
        public const uint GL_RGBA8 = 0x8058U;
        public const uint GL_RGB10_A2 = 0x8059U;
        public const uint GL_RGBA12 = 0x805AU;
        public const uint GL_RGBA16 = 0x805BU;
        public const uint GL_TEXTURE_RED_SIZE = 0x805CU;
        public const uint GL_TEXTURE_GREEN_SIZE = 0x805DU;
        public const uint GL_TEXTURE_BLUE_SIZE = 0x805EU;
        public const uint GL_TEXTURE_ALPHA_SIZE = 0x805FU;
        public const uint GL_TEXTURE_LUMINANCE_SIZE = 0x8060U;
        public const uint GL_TEXTURE_INTENSITY_SIZE = 0x8061U;
        public const uint GL_PROXY_TEXTURE_1D = 0x8063U;
        public const uint GL_PROXY_TEXTURE_2D = 0x8064U;

        // Texture object
        public const uint GL_TEXTURE_PRIORITY = 0x8066U;
        public const uint GL_TEXTURE_RESIDENT = 0x8067U;
        public const uint GL_TEXTURE_BINDING_1D = 0x8068U;
        public const uint GL_TEXTURE_BINDING_2D = 0x8069U;

        // Vertex array
        public const uint GL_VERTEX_ARRAY = 0x8074U;
        public const uint GL_NORMAL_ARRAY = 0x8075U;
        public const uint GL_COLOR_ARRAY = 0x8076U;
        public const uint GL_INDEX_ARRAY = 0x8077U;
        public const uint GL_TEXTURE_COORD_ARRAY = 0x8078U;
        public const uint GL_EDGE_FLAG_ARRAY = 0x8079U;
        public const uint GL_VERTEX_ARRAY_SIZE = 0x807AU;
        public const uint GL_VERTEX_ARRAY_TYPE = 0x807BU;
        public const uint GL_VERTEX_ARRAY_STRIDE = 0x807CU;
        public const uint GL_NORMAL_ARRAY_TYPE = 0x807EU;
        public const uint GL_NORMAL_ARRAY_STRIDE = 0x807FU;
        public const uint GL_COLOR_ARRAY_SIZE = 0x8081U;
        public const uint GL_COLOR_ARRAY_TYPE = 0x8082U;
        public const uint GL_COLOR_ARRAY_STRIDE = 0x8083U;
        public const uint GL_INDEX_ARRAY_TYPE = 0x8085U;
        public const uint GL_INDEX_ARRAY_STRIDE = 0x8086U;
        public const uint GL_TEXTURE_COORD_ARRAY_SIZE = 0x8088U;
        public const uint GL_TEXTURE_COORD_ARRAY_TYPE = 0x8089U;
        public const uint GL_TEXTURE_COORD_ARRAY_STRIDE = 0x808AU;
        public const uint GL_EDGE_FLAG_ARRAY_STRIDE = 0x808CU;
        public const uint GL_VERTEX_ARRAY_POINTER = 0x808EU;
        public const uint GL_NORMAL_ARRAY_POINTER = 0x808FU;
        public const uint GL_COLOR_ARRAY_POINTER = 0x8090U;
        public const uint GL_INDEX_ARRAY_POINTER = 0x8091U;
        public const uint GL_TEXTURE_COORD_ARRAY_POINTER = 0x8092U;
        public const uint GL_EDGE_FLAG_ARRAY_POINTER = 0x8093U;
        public const uint GL_V2F = 0x2A20U;
        public const uint GL_V3F = 0x2A21U;
        public const uint GL_C4UB_V2F = 0x2A22U;
        public const uint GL_C4UB_V3F = 0x2A23U;
        public const uint GL_C3F_V3F = 0x2A24U;
        public const uint GL_N3F_V3F = 0x2A25U;
        public const uint GL_C4F_N3F_V3F = 0x2A26U;
        public const uint GL_T2F_V3F = 0x2A27U;
        public const uint GL_T4F_V4F = 0x2A28U;
        public const uint GL_T2F_C4UB_V3F = 0x2A29U;
        public const uint GL_T2F_C3F_V3F = 0x2A2AU;
        public const uint GL_T2F_N3F_V3F = 0x2A2BU;
        public const uint GL_T2F_C4F_N3F_V3F = 0x2A2CU;
        public const uint GL_T4F_C4F_N3F_V4F = 0x2A2DU;

        // Extensions
        public const uint GL_EXT_vertex_array = 1U;
        public const uint GL_EXT_bgra = 1U;
        public const uint GL_EXT_paletted_texture = 1U;
        public const uint GL_WIN_swap_hint = 1U;
        public const uint GL_WIN_draw_range_elements = 1U;

        // EXT_vertex_array 
        public const uint GL_VERTEX_ARRAY_EXT = 0x8074U;
        public const uint GL_NORMAL_ARRAY_EXT = 0x8075U;
        public const uint GL_COLOR_ARRAY_EXT = 0x8076U;
        public const uint GL_INDEX_ARRAY_EXT = 0x8077U;
        public const uint GL_TEXTURE_COORD_ARRAY_EXT = 0x8078U;
        public const uint GL_EDGE_FLAG_ARRAY_EXT = 0x8079U;
        public const uint GL_VERTEX_ARRAY_SIZE_EXT = 0x807AU;
        public const uint GL_VERTEX_ARRAY_TYPE_EXT = 0x807BU;
        public const uint GL_VERTEX_ARRAY_STRIDE_EXT = 0x807CU;
        public const uint GL_VERTEX_ARRAY_COUNT_EXT = 0x807DU;
        public const uint GL_NORMAL_ARRAY_TYPE_EXT = 0x807EU;
        public const uint GL_NORMAL_ARRAY_STRIDE_EXT = 0x807FU;
        public const uint GL_NORMAL_ARRAY_COUNT_EXT = 0x8080U;
        public const uint GL_COLOR_ARRAY_SIZE_EXT = 0x8081U;
        public const uint GL_COLOR_ARRAY_TYPE_EXT = 0x8082U;
        public const uint GL_COLOR_ARRAY_STRIDE_EXT = 0x8083U;
        public const uint GL_COLOR_ARRAY_COUNT_EXT = 0x8084U;
        public const uint GL_INDEX_ARRAY_TYPE_EXT = 0x8085U;
        public const uint GL_INDEX_ARRAY_STRIDE_EXT = 0x8086U;
        public const uint GL_INDEX_ARRAY_COUNT_EXT = 0x8087U;
        public const uint GL_TEXTURE_COORD_ARRAY_SIZE_EXT = 0x8088U;
        public const uint GL_TEXTURE_COORD_ARRAY_TYPE_EXT = 0x8089U;
        public const uint GL_TEXTURE_COORD_ARRAY_STRIDE_EXT = 0x808AU;
        public const uint GL_TEXTURE_COORD_ARRAY_COUNT_EXT = 0x808BU;
        public const uint GL_EDGE_FLAG_ARRAY_STRIDE_EXT = 0x808CU;
        public const uint GL_EDGE_FLAG_ARRAY_COUNT_EXT = 0x808DU;
        public const uint GL_VERTEX_ARRAY_POINTER_EXT = 0x808EU;
        public const uint GL_NORMAL_ARRAY_POINTER_EXT = 0x808FU;
        public const uint GL_COLOR_ARRAY_POINTER_EXT = 0x8090U;
        public const uint GL_INDEX_ARRAY_POINTER_EXT = 0x8091U;
        public const uint GL_TEXTURE_COORD_ARRAY_POINTER_EXT = 0x8092U;
        public const uint GL_EDGE_FLAG_ARRAY_POINTER_EXT = 0x8093U;
        public const uint GL_DOUBLE_EXT = 1U;

        // EXT_paletted_texture
        public const uint GL_COLOR_TABLE_FORMAT_EXT = 0x80D8U;
        public const uint GL_COLOR_TABLE_WIDTH_EXT = 0x80D9U;
        public const uint GL_COLOR_TABLE_RED_SIZE_EXT = 0x80DAU;
        public const uint GL_COLOR_TABLE_GREEN_SIZE_EXT = 0x80DBU;
        public const uint GL_COLOR_TABLE_BLUE_SIZE_EXT = 0x80DCU;
        public const uint GL_COLOR_TABLE_ALPHA_SIZE_EXT = 0x80DDU;
        public const uint GL_COLOR_TABLE_LUMINANCE_SIZE_EXT = 0x80DEU;
        public const uint GL_COLOR_TABLE_INTENSITY_SIZE_EXT = 0x80DFU;
        public const uint GL_COLOR_INDEX1_EXT = 0x80E2U;
        public const uint GL_COLOR_INDEX2_EXT = 0x80E3U;
        public const uint GL_COLOR_INDEX4_EXT = 0x80E4U;
        public const uint GL_COLOR_INDEX8_EXT = 0x80E5U;
        public const uint GL_COLOR_INDEX12_EXT = 0x80E6U;
        public const uint GL_COLOR_INDEX16_EXT = 0x80E7U;

        // WIN_draw_range_elements
        public const uint GL_MAX_ELEMENTS_VERTICES_WIN = 0x80E8U;
        public const uint GL_MAX_ELEMENTS_INDICES_WIN = 0x80E9U;

        // WIN_phong_shading
        public const uint GL_PHONG_WIN = 0x80EAU;
        public const uint GL_PHONG_HINT_WIN = 0x80EBU;

        // WIN_specular_fog 
        public const uint FOG_SPECULAR_TEXTURE_WIN = 0x80ECU;

        // ''''''''''''''''''''''''''' GLU ''''''''''''''''''''''''''''

        // Version
        public const uint GLU_VERSION_1_1 = 1U;
        public const uint GLU_VERSION_1_2 = 1U;

        // Errors
        public const uint GLU_INVALID_ENUM = 100900U;
        public const uint GLU_INVALID_VALUE = 100901U;
        public const uint GLU_OUT_OF_MEMORY = 100902U;
        public const uint GLU_INCOMPATIBLE_GL_VERSION = 100903U;
        public const uint GLU_NO_ERROR = 0U;

        // StringName
        public const uint GLU_VERSION = 100800U;
        public const uint GLU_EXTENSIONS = 100801U;

        // Boolean
        public const uint GLU_TRUE = 1U;
        public const uint GLU_FALSE = 0U;

        // QuadricNormal
        public const uint GLU_SMOOTH = 100000U;
        public const uint GLU_FLAT = 100001U;
        public const uint GLU_NONE = 100002U;

        // QuadricDrawStyle
        public const uint GLU_POINT = 100010U;
        public const uint GLU_LINE = 100011U;
        public const uint GLU_FILL = 100012U;
        public const uint GLU_SILHOUETTE = 100013U;

        // QuadricOrientation
        public const uint GLU_OUTSIDE = 100020U;
        public const uint GLU_INSIDE = 100021U;

        // Tesselation constants
        public const double GLU_TESS_MAX_COORD = 1.0E+150d;
        public const uint GLU_TESS_WINDING_RULE = 100140U;
        public const uint GLU_TESS_BOUNDARY_ONLY = 100141U;

        // TessProperty
        public const uint GLU_TESS_TOLERANCE = 100142U;
        public const uint GLU_TESS_WINDING_ODD = 100130U;
        public const uint GLU_TESS_WINDING_NONZERO = 100131U;

        // TessWinding
        public const uint GLU_TESS_WINDING_POSITIVE = 100132U;
        public const uint GLU_TESS_WINDING_NEGATIVE = 100133U;
        public const uint GLU_TESS_WINDING_ABS_GEQ_TWO = 100134U;
        public const uint GLU_TESS_BEGIN = 100100U;
        public const uint GLU_TESS_VERTEX = 100101U;

        // TessCallback
        public const uint GLU_TESS_END = 100102U;
        public const uint GLU_TESS_ERROR = 100103U;
        public const uint GLU_TESS_EDGE_FLAG = 100104U;
        public const uint GLU_TESS_COMBINE = 100105U;
        public const uint GLU_TESS_BEGIN_DATA = 100106U;
        public const uint GLU_TESS_VERTEX_DATA = 100107U;
        public const uint GLU_TESS_END_DATA = 100108U;
        public const uint GLU_TESS_ERROR_DATA = 100109U;
        public const uint GLU_TESS_EDGE_FLAG_DATA = 100110U;
        public const uint GLU_TESS_COMBINE_DATA = 100111U;

        // TessError
        public const uint GLU_TESS_ERROR1 = 100151U;
        public const uint GLU_TESS_ERROR2 = 100152U;
        public const uint GLU_TESS_ERROR3 = 100153U;
        public const uint GLU_TESS_ERROR4 = 100154U;
        public const uint GLU_TESS_ERROR5 = 100155U;
        public const uint GLU_TESS_ERROR6 = 100156U;
        public const uint GLU_TESS_ERROR7 = 100157U;
        public const uint GLU_TESS_ERROR8 = 100158U;
        public const uint GLU_TESS_MISSING_BEGIN_POLYGON = 100151U;
        public const uint GLU_TESS_MISSING_BEGIN_CONTOUR = 100152U;
        public const uint GLU_TESS_MISSING_END_POLYGON = 100153U;
        public const uint GLU_TESS_MISSING_END_CONTOUR = 100154U;
        public const uint GLU_TESS_COORD_TOO_LARGE = 100155U;
        public const uint GLU_TESS_NEED_COMBINE_CALLBACK = 100156U;

        // NurbsProperty
        public const uint GLU_AUTO_LOAD_MATRIX = 100200U;
        public const uint GLU_CULLING = 100201U;
        public const uint GLU_SAMPLING_TOLERANCE = 100203U;
        public const uint GLU_DISPLAY_MODE = 100204U;
        public const uint GLU_PARAMETRIC_TOLERANCE = 100202U;
        public const uint GLU_SAMPLING_METHOD = 100205U;
        public const uint GLU_U_STEP = 100206U;
        public const uint GLU_V_STEP = 100207U;

        // NurbsSampling
        public const uint GLU_PATH_LENGTH = 100215U;
        public const uint GLU_PARAMETRIC_ERROR = 100216U;
        public const uint GLU_DOMAIN_DISTANCE = 100217U;

        // NurbsTrim
        public const uint GLU_MAP1_TRIM_2 = 100210U;
        public const uint GLU_MAP1_TRIM_3 = 100211U;

        // NurbsDisplay
        public const uint GLU_OUTLINE_POLYGON = 100240U;
        public const uint GLU_OUTLINE_PATCH = 100241U;

        // NurbsErrors
        public const uint GLU_NURBS_ERROR1 = 100251U;
        public const uint GLU_NURBS_ERROR2 = 100252U;
        public const uint GLU_NURBS_ERROR3 = 100253U;
        public const uint GLU_NURBS_ERROR4 = 100254U;
        public const uint GLU_NURBS_ERROR5 = 100255U;
        public const uint GLU_NURBS_ERROR6 = 100256U;
        public const uint GLU_NURBS_ERROR7 = 100257U;
        public const uint GLU_NURBS_ERROR8 = 100258U;
        public const uint GLU_NURBS_ERROR9 = 100259U;
        public const uint GLU_NURBS_ERROR10 = 100260U;
        public const uint GLU_NURBS_ERROR11 = 100261U;
        public const uint GLU_NURBS_ERROR12 = 100262U;
        public const uint GLU_NURBS_ERROR13 = 100263U;
        public const uint GLU_NURBS_ERROR14 = 100264U;
        public const uint GLU_NURBS_ERROR15 = 100265U;
        public const uint GLU_NURBS_ERROR16 = 100266U;
        public const uint GLU_NURBS_ERROR17 = 100267U;
        public const uint GLU_NURBS_ERROR18 = 100268U;
        public const uint GLU_NURBS_ERROR19 = 100269U;
        public const uint GLU_NURBS_ERROR20 = 100270U;
        public const uint GLU_NURBS_ERROR21 = 100271U;
        public const uint GLU_NURBS_ERROR22 = 100272U;
        public const uint GLU_NURBS_ERROR23 = 100273U;
        public const uint GLU_NURBS_ERROR24 = 100274U;
        public const uint GLU_NURBS_ERROR25 = 100275U;
        public const uint GLU_NURBS_ERROR26 = 100276U;
        public const uint GLU_NURBS_ERROR27 = 100277U;
        public const uint GLU_NURBS_ERROR28 = 100278U;
        public const uint GLU_NURBS_ERROR29 = 100279U;
        public const uint GLU_NURBS_ERROR30 = 100280U;
        public const uint GLU_NURBS_ERROR31 = 100281U;
        public const uint GLU_NURBS_ERROR32 = 100282U;
        public const uint GLU_NURBS_ERROR33 = 100283U;
        public const uint GLU_NURBS_ERROR34 = 100284U;
        public const uint GLU_NURBS_ERROR35 = 100285U;
        public const uint GLU_NURBS_ERROR36 = 100286U;
        public const uint GLU_NURBS_ERROR37 = 100287U;


        // OpenGL 1.2

        public const uint GL_UNSIGNED_BYTE_3_3_2 = 0x8032U;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033U;
        public const uint GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034U;
        public const uint GL_UNSIGNED_INT_8_8_8_8 = 0x8035U;
        public const uint GL_UNSIGNED_INT_10_10_10_2 = 0x8036U;
        public const uint GL_TEXTURE_BINDING_3D = 0x806AU;
        public const uint GL_PACK_SKIP_IMAGES = 0x806BU;
        public const uint GL_PACK_IMAGE_HEIGHT = 0x806CU;
        public const uint GL_UNPACK_SKIP_IMAGES = 0x806DU;
        public const uint GL_UNPACK_IMAGE_HEIGHT = 0x806EU;
        public const uint GL_TEXTURE_3D = 0x806FU;
        public const uint GL_PROXY_TEXTURE_3D = 0x8070U;
        public const uint GL_TEXTURE_DEPTH = 0x8071U;
        public const uint GL_TEXTURE_WRAP_R = 0x8072U;
        public const uint GL_MAX_3D_TEXTURE_SIZE = 0x8073U;
        public const uint GL_UNSIGNED_BYTE_2_3_3_REV = 0x8362U;
        public const uint GL_UNSIGNED_SHORT_5_6_5 = 0x8363U;
        public const uint GL_UNSIGNED_SHORT_5_6_5_REV = 0x8364U;
        public const uint GL_UNSIGNED_SHORT_4_4_4_4_REV = 0x8365U;
        public const uint GL_UNSIGNED_SHORT_1_5_5_5_REV = 0x8366U;
        public const uint GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367U;
        public const uint GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368U;
        public const uint GL_BGR = 0x80E0U;
        public const uint GL_BGRA = 0x80E1U;
        public const uint GL_MAX_ELEMENTS_VERTICES = 0x80E8U;
        public const uint GL_MAX_ELEMENTS_INDICES = 0x80E9U;
        public const uint GL_CLAMP_TO_EDGE = 0x812FU;
        public const uint GL_TEXTURE_MIN_LOD = 0x813AU;
        public const uint GL_TEXTURE_MAX_LOD = 0x813BU;
        public const uint GL_TEXTURE_BASE_LEVEL = 0x813CU;
        public const uint GL_TEXTURE_MAX_LEVEL = 0x813DU;
        public const uint GL_SMOOTH_POINT_SIZE_RANGE = 0xB12U;
        public const uint GL_SMOOTH_POINT_SIZE_GRANULARITY = 0xB13U;
        public const uint GL_SMOOTH_LINE_WIDTH_RANGE = 0xB22U;
        public const uint GL_SMOOTH_LINE_WIDTH_GRANULARITY = 0xB23U;
        public const uint GL_ALIASED_LINE_WIDTH_RANGE = 0x846EU;

        // OpenGL 1.3

        public const uint GL_TEXTURE0 = 0x84C0U;
        public const uint GL_TEXTURE1 = 0x84C1U;
        public const uint GL_TEXTURE2 = 0x84C2U;
        public const uint GL_TEXTURE3 = 0x84C3U;
        public const uint GL_TEXTURE4 = 0x84C4U;
        public const uint GL_TEXTURE5 = 0x84C5U;
        public const uint GL_TEXTURE6 = 0x84C6U;
        public const uint GL_TEXTURE7 = 0x84C7U;
        public const uint GL_TEXTURE8 = 0x84C8U;
        public const uint GL_TEXTURE9 = 0x84C9U;
        public const uint GL_TEXTURE10 = 0x84CAU;
        public const uint GL_TEXTURE11 = 0x84CBU;
        public const uint GL_TEXTURE12 = 0x84CCU;
        public const uint GL_TEXTURE13 = 0x84CDU;
        public const uint GL_TEXTURE14 = 0x84CEU;
        public const uint GL_TEXTURE15 = 0x84CFU;
        public const uint GL_TEXTURE16 = 0x84D0U;
        public const uint GL_TEXTURE17 = 0x84D1U;
        public const uint GL_TEXTURE18 = 0x84D2U;
        public const uint GL_TEXTURE19 = 0x84D3U;
        public const uint GL_TEXTURE20 = 0x84D4U;
        public const uint GL_TEXTURE21 = 0x84D5U;
        public const uint GL_TEXTURE22 = 0x84D6U;
        public const uint GL_TEXTURE23 = 0x84D7U;
        public const uint GL_TEXTURE24 = 0x84D8U;
        public const uint GL_TEXTURE25 = 0x84D9U;
        public const uint GL_TEXTURE26 = 0x84DAU;
        public const uint GL_TEXTURE27 = 0x84DBU;
        public const uint GL_TEXTURE28 = 0x84DCU;
        public const uint GL_TEXTURE29 = 0x84DDU;
        public const uint GL_TEXTURE30 = 0x84DEU;
        public const uint GL_TEXTURE31 = 0x84DFU;
        public const uint GL_ACTIVE_TEXTURE = 0x84E0U;
        public const uint GL_MULTISAMPLE = 0x809DU;
        public const uint GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809EU;
        public const uint GL_SAMPLE_ALPHA_TO_ONE = 0x809FU;
        public const uint GL_SAMPLE_COVERAGE = 0x80A0U;
        public const uint GL_SAMPLE_BUFFERS = 0x80A8U;
        public const uint GL_SAMPLES = 0x80A9U;
        public const uint GL_SAMPLE_COVERAGE_VALUE = 0x80AAU;
        public const uint GL_SAMPLE_COVERAGE_INVERT = 0x80ABU;
        public const uint GL_TEXTURE_CUBE_MAP = 0x8513U;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP = 0x8514U;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515U;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516U;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517U;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518U;
        public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519U;
        public const uint GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851AU;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP = 0x851BU;
        public const uint GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851CU;
        public const uint GL_COMPRESSED_RGB = 0x84EDU;
        public const uint GL_COMPRESSED_RGBA = 0x84EEU;
        public const uint GL_TEXTURE_COMPRESSION_HINT = 0x84EFU;
        public const uint GL_TEXTURE_COMPRESSED_IMAGE_SIZE = 0x86A0U;
        public const uint GL_TEXTURE_COMPRESSED = 0x86A1U;
        public const uint GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2U;
        public const uint GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3U;
        public const uint GL_CLAMP_TO_BORDER = 0x812DU;

        // OpenGL 1.4

        public const uint GL_BLEND_DST_RGB = 0x80C8U;
        public const uint GL_BLEND_SRC_RGB = 0x80C9U;
        public const uint GL_BLEND_DST_ALPHA = 0x80CAU;
        public const uint GL_BLEND_SRC_ALPHA = 0x80CBU;
        public const uint GL_POINT_FADE_THRESHOLD_SIZE = 0x8128U;
        public const uint GL_DEPTH_COMPONENT16 = 0x81A5U;
        public const uint GL_DEPTH_COMPONENT24 = 0x81A6U;
        public const uint GL_DEPTH_COMPONENT32 = 0x81A7U;
        public const uint GL_MIRRORED_REPEAT = 0x8370U;
        public const uint GL_MAX_TEXTURE_LOD_BIAS = 0x84FDU;
        public const uint GL_TEXTURE_LOD_BIAS = 0x8501U;
        public const uint GL_INCR_WRAP = 0x8507U;
        public const uint GL_DECR_WRAP = 0x8508U;
        public const uint GL_TEXTURE_DEPTH_SIZE = 0x884AU;
        public const uint GL_TEXTURE_COMPARE_MODE = 0x884CU;
        public const uint GL_TEXTURE_COMPARE_FUNC = 0x884DU;

        // OpenGL 1.5

        public const uint GL_BUFFER_SIZE = 0x8764U;
        public const uint GL_BUFFER_USAGE = 0x8765U;
        public const uint GL_QUERY_COUNTER_BITS = 0x8864U;
        public const uint GL_CURRENT_QUERY = 0x8865U;
        public const uint GL_QUERY_RESULT = 0x8866U;
        public const uint GL_QUERY_RESULT_AVAILABLE = 0x8867U;
        public const uint GL_ARRAY_BUFFER = 0x8892U;
        public const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893U;
        public const uint GL_ARRAY_BUFFER_BINDING = 0x8894U;
        public const uint GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889FU;
        public const uint GL_READ_ONLY = 0x88B8U;
        public const uint GL_WRITE_ONLY = 0x88B9U;
        public const uint GL_READ_WRITE = 0x88BAU;
        public const uint GL_BUFFER_ACCESS = 0x88BBU;
        public const uint GL_BUFFER_MAPPED = 0x88BCU;
        public const uint GL_BUFFER_MAP_POINTER = 0x88BDU;
        public const uint GL_STREAM_DRAW = 0x88E0U;
        public const uint GL_STREAM_READ = 0x88E1U;
        public const uint GL_STREAM_COPY = 0x88E2U;
        public const uint GL_STATIC_DRAW = 0x88E4U;
        public const uint GL_STATIC_READ = 0x88E5U;
        public const uint GL_STATIC_COPY = 0x88E6U;
        public const uint GL_DYNAMIC_DRAW = 0x88E8U;
        public const uint GL_DYNAMIC_READ = 0x88E9U;
        public const uint GL_DYNAMIC_COPY = 0x88EAU;
        public const uint GL_SAMPLES_PASSED = 0x8914U;

        // OpenGL 2.0

        public const uint GL_BLEND_EQUATION_RGB = 0x8009U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_SIZE = 0x8623U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_TYPE = 0x8625U;
        public const uint GL_CURRENT_VERTEX_ATTRIB = 0x8626U;
        public const uint GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_POINTER = 0x8645U;
        public const uint GL_STENCIL_BACK_FUNC = 0x8800U;
        public const uint GL_STENCIL_BACK_FAIL = 0x8801U;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802U;
        public const uint GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803U;
        public const uint GL_MAX_DRAW_BUFFERS = 0x8824U;
        public const uint GL_DRAW_BUFFER0 = 0x8825U;
        public const uint GL_DRAW_BUFFER1 = 0x8826U;
        public const uint GL_DRAW_BUFFER2 = 0x8827U;
        public const uint GL_DRAW_BUFFER3 = 0x8828U;
        public const uint GL_DRAW_BUFFER4 = 0x8829U;
        public const uint GL_DRAW_BUFFER5 = 0x882AU;
        public const uint GL_DRAW_BUFFER6 = 0x882BU;
        public const uint GL_DRAW_BUFFER7 = 0x882CU;
        public const uint GL_DRAW_BUFFER8 = 0x882DU;
        public const uint GL_DRAW_BUFFER9 = 0x882EU;
        public const uint GL_DRAW_BUFFER10 = 0x882FU;
        public const uint GL_DRAW_BUFFER11 = 0x8830U;
        public const uint GL_DRAW_BUFFER12 = 0x8831U;
        public const uint GL_DRAW_BUFFER13 = 0x8832U;
        public const uint GL_DRAW_BUFFER14 = 0x8833U;
        public const uint GL_DRAW_BUFFER15 = 0x8834U;
        public const uint GL_BLEND_EQUATION_ALPHA = 0x883DU;
        public const uint GL_MAX_VERTEX_ATTRIBS = 0x8869U;
        public const uint GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886AU;
        public const uint GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872U;
        public const uint GL_FRAGMENT_SHADER = 0x8B30U;
        public const uint GL_VERTEX_SHADER = 0x8B31U;
        public const uint GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49U;
        public const uint GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4AU;
        public const uint GL_MAX_VARYING_FLOATS = 0x8B4BU;
        public const uint GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4CU;
        public const uint GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4DU;
        public const uint GL_SHADER_TYPE = 0x8B4FU;
        public const uint GL_FLOAT_VEC2 = 0x8B50U;
        public const uint GL_FLOAT_VEC3 = 0x8B51U;
        public const uint GL_FLOAT_VEC4 = 0x8B52U;
        public const uint GL_INT_VEC2 = 0x8B53U;
        public const uint GL_INT_VEC3 = 0x8B54U;
        public const uint GL_INT_VEC4 = 0x8B55U;
        public const uint GL_BOOL = 0x8B56U;
        public const uint GL_BOOL_VEC2 = 0x8B57U;
        public const uint GL_BOOL_VEC3 = 0x8B58U;
        public const uint GL_BOOL_VEC4 = 0x8B59U;
        public const uint GL_FLOAT_MAT2 = 0x8B5AU;
        public const uint GL_FLOAT_MAT3 = 0x8B5BU;
        public const uint GL_FLOAT_MAT4 = 0x8B5CU;
        public const uint GL_SAMPLER_1D = 0x8B5DU;
        public const uint GL_SAMPLER_2D = 0x8B5EU;
        public const uint GL_SAMPLER_3D = 0x8B5FU;
        public const uint GL_SAMPLER_CUBE = 0x8B60U;
        public const uint GL_SAMPLER_1D_SHADOW = 0x8B61U;
        public const uint GL_SAMPLER_2D_SHADOW = 0x8B62U;
        public const uint GL_DELETE_STATUS = 0x8B80U;
        public const uint GL_COMPILE_STATUS = 0x8B81U;
        public const uint GL_LINK_STATUS = 0x8B82U;
        public const uint GL_VALIDATE_STATUS = 0x8B83U;
        public const uint GL_INFO_LOG_LENGTH = 0x8B84U;
        public const uint GL_ATTACHED_SHADERS = 0x8B85U;
        public const uint GL_ACTIVE_UNIFORMS = 0x8B86U;
        public const uint GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87U;
        public const uint GL_SHADER_SOURCE_LENGTH = 0x8B88U;
        public const uint GL_ACTIVE_ATTRIBUTES = 0x8B89U;
        public const uint GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8AU;
        public const uint GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8BU;
        public const uint GL_SHADING_LANGUAGE_VERSION = 0x8B8CU;
        public const uint GL_CURRENT_PROGRAM = 0x8B8DU;
        public const uint GL_POINT_SPRITE_COORD_ORIGIN = 0x8CA0U;
        public const uint GL_LOWER_LEFT = 0x8CA1U;
        public const uint GL_UPPER_LEFT = 0x8CA2U;
        public const uint GL_STENCIL_BACK_REF = 0x8CA3U;
        public const uint GL_STENCIL_BACK_VALUE_MASK = 0x8CA4U;
        public const uint GL_STENCIL_BACK_WRITEMASK = 0x8CA5U;

        // OpenGl 2.1

        public const uint GL_PIXEL_PACK_BUFFER = 0x88EBU;
        public const uint GL_PIXEL_UNPACK_BUFFER = 0x88ECU;
        public const uint GL_PIXEL_PACK_BUFFER_BINDING = 0x88EDU;
        public const uint GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EFU;
        public const uint GL_FLOAT_MAT2x3 = 0x8B65U;
        public const uint GL_FLOAT_MAT2x4 = 0x8B66U;
        public const uint GL_FLOAT_MAT3x2 = 0x8B67U;
        public const uint GL_FLOAT_MAT3x4 = 0x8B68U;
        public const uint GL_FLOAT_MAT4x2 = 0x8B69U;
        public const uint GL_FLOAT_MAT4x3 = 0x8B6AU;
        public const uint GL_SRGB = 0x8C40U;
        public const uint GL_SRGB8 = 0x8C41U;
        public const uint GL_SRGB_ALPHA = 0x8C42U;
        public const uint GL_SRGB8_ALPHA8 = 0x8C43U;
        public const uint GL_COMPRESSED_SRGB = 0x8C48U;
        public const uint GL_COMPRESSED_SRGB_ALPHA = 0x8C49U;

        // OpenGL 3.0

        public const uint GL_COMPARE_REF_TO_TEXTURE = 0x884EU;
        public const uint GL_CLIP_DISTANCE0 = 0x3000U;
        public const uint GL_CLIP_DISTANCE1 = 0x3001U;
        public const uint GL_CLIP_DISTANCE2 = 0x3002U;
        public const uint GL_CLIP_DISTANCE3 = 0x3003U;
        public const uint GL_CLIP_DISTANCE4 = 0x3004U;
        public const uint GL_CLIP_DISTANCE5 = 0x3005U;
        public const uint GL_CLIP_DISTANCE6 = 0x3006U;
        public const uint GL_CLIP_DISTANCE7 = 0x3007U;
        public const uint GL_MAX_CLIP_DISTANCES = 0xD32U;
        public const uint GL_MAJOR_VERSION = 0x821BU;
        public const uint GL_MINOR_VERSION = 0x821CU;
        public const uint GL_NUM_EXTENSIONS = 0x821DU;
        public const uint GL_CONTEXT_FLAGS = 0x821EU;
        public const uint GL_DEPTH_BUFFER = 0x8223U;
        public const uint GL_STENCIL_BUFFER = 0x8224U;
        public const uint GL_COMPRESSED_RED = 0x8225U;
        public const uint GL_COMPRESSED_RG = 0x8226U;
        public const uint GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 0x1U;
        public const uint GL_RGBA32F = 0x8814U;
        public const uint GL_RGB32F = 0x8815U;
        public const uint GL_RGBA16F = 0x881AU;
        public const uint GL_RGB16F = 0x881BU;
        public const uint GL_VERTEX_ATTRIB_ARRAY_INTEGER = 0x88FDU;
        public const uint GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FFU;
        public const uint GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904U;
        public const uint GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905U;
        public const uint GL_CLAMP_READ_COLOR = 0x891CU;
        public const uint GL_FIXED_ONLY = 0x891DU;
        public const uint GL_MAX_VARYING_COMPONENTS = 0x8B4BU;
        public const uint GL_TEXTURE_1D_ARRAY = 0x8C18U;
        public const uint GL_PROXY_TEXTURE_1D_ARRAY = 0x8C19U;
        public const uint GL_TEXTURE_2D_ARRAY = 0x8C1AU;
        public const uint GL_PROXY_TEXTURE_2D_ARRAY = 0x8C1BU;
        public const uint GL_TEXTURE_BINDING_1D_ARRAY = 0x8C1CU;
        public const uint GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1DU;
        public const uint GL_R11F_G11F_B10F = 0x8C3AU;
        public const uint GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3BU;
        public const uint GL_RGB9_E5 = 0x8C3DU;
        public const uint GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3EU;
        public const uint GL_TEXTURE_SHARED_SIZE = 0x8C3FU;
        public const uint GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76U;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7FU;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80U;
        public const uint GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83U;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_START = 0x8C84U;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85U;
        public const uint GL_PRIMITIVES_GENERATED = 0x8C87U;
        public const uint GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88U;
        public const uint GL_RASTERIZER_DISCARD = 0x8C89U;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8AU;
        public const uint GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8BU;
        public const uint GL_INTERLEAVED_ATTRIBS = 0x8C8CU;
        public const uint GL_SEPARATE_ATTRIBS = 0x8C8DU;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8EU;
        public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8FU;
        public const uint GL_READ_FRAMEBUFFER = 0x8CA8U;
        public const uint GL_DRAW_FRAMEBUFFER = 0x8CA9U;
        public const uint GL_RGBA32UI = 0x8D70U;
        public const uint GL_RGB32UI = 0x8D71U;
        public const uint GL_RGBA16UI = 0x8D76U;
        public const uint GL_RGB16UI = 0x8D77U;
        public const uint GL_RGBA8UI = 0x8D7CU;
        public const uint GL_RGB8UI = 0x8D7DU;
        public const uint GL_RGBA32I = 0x8D82U;
        public const uint GL_RGB32I = 0x8D83U;
        public const uint GL_RGBA16I = 0x8D88U;
        public const uint GL_RGB16I = 0x8D89U;
        public const uint GL_RGBA8I = 0x8D8EU;
        public const uint GL_RGB8I = 0x8D8FU;
        public const uint GL_RED_INTEGER = 0x8D94U;
        public const uint GL_GREEN_INTEGER = 0x8D95U;
        public const uint GL_BLUE_INTEGER = 0x8D96U;
        public const uint GL_RGB_INTEGER = 0x8D98U;
        public const uint GL_RGBA_INTEGER = 0x8D99U;
        public const uint GL_BGR_INTEGER = 0x8D9AU;
        public const uint GL_BGRA_INTEGER = 0x8D9BU;
        public const uint GL_SAMPLER_1D_ARRAY = 0x8DC0U;
        public const uint GL_SAMPLER_2D_ARRAY = 0x8DC1U;
        public const uint GL_SAMPLER_1D_ARRAY_SHADOW = 0x8DC3U;
        public const uint GL_SAMPLER_2D_ARRAY_SHADOW = 0x8DC4U;
        public const uint GL_SAMPLER_CUBE_SHADOW = 0x8DC5U;
        public const uint GL_UNSIGNED_INT_VEC2 = 0x8DC6U;
        public const uint GL_UNSIGNED_INT_VEC3 = 0x8DC7U;
        public const uint GL_UNSIGNED_INT_VEC4 = 0x8DC8U;
        public const uint GL_INT_SAMPLER_1D = 0x8DC9U;
        public const uint GL_INT_SAMPLER_2D = 0x8DCAU;
        public const uint GL_INT_SAMPLER_3D = 0x8DCBU;
        public const uint GL_INT_SAMPLER_CUBE = 0x8DCCU;
        public const uint GL_INT_SAMPLER_1D_ARRAY = 0x8DCEU;
        public const uint GL_INT_SAMPLER_2D_ARRAY = 0x8DCFU;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D = 0x8DD1U;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D = 0x8DD2U;
        public const uint GL_UNSIGNED_INT_SAMPLER_3D = 0x8DD3U;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4U;
        public const uint GL_UNSIGNED_INT_SAMPLER_1D_ARRAY = 0x8DD6U;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7U;
        public const uint GL_QUERY_WAIT = 0x8E13U;
        public const uint GL_QUERY_NO_WAIT = 0x8E14U;
        public const uint GL_QUERY_BY_REGION_WAIT = 0x8E15U;
        public const uint GL_QUERY_BY_REGION_NO_WAIT = 0x8E16U;
        public const uint GL_BUFFER_ACCESS_FLAGS = 0x911FU;
        public const uint GL_BUFFER_MAP_LENGTH = 0x9120U;
        public const uint GL_BUFFER_MAP_OFFSET = 0x9121U;
        public const uint GL_R8 = 0x8229U;
        public const uint GL_R16 = 0x822AU;
        public const uint GL_RG8 = 0x822BU;
        public const uint GL_RG16 = 0x822CU;
        public const uint GL_R16F = 0x822DU;
        public const uint GL_R32F = 0x822EU;
        public const uint GL_RG16F = 0x822FU;
        public const uint GL_RG32F = 0x8230U;
        public const uint GL_R8I = 0x8231U;
        public const uint GL_R8UI = 0x8232U;
        public const uint GL_R16I = 0x8233U;
        public const uint GL_R16UI = 0x8234U;
        public const uint GL_R32I = 0x8235U;
        public const uint GL_R32UI = 0x8236U;
        public const uint GL_RG8I = 0x8237U;
        public const uint GL_RG8UI = 0x8238U;
        public const uint GL_RG16I = 0x8239U;
        public const uint GL_RG16UI = 0x823AU;
        public const uint GL_RG32I = 0x823BU;
        public const uint GL_RG32UI = 0x823CU;
        public const uint GL_RG = 0x8227U;
        public const uint GL_RG_INTEGER = 0x8228U;

        // OpenGL 3.1

        public const uint GL_SAMPLER_2D_RECT = 0x8B63U;
        public const uint GL_SAMPLER_2D_RECT_SHADOW = 0x8B64U;
        public const uint GL_SAMPLER_BUFFER = 0x8DC2U;
        public const uint GL_INT_SAMPLER_2D_RECT = 0x8DCDU;
        public const uint GL_INT_SAMPLER_BUFFER = 0x8DD0U;
        public const uint GL_UNSIGNED_INT_SAMPLER_2D_RECT = 0x8DD5U;
        public const uint GL_UNSIGNED_INT_SAMPLER_BUFFER = 0x8DD8U;
        public const uint GL_TEXTURE_BUFFER = 0x8C2AU;
        public const uint GL_MAX_TEXTURE_BUFFER_SIZE = 0x8C2BU;
        public const uint GL_TEXTURE_BINDING_BUFFER = 0x8C2CU;
        public const uint GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2DU;
        public const uint GL_TEXTURE_BUFFER_FORMAT = 0x8C2EU;
        public const uint GL_TEXTURE_RECTANGLE = 0x84F5U;
        public const uint GL_TEXTURE_BINDING_RECTANGLE = 0x84F6U;
        public const uint GL_PROXY_TEXTURE_RECTANGLE = 0x84F7U;
        public const uint GL_MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8U;
        public const uint GL_RED_SNORM = 0x8F90U;
        public const uint GL_RG_SNORM = 0x8F91U;
        public const uint GL_RGB_SNORM = 0x8F92U;
        public const uint GL_RGBA_SNORM = 0x8F93U;
        public const uint GL_R8_SNORM = 0x8F94U;
        public const uint GL_RG8_SNORM = 0x8F95U;
        public const uint GL_RGB8_SNORM = 0x8F96U;
        public const uint GL_RGBA8_SNORM = 0x8F97U;
        public const uint GL_R16_SNORM = 0x8F98U;
        public const uint GL_RG16_SNORM = 0x8F99U;
        public const uint GL_RGB16_SNORM = 0x8F9AU;
        public const uint GL_RGBA16_SNORM = 0x8F9BU;
        public const uint GL_SIGNED_NORMALIZED = 0x8F9CU;
        public const uint GL_PRIMITIVE_RESTART = 0x8F9DU;
        public const uint GL_PRIMITIVE_RESTART_INDEX = 0x8F9EU;

        // OpenGL 3.2

        public const uint GL_CONTEXT_CORE_PROFILE_BIT = 0x1U;
        public const uint GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x2U;
        public const uint GL_LINES_ADJACENCY = 0xAU;
        public const uint GL_LINE_STRIP_ADJACENCY = 0xBU;
        public const uint GL_TRIANGLES_ADJACENCY = 0xCU;
        public const uint GL_TRIANGLE_STRIP_ADJACENCY = 0xDU;
        public const uint GL_PROGRAM_POINT_SIZE = 0x8642U;
        public const uint GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29U;
        public const uint GL_FRAMEBUFFER_ATTACHMENT_LAYERED = 0x8DA7U;
        public const uint GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8U;
        public const uint GL_GEOMETRY_SHADER = 0x8DD9U;
        public const uint GL_GEOMETRY_VERTICES_OUT = 0x8916U;
        public const uint GL_GEOMETRY_INPUT_TYPE = 0x8917U;
        public const uint GL_GEOMETRY_OUTPUT_TYPE = 0x8918U;
        public const uint GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDFU;
        public const uint GL_MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0U;
        public const uint GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1U;
        public const uint GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122U;
        public const uint GL_MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123U;
        public const uint GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124U;
        public const uint GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125U;
        public const uint GL_CONTEXT_PROFILE_MASK = 0x9126U;

        // OpenGL 3.3

        public const uint GL_VERTEX_ATTRIB_ARRAY_DIVISOR = 0x88FEU;

        // OpenGL 4.0

        public const uint GL_SAMPLE_SHADING = 0x8C36U;
        public const uint GL_MIN_SAMPLE_SHADING_VALUE = 0x8C37U;
        public const uint GL_MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5EU;
        public const uint GL_MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5FU;
        public const uint GL_TEXTURE_CUBE_MAP_ARRAY = 0x9009U;
        public const uint GL_TEXTURE_BINDING_CUBE_MAP_ARRAY = 0x900AU;
        public const uint GL_PROXY_TEXTURE_CUBE_MAP_ARRAY = 0x900BU;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY = 0x900CU;
        public const uint GL_SAMPLER_CUBE_MAP_ARRAY_SHADOW = 0x900DU;
        public const uint GL_INT_SAMPLER_CUBE_MAP_ARRAY = 0x900EU;
        public const uint GL_UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY = 0x900FU;

        // Extensions

        public const uint GL_COLOR_ATTACHMENT0_EXT = 0x8CE0U;
        public const uint GL_DEPTH_ATTACHMENT_EXT = 0x8D00U;
        public const uint GL_FRAMEBUFFER_EXT = 0x8D40U;
        public const uint GL_RENDERBUFFER_EXT = 0x8D41U;
        public const uint GL_TEXTURE_2D_MULTISAMPLE = 0x9100U;
        public const int WGL_CONTEXT_MAJOR_VERSION_ARB = 0x2091;
        public const int WGL_CONTEXT_MINOR_VERSION_ARB = 0x2092;
        public const int WGL_CONTEXT_FLAGS_ARB = 0x2094;
        public const int WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x2;
    }
}