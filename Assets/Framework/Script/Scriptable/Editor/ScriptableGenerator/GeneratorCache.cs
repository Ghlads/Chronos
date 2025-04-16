using System;
using System.Reflection;
using UnityEngine;

namespace Framework.Scriptable.Editor
{
    [CreateAssetMenu(fileName = "GeneratorCache", menuName = "Editor/Tool/Scriptable/GeneratorCache")]
    public class GeneratorCache : ScriptableObject
    {
        [Header( "Default" )]
        [SerializeField] private string m_defaultNamespace;
        [SerializeField] private string m_defaultOutputPath;
        [SerializeField] private string m_defaultCategory;

        [SerializeField][HideInInspector] private string m_lastSelectedTypeFullName = string.Empty;
        [SerializeField][HideInInspector] private string m_lastNamespace = string.Empty;
        [SerializeField][HideInInspector] private string m_lastOutputPath = string.Empty;
        [SerializeField][HideInInspector] private string m_lastCategory = string.Empty;

        public string DefaultNamespace => m_defaultNamespace;
        public string DefaultOutputPath => m_defaultOutputPath;
        public string DefaultCategory => m_defaultCategory;


        public string LastNamespace
        {
            get
            {
                return string.IsNullOrEmpty( m_lastNamespace ) ? m_defaultNamespace : m_lastNamespace;
            }
            set => m_lastNamespace = value;
        }


        public string LastOutputPath
        {
            get
            {
                return string.IsNullOrEmpty( m_lastOutputPath ) ? m_defaultOutputPath : m_lastOutputPath;
            }
            set => m_lastOutputPath = value;
        }


        public string LastCategory
        {
            get
            {
                return string.IsNullOrEmpty( m_lastCategory ) ? m_defaultCategory : m_lastCategory;
            }
            set => m_lastCategory = value;
        }

        
        public Type LastSelectedType
        {
            get
            {
                if ( string.IsNullOrEmpty( m_lastSelectedTypeFullName ) )
                {
                    return null;
                }

                foreach ( Assembly asm in AppDomain.CurrentDomain.GetAssemblies() )
                {
                    Type t = asm.GetType( m_lastSelectedTypeFullName );
                    if ( t != null )
                    {
                        return t;
                    }
                }

                return null;
            }
            set => m_lastSelectedTypeFullName = value != null ? value.FullName : string.Empty;
        }
    }
}
