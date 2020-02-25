﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using Assets.Plugins.EntityToPlayerPrefs.FieldHandlers;
using System.IO;

namespace Assets.Plugins.EntityToPlayerPrefs.Editor
{
    public class EntityToPlayerPrefsEditor : EditorWindow
    {
        private const string NotEntitiesTab = "Not Entities";
        public static string ClearOnStartKey = "EtoPPClearOnStart";

        private Dictionary<string, object> _keyValueDic;
        private List<PPEntity> _entities;
        private string _selectedType;
        private Vector2 _verticalScroll;

        [MenuItem("Window/Utils/Entity to PlayerPrefs %&p")]
        //[MenuItem("Window/Utils/Entity to PlayerPrefs")]
        public static void ShowEditor()
        {
            EntityToPlayerPrefsEditor window = GetWindow<EntityToPlayerPrefsEditor>();
            window.titleContent = new GUIContent("Entity to PlayerPrefs");
        }

        public void OnEnable()
        {
            Refresh();

        }

        public Dictionary<string, Type> allProvider = new Dictionary<string, Type>();

        void FindProvider()
        {
            allProvider = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IPlayerPrefsProvider).IsAssignableFrom(t) && !t.IsInterface)
                .ToDictionary(m => m.ToString(), m => m);
        }

        internal void Refresh()
        {
            FindProvider();
            if (string.IsNullOrEmpty(_selectedType))
                _selectedType = NotEntitiesTab;

            _keyValueDic = PlayerPrefsProvider.GetAllKeysAndValues();
            _entities = GroupByEntities(_keyValueDic);
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
            tabs.Add(NotEntitiesTab);

            foreach (string type in tabs)
            {
                if (GUILayout.Toggle(_selectedType == type, type, EditorStyles.toolbarButton))
                    _selectedType = type;
            }
            GUILayout.EndHorizontal();
        }

        private Rect _providerSelectorBtnRect;
        string currentProvider => PlayerPrefsProvider.Provider.ToString();
        private void DrawProviderSelector()
        {
            if (GUILayout.Button(currentProvider.Split('.').LastOrDefault(), EditorStyles.toolbarDropDown))
            {
                PopupWindow.Show(_providerSelectorBtnRect, new ProviderSelector
                {
                    editor = this
                });
            }
            if (Event.current.type == EventType.Repaint)
                _providerSelectorBtnRect = GUILayoutUtility.GetLastRect();
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

            if (entity.Id != PlayerPrefsMapper.SingleEntityId)
                GUILayout.Label("Id: " + entity.Id);

            foreach (string propertyName in entity.GetSortedKeys())
                DrawRecord(propertyName, entity.PPKeys[propertyName], entity.Values[propertyName]);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                DeleteEntity(entity);
                PlayerPrefsProvider.Save();
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

            ValueType valueType = GetValueType(value, ppKey);
            if (valueType == ValueType.String)
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
                    string oldValue = PlayerPrefsProvider.GetString(ppKey);
                    string newValue = EditorGUILayout.TextField(oldValue);
                    if (newValue != oldValue)
                    {
                        PlayerPrefsProvider.SetString(ppKey, newValue);
                        PlayerPrefsProvider.Save();
                        hasChanges = true;
                    }
                }
            }
            else if (valueType == ValueType.Int)
            {
                int oldValue = PlayerPrefsProvider.GetInt(ppKey);
                int newValue = EditorGUILayout.IntField(oldValue);
                if (newValue != oldValue)
                {
                    PlayerPrefsProvider.SetInt(ppKey, newValue);
                    PlayerPrefsProvider.Save();
                    hasChanges = true;
                }
            }
            else if (valueType == ValueType.Float)
            {
                float oldValue = PlayerPrefsProvider.GetFloat(ppKey);
                float newValue = EditorGUILayout.FloatField(oldValue);
                if (!Mathf.Approximately(oldValue, newValue))
                {
                    PlayerPrefsProvider.SetFloat(ppKey, newValue);
                    PlayerPrefsProvider.Save();
                    hasChanges = true;
                }
            }

            if (hasChanges)
                Refresh();

            if (GUILayout.Button("X", GUILayout.Width(50)))
            {
                PlayerPrefsProvider.DeleteKey(ppKey);
                PlayerPrefsProvider.Save();
                Refresh();
            }

            GUILayout.EndHorizontal();
        }

        private ValueType GetValueType(object value, string ppKey)
        {
            if (value is byte[] ||  //string on Windows
                value is string)    //string on MacOS
                return ValueType.String;
            else if (value is Single)
            {
                return ValueType.Float;
            }
            else if (value is int)
            {
                if (IsInt(ppKey))
                    return ValueType.Int;
                else //float on Windows
                    return ValueType.Float;
            }
            else if (value is double) //float on MacOS
                return ValueType.Float;

            throw new Exception("Not supported value type: " + value.GetType().Name);
        }

        private bool IsInt(string ppKey)
        {
            int intValue = PlayerPrefsProvider.GetInt(ppKey);
            if (intValue != 0)
                return true;

            float floatValue = PlayerPrefsProvider.GetFloat(ppKey);
            if (!Mathf.Approximately(floatValue, 0f))
                return false;

            return true;
        }

        private void DeleteEntity(PPEntity entity)
        {
            foreach (string ppKey in entity.PPKeys.Values)
                PlayerPrefsProvider.DeleteKey(ppKey);
        }

        #region Bottom Panel

        private void DrawBottomPanel()
        {
            GUILayout.BeginHorizontal("box");
            DrawProviderSelector();

            DrawVersion();
            DrawLatestVersionLink();
            GUILayout.Space(10f);
            DrawStatePanel();
            GUILayout.FlexibleSpace();
            DrawClearOnStartCheckbox();
            DrawClearTabButton();
            DrawClearAllButton();
            GUILayout.EndHorizontal();
        }


        private void DrawVersion()
        {
            GUILayout.Label("Version 1.0");
        }

        private void DrawLatestVersionLink()
        {
            if (GUILayout.Button("Latest version"))
            {
                Application.OpenURL("https://github.com/nubick/entity-to-playerprefs");
            }
        }

        private void DrawClearOnStartCheckbox()
        {
            bool oldValue = EditorPrefs.GetBool(ClearOnStartKey, false);
            bool newValue = GUILayout.Toggle(oldValue, "Clear on start in Editor");
            if (oldValue != newValue)
                EditorPrefs.SetBool(ClearOnStartKey, newValue);
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
                            PlayerPrefsProvider.DeleteKey(ppKey);
                    }
                    else
                    {
                        foreach (PPEntity entity in _entities.Where(_ => _.Type == _selectedType))
                            DeleteEntity(entity);
                        _selectedType = NotEntitiesTab;
                    }

                    PlayerPrefsProvider.Save();
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
                    PlayerPrefsProvider.DeleteAll();
                    PlayerPrefsProvider.Save();
                    Refresh();
                    _selectedType = NotEntitiesTab;
                }
            }
        }

        #endregion

        #region PlayerPrefsState Saving/Loading

        private Rect _createStateButtonRect;
        private Dictionary<string, Rect> _stateButtonPopupRects = new Dictionary<string, Rect>();

        private const string StatesFolderPath = "Assets/Plugins/EntityToPlayerPrefs/States/";

        private void DrawStatePanel()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create State"))
            {
                PopupWindow.Show(_createStateButtonRect, new InputNamePopup(name =>
                {
                    PlayerPrefsState state = CreateInstance<PlayerPrefsState>();
                    if (!Directory.Exists(StatesFolderPath))
                        Directory.CreateDirectory(StatesFolderPath);
                    string path = StatesFolderPath + name + ".asset";
                    AssetDatabase.CreateAsset(state, path);
                    AssetDatabase.SaveAssets();
                    SaveState(state);
                }));
            }

            if (Event.current.type == EventType.Repaint)
                _createStateButtonRect = GUILayoutUtility.GetLastRect();

            DrawStates();

            GUILayout.EndHorizontal();
        }

        private void DrawStates()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:PlayerPrefsState", null))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                PlayerPrefsState state = AssetDatabase.LoadAssetAtPath<PlayerPrefsState>(path);

                if (GUILayout.Button(state.name))
                {
                    StateActionsPopup stateActionsPopup = new StateActionsPopup();
                    stateActionsPopup.OnDelete(() =>
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(state));

                        string[] files = Directory.GetFiles(StatesFolderPath);
                        if (Directory.GetFiles(StatesFolderPath).Length == 0)
                            Directory.Delete(StatesFolderPath);
                        else
                            Debug.Log(files[0]);

                        AssetDatabase.Refresh();
                    });
                    stateActionsPopup.OnLoad(() =>
                    {
                        LoadState(state);
                        Refresh();
                    });
                    stateActionsPopup.OnSave(() =>
                    {
                        SaveState(state);
                    });
                    PopupWindow.Show(_stateButtonPopupRects[state.name], stateActionsPopup);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    if (!_stateButtonPopupRects.ContainsKey(state.name))
                        _stateButtonPopupRects.Add(state.name, new Rect());
                    _stateButtonPopupRects[state.name] = GUILayoutUtility.GetLastRect();
                }
            }
        }

        private void SaveState(PlayerPrefsState stateStorage)
        {
            stateStorage.PPKeys.Clear();
            stateStorage.ValueTypes.Clear();
            stateStorage.StringValues.Clear();
            foreach (PPEntity entity in _entities)
            {
                foreach (string key in entity.PPKeys.Keys)
                {
                    string ppKey = entity.PPKeys[key];
                    stateStorage.PPKeys.Add(ppKey);

                    ValueType valueType = GetValueType(entity.Values[key], ppKey);
                    stateStorage.ValueTypes.Add(valueType);

                    string valueAsString = GetValueAsString(ppKey, valueType);
                    stateStorage.StringValues.Add(valueAsString);
                }
            }
            EditorUtility.SetDirty(stateStorage);
        }

        private string GetValueAsString(string ppKey, ValueType valueType)
        {
            if (valueType == ValueType.String)
                return PlayerPrefsProvider.GetString(ppKey);
            else if (valueType == ValueType.Int)
                return PlayerPrefsProvider.GetInt(ppKey).ToString();
            else if (valueType == ValueType.Float)
                return PlayerPrefsProvider.GetFloat(ppKey).ToString();

            throw new Exception("Not supported recordType: " + valueType);
        }

        private void LoadState(PlayerPrefsState stateStorage)
        {
            PlayerPrefsProvider.DeleteAll();
            for (int i = 0; i < stateStorage.PPKeys.Count; i++)
            {
                ValueType valueType = stateStorage.ValueTypes[i];
                string ppKey = stateStorage.PPKeys[i];
                string stringValue = stateStorage.StringValues[i];

                if (valueType == ValueType.String)
                {
                    PlayerPrefsProvider.SetString(ppKey, stringValue);
                }
                else if (valueType == ValueType.Int)
                {
                    int intValue = int.Parse(stringValue);
                    PlayerPrefsProvider.SetInt(ppKey, intValue);
                }
                else if (valueType == ValueType.Float)
                {
                    float floatValue = float.Parse(stringValue);
                    PlayerPrefsProvider.SetFloat(ppKey, floatValue);
                }
            }
            PlayerPrefsProvider.Save();
        }

        #endregion
    }

    internal class InputNamePopup : PopupWindowContent
    {
        private string _name;
        private Action<string> _finishAction;

        public InputNamePopup(Action<string> finishAction)
        {
            _finishAction = finishAction;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Input name of ScriptableObject. New file with this name will be created in the plugin directory. This file will be used as storage for state. You will be able to save/load state to/from this file.", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Input name");
            _name = GUILayout.TextField(_name);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create"))
            {
                this.editorWindow.Close();
                _finishAction(_name);
            }

            GUILayout.EndVertical();
        }
    }
    internal class ProviderSelector : PopupWindowContent
    {
        public EntityToPlayerPrefsEditor editor;
        public override void OnGUI(Rect rect)
        {
            using (var vertical = new EditorGUILayout.VerticalScope())
            {
                foreach (var item in editor.allProvider)
                {
                    if (GUILayout.Button(item.Key.Split('.').LastOrDefault()))
                    {
                        // This is not the best solution but won't be a big problem.
                        // But will be better if the IPlayerPrefsProvider instance is cached
                        // TODO : pooling the each IPlayerPrefsProvider instance
                        PlayerPrefsProvider.SetProvider((IPlayerPrefsProvider)Activator.CreateInstance(item.Value));
                        editor.Refresh();
                        editorWindow.Close();
                    }
                }
            }
        }
    }
    internal class StateActionsPopup : PopupWindowContent
    {
        private Action _onDeleteAction;
        private Action _onLoadAction;
        private Action _onSaveAction;

        public void OnDelete(Action onDeleteAction)
        {
            _onDeleteAction = onDeleteAction;
        }

        public void OnLoad(Action onLoadAction)
        {
            _onLoadAction = onLoadAction;
        }

        public void OnSave(Action onSaveAction)
        {
            _onSaveAction = onSaveAction;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Load state"))
            {
                editorWindow.Close();
                _onLoadAction();
            }

            if (GUILayout.Button("Save state"))
            {
                editorWindow.Close();
                _onSaveAction();
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Delete"))
            {
                editorWindow.Close();
                _onDeleteAction();
            }

            GUILayout.EndVertical();
        }
    }

    [InitializeOnLoad]
    public static class EditorPlayModeCleaner
    {
        static EditorPlayModeCleaner()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange playModeStateChange)
        {
            bool shouldBeCleared = EditorPrefs.GetBool(EntityToPlayerPrefsEditor.ClearOnStartKey, false);
            if (shouldBeCleared && playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                Debug.Log("Entity to PlayerPrefs. Editor mode settings: clear All on start.");
                PlayerPrefsProvider.DeleteAll();
                PlayerPrefsProvider.Save();
            }
        }
    }
}