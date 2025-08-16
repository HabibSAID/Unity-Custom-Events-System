using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class CustomEvent
{
    [SerializeField]
    private List<SingleCustomEvent> events = new List<SingleCustomEvent>();

    [Serializable]
    public class SingleCustomEvent
    {
        public GameObject targetObject;
        public Component targetComponent;
        public string methodName;
        public Parameter[] parameters;

        [HideInInspector] public bool parametersFoldout = false;

        /// <summary>Invoke method directly via reflection</summary>
        public void Invoke()
        {
            if (targetComponent == null || string.IsNullOrEmpty(methodName))
            {
                Debug.LogWarning("CustomEvent: Missing target or method name.");
                return;
            }

            MethodInfo method = targetComponent.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (method == null)
            {
                Debug.LogWarning($"CustomEvent: Method {methodName} not found on {targetComponent?.GetType()}");
                return;
            }

            ParameterInfo[] paramInfos = method.GetParameters();
            object[] args = new object[paramInfos.Length];

            for (int i = 0; i < paramInfos.Length; i++)
            {
                args[i] = parameters != null && i < parameters.Length
                    ? parameters[i].GetValue(paramInfos[i].ParameterType)
                    : null;
            }

            method.Invoke(targetComponent, args);
        }
    }

    /// <summary>Invoke all stored sub-events</summary>
    public void Invoke()
    {
        foreach (var e in events)
            e.Invoke();
    }

    /// <summary>Add a new sub-event</summary>
    public void AddEvent(SingleCustomEvent e)
    {
        if (e != null)
            events.Add(e);
    }

    /// <summary>Remove a sub-event</summary>
    public void RemoveEvent(SingleCustomEvent e)
    {
        if (e != null)
            events.Remove(e);
    }

    /// <summary>Get read-only list of sub-events</summary>
    public List<SingleCustomEvent> GetEvents() => events;
}
