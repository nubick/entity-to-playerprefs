using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Assets.Plugins.EntityToPlayerPrefs;

namespace Assets.Tests.Scripts
{
    public class PerformanceTestManager : MonoBehaviour 
    {
        public CanvasGroup InteractableCanvas;
        public GameState GameState;
        public Text OperationsCount;
        public Text TotalSaveTime;
        public Text TotalLoadTime;
        public Text Platform;
        public int TotalCount;

        private int _count;
        private float _totalSaveTime;
        private float _totalLoadTime;

        public void Awake()
        {
            Platform.text = "Platform: " + Application.platform.ToString();;
        }

        public void StartPerformanceTest()
        {
            _count = 0;
            _totalSaveTime = 0f;
            _totalLoadTime = 0f;
            StartCoroutine(TestCoroutine());
        }

        private IEnumerator TestCoroutine()
        {
            InteractableCanvas.interactable = false;
            for(;;)
            {
                _count++;
                OperationsCount.text = string.Format("Operations count: {0}/{1}", _count, TotalCount);

                //save
                float start = Time.realtimeSinceStartup;
                PlayerPrefsMapper.Save(GameState);
                float finish = Time.realtimeSinceStartup;

                _totalSaveTime += finish - start;
                TotalSaveTime.text = "Total save time: " + _totalSaveTime;

                //load
                start = Time.realtimeSinceStartup;
                PlayerPrefsMapper.Load(GameState);
                finish = Time.realtimeSinceStartup;

                _totalLoadTime += finish - start;
                TotalLoadTime.text = "Total load time: " + _totalLoadTime;

                yield return null;

                if(_count == TotalCount)
                    break;
            }
            InteractableCanvas.interactable = true;
        }
    }
}
