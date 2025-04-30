using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Framework.Core
{
    public class CurlyBracketWrapper : IDisposable
    {
        private const char CURLY_BRACKET_OPEN = '{';
        private const char CURLY_BRACKET_CLOSE = '}';

        private StringBuilder m_wrappedBuilder;
        public CurlyBracketWrapper( StringBuilder wrappedBuilder )
        {
            m_wrappedBuilder = wrappedBuilder;
            wrappedBuilder.AppendLine()
                .Append( CURLY_BRACKET_OPEN )
                .AppendLine();
        }

        public void Dispose()
        {
            m_wrappedBuilder.AppendLine()
                .Append( CURLY_BRACKET_CLOSE )
                .AppendLine();
        }
    }


    public class ParenthesesWrapper : IDisposable
    {
        public const char PARENTHESES_OPEN = '(';
        public const char PARENTHESES_CLOSE = ')';

        private readonly StringBuilder m_wrappedBuilder;
        private readonly bool m_shouldAddSpaceBetween;


        public static ParenthesesWrapper WithoutSpaceBetween( StringBuilder wrappedBuilder )
        {
            return new ParenthesesWrapper( wrappedBuilder, shouldAddSpaceBetween: false );
        }


        public static ParenthesesWrapper WithSpaceBetween( StringBuilder wrappedBuilder )
        {
            return new ParenthesesWrapper( wrappedBuilder, shouldAddSpaceBetween: true );
        }


        public ParenthesesWrapper( StringBuilder wrappedBuilder, bool shouldAddSpaceBetween )
        {
            m_wrappedBuilder = wrappedBuilder;
            m_wrappedBuilder.Append( PARENTHESES_OPEN );
            if ( m_shouldAddSpaceBetween = shouldAddSpaceBetween )
            {
                m_wrappedBuilder.Append( ' ' );
            }
        }

        public void Dispose()
        {
            if ( m_shouldAddSpaceBetween )
            {
                m_wrappedBuilder.Append( ' ' );
            }

            m_wrappedBuilder.Append( PARENTHESES_CLOSE );
        }
    }


    public static class StringUtils
    {
        public static string Concat( IReadOnlyList<string> strings, string separator )
        {
            if ( strings == null || strings.Count <= 0 )
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            for ( int i = 0; i < strings.Count - 1; i++ )
            {
                builder.Append( strings[i] );
                builder.Append( separator );
            }

            return builder.Append( strings[strings.Count - 1] ).ToString();
        }


        public static void Capitalize( ref string value )
        {
            if ( string.IsNullOrEmpty( value ) )
            {
                return;
            }

            if ( value.Length == 1 )
            {
                value = value.ToUpperInvariant();
                return;
            }

            value = char.ToUpperInvariant( value[0] ) + value.Substring( 1 );
        }
    }
}
