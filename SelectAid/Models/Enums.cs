namespace SelectAid.Models;

public enum InputMode
{
    EyeOnly,
    EyeSwitch,
    EyeGyro,
    GyroOnly,
    SwitchScan
}

public enum InputAction
{
    PointerMove,
    PointerMoveAbs,
    LeftClick,
    RightClick,
    DoubleClick,
    DragStart,
    DragEnd,
    ScrollUp,
    ScrollDown,
    Confirm,
    Cancel,
    EmergencyStop,
    PauseToggle
}
