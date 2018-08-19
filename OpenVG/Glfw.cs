#if ! RPI
using System;
using System.Text;
using System.Runtime.InteropServices;

// Disable Warning CS0618: 'UnmanagedType.Struct' is obsolete: 'Applying UnmanagedType.Struct is unnecessary when marshalling a struct. Support for UnmanagedType.Struct when marshalling a reference type may be unavailable in future releases.' (CS0618) (e-sharp-minor)
#pragma warning disable CS0618

public static class Glfw
{
    const string dll = "glfw";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void ErrorFunc(ErrorCode error, string desc);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MonitorFunc([MarshalAs(UnmanagedType.Struct)] Monitor monitor, MonitorEvent ev);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WindowPosFunc([MarshalAs(UnmanagedType.Struct)] Window window, int x, int y);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WindowSizeFunc([MarshalAs(UnmanagedType.Struct)] Window window, int width, int height);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WindowFunc([MarshalAs(UnmanagedType.Struct)] Window window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void WindowBoolFunc([MarshalAs(UnmanagedType.Struct)] Window window, [MarshalAs(UnmanagedType.Bool)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void KeyFunc([MarshalAs(UnmanagedType.Struct)] Window window, KeyCode key, int scan, KeyMods mods);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CharFunc([MarshalAs(UnmanagedType.Struct)] Window window, char chr);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CharModFunc([MarshalAs(UnmanagedType.Struct)] Window window, char chr, KeyMods mods);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void MouseButtonFunc([MarshalAs(UnmanagedType.Struct)] Window window, MouseButton button, [MarshalAs(UnmanagedType.Bool)] bool state, KeyMods mods);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CursorPosFunc([MarshalAs(UnmanagedType.Struct)] Window window, double x, double y);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CursorBoolFunc([MarshalAs(UnmanagedType.Struct)] Window window, [MarshalAs(UnmanagedType.Bool)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DropFunc([MarshalAs(UnmanagedType.Struct)] Window window, string[] files);

    public enum ErrorCode
    {
        NotInitialized = 0x00010001,
        NoCurrentContext = 0x00010002,
        InvalidEnum = 0x00010003,
        InvalidValue = 0x00010004,
        OutOfMemory = 0x00010005,
        ApiUnavailable = 0x00010006,
        VersionUnavailable = 0x00010007,
        PlatformError = 0x00010008,
        FormatUnavailable = 0x00010009
    }

    public enum MonitorEvent
    {
        Connected = 0x00040001,
        Disconnected = 0x00040002
    }

    public enum Hint
    {
        Resizable = 0x00020003,
        Visible = 0x00020004,
        Decorated = 0x00020005,
        Focused = 0x00020001,
        AutoIconify = 0x00020006,
        Floating = 0x00020007,
        RedBits = 0x00021001,
        GreenBits = 0x00021002,
        BlueBits = 0x00021003,
        AlphaBits = 0x00021004,
        DepthBits = 0x00021005,
        StencilBits = 0x00021006,
        AccumRedBits = 0x00021007,
        AccumGreenBits = 0x00021008,
        AccumBlueBits = 0x00021009,
        AccumAlphaBits = 0x0002100A,
        AuxBuffers = 0x0002100B,
        Samples = 0x0002100D,
        RefreshRate = 0x0002100F,
        Stereo = 0x0002100C,
        SrgbCapable = 0x0002100E,
        DoubleBuffer = 0x00021010,
        ClientApi = 0x00022001,
        ContextVersionMajor = 0x00022002,
        ContextVersionMinor = 0x00022003,
        ContextRobustness = 0x00022005,
        ContextReleaseBehavior = 0x00022009,
        OpenGLForwardCompat = 0x00022006,
        OpenGLDebugContext = 0x00022007,
        OpenGLProfile = 0x00022008
    }

    public enum ClientApi
    {
        OpenGLApi = 0x00030001,
        OpenGLESApi = 0x00030002
    }

    public enum ContextRobustness
    {
        None = 0,
        NoResetNotification = 0x00031001,
        LoseContextOnReset = 0x00031002
    }

    public enum ContextReleaseBehavior
    {
        Any = 0,
        Flush = 0x00035001,
        None = 0x00035002
    }

    public enum OpenGLProfile
    {
        Any = 0,
        Core = 0x00032001,
        Compat = 0x00032002
    }

    public enum WindowAttrib
    {
        Focused = 0x00020001,
        Iconified = 0x00020002,
        Resizable = 0x00020003,
        Visible = 0x00020004,
        Decorated = 0x00020005,
        Floating = 0x00020007
    }

    public enum CursorMode
    {
        Normal = 0x00034001,
        Hidden = 0x00034002,
        Disabled = 0x00034003
    }

    public enum CursorType
    {
        Arrow = 0x00036001,
        Beam = 0x00036002,
        Crosshair = 0x00036003,
        Hand = 0x00036004,
        ResizeX = 0x00036005,
        ResizeY = 0x00036006
    }

    public enum KeyState
    {
        Release = 0,
        Press = 1,
        Repeat = 2
    }

    public enum KeyCode
    {
        Unknown = -1,
        Space = 32,
        Apostrophe = 39,
        Comma = 44,
        Minus = 45,
        Period = 46,
        Slash = 47,
        Alpha0 = 48,
        Alpha1 = 49,
        Alpha2 = 50,
        Alpha3 = 51,
        Alpha4 = 52,
        Alpha5 = 53,
        Alpha6 = 54,
        Alpha7 = 55,
        Alpha8 = 56,
        Alpha9 = 57,
        Semicolon = 59,
        Equal = 61,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LeftBracket = 91,
        Backslash = 92,
        RightBracket = 93,
        GraveAccent = 96,
        World1 = 161,
        World2 = 162,
        Escape = 256,
        Enter = 257,
        Tab = 258,
        Backspace = 259,
        Insert = 260,
        Delete = 261,
        Right = 262,
        Left = 263,
        Down = 264,
        Up = 265,
        PageUp = 266,
        PageDown = 267,
        Home = 268,
        End = 269,
        CapsLock = 280,
        ScrollLock = 281,
        NumLock = 282,
        PrintScreen = 283,
        Pause = 284,
        F1 = 290,
        F2 = 291,
        F3 = 292,
        F4 = 293,
        F5 = 294,
        F6 = 295,
        F7 = 296,
        F8 = 297,
        F9 = 298,
        F10 = 299,
        F11 = 300,
        F12 = 301,
        F13 = 302,
        F14 = 303,
        F15 = 304,
        F16 = 305,
        F17 = 306,
        F18 = 307,
        F19 = 308,
        F20 = 309,
        F21 = 310,
        F22 = 311,
        F23 = 312,
        F24 = 313,
        F25 = 314,
        Keypad0 = 320,
        Keypad1 = 321,
        Keypad2 = 322,
        Keypad3 = 323,
        Keypad4 = 324,
        Keypad5 = 325,
        Keypad6 = 326,
        Keypad7 = 327,
        Keypad8 = 328,
        Keypad9 = 329,
        KeypadDecimal = 330,
        KeypadDivide = 331,
        KeypadMultiply = 332,
        KeypadSubtract = 333,
        KeypadAdd = 334,
        KeypadEnter = 335,
        KeypadEqual = 336,
        LeftShift = 340,
        LeftControl = 341,
        LeftAlt = 342,
        LeftSuper = 343,
        RightShift = 344,
        RightControl = 345,
        RightAlt = 346,
        RightSuper = 347,
        Menu = 348
    }

    [Flags]
    public enum KeyMods
    {
        Shift = 0x0001,
        Control = 0x0002,
        Alt = 0x0004,
        Super = 0x0008
    }

    public enum Joystick
    {
        Joy0 = 0,
        Joy1 = 1,
        Joy2 = 2,
        Joy3 = 3,
        Joy4 = 4,
        Joy5 = 5,
        Joy6 = 6,
        Joy7 = 7,
        Joy8 = 8,
        Joy9 = 9,
        Joy10 = 10,
        Joy11 = 12,
        Joy12 = 13,
        Joy13 = 14,
        Joy14 = 15,
        Joy15 = 16
    }

    public enum MouseButton
    {
        Button0 = 0,
        Button1 = 1,
        Button2 = 2,
        Button3 = 3,
        Button4 = 4,
        Button5 = 5,
        Button6 = 6,
        Button7 = 7,
        Button8 = 8,
        Left = Button0,
        Right = Button1,
        Middle = Button2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Monitor
    {
        public static readonly Monitor None = new Monitor(IntPtr.Zero);

        public IntPtr Ptr;

        internal Monitor(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj is Monitor)
                return Equals((Monitor)obj);
            return false;
        }
        public bool Equals(Monitor obj)
        {
            return Ptr == obj.Ptr;
        }

        public override int GetHashCode()
        {
            return Ptr.GetHashCode();
        }

        public static bool operator ==(Monitor a, Monitor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Monitor a, Monitor b)
        {
            return !a.Equals(b);
        }

        public static implicit operator bool(Monitor obj)
        {
            return obj.Ptr != IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VideoMode
    {
        public int Width;
        public int Height;
        public int RedBits;
        public int GreenBits;
        public int BlueBits;
        public int RefreshRate;

        public VideoMode(int width, int height, int redBits, int greenBits, int blueBits, int refreshRate)
        {
            Width = width;
            Height = height;
            RedBits = redBits;
            GreenBits = greenBits;
            BlueBits = blueBits;
            RefreshRate = refreshRate;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GammaRamp
    {
        internal IntPtr Red;
        internal IntPtr Green;
        internal IntPtr Blue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Window
    {
        public static readonly Window None = new Window(IntPtr.Zero);

        public IntPtr Ptr;

        internal Window(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj is Window)
                return Equals((Window)obj);
            return false;
        }
        public bool Equals(Window obj)
        {
            return Ptr == obj.Ptr;
        }

        public override int GetHashCode()
        {
            return Ptr.GetHashCode();
        }

        public static bool operator ==(Window a, Window b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Window a, Window b)
        {
            return !a.Equals(b);
        }

        public static implicit operator bool(Window obj)
        {
            return obj.Ptr != IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Cursor
    {
        public static readonly Cursor None = new Cursor(IntPtr.Zero);

        public IntPtr Ptr;

        internal Cursor(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public override bool Equals(object obj)
        {
            if (obj is Cursor)
                return Equals((Cursor)obj);
            return false;
        }
        public bool Equals(Cursor obj)
        {
            return Ptr == obj.Ptr;
        }

        public override int GetHashCode()
        {
            return Ptr.GetHashCode();
        }

        public static bool operator ==(Cursor a, Cursor b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Cursor a, Cursor b)
        {
            return !a.Equals(b);
        }

        public static implicit operator bool(Cursor obj)
        {
            return obj.Ptr != IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Image
    {
        internal int Width;
        internal int Height;
        internal IntPtr Pixels;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwInit")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool Init();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwTerminate")]
    public static extern void Terminate();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static unsafe extern void glfwGetVersion(int* major, int* minor, int* rev);
    public static unsafe void GetVersion(out int major, out int minor, out int rev)
    {
        int a, b, c;
        glfwGetVersion(&a, &b, &c);
        major = a;
        minor = b;
        rev = c;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetVersionString();
    public static unsafe string GetVersionString()
    {
        IntPtr version = glfwGetVersionString();
        return Marshal.PtrToStringAnsi(version);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetErrorCallback(IntPtr callback);
    public static void SetErrorCallback(ErrorFunc callback)
    {
        glfwSetErrorCallback(Marshal.GetFunctionPointerForDelegate(callback));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetMonitors(int* count);
    public static unsafe Monitor[] GetMonitors()
    {
        int count;
        var array = glfwGetMonitors(&count);
        var monitors = new Monitor[count];
        var size = Marshal.SizeOf(typeof(IntPtr));
        for (int i = 0; i < count; ++i)
        {
            var ptr = Marshal.ReadIntPtr(array, i * size);
            monitors[i] = new Monitor(ptr);
        }
        return monitors;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetPrimaryMonitor")]
    [return: MarshalAs(UnmanagedType.Struct)]
    public static extern Monitor GetPrimaryMonitor();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetMonitorPos(IntPtr monitor, int* xpos, int* ypos);
    public static unsafe void GetMonitorPosition(Monitor monitor, out int x, out int y)
    {
        int xx, yy;
        glfwGetMonitorPos(monitor.Ptr, &xx, &yy);
        x = xx; y = yy;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetMonitorPhysicalSize(IntPtr monitor, int* w, int* h);
    public static unsafe void GetMonitorPhysicalSize(Monitor monitor, out int w, out int h)
    {
        int ww, hh;
        glfwGetMonitorPhysicalSize(monitor.Ptr, &ww, &hh);
        w = ww; h = hh;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetMonitorName(IntPtr monitor);
    public static unsafe string GetMonitortName(Monitor monitor)
    {
        IntPtr name = glfwGetMonitorName(monitor.Ptr);
        return Marshal.PtrToStringAnsi(name);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetMonitorCallback(IntPtr callback);
    public static void SetMonitorCallback(MonitorFunc callback)
    {
        glfwSetMonitorCallback(Marshal.GetFunctionPointerForDelegate(callback));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetVideoModes(IntPtr monitor, int* count);
    public static unsafe VideoMode[] GetVideoModes(Monitor monitor)
    {
        int count;
        var array = glfwGetVideoModes(monitor.Ptr, &count);
        var modes = new VideoMode[count];
        var size = Marshal.SizeOf(typeof(VideoMode));
        for (int i = 0; i < count; ++i)
        {
            var ptr = Marshal.ReadIntPtr(array, i * size);
            modes[i] = (VideoMode)Marshal.PtrToStructure(ptr, typeof(VideoMode));
        }
        return modes;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetVideoMode(IntPtr monitor);
    public static VideoMode GetVideoMode(Monitor monitor)
    {
        var ptr = glfwGetVideoMode(monitor.Ptr);
        return (VideoMode)Marshal.PtrToStructure(ptr, typeof(VideoMode));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetGamma")]
    public static extern void SetGamma([MarshalAs(UnmanagedType.LPStruct)] Monitor monitor, float gamma);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetGammaRamp(IntPtr monitor);
    public static unsafe void GetGammaRamp(Monitor monitor, out ushort[] red, out ushort[] green, out ushort[] blue)
    {
        var ptr = glfwGetGammaRamp(monitor.Ptr);
        var ramp = (GammaRamp)Marshal.PtrToStructure(ptr, typeof(GammaRamp));
        var r = (ushort*)ramp.Red.ToPointer();
        var g = (ushort*)ramp.Green.ToPointer();
        var b = (ushort*)ramp.Blue.ToPointer();
        red = new ushort[256];
        green = new ushort[256];
        blue = new ushort[256];
        for (int i = 0; i < 256; ++i)
            red[i] = r[i];
        for (int i = 0; i < 256; ++i)
            green[i] = g[i];
        for (int i = 0; i < 256; ++i)
            blue[i] = b[i];
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetGammaRamp(IntPtr monitor, IntPtr ramp);
    public static unsafe void SetGammaRamp(Monitor monitor, ushort[] red, ushort[] green, ushort[] blue)
    {
        var ramp = new GammaRamp();
        ramp.Red = Marshal.UnsafeAddrOfPinnedArrayElement(red, 0);
        ramp.Green = Marshal.UnsafeAddrOfPinnedArrayElement(green, 0);
        ramp.Blue = Marshal.UnsafeAddrOfPinnedArrayElement(blue, 0);
        glfwSetGammaRamp(monitor.Ptr, new IntPtr(&ramp));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwDefaultWindowHints")]
    public static extern void DefaultWindowHints();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwWindowHint(int target, int hint);

    public static void WindowHint(Hint hint, bool value)
    {
        glfwWindowHint((int)hint, value ? 1 : 0);
    }
    public static void WindowHint(Hint hint, int value)
    {
        if (value < 0)
            value = -1;
        glfwWindowHint((int)hint, value);
    }
    public static void WindowHint(Hint hint, Enum value)
    {
        glfwWindowHint((int)hint, Convert.ToInt32(value));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "glfwCreateWindow")]
    [return: MarshalAs(UnmanagedType.Struct)]
    public static extern Window CreateWindow(int width, int height, string title, [MarshalAs(UnmanagedType.Struct)] Monitor monitor, [MarshalAs(UnmanagedType.Struct)] Window share);
    public static Window CreateWindow(int width, int height, string title)
    {
        return CreateWindow(width, height, title, Monitor.None, Window.None);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwDestroyWindow")]
    public static extern void DestroyWindow([MarshalAs(UnmanagedType.Struct)] Window window);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern int glfwWindowShouldClose(IntPtr window);
    public static bool WindowShouldClose(Window window)
    {
        return glfwWindowShouldClose(window.Ptr) != 0;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetWindowShouldClose(IntPtr window, int value);
    public static void SetWindowShouldClose(Window window, bool value)
    {
        glfwSetWindowShouldClose(window.Ptr, value ? 1 : 0);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    static extern void glfwSetWindowTitle(IntPtr window, StringBuilder title);
    public static void SetWindowTitle(Window window, string title)
    {
        glfwSetWindowTitle(window.Ptr, new StringBuilder(title));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetWindowPos(IntPtr window, int* x, int* y);
    public static unsafe void GetWindowPos(Window window, out int x, out int y)
    {
        int xx, yy;
        glfwGetWindowPos(window.Ptr, &xx, &yy);
        x = xx; y = yy;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetWindowPos(IntPtr window, int x, int y);
    public static void SetWindowPos(Window window, int x, int y)
    {
        glfwSetWindowPos(window.Ptr, x, y);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetWindowSize(IntPtr window, int* width, int* height);
    public static unsafe void GetWindowSize(Window window, out int width, out int height)
    {
        int w, h;
        glfwGetWindowSize(window.Ptr, &w, &h);
        width = w; height = h;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetWindowSize(IntPtr window, int width, int height);
    public static void SetWindowSize(Window window, int width, int height)
    {
        glfwSetWindowSize(window.Ptr, width, height);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetFramebufferSize(IntPtr window, int* width, int* height);
    public static unsafe void GetFramebufferSize(Window window, out int width, out int height)
    {
        int w, h;
        glfwGetFramebufferSize(window.Ptr, &w, &h);
        width = w; height = h;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetWindowFrameSize(IntPtr window, int* left, int* top, int* right, int* bottom);
    public static unsafe void GetWindowFrameSize(Window window, out int left, out int top, out int right, out int bottom)
    {
        int l, t, r, b;
        glfwGetWindowFrameSize(window.Ptr, &l, &t, &r, &b);
        left = l; top = t; right = r; bottom = b;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwIconifyWindow(IntPtr window);
    public static void IconifyWindow(Window window)
    {
        glfwIconifyWindow(window.Ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwRestoreWindow(IntPtr window);
    public static void RestoreWindow(Window window)
    {
        glfwRestoreWindow(window.Ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwShowWindow(IntPtr window);
    public static void ShowWindow(Window window)
    {
        glfwShowWindow(window.Ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwHideWindow(IntPtr window);
    public static void HideWindow(Window window)
    {
        glfwHideWindow(window.Ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetWindowMonitor(IntPtr window);
    public static Monitor GetWindowMonitor(Window window)
    {
        var ptr = glfwGetWindowMonitor(window.Ptr);
        return new Monitor(ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern int glfwGetWindowAttrib(IntPtr window, int attrib);
    public static bool GetWindowAttrib(Window window, WindowAttrib attrib)
    {
        return glfwGetWindowAttrib(window.Ptr, (int)attrib) != 0;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetWindowUserPointer(IntPtr window, IntPtr ptr);
    public static void SetWindowUserPointer(Window window, IntPtr ptr)
    {
        glfwSetWindowUserPointer(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwGetWindowUserPointer(IntPtr window);
    public static IntPtr GetWindowUserPointer(Window window)
    {
        return glfwGetWindowUserPointer(window.Ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowPosCallback(IntPtr window, IntPtr callback);
    public static void SetWindowPosCallback(Window window, WindowPosFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowPosCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowSizeCallback(IntPtr window, IntPtr callback);
    public static void SetWindowSizeCallback(Window window, WindowSizeFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowSizeCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowCloseCallback(IntPtr window, IntPtr callback);
    public static void SetWindowCloseCallback(Window window, WindowFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowCloseCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowRefreshCallback(IntPtr window, IntPtr callback);
    public static void SetWindowRefreshCallback(Window window, WindowFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowRefreshCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowFocusCallback(IntPtr window, IntPtr callback);
    public static void SetWindowFocusCallback(Window window, WindowBoolFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowFocusCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetWindowIconifyCallback(IntPtr window, IntPtr callback);
    public static void SetWindowIconifyCallback(Window window, WindowBoolFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetWindowIconifyCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetFramebufferSizeCallback(IntPtr window, IntPtr callback);
    public static void SetFramebufferSizeCallback(Window window, WindowSizeFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetFramebufferSizeCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwPollEvents")]
    public static extern void PollEvents();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwWaitEvents")]
    public static extern void WaitEvents();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwPostEmptyEvent")]
    public static extern void PostEmptyEvent();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern int glfwGetInputMode(IntPtr window, int mode);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern void glfwSetInputMode(IntPtr window, int mode, int value);

    public static CursorMode GetCursorMode(Window window)
    {
        return (CursorMode)glfwGetInputMode(window.Ptr, 0x00033001);
    }

    public static void SetCursorMode(Window window, CursorMode mode)
    {
        glfwSetInputMode(window.Ptr, 0x00033001, (int)mode);
    }

    public static bool GetStickyKeys(Window window)
    {
        return glfwGetInputMode(window.Ptr, 0x00033002) != 0;
    }

    public static void SetStickyKeys(Window window, bool enabled)
    {
        glfwSetInputMode(window.Ptr, 0x00033002, enabled ? 1 : 0);
    }

    public static bool GetStickyMouseButtons(Window window)
    {
        return glfwGetInputMode(window.Ptr, 0x00033003) != 0;
    }

    public static void SetStickyMouseButtons(Window window, bool enabled)
    {
        glfwSetInputMode(window.Ptr, 0x00033003, enabled ? 1 : 0);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern int glfwGetKey(IntPtr window, int key);
    public static bool GetKey(Window window, KeyCode key)
    {
        return glfwGetKey(window.Ptr, (int)key) != 0;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern int glfwGetMouseButton(IntPtr window, int button);
    public static bool GetMouseButton(Window window, MouseButton button)
    {
        return glfwGetMouseButton(window.Ptr, (int)button) != 0;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe void glfwGetCursorPos(IntPtr window, double* x, double* y);
    public static unsafe void GetCursorPos(Window window, out double x, out double y)
    {
        double xx, yy;
        glfwGetCursorPos(window.Ptr, &xx, &yy);
        x = xx; y = yy;
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetCursorPos")]
    public static extern void SetCursorPos([MarshalAs(UnmanagedType.Struct)] Window window, double x, double y);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwCreateCursor(IntPtr image, int xhot, int yhot);
    public static unsafe Cursor CreateCursor(int width, int height, byte[] pixels, int xhot, int yhot)
    {
        Image image;
        image.Width = width;
        image.Height = height;

        int size = width * height * 4;
        image.Pixels = Marshal.AllocHGlobal(size);

        Marshal.Copy(pixels, 0, image.Pixels, Math.Min(size, pixels.Length));

        var ptr = new IntPtr(&image);
        ptr = glfwCreateCursor(ptr, xhot, yhot);

        Marshal.FreeHGlobal(image.Pixels);

        return new Cursor(ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwCreateStandardCursor")]
    [return: MarshalAs(UnmanagedType.Struct)]
    public static extern Cursor CreateStandardCursor(CursorType cursor);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwDestroyCursor")]
    public static extern void DestroyCursor([MarshalAs(UnmanagedType.Struct)] Cursor cursor);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetCursor")]
    public static extern void SetCursor([MarshalAs(UnmanagedType.Struct)] Window window, [MarshalAs(UnmanagedType.Struct)] Cursor cursor);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetKeyCallback(IntPtr window, IntPtr callback);
    public static void SetKeyCallback(Window window, KeyFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetKeyCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetCharCallback(IntPtr window, IntPtr callback);
    public static void SetCharCallback(Window window, CharFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetCharCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetCharModsCallback(IntPtr window, IntPtr callback);
    public static void SetCharModsCallback(Window window, CharModFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetCharModsCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetMouseButtonCallback(IntPtr window, IntPtr callback);
    public static void SetMouseButtonCallback(Window window, MouseButtonFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetMouseButtonCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetCursorPosCallback(IntPtr window, IntPtr callback);
    public static void SetCursorPosCallback(Window window, CursorPosFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetCursorPosCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetCursorEnterCallback(IntPtr window, IntPtr callback);
    public static void SetCursorEnterCallback(Window window, CursorBoolFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetCursorEnterCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetScrollCallback(IntPtr window, IntPtr callback);
    public static void SetScrollCallback(Window window, CursorPosFunc callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        glfwSetScrollCallback(window.Ptr, ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr glfwSetDropCallback(IntPtr window, IntPtr callback);
    public static void SetDropCallback(Window window, DropFunc callback)
    {
        var call = new Action<IntPtr, int, IntPtr>((w, n, p) =>
          {
              var files = new string[n];
              var size = Marshal.SizeOf(typeof(IntPtr));
              for (int i = 0; i < n; ++i)
              {
                  var ptr = Marshal.ReadIntPtr(p, size * i);
                  files[i] = Marshal.PtrToStringAnsi(ptr);
              }
              callback(window, files);
          });
        var callPtr = Marshal.GetFunctionPointerForDelegate(call);
        glfwSetDropCallback(window.Ptr, callPtr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwJoystickPresent")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool JoystickPresent(Joystick joy);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetJoystickAxes(int joy, int* count);
    public static unsafe void GetJoystickAxes(Joystick joy, out int count, float[] axes)
    {
        if (axes == null || axes.Length == 0)
        {
            count = 0;
            return;
        }
        int n;
        var array = glfwGetJoystickAxes((int)joy, &n);
        count = n;
        Marshal.Copy(array, axes, 0, Math.Min(n, axes.Length));
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetJoystickButtons(int joy, int* count);
    public static unsafe void GetJoystickButtons(Joystick joy, out int count, bool[] buttons)
    {
        if (buttons == null || buttons.Length == 0)
        {
            count = 0;
            return;
        }
        int n;
        var array = glfwGetJoystickButtons((int)joy, &n);
        count = n;
        if (n == 0 || array != IntPtr.Zero)
            return;
        var ptr = Marshal.ReadIntPtr(array, 0);
        var size = Marshal.SizeOf(ptr);
        n = Math.Min(n, buttons.Length);
        for (int i = 0; i < n; ++i)
        {
            ptr = Marshal.ReadIntPtr(array, i * size);
            buttons[i] = (bool)Marshal.PtrToStructure(ptr, typeof(bool));
        }
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetJoystickName(int joy);
    public static string GetJoystickName(Joystick joy)
    {
        var ptr = glfwGetJoystickName((int)joy);
        return Marshal.PtrToStringAnsi(ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "glfwSetClipboardString")]
    public static extern void SetClipboardString([MarshalAs(UnmanagedType.Struct)] Window window, string value);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe IntPtr glfwGetClipboardString(IntPtr window);
    public static string GetClipboardString(Window window)
    {
        var ptr = glfwGetClipboardString(window.Ptr);
        return Marshal.PtrToStringAnsi(ptr);
    }

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetTime")]
    public static extern double GetTime();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSetTime")]
    public static extern void SetTime(double time);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwMakeContextCurrent")]
    public static extern void MakeContextCurrent([MarshalAs(UnmanagedType.Struct)] Window window);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetCurrentContext")]
    [return: MarshalAs(UnmanagedType.Struct)]
    public static extern Window GetCurrentContext();

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSwapBuffers")]
    public static extern void SwapBuffers([MarshalAs(UnmanagedType.Struct)] Window window);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwSwapInterval")]
    public static extern void SwapInterval(int interval);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "glfwExtensionSupported")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ExtensionSupported(string extension);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "glfwGetProcAddress")]
    public static extern IntPtr GetProcAddress(string name);

    [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "glfwGetNSGLContext")]
    public static extern IntPtr GetNSGLContext([MarshalAs(UnmanagedType.Struct)] Window window);
}
#endif
