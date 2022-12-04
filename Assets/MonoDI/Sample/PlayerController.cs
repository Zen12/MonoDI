using MonoDI.Scripts.Core;
using UnityEngine;

namespace MonoDI.Sample
{
    public class PlayerController : InjectedMono
    {
        [Get] private Rigidbody _rig; // GetComponent
        [Get] private Renderer[] _renderers; // GetComponents (array)
        [In] private InputSystem _inputSystem; // Injects from System
        
        void Update()
        {
            if (_inputSystem.InputData.IsUp)
            {
                _rig.position += Vector3.up * Time.deltaTime;
            }
            
            if (_inputSystem.InputData.IsDown)
            {
                _rig.position += Vector3.down * Time.deltaTime;
            }
            
            if (_inputSystem.InputData.IsLeft)
            {
                _rig.position += Vector3.left * Time.deltaTime;
            }
            
            if (_inputSystem.InputData.IsRight)
            {
                _rig.position += Vector3.right * Time.deltaTime;
            }
        }

        // called by signalBus, check ColorSystem
        // it auto subscribes (Awake) and unsubscribes (OnDestroy)
        [Sub]
        private void OnColorChange(ChangeColorSignal changeColor)
        {
            foreach (var r in _renderers)
            {
                r.sharedMaterial.color = changeColor.ChangeColor;
            }
        }
    }
}
