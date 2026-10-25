using Microsoft.Extensions.Logging;
using System.Text;
namespace ExpertTracker.Services;
public sealed class FileLoggerProvider : ILoggerProvider {
 private readonly string path;
 private readonly object gate=new();
 public FileLoggerProvider(string logFilePath) { path=logFilePath; }
 public ILogger CreateLogger(string categoryName)=>new FileLogger(path,categoryName,gate);
 public void Dispose() {}
 private sealed class FileLogger : ILogger {
  private readonly string path,category;private readonly object gate;
  public FileLogger(string p,string c,object l){path=p;category=c;gate=l;}
  public IDisposable BeginScope<TState>(TState state) where TState:notnull=>Noop.Instance;
  public bool IsEnabled(LogLevel level)=>level>=LogLevel.Information;
  public void Log<TState>(LogLevel level,EventId eventId,TState state,
    Exception? exception,Func<TState,Exception?,string> formatter) {
   if(!IsEnabled(level))return;
   string line=$"{DateTimeOffset.Now:O} [{level}] {category}: {formatter(state,exception)}";
   if(exception is not null)line+=Environment.NewLine+exception;
   try {lock(gate){File.AppendAllText(path,line+Environment.NewLine,Encoding.UTF8);}}
   catch { System.Diagnostics.Debug.WriteLine("[FileLogger] Unable to write log"); }
  }
  private sealed class Noop:IDisposable {public static readonly Noop Instance=new();public void Dispose(){} }
 }
}
