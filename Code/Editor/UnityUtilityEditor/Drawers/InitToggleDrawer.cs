#if UNITY_2019_3_OR_NEWER
using System;
using UnityEditor;
using UnityEngine;
using UnityUtility;

namespace UnityUtilityEditor.Drawers
{
    [CustomPropertyDrawer(typeof(InitToggleAttribute))]
    internal class InitToggleDrawer : SerializeReferenceDrawer<InitToggleAttribute>
    {
        protected override void DrawContent(in Rect position, SerializedProperty property)
        {
            Type type = EditorUtilityExt.GetFieldType(this);

            if (type.IsAbstract)
            {
                GUI.Label(position, "Use non-abstract type.");
                return;
            }

            bool inited = !property.managedReferenceFullTypename.IsNullOrEmpty();

            bool switched = EditorGUI.Toggle(position, inited);

            if (switched != inited)
            {
                property.serializedObject.Update();
                property.managedReferenceValue = switched ? Activator.CreateInstance(type) : null;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif