using System.Collections.Generic;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public interface IPlayerPrefsProvider
    {
        void SetString(string key, string value);
        string GetString(string key, string defaultValue = "");
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        void SetFloat(string key, float value);
        float GetFloat(string key, float defaultValue = 0.0f);
        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();
        void Save();
        Dictionary<string, object> GetAllKeysAndValues();
    }
}