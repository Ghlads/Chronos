using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{
    [CustomEditor( typeof( ScriptableEvent<> ), true )]
    public class ScriptableEventInpsector : UnityEditor.Editor
    {
        private SerializedProperty m_dummyProperty;

        private MethodInfo m_raiseMethod;

        public override VisualElement CreateInspectorGUI()
        {
            Type generic = target.GetType().GetGenericInstanceOf( typeof( ScriptableEvent<> ) );
            Type ValueType = generic.GetGenericArguments()[0];
            m_raiseMethod = target.GetType().GetMethod( "Raise", new[] { ValueType } );

            VisualElement root = new VisualElement();
            Button raiseButton = new Button();
            raiseButton.text = "Raise";
            raiseButton.clicked += RaiseEvent;
            root.Add( raiseButton );

            m_dummyProperty = serializedObject.FindProperty( "m_dummyValue" );
            if ( m_dummyProperty != null )
            {
                PropertyField propertyField = new PropertyField( m_dummyProperty );
                propertyField.label = string.Empty;
                root.Add( propertyField );
            }

            return root;
        }



        private void RaiseEvent()
        {
            m_raiseMethod.Invoke( target, new object[] { m_dummyProperty.boxedValue } );
        }
    }
}
