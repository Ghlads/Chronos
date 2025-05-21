using Framework.Core.Editor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Core
{
    [CustomPropertyDrawer( typeof( Actyx<,,,> ), true )]
    public class ActyxDrawer : PropertyDrawer
    {
        [SerializeField] private VisualTreeAsset m_visualTreeAsset;


        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            VisualElement root = new();
            ActionGenerator.GetOrCreateSchema( property.GetAssetPath(), property, property.GetRootObjects(), ActyxPropertyToID( property ), schema =>
            {
                TemplateContainer element = m_visualTreeAsset.Instantiate();
                schema.Name = property.displayName;
                schema.ActionProperty = property;
                schema.ActionTypes = fieldInfo.FieldType;
                element.dataSource = schema;
                HandleActionElement( element, schema );
                root.Add( element );
            } );
            return root;
        }


        private static void OverrideActionID( ActionSchema schema )
        {
            if ( !schema.IsOverridable )
            {
                return;
            }

            if ( schema.IsOverridable && schema.IsOverridden )
            {
                Debug.Log( "Already overriden" );
                return;
            }

            SerializedProperty guidProperty = schema.ActionProperty.FindPropertyRelative( "m_id" ).FindPropertyRelative( "m_guid" );
            ID previousID = new ID ( guidProperty.stringValue );
            ID newID = new ID ();
            guidProperty.stringValue = newID.ToString();
            ActionGenerator.OverrideActionForID( schema.ActionProperty.GetAssetPath(), previousID, newID );
            guidProperty.ApplyModificationAndUpdate();
        }


        public static void HandleActionElement( VisualElement element, ActionSchema schema )
        {
            Type[] input = schema.ActionTypes.GetGenericArguments();
            ListView list = element.Q<ListView>();
            list.makeItem = () =>
            {
                VisualElement element = list.itemTemplate.Instantiate();
                element.Q<Toggle>( name: "foldout-toggle" ).AddFoldoutLogic( element.Q( name: "container" ) );
                return element;
            };
            list.bindItem = ( itemElement, index ) =>
            {
                BindModifierElement( itemElement, schema, schema.ModifierSchemas[index], input, schema.ActionProperty );
            };
            list.itemsAdded += ( indices ) =>
            {
                foreach ( int index in indices )
                {
                    schema.ModifierSchemas[index] = new();
                    schema.ModifierSchemas[index].Index = index;
                }

                schema.ActionProperty.FindPropertyRelative( "m_constantArguments" ).arraySize = list.itemsSource.Count;
                schema.ActionProperty.ApplyModificationAndUpdate();
                OverrideActionID( schema );
            };
            list.itemsRemoved += ( indices ) =>
            {
                schema.ActionProperty.FindPropertyRelative( "m_constantArguments" ).arraySize = list.itemsSource.Count;
                schema.ActionProperty.ApplyModificationAndUpdate();
                OverrideActionID( schema );
            };

            if ( schema.ActionProperty.FindPropertyRelative( "m_constantArguments" ).arraySize != schema.ModifierSchemas.Count )
            {
                schema.ActionProperty.FindPropertyRelative( "m_constantArguments" ).arraySize = schema.ModifierSchemas.Count;
                schema.ActionProperty.ApplyModificationAndUpdate();
                OverrideActionID( schema );
            }

            list.itemsSource = schema.ModifierSchemas;
        }


        private struct ActionContainer
        {
            public Action Action;
        }


        public static void BindModifierElement( VisualElement element, ActionSchema action, ModifierSchema schema, Type[] input, SerializedProperty actionProperty )
        {
            element.dataSource = schema;


            ParameterInfo[] parametersInfo = schema.Method.Method.GetParameters();
            actionProperty.FindPropertyRelative( "m_constantArguments" )
                .GetArrayElementAtIndex( schema.Index )
                .FindPropertyRelative( "Args" ).arraySize = parametersInfo.Length;

            actionProperty.ApplyModificationAndUpdate();
            if ( parametersInfo.Length != schema.Parameters.Count )
            {
                InitializeParameters( schema.Parameters, parametersInfo );
            }
            else
            {
                for ( int index = 0; index < parametersInfo.Length; index++ )
                {
                    schema.Parameters[index].Name = parametersInfo[index].Name;
                    schema.Parameters[index].ExpectedType = parametersInfo[index].ParameterType;
                    schema.Parameters[index].ConstIndex = index;
                }
            }

            // parameter toggle
            if ( schema.Parameters.Count > 0 )
            {
                element.Q<Toggle>( name: "foldout-toggle" ).Display();
            }
            else
            {
                Toggle toggle = element.Q<Toggle>( name: "foldout-toggle" );
                toggle.Hide();
                toggle.value = false;
            }

            // Is Light
            VisualElement isLightElement = element.Q( name: "is-light" );
            isLightElement.RegisterCallback<ChangeEvent<bool>>( evt =>
            {
                if ( evt.newValue != schema.IsLight )
                {
                    EditorUtility.SetDirty( actionProperty.serializedObject.targetObject );
                    OverrideActionID( action );
                }
            } );
            if ( schema.Method.Method.ReturnType == typeof( void ) )
            {
                schema.IsLight = false;
                isLightElement.Hide();
            }
            else
            {
                isLightElement.Display();
            }



            //parameter view
            ListView list = element.Q<ListView>();
            list.bindItem = ( itemElement, index ) =>
            {
                BindParameterElement( itemElement, action, schema, input, schema.Parameters.Count > 0 ? schema.Parameters[index] : null, actionProperty );
            };


            list.itemsSource = schema.Parameters;
            Button button = element.Q<Button>( name: "function-selector" );
            if ( button.userData != null ) // ugly but quicker than a full refactor to OOP
            {
                button.clicked -= ( ( ActionContainer )button.userData ).Action;
            }

            button.clicked += FunctionSelectorHandler;
            button.userData = new ActionContainer() { Action = FunctionSelectorHandler };
            void FunctionSelectorHandler()
            {
                ModalListSelectorUtils.OpenActionSelectorWindow( method =>
                {
                    if ( method == null || method == schema.Method.Method )
                    {
                        return;
                    }


                    schema.Method.Method = method;
                    ParameterInfo[] parametersInfo = schema.Method.Method.GetParameters();
                    actionProperty.FindPropertyRelative( "m_constantArguments" )
                    .GetArrayElementAtIndex( schema.Index )
                    .FindPropertyRelative( "Args" ).arraySize = parametersInfo.Length;

                    actionProperty.ApplyModificationAndUpdate();
                    InitializeParameters( schema.Parameters, parametersInfo );
                    list.itemsSource = schema.Parameters;
                    list.Rebuild();

                    // parameter toggle
                    if ( schema.Parameters.Count > 0 )
                    {
                        element.Q<Toggle>( name: "foldout-toggle" ).Display();
                    }
                    else
                    {
                        Toggle toggle = element.Q<Toggle>( name: "foldout-toggle" );
                        toggle.Hide();
                        toggle.value = false;
                    }

                    // Is Light
                    if ( schema.Method.Method.ReturnType == typeof( void ) )
                    {
                        schema.IsLight = false;
                        element.Q( name: "is-light" ).Hide();
                    }
                    else
                    {
                        element.Q( name: "is-light" ).Display();
                    }

                    EditorUtility.SetDirty( actionProperty.serializedObject.targetObject );
                    OverrideActionID( action );
                } );
            }
            ;
        }


        private static void InitializeParameters( List<ParameterSchema> parametersSchema , ParameterInfo[] parametersInfo )
        {
            parametersSchema.Clear();
            for ( int index = 0; index < parametersInfo.Length; index++ )
            {
                ParameterInfo info = parametersInfo[index];
                ParameterSchema parameterSchema = new();
                parameterSchema.ExpectedType = info.ParameterType;
                parameterSchema.Source = AnyValueUtils.IsTypeSupported( info.ParameterType ) ? ParameterSchema.Sources.Const : ParameterSchema.Sources.Return;
                parameterSchema.Index = 0;
                parameterSchema.ConstIndex = index;
                parameterSchema.Name = info.Name;
                parametersSchema.Add( parameterSchema );
            }
        }


        public static void BindParameterElement( VisualElement element, ActionSchema action, ModifierSchema modifier, Type[] input, ParameterSchema schema, SerializedProperty actionProperty )
        {
            element.dataSource = schema;

            VisualElement anyValueFieldContainer = element.Q( name: "any-value-field-container" );
            Button indexSelector = element.Q<Button>( name: "index-selector" );

            AnyValueField anyValueField = null;
            if ( AnyValueUtils.IsTypeSupported( schema.ExpectedType ) )
            {
                anyValueField = anyValueFieldContainer.Q<AnyValueField>( name: "any-value-field" );
                if ( anyValueField != null )
                {
                    anyValueFieldContainer.Remove( anyValueField );
                }

                anyValueField = AnyValueField.RestrictedField( actionProperty.FindPropertyRelative( "m_constantArguments" )
                    .GetArrayElementAtIndex( modifier.Index )
                    .FindPropertyRelative( "Args" )
                    .GetArrayElementAtIndex( schema.ConstIndex ), schema.ExpectedType );
                anyValueFieldContainer.Add( anyValueField );
                anyValueField.name = "any-value-field";
            }

            // const or indexed
            UpdateFieldVisibility( schema.Source, anyValueFieldContainer, indexSelector );

            EnumField field = element.Q<EnumField>( name: "source-field" );
            field.RegisterCallback<ChangeEvent<Enum>>( evt =>
            {
                ParameterSchema.Sources newValue = ( ParameterSchema.Sources )evt.newValue;
                if ( newValue == ParameterSchema.Sources.Const && !AnyValueUtils.IsTypeSupported( schema.ExpectedType ) )
                {
                    field.SetValueWithoutNotify( evt.previousValue );
                    schema.Source = ( ParameterSchema.Sources )evt.previousValue;
                    return;
                }

                if ( newValue != ParameterSchema.Sources.Const )
                {
                    List<int> list = GetValidIndexForSource( newValue, action, modifier, input, schema );
                    schema.Index = list.Count <= 0 ? -1 : list.FirstOrDefaultNoException();
                }

                UpdateFieldVisibility( newValue, anyValueFieldContainer, indexSelector );
                if ( newValue != schema.Source )
                {
                    EditorUtility.SetDirty( actionProperty.serializedObject.targetObject );
                    OverrideActionID( action );
                }
            } );

            if ( indexSelector.userData != null )
            {
                indexSelector.clicked -= ( ( ActionContainer )indexSelector.userData ).Action;
            }
            indexSelector.clicked += IndexSelectorHandler;
            indexSelector.userData = new ActionContainer() { Action = IndexSelectorHandler };

            void IndexSelectorHandler()
            {
                ModalListSelectorUtils.OpenSourceIndexSelectorWindow(
                    schema.Source,
                    GetValidIndexForSource( schema.Source, action, modifier, input, schema ),
                    newIndex => 
                        {
                            schema.Index = newIndex;
                            EditorUtility.SetDirty( actionProperty.serializedObject.targetObject );
                            OverrideActionID( action );
                        } );
            }
        }


        private static List<int> GetValidIndexForSource( ParameterSchema.Sources source, ActionSchema action, ModifierSchema modifier, Type[] input, ParameterSchema schema )
        {
            List<int> indices = new List<int>( schema.Source == ParameterSchema.Sources.Return ? modifier.Index : input.Length );
            if ( source == ParameterSchema.Sources.Return )
            {
                for ( int index = 0; index < modifier.Index; index++ )
                {
                    if ( action.ModifierSchemas[index].Method.Method.ReturnType == schema.ExpectedType )
                    {
                        indices.Add( index );
                    }
                }
            }
            else
            {
                for ( int index = 0; index < input.Length; index++ )
                {
                    if ( input[index] == schema.ExpectedType )
                    {
                        indices.Add( index );
                    }
                }
            }

            return indices;
        }


        private static void UpdateFieldVisibility( ParameterSchema.Sources source, VisualElement anyValueFieldContainer, Button indexSelector )
        {
            if ( source == ParameterSchema.Sources.Const )
            {
                indexSelector.Hide();
                anyValueFieldContainer.Display();
            }
            else
            {
                indexSelector.Display();
                anyValueFieldContainer.Hide();
            }
        }


        public static ID ActyxPropertyToID( SerializedProperty property )
        {
            return new ID( property.FindPropertyRelative( "m_id" ).FindPropertyRelative( "m_guid" ).stringValue );
        }
    }
}
