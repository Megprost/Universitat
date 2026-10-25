using System.Diagnostics;
using ExpertTracker.ViewModels;

namespace ExpertTracker;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly ExpensesViewModel _expenses;

    public App(AppShell shell, ExpensesViewModel expenses)
    {
        InitializeComponent();
        _shell = shell;
        _expenses = expenses;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Debug.WriteLine("[Lab02] App.CreateWindow");
        var window = new Window(_shell);

        window.Created += (_, _) =>
            Debug.WriteLine("[Lab02] Window.Created");

        window.Activated += (_, _) =>
            Debug.WriteLine("[Lab02] Window.Activated");

        window.Deactivated += (_, _) =>
            Debug.WriteLine("[Lab02] Window.Deactivated");

        window.Stopped += (_, _) =>
        {
            Debug.WriteLine("[Lab02] Window.Stopped");
            _expenses.SaveDraft();
        };

        window.Resumed += (_, _) =>
        {
            Debug.WriteLine("[Lab02] Window.Resumed");
            _expenses.RestoreDraft();
        };

        window.Destroying += (_, _) =>
        {
            Debug.WriteLine("[Lab02] Window.Destroying");
            _expenses.SaveDraft();
        };

        return window;
    }
}
