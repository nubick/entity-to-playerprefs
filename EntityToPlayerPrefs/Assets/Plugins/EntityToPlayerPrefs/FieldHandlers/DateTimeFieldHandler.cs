using System;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class DateTimeFieldHandler : PlayerPrefsFieldHandler
    {
        public override object GetValue(string fieldKey)
        {
            string binaryString = PlayerPrefs.GetString(fieldKey);
            DateTime dateTime = string.IsNullOrEmpty(binaryString)
                ? new DateTime()
                : DateTime.FromBinary(long.Parse(binaryString));
            return dateTime;
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            DateTime dateTime = dataMemberInfo.GetValue<DateTime>(entity);
            string binaryString = dateTime.ToBinary().ToString();
            PlayerPrefs.SetString(fieldKey, binaryString);
        }
    }
}
