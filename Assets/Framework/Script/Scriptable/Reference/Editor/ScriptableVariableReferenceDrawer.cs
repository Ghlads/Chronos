using Framework.Core.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{

    [CustomPropertyDrawer( typeof( VariableReference<> ), true )]
    public class ScriptableVariableReferenceDrawer : PropertyDrawer
    {
        private PropertyField m_valueField;
        private InterfaceReferenceField m_referenceField;

        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            VisualElement root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.AddToClassList( "unity-base-field" );
            root.AddToClassList( "unity-object-field" );
            root.AddToClassList( "unity-base-field__aligned" );
            root.AddToClassList( "unity-base-field__inspector-field" );

            Label label = new Label( property.displayName );
            label.style.width = Length.Percent( 41 );
            label.AddToClassList( "unity-text-element" );
            label.AddToClassList( "unity-label" );
            label.AddToClassList( "unity-base-field__label" );
            label.AddToClassList( "unity-object-field__label" );
            label.AddToClassList( "unity-property-field__label" );

            SerializedProperty referenceModeProperty = property.FindPropertyRelative( "m_variableReferenceType" );
            PropertyField referenceField = new PropertyField( referenceModeProperty );
            referenceField.label = string.Empty;
            referenceField.style.flexGrow = 0;
            referenceField.style.flexShrink = 0;
            referenceField.RegisterValueChangeCallback( ReferenceModeChangeHandler );

            SerializedProperty valueProperty = property.FindPropertyRelative( "m_value" );
            SerializedProperty referenceProperty = property.FindPropertyRelative( "m_variableReference" );
            m_valueField = new PropertyField( valueProperty );
            m_valueField.style.flexGrow = 1;
            m_valueField.label = string.Empty;
            m_valueField.style.marginRight = 1.5f;

            m_referenceField = new InterfaceReferenceField( referenceProperty, fieldInfo.FieldType.GetField( "m_variableReference", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance ) );
            m_referenceField.style.flexGrow = 1;
            m_referenceField.Label = string.Empty;
            m_referenceField.style.marginRight = 1.5f;

            UpdateFieldVisibility( referenceModeProperty );

            root.Add( label );

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = 1;

            container.Add( referenceField );

            container.Add( m_valueField );
            container.Add( m_referenceField );

            root.Add( container );

            return root;
        }


        private void ReferenceModeChangeHandler( SerializedPropertyChangeEvent @event )
        {
            UpdateFieldVisibility( @event.changedProperty );
        }


        private void UpdateFieldVisibility( SerializedProperty property )
        {
            SVReferenceMode mode = ( SVReferenceMode )( property.enumValueIndex );
            switch ( mode )
            {
                case SVReferenceMode.Value:
                    m_valueField.style.display = DisplayStyle.Flex;
                    m_referenceField.style.display = DisplayStyle.None;
                    break;
                case SVReferenceMode.Reference:
                    m_valueField.style.display = DisplayStyle.None;
                    m_referenceField.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }

}