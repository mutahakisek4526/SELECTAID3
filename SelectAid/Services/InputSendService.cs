using System;
using System.Runtime.InteropServices;

namespace SelectAid.Services;

public sealed class InputSendService
{
    private const int InputMouse = 0;
    private const int InputKeyboard = 1;
    private const int MouseEventLeftDown = 0x0002;
    private const int MouseEventLeftUp = 0x0004;
    private const int MouseEventRightDown = 0x0008;
    private const int MouseEventRightUp = 0x0010;
    private const int MouseEventWheel = 0x0800;
    private const int MouseEventMove = 0x0001;
    private const int MouseEventAbsolute = 0x8000;
    private const int MouseEventMiddleDown = 0x0020;
    private const int MouseEventMiddleUp = 0x0040;
    private const int KeyEventKeyUp = 0x0002;

    public void LeftClick()
    {
        MouseClick(MouseEventLeftDown | MouseEventLeftUp);
    }

    public void RightClick()
    {
        MouseClick(MouseEventRightDown | MouseEventRightUp);
    }

    public void DoubleClick()
    {
        LeftClick();
        LeftClick();
    }

    public void DragStart()
    {
        MouseClick(MouseEventLeftDown);
    }

    public void DragEnd()
    {
        MouseClick(MouseEventLeftUp);
    }

    public void Scroll(int delta)
    {
        var input = new Input
        {
            Type = InputMouse,
            Data = new InputUnion
            {
                Mouse = new MouseInput
                {
                    MouseData = delta,
                    Flags = MouseEventWheel
                }
            }
        };
        SendInputs(new[] { input });
    }

    public void MoveAbsolute(int x, int y)
    {
        var screenWidth = GetSystemMetrics(0);
        var screenHeight = GetSystemMetrics(1);
        var mappedX = (int)Math.Round(x * 65535.0 / Math.Max(1, screenWidth - 1));
        var mappedY = (int)Math.Round(y * 65535.0 / Math.Max(1, screenHeight - 1));

        var input = new Input
        {
            Type = InputMouse,
            Data = new InputUnion
            {
                Mouse = new MouseInput
                {
                    X = mappedX,
                    Y = mappedY,
                    Flags = MouseEventMove | MouseEventAbsolute
                }
            }
        };
        SendInputs(new[] { input });
    }

    public void SendKey(ushort key)
    {
        var down = new Input
        {
            Type = InputKeyboard,
            Data = new InputUnion { Keyboard = new KeyboardInput { Key = key } }
        };
        var up = new Input
        {
            Type = InputKeyboard,
            Data = new InputUnion { Keyboard = new KeyboardInput { Key = key, Flags = KeyEventKeyUp } }
        };
        SendInputs(new[] { down, up });
    }

    public void SendKeyWithModifiers(ushort key, params ushort[] modifiers)
    {
        var inputs = new Input[modifiers.Length * 2 + 2];
        var index = 0;
        foreach (var modifier in modifiers)
        {
            inputs[index++] = new Input { Type = InputKeyboard, Data = new InputUnion { Keyboard = new KeyboardInput { Key = modifier } } };
        }

        inputs[index++] = new Input { Type = InputKeyboard, Data = new InputUnion { Keyboard = new KeyboardInput { Key = key } } };
        inputs[index++] = new Input { Type = InputKeyboard, Data = new InputUnion { Keyboard = new KeyboardInput { Key = key, Flags = KeyEventKeyUp } } };

        for (var i = modifiers.Length - 1; i >= 0; i--)
        {
            inputs[index++] = new Input { Type = InputKeyboard, Data = new InputUnion { Keyboard = new KeyboardInput { Key = modifiers[i], Flags = KeyEventKeyUp } } };
        }

        SendInputs(inputs);
    }

    private static void MouseClick(int flags)
    {
        var input = new Input
        {
            Type = InputMouse,
            Data = new InputUnion
            {
                Mouse = new MouseInput
                {
                    Flags = flags
                }
            }
        };
        SendInputs(new[] { input });
    }

    private static void SendInputs(Input[] inputs)
    {
        _ = SendInput(checked((uint)inputs.Length), inputs, Marshal.SizeOf<Input>());
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint inputs, Input[] input, int size);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public int Type;
        public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)]
        public MouseInput Mouse;
        [FieldOffset(0)]
        public KeyboardInput Keyboard;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int X;
        public int Y;
        public int MouseData;
        public int Flags;
        public int Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInput
    {
        public ushort Key;
        public ushort Scan;
        public int Flags;
        public int Time;
        public IntPtr ExtraInfo;
    }
}
