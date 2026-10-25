using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Services;
using Microsoft.Extensions.Logging;
namespace ExpertTracker.ViewModels;
public partial class ExpenseDetailsViewModel:ObservableObject,IQueryAttributable,ILifecycleAware {
 private readonly IExpenseRepository repo;private readonly ILogger<ExpenseDetailsViewModel> log;
 private readonly AuthService auth;
 private readonly IDeviceService device;
 public bool CanDelete=>auth.IsOwner;private int id;
 [ObservableProperty]public partial string Description {get;set;}
 [ObservableProperty]public partial string Category {get;set;}
 [ObservableProperty]public partial string AmountText {get;set;}
 [ObservableProperty]public partial string DateText {get;set;}
 [ObservableProperty]public partial string Message {get;set;}
 [ObservableProperty]public partial string? ImagePath {get;set;}
 [ObservableProperty]public partial string CoordinatesText {get;set;}
 public ExpenseDetailsViewModel(IExpenseRepository repository, ILogger<ExpenseDetailsViewModel> logger, AuthService authService, IDeviceService deviceService) {
  log=logger;auth=authService;device=deviceService;CoordinatesText="";
  repo=repository;Description=Category=AmountText=DateText=Message="";
 }
 public void ApplyQueryAttributes(IDictionary<string,object> query) {
  if(!query.TryGetValue("id",out var v)||
     !int.TryParse(v?.ToString(),out id)){Message="Некоректний ID.";return;}
  _=LoadAsync();
 }
 [RelayCommand]private async Task LoadAsync() {
  if(!auth.IsSignedIn){Message="Спочатку увійдіть.";return;}
  try {
   var x=await repo.GetByIdAsync(id);
   if(x is null){Message="Не знайдено.";return;}
   Description=x.Description;Category=x.Category;
   AmountText=x.Amount.ToString("0.00",CultureInfo.InvariantCulture);
   DateText=x.Date.ToString("dd.MM.yyyy");ImagePath=x.ImagePath;
   CoordinatesText=x.Latitude is not null&&x.Longitude is not null?$"GPS: {x.Latitude:F5}, {x.Longitude:F5}":"GPS: не вказано";Message="";
   OnPropertyChanged(nameof(CanDelete));
  }catch(Exception ex){log.LogError(ex,"Expense detail operation failed");Message="Помилка завантаження.";}
 }
 [RelayCommand]private async Task SaveAsync() {
  if(!auth.IsSignedIn){Message="Спочатку увійдіть.";return;}
  if(string.IsNullOrWhiteSpace(Description)||string.IsNullOrWhiteSpace(Category)||
   !decimal.TryParse((AmountText??"").Replace(',','.'),NumberStyles.AllowDecimalPoint,
    CultureInfo.InvariantCulture,out var a)||a<=0||a>1000000000m||decimal.Round(a,2)!=a){
   Message="Перевірте поля.";return;
  }
  try {
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Не знайдено.";return;}
   x.Description=Description.Trim();x.Category=Category.Trim();x.Amount=a;
   await repo.UpdateAsync(x);await Shell.Current.GoToAsync("..");
  }catch(Exception ex){log.LogError(ex,"Expense detail operation failed");Message="Не вдалося оновити.";}
 }
 [RelayCommand]private async Task DeleteAsync() {
  if(!auth.IsOwner){Message="Лише власник може видаляти витрати.";return;}
  try {
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Не знайдено.";return;}
   await repo.DeleteAsync(x);await Shell.Current.GoToAsync("..");
  }catch(Exception ex){log.LogError(ex,"Expense detail operation failed");Message="Не вдалося видалити.";}
 }
 [RelayCommand]private async Task AttachReceiptAsync(){
  try {
   if(!auth.IsSignedIn){Message="Потрібно увійти.";return;}
   string? path=await device.CaptureReceiptAsync();
   if(path is null){Message="Камера недоступна або дозвіл не надано.";return;}
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Запис відсутній.";return;}
   x.ImagePath=path;await repo.UpdateAsync(x);ImagePath=path;Message="Фото чека додано.";
  }catch(Exception ex){log.LogError(ex,"Receipt attach failed");Message="Не вдалося прикріпити фото.";}
 }
 [RelayCommand]private async Task CaptureLocationAsync(){
  try {
   if(!auth.IsSignedIn){Message="Потрібно увійти.";return;}
   var location=await device.GetCoordinatesAsync();
   if(location is null){Message="GPS недоступний або дозвіл не надано.";return;}
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Запис відсутній.";return;}
   x.Latitude=location.Latitude;x.Longitude=location.Longitude;
   await repo.UpdateAsync(x);CoordinatesText=$"GPS: {location.Latitude:F5}, {location.Longitude:F5}";
   Message="Координати збережено.";
  }catch(Exception ex){log.LogError(ex,"Location save failed");Message="Не вдалося визначити місце.";}
 }
 [RelayCommand]private Task GoBackAsync()=>Shell.Current.GoToAsync("..");
 public void OnAppearing(){}public void OnDisappearing(){}
}
