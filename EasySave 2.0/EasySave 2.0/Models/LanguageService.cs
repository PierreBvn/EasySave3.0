using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System.Windows.Media;

namespace EasySave_2._0.Models;

public class LanguageService
{
    private string currentLanguage = "fr"; // Default language is French
    private Dictionary<string, string> menuTextFr = new Dictionary<string, string>
{
    //Menu
    { "MenuTitle", "EasySave Backup" },
    { "CreateBackup", "Créer un travail" },
    { "ExecuteBackup", "Exécuter un travail" },
    { "ListBackups", "Lister tous les travaux" },
    { "Quit", "Quitter" },
    { "ModifyBackup", "Modifier un travail" },
    { "DeleteBackup", "Supprimer un travail" },
    { "LanguageChoice", "Changer de langue" },
    { "Settings", "Parametres Generaux" },


    //Create
    { "TitleCreateBackup", "Créer un nouveau travail de sauvegarde :" },
    { "EnterBackupJobName", "Nom du travail :" },
    { "EnterSourceDirectory", "Répertoire source :" },
    { "EnterTargetDirectory", "Répertoire cible :" },
    { "EnterBackupType", "Type de sauvegarde :" },
    { "ButtonCreateBackup", "Créer la sauvegarde"},
    { "BackupJobCreatedSuccess", "Travail de sauvegarde créé avec succès !" },

    //Edit
    { "TitleSelectEditBackup", "Selectionner le travail à modifier :" },
    { "ButtonEditSelectBackup", "Modifier ce travail" },
    { "TitleEditBackup", "Modifier les champs de ce travail :" },
    { "ButtonEditBackup", "Enregistrer les modifications" },
    { "EditSave", "Modifications enregistées"},

    //Delete
    { "TitleDeleteBackup", "Choisir le ou les travaux à supprimer :" },
    { "ButtonDeleteBackup", "Supprimer ce ou ces travaux" },
    { "JobDeleted", "Travail supprimé avec succès" },

    //List
    { "TitleListBackup", "Liste des travaux de sauvegarde :" },
    { "NoBackupJobsFound", "Aucun travail trouvé !"},
    { "StateUnknown", "Etat inconnu"},
    
    //Execute
    { "TitleExecuteBackup", "Selectionner le ou les travaux à executer :" },
    { "ButtonExecuteBackup", "Exécuter ce ou ces travaux" },
    { "SourceFolderNotFound", "Le dossier source '{0}' n'existe pas !" },
    { "BackupJobExecutedSuccess", "Travaux executés avec succès !" },
    { "ProcessDetected", "Le logiciel métier est en cours d'execution, veuillez le fermer pour continuer : " },

    //Language
    { "TitleChoiceLanguage", "Selectionner une langue :" },
    { "ButtonChangeLanguage", "Changer de langue" },

    //Settings
    { "SelectedFormat", "Le format choisi est  : " },
    { "ButtonSelectSettings", "Enregistrer les Parametres " },

    //error
    { "FillAllFields", "Veuillez remplir tous les champs !"},
    { "UnselectedBackup", "Veuillez selectionner au moins un travail !" },
    { "SourceNotFound", "Le fichier ou dossier source n'a pas été trouvé : " },
    { "UnexpectedError", "Erreur inattendue : " },
    { "TargetDirectoryCreated", "Dossier cible créé car inexistant : "}
};

    private Dictionary<string, string> menuTextEn = new Dictionary<string, string>
{
    //Menu
    { "MenuTitle", "EasySave Backup" },
    { "CreateBackup", "Create a job" },
    { "ExecuteBackup", "Execute a job" },
    { "ListBackups", "List all jobs" },
    { "Quit", "Quit" },
    { "ModifyBackup", "Edit a job" },
    { "DeleteBackup", "Delete a job" },
    { "LanguageChoice", "Change language" },
    { "Settings", "General settings" },


    //Create
    { "TitleCreateBackup", "Create a new backup job:" },
    { "EnterBackupJobName", "Job name:" },
    { "EnterSourceDirectory", "Source directory:" },
    { "EnterTargetDirectory", "Target directory:" },
    { "EnterBackupType", "Backup type:" },
    { "ButtonCreateBackup", "Create backup" },
    { "BackupJobCreatedSuccess", "Backup job created successfully!" },

    //Edit
    { "TitleSelectEditBackup", "Select the job to edit:" },
    { "ButtonEditSelectBackup", "Edit this job" },
    { "TitleEditBackup", "Edit this job's fields:" },
    { "ButtonEditBackup", "Save changes" },
    { "EditSave", "Changes saved" },

    //Delete
    { "TitleDeleteBackup", "Select job(s) to delete:" },
    { "ButtonDeleteBackup", "Delete selected job(s)" },
    { "JobDeleted", "Job deleted successfully" },

    //List
    { "TitleListBackup", "List of backup jobs:" },
    { "NoBackupJobsFound", "No jobs found!" },
    { "StateUnknown", "Unknown state" },

    //Execute
    { "TitleExecuteBackup", "Select job(s) to execute:" },
    { "ButtonExecuteBackup", "Execute selected job(s)" },
    { "SourceFolderNotFound", "The source folder '{0}' does not exist!" },
    { "BackupJobExecutedSuccess", "Jobs executed successfully!" },
    { "ProcessDetected", "The business software is running, please close it to continue: " },

    //Language
    { "TitleChoiceLanguage", "Select a language:" },
    { "ButtonChangeLanguage", "Change language" },

    //Settings
    { "SelectedFormat", "Selected Format : " },
    { "ButtonSelectSettings", "Save Parameters " },

    //Error
    { "FillAllFields", "Please fill in all fields!" },
    { "UnselectedBackup", "Please select at least one job!" },
    { "SourceNotFound", "Source file or folder not found: " },
    { "UnexpectedError", "Unexpected error: " },
    { "TargetDirectoryCreated", "Target folder created as it did not exist: " },

};

    // Change language method
    public void SetLanguage(string lang)
    {
        currentLanguage = lang.ToLower() == "en" ? "en" : "fr"; // Support "fr" and "en"
    }

    // Get translation method
    public string GetTranslation(string key)
    {
        if (currentLanguage == "en")
        {
            return menuTextEn.ContainsKey(key) ? menuTextEn[key] : key;
        }
        return menuTextFr.ContainsKey(key) ? menuTextFr[key] : key;
    }
}
