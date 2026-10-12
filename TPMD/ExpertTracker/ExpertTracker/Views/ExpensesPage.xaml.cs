
using ExpertTracker.ViewModels;

namespace ExpertTracker.Views;

public partial class ExpensesPage : ContentPage
{
    public ExpensesPage(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
