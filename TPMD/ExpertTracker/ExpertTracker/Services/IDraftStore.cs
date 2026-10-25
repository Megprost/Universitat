namespace ExpertTracker.Services;
public interface IDraftStore {
 void Set(string key,string value);
 string Get(string key,string fallback);
}
