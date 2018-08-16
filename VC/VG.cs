using System;
using System.Runtime.InteropServices;

namespace VC
{
    public static class VG
    {
        #region DllImports

        const string vg = "OpenVG";

        [DllImport(vg, EntryPoint = "vgSetfv")]
        extern public static void Setfv(ParamType paramType, float[] values);

        [DllImport(vg, EntryPoint = "vgSetParameteri")]
        extern public static void SetPaintParameteri(uint paint, PaintParamType paramType, int value);

        [DllImport(vg, EntryPoint = "vgSetParameterfv")]
        extern public static void SetPaintParameterfv(uint paint, PaintParamType paramType, int count, float[] values);

        [DllImport(vg, EntryPoint = "vgClear")]
        extern public static void Clear(int x, int y, int width, int height);

        [DllImport(vg, EntryPoint = "vgCreatePath")]
        extern public static uint CreatePath(int pathFormat,
                                PathDatatype datatype,
                                float scale, float bias,
                                int segmentCapacityHint,
                                int coordCapacityHint,
                                PathCapabilities capabilities);

        [DllImport(vg, EntryPoint = "vgDrawPath")]
        extern public static void DrawPath(uint path, PaintMode paintModes);

        [DllImport(vg, EntryPoint = "vgCreatePaint")]
        extern public static uint CreatePaint();

        [DllImport(vg, EntryPoint = "vgSetPaint")]
        extern public static void SetPaint(uint paint, PaintMode paintModes);

        #endregion

        const int VG_MAX_ENUM = 0x7FFFFFFF;

        public const int VG_PATH_FORMAT_STANDARD = 0;

        public enum PathDatatype : int
        {
            VG_PATH_DATATYPE_S_8 = 0,
            VG_PATH_DATATYPE_S_16 = 1,
            VG_PATH_DATATYPE_S_32 = 2,
            VG_PATH_DATATYPE_F = 3,

            VG_PATH_DATATYPE_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum PathCapabilities : uint
        {
            VG_PATH_CAPABILITY_APPEND_FROM = (1 << 0),
            VG_PATH_CAPABILITY_APPEND_TO = (1 << 1),
            VG_PATH_CAPABILITY_MODIFY = (1 << 2),
            VG_PATH_CAPABILITY_TRANSFORM_FROM = (1 << 3),
            VG_PATH_CAPABILITY_TRANSFORM_TO = (1 << 4),
            VG_PATH_CAPABILITY_INTERPOLATE_FROM = (1 << 5),
            VG_PATH_CAPABILITY_INTERPOLATE_TO = (1 << 6),
            VG_PATH_CAPABILITY_PATH_LENGTH = (1 << 7),
            VG_PATH_CAPABILITY_POINT_ALONG_PATH = (1 << 8),
            VG_PATH_CAPABILITY_TANGENT_ALONG_PATH = (1 << 9),
            VG_PATH_CAPABILITY_PATH_BOUNDS = (1 << 10),
            VG_PATH_CAPABILITY_PATH_TRANSFORMED_BOUNDS = (1 << 11),
            VG_PATH_CAPABILITY_ALL = (1 << 12) - 1,

            VG_PATH_CAPABILITIES_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum ParamType : int
        {
            /* Mode settings */
            VG_MATRIX_MODE = 0x1100,
            VG_FILL_RULE = 0x1101,
            VG_IMAGE_QUALITY = 0x1102,
            VG_RENDERING_QUALITY = 0x1103,
            VG_BLEND_MODE = 0x1104,
            VG_IMAGE_MODE = 0x1105,

            /* Scissoring rectangles */
            VG_SCISSOR_RECTS = 0x1106,

            /* Color Transformation */
            VG_COLOR_TRANSFORM = 0x1170,
            VG_COLOR_TRANSFORM_VALUES = 0x1171,

            /* Stroke parameters */
            VG_STROKE_LINE_WIDTH = 0x1110,
            VG_STROKE_CAP_STYLE = 0x1111,
            VG_STROKE_JOIN_STYLE = 0x1112,
            VG_STROKE_MITER_LIMIT = 0x1113,
            VG_STROKE_DASH_PATTERN = 0x1114,
            VG_STROKE_DASH_PHASE = 0x1115,
            VG_STROKE_DASH_PHASE_RESET = 0x1116,

            /* Edge fill color for VG_TILE_FILL tiling mode */
            VG_TILE_FILL_COLOR = 0x1120,

            /* Color for vgClear */
            VG_CLEAR_COLOR = 0x1121,

            /* Glyph origin */
            VG_GLYPH_ORIGIN = 0x1122,

            /* Enable/disable alpha masking and scissoring */
            VG_MASKING = 0x1130,
            VG_SCISSORING = 0x1131,

            /* Pixel layout information */
            VG_PIXEL_LAYOUT = 0x1140,
            VG_SCREEN_LAYOUT = 0x1141,

            /* Source format selection for image filters */
            VG_FILTER_FORMAT_LINEAR = 0x1150,
            VG_FILTER_FORMAT_PREMULTIPLIED = 0x1151,

            /* Destination write enable mask for image filters */
            VG_FILTER_CHANNEL_MASK = 0x1152,

            /* Implementation limits (read-only) */
            VG_MAX_SCISSOR_RECTS = 0x1160,
            VG_MAX_DASH_COUNT = 0x1161,
            VG_MAX_KERNEL_SIZE = 0x1162,
            VG_MAX_SEPARABLE_KERNEL_SIZE = 0x1163,
            VG_MAX_COLOR_RAMP_STOPS = 0x1164,
            VG_MAX_IMAGE_WIDTH = 0x1165,
            VG_MAX_IMAGE_HEIGHT = 0x1166,
            VG_MAX_IMAGE_PIXELS = 0x1167,
            VG_MAX_IMAGE_BYTES = 0x1168,
            VG_MAX_FLOAT = 0x1169,
            VG_MAX_GAUSSIAN_STD_DEVIATION = 0x116A,

            VG_PARAM_TYPE_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum FillRule : uint
        {
            VG_EVEN_ODD = 0x1900,
            VG_NON_ZERO = 0x1901,

            VG_FILL_RULE_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum PaintMode : uint
        {
            VG_STROKE_PATH = (1 << 0),
            VG_FILL_PATH = (1 << 1),

            VG_PAINT_MODE_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum PaintParamType : int
        {
            /* Color paint parameters */
            VG_PAINT_TYPE = 0x1A00,
            VG_PAINT_COLOR = 0x1A01,
            VG_PAINT_COLOR_RAMP_SPREAD_MODE = 0x1A02,
            VG_PAINT_COLOR_RAMP_PREMULTIPLIED = 0x1A07,
            VG_PAINT_COLOR_RAMP_STOPS = 0x1A03,

            /* Linear gradient paint parameters */
            VG_PAINT_LINEAR_GRADIENT = 0x1A04,

            /* Radial gradient paint parameters */
            VG_PAINT_RADIAL_GRADIENT = 0x1A05,

            /* Pattern paint parameters */
            VG_PAINT_PATTERN_TILING_MODE = 0x1A06,

            VG_PAINT_PARAM_TYPE_FORCE_SIZE = VG_MAX_ENUM
        }

        public enum PaintType : int
        {
            VG_PAINT_TYPE_COLOR = 0x1B00,
            VG_PAINT_TYPE_LINEAR_GRADIENT = 0x1B01,
            VG_PAINT_TYPE_RADIAL_GRADIENT = 0x1B02,
            VG_PAINT_TYPE_PATTERN = 0x1B03,

            VG_PAINT_TYPE_FORCE_SIZE = VG_MAX_ENUM
        }
    }
}
