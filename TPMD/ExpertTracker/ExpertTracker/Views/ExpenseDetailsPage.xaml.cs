using ExpertTracker.Helpers;
using ExpertTracker.ViewModels;

namespace ExpertTracker.Views;

public partial class ExpenseDetailsPage : ContentPage
{
    public ExpenseDetailsPage(ExpenseDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as ILifecycleAware)?.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        (BindingContext as ILifecycleAware)?.OnDisappearing();
        base.OnDisappearing();
    }
}
