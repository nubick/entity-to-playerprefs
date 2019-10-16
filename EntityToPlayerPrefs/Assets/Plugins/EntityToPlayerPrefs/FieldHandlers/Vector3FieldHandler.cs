using System.Globalization;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class Vector3FieldHandler : PlayerPrefsFieldHandler
    {
        private const string ValuePrefix = "__v3.";

        public override object GetValue(string fieldKey)
        {
            return GetVector3(fieldKey);
        }

        private static Vector3 GetVector3(string fieldKey)
        {
            //Format: __v3.2345.432;234.532;2.23
            string valueString = PlayerPrefsProvider.GetString(fieldKey);
            valueString = valueString.Substring(5);//remove prefix
            string[] coordinates = valueString.Split(';');
            float x = float.Parse(coordinates[0], CultureInfo.InvariantCulture.NumberFormat);
            float y = float.Parse(coordinates[1], CultureInfo.InvariantCulture.NumberFormat);
            float z = float.Parse(coordinates[2], CultureInfo.InvariantCulture.NumberFormat);
            return new Vector3(x, y, z);
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            Vector3 vector3 = dataMemberInfo.GetValue<Vector3>(entity);
            string vector3String = string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0}{1};{2};{3}", ValuePrefix, vector3.x, vector3.y, vector3.z);
            PlayerPrefsProvider.SetString(fieldKey, vector3String);
        }
    }
}