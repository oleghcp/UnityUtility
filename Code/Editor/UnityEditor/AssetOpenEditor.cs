using System.IO;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityUtility.NodeBased;
using UnityUtilityEditor.Window.NodeBased;
#endif
using UnityObject = UnityEngine.Object;

namespace UnityEditor
{
    internal static class AssetOpenEditor
    {
        [OnOpenAsset]
        private static bool OpenScriptableObjectClass(int instanceID, int _)
        {
            UnityObject obj = EditorUtility.InstanceIDToObject(instanceID);

#if UNITY_2019_3_OR_NEWER
            if (obj is RawGraph graphAsset)
            {
                GraphEditorWindow.Open(graphAsset);
                return true;
            }
#endif
            if (obj is ScriptableObject)
            {
                EditorUtilityExt.OpenScriptableObjectCode(obj);
                return true;
            }

            if (ProjectWindowUtil.IsFolder(instanceID))
            {
                EditorUtilityExt.OpenFolder(AssetDatabase.GetAssetPath(obj));
                return true;
            }

            return false;
        }
    }
}
