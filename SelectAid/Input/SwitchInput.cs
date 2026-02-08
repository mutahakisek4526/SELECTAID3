using System.Windows.Input;
using SelectAid.Models;

namespace SelectAid.Input;

public sealed class SwitchInput
{
    private readonly InputRouter _router;

    public SwitchInput(InputRouter router)
    {
        _router = router;
    }

    public void HandleKeyDown(Key key)
    {
        if (key == Key.Space || key == Key.Enter)
        {
            _router.Trigger(InputAction.Confirm);
        }
        else if (key == Key.Back)
        {
            _router.Trigger(InputAction.Cancel);
        }
        else if (key == Key.Escape)
        {
            _router.Trigger(InputAction.EmergencyStop);
        }
        else if (key == Key.Pause)
        {
            _router.Trigger(InputAction.PauseToggle);
        }
    }
}
