using System;
using System.Runtime.InteropServices;

namespace VC
{
    internal static class BcmHost
    {
        [DllImport("bcm_host", EntryPoint = "bcm_host_init")]
        internal extern static void Init();
        [DllImport("bcm_host", EntryPoint = "bcm_host_deinit")]
        internal extern static void Deinit();

        [DllImport("bcm_host", EntryPoint = "graphics_get_display_size")]
        internal extern static int GetDisplaySize(ushort displayNumber, out uint width, out uint height);
    }
}
