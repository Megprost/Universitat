using System.Text;
using Microsoft.Extensions.Logging;

namespace ExpertTracker.Services;

// One provider and one lock are shared by all logger categories.
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _path;
    private readonly object _sync = new();

    public FileLoggerProvider(string logFilePath) => _path = logFilePath;

    public ILogger CreateLogger(string categoryName) =>
        new FileLogger(_path, categoryName, _sync);

    public void Dispose() { }

    private sealed class FileLogger : ILogger
    {
        private readonly string _path;
        private readonly string _category;
        private readonly object _sync;

        public FileLogger(string path, string category, object sync)
        {
            _path = path;
            _category = category;
            _sync = sync;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
            EmptyScope.Instance;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel >= LogLevel.Information && logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            string line = $"{DateTimeOffset.Now:O} [{logLevel}] {_category}: " +
                          formatter(state, exception);
            if (exception != null)
                line += Environment.NewLine + exception;

            try
            {
                lock (_sync)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                    File.AppendAllText(_path, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (IOException)
            {
                System.Diagnostics.Debug.WriteLine("[Lab04] Could not write log file.");
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine("[Lab04] Access to log file denied.");
            }
        }

        private sealed class EmptyScope : IDisposable
        {
            public static EmptyScope Instance { get; } = new();
            public void Dispose() { }
        }
    }
}
