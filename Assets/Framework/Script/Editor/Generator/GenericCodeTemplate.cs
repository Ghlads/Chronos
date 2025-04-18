using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Editor
{
    [CreateAssetMenu(fileName = "CodeTemplate", menuName = "Editor/Tool/Generator/Template")]
    public class GenericCodeTemplate : CodeTemplateSource
    {
        [Tooltip( @"
When setting your format : 
typeName can't use custom args only content
0 is for the Name of your type generic param
1 is for the FullName of your type generic param
2 to n is for your custom additional params
n to x is for the name of your dependencies ( 2 is the first dependencies )
" )]
        [SerializeField] private List<GenericCodeTemplate> m_dependencies;
        [SerializeField] private int m_customParamsCount = 0;
        [Space]
        [Tooltip( @"
When setting your format : 
typeName can't use custom args only content
0 is for the Name of your type generic param
1 is for the FullName of your type generic param
2 to n is for your custom additional params
n to x is for the name of your dependencies ( 2 is the first dependencies )
" )]
        [SerializeField] private string m_typeNameFormat = string.Empty;
        [Tooltip( @"
When setting your format : 
typeName can't use custom args only content
0 is for the Name of your type generic param
1 is for the FullName of your type generic param
2 to n is for your custom additional params
n to x is for the name of your dependencies ( 2 is the first dependencies )
" )]
        [SerializeField][TextArea] private string m_typeContentFormat = string.Empty;

        [Tooltip( "Becareful, this is a cache if you edit this, it might corupt generated output if it happens, simply set length to 0 and it will be regenerate correlty" )]
        [SerializeField] private string[] m_formatParams = null;
        private Type m_lastProcessedType = null;

        public List<GenericCodeTemplate> Dependencies => m_dependencies;


        private string[] GetFormatParams( Type type )
        {
            if ( type != m_lastProcessedType || m_formatParams == null || m_formatParams.Length != m_dependencies.Count + 2 + m_customParamsCount )
            {
                m_formatParams = new string[m_dependencies.Count + 2 + m_customParamsCount];
                m_formatParams[0] = type.Name;
                m_formatParams[1] = type.FullName;
                for ( int index = 0; index < m_dependencies.Count; index++ )
                {
                    m_formatParams[index + 2 + m_customParamsCount] = m_dependencies[index].GetTypeName( type );
                }
            }

            return m_formatParams;
        }


        private string[] GetFormatParamsWithCustomArgs( Type type, params object[] customArgs )
        {
            string[] formatParams = GetFormatParams( type );

            if ( ( customArgs == null && m_customParamsCount > 0 ) || customArgs.Length != m_customParamsCount )
            {
                Debug.LogWarning( $"Custom args count [{( customArgs == null ? 0 : customArgs.Length )}] doesn't match expected count [{m_customParamsCount}] for template {name}" );
            }

            for ( int index = 0; index < customArgs.Length && index < m_customParamsCount; index++ )
            {
                formatParams[index + 2] = customArgs[index].ToString();
            }

            return formatParams;
        }



        public string GetTypeName( Type type )
        {
            return string.Format( m_typeNameFormat, GetFormatParams( type ) );
        }


        public string GetTypeContent( Type type, params object[] customArgs )
        {

            return string.Format( m_typeContentFormat, GetFormatParamsWithCustomArgs( type, customArgs ) );
        }
    }
}
