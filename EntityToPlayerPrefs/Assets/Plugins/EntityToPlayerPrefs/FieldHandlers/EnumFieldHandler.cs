using System;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class EnumFieldHandler : PlayerPrefsFieldHandler
    {
        private readonly Type _enumType;

        public EnumFieldHandler(Type enumType)
        {
            _enumType = enumType;
        }

        public override object GetValue(string fieldKey)
        {
            string enumString = PlayerPrefsProvider.GetString(fieldKey);
            object enumValue = Enum.Parse(_enumType, enumString);
            return enumValue;
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            Enum enumValue = dataMemberInfo.GetValue<Enum>(entity);
            PlayerPrefsProvider.SetString(fieldKey, enumValue.ToString());
        }
    }
}

