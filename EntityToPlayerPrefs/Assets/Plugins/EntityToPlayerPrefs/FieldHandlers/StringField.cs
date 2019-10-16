namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    class StringFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            return PlayerPrefsProvider.GetString(fieldKey);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            PlayerPrefsProvider.SetString(fieldKey, dataMemberInfo.GetValue<string>(entity));
        }
    }
}
