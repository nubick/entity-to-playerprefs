using System.Collections.Generic;
using System.Linq;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
	public class ListOfIntsHandler : PlayerPrefsFieldHandler
	{
		private const string ValuePrefix = "__li.";

		public override object GetValue(string fieldKey)
		{
			return GetListOfInts(fieldKey);
		}

		private static List<int> GetListOfInts(string fieldKey)
		{
			//Format: __li.23;43;53
			string valueString = PlayerPrefsProvider.GetString(fieldKey, ValuePrefix);
			valueString = valueString.Substring(5);//remove prefix
			return string.IsNullOrEmpty(valueString) ? new List<int>() : valueString.Split(';').Select(int.Parse).ToList();
		}

		public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
		{
			List<int> list = dataMemberInfo.GetValue<List<int>>(entity);
			
			if (list == null || list.Count == 0)
			{
				PlayerPrefsProvider.DeleteKey(fieldKey);
			}
			else
			{
				string listStringValue = $"{ValuePrefix}{string.Join(";", list)}";
				PlayerPrefsProvider.SetString(fieldKey, listStringValue);
			}
		}
	}
}