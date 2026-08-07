using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace TTech.IP_Switcher.Helpers.Logging;

internal sealed class AppLogger : IAppLogger
{
    private static readonly object SyncRoot = new();
    private static readonly Lazy<AppLogger> LazyInstance = new(() => new AppLogger());

    private readonly string logFilePath;

    private AppLogger()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
        var logDirectory = Path.Combine(appDataPath, "TTech", "IP switcher", "Logs");
        logFilePath = Path.Combine(logDirectory, "application.log");
    }

    internal static IAppLogger Instance => LazyInstance.Value;

    public void LogInfo(string context, string message)
    {
        WriteLine("INFO", context, message, null);
    }

    public void LogError(string context, string message, Exception exception = null)
    {
        WriteLine("ERROR", context, message, exception);
    }

    private void WriteLine(string level, string context, string message, Exception exception)
    {
        try
        {
            var directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var builder = new StringBuilder();
            builder.Append(DateTimeOffset.Now.ToString("O", CultureInfo.InvariantCulture));
            builder.Append(" [");
            builder.Append(level);
            builder.Append("] ");
            builder.Append(string.IsNullOrWhiteSpace(context) ? "General" : context);
            builder.Append(": ");
            builder.AppendLine(message ?? string.Empty);

            if (exception != null)
            {
                builder.AppendLine(exception.ToString());
            }

            lock (SyncRoot)
            {
                File.AppendAllText(logFilePath, builder.ToString());
            }
        }
        catch
        {
            // Never let logging impact UI stability.
        }
    }
}
