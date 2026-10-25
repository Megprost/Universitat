using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Models;
using ExpertTracker.Services;
using ExpertTracker.Views;
using Microsoft.Maui.Storage;

namespace ExpertTracker.ViewModels;

public partial class ExpensesViewModel : ObservableObject, ILifecycleAware
{
    private const string DraftDescriptionKey = "lab02.draft.description";
    private const string DraftCategoryKey = "lab02.draft.category";
    private const string DraftAmountKey = "lab02.draft.amount";

    private readonly InMemoryExpenseStore _store;

    // Do not save partial draft values during the restore operation.
    private bool _draftInitialized;
    private bool _restoringDraft;

    partial void OnDescriptionChanged(string value) => SaveDraftIfReady();
    partial void OnCategoryChanged(string value) => SaveDraftIfReady();
    partial void OnAmountTextChanged(string value) => SaveDraftIfReady();

    private void SaveDraftIfReady()
    {
        if (_draftInitialized && !_restoringDraft)
            SaveDraft();
    }

    public ObservableCollection<Expense> Expenses => _store.Expenses;

    [ObservableProperty]
    public partial string Description { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

    [ObservableProperty]
    public partial string AmountText { get; set; }

    [ObservableProperty]
    public partial string ErrorMessage { get; set; }

    [ObservableProperty]
    public partial Expense? SelectedExpense { get; set; }

    public decimal TotalAmount => Expenses.Sum(e => e.Amount);

    public ExpensesViewModel(InMemoryExpenseStore store)
    {
        _store = store;
        Description = "";
        Category = "";
        AmountText = "";
        ErrorMessage = "";
        SelectedExpense = null;
        RestoreDraft();
    }

    [RelayCommand]
    private void AddExpense()
    {
        if (string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(Category))
        {
            ErrorMessage = "Введіть назву та категорію.";
            return;
        }

        string input = (AmountText ?? "").Trim().Replace(',', '.');
        if (!decimal.TryParse(
                input,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out decimal amount) || amount <= 0)
        {
            ErrorMessage = "Введіть коректну суму.";
            return;
        }

        _store.Add(Description.Trim(), Category.Trim(), amount);
        OnPropertyChanged(nameof(TotalAmount));

        Description = "";
        Category = "";
        AmountText = "";
        ErrorMessage = "";
        SaveDraft();
    }

    // CollectionView.SelectionChangedCommand викликає цю команду.
    [RelayCommand]
    private async Task OpenDetailsAsync()
    {
        var selected = SelectedExpense;
        if (selected is null)
            return;

        // Дозволяє натиснути той самий запис після повернення назад.
        SelectedExpense = null;
        SaveDraft();

        await Shell.Current.GoToAsync(
            nameof(ExpenseDetailsPage) + "?id=" +
            selected.Id.ToString(CultureInfo.InvariantCulture));
    }

    public void SaveDraft()
    {
        Preferences.Default.Set(DraftDescriptionKey, Description ?? "");
        Preferences.Default.Set(DraftCategoryKey, Category ?? "");
        Preferences.Default.Set(DraftAmountKey, AmountText ?? "");
        Debug.WriteLine("[Lab02] Draft saved with Preferences");
    }

    public void RestoreDraft()
    {
        // Read all keys before changing any bound property.
        string description = Preferences.Default.Get(DraftDescriptionKey, "");
        string category = Preferences.Default.Get(DraftCategoryKey, "");
        string amount = Preferences.Default.Get(DraftAmountKey, "");

        _restoringDraft = true;
        try
        {
            Description = description;
            Category = category;
            AmountText = amount;
        }
        finally
        {
            _restoringDraft = false;
            _draftInitialized = true;
        }
        Debug.WriteLine("[Lab02] Draft restored from Preferences");
    }

    public void OnAppearing()
    {
        Debug.WriteLine("[Lab02] ExpensesViewModel.OnAppearing");
        RestoreDraft();
    }

    public void OnDisappearing()
    {
        Debug.WriteLine("[Lab02] ExpensesViewModel.OnDisappearing");
        SaveDraft();
    }
}
