
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AI_ScoutDrone_Attribute))]
public class AI_ScoutDrone_Editor : PropertyDrawer
{


    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.PropertyField(rect, property, new GUIContent(((AI_ScoutDrone_Attribute)attribute).names[pos]));
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}
