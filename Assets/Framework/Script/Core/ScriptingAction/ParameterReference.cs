using System;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public class ParameterReference
    {
        public enum SourceType
        {
            Value = 0,
            Return = 1,
            Input = 2,
        }

        [SerializeField] private AnyValue m_value;
        [SerializeField] private int m_returnValueIndex;
        [SerializeField] private SourceType m_source;
        [SerializeField] private string m_expectedTypeFullName;
        [NonSerialized] private Type m_expectedType;

        public AnyValue Value => m_value;
        public SourceType Source
        {
            get => m_source;
            set => m_source = value; 
        }


        public int ReturnValueIndex
        {
            get => m_returnValueIndex;
            set => m_returnValueIndex = value;
        } 

        public Type ExpectedType
        {
            get
            {
                if ( m_expectedType == null && !string.IsNullOrEmpty( m_expectedTypeFullName ) )
                {
                    m_expectedType = ReflexionUtils.FindTypesByFullName( m_expectedTypeFullName ).FirstOrDefaultNoException();
                }

                return m_expectedType;
            }
            set
            {
                m_expectedType = value;
                m_expectedTypeFullName = value.FullName;
            }
        }
    }
}
