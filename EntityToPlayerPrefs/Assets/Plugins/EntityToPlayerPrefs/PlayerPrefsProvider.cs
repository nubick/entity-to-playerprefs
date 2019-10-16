using System;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public static class PlayerPrefsProvider
    {
        private static IPlayerPrefsProvider _provider;
        private static IPlayerPrefsProvider Provider
        {
            get { return _provider ?? (_provider = new UnityPlayerPrefsProvider()); }
            set { _provider = value; }
        }

        public static void SetProvider(IPlayerPrefsProvider playerPrefsProvider)
        {
            if (playerPrefsProvider == null)
                throw new ArgumentNullException(nameof(playerPrefsProvider));
            
            Provider = playerPrefsProvider;
        }
        
        public static void SetString(string key, string value)
        {
            Provider.SetString(key, value);
        }

        public static string GetString(string key)
        {
            return Provider.GetString(key);
        }

        public static string GetString(string key, string defaultValue)
        {
            return Provider.GetString(key);
        }

        public static void SetInt(string key, int value)
        {
            Provider.SetInt(key, value);
        }

        public static int GetInt(string key)
        {
            return Provider.GetInt(key);
        }

        public static void SetFloat(string key, float value)
        {
            Provider.SetFloat(key, value);
        }

        public static float GetFloat(string key)
        {
            return Provider.GetFloat(key);
        }

        public static bool HasKey(string key)
        {
            return Provider.HasKey(key);
        }
        
        public static void DeleteKey(string key)
        {
            Provider.DeleteKey(key);
        }

        public static void DeleteAll()
        {
            Provider.DeleteAll();
        }
        
        public static void Save()
        {
            Provider.Save();
        }
    }
}