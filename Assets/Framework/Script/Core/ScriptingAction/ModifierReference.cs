using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public struct ModifierReference
    {
        [SerializeField] private SerializedMethod m_method;
        [SerializeField] private List<ParameterReference> m_parameterReferences;
        [SerializeField] private bool m_isLight;


        public bool IsLight
        {
            get { return m_isLight; }
            set { m_isLight = value; }
        }


        public MethodInfo MethodInfo
        {
            get
            {
                return m_method;
            }
            set
            {
                m_method.Method = value;
            }
        }


        public MethodCallExpression CreateExpression( List<Expression> variables, ParameterExpression[] inputs )
        {
            List<Expression> arguments = ScriptingActionUtils.BuildArguments( m_parameterReferences, variables, inputs );
            return Expression.Call( MethodInfo, arguments );
        }
    }
}
