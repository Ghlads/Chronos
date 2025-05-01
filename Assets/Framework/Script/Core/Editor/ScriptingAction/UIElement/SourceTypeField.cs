using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Framework.Core.ParameterReference;

namespace Framework.Core.Editor
{
    public class SourceTypeField : VisualElement
    {
        public const string ELEMENT_CLASS = "source-type-field";
        public const string BUTTON_CLASS = ELEMENT_CLASS + "__button";
        public const string SELECTED_ELEMENT_CLASS = ELEMENT_CLASS + "__selected";
        public const string DROPDOWN_INDICATOR_CLASS = ELEMENT_CLASS + "__indicator";
        public const string DROPDOWN_CLASS = ELEMENT_CLASS + "__generic-dropdown";
        public const string DROPDOWN_ELEMENT_CLASS = DROPDOWN_CLASS + "__element";

        private readonly Button m_fieldButton;
        private readonly VisualElement m_selectedElement;
        private readonly VisualElement m_dropdownIndicatorElement;
        private readonly GenericDropdown<SourceType> m_sourceTypeDropdown;
        private readonly SerializedProperty m_property;

        private readonly bool m_onlyRef;

        public SourceTypeField( SerializedProperty property, Type expectedType = null )
        {
            m_property = property;
            Add( m_fieldButton = new Button() );
            m_fieldButton.Add( m_selectedElement = new VisualElement() );
            m_fieldButton.Add( m_dropdownIndicatorElement = new VisualElement() );
            List<SourceType> list = ListUtils.MakeListFromEnum<SourceType>();
            m_onlyRef = false;
            if ( expectedType != null && !AnyValueUtils.IsTypeSupported( expectedType ) )
            {
                list.Remove( SourceType.Value );
                m_onlyRef = true;
            }

            Add( m_sourceTypeDropdown = new GenericDropdown<SourceType>( 
                    list,
                    MakeTypeEntry,
                    BindEntry,
                    UnbindEntry
                ) );

            SourceType startingValue = ( SourceType )m_property.enumValueIndex;
            if ( !list.Contains( startingValue ) )
            {
                startingValue = list[0];
            }

            m_sourceTypeDropdown.SetSelected( EnumToListIndex( ( int )startingValue ) );
            TypeChangeHandler( startingValue );
            m_sourceTypeDropdown.OnSelectedChanged += TypeChangeHandler;
            m_fieldButton.RegisterCallback<ClickEvent>( ButtonClickHandler );

            this.TrackPropertyValue( m_property, PropertyChangeHandler );

            AddToClassList( ELEMENT_CLASS );
            m_fieldButton.AddToClassList( BUTTON_CLASS );
            m_selectedElement.AddToClassList( SELECTED_ELEMENT_CLASS );
            m_dropdownIndicatorElement.AddToClassList( DROPDOWN_INDICATOR_CLASS );
            m_sourceTypeDropdown.AddToClassList( DROPDOWN_CLASS );

            m_sourceTypeDropdown.style.position = Position.Absolute;
            m_sourceTypeDropdown.style.display = DisplayStyle.None;
        }


        private void PropertyChangeHandler( SerializedProperty property )
        {
            if ( property.enumValueIndex == ( int )m_sourceTypeDropdown.SelectedValue )
            {
                return;
            }

            m_sourceTypeDropdown.SetSelected( EnumToListIndex( property.enumValueIndex ) );
        }


        private int EnumToListIndex( int index )
        {
            return Mathf.Clamp( m_onlyRef ? index - 1 : index, 0, m_sourceTypeDropdown.Items.Count - 1 );
        }


        private void TypeChangeHandler( SourceType type )
        {
            foreach ( SourceType item in m_sourceTypeDropdown.Items )
            {
                m_selectedElement.RemoveFromClassList( item.ToString().ToLower() );
            }

            m_selectedElement.AddToClassList( type.ToString().ToLower() );
            m_sourceTypeDropdown.Hide( this );

            if ( m_property.enumValueIndex == ( int )m_sourceTypeDropdown.SelectedValue )
            {
                return;
            }

            m_property.enumValueIndex = ( int )type;
            m_property.serializedObject.ApplyModifiedProperties();
            m_property.serializedObject.Update();
        }


        private void ButtonClickHandler( ClickEvent @event )
        {
            if ( m_sourceTypeDropdown.style.display.value == DisplayStyle.None )
            {
                m_sourceTypeDropdown.Show( this );
            }
            else
            {
                m_sourceTypeDropdown.Hide( this );
            }
        }


        private VisualElement MakeTypeEntry()
        {
            VisualElement element = new VisualElement();
            VisualElement icon = new VisualElement();
            icon.name = "icon";
            element.Add( icon );
            element.AddToClassList( DROPDOWN_ELEMENT_CLASS );
            return element;
        }


        private void BindEntry( SourceType type, VisualElement element )
        {
            element.Q( name: "icon" ).AddToClassList( type.ToString().ToLower() );
        }


        private void UnbindEntry( SourceType type, VisualElement element )
        {
            element.Q( name: "icon" ).RemoveFromClassList( type.ToString().ToLower() );
        }
    }
}
