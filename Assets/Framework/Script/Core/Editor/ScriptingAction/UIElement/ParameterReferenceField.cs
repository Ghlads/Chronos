using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Framework.Core.StateMachine<Framework.Core.ParameterReference.SourceType>;

namespace Framework.Core.Editor
{
    public class SourceValueController 
    {
        public class InvalidSource : ISourceReference
        {
            public int Index => -1;

            public Type Type => typeof( void );

            public string GetDisplayString()
            {
                return string.Empty;
            }
        }

        private readonly int m_maxIndexAllowed = -1;
        private readonly Func<ParameterReference.SourceType, int, List<ISourceReference>> m_sourceGetter;
        
        public SourceValueController( Func<ParameterReference.SourceType, int, List<ISourceReference>> sourceGetter, int maxIndexAllowed = -1 )
        {
            m_sourceGetter = sourceGetter;
            m_maxIndexAllowed = maxIndexAllowed;
        }


        public ISourceReference GetReference( ParameterReference.SourceType type, int index )
        {
            List<ISourceReference> references = m_sourceGetter( type, m_maxIndexAllowed );
            if ( references == null )
            {
                return new InvalidSource();
            }
            return index >= 0 && index < references.Count ? references[index] : new InvalidSource();
        }


        public void OpenSelector( ParameterReference.SourceType type, Type restrictedType, Action<ISourceReference> onSelectCallback )
        {
            SourceSelectorWindow.OpenSelection( m_sourceGetter( type, m_maxIndexAllowed )
                .NewFiltered( reference => reference.Type == restrictedType || restrictedType.IsAssignableFrom( reference.Type ) )
                , onSelectCallback );
        }
    }


    public class ParameterReferenceField : VisualElement
    {
        public const string ELEMENT_CLASS = "parameter-reference-field";
        public const string SOURCE_TYPE_CLASS = ELEMENT_CLASS + "__" + SourceTypeField.ELEMENT_CLASS;
        public const string ANY_VALUE_CLASS = ELEMENT_CLASS + "__" + AnyValueField.ELEMENT_CLASS;
        public const string SELECT_SOURCE_BUTTON_CLASS = ELEMENT_CLASS + "__select-source__button";
        public const string ACTION_CONTAINER_CLASS = "action-container";
        public const string ACTION_LABEL_CLASS = "selected-label";
        public const string ACTION_INDICATOR_CLASS = "indicator";

        private readonly VisualElement m_sourceTypeFieldContainer;
        private readonly VisualElement m_valueFieldContainer;

        private readonly Label m_label;
        private readonly InternalDisplayField m_displayField;
        private readonly Button m_selectSource;
        private readonly Label m_sourceLabel;

        private readonly StateMachine<ParameterReference.SourceType> m_stateMachine;

        private SerializedProperty m_property;
        private SerializedProperty m_sourceTypeProperty;
        private SerializedProperty m_anyValueProperty;
        private SerializedProperty m_sourceIndexProperty;

        private VisualElement m_propertyTracker = null;
        private SourceValueController m_controller = null;
        private Type m_expectedType;

        public string Label
        {
            get => m_label.text;
            set
            {
                m_label.text = value;
                if ( string.IsNullOrEmpty( value ) )
                {
                    m_label.style.display = DisplayStyle.None;
                }
            }
        }


        private class InternalDisplayField : VisualElement
        {
            public InternalDisplayField( ParameterReferenceField owner )
            {
                Add( owner.m_sourceTypeFieldContainer );
                Add( owner.m_valueFieldContainer );
                Add( owner.m_selectSource );
                style.flexDirection = FlexDirection.Row;
                style.flexGrow = 1;
                style.alignItems = Align.Center;
            }
        }


        public ParameterReferenceField( /*SerializedProperty parameterProperty*/ )
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Core/Editor/ScriptingAction/USS/ScriptingActionSheet.uss" );
            styleSheets.Add( styleSheet );
            

            // Field
            m_label = new Label();
            m_sourceTypeFieldContainer = new VisualElement();
            m_valueFieldContainer = new VisualElement();
            m_selectSource = new Button();
            m_sourceLabel = new( "select source" );
            VisualElement sourceInternalContainer = new();
            VisualElement sourceIndicator = new();

            m_valueFieldContainer.Hide();
            m_selectSource.Hide();


            // Property


