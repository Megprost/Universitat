using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Services;
using Microsoft.Extensions.Logging;

namespace ExpertTracker.ViewModels;

public partial class ExpenseDetailsViewModel : ObservableObject, IQueryAttributable, ILifecycleAware
{
    private readonly IExpenseRepository repo;
    private readonly ILogger<ExpenseDetailsViewModel> log;
    private readonly IAuthService auth;
    private readonly IDeviceService device;
    private int id;

    public bool CanDelete => auth.IsSignedIn && auth.IsOwner;

    [ObservableProperty] public partial string Description { get; set; }
    [ObservableProperty] public partial string Category { get; set; }
    [ObservableProperty] public partial string AmountText { get; set; }
    [ObservableProperty] public partial string DateText { get; set; }
    [ObservableProperty] public partial string Message { get; set; }
    [ObservableProperty] public partial string? ImagePath { get; set; }
    [ObservableProperty] public partial string CoordinatesText { get; set; }

    public ExpenseDetailsViewModel(IExpenseRepository repository,
        ILogger<ExpenseDetailsViewModel> logger, IAuthService authService,
        IDeviceService deviceService)
    {
        repo = repository;
        log = logger;
        auth = authService;
        device = deviceService;
        Description = Category = AmountText = DateText = Message = "";
        CoordinatesText = "GPS: не вказано";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("id", out var raw) ||
            !int.TryParse(raw?.ToString(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out id))
        {
            Message = "Некоректний ID витрати.";
            return;
        }
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (!auth.IsSignedIn) { Message = "Спочатку увійдіть."; return; }
        try
        {
            var item = await repo.GetByIdAsync(id);
            if (item is null)
            {
                log.LogWarning("Expense ID {ExpenseId} not found", id);
                Message = "Витрату не знайдено.";
                return;
            }
            Description = item.Description;
            Category = item.Category;
            AmountText = item.Amount.ToString("0.00", CultureInfo.InvariantCulture);
            DateText = item.Date.ToString("dd.MM.yyyy");
            ImagePath = item.ImagePath;
            CoordinatesText = item.Latitude.HasValue && item.Longitude.HasValue
                ? $"GPS: {item.Latitude.Value:F5}, {item.Longitude.Value:F5}"
                : "GPS: не вказано";
            Message = "";
            OnPropertyChanged(nameof(CanDelete));
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Expense detail loading failed");
            Message = "Не вдалося завантажити витрату.";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!auth.IsSignedIn) { Message = "Спочатку увійдіть."; return; }
        if (string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(Category) ||
            !decimal.TryParse((AmountText ?? "").Replace(',', '.'),
                NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var amount) ||
            amount <= 0 || amount > 1000000000m || decimal.Round(amount, 2) != amount)
        {
            Message = "Перевірте назву, категорію та суму.";
            return;
        }
        try
        {
            var item = await repo.GetByIdAsync(id);
            if (item is null) { Message = "Витрату не знайдено."; return; }
            item.Description = Description.Trim();
            item.Category = Category.Trim();
            item.Amount = amount;
            if (await repo.UpdateAsync(item) == 0)
            {
                Message = "Запис не вдалося оновити.";
                return;
            }
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Expense update failed");
            Message = "Не вдалося оновити витрату.";
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (!auth.IsSignedIn || !auth.IsOwner)
        {
            log.LogWarning("Attempted deletion without Owner permission");
            Message = "Лише власник ФОП може видаляти витрати.";
            return;
        }
        try
        {
            var item = await repo.GetByIdAsync(id);
            if (item is null) { Message = "Витрату не знайдено."; return; }
            if (await repo.DeleteAsync(item) == 0)
            {
                Message = "Не вдалося видалити витрату.";
                return;
            }
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Expense deletion failed");
            Message = "Не вдалося видалити витрату.";
        }
    }

    [RelayCommand]
    private async Task AttachReceiptAsync()
    {
        if (!auth.IsSignedIn) { Message = "Спочатку увійдіть."; return; }
        try
        {
            string? path = await device.CaptureReceiptAsync();
            if (path is null)
            {
                Message = "Фото не отримано. Перевірте дозвіл камери.";
                return;
            }
            var item = await repo.GetByIdAsync(id);
            if (item is null) { Message = "Витрату не знайдено."; return; }
            item.ImagePath = path;
            if (await repo.UpdateAsync(item) == 0)
            {
                Message = "Не вдалося зберегти фото чека.";
                return;
            }
            ImagePath = path;
            Message = "Фото чека збережено.";
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Receipt attachment failed");
            Message = "Не вдалося додати фото чека.";
        }
    }

    [RelayCommand]
    private async Task CaptureLocationAsync()
    {
        if (!auth.IsSignedIn) { Message = "Спочатку увійдіть."; return; }
        try
        {
            var coordinates = await device.GetCoordinatesAsync();
            if (coordinates is null)
            {
                Message = "GPS недоступний або дозвіл не надано.";
                return;
            }
            var item = await repo.GetByIdAsync(id);
            if (item is null) { Message = "Витрату не знайдено."; return; }
            item.Latitude = coordinates.Latitude;
            item.Longitude = coordinates.Longitude;
            if (await repo.UpdateAsync(item) == 0)
            {
                Message = "Не вдалося зберегти координати.";
                return;
            }
            CoordinatesText = string.Format(CultureInfo.InvariantCulture,
                "GPS: {0:F5}, {1:F5}", coordinates.Latitude, coordinates.Longitude);
            Message = "Координати збережено.";
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Expense coordinates update failed");
            Message = "Не вдалося отримати координати.";
        }
    }

    [RelayCommand]
    private Task GoBackAsync() => Shell.Current.GoToAsync("..");

    public void OnAppearing() => OnPropertyChanged(nameof(CanDelete));
    public void OnDisappearing() { }
}
