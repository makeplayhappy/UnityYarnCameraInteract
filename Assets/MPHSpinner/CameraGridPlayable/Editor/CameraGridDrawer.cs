using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
/*
[CustomPropertyDrawer(typeof(CameraGridBehaviour))]
public class CameraGridDrawer : PropertyDrawer
{
   
   
   
    public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        
        public Vector2 gridSize = new Vector2 (1, 1);

    //This cameras screen position in the grid
	public Rect gridPosition = new Rect(0, 0, 1, 1);

    //Margin: clockwise: x = left, y= top, z = right, w = bottom
	public Vector4 margin = new Vector4 (12, 12, 12, 12);

        SerializedProperty colorProp = property.FindPropertyRelative("color");
        SerializedProperty intensityProp = property.FindPropertyRelative("intensity");
        SerializedProperty bounceIntensityProp = property.FindPropertyRelative("bounceIntensity");
        SerializedProperty rangeProp = property.FindPropertyRelative("range");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, colorProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, intensityProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, bounceIntensityProp);
        
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, rangeProp);
    }

    
}
*/