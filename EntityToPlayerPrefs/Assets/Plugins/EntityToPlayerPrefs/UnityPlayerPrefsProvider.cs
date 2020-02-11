using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UnityEditor;
using Assets.Plugins.EntityToPlayerPrefs.Editor;
#endif

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public class UnityPlayerPrefsProvider : IPlayerPrefsProvider
    {
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public string GetString(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public Dictionary<string, object> GetAllKeysAndValues()
        {
            Dictionary<string, object> _keyValueDic = null;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                _keyValueDic = LoadForWindows();
            else if (Application.platform == RuntimePlatform.OSXEditor)
                _keyValueDic = LoadForMacOS();
            else
                Debug.Log("Not supported platform: " + Application.platform);
            return _keyValueDic;
        }

#if UNITY_EDITOR
        private Dictionary<string, object> LoadForWindows()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            string registryPath = GetWindowsRegestryPath();
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(registryPath);
            //When there are no any records, OpenSubKey return null but not object with no values.
            if (registryKey != null)
            {
                foreach (string valueName in registryKey.GetValueNames())
                {
                    object ppValue = registryKey.GetValue(valueName);
                    string ppKey = valueName.Remove(valueName.LastIndexOf("_"));
                    if (PlayerPrefsProvider.HasKey(ppKey))
                        values.Add(ppKey, ppValue);
                }
            }
            return values;
        }

        private string GetWindowsRegestryPath()
        {
#if UNITY_5_5_OR_NEWER
            return string.Format("Software\\Unity\\UnityEditor\\{0}\\{1}", PlayerSettings.companyName, PlayerSettings.productName);
#else
            return string.Format("Software\\{0}\\{1}", PlayerSettings.companyName, PlayerSettings.productName);
#endif
        }

        private Dictionary<string, object> LoadForMacOS()
        {
            // Plist from from https://github.com/animetrics/PlistCS:
            string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";
            Dictionary<string, object> plistDic = new Dictionary<string, object>();
            if (File.Exists(fullPath))
            {
                plistDic = (Dictionary<string, object>)Plist.readPlist(fullPath);
                foreach (string key in plistDic.Keys.ToArray())
                    if (!PlayerPrefsProvider.HasKey(key))
                        plistDic.Remove(key);
            }
            return plistDic;
        }

#endif
    }
}