using System;
using Assets.Plugins.EntityToPlayerPrefs;
using UnityEngine;

namespace Assets.Tests.Scripts
{
    public class GameState : MonoBehaviour
    {
        [PlayerPrefsField] public string FirstName;
        [PlayerPrefsField] public string LastName { get; set; }

        [PlayerPrefsField] public bool HasSubscription;
        [PlayerPrefsField] public bool WasGameRated { get; set; }

        [PlayerPrefsField] public int Lifes;
        [PlayerPrefsField] public int RatedStars { get; set; }

        [PlayerPrefsField] public float MusicVolume;
        [PlayerPrefsField] public float SoundsVolume { get; set; }

        [PlayerPrefsField] public DateTime SubscriptionEndDate;
        [PlayerPrefsField] public DateTime LastDailyRewardDate { get; set; }

        public void Awake()
        {
            PlayerPrefsMapper.Load(this);
            Debug.Log("GameState succesffully loaded.");
        }

        public void Save()
        {
            PlayerPrefsMapper.Save(this);
            Debug.Log("GameState sucessfully saved.");
        }
    }
}
