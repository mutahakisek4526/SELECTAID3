using System;
using System.Collections.ObjectModel;
using System.Linq;
using SelectAid.Models;
using SelectAid.Services;

namespace SelectAid.ViewModels;

public sealed class PhrasesViewModel : ObservableObject
{
    private readonly AacViewModel _aacViewModel;
    private PhraseScene? _selectedScene;
    private PhraseCategory? _selectedCategory;
    private string _newPhrase = string.Empty;

    public PhrasesViewModel(AacViewModel aacViewModel)
    {
        _aacViewModel = aacViewModel;
        Scenes = new ObservableCollection<PhraseScene>(AppServices.Persistence.Phrases.Scenes);
        AddPhraseCommand = new RelayCommand(_ => AddPhrase());
        SpeakPhraseCommand = new RelayCommand(param => SpeakPhrase(param as PhraseItem));
        InsertPhraseCommand = new RelayCommand(param => InsertPhrase(param as PhraseItem));
        if (Scenes.Count > 0)
        {
            SelectedScene = Scenes[0];
        }
    }

    public ObservableCollection<PhraseScene> Scenes { get; }

    public PhraseScene? SelectedScene
    {
        get => _selectedScene;
        set
        {
            SetProperty(ref _selectedScene, value);
            SelectedCategory = value?.Categories.FirstOrDefault();
        }
    }

    public PhraseCategory? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string NewPhrase
    {
        get => _newPhrase;
        set => SetProperty(ref _newPhrase, value);
    }

    public RelayCommand AddPhraseCommand { get; }
    public RelayCommand SpeakPhraseCommand { get; }
    public RelayCommand InsertPhraseCommand { get; }

    private void AddPhrase()
    {
        if (SelectedCategory == null || string.IsNullOrWhiteSpace(NewPhrase))
        {
            return;
        }

        var item = new PhraseItem { Id = Guid.NewGuid().ToString("N"), Text = NewPhrase };
        SelectedCategory.Items.Add(item);
        AppServices.Persistence.SavePhrases();
        NewPhrase = string.Empty;
    }

    private void SpeakPhrase(PhraseItem? item)
    {
        if (item == null)
        {
            return;
        }

        AppServices.Speech.Speak(item.Text);
    }

    private void InsertPhrase(PhraseItem? item)
    {
        if (item == null)
        {
            return;
        }
        _aacViewModel.AppendText(item.Text);
    }
}
