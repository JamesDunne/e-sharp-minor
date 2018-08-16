using System;
using System.Runtime.InteropServices;

namespace VC
{
    public static class VGU
    {
        const string vgu = "OpenVG";

        [DllImport(vgu, EntryPoint = "vguLine")]
        extern public static uint Line(uint path, float x0, float y0, float x1, float y1);
    }
}
