using System.Collections;
using System;
using UnityEngine;
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class HideAttributes : PropertyAttribute
{
    // NAME OF BOOL IN CONTROL
    public string ConditionalSourceField = "";
    // TRUE = Hide In Inspector / FALSE = Disable In Inspector
    public bool HideInInspector = false;

    public HideAttributes(string conditionalSourceField)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = false;
    }

    public HideAttributes(string conditionalSourceField, bool hideInInspector)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
    }
}
