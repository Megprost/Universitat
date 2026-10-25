namespace ExpertTracker.Services;
public sealed class PreferencesDraftStore : IDraftStore {
 public void Set(string key,string value)=>Preferences.Default.Set(key,value);
 public string Get(string key,string fallback)=>Preferences.Default.Get(key,fallback);
}
