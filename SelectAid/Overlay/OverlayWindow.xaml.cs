using System;
using System.Windows;
using SelectAid.Services;

namespace SelectAid.Overlay;

public partial class OverlayWindow : Window
{
    private readonly InputSendService _inputSend;
    private bool _dragging;
    private bool _transparent;
    private MouseGridWindow? _gridWindow;
    private readonly Scan.ScanEngine _scanEngine = new();
    private readonly List<Scan.IScanTarget> _scanTargets = new();

    public OverlayWindow(InputSendService inputSend)
    {
        InitializeComponent();
        _inputSend = inputSend;
        Loaded += OnLoaded;
        Closed += (_, _) => UnsubscribeScan();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BuildScanTargets();
        SubscribeScan();
    }

    private void OnLeftClick(object sender, RoutedEventArgs e) => _inputSend.LeftClick();
    private void OnRightClick(object sender, RoutedEventArgs e) => _inputSend.RightClick();
    private void OnDoubleClick(object sender, RoutedEventArgs e) => _inputSend.DoubleClick();
    private void OnScrollUp(object sender, RoutedEventArgs e) => _inputSend.Scroll(120);
    private void OnScrollDown(object sender, RoutedEventArgs e) => _inputSend.Scroll(-120);

    private void OnDragToggle(object sender, RoutedEventArgs e)
    {
        if (_dragging)
        {
            _inputSend.DragEnd();
        }
        else
        {
            _inputSend.DragStart();
        }

        _dragging = !_dragging;
    }

    private void OnBack(object sender, RoutedEventArgs e)
    {
        _inputSend.SendKeyWithModifiers(0x25, 0x12);
    }

    private void OnTab(object sender, RoutedEventArgs e)
    {
        _inputSend.SendKey(0x09);
    }

    private void OnShiftTab(object sender, RoutedEventArgs e)
    {
        _inputSend.SendKeyWithModifiers(0x09, 0x10);
    }

    private void OnEnter(object sender, RoutedEventArgs e) => _inputSend.SendKey(0x0D);
    private void OnSpace(object sender, RoutedEventArgs e) => _inputSend.SendKey(0x20);
    private void OnBackspace(object sender, RoutedEventArgs e) => _inputSend.SendKey(0x08);

    private void OnToggleTransparent(object sender, RoutedEventArgs e)
    {
        _transparent = !_transparent;
        Background = _transparent ? System.Windows.Media.Brushes.Transparent : (System.Windows.Media.Brush)FindResource("BackgroundBrush");
        AllowsTransparency = _transparent;
        IsHitTestVisible = !_transparent;
    }

    private void OnMouseGrid(object sender, RoutedEventArgs e)
    {
        if (_gridWindow == null || !_gridWindow.IsVisible)
        {
            _gridWindow = new MouseGridWindow(_inputSend);
            _gridWindow.Show();
        }
        else
        {
            _gridWindow.Activate();
        }
    }

    private void OnPause(object sender, RoutedEventArgs e)
    {
        AppServices.InputRouter.Trigger(Models.InputAction.PauseToggle);
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void BuildScanTargets()
    {
        _scanTargets.Clear();
        _scanTargets.Add(new ButtonScanTarget(LeftButton, OnLeftClickButton));
        _scanTargets.Add(new ButtonScanTarget(RightButton, OnRightClickButton));
        _scanTargets.Add(new ButtonScanTarget(DoubleButton, OnDoubleClickButton));
        _scanTargets.Add(new ButtonScanTarget(DragButton, OnDragToggleButton));
        _scanTargets.Add(new ButtonScanTarget(ScrollUpButton, OnScrollUpButton));
        _scanTargets.Add(new ButtonScanTarget(ScrollDownButton, OnScrollDownButton));
        _scanTargets.Add(new ButtonScanTarget(BackButton, OnBackButton));
        _scanTargets.Add(new ButtonScanTarget(TabButton, OnTabButton));
        _scanTargets.Add(new ButtonScanTarget(ShiftTabButton, OnShiftTabButton));
        _scanTargets.Add(new ButtonScanTarget(EnterButton, OnEnterButton));
        _scanTargets.Add(new ButtonScanTarget(SpaceButton, OnSpaceButton));
        _scanTargets.Add(new ButtonScanTarget(BackspaceButton, OnBackspaceButton));
        _scanEngine.RegisterTargets(_scanTargets);
    }

    private void SubscribeScan()
    {
        AppServices.InputRouter.ActionTriggered += OnActionTriggered;
        AppServices.TimingController.ScanTick += OnScanTick;
    }

    private void UnsubscribeScan()
    {
        AppServices.InputRouter.ActionTriggered -= OnActionTriggered;
        AppServices.TimingController.ScanTick -= OnScanTick;
    }

    private void OnScanTick(object? sender, EventArgs e)
    {
        if (AppServices.Persistence.Settings.CurrentInputMode != Models.InputMode.SwitchScan || AppServices.InputRouter.IsPaused)
        {
            return;
        }

        Dispatcher.BeginInvoke(() => _scanEngine.Next());
    }

    private void OnActionTriggered(object? sender, Models.InputAction action)
    {
        if (AppServices.Persistence.Settings.CurrentInputMode != Models.InputMode.SwitchScan)
        {
            return;
        }

        if (action == Models.InputAction.Confirm)
        {
            Dispatcher.BeginInvoke(() => _scanEngine.SelectCurrent());
        }
        else if (action == Models.InputAction.Cancel)
        {
            Dispatcher.BeginInvoke(() => _scanEngine.ResetCycle());
        }
    }

    private void OnLeftClickButton() => _inputSend.LeftClick();
    private void OnRightClickButton() => _inputSend.RightClick();
    private void OnDoubleClickButton() => _inputSend.DoubleClick();
    private void OnDragToggleButton() => OnDragToggle(this, new RoutedEventArgs());
    private void OnScrollUpButton() => _inputSend.Scroll(120);
    private void OnScrollDownButton() => _inputSend.Scroll(-120);
    private void OnBackButton() => OnBack(this, new RoutedEventArgs());
    private void OnTabButton() => _inputSend.SendKey(0x09);
    private void OnShiftTabButton() => _inputSend.SendKeyWithModifiers(0x09, 0x10);
    private void OnEnterButton() => _inputSend.SendKey(0x0D);
    private void OnSpaceButton() => _inputSend.SendKey(0x20);
    private void OnBackspaceButton() => _inputSend.SendKey(0x08);

    private sealed class ButtonScanTarget : Scan.ScanTargetBase
    {
        private readonly Action _action;
        private readonly string _label;
        private readonly System.Windows.Controls.Button _button;

        public ButtonScanTarget(System.Windows.Controls.Button button, Action action)
        {
            _button = button;
            _label = button.Content?.ToString() ?? string.Empty;
            _action = action;
            PropertyChanged += (_, _) => UpdateHighlight();
        }

        public override string Label => _label;

        public override void Activate()
        {
            _action();
        }

        private void UpdateHighlight()
        {
            _button.Dispatcher.BeginInvoke(() =>
            {
                _button.BorderThickness = IsHighlighted ? new Thickness(4) : new Thickness(2);
                _button.Background = IsHighlighted ? (System.Windows.Media.Brush)_button.FindResource("AccentBrush") : (System.Windows.Media.Brush)_button.FindResource("CardBrush");
            });
        }
    }
}
