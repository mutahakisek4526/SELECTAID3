using System.Collections.Generic;
using System.Collections.ObjectModel;
using SelectAid.Models;

namespace SelectAid.ViewModels;

public sealed class AacViewModel : ObservableObject
{
    private readonly Stack<string> _undoStack = new();
    private string _composeText = string.Empty;
    private KeyboardLayout _currentLayout;

    public AacViewModel()
    {
        var defaultLayout = new KeyboardLayout
        {
            Id = "basic",
            Name = "Basic",
            Rows = new List<KeyboardRow>
            {
                new()
                {
                    Keys = new List<KeyDefinition>
                    {
                        new() { Label = "A", OutputText = "A" },
                        new() { Label = "I", OutputText = "I" },
                        new() { Label = "U", OutputText = "U" },
                        new() { Label = "E", OutputText = "E" },
                        new() { Label = "O", OutputText = "O" }
                    }
                },
                new()
                {
                    Keys = new List<KeyDefinition>
                    {
                        new() { Label = "Ka", OutputText = "Ka" },
                        new() { Label = "Sa", OutputText = "Sa" },
                        new() { Label = "Ta", OutputText = "Ta" },
                        new() { Label = "Na", OutputText = "Na" },
                        new() { Label = "Ha", OutputText = "Ha" }
                    }
                },
                new()
                {
                    Keys = new List<KeyDefinition>
                    {
                        new() { Label = "Space", OutputText = " " },
                        new() { Label = "Back", Action = "Backspace" },
                        new() { Label = "Clear", Action = "Clear" }
                    }
                }
            }
        };

        Layouts = new List<KeyboardLayout> { defaultLayout };
        _currentLayout = defaultLayout;
        History = new ObservableCollection<HistoryItem>
        {
            new() { Text = "こんにちは", Timestamp = "00:00" },
            new() { Text = "よろしくお願いします", Timestamp = "00:01" }
        };

        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        UndoCommand = new RelayCommand(_ => Undo(), _ => _undoStack.Count > 0);
        SpeakCommand = new RelayCommand(_ => Speak());
        SelectLayoutCommand = new RelayCommand(param => SelectLayout(param as KeyboardLayout));
        KeyPressCommand = new RelayCommand(param => OnKeyPress(param as KeyDefinition));

        ControlTargets = new List<ControlTarget>
        {
            new("Speak", SpeakCommand),
            new("Backspace", BackspaceCommand),
            new("Clear", ClearCommand),
            new("Undo", UndoCommand)
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

    public RelayCommand SpeakCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand BackspaceCommand { get; }
    public RelayCommand UndoCommand { get; }
    public RelayCommand SelectLayoutCommand { get; }
    public RelayCommand KeyPressCommand { get; }

    public List<ControlTarget> ControlTargets { get; }

    public IReadOnlyList<KeyboardLayout> Layouts { get; }

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

        var item = new HistoryItem { Text = ComposeText, Timestamp = "Now" };
        History.Insert(0, item);
        ComposeText = string.Empty;
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
        UndoCommand.RaiseCanExecuteChanged();
    }

    private void SelectLayout(KeyboardLayout? layout)
    {
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
