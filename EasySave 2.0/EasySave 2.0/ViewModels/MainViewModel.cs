using EasySave_2._0.Models;
using EasySave_2._0.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySave_2._0.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<BackupJob> BackupJobs { get; set; }
        public ObservableCollection<BackupJob> SelectedBackupJobs { get; set; } = new();
        public ObservableCollection<LogFormat> LogFormats { get; } = new((LogFormat[])Enum.GetValues(typeof(LogFormat)));

        public BackupJob SelectedJob { get; set; }

        // Liste des extensions disponibles
        public ObservableCollection<BaseViewModel> AvailableExtensions { get; set; } = new()
        {
            new BaseViewModel { FILEFormat = ".txt" },
            new BaseViewModel { FILEFormat = ".docx" },
            new BaseViewModel { FILEFormat = ".pdf" },
            new BaseViewModel { FILEFormat = ".png" },
            new BaseViewModel { FILEFormat = ".jpg" }
        };

        public ObservableCollection<string> EncryptionExtensions { get; } = new() { ".txt", ".docx", ".pdf", ".png", ".jpg" };

        public string SelectedExtensionToEncrypt { get; set; } = ".txt";

        private readonly BackupManager _backupManager;
        private readonly Logger _logger;
        private readonly LanguageService _languageService;
        private readonly StateService _stateService;

        private string _backupName;
        private string _sourcePath;
        private string _targetPath;

        public string BackupName
        {
            get => _backupName;
            set { _backupName = value; OnPropertyChanged(nameof(BackupName)); }
        }

        public string SourcePath
        {
            get => _sourcePath;
            set { _sourcePath = value; OnPropertyChanged(nameof(SourcePath)); }
        }

        public string TargetPath
        {
            get => _targetPath;
            set { _targetPath = value; OnPropertyChanged(nameof(TargetPath)); }
        }

        public string BackupJobsList { get; set; }
        public string Name { get; set; }

        // Traductions UI
        public string TextCreateBackup => _languageService.GetTranslation("CreateBackup");
        public string TextModifyBackup => _languageService.GetTranslation("ModifyBackup");
        public string TextDeleteBackup => _languageService.GetTranslation("DeleteBackup");
        public string TextListBackups => _languageService.GetTranslation("ListBackups");
        public string TextExecuteBackup => _languageService.GetTranslation("ExecuteBackup");
        public string TextLanguageChoice => _languageService.GetTranslation("LanguageChoice");
        public string TextSettings => _languageService.GetTranslation("Settings");
        public string TextQuit => _languageService.GetTranslation("Quit");
        public string TextTitleCreateBackup => _languageService.GetTranslation("TitleCreateBackup");
        public string TextEnterBackupJobName => _languageService.GetTranslation("EnterBackupJobName");
        public string TextEnterSourceDirectory => _languageService.GetTranslation("EnterSourceDirectory");
        public string TextEnterTargetDirectory => _languageService.GetTranslation("EnterTargetDirectory");
        public string TextEnterBackupType => _languageService.GetTranslation("EnterBackupType");
        public string ButtonCreateBackup => _languageService.GetTranslation("ButtonCreateBackup");
        public string TextTitleSelectEditBackup => _languageService.GetTranslation("TitleSelectEditBackup");
        public string ButtonEditSelectBackup => _languageService.GetTranslation("ButtonEditSelectBackup");
        public string TextTitleEditBackup => _languageService.GetTranslation("TitleEditBackup");
        public string ButtonEditBackup => _languageService.GetTranslation("ButtonEditBackup");
        public string TextTitleDeleteBackup => _languageService.GetTranslation("TitleDeleteBackup");
        public string ButtonDeleteBackup => _languageService.GetTranslation("ButtonDeleteBackup");
        public string TextTitleListBackup => _languageService.GetTranslation("TitleListBackup");
        public string TextTitleExecuteBackup => _languageService.GetTranslation("TitleExecuteBackup");
        public string ButtonExecuteBackup => _languageService.GetTranslation("ButtonExecuteBackup");
        public string TextTitleChoiceLanguage => _languageService.GetTranslation("TitleChoiceLanguage");
        public string ButtonChangeLanguage => _languageService.GetTranslation("ButtonChangeLanguage");
        public string ButtonSelectSettings => _languageService.GetTranslation("ButtonSelectSettings");

        // Commandes
        public ICommand ShowCreateBackupCommand { get; }
        public ICommand ShowListBackupJobsCommand { get; }
        public ICommand ShowDeleteBackupCommand { get; }
        public ICommand ShowExecuteBackupCommand { get; }
        public ICommand ShowEditBackupCommand { get; }
        public ICommand ShowSelectLangageCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand ListBackupJobsCommand { get; }
        public ICommand ExecuteSelectedJobsCommand { get; }
        public ICommand DeleteSelectedJobsCommand { get; }
        public ICommand EditBackupJobCommand { get; }
        public ICommand EditSelectedJobCommand { get; }
        public ICommand SelectLangageCommand { get; }
        public ICommand QuitCommand { get; }
        public ICommand ValidSettings { get; }
        public ICommand PlayJobCommand { get; }
        public ICommand PauseJobCommand { get; }
        public ICommand StopJobCommand { get; }

        public MainViewModel()
        {
            _languageService = new LanguageService();
            _languageService.SetLanguage("fr");

            _stateService = new StateService();
            _backupManager = new BackupManager(_languageService);
            _logger = new Logger();

            ProcessDetectorSaved = "Ex : CalculatorApp";

            BackupJobs = new ObservableCollection<BackupJob>(_backupManager.GetAllJobs());

            ShowCreateBackupCommand = new RelayCommand(() => { ClearFields(); SetMode(create: true); });
            ShowListBackupJobsCommand = new RelayCommand(() => { ListBackupJobs(); SetMode(list: true); });
            ShowExecuteBackupCommand = new RelayCommand(() => SetMode(execute: true));
            ShowDeleteBackupCommand = new RelayCommand(() => SetMode(delete: true));
            ShowEditBackupCommand = new RelayCommand(() => SetMode(edit: true));
            ShowSelectLangageCommand = new RelayCommand(() => SetMode(langage: true));
            ShowSettingsCommand = new RelayCommand(() => SetMode(settings: true));
            EditSelectedJobCommand = new RelayCommand(() => { ShowEditBackup(); SetMode(editJob: true); });
            EditBackupJobCommand = new RelayCommand(() => EditBackup());
            CreateBackupCommand = new RelayCommand(() => CreateBackup());
            ExecuteSelectedJobsCommand = new RelayCommand(() => ExecuteSelectedJobs());
            DeleteSelectedJobsCommand = new RelayCommand(() => DeleteSelectedJobs());
            SelectLangageCommand = new RelayCommand(() => ChangeLanguage());
            QuitCommand = new RelayCommand(() => QuitApplication());
            ValidSettings = new RelayCommand(() => SaveSettings());
            PlayJobCommand = new RelayCommand<BackupJob>(job => _backupManager.PlayJob(job));
            PauseJobCommand = new RelayCommand<BackupJob>(job => _backupManager.PauseJob(job));
            StopJobCommand = new RelayCommand<BackupJob>(job => _backupManager.StopJob(job));

        }

        private void SetMode(bool create = false, bool list = false, bool execute = false, bool delete = false,
                             bool editJob = false, bool edit = false, bool langage = false, bool settings = false)
        {
            IsInCreateMode = create;
            IsInListMode = list;
            IsInExecuteMode = execute;
            IsInDeleteMode = delete;
            IsInEditJobMode = editJob;
            IsInEditMode = edit;
            IsInLangageMode = langage;
            IsInSettingsMode = settings;
        }

        private void ClearFields()
        {
            BackupName = string.Empty;
            SourcePath = string.Empty;
            TargetPath = string.Empty;
            BackupTypeChoice = 0;
        }

        private void CreateBackup()
        {
            if (string.IsNullOrWhiteSpace(BackupName) ||
                string.IsNullOrWhiteSpace(SourcePath) ||
                string.IsNullOrWhiteSpace(TargetPath))
            {
                MessageBox.Show(_languageService.GetTranslation("FillAllFields"));
                return;
            }

            IBackupType backupType = BackupTypeChoice == 0
                ? new FullBackup(_languageService)
                : new DifferentialBackup(_languageService);

            BackupJob.SetLanguageService(_languageService);

            var job = new BackupJob
            {
                Name = BackupName,
                Source = SourcePath,
                Target = TargetPath,
                Type = backupType
            };

            _backupManager.AddJob(job);
            BackupJobs.Add(job);

            MessageBox.Show(_languageService.GetTranslation("BackupJobCreatedSuccess"));

            ClearFields();
        }

        private void EditBackup()
        {
            if (SelectedJob == null) return;

            if (string.IsNullOrWhiteSpace(BackupName) ||
                string.IsNullOrWhiteSpace(SourcePath) ||
                string.IsNullOrWhiteSpace(TargetPath))
            {
                MessageBox.Show(_languageService.GetTranslation("FillAllFields"));
                return;
            }

            IBackupType newType = BackupTypeChoice == 0
                ? new FullBackup(_languageService)
                : new DifferentialBackup(_languageService);

            // Remplacement direct dans la job
            SelectedJob.Name = BackupName;
            SelectedJob.Source = SourcePath;
            SelectedJob.Target = TargetPath;
            SelectedJob.Type = newType;

            MessageBox.Show(_languageService.GetTranslation("EditSave"));

            IsInEditJobMode = false;
            OnPropertyChanged(nameof(IsInEditJobMode));
            IsInEditMode = true;
            OnPropertyChanged(nameof(IsInEditMode));
        }

        private void DeleteSelectedJobs()
        {
            var selectedJobs = BackupJobs.Where(j => j.IsSelected).ToList();

            if (!selectedJobs.Any())
            {
                MessageBox.Show(_languageService.GetTranslation("UnselectedBackup"));
                return;
            }

            foreach (var job in selectedJobs)
            {
                _backupManager.DeleteJob(job);
                BackupJobs.Remove(job);
            }

            MessageBox.Show(_languageService.GetTranslation("JobDeleted"));
        }

        private async void ExecuteSelectedJobs()
        {
            var selectedJobs = BackupJobs.Where(j => j.IsSelected).ToList();

            if (!selectedJobs.Any())
            {
                MessageBox.Show(_languageService.GetTranslation("UnselectedBackup"));
                return;
            }

            if (!string.IsNullOrEmpty(ProcessDetector))
            {
                if (Process.GetProcessesByName(ProcessDetector).Length > 0)
                {
                    MessageBox.Show(_languageService.GetTranslation("ProcessDetected") + ProcessDetector);
                    return;
                }
            }

            foreach (var job in selectedJobs)
            {
                var localJob = job; // Pour closure
                _ = Task.Run(() =>
                {
                    try
                    {
                        localJob.Etat = "En cours";
                        OnPropertyChanged(nameof(BackupJobs)); // Pour mise à jour UI
                        localJob.Execute(); // Ton execute lit PauseEvent et Token
                        localJob.Etat = "Terminé";
                    }
                    catch
                    {
                        localJob.Etat = "Erreur";
                    }
                });
            }
        }

        private void ShowEditBackup()
        {
            if (SelectedJob == null)
            {
                MessageBox.Show(_languageService.GetTranslation("UnselectedBackup"));
                return;
            }

            BackupName = SelectedJob.Name;
            SourcePath = SelectedJob.Source;
            TargetPath = SelectedJob.Target;
            BackupTypeChoice = SelectedJob.Type is FullBackup ? 0 : 1;

            IsInEditJobMode = true;
            OnPropertyChanged(nameof(IsInEditJobMode));
        }

        private void ListBackupJobs()
        {
            BackupJobs.Clear();
            var jobs = _backupManager.GetAllJobs();
            if (jobs.Count > 0)
            {
                foreach (var job in jobs)
                {
                    var state = _stateService.GetBackupStateByName(job.Name);
                    job.Etat = state != null ? state.State : _languageService.GetTranslation("StateUnknown");
                    BackupJobs.Add(job);
                }
            }
            else
            {
                BackupJobs.Clear();
                BackupJobs.Add(new BackupJob { Name = _languageService.GetTranslation("NoBackupJobsFound"), Etat = ""});
            }
        }

        private void SaveSettings()
        {
            var selectedExtensions = AvailableExtensions
                .Where(e => e.IsSelected)
                .Select(e => e.FILEFormat.ToLower())
                .ToList();

            LogFormat format = SelectedLogFormat;
            Logger.Instance.SetLogFormat(format);
            CryptoSoft.XORFileEncryptor.ExtensionsToEncrypt = selectedExtensions;

            ProcessDetector = ProcessDetectorSaved;

            string extensions = string.Join(", ", selectedExtensions);
            MessageBox.Show($"Format de logs choisi :   {SelectedLogFormat}" +
                $"\nFichiers cryptés :  {extensions}" +
                $"\nExclusion enregistrée : {ProcessDetector}");
        }

        private void ChangeLanguage()
        {
            switch (SelectedLanguageIndex)
            {
                case 0: _languageService.SetLanguage("fr"); break;
                case 1: _languageService.SetLanguage("en"); break;
                default: _languageService.SetLanguage("fr"); break;
            }

            RefreshTranslations();
        }

        public void RefreshTranslations()
        {
            OnPropertyChanged(nameof(TextCreateBackup));
            OnPropertyChanged(nameof(TextModifyBackup));
            OnPropertyChanged(nameof(TextDeleteBackup));
            OnPropertyChanged(nameof(TextListBackups));
            OnPropertyChanged(nameof(TextExecuteBackup));
            OnPropertyChanged(nameof(TextLanguageChoice));
            OnPropertyChanged(nameof(TextQuit));
            OnPropertyChanged(nameof(TextTitleCreateBackup));
            OnPropertyChanged(nameof(TextEnterBackupJobName));
            OnPropertyChanged(nameof(TextEnterSourceDirectory));
            OnPropertyChanged(nameof(TextEnterTargetDirectory));
            OnPropertyChanged(nameof(TextEnterBackupType));
            OnPropertyChanged(nameof(ButtonCreateBackup));
            OnPropertyChanged(nameof(TextTitleSelectEditBackup));
            OnPropertyChanged(nameof(ButtonEditSelectBackup));
            OnPropertyChanged(nameof(TextTitleEditBackup));
            OnPropertyChanged(nameof(ButtonEditBackup));
            OnPropertyChanged(nameof(TextTitleListBackup));
            OnPropertyChanged(nameof(TextTitleDeleteBackup));
            OnPropertyChanged(nameof(ButtonDeleteBackup));
            OnPropertyChanged(nameof(TextTitleExecuteBackup));
            OnPropertyChanged(nameof(ButtonExecuteBackup));
            OnPropertyChanged(nameof(TextTitleChoiceLanguage));
            OnPropertyChanged(nameof(ButtonChangeLanguage));
            OnPropertyChanged(nameof(ButtonSelectSettings));
        }

        private void QuitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
