using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HideAttributes))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    // Prepares are inspector without this the inspector is cluttered and is hard to read
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HideAttributes condHAtt = (HideAttributes)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
        // Enable the variables in the inspector
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }
    // We are getting the Height of where the Variables will be in the inspector so we arent just allowing for large gapes
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        HideAttributes condHAtt = (HideAttributes)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
    // Do we hide the variable?
    private bool GetConditionalHideAttributeResult(HideAttributes condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, condHAtt.ConditionalSourceField); //changes the path to the conditionalsource property path
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            enabled = sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}