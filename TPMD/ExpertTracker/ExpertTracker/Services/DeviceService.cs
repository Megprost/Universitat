using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;
namespace ExpertTracker.Services;
public sealed class DeviceService : IDeviceService
{
 private readonly ILogger<DeviceService> log;
 public DeviceService(ILogger<DeviceService> logger)=>log=logger;

 public async Task<string?> CaptureReceiptAsync()
 {
  try {
   var permission=await Permissions.CheckStatusAsync<Permissions.Camera>();
   if(permission!=PermissionStatus.Granted)
    permission=await Permissions.RequestAsync<Permissions.Camera>();
   if(permission!=PermissionStatus.Granted){log.LogWarning("Camera permission denied");return null;}
   var photo=await MediaPicker.Default.CapturePhotoAsync();
   if(photo is null)return null;
   var extension=Path.GetExtension(photo.FileName);
   if(string.IsNullOrWhiteSpace(extension))extension=".jpg";
   string path=Path.Combine(FileSystem.AppDataDirectory,"receipt-"+Guid.NewGuid().ToString("N")+extension);
   await using var source=await photo.OpenReadAsync();
   await using var dest=File.Create(path);
   await source.CopyToAsync(dest);
   log.LogInformation("Receipt photo saved inside app storage");
   return path;
  }
  catch(Exception ex) when(ex is FeatureNotSupportedException or PermissionException or InvalidOperationException) {
   log.LogWarning(ex,"Camera unavailable or access refused");return null;
  }
 }

 public async Task<Coordinates?> GetCoordinatesAsync()
 {
  try {
   var permission=await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
   if(permission!=PermissionStatus.Granted)
    permission=await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
   if(permission!=PermissionStatus.Granted){log.LogWarning("Location permission denied");return null;}
   var request=new GeolocationRequest(GeolocationAccuracy.Medium,TimeSpan.FromSeconds(15));
   var location=await Geolocation.Default.GetLocationAsync(request);
   if(location is null)return null;
   log.LogInformation("Location obtained (coordinates not included in log)");
   return new Coordinates(location.Latitude,location.Longitude);
  }
  catch(Exception ex) when(ex is FeatureNotSupportedException or PermissionException or InvalidOperationException) {
   log.LogWarning(ex,"Location unavailable or access refused");return null;
  }
 }
}
