using System;
using System.Runtime.InteropServices;

namespace VC
{
    public class DispmanXDisplay : IDisposable
    {
        internal readonly BcmDisplay bcmDisplay;

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_display_open")]
        extern static uint vc_dispmanx_display_open(uint device);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_update_start")]
        extern static uint vc_dispmanx_update_start(int priority);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_element_add")]
        extern static uint vc_dispmanx_element_add(
            uint update,
            uint display,
            int layer,
            ref VC_RECT_T dest_rect,
            uint src,
            ref VC_RECT_T src_rect,
            DISPMANX_PROTECTION_T protection,
            IntPtr /* ref VC_DISPMANX_ALPHA_T */ alpha,
            IntPtr /* ref DISPMANX_CLAMP_T */ clamp,
            DISPMANX_TRANSFORM_T transform
        );

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_element_remove")]
        extern static int vc_dispmanx_element_remove(uint update, uint element);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_update_submit_sync")]
        extern static int vc_dispmanx_update_submit_sync(uint update);

        [DllImport("bcm_host", EntryPoint = "vc_dispmanx_display_close")]
        extern static int vc_dispmanx_display_close(uint display);

        internal uint dispman_display;
        internal uint dispman_update;
        internal uint dispman_element;

        internal DispmanXDisplay(BcmDisplay bcmDisplay)
        {
            this.bcmDisplay = bcmDisplay;

            VC_RECT_T dst_rect;
            VC_RECT_T src_rect;

            dst_rect.x = 0;
            dst_rect.y = 0;
            dst_rect.width = (int)this.bcmDisplay.width;
            dst_rect.height = (int)this.bcmDisplay.height;

            src_rect.x = 0;
            src_rect.y = 0;
            src_rect.width = (int)this.bcmDisplay.width << 16;
            src_rect.height = (int)this.bcmDisplay.height << 16;

            // TODO: translate error codes into exceptions?
            this.dispman_display = vc_dispmanx_display_open(this.bcmDisplay.display);
            this.dispman_update = vc_dispmanx_update_start(0 /* priority */);

            this.dispman_element = vc_dispmanx_element_add(
                this.dispman_update,
                this.dispman_display,
                1 /*layer*/,
                ref dst_rect,
                0 /*src*/,
                ref src_rect,
                DISPMANX_PROTECTION_T.DISPMANX_PROTECTION_NONE,
                IntPtr.Zero /*alpha*/,
                IntPtr.Zero /*clamp*/,
                0 /*transform*/
            );

            vc_dispmanx_update_submit_sync(this.dispman_update);
        }

        public void Dispose()
        {
            vc_dispmanx_element_remove(this.dispman_update, this.dispman_element);
            vc_dispmanx_display_close(this.dispman_display);
        }

        public EGLContext CreateEGLContext()
        {
            return new EGLContext(this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VC_RECT_T
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    internal enum DISPMANX_PROTECTION_T : uint
    {
        DISPMANX_PROTECTION_NONE = 0,
        DISPMANX_PROTECTION_HDCP = 11,   // Derived from the WM DRM levels, 101-300
        DISPMANX_PROTECTION_MAX = 0x0f
    }

    internal enum DISPMANX_TRANSFORM_T : uint
    {
        /* Bottom 2 bits sets the orientation */
        DISPMANX_NO_ROTATE = 0,
        DISPMANX_ROTATE_90 = 1,
        DISPMANX_ROTATE_180 = 2,
        DISPMANX_ROTATE_270 = 3,

        DISPMANX_FLIP_HRIZ = 1 << 16,
        DISPMANX_FLIP_VERT = 1 << 17,

        /* invert left/right images */
        DISPMANX_STEREOSCOPIC_INVERT = 1 << 19,
        /* extra flags for controlling 3d duplication behaviour */
        DISPMANX_STEREOSCOPIC_NONE = 0 << 20,
        DISPMANX_STEREOSCOPIC_MONO = 1 << 20,
        DISPMANX_STEREOSCOPIC_SBS = 2 << 20,
        DISPMANX_STEREOSCOPIC_TB = 3 << 20,
        DISPMANX_STEREOSCOPIC_MASK = 15 << 20,

        /* extra flags for controlling snapshot behaviour */
        DISPMANX_SNAPSHOT_NO_YUV = 1 << 24,
        DISPMANX_SNAPSHOT_NO_RGB = 1 << 25,
        DISPMANX_SNAPSHOT_FILL = 1 << 26,
        DISPMANX_SNAPSHOT_SWAP_RED_BLUE = 1 << 27,
        DISPMANX_SNAPSHOT_PACK = 1 << 28
    }

    internal enum DISPMANX_FLAGS_ALPHA_T
    {
        /* Bottom 2 bits sets the alpha mode */
        DISPMANX_FLAGS_ALPHA_FROM_SOURCE = 0,
        DISPMANX_FLAGS_ALPHA_FIXED_ALL_PIXELS = 1,
        DISPMANX_FLAGS_ALPHA_FIXED_NON_ZERO = 2,
        DISPMANX_FLAGS_ALPHA_FIXED_EXCEED_0X07 = 3,

        DISPMANX_FLAGS_ALPHA_PREMULT = 1 << 16,
        DISPMANX_FLAGS_ALPHA_MIX = 1 << 17
    }

    internal struct VC_DISPMANX_ALPHA_T
    {
        public DISPMANX_FLAGS_ALPHA_T flags;
        public uint opacity;
        public uint mask;
    }

    enum DISPMANX_FLAGS_CLAMP_T : uint
    {
        DISPMANX_FLAGS_CLAMP_NONE = 0,
        DISPMANX_FLAGS_CLAMP_LUMA_TRANSPARENT = 1,
        // NOTE(jsd): Wild guess here.
        //#if __VCCOREVER__ >= 0x04000000
        DISPMANX_FLAGS_CLAMP_TRANSPARENT = 2,
        DISPMANX_FLAGS_CLAMP_REPLACE = 3
        //#else
        //        DISPMANX_FLAGS_CLAMP_CHROMA_TRANSPARENT = 2,
        //        DISPMANX_FLAGS_CLAMP_TRANSPARENT = 3
        //#endif
    }

    enum DISPMANX_FLAGS_KEYMASK_T : uint
    {
        DISPMANX_FLAGS_KEYMASK_OVERRIDE = 1,
        DISPMANX_FLAGS_KEYMASK_SMOOTH = 1 << 1,
        DISPMANX_FLAGS_KEYMASK_CR_INV = 1 << 2,
        DISPMANX_FLAGS_KEYMASK_CB_INV = 1 << 3,
        DISPMANX_FLAGS_KEYMASK_YY_INV = 1 << 4
    }

    internal struct DISPMANX_CLAMP_KEYS_T
    {
        public byte red_upper;
        public byte red_lower;
        public byte blue_upper;
        public byte blue_lower;
        public byte green_upper;
        public byte green_lower;
    }

    internal struct DISPMANX_CLAMP_T
    {
        public DISPMANX_FLAGS_CLAMP_T mode;
        public DISPMANX_FLAGS_KEYMASK_T key_mask;
        public DISPMANX_CLAMP_KEYS_T key_value;
        public uint replace_value;
    }
}