using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Services;

namespace ExpertTracker.ViewModels;

// Ідентифікатор надходить через Shell query: ExpenseDetailsPage?id=...
public partial class ExpenseDetailsViewModel : ObservableObject, IQueryAttributable, ILifecycleAware
{
    private readonly InMemoryExpenseStore _store;

    [ObservableProperty]
    public partial string Description { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

    [ObservableProperty]
    public partial string AmountText { get; set; }

    [ObservableProperty]
    public partial string DateText { get; set; }

    [ObservableProperty]
    public partial string Message { get; set; }

    public ExpenseDetailsViewModel(InMemoryExpenseStore store)
    {
        _store = store;
        Description = "";
        Category = "";
        AmountText = "";
        DateText = "";
        Message = "";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out object? raw) ||
            !int.TryParse(raw?.ToString(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out int id))
        {
            Message = "Некоректний ID витрати.";
            return;
        }

        var item = _store.GetById(id);
        if (item is null)
        {
            Message = "Витрату не знайдено.";
            return;
        }

        Description = item.Description;
        Category = item.Category;
        AmountText = item.Amount.ToString("N2", CultureInfo.GetCultureInfo("uk-UA")) + " грн";
        DateText = item.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        Message = "";
        Debug.WriteLine("[Lab02] ExpenseDetails received expense ID=" + id);
    }

    [RelayCommand]
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    public void OnAppearing() =>
        Debug.WriteLine("[Lab02] ExpenseDetailsViewModel.OnAppearing");

    public void OnDisappearing() =>
        Debug.WriteLine("[Lab02] ExpenseDetailsViewModel.OnDisappearing");
}
