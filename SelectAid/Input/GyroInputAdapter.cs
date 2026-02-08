using System.Windows.Forms;
using SelectAid.Services;

namespace SelectAid.Input;

public sealed class GyroInputAdapter
{
    private readonly InputSendService _inputSend;

    public GyroInputAdapter(InputSendService inputSend)
    {
        _inputSend = inputSend;
    }

    public void MovePointerRelative(int dx, int dy)
    {
        var cursor = Cursor.Position;
        _inputSend.MoveAbsolute(cursor.X + dx, cursor.Y + dy);
    }
}
