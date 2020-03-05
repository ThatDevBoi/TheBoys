using UnityEngine;
using UnityEditor;
using System.Linq;
/// <summary>
/// Edits the IDE to make the names appear 
/// </summary>
[CustomPropertyDrawer(typeof(LabelArray))]
public class LableArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        try
        {
            var path = property.propertyPath;
            int pos = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
            EditorGUI.PropertyField(rect, property, new GUIContent(((LabelArray)attribute).name[pos]), true);
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, label, true);
        }
        EditorGUI.EndProperty();
    }
}
