using System;
using System.Runtime.InteropServices;

namespace VC
{
    public static class VG
    {
        const string vg = "OpenVG";

        [DllImport(vg, EntryPoint = "vgSetfv")]
        extern public static void Setfv([MarshalAs(UnmanagedType.I4)] ParamType paramType, float[] values);

        [DllImport(vg, EntryPoint = "vgClear")]
        extern public static void Clear(int x, int y, int width, int height);

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

            VG_PARAM_TYPE_FORCE_SIZE = 0x7FFFFFFF
        }
    }
}
