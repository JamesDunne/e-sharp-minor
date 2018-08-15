using System;
using System.Runtime.InteropServices;

namespace VC
{
    public class BcmDisplay : IDisposable
    {
        public readonly ushort display;
        public readonly uint width;
        public readonly uint height;

        public BcmDisplay(int display)
        {
            this.display = (ushort)display;

            bcm_host_init();
            graphics_get_display_size(this.display, out this.width, out this.height);
        }

        public void Dispose()
        {
            bcm_host_deinit();
        }

        public DispmanXDisplay CreateDispmanXDisplay()
        {
            return new DispmanXDisplay(this);
        }

        [DllImport("bcm_host", EntryPoint = "bcm_host_init")]
        internal extern static void bcm_host_init();
        [DllImport("bcm_host", EntryPoint = "bcm_host_deinit")]
        internal extern static void bcm_host_deinit();

        [DllImport("bcm_host", EntryPoint = "graphics_get_display_size")]
        internal extern static int graphics_get_display_size(ushort displayNumber, out uint width, out uint height);
    }
}