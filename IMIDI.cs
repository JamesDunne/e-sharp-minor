namespace e_sharp_minor
{
    public interface IMIDI
    {
        void SetController(int channel, int controller, int value);
        void SetProgram(int channel, int program);
    }
}