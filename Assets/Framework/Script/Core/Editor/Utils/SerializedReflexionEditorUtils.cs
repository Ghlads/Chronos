using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core.Editor
{
    public static class SerializedReflexionEditorUtils
    {
        public static void SetSerializedMethod( this SerializedProperty property, MethodInfo info )
        {
            Assert.IsNotNull( property );
            SerializedProperty methodNameProperty = property.FindPropertyRelative( "m_methodName" );
            SerializedProperty declaringTypeProperty = property.FindPropertyRelative( "m_declaringType" );
            SerializedProperty isStaticProperty = property.FindPropertyRelative( "m_isStatic" );
            SerializedProperty parametersTypeProperty = property.FindPropertyRelative( "m_parametersType" );
            if ( info != null )
            {
                methodNameProperty.stringValue = info.Name;
                declaringTypeProperty.SetSerializedType( info.DeclaringType );  
                isStaticProperty.boolValue = info.IsStatic;
                parametersTypeProperty.SetArrayProperties( 
                    info.GetParameters(), 
                    setter: ( arrayProperty, paramInfo ) => arrayProperty.SetSerializedType( paramInfo.ParameterType ) );
            }
            else
            {
                methodNameProperty.stringValue = string.Empty;
                declaringTypeProperty?.SetSerializedType( null );
                isStaticProperty.boolValue = false;
                parametersTypeProperty.arraySize = 0;
            }
        }


        public static MethodInfo GetSerializedMethod( this SerializedProperty property )
        {
            Assert.IsNotNull( property );
            SerializedProperty methodNameProperty = property.FindPropertyRelative( "m_methodName" );
            SerializedProperty declaringTypeProperty = property.FindPropertyRelative( "m_declaringType" );
            SerializedProperty isStaticProperty = property.FindPropertyRelative( "m_isStatic" );
            SerializedProperty parametersTypeProperty = property.FindPropertyRelative( "m_parametersType" );
            
            string methodName = methodNameProperty.stringValue;
            Type declaringType = declaringTypeProperty?.GetSerializedType();
            bool isStatic = isStaticProperty.boolValue;
            Type[] parameters = parametersTypeProperty.GetArrayProperties( arrayProperty => arrayProperty?.GetSerializedType() );

            if ( declaringType == null || string.IsNullOrEmpty( methodName ) )
            {
                return null;
            }

            try
            {
                return declaringType.GetMethod(
                methodName,
                bindingAttr: isStatic ? SerializedMethod.STATIC_FLAGS : SerializedMethod.INSTANCE_FLAGS,
                binder: null,
                types: parameters,
                modifiers: new ParameterModifier[0] );
            }
            catch ( Exception e )
            {
                Debug.LogError( e );
                return null;
            }
        }


        public static void SetSerializedType( this SerializedProperty property, Type info )
        {
            Assert.IsNotNull( property );
            SerializedProperty nodesProperty = property.FindPropertyRelative( "m_serializedNode" );
            nodesProperty.SetArrayProperties<SerializedType.TypeNode>( 
                SerializedType.FlattenTypeToNodeList( info ), 
                ( prop, node ) => prop.SetSerializedNode( node ) );
        }


        public static void SetSerializedNode( this SerializedProperty property, SerializedType.TypeNode node )
        {
            Assert.IsNotNull( property );
            SerializedProperty assemblyNameProperty = property.FindPropertyRelative( "AssemblyQualifiedName" );
            SerializedProperty genericParamCountProperty = property.FindPropertyRelative( "GenericParametersCount" );
            SerializedProperty genericFirstIndexProperty = property.FindPropertyRelative( "FirstGenericParameterIndex" );
            assemblyNameProperty.stringValue = node.AssemblyQualifiedName;
            genericParamCountProperty.intValue = node.GenericParametersCount;
            genericFirstIndexProperty.intValue = node.FirstGenericParameterIndex;
        }


        public static Type GetSerializedType( this SerializedProperty property )
        {
            Assert.IsNotNull( property );
            SerializedProperty nodesProperty = property.FindPropertyRelative( "m_serializedNode" );
            SerializedType.TypeNode[] nodes = nodesProperty.GetArrayProperties<SerializedType.TypeNode>( prop => prop.GetSerializedNode() );
            return SerializedType.RecurseBuild( nodes.ToList(), 0 );
        }


        public static SerializedType.TypeNode GetSerializedNode( this SerializedProperty property )
        {
            Assert.IsNotNull( property );
            SerializedType.TypeNode node = new SerializedType.TypeNode();
            SerializedProperty assemblyNameProperty = property.FindPropertyRelative( "AssemblyQualifiedName" );
            SerializedProperty genericParamCountProperty = property.FindPropertyRelative( "GenericParametersCount" );
            SerializedProperty genericFirstIndexProperty = property.FindPropertyRelative( "FirstGenericParameterIndex" );

            node.AssemblyQualifiedName = assemblyNameProperty.stringValue;
            node.GenericParametersCount = genericParamCountProperty.intValue;
            node.FirstGenericParameterIndex = genericFirstIndexProperty.intValue;
            return node;
        }
    }
}
