using System;
using System.Collections.Generic;
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

        [PlayerPrefsField] public GameMode GameMode;
        [PlayerPrefsField] public PurchaseMode PurchaseMode { get; set; }

        [PlayerPrefsField] public List<int> ItemIds;
        [PlayerPrefsField] public List<int> CharacterIds { get; set; }

        [PlayerPrefsField] public List<string> Names;
        [PlayerPrefsField] public List<string> Titles { get; set; }

        [PlayerPrefsField] public Vector2 Center;
        [PlayerPrefsField] public Vector2 TopLeft { get; set; }

        [PlayerPrefsField] public Vector3 Point;
        [PlayerPrefsField] public Vector3 Position { get; set; }
        
        public void Awake()
        {
            PlayerPrefsProvider.SetProvider(new FilePlayerPrefsProvider());
            PlayerPrefsMapper.Load(this);
            Debug.Log("SubscriptionEndDate: " + SubscriptionEndDate);
            Debug.Log("LastDailyRewardDate: " + LastDailyRewardDate);
            Debug.Log("GameState successfully loaded.");
        }

        public void Save()
        {
            PlayerPrefsMapper.Save(this);
            Debug.Log("GameState successfully saved.");
        }
    }

    public enum GameMode
    {
        Tutorial,
        FreePlay,
        PvP,
        Tournament
    }

    public enum PurchaseMode
    {
        ShowAds,
        MonthlySubscription,
        VipMode
    }
}
