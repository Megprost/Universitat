
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ExpertTracker.ViewModels;

public partial class ExpensesViewModel : ObservableObject
{
    public ObservableCollection<Expense> Expenses { get; } = new();


    [ObservableProperty]
    public partial string Description { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

    [ObservableProperty]
    public partial string AmountText { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; }

    public ExpensesViewModel()
    {
        Description = "";
        Category = "";
        AmountText = "";
        ErrorMessage = "";
    }


    public decimal TotalAmount => Expenses.Sum(e => e.Amount);

    [RelayCommand]
    private void AddExpense()
    {
        if (string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(Category))
        {
            ErrorMessage = "Введіть назву та категорію.";
            return;
        }

        string input = AmountText.Trim().Replace(',', '.');

        if (!decimal.TryParse(
                input,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal amount) || amount <= 0)
        {
            ErrorMessage = "Введіть коректну суму.";
            return;
        }

        Expenses.Add(new Expense
        {
            Id = Expenses.Count + 1,
            Description = Description.Trim(),
            Category = Category.Trim(),
            Amount = amount,
            Date = DateTime.Today
        });

        OnPropertyChanged(nameof(TotalAmount));

        Description = "";
        Category = "";
        AmountText = "";
        ErrorMessage = "";
    }
}
