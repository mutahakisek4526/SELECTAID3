using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SelectAid.Services;

namespace SelectAid.Overlay;

public partial class MouseGridWindow : Window
{
    private readonly InputSendService _inputSend;
    private readonly List<Rect> _stack = new();
    private Rect _currentRect;
    private int _splitCount;

    public MouseGridWindow(InputSendService inputSend)
    {
        InitializeComponent();
        _inputSend = inputSend;
        _splitCount = Math.Clamp(AppServices.Persistence.Settings.MouseGridSplit, 2, 6);
        _currentRect = SystemParameters.WorkArea;
        BuildGrid();
    }

    private void BuildGrid()
    {
        GridCells.Children.Clear();
        GridCells.Columns = _splitCount;
        GridCells.Rows = _splitCount;

        var total = _splitCount * _splitCount;
        for (var i = 0; i < total; i++)
        {
            var button = new Button
            {
                Content = (i + 1).ToString(),
                Background = (Brush)FindResource("CardBrush")
            };
            var index = i;
            button.Click += (_, _) => SelectCell(index);
            GridCells.Children.Add(button);
        }
    }

    private void SelectCell(int index)
    {
        var cellWidth = _currentRect.Width / _splitCount;
        var cellHeight = _currentRect.Height / _splitCount;
        var row = index / _splitCount;
        var col = index % _splitCount;
        var next = new Rect(
            _currentRect.Left + col * cellWidth,
            _currentRect.Top + row * cellHeight,
            cellWidth,
            cellHeight);

        _stack.Add(_currentRect);
        _currentRect = next;

        if (_stack.Count >= 2)
        {
            ShowConfirm();
        }
    }

    private void ShowConfirm()
    {
        GridCells.Children.Clear();
        GridCells.Columns = 1;
        GridCells.Rows = 1;
        var label = new TextBlock
        {
            Text = "Select Left/Right Click",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 36
        };
        GridCells.Children.Add(label);
    }

    private void OnLeftClick(object sender, RoutedEventArgs e)
    {
        if (_stack.Count == 0)
        {
            return;
        }

        ClickAtCurrent(true);
    }

    private void OnRightClick(object sender, RoutedEventArgs e)
    {
        if (_stack.Count == 0)
        {
            return;
        }

        ClickAtCurrent(false);
    }

    private void ClickAtCurrent(bool left)
    {
        var x = (int)Math.Round(_currentRect.Left + _currentRect.Width / 2.0);
        var y = (int)Math.Round(_currentRect.Top + _currentRect.Height / 2.0);
        _inputSend.MoveAbsolute(x, y);
        if (left)
        {
            _inputSend.LeftClick();
        }
        else
        {
            _inputSend.RightClick();
        }

        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        if (_stack.Count == 0)
        {
            Close();
            return;
        }

        _currentRect = _stack.Last();
        _stack.RemoveAt(_stack.Count - 1);
        BuildGrid();
    }
}
