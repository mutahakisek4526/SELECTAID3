using System;
using SelectAid.Models;

namespace SelectAid.Input;

public sealed class InputRouter
{
    public event EventHandler<InputAction>? ActionTriggered;

    public InputMode CurrentMode { get; private set; } = InputMode.EyeOnly;
    public bool IsPaused { get; private set; }
    public bool IsEmergencyStop { get; private set; }

    public void SetMode(InputMode mode)
    {
        CurrentMode = mode;
        Trigger(InputAction.PauseToggle, false);
    }

    public void Trigger(InputAction action, bool fromHardware = true)
    {
        if (IsEmergencyStop && action != InputAction.EmergencyStop)
        {
            return;
        }

        if (action == InputAction.EmergencyStop)
        {
            IsEmergencyStop = true;
        }

        if (action == InputAction.PauseToggle)
        {
            if (fromHardware)
            {
                IsPaused = !IsPaused;
            }
        }

        ActionTriggered?.Invoke(this, action);
    }

    public void ResetEmergency()
    {
        IsEmergencyStop = false;
    }
}
