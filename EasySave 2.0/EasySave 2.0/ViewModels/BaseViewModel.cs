using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasySave_2._0.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //  Propriété statique pour l'exclusion de fichier (partagée globalement)
        public static string ProcessDetector { get; set; } = string.Empty;

        private string _processDetectorSaved;
        public string ProcessDetectorSaved
        {
            get => _processDetectorSaved;
            set
            {
                _processDetectorSaved = value;
                OnPropertyChanged(nameof(ProcessDetectorSaved));
            }
        }

        // États de navigation de l'application
        private bool _isInCreateMode = true;
        public bool IsInCreateMode
        {
            get => _isInCreateMode;
            set { _isInCreateMode = value; OnPropertyChanged(nameof(IsInCreateMode)); }
        }

        private bool _isInListMode;
        public bool IsInListMode
        {
            get => _isInListMode;
            set { _isInListMode = value; OnPropertyChanged(nameof(IsInListMode)); }
        }

        private bool _isInExecuteMode;
        public bool IsInExecuteMode
        {
            get => _isInExecuteMode;
            set { _isInExecuteMode = value; OnPropertyChanged(nameof(IsInExecuteMode)); }
        }

        private bool _isInDeleteMode;
        public bool IsInDeleteMode
        {
            get => _isInDeleteMode;
            set { _isInDeleteMode = value; OnPropertyChanged(nameof(IsInDeleteMode)); }
        }

        private bool _isInEditJobMode;
        public bool IsInEditJobMode
        {
            get => _isInEditJobMode;
            set { _isInEditJobMode = value; OnPropertyChanged(nameof(IsInEditJobMode)); }
        }

        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set { _isInEditMode = value; OnPropertyChanged(nameof(IsInEditMode)); }
        }

        private bool _isInLangageMode;
        public bool IsInLangageMode
        {
            get => _isInLangageMode;
            set { _isInLangageMode = value; OnPropertyChanged(nameof(IsInLangageMode)); }
        }

        private bool _isInSettingsMode;
        public bool IsInSettingsMode
        {
            get => _isInSettingsMode;
            set { _isInSettingsMode = value; OnPropertyChanged(nameof(IsInSettingsMode)); }
        }

        // Paramètres de configuration
        private LogFormat _selectedLogFormat;
        public LogFormat SelectedLogFormat
        {
            get => _selectedLogFormat;
            set { _selectedLogFormat = value; OnPropertyChanged(nameof(SelectedLogFormat)); }
        }

        private int _selectedLanguageIndex;
        public int SelectedLanguageIndex
        {
            get => _selectedLanguageIndex;
            set { _selectedLanguageIndex = value; OnPropertyChanged(nameof(SelectedLanguageIndex)); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        // Utilisé pour les extensions sélectionnées dans la liste
        public bool IsSelected { get; set; }
        public string FILEFormat { get; set; }

        // Types de sauvegarde disponibles
        public List<string> BackupTypeOptions { get; } = new() { "Complet", "Différentiel" };

        private int _backupTypeChoice = 0;
        public int BackupTypeChoice
        {
            get => _backupTypeChoice;
            set { _backupTypeChoice = value; OnPropertyChanged(nameof(BackupTypeChoice)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
