using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class DateTimeFieldHandler : PlayerPrefsFieldHandler
    {
        private const string ValuePrefix = "__dt.";

        public override object GetValue(string fieldKey)
        {
            return GetDateTime(fieldKey);
        }

        private static DateTime GetDateTime(string fieldKey)
        {
            string valueString = PlayerPrefs.GetString(fieldKey);
            string binaryString;
            if (valueString.StartsWith(ValuePrefix))
                binaryString = valueString.Substring(5);
            else //version 0.1 format without prefix
                binaryString = valueString;

            DateTime dateTime = string.IsNullOrEmpty(binaryString)
                ? new DateTime()
                : DateTime.FromBinary(long.Parse(binaryString));
            return dateTime;
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            DateTime dateTime = dataMemberInfo.GetValue<DateTime>(entity);
            PlayerPrefs.SetString(fieldKey, ToBinaryString(dateTime));
        }

        private static string ToBinaryString(DateTime dateTime)
        {
            return ValuePrefix + dateTime.ToBinary();
        }

#if UNITY_EDITOR

        public static bool IsDateTimeRecord(string fieldKey)
        {
            string stringValue = PlayerPrefs.GetString(fieldKey);
            return stringValue.StartsWith(ValuePrefix);
        }

        public static bool DrawEditor(string fieldKey)
        {
            string dateTimeString = GetDateTime(fieldKey).ToString();
            string newDateTimeString = EditorGUILayout.TextField(dateTimeString);
            if (newDateTimeString != dateTimeString)
            {
                DateTime newDateTime;
                if (DateTime.TryParse(newDateTimeString, out newDateTime))
                {
                    PlayerPrefs.SetString(fieldKey, ToBinaryString(newDateTime));
                    PlayerPrefs.Save();
                    return true;
                }
            }
            return false;
        }
#endif

    }
}
