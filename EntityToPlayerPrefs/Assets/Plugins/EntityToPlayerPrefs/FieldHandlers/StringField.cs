using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    class StringFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            return PlayerPrefs.GetString(fieldKey);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefs.SetString(fieldKey, dataMemberInfo.GetValue<string>(entity));
        }
    }
}
