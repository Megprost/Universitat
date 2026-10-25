using ExpertTracker.Services;
using ExpertTracker.ViewModels;
using ExpertTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExpertTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Singleton: один список у пам'яті для головного екрана та екрана деталей.
        builder.Services.AddSingleton<InMemoryExpenseStore>();
        builder.Services.AddSingleton<ExpensesViewModel>();
        builder.Services.AddSingleton<ExpensesPage>();
        builder.Services.AddSingleton<AppShell>();

        // Transient: нова VM та сторінка для кожного відкриття деталей.
        builder.Services.AddTransient<ExpenseDetailsViewModel>();
        builder.Services.AddTransient<ExpenseDetailsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
