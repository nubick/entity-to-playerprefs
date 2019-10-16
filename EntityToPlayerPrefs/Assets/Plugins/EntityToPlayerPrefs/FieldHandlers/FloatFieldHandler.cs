namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class FloatFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            return PlayerPrefsProvider.GetFloat(fieldKey);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefsProvider.SetFloat(fieldKey, dataMemberInfo.GetValue<float>(entity));
        }
    }
}
