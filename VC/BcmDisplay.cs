using System;

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

            BcmHost.Init();
            BcmHost.GetDisplaySize(this.display, out this.width, out this.height);
        }

        public void Dispose()
        {
            BcmHost.Deinit();
        }

        public DispmanXDisplay CreateDispmanXDisplay()
        {
            return new DispmanXDisplay(this);
        }
    }
}