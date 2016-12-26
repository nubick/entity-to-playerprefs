using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class FloatFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            return PlayerPrefs.GetFloat(fieldKey);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefs.SetFloat(fieldKey, dataMemberInfo.GetValue<float>(entity));
        }
    }
}