            // Callback
            m_selectSource.RegisterCallback<ClickEvent>( _ =>
            {
                ParameterReference.SourceType source = ( ParameterReference.SourceType )m_sourceTypeProperty.enumValueIndex;
                if ( source == ParameterReference.SourceType.Value )
                {
                    return;
                }

                m_controller.OpenSelector( source, m_expectedType, SourceSelectionHandler );
            } );

            // Class
            AddToClassList( ELEMENT_CLASS );
            m_sourceTypeFieldContainer.AddToClassList( SOURCE_TYPE_CLASS );
            m_valueFieldContainer.AddToClassList( ANY_VALUE_CLASS );
            m_selectSource.AddToClassList( SELECT_SOURCE_BUTTON_CLASS );
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;
            sourceInternalContainer .AddToClassList( ACTION_CONTAINER_CLASS );
            m_sourceLabel.AddToClassList( ACTION_LABEL_CLASS );
            sourceIndicator.AddToClassList( ACTION_INDICATOR_CLASS );

            // Layout
            Add( m_label );
            Add( m_displayField = new InternalDisplayField( this ) );
            m_selectSource.Add( sourceInternalContainer );
            sourceInternalContainer.Add( m_sourceLabel );
            sourceInternalContainer.Add( sourceIndicator );

            // StateMachine
            m_stateMachine = CreateStateMachine();
        }


        private void SourceSelectionHandler( ISourceReference reference )
        {
            if ( reference.Type != m_expectedType )
            {
                reference = new SourceValueController.InvalidSource();
            }

            m_sourceIndexProperty.intValue = reference.Index;
            m_sourceLabel.text = reference.Index == -1 ? "select source" : reference.GetDisplayString();
            m_sourceIndexProperty.ApplyModificationAndUpdate();
        }


        public void Bind( SerializedProperty property, SourceValueController controller )
        {
            m_controller = controller;
            m_property = property;
            m_sourceTypeProperty = m_property.FindPropertyRelative( "m_source" );
            m_anyValueProperty = m_property.FindPropertyRelative( "m_value" );
            m_sourceIndexProperty = m_property.FindPropertyRelative( "m_returnValueIndex" );

            m_sourceTypeFieldContainer.Clear();
            m_valueFieldContainer.Clear();
            m_expectedType = ReflexionUtils.FindTypesByFullName( m_property.FindPropertyRelative( "m_expectedTypeFullName" ).stringValue ).FirstOrDefaultNoException();
            m_sourceTypeFieldContainer.Add( new SourceTypeField( m_sourceTypeProperty, m_expectedType ) );
            m_valueFieldContainer.Add( AnyValueField.RestrictedField( m_anyValueProperty, m_expectedType ) );

            Add( m_propertyTracker = new() );
            m_propertyTracker.TrackPropertyValue( m_sourceTypeProperty, property =>
            {
                m_stateMachine.ChangeState( ( ParameterReference.SourceType )property.enumValueIndex );
                SourceSelectionHandler( new SourceValueController.InvalidSource() );
            } );

            ParameterReference.SourceType source = ( ParameterReference.SourceType )m_sourceTypeProperty.enumValueIndex;
            m_stateMachine.ChangeState( source );

            SourceSelectionHandler( m_controller.GetReference( source, m_sourceIndexProperty.intValue ) );
        }


        public void Unbind()
        {
            Remove( m_propertyTracker );
            m_propertyTracker = null;
            m_sourceTypeFieldContainer.Clear();
            m_valueFieldContainer.Clear();
        }


        private StateMachine<ParameterReference.SourceType> CreateStateMachine()
        {
            return new Builder(
                new State
                (
                    ParameterReference.SourceType.Value,
                    _ => m_valueFieldContainer.Display(),
                    _ => m_valueFieldContainer.Hide(),
                    _ => ParameterReference.SourceType.Value
                ) )
                .AddState(
                new State
                (
                    ParameterReference.SourceType.Return,
                    _ => m_selectSource.Display(),
                    _ => m_selectSource.Hide(),
                    _ => ParameterReference.SourceType.Return
                ) )
                .AddState(
                new State
                (
                    ParameterReference.SourceType.Input,
                    _ => m_selectSource.Display(),
                    _ => m_selectSource.Hide(),
                    _ => ParameterReference.SourceType.Input
                ) )
                .Build();
        }


        public static ParameterReference.SourceType TypeToSourceType( Type type )
        {
            if ( AnyValueUtils.ValidTypeForAnyValue.Contains( type ) || type.InheritsFrom<UnityEngine.Object>() )
            {
                return ParameterReference.SourceType.Value;
            }

            return ParameterReference.SourceType.Input;
        }
    }
}
