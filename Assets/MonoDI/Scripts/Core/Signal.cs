using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonoDI.Scripts.Core
{
    public sealed class SignalBus : IDisposable
    {
        private readonly Dictionary<Type, List<ActualPointerToMethod>> _dictionary
            = new Dictionary<Type, List<ActualPointerToMethod>>();

        [Obsolete("Please use attribute version")]
        public SignalBus Sub<T>(System.Action<T> callBack)
        {
            this.Sub(typeof(T), callBack.Target, callBack.Method);
            return this;
        }
        
        public SignalBus Sub(Type type, System.Object obj, MethodInfo info)
        {
            var pointer = new ActualPointerToMethod(obj, info);
            GetList(type).Add(pointer);
            return this;
        }

        public SignalBus Fire<T>(T data) where T : struct, ISignal
        {
            var l = GetList(typeof(T));
            foreach (var obj in l)
            {
                obj.CallAll(data);
            }
            return this;
        }
        
        
        public SignalBus UnSub(Type type, System.Object obj, MethodInfo info)
        {
            GetList(type).RemoveAll(_ => _.IsSame(obj, info));
            return this;
        }

        private List<ActualPointerToMethod> GetList(Type type)
        {
            if (_dictionary.ContainsKey(type) == false)
            {
                var list = new List<ActualPointerToMethod>();
                _dictionary.Add(type, list);
            }

            return _dictionary[type];
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        //when it loses reference (AKA reload scene) => AutoClear by GC
        public void Dispose()
        {
            Clear();
        }
    }

    internal sealed class ActualPointerToMethod
    {
        private readonly System.Object _obj;
        private readonly MethodInfo _info;

        public ActualPointerToMethod(object obj, MethodInfo info)
        {
            _obj = obj;
            _info = info;
        }

        public void CallAll(System.Object data)
        {
            if (_obj != null && _info != null)
            {
                if (_obj is MonoBehaviour)
                {
                    var mono = (MonoBehaviour) _obj;
                    if (mono == null)
                        return;
                }
                
                try
                {
                    _info.Invoke(_obj, new[] {data});
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError(e.StackTrace);
                }
            }
        }

        public bool IsSame(System.Object obj, MethodInfo info)
        {
            return obj == _obj && _info == info;
        }
    }
    

    public interface ISignal
    {
        
    }
}
