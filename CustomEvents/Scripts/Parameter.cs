using System;
using UnityEngine;

[Serializable]
public class Parameter
{
    public string typeName; // For reference
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public string stringValue;
    public UnityEngine.Object objectValue;

    public object GetValue(Type targetType)
    {
        if (targetType == typeof(int)) return intValue;
        if (targetType == typeof(float)) return floatValue;
        if (targetType == typeof(bool)) return boolValue;
        if (targetType == typeof(string)) return stringValue;
        if (typeof(UnityEngine.Object).IsAssignableFrom(targetType)) return objectValue;
        if (targetType.IsEnum) return Enum.Parse(targetType, stringValue);
        return null;
    }
}
