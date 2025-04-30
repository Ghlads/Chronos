using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core
{
    [Serializable]
    public struct SerializedType : ISerializationCallbackReceiver, IRecoverable
    {
        [Serializable]
        public struct TypeNode
        {
            public string AssemblyQualifiedName;
            public int GenericParametersCount;
            public int FirstGenericParameterIndex;
        }

        [SerializeField][HideInInspector] private List<TypeNode> m_serializedNode;
        [NonSerialized] private bool m_isInRecoveryMode;
        [NonSerialized] private Type m_type;

        public Type Type
        {
            get => m_type;
            set
            {
                if ( m_type != value )
                {
                    ResetRecovery();
                    m_type = value;
                }
            }
        }


        public bool IsInRecovery => m_isInRecoveryMode;


        public void ResetRecovery()
        {
            if ( m_isInRecoveryMode )
            {
                m_type = null;
                m_serializedNode = new List<TypeNode>( 0 );
                m_isInRecoveryMode = false;
            }
        }


        public void OnAfterDeserialize()
        {
            m_type = RecurseBuild( m_serializedNode, index: 0 );
            if ( m_type == typeof( void ) )
            {
                Debug.LogWarning( $"[SerializedType] couldn't retrieve type from assembly qualified name | recovery mode enabled, previous value won't be auto overriden you must assign a new value" );
                m_isInRecoveryMode = true;
            }
        }


        public static Type RecurseBuild( List<TypeNode> nodes, int index )
        {
            if ( nodes == null || index < 0 || index >= nodes.Count || string.IsNullOrEmpty( nodes[index].AssemblyQualifiedName ) )
            {
                return null;
            }

            TypeNode node = nodes[index];
            Type baseType = ReflexionUtils.GetTypeByAssemblyName( node.AssemblyQualifiedName );
            if ( baseType == null )
            {
                UnityEngine.Debug.LogError( $"[SerializedType] Could not load type: {node.AssemblyQualifiedName}" );
                return typeof( void );
            }

            if ( node.GenericParametersCount == 0 )
            {
                Assert.IsFalse( baseType.IsGenericType );
                return baseType;
            }

            Type[] genericArgs = new Type[node.GenericParametersCount];
            for ( int i = 0; i < node.GenericParametersCount; i++ )
            {
                int argIndex = node.FirstGenericParameterIndex + i;
                if ( argIndex >= nodes.Count )
                {
                    UnityEngine.Debug.LogError( $"[SerializedType] Invalid arg index: {argIndex}" );
                    return typeof( void );
                }

                genericArgs[i] = RecurseBuild( nodes, argIndex );
            }

            return baseType.MakeGenericType( genericArgs );
        }


        public static Type RecurseBuild( TypeNode[] nodes, int index )
        {
            if ( nodes == null || index < 0 || index >= nodes.Length || string.IsNullOrEmpty( nodes[index].AssemblyQualifiedName ) )
            {
                return null;
            }

            TypeNode node = nodes[index];
            Type baseType = ReflexionUtils.GetTypeByAssemblyName( node.AssemblyQualifiedName );
            if ( baseType == null )
            {
                UnityEngine.Debug.LogError( $"[SerializedType] Could not load type: {node.AssemblyQualifiedName}" );
                return typeof( void );
            }

            if ( node.GenericParametersCount == 0 )
            {
                Assert.IsFalse( baseType.IsGenericType );
                return baseType;
            }

            Type[] genericArgs = new Type[node.GenericParametersCount];
            for ( int i = 0; i < node.GenericParametersCount; i++ )
            {
                int argIndex = node.FirstGenericParameterIndex + i;
                if ( argIndex >= nodes.Length )
                {
                    UnityEngine.Debug.LogError( $"[SerializedType] Invalid arg index: {argIndex}" );
                    return typeof( void );
                }

                genericArgs[i] = RecurseBuild( nodes, argIndex );
            }

            return baseType.MakeGenericType( genericArgs );
        }


        public void OnBeforeSerialize()
        {
            if ( m_isInRecoveryMode )
            {
                return;
            }

            m_serializedNode = FlattenTypeToNodeList( Type );
        }


        public static List<TypeNode> FlattenTypeToNodeList( Type inType )
        {
            int index = 0;
            List<TypeNode> serializedNode = new List<TypeNode>();
            Queue<Type> typeToProcess = new Queue<Type>( new Type[] { inType } );
            while ( typeToProcess.Count > 0 )
            {
                Type type = typeToProcess.Dequeue();
                if ( type == null )
                {
                    continue;
                }

                TypeNode node = new();
                if ( type.IsGenericType )
                {
                    node.AssemblyQualifiedName = type.GetGenericTypeDefinition().AssemblyQualifiedName;
                    Type[] genericArgs = type.GetGenericArguments();
                    node.GenericParametersCount = genericArgs.Length;
                    node.FirstGenericParameterIndex = index++ + typeToProcess.Count + 1;
                    foreach ( Type t in genericArgs )
                    {
                        typeToProcess.Enqueue( t );
                    }
                }
                else
                {
                    node.AssemblyQualifiedName = type.AssemblyQualifiedName;
                    node.GenericParametersCount = 0;
                    node.FirstGenericParameterIndex = -1;
                }

                serializedNode.Add( node );
            }

            return serializedNode;
        }


        public static implicit operator Type( SerializedType type )
        {
            return type.Type;
        }
    }
}
