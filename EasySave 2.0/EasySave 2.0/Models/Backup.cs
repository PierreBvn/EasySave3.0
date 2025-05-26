using CryptoSoft;
using EasySave_2._0.Models;
using EasySave_2._0.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

public class FullBackup : IBackupType
{
    private readonly StateService stateService = new();
    private readonly LanguageService _languageService;
    private static readonly Logger _logger = new();

    public FullBackup(LanguageService languageService)
    {
        _languageService = languageService;
    }

    public void Transfer(int id, string name, string source, string target)
    {
        
        MessageBox.Show(_languageService.GetTranslation("BackupJobExecutedSuccess"));

        string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);

        int totalFiles = files.Length;
        int nbFilesLeft = totalFiles;
        long totalSize = files.Sum(f => new FileInfo(f).Length);

        stateService.StartTracking(id, name, source, target, totalFiles, totalSize);

        foreach (string file in files)
        {
            BaseViewModel.PauseEvent.Wait();

            if (BaseViewModel.CancellationToken.IsCancellationRequested)
            {
                stateService.Cancel(id);
                return;
            }

            string destFile = Path.Combine(target, Path.GetFileName(file));
            Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);

            Stopwatch sw = Stopwatch.StartNew();

            if (XORFileEncryptor.ShouldEncrypt(file))
                XORFileEncryptor.EncryptDecrypt(file, destFile);
            else
                File.Copy(file, destFile, true);

            sw.Stop();
            long transferTimeMs = sw.ElapsedMilliseconds;

            _logger.LogAction(name, file, destFile, new FileInfo(file).Length, transferTimeMs, "Execution");

            nbFilesLeft--;
            int progression = totalFiles > 0 ? (100 * (totalFiles - nbFilesLeft) / totalFiles) : 100;

            stateService.Update(id, nbFilesLeft);
            Thread.Sleep(2000);
        }

        if (!BaseViewModel.CancellationToken.IsCancellationRequested)
        {
            stateService.Finish(id);
        }
    }
}


public class DifferentialBackup : IBackupType
{
    private readonly StateService stateService = new();
    private readonly LanguageService _languageService;
    private static readonly Logger _logger = new();

    public DifferentialBackup(LanguageService languageService)
    {
        _languageService = languageService;
    }

    public void Transfer(int id, string name, string source, string target)
    {
        MessageBox.Show(_languageService.GetTranslation("BackupJobExecutedSuccess"));

        string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);

        int totalFiles = files.Length;
        int nbFilesLeft = totalFiles;
        long totalSize = files.Sum(f => new FileInfo(f).Length);

        stateService.StartTracking(id, name, source, target, totalFiles, totalSize);

        foreach (string file in files)
        {
            BaseViewModel.PauseEvent.Wait();

            if (BaseViewModel.CancellationToken.IsCancellationRequested)
            {
                stateService.Cancel(id);
                return;
            }

            string destFile = Path.Combine(target, Path.GetFileName(file));

            if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);

                Stopwatch sw = Stopwatch.StartNew();

                if (XORFileEncryptor.ShouldEncrypt(file))
                    XORFileEncryptor.EncryptDecrypt(file, destFile);
                else
                    File.Copy(file, destFile, true);

                sw.Stop();
                long transferTimeMs = sw.ElapsedMilliseconds;

                _logger.LogAction(name, file, destFile, new FileInfo(file).Length, transferTimeMs, "Execution");
            }

            nbFilesLeft--;
            int progression = totalFiles > 0 ? (100 * (totalFiles - nbFilesLeft) / totalFiles) : 100;
            stateService.Update(id, nbFilesLeft);

            Thread.Sleep(2000);
        }

        if (!BaseViewModel.CancellationToken.IsCancellationRequested)
        {
            stateService.Finish(id);
        }
    }
}
