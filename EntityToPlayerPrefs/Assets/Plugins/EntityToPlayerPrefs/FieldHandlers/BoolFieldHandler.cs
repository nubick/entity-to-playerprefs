using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class BoolFieldHandler : PlayerPrefsFieldHandler 
    {
        public override object GetValue(string fieldKey)
        {
            return bool.Parse(PlayerPrefs.GetString(fieldKey));
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefs.SetString(fieldKey, dataMemberInfo.GetValue<bool>(entity).ToString());
        }
    }
}
