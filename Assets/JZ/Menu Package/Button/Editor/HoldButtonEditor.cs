using JZ.MENU.BUTTON;
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// <para>Editor extension required to show
/// hold button parameters</para>
/// </summary>
[CustomEditor(typeof(HoldButton))]
public class HoldButtonEditor : ButtonEditor
{
    public SerializedProperty holdtimerProp;

    protected override void OnEnable() 
    {
        base.OnEnable();
        holdtimerProp = serializedObject.FindProperty("timeToHold");
    }

    public override void OnInspectorGUI()
    {
        HoldButton holdButton = (HoldButton)target;
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(holdtimerProp);
        serializedObject.ApplyModifiedProperties();
    }
}