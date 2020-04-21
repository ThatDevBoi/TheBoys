using UnityEngine;
using System;
// Changes the name of the arrays
#if UNITY_EDITOR
public class LabelArray : PropertyAttribute
{
    public readonly string[] name;
    public LabelArray(string[] names) { this.name = names; }
    public LabelArray(Type enumType) { name = Enum.GetNames(enumType); }
}
#endif
