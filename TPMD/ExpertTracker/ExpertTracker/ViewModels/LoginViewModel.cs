using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpertTracker.Services;
namespace ExpertTracker.ViewModels;
public partial class LoginViewModel:ObservableObject
{
 private readonly AuthService auth;
 [ObservableProperty] public partial string Login {get;set;}
 [ObservableProperty] public partial string Password {get;set;}
 [ObservableProperty] public partial string Message {get;set;}
 public LoginViewModel(AuthService service){auth=service;Login=Password=Message="";}
 [RelayCommand] private async Task SignInAsync()
 {
  try {
   if(await auth.LoginAsync(Login.Trim(),Password))
   {
    Password="";Message="";
    await Shell.Current.GoToAsync("//expenses");
   }else{Message="Неправильний логін або пароль.";}
  }catch(Exception){Message="Не вдалося виконати вхід.";}
 }
}
