using System;
using System.IO;

namespace MediaCatalog.Core.Services
{
    public static class Logger
    {
        private static readonly string LogFile = "app.log";

        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            try
            {
                File.AppendAllText(LogFile, logEntry + Environment.NewLine);
            }
            catch { /* Игнорируем ошибки логгера, чтобы не ронять приложение */ }
        }
    }
}