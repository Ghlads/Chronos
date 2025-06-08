using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor( typeof( RuntimeSet<> ), true )]    
    public class RuntimeSetInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            //SerializedProperty listProperty = serializedObject.FindProperty( "m_values" );
            SerializedProperty removeModeProperty = serializedObject.FindProperty( "m_mode" );
            SerializedProperty disallowNullProperty = serializedObject.FindProperty( "m_disallowNull" );
            VisualElement root = new();
            //root.styleSheets.Add( AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Scriptable/Editor/ScriptableGenerator/UI/ScriptableClassGenerator.uss" ) );
            PropertyField removeModeField = new PropertyField( removeModeProperty );
            PropertyField disallowNullField = new( disallowNullProperty );
            //PropertyField listField = new PropertyField( listProperty );
            root.Add( removeModeField );
            root.Add( disallowNullField );
            //root.Add( listField );
            //listField.AddToClassList( "readonly" );
            return root;
        }
    }
}
