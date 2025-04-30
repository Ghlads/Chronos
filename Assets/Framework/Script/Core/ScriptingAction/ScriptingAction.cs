using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Framework.Core
{
    public struct NullStruct
    {
        public static NullStruct Default = default;
    }


    [System.Serializable]
    public class ScriptingAction<T1, T2, T3, T4> : ISerializationCallbackReceiver
    {
        [SerializeField] private UnityEngine.Object m_target;
        [SerializeField] private SerializedMethod m_method;
        [SerializeField] private List<ParameterReference> m_parameterReferences = new();
        [SerializeField] private List<ModifierReference> m_modifierReferences = new();

        [NonSerialized] private Action<T1, T2, T3, T4> m_cachedAction = null;
        [NonSerialized] private bool m_dirty = true;

        public void Invoke( T1 arg1, T2 arg2, T3 arg3, T4 arg4 )
        {
            if ( m_dirty )
            {
                Rebuild();
            }

            if ( m_cachedAction == null )
            {
                Debug.LogError( "[ScriptingAction] Couldn't invoke ScriptingAction error happened while building delegate" );
                return;
            }

            m_cachedAction.Invoke( arg1, arg2, arg3, arg4 );
        }


        private void Rebuild()
        {
            m_cachedAction = null;
            if ( !ValidateAction( out MethodInfo actionInfo ) )
            {
                return;
            }

            ParameterExpression[] inputs = {
                Expression.Parameter( typeof( T1 ), "arg1" ),
                Expression.Parameter( typeof( T2 ), "arg2" ),
                Expression.Parameter( typeof( T3 ), "arg3" ),
                Expression.Parameter( typeof( T4 ), "arg4" )
            };

            List<Expression> variables = new List<Expression>();
            List<Expression> body = new List<Expression>();

            foreach ( ModifierReference modifierRef in m_modifierReferences )
            {
                ScriptingActionUtils.BuildModifier( modifierRef, variables, body, inputs );
            }

            List<Expression> arguments = ScriptingActionUtils.BuildArguments( m_parameterReferences, variables, inputs );
            if ( actionInfo.IsStatic )
            {
                body.Add( Expression.Call( null, actionInfo, arguments ) );

            }
            else
            {
                body.Add( Expression.Call( Expression.Constant( m_target ), actionInfo, arguments ) );
            }

            Expression block = Expression.Block( variables.OfType<ParameterExpression>(), body );
            m_cachedAction = Expression.Lambda<Action<T1, T2, T3, T4>>( block, inputs ).Compile();
            m_dirty = false;
        }


        private bool ValidateAction( out MethodInfo actionInfo )
        {
            actionInfo = null;
            if ( m_method.Method == null )
            {
                return false;
            }

            if ( m_target == null && !m_method.Method.IsStatic )
            {
                Debug.LogError( "[ScriptingAction] no target" );
                return false;
            }

            actionInfo = m_method.Method;
            return true;
        }


        public void OnAfterDeserialize()
        {
            m_dirty = true;
            Rebuild();
        }


        public void OnBeforeSerialize() {}


        public void SetDirty()
        {
            m_dirty = true;
        }
    }


    [System.Serializable]
    public class ScriptingAction<T1, T2, T3> : ScriptingAction<T1, T2, T3, NullStruct> 
    {
        public void Invoke( T1 arg1, T2 arg2, T3 arg3 )
        {
            Invoke( arg1, arg2, arg3, NullStruct.Default ); 
        }
    }


    [System.Serializable]
    public class ScriptingAction<T1, T2> : ScriptingAction<T1, T2, NullStruct> 
    {
        public void Invoke( T1 arg1, T2 arg2 )
        {
            Invoke( arg1, arg2, NullStruct.Default );
        }
    }


    [System.Serializable]
    public class ScriptingAction<T1> : ScriptingAction<T1, NullStruct> 
    {
        public void Invoke( T1 arg1 )
        {
            Invoke( arg1, NullStruct.Default );
        }
    }

    [System.Serializable]
    public class ScriptingAction : ScriptingAction<NullStruct> 
    {
        public void Invoke()
        {
            Invoke( NullStruct.Default );
        }
    }
}
