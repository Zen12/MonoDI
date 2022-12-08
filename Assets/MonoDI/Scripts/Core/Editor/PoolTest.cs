using NUnit.Framework;

namespace MonoDI.Scripts.Core.Editor
{
    public class PoolTest
    {
        [Test]
        public void PoolTestBasic()
        {
            var moq = Pool<TestClass>.GetFromPool(() => new TestClass());
            Assert.IsTrue(moq.IsCalledEnter == false);
            Assert.IsTrue(moq.IsCalledExit == false);
        }
        
        [Test]
        public void PoolTestEnter()
        {
            var r = new TestClass();
            Pool<TestClass>.SetInPool(r);
            Assert.IsTrue(r.IsCalledEnter == true);
            Assert.IsTrue(r.IsCalledExit == false);
        }
        
        [Test]
        public void PoolTestExit()
        {
            var r = new TestClass();
            Pool<TestClass>.SetInPool(r);
            r = Pool<TestClass>.GetFromPool(() => new TestClass());
            Assert.IsTrue(r.IsCalledEnter == true);
            Assert.IsTrue(r.IsCalledExit == true);
        }

        [Test]
        public void ClearAll()
        {
            var r = new TestClass();
            Pool<TestClass>.SetInPool(r);
            Pool<TestClass>.ForceClearAll();
            var r2 = Pool<TestClass>.GetFromPool(() => new TestClass());
            Assert.AreNotEqual(r, r2);
        }
        
        [Test]
        public void PoolTestWithKeyValue()
        {
            var r = new TestClass();
            var key1 = new PoolKey();
            
            PoolWithSO<PoolKey, TestClass>.SetInPool(key1, r);
            r = PoolWithSO<PoolKey, TestClass>.GetFromPool(key1,() => new TestClass());
            Assert.IsTrue(r.IsCalledEnter == true);
            Assert.IsTrue(r.IsCalledExit == true);
        }
        
        [Test]
        public void PoolTestWithKeyValue_WrongKey()
        {
            var r = new TestClass();
            var key1 = new PoolKey();
            var key2 = new PoolKey();
            
            PoolWithSO<PoolKey, TestClass>.SetInPool(key1, r);
            r = PoolWithSO<PoolKey, TestClass>.GetFromPool(key2,() => new TestClass());
            Assert.IsTrue(r.IsCalledEnter == false);
            Assert.IsTrue(r.IsCalledExit == false);
        }
        
        public class TestClass : IPoolObject
        {
            public bool IsCalledEnter;
            public bool IsCalledExit;
            
            public void OnEnterPool()
            {
                IsCalledEnter = true;
            }

            public void OnExitPool()
            {
                IsCalledExit = true;
            }
        }
        
        public class PoolKey
        {
            
        }

    }
}