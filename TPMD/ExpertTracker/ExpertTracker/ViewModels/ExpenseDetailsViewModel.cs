using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Services;
namespace ExpertTracker.ViewModels;
public partial class ExpenseDetailsViewModel:ObservableObject,IQueryAttributable,ILifecycleAware {
 private readonly IExpenseRepository repo;private int id;
 [ObservableProperty]public partial string Description {get;set;}
 [ObservableProperty]public partial string Category {get;set;}
 [ObservableProperty]public partial string AmountText {get;set;}
 [ObservableProperty]public partial string DateText {get;set;}
 [ObservableProperty]public partial string Message {get;set;}
 public ExpenseDetailsViewModel(IExpenseRepository repository) {
  repo=repository;Description=Category=AmountText=DateText=Message="";
 }
 public void ApplyQueryAttributes(IDictionary<string,object> query) {
  if(!query.TryGetValue("id",out var v)||
     !int.TryParse(v?.ToString(),out id)){Message="Некоректний ID.";return;}
  _=LoadAsync();
 }
 [RelayCommand]private async Task LoadAsync() {
  try {
   var x=await repo.GetByIdAsync(id);
   if(x is null){Message="Не знайдено.";return;}
   Description=x.Description;Category=x.Category;
   AmountText=x.Amount.ToString("0.00",CultureInfo.InvariantCulture);
   DateText=x.Date.ToString("dd.MM.yyyy");Message="";
  }catch(Exception ex){System.Diagnostics.Debug.WriteLine(ex);Message="Помилка завантаження.";}
 }
 [RelayCommand]private async Task SaveAsync() {
  if(string.IsNullOrWhiteSpace(Description)||string.IsNullOrWhiteSpace(Category)||
   !decimal.TryParse((AmountText??"").Replace(',','.'),NumberStyles.AllowDecimalPoint,
    CultureInfo.InvariantCulture,out var a)||a<=0||a>1000000000m||decimal.Round(a,2)!=a){
   Message="Перевірте поля.";return;
  }
  try {
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Не знайдено.";return;}
   x.Description=Description.Trim();x.Category=Category.Trim();x.Amount=a;
   await repo.UpdateAsync(x);await Shell.Current.GoToAsync("..");
  }catch(Exception ex){System.Diagnostics.Debug.WriteLine(ex);Message="Не вдалося оновити.";}
 }
 [RelayCommand]private async Task DeleteAsync() {
  try {
   var x=await repo.GetByIdAsync(id);if(x is null){Message="Не знайдено.";return;}
   await repo.DeleteAsync(x);await Shell.Current.GoToAsync("..");
  }catch(Exception ex){System.Diagnostics.Debug.WriteLine(ex);Message="Не вдалося видалити.";}
 }
 [RelayCommand]private Task GoBackAsync()=>Shell.Current.GoToAsync("..");
 public void OnAppearing(){}public void OnDisappearing(){}
}
