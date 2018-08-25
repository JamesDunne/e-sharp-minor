using System;

namespace e_sharp_minor
{
    public interface IMIDI : IDisposable
    {
        void SetController(int channel, int controller, int value);
        void SetProgram(int channel, int program);
    }
}