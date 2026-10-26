using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Helpers;
using ExpertTracker.Models;
using ExpertTracker.Services;
using ExpertTracker.Views;
using Microsoft.Extensions.Logging;

namespace ExpertTracker.ViewModels;
public partial class ExpensesViewModel : ObservableObject, ILifecycleAware {
 private bool draftReady;
 private bool restoringDraft;
 partial void OnDescriptionChanged(string value)=>SaveDraftIfReady();
 partial void OnCategoryChanged(string value)=>SaveDraftIfReady();
 partial void OnAmountTextChanged(string value)=>SaveDraftIfReady();
 private void SaveDraftIfReady(){if(draftReady&&!restoringDraft)SaveDraft();}

 private readonly IExpenseRepository repo;
 private readonly ILogger<ExpensesViewModel> log;
 private readonly IDraftStore drafts;
 public ObservableCollection<Expense> Expenses {get;}=new();
 [ObservableProperty] public partial string Description {get;set;}
 [ObservableProperty] public partial string Category {get;set;}
 [ObservableProperty] public partial string AmountText {get;set;}
 [ObservableProperty] public partial string ErrorMessage {get;set;}
 [ObservableProperty] public partial Expense? SelectedExpense {get;set;}
 public decimal TotalAmount=>Expenses.Sum(x=>x.Amount);

 // Visible only in Debug builds. This test does not modify SQLite data.
 public bool ShowDiagnostics
 {
  get
  {
#if DEBUG
   return true;
#else
   return false;
#endif
  }
 }

 [RelayCommand]
 private void SimulateFailure()
 {
#if DEBUG
  try
  {
   throw new InvalidOperationException("Controlled failure for checking error logging.");
  }
  catch (Exception ex)
  {
   log.LogError(ex, "Simulated failed expense operation");
   ErrorMessage = "Не вдалося зберегти дані, спробуйте ще раз.";
  }
#endif
 }
 public ExpensesViewModel(IExpenseRepository repository, ILogger<ExpensesViewModel> logger, IDraftStore draftStore) {
  drafts=draftStore;
  log=logger;
  repo=repository; Description=Category=AmountText=ErrorMessage=""; RestoreDraft();
 }
 [RelayCommand] private async Task LoadExpensesAsync() {
  try {
   var items=await repo.GetAllAsync(); Expenses.Clear();
   foreach(var item in items)Expenses.Add(item);
   OnPropertyChanged(nameof(TotalAmount)); ErrorMessage="";
  } catch(Exception ex) {
   log.LogError(ex,"Expense list loading failed");
   ErrorMessage="Не вдалося завантажити дані.";
  }
 }
 [RelayCommand] private async Task AddExpenseAsync() {
  if(string.IsNullOrWhiteSpace(Description)||string.IsNullOrWhiteSpace(Category)){
   log.LogWarning("Invalid expense: required fields missing");ErrorMessage="Введіть назву та категорію.";return;
  }
  if(!decimal.TryParse((AmountText??"").Replace(',','.'),
   NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out var amount)
   ||amount<=0||amount>1000000000m||decimal.Round(amount,2)!=amount) {
   log.LogWarning("Invalid expense amount");
   ErrorMessage="Введіть коректну суму (до 2 знаків після коми).";return;
  }
  try {
   var item=new Expense{Description=Description.Trim(),Category=Category.Trim(),Amount=amount,Date=DateTime.Today};
   int inserted = await repo.InsertAsync(item);
   if (inserted == 0)
   {
    log.LogWarning("Insert returned zero affected records");
    ErrorMessage = "Не вдалося зберегти дані, спробуйте ще раз.";
    return;
   }
   await LoadExpensesAsync();
   Description=Category=AmountText="";SaveDraft();
  } catch(Exception ex) { log.LogError(ex,"Expense insert failed");ErrorMessage="Не вдалося зберегти дані, спробуйте ще раз."; }
 }
 [RelayCommand] private async Task OpenDetailsAsync() {
  var item=SelectedExpense;if(item is null)return;
  SelectedExpense=null;SaveDraft();
  await Shell.Current.GoToAsync(nameof(ExpenseDetailsPage)+"?id="+item.Id.ToString(CultureInfo.InvariantCulture));
 }
 public void SaveDraft() {
  drafts.Set("draft.description",Description??"");
  drafts.Set("draft.category",Category??"");
  drafts.Set("draft.amount",AmountText??"");
 }
 public void RestoreDraft() {
  string description=drafts.Get("draft.description","");
  string category=drafts.Get("draft.category","");
  string amount=drafts.Get("draft.amount","");
  restoringDraft=true;
  try { Description=description;Category=category;AmountText=amount; }
  finally {restoringDraft=false;draftReady=true;}
 }
 public void OnAppearing() {RestoreDraft();_=LoadExpensesAsync();}
 public void OnDisappearing()=>SaveDraft();
}
