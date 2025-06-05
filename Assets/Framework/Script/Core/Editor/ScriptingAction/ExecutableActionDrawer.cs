using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    [CustomPropertyDrawer( typeof( ExecutableAction ), true )]
    public class ExecutableActionDrawer : PropertyDrawer
    {
        private Dictionary<string, Type> m_typeCache;

        public override VisualElement CreatePropertyGUI( SerializedProperty property )
        {
            VisualElement container = new VisualElement();
            container.style.marginBottom = 6;

            string label = property.displayName;
            Label labelElement = new Label( label ) { style = { unityFontStyleAndWeight = FontStyle.Bold } };
            container.Add( labelElement );

            InitTypeCache( fieldInfo.FieldType );

            Type currentType = GetTypeFromProperty( property );
            string currentTypeName = currentType?.Name ?? "ExecutableAction";

            PopupField<string> popup = new PopupField<string>( m_typeCache.Keys.ToList(), currentTypeName );
            popup.label = "Type";
            popup.RegisterValueChangedCallback( evt =>
            {
                Undo.RecordObject( property.serializedObject.targetObject, "Change SerializeReference type" );

                Type selectedType = m_typeCache[evt.newValue];
                property.managedReferenceValue = Activator.CreateInstance( selectedType );
                property.serializedObject.ApplyModifiedProperties();

                container.Clear();
                container.Add( CreatePropertyGUI( property ) );
            } );
            container.Add( popup );

            if ( property.managedReferenceValue != null )
            {
                SerializedProperty iterator = property.Copy();
                SerializedProperty end = iterator.GetEndProperty();

                bool enterChildren = true;
                while ( iterator.NextVisible( enterChildren ) && !SerializedProperty.EqualContents( iterator, end ) )
                {
                    PropertyField field = new PropertyField( iterator );
                    container.Add( field );
                    enterChildren = false;
                }
            }

            return container;
        }


        private void InitTypeCache( Type fieldType )
        {
            if ( m_typeCache != null && m_typeCache.Count > 0 )
            {
                return;
            }

            if ( fieldType.IsGenericType )
            {
                fieldType = fieldType.GetGenericArguments()[0];
            }

            m_typeCache = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( a => a.GetTypes() )
                .Where( t => !t.IsAbstract && fieldType.IsAssignableFrom( t ) )
                .ToDictionary( t => t.Name, t => t );
        }


        private Type GetTypeFromProperty( SerializedProperty property )
        {
            if ( string.IsNullOrEmpty( property.managedReferenceFullTypename ) )
            {
                return null;
            }

            string[] typeInfo = property.managedReferenceFullTypename.Split( ' ' );
            if ( typeInfo.Length != 2 )
            {
                return null;
            }

            string assemblyQualifiedName = $"{typeInfo[1]}, {typeInfo[0]}";
            return Type.GetType( assemblyQualifiedName );
        }
    }
}
