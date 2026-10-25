using System.Security.Cryptography;
using ExpertTracker.Models;
using Microsoft.Extensions.Logging;
using SQLite;
namespace ExpertTracker.Services;
// Local teaching example; demo credentials must never be reused in production.
public sealed class AuthService
{
 private readonly SQLiteAsyncConnection db;
 private readonly ILogger<AuthService> log;
 private readonly SemaphoreSlim gate=new(1,1);
 private bool ready;
 public User? CurrentUser {get;private set;}
 public bool IsOwner=>CurrentUser?.Role==UserRole.Owner;
 public bool IsSignedIn=>CurrentUser is not null;
 public AuthService(string databasePath, ILogger<AuthService> logger)
 {
  db=new SQLiteAsyncConnection(databasePath); log=logger;
 }
 private async Task Init()
 {
  if(ready)return;
  await gate.WaitAsync();
  try{
   if(ready)return;
   await db.CreateTableAsync<User>();
   if(await db.Table<User>().CountAsync()==0)
   {
    await CreateDemoUserAsync("owner","demo-owner-2026",UserRole.Owner);
    await CreateDemoUserAsync("accountant","demo-accountant-2026",UserRole.Accountant);
   }
   ready=true;
  }finally{gate.Release();}
 }
 private async Task CreateDemoUserAsync(string login,string password,UserRole role)
 {
  byte[] salt=RandomNumberGenerator.GetBytes(16);
  byte[] hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,150000,HashAlgorithmName.SHA256,32);
  await db.InsertAsync(new User{Login=login,PasswordSalt=Convert.ToBase64String(salt),
   PasswordHash=Convert.ToBase64String(hash),Role=role});
 }
 public async Task<bool> LoginAsync(string login,string password)
 {
  await Init();
  User? user=await db.Table<User>().Where(u=>u.Login==login).FirstOrDefaultAsync();
  bool valid=false;
  if(user is not null)
  {
   byte[] salt=Convert.FromBase64String(user.PasswordSalt);
   byte[] expected=Convert.FromBase64String(user.PasswordHash);
   byte[] actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,150000,HashAlgorithmName.SHA256,32);
   valid=CryptographicOperations.FixedTimeEquals(expected,actual);
  }
  CurrentUser=valid?user:null;
  if(!valid)log.LogWarning("Rejected local login");
  else log.LogInformation("Local user authenticated with role {Role}",user!.Role);
  return valid;
 }
 public void Logout()=>CurrentUser=null;
}
