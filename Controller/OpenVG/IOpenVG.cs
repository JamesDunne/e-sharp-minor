using System;
using System.Runtime.InteropServices;

namespace OpenVG
{
    public interface IOpenVG
    {
        #region VG

        int Geti(ParamType type);
        float Getf(ParamType type);
        float[] Getfv(ParamType type);
        int[] Getiv(ParamType type);

        void Seti(ParamType paramType, int value);
        void Setf(ParamType paramType, float value);
        void Setfv(ParamType paramType, float[] values);

        void SetParameteri(uint handle, int paramType, int value);
        void SetParameterfv(uint handle, int paramType, float[] values);

        void Clear(int x, int y, int width, int height);

        void LoadIdentity();
        void Translate(float tx, float ty);
        void Scale(float sx, float sy);
        void Shear(float shx, float shy);
        void Rotate(float angle);

        PathHandle CreatePath(
            int pathFormat,
            PathDatatype datatype,
            float scale, float bias,
            int segmentCapacityHint,
            int coordCapacityHint,
            PathCapabilities capabilities
        );
        PathHandle CreatePathStandardFloat();
        void DestroyPath(PathHandle path);

        void DrawPath(PathHandle path, PaintMode paintModes);
        void AppendPathData(PathHandle path, byte[] segments, float[] coords);

        PaintHandle CreatePaint();
        void DestroyPaint(PaintHandle paint);
        PaintHandle GetPaint(PaintMode paintModes);
        void SetPaint(PaintHandle paint, PaintMode paintModes);

        FontHandle CreateFont(int glyphCapacityHint);
        void DestroyFont(FontHandle font);
        void SetGlyphToPath(FontHandle font, int glyphIndex, PathHandle path, bool isHinted, float[] origin, float[] escapement);

        #endregion

        #region VG Properties

        float[] ClearColor { get; set; }

        PaintHandle StrokePaint { get; set; }

        PaintHandle FillPaint { get; set; }

        #endregion

        #region VGU

        uint Line(PathHandle path, float x0, float y0, float x1, float y1);

        uint RoundRect(PathHandle path, float x, float y, float width, float height, float arcWidth, float arcHeight);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PaintHandle
    {
        public uint Handle;

        private PaintHandle(uint handle) => Handle = handle;

        public static implicit operator uint(PaintHandle paint)
        {
            return paint.Handle;
        }

        public static implicit operator PaintHandle(uint handle)
        {
            return new PaintHandle(handle);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PathHandle
    {
        public uint Handle;

        private PathHandle(uint handle) => Handle = handle;

        public static implicit operator uint(PathHandle paint)
        {
            return paint.Handle;
        }

        public static implicit operator PathHandle(uint handle)
        {
            return new PathHandle(handle);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FontHandle
    {
        public uint Handle;

        private FontHandle(uint handle) => Handle = handle;

        public static implicit operator uint(FontHandle paint)
        {
            return paint.Handle;
        }

        public static implicit operator FontHandle(uint handle)
        {
            return new FontHandle(handle);
        }
    }

    static class Constants
    {
        internal const int VG_MAX_ENUM = 0x7FFFFFFF;

        public const int VG_PATH_FORMAT_STANDARD = 0;
    }

    public enum PathDatatype : int
    {
        VG_PATH_DATATYPE_S_8 = 0,
        VG_PATH_DATATYPE_S_16 = 1,
        VG_PATH_DATATYPE_S_32 = 2,
        VG_PATH_DATATYPE_F = 3,

        VG_PATH_DATATYPE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    [Flags]
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

        VG_PATH_CAPABILITIES_FORCE_SIZE = Constants.VG_MAX_ENUM
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

        VG_PARAM_TYPE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    public enum MatrixMode : int
    {
        VG_MATRIX_PATH_USER_TO_SURFACE = 0x1400,
        VG_MATRIX_IMAGE_USER_TO_SURFACE = 0x1401,
        VG_MATRIX_FILL_PAINT_TO_USER = 0x1402,
        VG_MATRIX_STROKE_PAINT_TO_USER = 0x1403,
        VG_MATRIX_GLYPH_USER_TO_SURFACE = 0x1404,

        VG_MATRIX_MODE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    public enum FillRule : int
    {
        VG_EVEN_ODD = 0x1900,
        VG_NON_ZERO = 0x1901,

        VG_FILL_RULE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    [Flags]
    public enum PaintMode : uint
    {
        VG_STROKE_PATH = (1 << 0),
        VG_FILL_PATH = (1 << 1),

        VG_PAINT_MODE_FORCE_SIZE = Constants.VG_MAX_ENUM
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

        VG_PAINT_PARAM_TYPE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    public enum PaintType : int
    {
        VG_PAINT_TYPE_COLOR = 0x1B00,
        VG_PAINT_TYPE_LINEAR_GRADIENT = 0x1B01,
        VG_PAINT_TYPE_RADIAL_GRADIENT = 0x1B02,
        VG_PAINT_TYPE_PATTERN = 0x1B03,

        VG_PAINT_TYPE_FORCE_SIZE = Constants.VG_MAX_ENUM
    }

    public enum PathSegment : byte
    {
        VG_CLOSE_PATH = (0 << 1),
        VG_MOVE_TO = (1 << 1),
        VG_LINE_TO = (2 << 1),
        //VG_HLINE_TO = (3 << 1),
        //VG_VLINE_TO = (4 << 1),
        VG_QUAD_TO = (5 << 1),
        VG_CUBIC_TO = (6 << 1),
        VG_SQUAD_TO = (7 << 1),
        //VG_SCUBIC_TO = (8 << 1),
        //VG_SCCWARC_TO = (9 << 1),
        //VG_SCWARC_TO = (10 << 1),
        //VG_LCCWARC_TO = (11 << 1),
        //VG_LCWARC_TO = (12 << 1),

        //VG_SEGMENT_MASK = 0xf << 1,

        //VG_PATH_SEGMENT_FORCE_SIZE = Constants.VG_MAX_ENUM
    }
}