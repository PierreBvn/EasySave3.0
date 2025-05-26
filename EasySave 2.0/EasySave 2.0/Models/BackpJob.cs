using EasySave_2._0.Models;
using EasySave_2._0.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

public class BackupJob : INotifyPropertyChanged
{
    private static LanguageService _languageService = new LanguageService();
    public int Id { get; set; }
    private string _name;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(nameof(Name)); }
    }
    public string Source { get; set; }
    public string Target { get; set; }
    public IBackupType Type { get; set; }
    public int Count { get; internal set; }

    public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    public ManualResetEventSlim PauseEvent { get; set; } = new(true);
    public int Progress { get; set; }
    //public string Etat { get; set; } = "En attente";
    public bool IsRunning => Etat == "ACTIVE" || Etat == "En pause" || Etat == "En cours";

    private string _etat;
    public string Etat
    {
        get => _etat;
        set
        {
            _etat = value;
            OnPropertyChanged(nameof(Etat));
            OnPropertyChanged(nameof(IsRunning)); // Important si IsRunning dépend de Etat
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public static void SetLanguageService(LanguageService languageService)
    {
        _languageService = languageService;
    }

    public void Execute()
    {
        if (!Directory.Exists(Source))
        {
            MessageBox.Show(_languageService.GetTranslation("SourceNotFound"), Source);

            return;
        }

        if (!Directory.Exists(Target))
        {
            Directory.CreateDirectory(Target);
            MessageBox.Show(_languageService.GetTranslation("TargetDirectoryCreated"), Target);
        }

        BaseViewModel.PauseEvent = PauseEvent;
        BaseViewModel.CancellationToken = CancellationTokenSource.Token;

        Type.Transfer(Id, Name, Source, Target);
    }

}
