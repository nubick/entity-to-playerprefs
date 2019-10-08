using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.Plugins.EntityToPlayerPrefs;

namespace Assets.Tests.Scripts
{
    public class TestManager : MonoBehaviour
    {
        public GameState GameState;
        public Level[] Levels;

        public void Awake()
        {
            PlayerPrefsMapper.Load(Levels);
        }

        public void FillGameStateWithRandomValues(GameState gameState)
        {
            gameState.FirstName = GetRandomString();
            gameState.LastName = GetRandomString();

            gameState.HasSubscription = Random.Range(0, 2) == 0;
            gameState.WasGameRated = Random.Range(0, 2) == 0;

            gameState.Lifes = Random.Range(int.MinValue, int.MaxValue);
            gameState.RatedStars = Random.Range(int.MinValue, int.MaxValue);

            gameState.MusicVolume = Random.Range(float.MinValue, float.MaxValue);
            gameState.SoundsVolume = Random.Range(0f, 1f);

            gameState.SubscriptionEndDate = GetRandomDateTime();
            gameState.LastDailyRewardDate = GetRandomDateTime();

            gameState.GameMode = GetRandomEnum<GameMode>();
            gameState.PurchaseMode = GetRandomEnum<PurchaseMode>();

            gameState.ItemIds = new List<int>();
            for (int i = 0; i < Random.Range(1, 100); i++)
                gameState.ItemIds.Add(Random.Range(1, 100));

            gameState.CharacterIds = new List<int>();
            for (int i = 0; i < Random.Range(1, 100); i++)
                gameState.CharacterIds.Add(Random.Range(1, 100));

            gameState.Names = new List<string>();
            for (int i = 0; i < Random.Range(1, 100); i++)
                gameState.Names.Add(GetRandomString());

            gameState.Titles = new List<string>();
            for (int i = 0; i < Random.Range(1, 100); i++)
                gameState.Titles.Add(GetRandomString());

            gameState.Center = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            gameState.TopLeft = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));

            gameState.Point = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            gameState.Position = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        }

        public void SaveRandomGameState()
        {
            FillGameStateWithRandomValues(GameState);
            GameState.Save();
        }

        private string GetRandomString()
        {
            string[] values =
            {
                "adlkf",
                "nubick",
                "",
                null,
                "daldkfjoeijfowijeroewijfe\ndalkfjalfjasfk\tadlfkjdlajfladffdlkfjlsdjfslfjwo109231093810831031",
                "1",
                "1234567890"
            };
            return values[Random.Range(0, values.Length)];
        }

        private DateTime GetRandomDateTime()
        {
            long minTicks = DateTime.MinValue.Ticks;
            long maxTicks = DateTime.MaxValue.Ticks;
            long randomTicks = (long) Random.Range(minTicks, maxTicks);
            return new DateTime(randomTicks);
        }

        private T GetRandomEnum<T>() where T : struct, IComparable
        {
            Type enumType = typeof(T);
            if(!enumType.IsEnum)
                throw new Exception("T must be an enumerated type.");
            Array array = Enum.GetValues(enumType);
            return (T)array.GetValue(Random.Range(0, array.GetLength(0)));
        }
    
        public void CompleteRandomLevel()
        {
            Level level = Levels[Random.Range(0, Levels.Length)];
            level.Complete(Random.Range(0,3) + 1);
        }

        public void DeleteProperty()
        {
            PlayerPrefsMapper.Delete(GameState, _ => _.LastName);
            PlayerPrefsMapper.Delete(GameState, _ => _.FirstName);
            PlayerPrefsMapper.Delete(Levels[0], _ => _.IsCompleted);
        }

        public void HasKey()
        {
            bool hasLastName = PlayerPrefsMapper.HasKey(GameState, _ => _.LastName);
            Debug.Log("Has last name: " + hasLastName);
        }
    }
}
