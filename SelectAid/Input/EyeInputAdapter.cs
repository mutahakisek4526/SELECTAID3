using SelectAid.Services;

namespace SelectAid.Input;

public sealed class EyeInputAdapter
{
    private readonly InputSendService _inputSend;

    public EyeInputAdapter(InputSendService inputSend)
    {
        _inputSend = inputSend;
    }

    public void MovePointerAbsolute(int x, int y)
    {
        _inputSend.MoveAbsolute(x, y);
    }
}
