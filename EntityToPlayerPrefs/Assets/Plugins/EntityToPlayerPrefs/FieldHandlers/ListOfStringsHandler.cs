using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
	//todo: It doesn't work if any string has ';' char.
	public class ListOfStringsHandler : PlayerPrefsFieldHandler
	{
		private const string ValuePrefix = "__ls.";

		public override object GetValue(string fieldKey)
		{
			return GetListOfStrings(fieldKey);
		}

		private static List<string> GetListOfStrings(string fieldKey)
		{
			//Format: __ls.23;43;53
			string valueString = PlayerPrefs.GetString(fieldKey, ValuePrefix);
			valueString = valueString.Substring(5);//remove prefix
			return string.IsNullOrEmpty(valueString) ? new List<string>() : valueString.Split(';').ToList();
		}

		public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
		{
			List<string> list = dataMemberInfo.GetValue<List<string>>(entity);

			if (list == null || list.Count == 0)
			{
				PlayerPrefs.DeleteKey(fieldKey);
			}
			else
			{
				string listStringValue = $"{ValuePrefix}{string.Join(";", list)}";
				PlayerPrefs.SetString(fieldKey, listStringValue);
			}
		}
	}
}