using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{
    [CustomPropertyDrawer( typeof( ScriptableEventReference<,,> ), true )]
    public class ScriptableEventReferenceDrawer : PropertyDrawer
    {
        private PropertyField m_eventField;
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

            SerializedProperty eventProperty = property.FindPropertyRelative( "m_event" );
            SerializedProperty injectorProperty = property.FindPropertyRelative( "m_injector" );

            m_eventField = new PropertyField( eventProperty );
            m_eventField.style.flexGrow = 1;
            m_eventField.label = string.Empty;
            m_eventField.style.marginRight = 1.5f;

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

            container.Add( m_eventField );
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
            SEReferenceMode mode = ( SEReferenceMode )( property.enumValueIndex + 1 );
            switch ( mode )
            {
                case SEReferenceMode.Event:
                    m_eventField.style.display = DisplayStyle.Flex;
                    m_injectorField.style.display = DisplayStyle.None;
                    break;
                case SEReferenceMode.Injector:
                    m_eventField.style.display = DisplayStyle.None;
                    m_injectorField.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}
