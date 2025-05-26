using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json;
using System.Windows;

public enum LogFormat
{
    JSON,
    XML
}

public class LogEntry
{
    public string timestamp { get; set; }
    public string backup_name { get; set; }
    public string source_file { get; set; }
    public string target_file { get; set; }
    public long file_size { get; set; }
    public long transfer_time_ms { get; set; }
    public string Action { get; set; }
}
public class Logger
{
    private string logDirectory; //Log directory path
    private string logFilePath;  // Log file path
    private static LogFormat _logFormat = LogFormat.JSON;
    private string currentDate;  // Actual date format "yyyy-MM-dd"
    private LogFormat logFormat = LogFormat.JSON; // Default log format  
    private static Logger  _instance; //Unique instance of Logger
    public static Logger Instance => _instance ??= new Logger();

    public Logger()
    {
        // Logs directory path
        logDirectory = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")), "Logs");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Get the current date in "yyyy-MM-dd" format
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    public void SetLogFormat(LogFormat format)
    {
        _logFormat = format; // Update the static log format
        this.logFormat = _logFormat; // Sync the instance log format with the static one
        //logFilePath = Path.Combine(logDirectory, $"Log_{currentDate}.{(logFormat == LogFormat.JSON ? "json" : "xml")}");
    }

    // Method to log actions
    public void LogAction(string backupName, string sourceFile, string targetFile, long fileSize, long transferTimeMs, string action)
    {
        var logEntry = new LogEntry
        {
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            backup_name = backupName,
            source_file = sourceFile,
            target_file = targetFile,
            file_size = fileSize,
            transfer_time_ms = transferTimeMs,
            Action = action,
        };

        List<LogEntry> logList = new();

        // Defnies the log file path
        logFilePath = Path.Combine(logDirectory, $"Log_{currentDate}.{(_logFormat == LogFormat.JSON ? "json" : "xml")}");
        // VVerify if the log file exists and read its content
        if (File.Exists(logFilePath))
        {
            string existing = File.ReadAllText(logFilePath);

            if (logFilePath.EndsWith(".json"))
            {
                logList = JsonSerializer.Deserialize<List<LogEntry>>(existing) ?? new List<LogEntry>();
            }
            else if (logFilePath.EndsWith(".xml"))
            {
                var serializer = new XmlSerializer(typeof(List<LogEntry>));
                using var reader = new StringReader(existing);
                logList = (List<LogEntry>)serializer.Deserialize(reader);
            }
        }

        logList.Add(logEntry);

        // Write the log entry to the file in the specified format
        if (_logFormat == LogFormat.JSON)
        {
            File.WriteAllText(logFilePath, JsonSerializer.Serialize(logList, new JsonSerializerOptions { WriteIndented = true }));
        }
        else if (_logFormat == LogFormat.XML)
        {
            var serializer = new XmlSerializer(typeof(List<LogEntry>));
            using var writer = new StringWriter();
            serializer.Serialize(writer, logList);
            File.WriteAllText(logFilePath, writer.ToString());
        }
    }



}