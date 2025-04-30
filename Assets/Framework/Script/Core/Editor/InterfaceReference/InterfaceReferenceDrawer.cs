using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    public struct InterfaceArgs
    {
        public Type @Interface;
        public Type Raw;

        public InterfaceArgs( Type @interface, Type raw )
        {
            Assert.IsTrue( @interface == null || @interface.IsInterface );
            Assert.IsTrue( raw == null || raw.InheritsFrom<UnityEngine.Object>() );
            @Interface = @interface;
            Raw = raw;
        }
    }


    [CustomPropertyDrawer( typeof( InterfaceReference<,> ), true )]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            return new InterfaceReferenceField( property, fieldInfo );
        }
    }


    public class InterfaceReferenceField : VisualElement
    {
        private readonly SerializedProperty m_rawObjectProperty;
        private readonly ObjectField m_field;
        private readonly Label m_displayLabel;
        private readonly InterfaceArgs m_interfaceArgs;

        public string Label
        {
            get => m_field.label;
            set => m_field.label = value;
        }

        public InterfaceReferenceField( SerializedProperty property, FieldInfo fieldInfo )
        {
            m_interfaceArgs = GetArguments( fieldInfo );

            m_rawObjectProperty = property.FindPropertyRelative( "m_rawObject" );


            m_field = new ObjectField( property.displayName );
            m_displayLabel = m_field.Q<Label>( className: "unity-object-field-display__label" );
            this.TrackPropertyValue( m_rawObjectProperty, RawPropertyChangeHandler );
            m_field.allowSceneObjects = true;
            m_field.value = m_rawObjectProperty.objectReferenceValue;
            m_field.RegisterCallback<ChangeEvent<UnityEngine.Object>>( ReferenceChangeHandler );
            m_displayLabel.parent.RegisterCallback<DragEnterEvent>( DragEnterHandler, useTrickleDown: TrickleDown.TrickleDown );
            m_displayLabel.parent.RegisterCallback<DragExitedEvent>( DragExitHandler, useTrickleDown: TrickleDown.TrickleDown );
            m_displayLabel.parent.RegisterCallback<DragUpdatedEvent>( DragUpdateHandler, useTrickleDown: TrickleDown.TrickleDown );

            Add( m_field );
            CorrectDisplayIfNullRef();
            schedule.Execute( CorrectDisplayIfNullRef ).Every( 1000 ).ExecuteLater( 1 );
        }


        private static InterfaceArgs GetArguments( FieldInfo info )
        {
            Type fieldType = info.FieldType;

            if ( !TryGetTypesFromInterfaceReference( fieldType, out Type rawType, out Type interfaceType ) )
            {
                GetTypesFromList( fieldType, out rawType, out interfaceType );
            }

            return new InterfaceArgs( interfaceType, rawType );
            bool TryGetTypesFromInterfaceReference( Type type, out Type raw, out Type @interface )
            {
                raw = @interface = null;
                if ( type?.IsGenericType != true )
                {
                    return false;
                }

                Type genericType = type.GetGenericTypeDefinition();
                if ( genericType == typeof( InterfaceReference<> ) )
                {
                    type = type.BaseType;
                }

                if ( type?.GetGenericTypeDefinition() == typeof( InterfaceReference<,> ) )
                {
                    Type[] types = type.GetGenericArguments();
                    @interface = types[0];
                    raw = types[1];
                    return true;
                }

                return false;
            }


            void GetTypesFromList( Type type, out Type raw, out Type @interface )
            {
                raw = @interface = null;

                Type listInterface = type?.GetInterfaces()
                        .FirstOrDefault( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof( IList<> ) );

                if ( listInterface != null )
                {
                    Type elementType = listInterface.GetGenericArguments()[0];
                    TryGetTypesFromInterfaceReference( elementType, out raw, out @interface );
                }
            }
        }


        private void DragUpdateHandler( DragUpdatedEvent evt )
        {
            DragEventEvaluate( evt );
        }


        private void DragExitHandler( DragExitedEvent _ )
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void DragEnterHandler( DragEnterEvent evt )
        {
            DragEventEvaluate( evt );
        }


        private void DragEventEvaluate( EventBase @event )
        {
            if ( DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length <= 0 )
            {
                return;
            }

            if ( MeetConditions( DragAndDrop.objectReferences[0], out UnityEngine.Object _ ) )
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                @event.StopImmediatePropagation();
            }
        }


        private bool MeetConditions( UnityEngine.Object @object, out UnityEngine.Object newObject )
        {
            newObject = @object;
            if ( @object == null )
            {
                return false;
            }

            Type objectType = @object.GetType();
            if ( !objectType.InheritsFrom( m_interfaceArgs.Raw ) )
            {
                return false;
            }

            if ( m_interfaceArgs.Interface.IsAssignableFrom( objectType ) )
            {
                return true;
            }

            if ( @object is not GameObject gameObject )
            {
                return false;
            }

            if ( gameObject.TryGetComponent( m_interfaceArgs.Interface, out Component component ) )
            {
                newObject = component;
                return true;
            }

            return false;
        }


        private void ReferenceChangeHandler( ChangeEvent<UnityEngine.Object> evt )
        {
            UnityEngine.Object newObject = evt.newValue;
            if ( newObject == null || MeetConditions( newObject, out newObject ) )
            {
                m_rawObjectProperty.objectReferenceValue = newObject;
                m_rawObjectProperty.serializedObject.ApplyModifiedProperties();
                m_rawObjectProperty.serializedObject.Update();
            }
            else
            {
                m_field.SetValueWithoutNotify( m_rawObjectProperty.objectReferenceValue );
            }

            CorrectDisplayIfNullRef();
        }


        private void CorrectDisplayIfNullRef()
        {
            if ( m_field.value == null || m_rawObjectProperty.objectReferenceValue == null )
            {
                m_displayLabel.text = $"None ({m_interfaceArgs.Raw.GetPrettyName()} : {m_interfaceArgs.Interface.GetPrettyName()})";
            }
        }


        private void RawPropertyChangeHandler( SerializedProperty property )
        {
            if ( property.objectReferenceValue == null )
            {
                m_displayLabel.text = $"None ({m_interfaceArgs.Raw.GetPrettyName()} : {m_interfaceArgs.Interface.GetPrettyName()})";
            }
        }
    }
}
