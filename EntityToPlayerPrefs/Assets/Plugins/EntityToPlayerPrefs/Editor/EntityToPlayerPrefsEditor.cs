using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;
using System;
using Assets.Plugins.EntityToPlayerPrefs.FieldHandlers;

namespace Assets.Plugins.EntityToPlayerPrefs.Editor
{
    public class EntityToPlayerPrefsEditor : EditorWindow
    {
        private const string NotEntitiesTab = "Not Entities";

        private Dictionary<string, object> _keyValueDic;
        private List<PPEntity> _entities;
        private string _selectedType;
        private Vector2 _verticalScroll;

		//[MenuItem("Window/Utils/Entity to PlayerPrefs %p")]
		[MenuItem("Window/Utils/Entity to PlayerPrefs")]
        public static void ShowEditor()
        {
            EntityToPlayerPrefsEditor window = GetWindow<EntityToPlayerPrefsEditor>();
            window.titleContent = new GUIContent("Entity to PlayerPrefs");
        }

        public void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (string.IsNullOrEmpty(_selectedType))
                _selectedType = NotEntitiesTab;

			if(Application.platform == RuntimePlatform.WindowsEditor)
				_keyValueDic = LoadForWindows();
			else if(Application.platform == RuntimePlatform.OSXEditor)
				_keyValueDic = LoadForMacOS();
			else
				Debug.Log("Not supported platform: " + Application.platform);
			
            _entities = GroupByEntities(_keyValueDic);
        }

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
                    if (PlayerPrefs.HasKey(ppKey))
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
			string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + 
				"/Library/Preferences/unity." + PlayerSettings.companyName + "." + PlayerSettings.productName + ".plist";
			Dictionary<string, object> plistDic = (Dictionary<string, object>) PlistCS.Plist.readPlist(fullPath);

			foreach(string key in plistDic.Keys.ToArray())
				if(!PlayerPrefs.HasKey(key))
					plistDic.Remove(key);

