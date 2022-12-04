using System.Collections;
using System.Collections.Generic;
using MonoDI.Scripts.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace USI.Scripts.Core.Editor
{
    public class InjectionTest
    {
        
        [Test]
        public void TestInjection()
        {
            var installer = new GeneralInstaller();
            var to = new GameObject().AddComponent<ToInject_Test>();
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            
            installer.Inject(intoInject, new List<System.Object>
            {
                to
            });
            
            Assert.IsNotNull(intoInject.injectTest);
        }
        
        [Test]
        public void TestInjectionInteface()
        {
            var installer = new GeneralInstaller();
            var to = new GameObject().AddComponent<ToInject_Test>();
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            
            installer.Inject(intoInject, new List<System.Object>
            {
                to
            });
            
            Assert.IsNotNull(intoInject.injectInterface);
        }

        
        [Test]
        public void TestGet()
        {
            var installer = new GeneralInstaller();
            var to = new GameObject().AddComponent<ToInject_Test>();
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            var comp = intoInject.gameObject.AddComponent<GetCompoenent_Test>();
            
            installer.Inject(intoInject, new List<System.Object>
            {
                to
            });
            
            Assert.AreEqual(intoInject.injectTest, to);
            Assert.AreEqual(intoInject.Com, comp);
            Assert.AreEqual(intoInject.Coms[0], comp);
        }
        
                
        [Test]
        public void TestGetChild()
        {
            var installer = new GeneralInstaller();
            var to = new GameObject().AddComponent<ToInject_Test>();
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            var comp = intoInject.gameObject.AddComponent<GetCompoenent_Test>();
            var comp2 = new GameObject().AddComponent<GetCompoenentChild_Test>();
            comp2.transform.parent = comp.transform;
            
            installer.Inject(intoInject, new List<System.Object>
            {
                to
            });
            
            Assert.AreEqual(intoInject.injectTest, to);
            Assert.AreEqual(intoInject.Com, comp);
            Assert.AreEqual(intoInject.ComChild, comp2);
            Assert.AreEqual(intoInject.ComsChild[0], comp2);
        }
        
        
        [Test]
        public void TestFind()
        {
            var installer = new GeneralInstaller();
            var to = new GameObject().AddComponent<ToInject_Test>();
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            var comp = intoInject.gameObject.AddComponent<FindComponent_Test>();
            
            installer.Inject(intoInject, new List<System.Object>
            {
                to
            });
            
            Assert.AreEqual(intoInject.injectTest, to);
            Assert.AreEqual(intoInject.FindTest,comp);
            Assert.AreEqual(intoInject.FindsTest[0],comp);
        }
        
        
        [UnityTest]
        public IEnumerator TestSubscribeToSignalBus()
        {
            var intoInject = new GameObject().AddComponent<InToInject_Test>();
            
            var installer = new GeneralInstaller();
            var signalBus = new SignalBus();
            
            installer.Inject(intoInject, new List<object>
            {
                signalBus
            });
            
            installer.SubscribeSignals(signalBus, intoInject);

            yield return null;
            yield return null;

            intoInject.FireTestSignal();
            Assert.IsTrue(intoInject.IsCalled);
        }
        
        [UnityTest]
        public IEnumerator  TestUnSubscribeToSignalBus()
        {
            var intoInject = new GameObject().AddComponent<InToInject_Test>();

            new GameObject().AddComponent<MonoDI.Scripts.Core.MonoDI>();
            
            //awake is not called...
            MonoDI.Scripts.Core.MonoDI.Instance.FixDependencies(intoInject);

            yield return null;
            yield return null;

            intoInject.FireTestSignal();
            intoInject.IsCalled = false;
            
            //simulate OnDestroy
            MonoDI.Scripts.Core.MonoDI.Instance.ClearAll();
            
            intoInject.FireTestSignal();
            Assert.IsTrue(intoInject.IsCalled == false);
        }

    }
}
