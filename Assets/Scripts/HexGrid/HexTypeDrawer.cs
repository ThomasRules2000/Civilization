using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexType))]
public class HexTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Get type
        string type = property.FindPropertyRelative("HexType").ToString();

        //Add read only label showing coordinates
        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, type);
    }
}