			return plistDic;
		}

        private List<PPEntity> GroupByEntities(Dictionary<string, object> keyValueDic)
        {
            List<PPEntity> entities = new List<PPEntity>();
            foreach (string ppKey in keyValueDic.Keys)
            {
                if (ppKey.StartsWith(PlayerPrefsMapper.EntityKeyPrefix))
                {
                    string[] parts = ppKey.Split('.');

                    string type = parts[1];
                    string id = parts[2];
                    PPEntity entity = entities.SingleOrDefault(_ => _.Type == type && _.Id == id);

                    if (entity == null)
                    {
                        entity = new PPEntity(type, id);
                        entities.Add(entity);
                    }

                    string propertyName = parts[3];
                    object value = keyValueDic[ppKey];
                    entity.AddValue(propertyName, value, ppKey);
                }
            }
            return entities;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            DrawTopTabs();
            DrawSelectedTypeEntities();
            DrawBottomPanel();
            GUILayout.EndVertical();
        }

        private void DrawTopTabs()
        {
            GUILayout.BeginHorizontal();

            List<string> tabs = _entities.Select(_ => _.Type).Distinct().ToList();
            tabs.Insert(0, NotEntitiesTab);

            foreach (string type in tabs)
            {
                if (GUILayout.Toggle(_selectedType == type, type, EditorStyles.toolbarButton))
                    _selectedType = type;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawSelectedTypeEntities()
        {
            _verticalScroll = GUILayout.BeginScrollView(_verticalScroll);

            if (_selectedType == NotEntitiesTab)
            {
                foreach (string ppKey in GetNoEntitiesKeys())
                    DrawRecord(ppKey, ppKey, _keyValueDic[ppKey]);
            }
            else
            {
                foreach (PPEntity entity in _entities.Where(_ => _.Type == _selectedType))
                    DrawEntity(entity);
            }

            GUILayout.EndScrollView();
        }

        private List<string> GetNoEntitiesKeys()
        {
            return _keyValueDic.Keys.Where(_ => !_.StartsWith(PlayerPrefsMapper.EntityKeyPrefix)).ToList();
        }

        private void DrawEntity(PPEntity entity)
        {
            GUILayout.BeginVertical("Box");

            GUILayout.Label("Id: " + entity.Id);

            foreach (string propertyName in entity.GetSortedKeys())
                DrawRecord(propertyName, entity.PPKeys[propertyName], entity.Values[propertyName]);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                DeleteEntity(entity);
                PlayerPrefs.Save();
                Refresh();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawRecord(string recordKey, string ppKey, object value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(recordKey, GUILayout.Width(150));
            bool hasChanges = false;

			if (value is byte[] ||	//string on Windows
				value is string)	//string on MacOS
			{
			    if (DateTimeFieldHandler.IsDateTimeRecord(ppKey))
			    {
			        hasChanges = DateTimeFieldHandler.DrawEditor(ppKey);
			    }
                else if (BoolFieldHandler.IsBoolRecord(ppKey))
                {
                    hasChanges = BoolFieldHandler.DrawEditor(ppKey);
                }
			    else
			    {
                    string oldValue = PlayerPrefs.GetString(ppKey);
                    string newValue = EditorGUILayout.TextField(oldValue);
                    if (newValue != oldValue)
                    {
                        PlayerPrefs.SetString(ppKey, newValue);
                        PlayerPrefs.Save();
                        hasChanges = true;
                    }
                }
            }
            else if (value is int)
            {
                if (IsInt(ppKey))
                {
                    int oldValue = PlayerPrefs.GetInt(ppKey);
                    int newValue = EditorGUILayout.IntField(oldValue);
                    if (newValue != oldValue)
                    {
                        PlayerPrefs.SetInt(ppKey, newValue);
                        PlayerPrefs.Save();
                        hasChanges = true;
                    }
                }
                else //float on Windows
                {
                    hasChanges = DrawFloatField(ppKey);
                }
            }
            else if(value is double) //float on MacOS
            {
                hasChanges = DrawFloatField(ppKey);
            }
            else
            {
                GUILayout.Label("Not implemented value type: " + value.GetType().Name);
            }

            if (hasChanges)
                Refresh();

            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                PlayerPrefs.DeleteKey(ppKey);
                PlayerPrefs.Save();
                Refresh();
            }

            GUILayout.EndHorizontal();
        }

        private bool IsInt(string ppKey)
        {
            int intValue = PlayerPrefs.GetInt(ppKey);
            if (intValue != 0)
                return true;

            float floatValue = PlayerPrefs.GetFloat(ppKey);
            if (!Mathf.Approximately(floatValue, 0f))
                return false;

            return true;
        }

        private bool DrawFloatField(string ppKey)
        {
            float oldValue = PlayerPrefs.GetFloat(ppKey);
            float newValue = EditorGUILayout.FloatField(oldValue);
            if (!Mathf.Approximately(oldValue, newValue))
            {
                PlayerPrefs.SetFloat(ppKey, newValue);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        private void DeleteEntity(PPEntity entity)
        {
            foreach (string ppKey in entity.PPKeys.Values)
                PlayerPrefs.DeleteKey(ppKey);
        }
         
        private void DrawBottomPanel()
        {
            GUILayout.BeginHorizontal("box");
            DrawVersion();
            GUILayout.FlexibleSpace();
            DrawClearTabButton();
            DrawClearAllButton();
            GUILayout.EndHorizontal();
        }

        private void DrawVersion()
        {
            GUILayout.Label("Version 0.3");
        }

        private void DrawClearTabButton()
        {
            if (GUILayout.Button(string.Format("Clear '{0}' tab", _selectedType)))
            {
                if (EditorUtility.DisplayDialog(
                    string.Format("Delete all records on '{0}' tab", _selectedType),
                    string.Format("All records on tab '{0}' will be deleted!", _selectedType), "Delete", "Cancel"))
                {
                    if (_selectedType == NotEntitiesTab) 
                    {
                        foreach (string ppKey in GetNoEntitiesKeys())
                            PlayerPrefs.DeleteKey(ppKey);
                    }
                    else
                    {
                        foreach (PPEntity entity in _entities.Where(_ => _.Type == _selectedType))
                            DeleteEntity(entity);
                        _selectedType = NotEntitiesTab;
                    }

                    PlayerPrefs.Save();
                    Refresh();
                }
            }
        }

        private void DrawClearAllButton()
        {
            if (GUILayout.Button("Clear All"))
            {
                if (EditorUtility.DisplayDialog("Delete all records", "All records on all tabs will be deleted!", "Delete", "Cancel"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    Refresh();
                    _selectedType = NotEntitiesTab;
                }
            }
        }
    }
}
