using System;
using System.Reflection;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public struct SerializedMethod : ISerializationCallbackReceiver, IRecoverable
    {
        public const BindingFlags STATIC_FLAGS = BindingFlags.Static | BindingFlags.Public;
        public const BindingFlags INSTANCE_FLAGS = BindingFlags.Instance | BindingFlags.Public;

        [SerializeField][HideInInspector] private string m_methodName;
        [SerializeField][HideInInspector] private SerializedType m_declaringType;
        [SerializeField][HideInInspector] private bool m_isStatic;
        [SerializeField][HideInInspector] private SerializedType[] m_parametersType;

        [NonSerialized] private bool m_isInRecovery;
        [NonSerialized] private MethodInfo m_method;

        public bool IsInRecovery => m_isInRecovery || m_declaringType.IsInRecovery || m_parametersType.HasAnItemInRecovery();

        public MethodInfo Method
        {
            get => m_method;
            set
            {
                if ( m_method != value )
                {
                    ResetRecovery();
                    m_method = value;
                }
            }
        }


        public void ResetRecovery()
        {
            if ( !IsInRecovery )
            {
                return;
            }

            m_isInRecovery = false;
            m_declaringType.Type = null;
            m_methodName = string.Empty;
            m_isStatic = false;
            m_parametersType = new SerializedType[0];
        }


        public void OnAfterDeserialize()
        {
            if ( IsInRecovery )
            {
                Debug.LogWarning( "[SerializedMethod] Recovery mode is enabled the depending types saved was deleted/renamed/moved. Will be null until another method is assign or a reset is done" );
                m_method = null;
                return;
            }


            if ( m_declaringType.Type == null || 
                string.IsNullOrEmpty( m_methodName ) )
            {
                m_method = null;
                return;
            }


            try
            {
                m_method = m_declaringType.Type.GetMethod( m_methodName, bindingAttr: m_isStatic ? STATIC_FLAGS : INSTANCE_FLAGS, binder: null, types: m_parametersType.ToTypeArray(), modifiers: new ParameterModifier[0] );
            }
            catch ( Exception ex )
            {
                Debug.LogException( ex );
                m_method = null;
            }

            if ( m_method == null )
            {
                Debug.LogWarning( $"[SerializedMethod] Couldn't get method from name : {m_methodName} on type : {m_declaringType.Type.FullName} with params : {m_parametersType.ToFullName()}" );
                m_isInRecovery = true;
            }
        }


        public void OnBeforeSerialize() 
        {
            if ( IsInRecovery )
            {
                return;
            }

            if ( m_method != null )
            {
                m_methodName = m_method.Name;
                m_declaringType.Type = m_method.DeclaringType;
                m_isStatic = m_method.IsStatic;
                ParameterInfo[] parameters = m_method.GetParameters();
                SerializedType[] serializedParameters = new SerializedType[parameters.Length];
                for ( int index = 0; index < parameters.Length; index++ )
                {
                    serializedParameters[index] = new SerializedType();
                    serializedParameters[index].Type = parameters[index].ParameterType;
                }

                m_parametersType = serializedParameters;
            }
            else
            {
                m_methodName = string.Empty;
                m_declaringType.Type = null;
                m_isStatic = false;
                m_parametersType = new SerializedType[0];
            }
        }


        public static implicit operator MethodInfo( SerializedMethod method )
        {
            return method.Method;
        }
    }


    public interface IRecoverable
    {
        bool IsInRecovery { get; }
        void ResetRecovery();
    }
}
