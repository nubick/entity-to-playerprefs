using System;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class EnumFieldHandler : PlayerPrefsFieldHandler
    {
        private Type _enumType;

        public EnumFieldHandler(Type enumType)
        {
            _enumType = enumType;
        }

        public override object GetValue(string fieldKey)
        {
            string enumString = PlayerPrefs.GetString(fieldKey);
            object enumValue = Enum.Parse(_enumType, enumString);
            return enumValue;
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            Enum enumValue = dataMemberInfo.GetValue<Enum>(entity);
            PlayerPrefs.SetString(fieldKey, enumValue.ToString());
        }
    }
}

