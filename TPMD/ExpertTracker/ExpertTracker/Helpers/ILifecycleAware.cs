namespace ExpertTracker.Helpers;

// View лише передає події; логіку виконує ViewModel.
public interface ILifecycleAware
{
    void OnAppearing();
    void OnDisappearing();
}
