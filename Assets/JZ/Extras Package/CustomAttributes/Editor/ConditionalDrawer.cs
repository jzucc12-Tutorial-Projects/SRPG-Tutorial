using JZ.MATH;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalAttribute))]
public abstract class ConditionalDrawer: PropertyDrawer 
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalAttribute condAtt = (ConditionalAttribute)attribute;
        if(ShouldShow(condAtt, property))
        {
            //Make normal height if it should be shown
            if(property.isExpanded)
                return property.CountInProperty() * 20;
            else
                return base.GetPropertyHeight(property, label);
        }
        else
        {
            //Collapse the variable if it should not be shown
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        ConditionalAttribute condAtt = (ConditionalAttribute)attribute;
        if(ShouldShow(condAtt, property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    protected SerializedProperty GetReferenceProperty(ConditionalAttribute condAtt, SerializedProperty myProperty)
    {
        SerializedProperty prop;
        if(myProperty.depth == 0)
        {
            prop = myProperty.serializedObject.FindProperty(condAtt.variableName);
        }
        else
        {
            string[] path = myProperty.propertyPath.Split('.');
            prop = myProperty.serializedObject.FindProperty(path[0]).FindPropertyRelative(condAtt.variableName);
        }

        if (prop == null)
        {
            Debug.LogError($"{prop.name}: Either property does not exist or it is not serialized");
            return null;
        }
        else if (!condAtt.isComparison)
        {
            if(prop.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogError($"{prop.name}: Conditional attribute must be bool if there is no comparison type");
                return null;
            }
        }
        else if(prop.propertyType != SerializedPropertyType.Float && prop.propertyType != SerializedPropertyType.Integer)
        {
            Debug.LogError($"{prop.name}: Conditional attribute must be float or int if using a comparison type");
            return null;
        }
        return prop;
    }

    protected abstract bool ShouldShow(ConditionalAttribute condAtt, SerializedProperty myProperty);
}

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer: ConditionalDrawer 
{
    protected override bool ShouldShow(ConditionalAttribute condAtt, SerializedProperty myProperty)
    {
        //check non-comparison options
        SerializedProperty refProp = GetReferenceProperty(condAtt, myProperty);
        if(refProp == null) return false;
        if(!condAtt.isComparison) return refProp.boolValue;

        //Make sure comparison value is a float
        float compValue;
        if(refProp.propertyType == SerializedPropertyType.Integer)
            compValue = (float)refProp.intValue;
        else
            compValue = refProp.floatValue;

        //Check comparison
        switch(condAtt.comparisonType)
        {
            case ComparisonType.greaterThan:
            default:
                return compValue > condAtt.comparisonValue;
            case ComparisonType.lessThan:
                return compValue < condAtt.comparisonValue;
            case ComparisonType.EqualTo:
                return compValue == condAtt.comparisonValue; 
            case ComparisonType.greaterThanOrEqualTo:
                return compValue >= condAtt.comparisonValue;
            case ComparisonType.lessThanOrEqualTo:
                return compValue <= condAtt.comparisonValue;
        }
    }
}

[CustomPropertyDrawer(typeof(HideIfAttribute))]
public class HideIfDrawer: ShowIfDrawer
{
    protected override bool ShouldShow(ConditionalAttribute condAtt, SerializedProperty myProperty)
    {
        return !base.ShouldShow(condAtt, myProperty);
    }
}