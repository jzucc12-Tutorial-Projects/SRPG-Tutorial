using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GridVisualType", menuName = "SRPGTutorial/GridVisualType", order = 0)]
public class GridVisualType : ScriptableObject
{
    public List<string> actionNames;
    [Tooltip("Used for all actions")] public Material baseMaterial;
}


// CUSTOM EDITOR IN CASE I DECIDE TO GO BACK AND RESTRICT MATERIAL ENTRY BY STRING NAME//
// [CustomEditor(typeof(GridVisualType))]
// public class GridVisualTypeEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         var grid = (GridVisualType)target;
        
//         var indent = EditorGUI.indentLevel;
//         EditorGUI.indentLevel = 0;


//         var actionProp = serializedObject.FindProperty("actionName");
//         EditorGUILayout.PropertyField(actionProp);
//         var actionType = Type.GetType(actionProp.stringValue);
//         Debug.Log(grid.actionName);
//         Debug.Log(actionType);

//         if(actionType != null)
//         {
//             if(actionType.IsSubclassOf(typeof(BaseAction)))
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("baseMaterial"));
            
//             if(actionType.IsSubclassOf(typeof(TargetedAction)))
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("targetedMaterial"));
//         }

//         EditorGUI.indentLevel = indent;
//     }
// }