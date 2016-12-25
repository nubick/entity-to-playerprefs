using System;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PlayerPrefsFieldAttribute : Attribute
    {
    }
}
