using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Tests.Scripts
{
    public class TestManager : MonoBehaviour
    {
        public GameState GameState;

        public void FillGameStateWithRandomValues(GameState gameState)
        {
            gameState.FirstName = GetRandomString();
            gameState.LastName = GetRandomString();

            gameState.HasSubscription = Random.Range(0,2) == 0;
            gameState.WasGameRated = Random.Range(0, 2) == 0;

            gameState.Lifes = Random.Range(int.MinValue, int.MaxValue);
            gameState.RatedStars = Random.Range(int.MinValue, int.MaxValue);

            gameState.MusicVolume = Random.Range(float.MinValue, float.MaxValue);
            gameState.SoundsVolume = Random.Range(0f, 1f);

            gameState.SubscriptionEndDate = GetRandomDateTime();
            gameState.LastDailyRewardDate = GetRandomDateTime();
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
    }
}
