using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(CustomEvent))]
public class CustomEventDrawer : PropertyDrawer
{
    private ReorderableList list;
    private float lineHeight = EditorGUIUtility.singleLineHeight;
    private float spacing = 6f; // Spacing between lines

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var eventsProp = property.FindPropertyRelative("events");

        if (list == null)
        {
            list = new ReorderableList(property.serializedObject, eventsProp, true, true, true, true);

            // Header
            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Custom Events");
            };

            // Element drawing
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = eventsProp.GetArrayElementAtIndex(index);
                float y = rect.y;

                // Foldout (bold + white text for Event label)
                GUIStyle boldWhite = new GUIStyle(EditorStyles.foldout);
                boldWhite.fontStyle = FontStyle.Bold;
                boldWhite.normal.textColor = Color.white;
                boldWhite.onNormal.textColor = Color.white;

                element.isExpanded = EditorGUI.Foldout(
                    new Rect(rect.x, y, rect.width, lineHeight),
                    element.isExpanded, $"Event {index + 1}", true, boldWhite
                );
                y += lineHeight + spacing;

                if (element.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    // Target Object
                    var targetObjectProp = element.FindPropertyRelative("targetObject");
                    EditorGUI.PropertyField(new Rect(rect.x + 4, y, rect.width - 8, lineHeight), targetObjectProp);
                    y += lineHeight + spacing;

                    // Component
                    var targetComponentProp = element.FindPropertyRelative("targetComponent");
                    var targetObject = targetObjectProp.objectReferenceValue as GameObject;
                    if (targetObject != null)
                    {
                        var components = targetObject.GetComponents<MonoBehaviour>();
                        if (components.Length > 0)
                        {
                            string[] componentNames = components.Select(c => c.GetType().Name).ToArray();
                            int currentIndex = Mathf.Max(0, Array.IndexOf(components, targetComponentProp.objectReferenceValue));
                            int newIndex = EditorGUI.Popup(
                                new Rect(rect.x + 4, y, rect.width - 8, lineHeight),
                                "Component", currentIndex, componentNames
                            );

                            targetComponentProp.objectReferenceValue = components[newIndex];
                            var selectedComponent = components[newIndex];
                            y += lineHeight + spacing;

                            // Method
                            var methodNameProp = element.FindPropertyRelative("methodName");
                            var methods = selectedComponent.GetType()
                                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(m => !m.IsSpecialName && !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_"))
                                .ToArray();

                            string[] methodNames = methods.Select(m => m.Name).ToArray();
                            int methodIndex = Mathf.Max(0, Array.IndexOf(methodNames, methodNameProp.stringValue));
                            int newMethodIndex = EditorGUI.Popup(
                                new Rect(rect.x + 4, y, rect.width - 8, lineHeight),
                                "Method", methodIndex, methodNames
                            );

                            if (newMethodIndex >= 0 && newMethodIndex < methods.Length)
                            {
                                methodNameProp.stringValue = methodNames[newMethodIndex];
                                MethodInfo selectedMethod = methods[newMethodIndex];
                                ParameterInfo[] paramInfos = selectedMethod.GetParameters();

                                // Parameters array size
                                var parametersProp = element.FindPropertyRelative("parameters");
                                if (parametersProp.arraySize != paramInfos.Length)
                                    parametersProp.arraySize = paramInfos.Length;

                                y += lineHeight + spacing;

                                // Parameters foldout
                                var parametersFoldoutProp = element.FindPropertyRelative("parametersFoldout");
                                if (paramInfos.Length > 0)
                                {
                                    parametersFoldoutProp.boolValue = EditorGUI.Foldout(
                                        new Rect(rect.x + 4, y, rect.width - 8, lineHeight),
                                        parametersFoldoutProp.boolValue, "Parameters", true
                                    );

                                    // Force inspector to repaint to recalc height
                                    // (GUI.changed)
                                    //{
                                    //property.serializedObject.ApplyModifiedProperties();
                                    //EditorUtility.SetDirty(property.serializedObject.targetObject);
                                    //GUI.changed = false;
                                    //}

                                    y += lineHeight + spacing;

                                    if (parametersFoldoutProp.boolValue)
                                    {
                                        EditorGUI.indentLevel++;
                                        for (int i = 0; i < paramInfos.Length; i++)
                                        {
                                            var paramInfo = paramInfos[i];
                                            var paramProp = parametersProp.GetArrayElementAtIndex(i);

                                            // Parameter label
                                            EditorGUI.LabelField(
                                                new Rect(rect.x + 8, y, rect.width - 12, lineHeight),
                                                $"{paramInfo.Name} ({paramInfo.ParameterType.Name})"
                                            );
                                            y += lineHeight + spacing;

                                            // Parameter field
                                            DrawParameterField(
                                                new Rect(rect.x + 8, y, rect.width - 12, lineHeight),
                                                paramProp, paramInfo.ParameterType
                                            );
                                            y += lineHeight + spacing;
                                        }
                                        EditorGUI.indentLevel--;
                                    }
                                }
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                // White separator line between events
                EditorGUI.DrawRect(
                    new Rect(rect.x, rect.yMax - 1, rect.width, 1),
                    Color.white
                );
            };

            // Dynamic height calculation
            list.elementHeightCallback = index =>
            {
                var element = eventsProp.GetArrayElementAtIndex(index);
                float height = lineHeight + spacing; // foldout

                if (element.isExpanded)
                {
                    height += (lineHeight + spacing) * 2; // targetObject + component

                    var targetObjectProp = element.FindPropertyRelative("targetObject");
                    var targetComponentProp = element.FindPropertyRelative("targetComponent");
                    var targetObject = targetObjectProp.objectReferenceValue as GameObject;

                    if (targetObject != null && targetComponentProp.objectReferenceValue != null)
                    {
                        height += lineHeight + spacing; // method

                        var parametersFoldoutProp = element.FindPropertyRelative("parametersFoldout");
                        var parametersProp = element.FindPropertyRelative("parameters");

                        if (parametersProp.arraySize > 0)
                        {
                            height += lineHeight + spacing; // parameters foldout

                            if (parametersFoldoutProp.boolValue)
                            {
                                height += parametersProp.arraySize * (lineHeight * 2 + spacing);
                                // label + field per parameter
                            }
                        }
                    }

                    height += spacing; // bottom padding
                }

                height += 10f; // extra gap between events
                return height;
            };
        }

        list.DoLayoutList();
    }

    private void DrawParameterField(Rect rect, SerializedProperty paramProp, Type paramType)
    {
        if (paramType == typeof(int))
            paramProp.FindPropertyRelative("intValue").intValue = EditorGUI.IntField(rect, "Value", paramProp.FindPropertyRelative("intValue").intValue);
        else if (paramType == typeof(float))
            paramProp.FindPropertyRelative("floatValue").floatValue = EditorGUI.FloatField(rect, "Value", paramProp.FindPropertyRelative("floatValue").floatValue);
        else if (paramType == typeof(bool))
            paramProp.FindPropertyRelative("boolValue").boolValue = EditorGUI.Toggle(rect, "Value", paramProp.FindPropertyRelative("boolValue").boolValue);
        else if (paramType == typeof(string))
            paramProp.FindPropertyRelative("stringValue").stringValue = EditorGUI.TextField(rect, "Value", paramProp.FindPropertyRelative("stringValue").stringValue);
        else if (paramType.IsEnum)
        {
            var enumNames = Enum.GetNames(paramType);
            int enumIndex = Mathf.Max(0, Array.IndexOf(enumNames, paramProp.FindPropertyRelative("stringValue").stringValue));
            int newEnumIndex = EditorGUI.Popup(rect, "Value", enumIndex, enumNames);
            paramProp.FindPropertyRelative("stringValue").stringValue = enumNames[newEnumIndex];
        }
        else if (typeof(UnityEngine.Object).IsAssignableFrom(paramType))
        {
            paramProp.FindPropertyRelative("objectValue").objectReferenceValue =
                EditorGUI.ObjectField(rect, "Value", paramProp.FindPropertyRelative("objectValue").objectReferenceValue, paramType, true);
        }
        else
        {
            EditorGUI.LabelField(rect, "Unsupported Type");
        }
    }
}
