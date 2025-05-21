using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    [CustomPropertyDrawer( typeof( ScriptingAction<,,,> ), true )]
    public class ScriptingActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            return new ScriptingActionField( property, fieldInfo );
        }






    }

    public interface ISourceReference 
    {
        public string GetDisplayString();
        public int Index { get; }
        public Type Type { get; }
    }


    public struct InputReference : ISourceReference
    {
        private int m_index;
        private Type m_type;

        public InputReference( int index, Type type )
        {
            m_index = index;
            m_type = type;
        }

        public int Index => m_index;

        public Type Type => m_type;

        public string GetDisplayString()
        {
            return $"{m_type.Beautified().Name} : inputs[{m_index}]";
        }
    }


    public struct ReturnReference : ISourceReference
    {
        private int m_index;
        private MethodInfo m_method;

        public ReturnReference( int index, MethodInfo method )
        {
            m_index = index;
            m_method = method;
        }

        public int Index => m_index;

        public Type Type => m_method.ReturnType;

        public string GetDisplayString()
        {
            return $"{ActionSelectorWindow.MethodInfoToString( m_method )} : return[{m_index}]";
        }
    }


    public class ScriptingActionField : VisualElement
    {
        public const string ELEMENT_CLASS = "scripting-action-field";
        public const string HEADER_CLASS = ELEMENT_CLASS + "__header";
        public const string BODY_CLASS = ELEMENT_CLASS + "__body";
        public const string ACTION_DECLARATION_CLASS = ELEMENT_CLASS + "__action-declaration";
        public const string ACTION_LABEL_CLASS = ELEMENT_CLASS + "__selected";
        public const string ACTION_INDICATOR_CLASS = ELEMENT_CLASS + "__indicator";
        public const string MODIFIER_ADD_REMOVE_CLASS = ELEMENT_CLASS + "__add-remove-container";
        public const string MODIFIER_CLASS = ELEMENT_CLASS + "__modifier-container";
        public const string ADD_CLASS = "add";
        public const string REMOVE_CLASS = "remove";

        private readonly Label m_label;
        private readonly Button m_addModifierButton;
        private readonly Button m_removeModifierButton;
        private readonly Button m_generateAction;

        private readonly ObjectField m_targetField;
        private readonly Button m_selectActionButton;
        private readonly Label m_actionLabel;
        private readonly ListView m_actionParameterView;
        private readonly ListView m_modifierView;
        private readonly Type m_fieldType;

        
        private SerializedProperty m_property;
        private SerializedProperty m_methodProperty;
        private SerializedProperty m_targetObjectProperty;
        private SerializedProperty m_parameterReferencesProperty;
        private SerializedProperty m_modifierReferencesProperty;


        private List<ParameterInfo> m_parameterInfos;
        private List<int> m_dummyModifierList;
        private List<ISourceReference> m_inputs = null;


        public ScriptingActionField( SerializedProperty property, FieldInfo fieldInfo )
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Core/Editor/ScriptingAction/USS/ScriptingActionSheet.uss" );
            styleSheets.Add( styleSheet );
            m_fieldType = fieldInfo.FieldType;

            //Field
            VisualElement header = new();
            m_label = new Label();

            VisualElement addRemoveContainer = new();
            m_addModifierButton = new Button();
            m_removeModifierButton = new Button();
            VisualElement body = new();
            VisualElement modifierContainer = new();
            VisualElement actionDeclarationContainer = new();
            GenericFoldout lastActionContainer = new( actionDeclarationContainer );
            m_targetField = new ObjectField();
            m_selectActionButton = new Button();
            m_actionLabel = new Label( "select action" );
            VisualElement indicatorElement = new VisualElement();
            m_actionParameterView = new ListView();
            m_actionParameterView.makeItem = MakeParameterItemElement;
            m_actionParameterView.bindItem = ( VisualElement element, int index ) =>
            {
                ParameterReferenceField field = element.Q<ParameterReferenceField>();
                field.Bind( m_parameterReferencesProperty.GetArrayElementAtIndex( index ), new SourceValueController( GetReferences, int.MaxValue ) );
                field.Label = string.Empty;
                element.Q<Label>( name: "param-name" ).text = m_parameterInfos[index].Name;
            };
            m_actionParameterView.unbindItem = ( VisualElement element, int _ ) =>
            {
                ParameterReferenceField field = element.Q<ParameterReferenceField>();
                field.Unbind();
                element.Q<Label>( name: "param-name" ).text = string.Empty;
            };

            m_modifierView = new ListView();
            m_modifierView.makeItem = () => new ModifierReferenceField();
            m_modifierView.bindItem = ( element, index ) =>
            {
                ModifierReferenceField field = element as ModifierReferenceField;
                field.Bind( m_modifierReferencesProperty.GetArrayElementAtIndex( index ), new SourceValueController( GetReferences, index ) );
            };
            m_modifierView.unbindItem = ( element, index ) =>
            {
                ModifierReferenceField field = element as ModifierReferenceField;
                field.Unbind();
            };
            m_modifierView.reorderable = true;
            m_modifierView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            m_generateAction = new Button();

            // Property
            if ( property != null )
            {
                Bind( property );
            }


            // Callback
            m_selectActionButton.RegisterCallback<ClickEvent>( SelectActionHandler );
            m_targetField.RegisterCallback<ChangeEvent<UnityEngine.Object>>( TargetChangeHandler );
            m_addModifierButton.RegisterCallback<ClickEvent>( AddModifierHandler );
            m_removeModifierButton.RegisterCallback<ClickEvent>( RemoveModifierHandler );
            m_generateAction.RegisterCallback<ClickEvent>( GenerateActionHandler );

            // Style
            m_label.style.overflow = Overflow.Hidden;
            header.style.flexDirection = FlexDirection.Row;
            actionDeclarationContainer.style.flexDirection = FlexDirection.Row;
            actionDeclarationContainer.style.flexGrow = 1;
            m_selectActionButton.style.flexDirection = FlexDirection.Row;

            AddToClassList( ELEMENT_CLASS );
            header.AddToClassList( HEADER_CLASS );
            body.AddToClassList( BODY_CLASS );
            addRemoveContainer.AddToClassList( MODIFIER_ADD_REMOVE_CLASS );
            actionDeclarationContainer.AddToClassList( ACTION_DECLARATION_CLASS );
            m_actionLabel.AddToClassList( ACTION_LABEL_CLASS );
            indicatorElement.AddToClassList( ACTION_INDICATOR_CLASS );
            m_addModifierButton.AddToClassList( ADD_CLASS );
            m_removeModifierButton.AddToClassList( REMOVE_CLASS );
            modifierContainer.AddToClassList( MODIFIER_CLASS );


            // Layout
            Add( header );
            Add( body );
            header.Add( m_label );
            addRemoveContainer.Add( m_addModifierButton );
            addRemoveContainer.Add( m_removeModifierButton );
            header.Add( addRemoveContainer );
            body.Add( modifierContainer );
            body.Add( lastActionContainer );
            body.Add( m_generateAction );
            modifierContainer.Add( m_modifierView );
            lastActionContainer.Add( m_actionParameterView );
            actionDeclarationContainer.Add( m_targetField );
            actionDeclarationContainer.Add( m_selectActionButton );
            m_selectActionButton.Add( m_actionLabel );
            m_selectActionButton.Add( indicatorElement );
        }


        private void GenerateActionHandler( ClickEvent evt )
        {

        }


        private void RemoveModifierHandler( ClickEvent _ )
        {
            int index = m_modifierView.selectedIndex;
            if ( index < 0 || index >= m_modifierReferencesProperty.arraySize )
            {
                index = m_modifierReferencesProperty.arraySize - 1;
            }

            m_modifierReferencesProperty.DeleteArrayElementAtIndex( index );
            m_dummyModifierList.RemoveAt( index );
            m_modifierView.itemsSource = m_dummyModifierList;
            m_modifierView.Rebuild();
        }


        private void AddModifierHandler( ClickEvent _ )
        {
            m_modifierReferencesProperty.InsertArrayElementAtIndex( m_modifierReferencesProperty.arraySize );
            m_dummyModifierList.Add( 0 );
            m_modifierReferencesProperty.ApplyModificationAndUpdate();
            m_modifierView.itemsSource = m_dummyModifierList;
            m_modifierView.Rebuild();
        }


        public static VisualElement MakeParameterItemElement()
        {
            VisualElement root = new();
            ParameterReferenceField field = new ParameterReferenceField();
            root.Add( field );
            Label paramNameLabel = new Label();
            paramNameLabel.name = "param-name";
            root.Add( paramNameLabel );
            field.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Row;
            root.style.justifyContent = Justify.SpaceBetween;
            root.style.alignItems = Align.Center;
            return root;
        }


        private void SetPropertyValueToFields()
        {
            if ( m_property != null )
            {
                m_label.text = DisplayNameAndTypeToString( m_property, m_fieldType );
                m_targetField.SetValueWithoutNotify( m_targetObjectProperty.objectReferenceValue );
                MethodInfo method = GetActionMethodInfo();
                ValidateActionName( method );
                ApplyCurrentMethodParamsToReference( method );
                m_dummyModifierList = new List<int>( m_modifierReferencesProperty.arraySize );
                for ( int index = 0; index < m_modifierReferencesProperty.arraySize; index++ )
                {
                    m_dummyModifierList.Add( 0 );
                }

                m_modifierView.itemsSource = m_dummyModifierList;
                m_modifierView.Rebuild();
            }
        }


        private void ApplyCurrentMethodParamsToReference( MethodInfo method = null )
        {
            SetMethodParamsToReference( method );
            m_parameterReferencesProperty.ApplyModificationAndUpdate();
        }


        private void SetMethodParamsToReference( MethodInfo method = null )
        {
            method ??= GetActionMethodInfo();
            if ( method is null )
            {
                m_parameterReferencesProperty.arraySize = 0;
                m_parameterReferencesProperty.ApplyModificationAndUpdate();
                return;
            }

            m_parameterInfos = new List<ParameterInfo>( method.GetParameters() );
            m_parameterReferencesProperty.arraySize = m_parameterInfos.Count;
            for ( int index = 0; index < m_parameterInfos.Count; index++ )
            {
                ParameterInfo info = m_parameterInfos[index];
                SerializedProperty referenceProperty = m_parameterReferencesProperty.GetArrayElementAtIndex( index );

                SerializedProperty expectedTypeName = referenceProperty.FindPropertyRelative( "m_expectedTypeFullName" );
                expectedTypeName.stringValue = info.ParameterType.FullName;

                if ( ParameterReferenceField.TypeToSourceType( info.ParameterType ) == ParameterReference.SourceType.Value )
                {
                    SerializedProperty valueTypeProperty = referenceProperty.FindPropertyRelative( "m_value" ).FindPropertyRelative( "m_type" );
                    int valueType = ( int )AnyValueUtils.TypeToEnum( info.ParameterType );
                    if ( valueTypeProperty.enumValueIndex != valueType )
                    {
                        valueTypeProperty.enumValueIndex = valueType;
                    }
                }
            }

            m_actionParameterView.itemsSource = m_parameterInfos;
        }


        private void TargetChangeHandler( ChangeEvent<UnityEngine.Object> @event )
        {
            if ( @event.newValue == m_targetObjectProperty.objectReferenceValue )
            {
                return;
            }

            m_targetObjectProperty.objectReferenceValue = @event.newValue;
            m_targetObjectProperty.ApplyModificationAndUpdate();
        }


        public void Bind( SerializedProperty property )
        {
            m_property = property;
            m_targetObjectProperty = m_property.FindPropertyRelative( "m_target" );
            m_methodProperty = m_property.FindPropertyRelative( "m_method" );
            m_parameterReferencesProperty = m_property.FindPropertyRelative( "m_parameterReferences" );
            m_modifierReferencesProperty = m_property.FindPropertyRelative( "m_modifierReferences" );
            
            TrackProperties();
            SetPropertyValueToFields();
        }


        private void TrackProperties()
        {
            this.TrackPropertyValue( m_targetObjectProperty, property =>
            {
                if ( property.objectReferenceValue != m_targetField.value )
                {
                    m_targetField.SetValueWithoutNotify( property.objectReferenceValue );
                    MethodInfo method = GetActionMethodInfo();
                    ValidateActionName( method );
                    ApplyCurrentMethodParamsToReference( method );
                }
            } );

            this.TrackPropertyValue( m_methodProperty, property => 
            {
                if ( m_targetObjectProperty.objectReferenceValue != null )
                {
                    MethodInfo method = GetActionMethodInfo();
                    ValidateActionName( method );
                    ApplyCurrentMethodParamsToReference( method );
                }
                // Handle utils case
            } );
        }


        private void ValidateActionName( MethodInfo method = null )
        {
            method ??= GetActionMethodInfo();
            if ( method != null )
            {
                m_actionLabel.text = ActionSelectorWindow.MethodInfoToString( method );
            }
            else
            {
                m_actionLabel.text = "select action";
                m_methodProperty.SetSerializedMethod( null );
                m_methodProperty.ApplyModificationAndUpdate();
            }
        }


        private MethodInfo GetActionMethodInfo()
        {
            return m_methodProperty.GetSerializedMethod();
        }


        public void Unbind()
        {

        }


        private void SelectActionHandler( ClickEvent _ )
        {
            ActionSelectorWindow.Open( m_targetField.value != null ? m_targetField.value.GetType() : null, method =>
            {
                m_methodProperty.SetSerializedMethod( method );
                method = GetActionMethodInfo();// Force update to validate in case 
                ValidateActionName( method );
                SetMethodParamsToReference( method );
                m_methodProperty.ApplyModificationAndUpdate();
            } );
        }


        public static string DisplayNameAndTypeToString( SerializedProperty property, Type fieldType )
        {
            StringBuilder builder = new StringBuilder();
            builder.Append( property.displayName ).Append( " " );
            Type[] genericArgs = fieldType.GetGenericArguments();
            using ( new ParenthesesWrapper( builder, genericArgs.Length > 0 && genericArgs[0] != typeof( NullStruct ) ) )
            {
                for ( int index = 0; index < genericArgs.Length - 1; index++ )
                {
                    Type type = genericArgs[ index ].Beautified();
                    if ( type != typeof( NullStruct ) )
                    {
                        builder.Append( index ).Append( ": " ).Append( type.Name ).Append( ", " );
                    }
                }

                if ( genericArgs.Length > 0 )
                {
                    Type lastType = genericArgs[genericArgs.Length - 1].Beautified();
                    if ( lastType != typeof( NullStruct ) )
                    {
                        builder.Append( genericArgs.Length - 1 ).Append( ": " ).Append( lastType.Name );
                    }
                }
            }    

            return builder.ToString();
        }


        private List<ISourceReference> GetReferences( ParameterReference.SourceType sourceType, int maxIndexAllowed )
        {
            if ( sourceType == ParameterReference.SourceType.Value )
            {
                return null;
            }

            if ( sourceType == ParameterReference.SourceType.Input )
            {
                if ( m_inputs != null )
                {
                    return m_inputs;
                }

                Type[] args = m_fieldType.GetGenericArguments();
                m_inputs = new List<ISourceReference>( args.Length );
                for ( int index = 0; index < args.Length; index++ )
                {
                    m_inputs.Add( new InputReference( index, args[index] ) );
                }

                return m_inputs;
            }
            else
            {
                int resolvedMaxIndex = Mathf.Clamp( maxIndexAllowed, -1, m_modifierReferencesProperty.arraySize );
                if ( resolvedMaxIndex < 0 )
                {
                    return new( 0 );
                }

                List<ISourceReference> returnRef = new List<ISourceReference>( resolvedMaxIndex );
                for ( int index = 0; index < resolvedMaxIndex; index++ )
                {
                    MethodInfo method = m_modifierReferencesProperty.GetArrayElementAtIndex( index ).FindPropertyRelative( "m_method" ).GetSerializedMethod();
                    returnRef.Add( new ReturnReference( index, method ) );
                }

                return returnRef;
            }
        }
    }

}
