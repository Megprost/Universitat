using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace ExpertTracker.Services;

public sealed class DeviceService : IDeviceService
{
    private readonly ILogger<DeviceService> log;
    public DeviceService(ILogger<DeviceService> logger) => log = logger;

    public async Task<string?> CaptureReceiptAsync()
    {
        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (permission != PermissionStatus.Granted)
                permission = await Permissions.RequestAsync<Permissions.Camera>();

            if (permission != PermissionStatus.Granted)
            {
                log.LogWarning("Camera permission not granted");
                return null;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null) return null;

            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png") ext = ".jpg";

            var path = Path.Combine(FileSystem.AppDataDirectory,
                "receipt-" + Guid.NewGuid().ToString("N") + ext);
            await using var source = await photo.OpenReadAsync();
            await using var target = File.Create(path);
            await source.CopyToAsync(target);

            log.LogInformation("Receipt photo stored in application directory");
            return path;
        }
        catch (OperationCanceledException)
        {
            log.LogInformation("Photo capture cancelled by user");
            return null;
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Photo capture not available or failed");
            return null;
        }
    }

    public async Task<Coordinates?> GetCoordinatesAsync()
    {
        try
        {
            var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permission != PermissionStatus.Granted)
                permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (permission != PermissionStatus.Granted)
            {
                log.LogWarning("Location permission not granted");
                return null;
            }

            var request = new GeolocationRequest(
                GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));
            var position = await Geolocation.Default.GetLocationAsync(request);
            if (position is null)
            {
                log.LogWarning("Device did not return location");
                return null;
            }
            log.LogInformation("Expense location obtained");
            return new Coordinates(position.Latitude, position.Longitude);
        }
        catch (OperationCanceledException)
        {
            log.LogInformation("Location request cancelled");
            return null;
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Location unavailable or permission denied");
            return null;
        }
    }
}
