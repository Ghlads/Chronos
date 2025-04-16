using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Framework.Scriptable.Editor
{
    [CustomEditor( typeof( ScriptableVariable<> ), true )]
    public class ScriptableVariableCustomInspector : UnityEditor.Editor
    {
        private PropertyField m_currentValueField = null;

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChangedHandler;
        }


        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            SerializedProperty defaultValueProperty = serializedObject.FindProperty( "m_defaultValue" );
            PropertyField defaultField = new PropertyField( defaultValueProperty );
            SerializedProperty currentValueProperty = serializedObject.FindProperty( "m_value" );
            m_currentValueField = new PropertyField( currentValueProperty );
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChangedHandler;
            m_currentValueField.SetEnabled( EditorApplication.isPlaying );
            root.Add( defaultField );
            root.Add( m_currentValueField );
            return root;
        }


        private void EditorApplication_playModeStateChangedHandler( PlayModeStateChange state )
        {
            switch ( state )
            {
                case PlayModeStateChange.EnteredPlayMode:
                    m_currentValueField.SetEnabled( true );
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    m_currentValueField.SetEnabled( false );
                    break;
            }
        }
    }
}
