using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MonoDI.Scripts.Core
{
    public sealed class GeneralInstaller
    {
        public void Inject(InjectedMono to, List<System.Object> injection)
        {
            var fields = to.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var info in fields)
            {
                foreach (var attribute in info.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(In))
                    {
                        In.FixDeps(info, injection, to);
                    }
                    else if (attribute.AttributeType == typeof(Get))
                    {
                        Get.FixDeps(info, injection, to);
                    }
                    else if (attribute.AttributeType == typeof(GetChild))
                    {
                        GetChild.FixDeps(info, injection, to);
                    }
                    else if (attribute.AttributeType == typeof(Find))
                    {
                        Find.FixDeps(info, injection, to);
                    }
                }
            }
        }

        public void SubscribeSignals(SignalBus signalBus, System.Object to)
        {
            var methods = to.GetType().GetMethods
                (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var info in methods)
            {
                if (info.GetCustomAttributes(typeof(Sub), false).Length > 0)
                {
                    var param = info.GetParameters()[0];
                    signalBus.Sub(param.ParameterType, to, info);
                }
            }
        }

        public void UnSubscribeSignals(SignalBus signalBus, System.Object to)
        {
            var methods = to.GetType().GetMethods
                (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var info in methods)
            {
                if (info.GetCustomAttributes(typeof(Sub), false).Length > 0)
                {
                    var param = info.GetParameters()[0];
                    signalBus.UnSub(param.ParameterType, to, info);
                }
            }
        }
    }


    public class Sub : Attribute
    {

    }

    public class In : Attribute
    {
        internal static void FixDeps(FieldInfo info, List<System.Object> injections, InjectedMono to)
        {
            foreach (var inj in injections)
            {
                if (inj != null)
                {
                    if (info.FieldType == inj.GetType() ||
                        inj.GetType().GetInterfaces().Contains(info.FieldType))
                    {
                        info.SetValue(to, inj);
                        break;
                    }
                }
            }
        }
    }

    public class Get : Attribute
    {
        internal static void FixDeps(FieldInfo info, List<System.Object> injections, InjectedMono to)
        {
            var g = to;
            var type = info.FieldType;
            if (type.IsArray)
            {
                var t = type.GetElementType();
                if (t != null)
                {
                    var c = g.GetComponents(t);
                    var val = Array.CreateInstance(t, c.Length);
                    //span or memcpy... also duplicate code...
                    for (int i = 0; i < c.Length; i++)
                    {
                        val.SetValue(c[i], i);
                    }

                    info.SetValue(g, val);
                }

            }
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition()
                    == typeof(List<>))
                {
                    Debug.LogError("No support for list");
                }
                else
                {
                    var c = g.GetComponent(type);
                    info.SetValue(to, c);
                }
            }
        }
    }

    public class GetChild : Attribute
    {
        internal static void FixDeps(FieldInfo info, List<System.Object> injections, InjectedMono to)
        {
            var g = to;
            var type = info.FieldType;
            if (type.IsArray)
            {
                var t = type.GetElementType();
                if (t != null)
                {

                    var c = g.GetComponentsInChildren(t);
                    var val = Array.CreateInstance(t, c.Length);
                    //span or memcpy... also duplicate code...
                    for (int i = 0; i < c.Length; i++)
                    {
                        val.SetValue(c[i], i);
                    }

                    info.SetValue(g, val);
                }
            }
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition()
                    == typeof(List<>))
                {
                    Debug.LogError("Not support for list");
                }
                else
                {
                    var c = g.GetComponentInChildren(type, true);
                    info.SetValue(to, c);
                }
            }
        }
    }

    public class Find : Attribute
    {
        internal static void FixDeps(FieldInfo info, List<System.Object> injections, InjectedMono to)
        {
            var g = to;
            var type = info.FieldType;
            if (type.IsArray)
            {
                var t = type.GetElementType();
                var c = GameObject.FindObjectsOfType(t);
                var val = Array.CreateInstance(t, c.Length);
                //span or memcpy... also duplicate code...
                for (int i = 0; i < c.Length; i++)
                {
                    val.SetValue(c[i], i);
                }

                info.SetValue(g, val);
            }
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Debug.LogError("No support for list");
                }
                else
                {
                    var obj = GameObject.FindObjectOfType(type);
                    info.SetValue(to, obj);
                }
            }
        }
    }
}


