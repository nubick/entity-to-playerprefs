using System.Collections.Generic;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public class PlayerPrefsState : ScriptableObject
    {
        public List<string> PPKeys;
        public List<ValueType> ValueTypes;
        public List<string> StringValues;

        public PlayerPrefsState()
        {
            PPKeys = new List<string>();
            ValueTypes = new List<ValueType>();
            StringValues = new List<string>();
        }
    }

    public enum ValueType
    {
        String,
        Int,
        Float
    }
}