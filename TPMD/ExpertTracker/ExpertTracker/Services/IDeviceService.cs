namespace ExpertTracker.Services;
public sealed record Coordinates(double Latitude,double Longitude);
public interface IDeviceService {
 Task<string?> CaptureReceiptAsync();
 Task<Coordinates?> GetCoordinatesAsync();
}
