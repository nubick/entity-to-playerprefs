
namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public abstract class PlayerPrefsFieldHandler
    {
        public abstract object GetValue(string fieldKey);
        public abstract void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity);
    }
}
