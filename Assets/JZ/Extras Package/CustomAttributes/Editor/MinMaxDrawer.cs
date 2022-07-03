using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxDrawer: PropertyDrawer 
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxAttribute mmAtt = (MinMaxAttribute)attribute;
        if(property.propertyType == SerializedPropertyType.Float)
        {
            property.floatValue = Mathf.Clamp(property.floatValue, mmAtt.min, mmAtt.max);
        }
        else if(property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = (int)Mathf.Clamp(property.intValue, mmAtt.min, mmAtt.max);
        }
        else
        {
            Debug.LogWarning("You can only use this for integer and floats");
        }

        EditorGUI.PropertyField(position, property, label, true);
    }
}