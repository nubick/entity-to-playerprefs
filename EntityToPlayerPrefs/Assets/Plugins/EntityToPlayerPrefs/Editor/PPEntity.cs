
using System.Collections.Generic;
using System.Linq;

namespace Assets.Plugins.EntityToPlayerPrefs.Editor
{
    public class PPEntity
    {
        public string Type { get; private set; }
        public string Id { get; private set; }
        public Dictionary<string, object> Values { get; private set; }
        public Dictionary<string, string> PPKeys { get; private set; }

        public PPEntity(string type, string id)
        {
            Type = type;
            Id = id;
            Values = new Dictionary<string, object>();
            PPKeys = new Dictionary<string, string>();
        }

        public void AddValue(string key, object value, string ppKey)
        {
            Values.Add(key, value);
            PPKeys.Add(key, ppKey);
        }

        public List<string> GetSortedKeys()
        {
            List<string> keys = Values.Keys.ToList();
            keys.Sort();
            return keys;
        }
    }
}
