using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Plugins.EntityToPlayerPrefs;

namespace Assets.Tests.Scripts
{
    public class Level : MonoBehaviour
    {
        [PlayerPrefsEntityId] public string Id { get { return Number.ToString(); } }
        [PlayerPrefsField] public int Number;
        [PlayerPrefsField] public bool IsCompleted;
        [PlayerPrefsField] public int Stars;

        public void Complete(int stars)
        {
            Stars = stars;
            IsCompleted = true;
            PlayerPrefsMapper.Save(this);
            Debug.Log(string.Format("Level '{0}' is completed with stars '{1}'.", Number, Stars));
        }
    }
}