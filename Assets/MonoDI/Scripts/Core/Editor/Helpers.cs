using MonoDI.Scripts.Core;
using UnityEngine;

namespace USI.Scripts.Core.Editor
{

    public class ToInject_Test : MonoBehaviour, TestInterface
    {

    }

    public interface TestInterface
    {
        
    }

    public class GetCompoenent_Test : MonoBehaviour
    {

    }

    public class GetCompoenentChild_Test : MonoBehaviour
    {

    }

    public class FindComponent_Test : MonoBehaviour
    {

    }

    public class InToInject_Test : InjectedMono
    {
        [In] private SignalBus _signalBus;
        [In] public ToInject_Test injectTest;
        [In] public TestInterface injectInterface;
        [Get] public GetCompoenent_Test Com;
        [GetChild] public GetCompoenentChild_Test ComChild;
        [Find] public FindComponent_Test FindTest;

        [Get] public GetCompoenent_Test[] Coms;
        [GetChild] public GetCompoenentChild_Test[] ComsChild;
        [Find] public FindComponent_Test[] FindsTest;

        public bool IsCalled
        {
            get { return _isCalled; }
            set { _isCalled = value; }
        }

        private bool _isCalled;

        [Sub]
        public void CallFromSignal(DummySignalData data)
        {
            _isCalled = true;
        }


        public void FireTestSignal()
        {
            _signalBus.Fire(new DummySignalData());
        }
    }

    public struct DummySignalData : ISignal
    {

    }
}