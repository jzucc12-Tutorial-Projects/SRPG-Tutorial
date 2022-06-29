using UnityEngine;
using JZ.MATH;

/// <summary>
/// <para>Changes attribute depending upon specified conditions</para>
/// </summary>
public abstract class ConditionalAttribute : PropertyAttribute
{
    public readonly string variableName;
    public readonly float comparisonValue;
    public readonly ComparisonType comparisonType;
    public bool isComparison => comparisonType != ComparisonType.None;

    public ConditionalAttribute(string variableName)
    {
        this.variableName = variableName;
        comparisonType = ComparisonType.None;
    }

    public ConditionalAttribute(string variableName, ComparisonType comparisonType, float comparisonValue = 0)
    {
        this.variableName = variableName;
        this.comparisonValue = comparisonValue;
        this.comparisonType = comparisonType;
    }
}

/// <summary>
/// <para>Attribute is hidden unless condition is met</para>
/// </summary>
public class ShowIfAttribute : ConditionalAttribute
{
    public ShowIfAttribute(string variableName) : base(variableName)
    {
    }

    public ShowIfAttribute(string variableName, ComparisonType comparisonType, float comparisonValue) : base(variableName, comparisonType, comparisonValue)
    {
    }
}

/// <summary>
/// <para>Attribute is visible unless condition is met</para>
/// </summary>
public class HideIfAttribute : ShowIfAttribute
{
    public HideIfAttribute(string variableName) : base(variableName)
    {
    }

    public HideIfAttribute(string variableName, ComparisonType comparisonType, float comparisonValue) : base(variableName, comparisonType, comparisonValue)
    {
    }
}