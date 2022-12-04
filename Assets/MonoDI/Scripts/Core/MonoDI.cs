using System.Collections.Generic;
using UnityEngine;

namespace MonoDI.Scripts.Core
{
    public sealed class MonoDI : MonoBehaviour
    {
        private static MonoDI _instance;
        private readonly List<System.Object> _toInject = new List<System.Object>();
        private readonly GeneralInstaller _generalInstaller = new GeneralInstaller();
        private SignalBus _signal = new SignalBus();
        
        private bool _isInjecting;
        private readonly List<InjectedMono> _waitForInject = new List<InjectedMono>();
        
        public static MonoDI Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<MonoDI>();
                if (_instance != null)
                {
                    _instance.Init();
                }
                return _instance;
            }
        }
        private void Init()
        {
            var r = gameObject.GetComponentsInChildren<MonoBehaviour>();
            foreach (var behaviour in r)
            {
                _toInject.Add(behaviour);
            }

            var systems = gameObject.GetComponents<IInitSystem>();
            foreach (var system in systems)
            {
                system.OnInit();
            }
            
            _toInject.Add(_signal);
        }



        /// <summary>
        /// Add Injection to a specific object
        /// </summary>
        /// <param name="injectedMono"></param>
        public void FixDependencies(InjectedMono injectedMono)
        {
            if (_isInjecting == false)
            {
                _isInjecting = true;
                _generalInstaller.Inject(injectedMono, _toInject);
                _generalInstaller.SubscribeSignals(_signal, injectedMono);
                injectedMono.OnSyncStart();
                injectedMono.OnSyncStartInternal();
                _isInjecting = false;
                if (_waitForInject.Count > 0)
                {
                    var first = _waitForInject[0];
                    _waitForInject.RemoveAt(0);
                    FixDependencies(first);
                }
            }
            else
            {
                _waitForInject.Add(injectedMono);
            }

        }
        
        /// <summary>
        /// Clear specific object
        /// </summary>
        public void ClearAll()
        {
            _signal.Clear();
        }

        public void ClearObject(InjectedMono injectedMono)
        {
            _generalInstaller.UnSubscribeSignals(_signal, injectedMono);
        }
    }

    public interface IInitSystem
    {
        void OnInit();
    }
}
