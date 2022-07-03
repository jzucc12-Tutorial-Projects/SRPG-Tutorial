using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MaxAttribute))]
public class MaxDrawer: PropertyDrawer 
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MaxAttribute maxAtt = (MaxAttribute)attribute;
        if(property.propertyType == SerializedPropertyType.Float)
        {
            property.floatValue = Mathf.Min(property.floatValue, maxAtt.max);
        }
        else if(property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = Mathf.Min(property.intValue, (int)maxAtt.max);
        }
        else
        {
            Debug.LogWarning("You can only use this for integer and floats");
        }

        EditorGUI.PropertyField(position, property, label, true);
    }
}