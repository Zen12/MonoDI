using System;
using MonoDI.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace MonoDI.Scripts.Systems
{
    public class SceneProgressionSystem : InjectedMono
    {
        [In] private AnalyticsSystem _analytics;
        public int CurrentScene
        {
            get => PlayerPrefs.GetInt("CurrentScene", 1);
            private set => PlayerPrefs.SetInt("CurrentScene", value);
        }
        
        public int TotalFinishedScenes
        {
            get => PlayerPrefs.GetInt("TotalFinishedScenes", 1);
            private set => PlayerPrefs.SetInt("TotalFinishedScenes", value);
        }

        private int LastLoadedScene
        {
            get => PlayerPrefs.GetInt("LastLoadedRandom", -1);
            set => PlayerPrefs.SetInt("LastLoadedRandom", value);
        }

        private DateTime _startLevel;

        public void ReloadScene()
        {
            if (CurrentScene >= SceneManager.sceneCountInBuildSettings)
            {
                if (LastLoadedScene > 0)
                {
                    CurrentScene = LastLoadedScene;
                }
                else
                {
                    CurrentScene = SceneManager.sceneCountInBuildSettings - 1;
                }
            }
            
            _analytics.Restart(TotalFinishedScenes, (int) (DateTime.Now - _startLevel).TotalSeconds);
            
            SceneManager.LoadScene(CurrentScene);
        }

        public override void OnSyncAfterStart()
        {
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                _analytics.LevelStart(TotalFinishedScenes);
                _startLevel = DateTime.Now;
            }
        }

        public void LoadNextSceneFromFirstScene()
        {
            if (CurrentScene >= SceneManager.sceneCountInBuildSettings)
            {
                if (LastLoadedScene > 0)
                {
                    CurrentScene = LastLoadedScene;
                }
                else
                {
                    CurrentScene = SceneManager.sceneCountInBuildSettings - 1;
                }
                
            }

            if (CurrentScene < 1)
            {
                CurrentScene = 1;
            }
            
            SceneManager.LoadScene(CurrentScene);
        }
        
        public void LoadNextScene(bool isFinalRandom = false)
        {
            _analytics.LevelFinish(TotalFinishedScenes, (int) (DateTime.Now - _startLevel).TotalSeconds);
            TotalFinishedScenes += 1;
            var nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (CurrentScene < nextLevel)
            {
                CurrentScene = nextLevel;
            }
            
            if (CurrentScene >= SceneManager.sceneCountInBuildSettings)
            {
                if (isFinalRandom)
                {
                    //if there are only 1 levels
                    if (SceneManager.sceneCountInBuildSettings == 2)
                    {
                        SceneManager.LoadScene(1);
                        return;
                    }
                    //first level is usually tutorial
                    if (SceneManager.sceneCountInBuildSettings >= 2)
                    {
                        var r = Random.Range(2, SceneManager.sceneCountInBuildSettings);
                        LastLoadedScene = r;
                        SceneManager.LoadScene(r);
                    }
                    else
                    {
                        SceneManager.LoadScene(2);
                    }
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
                }
            }
            else
            {
                SceneManager.LoadScene(CurrentScene);
            }
        }
    }
}
