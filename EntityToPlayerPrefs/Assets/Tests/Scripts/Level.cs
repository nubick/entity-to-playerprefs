using UnityEngine;
using Assets.Plugins.EntityToPlayerPrefs;

namespace Assets.Tests.Scripts
{
    [PlayerPrefsEntityName("NewLevel")]
    public class Level : MonoBehaviour
    {
        [PlayerPrefsEntityId] public string Id => Number.ToString();
        public int Number;
        [PlayerPrefsField] public bool IsCompleted;
        [PlayerPrefsField] public int Stars;

        public void Complete(int stars)
        {
            Stars = stars;
            IsCompleted = true;
            PlayerPrefsMapper.Save(this);
            Debug.Log($"Level '{Number}' is completed with stars '{Stars}'.");
        }
    }
}