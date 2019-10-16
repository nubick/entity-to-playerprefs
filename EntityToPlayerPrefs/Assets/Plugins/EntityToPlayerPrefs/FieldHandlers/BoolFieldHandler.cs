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
            string valueString = PlayerPrefsProvider.GetString(fieldKey);

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
            PlayerPrefsProvider.SetString(fieldKey, ValuePrefix + boolValue);
        }

#if UNITY_EDITOR
        public static bool IsBoolRecord(string fieldKey)
        {
            return PlayerPrefsProvider.GetString(fieldKey).StartsWith(ValuePrefix);
        }

        public static bool DrawEditor(string fieldKey)
        {
            bool boolValue = GetBool(fieldKey);
            bool newBoolValue = UnityEditor.EditorGUILayout.Toggle(boolValue);
            if (newBoolValue != boolValue)
            {
                SetBoolValue(fieldKey, newBoolValue);
                PlayerPrefsProvider.Save();
                return true;
            }
            return false;
        }
#endif
    }
}
