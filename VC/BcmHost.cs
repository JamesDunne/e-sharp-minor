using System;
using System.Runtime.InteropServices;

namespace VC
{
    public static class BcmHost
    {
        [DllImport("bcm_host", EntryPoint = "bcm_host_init")]
        public extern static void Init();
        [DllImport("bcm_host", EntryPoint = "bcm_host_deinit")]
        public extern static void Deinit();

        [DllImport("bcm_host", EntryPoint = "graphics_get_display_size")]
        public extern static int GetDisplaySize(ushort displayNumber, out uint width, out uint height);
    }
}