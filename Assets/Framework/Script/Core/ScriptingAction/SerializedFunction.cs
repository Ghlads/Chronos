using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public class SerializedFunction<TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField] private UnityEngine.Object m_target;
        [SerializeField] private string m_functionName;
        [SerializeField] private AnyValue[] m_parameters;
        [NonSerialized] private Delegate m_function;
        [NonSerialized] private bool m_needsRebuild;

        public TReturn Call()
        {
            return Call( m_parameters );
        }


        public TReturn Call( AnyValue[] parameters ) 
        {
            if ( m_needsRebuild )
            {
                BuildFunction();
            }

            if ( m_function == null )
            {
                Debug.LogError( "No function to call" );
                return default;
            }

            return ( TReturn )Convert.ChangeType( m_function.DynamicInvoke( ConvertToObject( parameters ) ), typeof( TReturn ) );
        }


        private object[] ConvertToObject( AnyValue[] values )
        {
            object[] result = new object[values.Length];

            for ( int index = 0; index < values.Length; index++ )
            {
                result[index] = values[index].Get<object>();
            }

            return result;
        }


        private void BuildFunction()
        {
            m_function = null;
            if ( m_target == null || string.IsNullOrEmpty( m_functionName ) )
            {
                return;
            }

            Type targetType = m_target.GetType();
            MethodInfo methodInfo = targetType.GetMethod( m_functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
            if ( methodInfo == null )
            {
                Debug.LogWarning( "No method found" );
                return;
            }

            Type[] parameters = methodInfo.GetParameters().Select( p => p.ParameterType ).Append( methodInfo.ReturnType ).ToArray();
            Type functionType = Expression.GetDelegateType( parameters );
            m_function = methodInfo.CreateDelegate( functionType, m_target );
            m_needsRebuild = false;
        }


        public void OnAfterDeserialize()
        {
            m_needsRebuild = true;
        }

        public void OnBeforeSerialize() {}
    }


    [Serializable]
    public class SerializedFunction
    {
        [SerializeField] private SerializedFunction<object> m_internalSerialization;

        public void Call()
        {
            m_internalSerialization.Call();
        }


        public void Call( AnyValue[] parameters )
        {
            m_internalSerialization.Call( parameters );
        }


        public static implicit operator SerializedFunction<object>( SerializedFunction function )
        {
            return function.m_internalSerialization;
        }
    }
}
