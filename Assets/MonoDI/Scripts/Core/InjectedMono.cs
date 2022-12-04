using System.Collections;
using UnityEngine;

namespace MonoDI.Scripts.Core
{
    public abstract class InjectedMono : MonoBehaviour
    {
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                Debug.LogWarning("No support for ExecuteInEditMode");
                return;
            }
#endif
            MonoDI.Instance.FixDependencies(this);
        }

        protected virtual void OnDestroy()
        {
            if (MonoDI.Instance != null)
                MonoDI.Instance.ClearObject(this);
        }

        public virtual void OnSyncStart()
        {
            
        }

        public virtual void OnSyncAfterStart()
        {
            
        }

        public void OnSyncStartInternal()
        {
            OnSyncAfterStart();
            if (gameObject.activeInHierarchy == false)
                return;
            StartCoroutine(OnSyncLastCallRoutine());
        }

        public virtual IEnumerator OnSyncLastCallRoutine()
        {
            yield return null;
        }
    }
}
