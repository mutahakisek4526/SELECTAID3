using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Windows;
using SelectAid.Models;
using SelectAid.Services;
using UITimer = System.Timers.Timer;

namespace SelectAid.ViewModels;

public sealed class AacViewModel : ObservableObject, Scan.IScanContext
{
    private readonly Stack<string> _undoStack = new();
    private readonly UITimer _buzzerTimer;
    private string _composeText = string.Empty;
    private KeyboardLayout _currentLayout;
    private bool _buzzerActive;

    public AacViewModel()
    {
        var persistence = AppServices.Persistence;
        _currentLayout = persistence.KeyboardLayouts.Layouts.First();
        History = new ObservableCollection<HistoryItem>(persistence.History.Items);
        SpeakCommand = new RelayCommand(_ => Speak());
        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        UndoCommand = new RelayCommand(_ => Undo(), _ => _undoStack.Count > 0);
        BuzzerCommand = new RelayCommand(_ => Buzzer());
        SelectLayoutCommand = new RelayCommand(param => SelectLayout(param?.ToString() ?? string.Empty));
        KeyPressCommand = new RelayCommand(param => OnKeyPress(param as KeyDefinition));

        ControlTargets = new List<Scan.ScanTargetViewModel>
        {
            new Scan.ScanTargetViewModel(\"Speak\", SpeakCommand),
            new Scan.ScanTargetViewModel(\"Backspace\", BackspaceCommand),
            new Scan.ScanTargetViewModel(\"Clear\", ClearCommand),
            new Scan.ScanTargetViewModel(\"Undo\", UndoCommand),
            new Scan.ScanTargetViewModel(\"Buzzer\", BuzzerCommand)
        };

        _buzzerTimer = new UITimer(300) { AutoReset = false };
        _buzzerTimer.Elapsed += (_, _) =>
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                BuzzerActive = false;
            });
        };
    }

    public ObservableCollection<HistoryItem> History { get; }

    public KeyboardLayout CurrentLayout
    {
        get => _currentLayout;
        set => SetProperty(ref _currentLayout, value);
    }

    public string ComposeText
    {
        get => _composeText;
        set => SetProperty(ref _composeText, value);
    }

    public bool BuzzerActive
    {
        get => _buzzerActive;
        set => SetProperty(ref _buzzerActive, value);
    }

    public RelayCommand SpeakCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand BackspaceCommand { get; }
    public RelayCommand UndoCommand { get; }
    public RelayCommand BuzzerCommand { get; }
    public RelayCommand SelectLayoutCommand { get; }
    public RelayCommand KeyPressCommand { get; }

    public List<Scan.ScanTargetViewModel> ControlTargets { get; }

    public IReadOnlyList<KeyboardLayout> Layouts => AppServices.Persistence.KeyboardLayouts.Layouts;

    public IReadOnlyList<Scan.IScanTarget> GetScanTargets()
    {
        return ControlTargets;
    }

    private void OnKeyPress(KeyDefinition? key)
    {
        if (key == null)
        {
            return;
        }

        PushUndo();
        if (key.Action == "Backspace")
        {
            Backspace();
            return;
        }

        if (key.Action == "Clear")
        {
            Clear();
            return;
        }

        if (key.Action == "Speak")
        {
            Speak();
            return;
        }

        ComposeText += key.OutputText;
    }

    public void AppendText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        PushUndo();
        ComposeText += text;
    }

    private void Speak()
    {
        if (string.IsNullOrWhiteSpace(ComposeText))
        {
            return;
        }

        AppServices.Speech.Speak(ComposeText);
        var item = new HistoryItem { Text = ComposeText, Timestamp = DateTime.Now.ToString("s") };
        History.Insert(0, item);
        AppServices.Persistence.History.Items.Insert(0, item);
        AppServices.Persistence.SaveHistory();
        if (AppServices.Persistence.Settings.ClearAfterSpeak)
        {
            ComposeText = string.Empty;
        }
    }

    private void Clear()
    {
        ComposeText = string.Empty;
    }

    private void Backspace()
    {
        if (ComposeText.Length == 0)
        {
            return;
        }

        ComposeText = ComposeText[..^1];
    }

    private void Undo()
    {
        if (_undoStack.Count == 0)
        {
            return;
        }

        ComposeText = _undoStack.Pop();
        AppServices.Metrics.RecordUndo();
        UndoCommand.RaiseCanExecuteChanged();
    }

    private void Buzzer()
    {
        BuzzerActive = true;
        SystemSounds.Beep.Play();
        _buzzerTimer.Stop();
        _buzzerTimer.Start();
    }

    private void SelectLayout(string layoutId)
    {
        var layout = AppServices.Persistence.KeyboardLayouts.Layouts.FirstOrDefault(item => item.Id == layoutId);
        if (layout != null)
        {
            CurrentLayout = layout;
        }
    }

    private void PushUndo()
    {
        _undoStack.Push(ComposeText);
        UndoCommand.RaiseCanExecuteChanged();
    }
}
