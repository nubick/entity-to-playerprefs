using System;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayerPrefsEntityNameAttribute : Attribute
    {
        public string Name { get; }
        
        public PlayerPrefsEntityNameAttribute(string name)
        {
            Name = name;
        }
    }
}