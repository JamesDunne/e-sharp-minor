using System;

namespace EMinor
{
    public interface IMIDI : IDisposable
    {
        void SetController(int channel, int controller, int value);
        void SetProgram(int channel, int program);

        void StartBatch();
        void EndBatch();
    }
}