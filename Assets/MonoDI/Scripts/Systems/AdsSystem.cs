using System;
using System.Collections;
using MonoDI.Scripts.Core;
using UnityEngine;

namespace MonoDI.Scripts.Systems
{
    public class AdsSystem : MonoBehaviour, IInitSystem
    {
        private static IAdsProvider _adsProvider;
        
        public void OnInit()
        {
            if (_adsProvider == null)
            {
                Init();
            }
        }

        protected virtual void Init()
        {
            //here you need to add new provider
            _adsProvider = new FakeAdsProvider();
            _adsProvider.Init();
        }

        public bool IsAvailable()
        {
            return _adsProvider.IsAvailable();
        }

        private Coroutine _current;

        public void ShowAds(System.Action<bool> onFinish)
        {
            StopCoroutine(_current);
            _current = StartCoroutine(ShowAdsRoutine(onFinish));
        }

        private IEnumerator ShowAdsRoutine(System.Action<bool> onFinish)
        {
            if (IsAvailable() == false)
            {
                onFinish.Invoke(false);
                yield break;
            }

            var waiting = true;
            var result = false;
            
            _adsProvider.ShowAds(success =>
            {
                waiting = false;
                result = success;
                // potential crash because of multithreading....
                // maybe there is a more elegant way to do it without UniTask
                // Please send PR
            });

            while (waiting)
            {
                yield return null;
            }
            
            onFinish?.Invoke(result);
        }


    }

    public class FakeAdsProvider : IAdsProvider
    {
        public void Init()
        {
            
        }

        public bool IsAvailable()
        {
            return true;
        }

        public void ShowAds(Action<bool> onFinish)
        {
            onFinish.Invoke(true);
        }
    }


    public interface IAdsProvider
    {
        void Init();
        bool IsAvailable();
        void ShowAds(System.Action<bool> onFinish);
    }
}
