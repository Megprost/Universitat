using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using ExpertTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace ExpertTracker;
public static class MauiProgram {
 public static MauiApp CreateMauiApp() {
  var b=MauiApp.CreateBuilder();
  b.UseMauiApp<App>().ConfigureFonts(f=>{
   f.AddFont("OpenSans-Regular.ttf","OpenSansRegular");
   f.AddFont("OpenSans-Semibold.ttf","OpenSansSemibold");
  });
  string path=Path.Combine(FileSystem.AppDataDirectory,"expenses.db3");
  string logPath=Path.Combine(FileSystem.AppDataDirectory,"experttracker.log");
  b.Logging.AddProvider(new FileLoggerProvider(logPath));
  b.Services.AddSingleton<IExpenseRepository>(sp=>new DatabaseService(path,sp.GetRequiredService<ILogger<DatabaseService>>()));
  b.Services.AddSingleton<IDraftStore, PreferencesDraftStore>();
  b.Services.AddSingleton<IAuthService>(sp=>new AuthService(path,sp.GetRequiredService<ILogger<AuthService>>()));
  b.Services.AddSingleton<IDeviceService,DeviceService>();
  b.Services.AddSingleton<LoginViewModel>();
  b.Services.AddSingleton<LoginPage>();
  b.Services.AddSingleton<ExpensesViewModel>();
  b.Services.AddSingleton<ExpensesPage>();
  b.Services.AddSingleton<AppShell>();
  b.Services.AddTransient<ExpenseDetailsViewModel>();
  b.Services.AddTransient<ExpenseDetailsPage>();
#if DEBUG
  b.Logging.AddDebug();
#endif
  var app=b.Build();
  var critical=app.Services.GetRequiredService<ILogger<App>>();
  AppDomain.CurrentDomain.UnhandledException+=(_,e)=>critical.LogCritical(e.ExceptionObject as Exception,"Unhandled application error");
  TaskScheduler.UnobservedTaskException+=(_,e)=>critical.LogCritical(e.Exception,"Unobserved task error");
  return app;
 }
}
