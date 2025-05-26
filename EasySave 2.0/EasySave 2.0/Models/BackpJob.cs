using EasySave_2._0.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using EasySave_2._0.ViewModels;

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

        Type.Transfer(Id, Name, Source, Target);
    }

}
