using ExpertTracker.Models;
namespace ExpertTracker.Services;
public interface IAuthService {
 User? CurrentUser {get;}
 bool IsSignedIn {get;}
 bool IsOwner {get;}
 Task<bool> LoginAsync(string login,string password);
 void Logout();
}
