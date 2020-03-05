using UnityEngine;
using System;
// Changes the name of the arrays
public class LabelArray : PropertyAttribute
{
    public readonly string[] name;
    public LabelArray(string[] names) { this.name = names; }
    public LabelArray(Type enumType) { name = Enum.GetNames(enumType); }
}
