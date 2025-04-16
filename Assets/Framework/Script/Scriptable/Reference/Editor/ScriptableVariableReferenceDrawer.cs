using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{

    [CustomPropertyDrawer( typeof( ScriptableVariableReference<,,> ), true )]
    public class ScriptableVariableReferenceDrawer : PropertyDrawer
    {
        private PropertyField m_valueField;
        private PropertyField m_variableField;
        private PropertyField m_injectorField;

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

            SerializedProperty referenceModeProperty = property.FindPropertyRelative( "m_referenceMode" );
            PropertyField referenceField = new PropertyField( referenceModeProperty );
            referenceField.label = string.Empty;
            referenceField.style.flexGrow = 0;
            referenceField.style.flexShrink = 0;
            referenceField.RegisterValueChangeCallback( ReferenceModeChangeHandler );

            SerializedProperty valueProperty = property.FindPropertyRelative( "m_value" );
            SerializedProperty variableProperty = property.FindPropertyRelative( "m_variable" );
            SerializedProperty injectorProperty = property.FindPropertyRelative( "m_injector" );
            m_valueField = new PropertyField( valueProperty );
            m_valueField.style.flexGrow = 1;
            m_valueField.label = string.Empty;
            m_valueField.style.marginRight = 1.5f;

            m_variableField = new PropertyField( variableProperty );
            m_variableField.style.flexGrow = 1;
            m_variableField.label = string.Empty;
            m_variableField.style.marginRight = 1.5f;

            m_injectorField = new PropertyField( injectorProperty );
            m_injectorField.style.flexGrow = 1;
            m_injectorField.label = string.Empty;
            m_injectorField.style.marginRight = 1.5f;

            UpdateFieldVisibility( referenceModeProperty );

            root.Add( label );

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = 1;

            container.Add( referenceField );

            container.Add( m_valueField );
            container.Add( m_variableField );
            container.Add( m_injectorField );

            root.Add( container );

            return root;
        }


        private void ReferenceModeChangeHandler( SerializedPropertyChangeEvent @event )
        {
            UpdateFieldVisibility( @event.changedProperty );
        }


        private void UpdateFieldVisibility( SerializedProperty property )
        {
            SVReferenceMode mode = ( SVReferenceMode )( property.enumValueIndex + 1 );
            switch ( mode )
            {
                case SVReferenceMode.Value:
                    m_valueField.style.display = DisplayStyle.Flex;
                    m_variableField.style.display = DisplayStyle.None;
                    m_injectorField.style.display = DisplayStyle.None;
                    break;
                case SVReferenceMode.Variable:
                    m_valueField.style.display = DisplayStyle.None;
                    m_variableField.style.display = DisplayStyle.Flex;
                    m_injectorField.style.display = DisplayStyle.None;
                    break;
                case SVReferenceMode.Injector:
                    m_valueField.style.display = DisplayStyle.None;
                    m_variableField.style.display = DisplayStyle.None;
                    m_injectorField.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }

}