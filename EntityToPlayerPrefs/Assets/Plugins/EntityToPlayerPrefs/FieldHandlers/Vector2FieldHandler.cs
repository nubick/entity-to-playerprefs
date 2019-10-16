using System.Globalization;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
	public class Vector2FieldHandler : PlayerPrefsFieldHandler
	{
		private const string ValuePrefix = "__v2.";

		public override object GetValue(string fieldKey)
		{
			return GetVector2(fieldKey);
		}

		private static Vector2 GetVector2(string fieldKey)
		{
			//Format: __v2.2345.432;234.532
			string valueString = PlayerPrefsProvider.GetString(fieldKey);
			valueString = valueString.Substring(5);//remove prefix
			string[] coordinates = valueString.Split(';');
			float x = float.Parse(coordinates[0], CultureInfo.InvariantCulture.NumberFormat);
			float y = float.Parse(coordinates[1], CultureInfo.InvariantCulture.NumberFormat);
			return new Vector2(x, y);
		}

		public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
		{
			Vector2 vector2 = dataMemberInfo.GetValue<Vector2>(entity);
			string vector2String = string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0}{1};{2}", ValuePrefix, vector2.x, vector2.y);
			PlayerPrefsProvider.SetString(fieldKey, vector2String);
		}
	}
}