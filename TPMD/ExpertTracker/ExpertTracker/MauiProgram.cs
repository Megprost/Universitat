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
  b.Services.AddSingleton<IExpenseRepository>(_=>new DatabaseService(path));
  b.Services.AddSingleton<ExpensesViewModel>();
  b.Services.AddSingleton<ExpensesPage>();
  b.Services.AddSingleton<AppShell>();
  b.Services.AddTransient<ExpenseDetailsViewModel>();
  b.Services.AddTransient<ExpenseDetailsPage>();
#if DEBUG
  b.Logging.AddDebug();
#endif
  return b.Build();
 }
}
