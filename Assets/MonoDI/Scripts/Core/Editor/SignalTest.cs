using NUnit.Framework;
using UnityEngine;

namespace MonoDI.Scripts.Core.Editor
{
    public class SignalTest
    {
        [Test]
        public void SignalBusTest()
        {
            var bus = new SignalBus();
            
            var dummyClass = new GameObject().AddComponent<DummyClass>();
            var installer = new GeneralInstaller();
            installer.SubscribeSignals(bus, dummyClass);
            
            bus.Fire(new DummyStruct());
            Assert.IsTrue(dummyClass.IsCalled);
        }
        
        [Test]
        public void SignalBusTestNoSubAttribute()
        {
            var bus = new SignalBus();
            bool isCalled = false;
            bus.Sub<DummyStruct>(data =>
            {
                isCalled = true;
            });

            bus.Fire(new DummyStruct());
            Assert.IsTrue(isCalled);
        }
        
        [Test]
        public void SignalBusTestRemove()
        {
            var bus = new SignalBus();
            var dummyClass = new GameObject().AddComponent<DummyClass>();
            var installer = new GeneralInstaller();
            installer.SubscribeSignals(bus, dummyClass);
            installer.UnSubscribeSignals(bus, dummyClass);
            bus.Fire(new DummyStruct());
            Assert.IsTrue(dummyClass.IsCalled == false);
        }
        
                
        [Test]
        public void SignalBusTestClear()
        {
            bool isCalled = false;
            var bus = new SignalBus();
           // bus.Sub<DummyStruct>(CallBack);

            void CallBack(DummyStruct obj)
            {
                isCalled = true;
            }
            
            bus.Clear();
            bus.Fire(new DummyStruct());
            Assert.IsFalse(isCalled);
        }

    }
    
    internal class DummyClass : InjectedMono
    {
        public bool IsCalled = false;
        
        [Sub]
        public void Call(DummyStruct dummyClass)
        {
            IsCalled = true;
        }
    }
    
    public struct DummyStruct : ISignal
    {
        
    }
}
