using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace EasySave_2._0.Models
{
    public class BackupManager
    {
        private List<BackupJob> jobs = new List<BackupJob>();
        private int nextId = 1;  // Next ID 
        private LanguageService languageService; // Added field for LanguageService
        private static Logger _logger = new Logger();

        // Constructor to initialize LanguageService
        public BackupManager(LanguageService languageService)
        {
            this.languageService = languageService;
        }

        // Method to add a new backup job
        public void AddJob(BackupJob job)
        {
            job.Id = nextId;
            jobs.Add(job);
            nextId++;  // Increment the ID for the next job
            _logger.LogAction(job.Name, job.Source, job.Target, 0, 0, "Creation");
        }

        // Method to get a backup job by id
        public BackupJob GetJob(int id)
        {
            return jobs.FirstOrDefault(j => j.Id == id);
        }

        // Method to get all backup job
        public List<BackupJob> GetAllJobs()
        {
            return jobs;
        }

        // Method to get the next ID
        public int GetNextId()
        {
            return nextId;
        }

        // Logic to modify a backup job
        public void ModifyJob(string name, string source, string target, IBackupType type, BackupJob job)
        {
            if (job != null)
            {
                job.Name = name;
                job.Source = source;
                job.Target = target;
                job.Type = type;
            }
            _logger.LogAction(job.Name, job.Source, job.Target, 0, 0, "Modification");
        }

        // Logic to delete a backup job
        public void DeleteJob(BackupJob job)
        {
            // Verify if the job exists in the list
            if (job == null) return;

            jobs.Remove(job);
            _logger.LogAction(job.Name, job.Source, job.Target, 0, 0, "Deletion");
        }
        public void PauseJob(BackupJob job)
        {
            if (job == null) return;
            job.PauseEvent.Reset();  // Met en pause
            job.Etat = "En pause";
            MessageBox.Show("Travail en pause");
        }

        // Reprend un job
        public void PlayJob(BackupJob job)
        {
            if (job == null) return;
            job.PauseEvent.Set(); // Reprend
            job.Etat = "En cours";
            MessageBox.Show("Travail repris");
        }

        // Stop un job
        public void StopJob(BackupJob job)
        {
            if (job == null) return;
            job.CancellationTokenSource.Cancel(); // Annule l'exécution
            job.Etat = "Arrêté";
            MessageBox.Show("Travail arreté");
        }
    }
}
