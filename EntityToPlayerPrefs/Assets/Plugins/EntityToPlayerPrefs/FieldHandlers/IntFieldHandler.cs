using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class IntFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            return PlayerPrefsProvider.GetInt(fieldKey);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefsProvider.SetInt(fieldKey, dataMemberInfo.GetValue<int>(entity));
        }
    }
}
