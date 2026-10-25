using SQLite;
namespace ExpertTracker.Models;
[Table("Users")]
public sealed class User
{
 [PrimaryKey,AutoIncrement]public int Id {get;set;}
 [Unique]public string Login {get;set;}="";
 public string PasswordSalt {get;set;}="";
 public string PasswordHash {get;set;}="";
 public int RoleValue {get;set;}
 [Ignore]public UserRole Role {get=> (UserRole)RoleValue;set=>RoleValue=(int)value;}
}
