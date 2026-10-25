using ExpertTracker.ViewModels;
namespace ExpertTracker.Views;
public partial class LoginPage:ContentPage
{
 public LoginPage(LoginViewModel vm){InitializeComponent();BindingContext=vm;}
}
