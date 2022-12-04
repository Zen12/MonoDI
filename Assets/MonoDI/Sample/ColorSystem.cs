using MonoDI.Scripts.Core;
using UnityEngine;

namespace MonoDI.Sample
{
    public class ColorSystem : InjectedMono
    {
        [In] private SignalBus _signal;
        
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Y))
            {
                _signal.Fire(new ChangeColorSignal
                {
                    ChangeColor = Color.Lerp(Color.red, Color.green, Random.Range(0f, 1f))
                });
            }
        }
    }

    public struct ChangeColorSignal : ISignal
    {
        public Color ChangeColor;
    }
}
