using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class BoolFieldHandler : PlayerPrefsFieldHandler
    {
        private const string ValuePrefix = "__bool.";

        public override object GetValue(string fieldKey)
        {
            return GetBool(fieldKey);
        }

        private static bool GetBool(string fieldKey)
        {
            string valueString = PlayerPrefs.GetString(fieldKey);

            string boolString;
            if (valueString.StartsWith(ValuePrefix))
                boolString = valueString.Substring(ValuePrefix.Length);
            else //version 0.1 format without prefix
                boolString = valueString;

            return bool.Parse(boolString);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            SetBoolValue(fieldKey, dataMemberInfo.GetValue<bool>(entity));
        }

        private static void SetBoolValue(string fieldKey, bool boolValue)
        {
            PlayerPrefs.SetString(fieldKey, ValuePrefix + boolValue);
        }

#if UNITY_EDITOR

        public static bool IsBoolRecord(string fieldKey)
        {
            return PlayerPrefs.GetString(fieldKey).StartsWith(ValuePrefix);
        }

        public static bool DrawEditor(string fieldKey)
        {
            bool boolValue = GetBool(fieldKey);
            bool newBoolValue = EditorGUILayout.Toggle(boolValue);
            if (newBoolValue != boolValue)
            {
                SetBoolValue(fieldKey, newBoolValue);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }
#endif
    }
}
