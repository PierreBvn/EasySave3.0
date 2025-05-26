using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text.Json;
using System.Linq;

namespace EasySave_2._0.Models
{
    public class BackupState
    {
        public string Name { get; set; }
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public string State { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSize { get; set; }
        public int NbFilesLeftToDo { get; set; }
        public int Progression { get; set; }
    }

    public class StateService
    {
        private static bool alreadyReset = false;
        private Dictionary<int, BackupState> jobStates = new Dictionary<int, BackupState>();
        private string jsonPath = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")), "States/BackupState.json");
        public StateService()
        {
            if (!alreadyReset)
            {
                if (File.Exists(jsonPath))
                    File.Delete(jsonPath);

                File.WriteAllText(jsonPath, "[]");
                alreadyReset = true;
            }
        }
        public void StartTracking(int jobId, string name, string source, string target, int totalFiles, long totalSize)
        {
            jobStates[jobId] = new BackupState
            {
                Name = name,
                SourceFilePath = source,
                TargetFilePath = target,
                State = "Started",
                TotalFilesToCopy = totalFiles,
                TotalFilesSize = totalSize,
                NbFilesLeftToDo = totalFiles,
                Progression = 0
            };

            SaveStateToJson();
        }

        public void Update(int jobId, int nbFilesLeft)
        {
            if (jobStates.ContainsKey(jobId))
            {
                jobStates[jobId].NbFilesLeftToDo = nbFilesLeft;
                jobStates[jobId].Progression = jobStates[jobId].TotalFilesToCopy > 0
                    ? 100 * (jobStates[jobId].TotalFilesToCopy - nbFilesLeft) / jobStates[jobId].TotalFilesToCopy
                    : 100;
                jobStates[jobId].State = "ACTIVE";

                SaveStateToJson();
            }
        }

        public void Finish(int jobId)
        {
            if (jobStates.ContainsKey(jobId))
            {
                jobStates[jobId].State = "Finished";
                jobStates[jobId].NbFilesLeftToDo = 0;
                jobStates[jobId].Progression = 100;

                SaveStateToJson();
            }
        }

        public BackupState? GetBackupStateByName(string jobName)
        {
            if (!File.Exists(jsonPath))
                return null;

            string json = File.ReadAllText(jsonPath);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var states = JsonSerializer.Deserialize<List<BackupState>>(json);
            if (states == null)
                return null;

            // Research for the state with the specified name
            return states.FirstOrDefault(s => s.Name == jobName);
        }

        private void SaveStateToJson()
        {
            Dictionary<int, BackupState> backupStates = new Dictionary<int, BackupState>();

            // Read existing states from the JSON file if it exists
            if (File.Exists(jsonPath))
            {
                string existingJson = File.ReadAllText(jsonPath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    var existingStates = JsonSerializer.Deserialize<List<BackupState>>(existingJson);
                    if (existingStates != null)
                    {
                        foreach (var state in existingStates)
                        {
                            //Stock state by hash code to avoid duplicates
                            backupStates[state.Name.GetHashCode()] = state;
                        }
                    }
                }
            }
            // Update the states with the current job states
            foreach (var job in jobStates.Values)
            {
                backupStates[job.Name.GetHashCode()] = job;
            }
            // Write the updated states to the JSON file
            string json = JsonSerializer.Serialize(backupStates.Values, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonPath, json);
        }
        public void Cancel(int jobId)
        {
            if (jobStates.ContainsKey(jobId))
            {
                jobStates[jobId].State = "Cancel";

                SaveStateToJson();
            }
        }
    }
}

