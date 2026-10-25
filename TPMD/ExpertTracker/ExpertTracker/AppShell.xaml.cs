using ExpertTracker.Views;

namespace ExpertTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ExpenseDetailsPage), typeof(ExpenseDetailsPage));
    }
}
