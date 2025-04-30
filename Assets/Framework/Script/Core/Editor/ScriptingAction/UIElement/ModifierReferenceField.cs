using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static Framework.Core.ParameterReference;

namespace Framework.Core.Editor
{
    public class ModifierReferenceField : VisualElement
    {
        public const string ELEMENT_CLASS = "modifier-reference-field";
        public const string BUTTON_CLASS = ELEMENT_CLASS + "__button";

        private readonly Toggle m_isLightToggle;
        private readonly Button m_actionSelector;
        private readonly Label m_actionLabel;
        private readonly GenericFoldout m_foldout;
        private readonly ListView m_list;

        private SerializedProperty m_property;
        private SerializedProperty m_methodProperty;
        private SerializedProperty m_parametersProperty;
        private SerializedProperty m_isLightProperty;


        private List<ParameterInfo> m_parameterInfos;
        private SourceValueController m_controller;

        private VisualElement m_propertyTracker;

        public ModifierReferenceField()
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Core/Editor/ScriptingAction/USS/ScriptingActionSheet.uss" );
            styleSheets.Add( styleSheet );

            m_isLightToggle = new Toggle();
            m_actionSelector = new Button();
            m_actionLabel = new Label();
            m_foldout = new GenericFoldout( m_actionSelector );
            m_list = new ListView();
            VisualElement sourceContainer = new();
            VisualElement indicator = new();


            //Callback
            m_list.makeItem = () => ScriptingActionField.MakeParameterItemElement();
            m_list.bindItem = ( element, index ) => // utils candidate
            {
                ParameterReferenceField field = element.Q<ParameterReferenceField>();
                field.Bind( m_parametersProperty.GetArrayElementAtIndex( index ), m_controller );
                field.Label = string.Empty;
                element.Q<Label>( name: "param-name" ).text = m_parameterInfos[index].Name;
            };
            m_list.unbindItem = ( element, index ) =>// utlis candidate
            {
                ParameterReferenceField field = element.Q<ParameterReferenceField>();
                field.Unbind();
                element.Q<Label>( name: "param-name" ).text = string.Empty;
            };
            m_actionSelector.RegisterCallback<ClickEvent>( ActionSelectionHandler );
            m_isLightToggle.RegisterCallback<ChangeEvent<bool>>( @event =>
            {
                m_isLightProperty.boolValue = @event.newValue;
                m_isLightProperty.ApplyModificationAndUpdate();
            } );

            // Style
            AddToClassList( ELEMENT_CLASS );
            m_actionSelector.AddToClassList( BUTTON_CLASS );
            sourceContainer.AddToClassList( ParameterReferenceField.ACTION_CONTAINER_CLASS );
            m_actionLabel.AddToClassList( ParameterReferenceField.ACTION_LABEL_CLASS );
            indicator.AddToClassList( ParameterReferenceField .ACTION_INDICATOR_CLASS );

            // Layout
            m_actionSelector.Add( sourceContainer );
            sourceContainer.Add( m_actionLabel );
            sourceContainer.Add( indicator );
            m_foldout.Add( m_list );
            Add( m_foldout );
        }


        public void Bind( SerializedProperty property, SourceValueController controller )
        {
            m_property = property;
            m_controller = controller;

            m_isLightProperty = m_property.FindPropertyRelative( "m_isLight" );
            m_methodProperty = m_property.FindPropertyRelative( "m_method" );
            m_parametersProperty = m_property.FindPropertyRelative( "m_parameterReferences" );

            m_propertyTracker = new VisualElement();
            Add( m_propertyTracker );

            m_propertyTracker.TrackPropertyValue( m_isLightProperty, prop =>
            {
                m_isLightToggle.SetValueWithoutNotify( prop.boolValue );
            } );
            m_propertyTracker.TrackPropertyValue( m_methodProperty, prop =>
            {
                MethodInfo method = prop.GetSerializedMethod();
                ValidateActionName( method );
                ApplyCurrentMethodParamsToReference( method );
            } );

            m_isLightToggle.SetValueWithoutNotify( m_isLightProperty.boolValue );
            MethodInfo method = m_methodProperty.GetSerializedMethod();
            ValidateActionName( method );
            ApplyCurrentMethodParamsToReference( method );
        }


        public void Unbind()
        {
            Remove( m_propertyTracker );
            m_propertyTracker = null;
        }


        private void ActionSelectionHandler( ClickEvent evt ) // utils candidate
        {
            ActionSelectorWindow.Open( null, method =>
            {
                m_methodProperty.SetSerializedMethod( method );
                method = m_methodProperty.GetSerializedMethod();
                ValidateActionName( method );
                SetMethodParamsToReference( method );
                m_methodProperty.ApplyModificationAndUpdate();
            } );
        }


        private void ValidateActionName( MethodInfo method = null )// Utils candidate
        {
            method ??= m_methodProperty.GetSerializedMethod();
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


        private void ApplyCurrentMethodParamsToReference( MethodInfo method = null )// Utils candidate
        {
            SetMethodParamsToReference( method );
            m_parametersProperty.ApplyModificationAndUpdate();
        }


        private void SetMethodParamsToReference( MethodInfo method = null ) // Utils Candidate
        {
            method ??= m_methodProperty.GetSerializedMethod();
            if ( method is null )
            {
                m_parametersProperty.arraySize = 0;
                return;
            }

            m_parameterInfos = new List<ParameterInfo>( method.GetParameters() );
            m_parametersProperty.arraySize = m_parameterInfos.Count;
            for ( int index = 0; index < m_parameterInfos.Count; index++ )
            {
                ParameterInfo info = m_parameterInfos[index];
                SerializedProperty referenceProperty = m_parametersProperty.GetArrayElementAtIndex( index );

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

            m_list.itemsSource = m_parameterInfos;
        }
    }

}
