using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Core
{
    public class CurlyBracketWrapper : IDisposable
    {
        private const char CURLY_BRACKET_OPEN = '{';
        private const char CURLY_BRACKET_CLOSE = '}';

        private StringBuilder m_wrappedBuilder;
        private IndentedStringBuilder m_indentedStringBuilder;

        public CurlyBracketWrapper( StringBuilder wrappedBuilder )
        {
            m_wrappedBuilder = wrappedBuilder;
            wrappedBuilder.AppendLine()
                .Append( CURLY_BRACKET_OPEN )
                .AppendLine();
        }


        public CurlyBracketWrapper( IndentedStringBuilder wrappedBuilder )
        {
            m_indentedStringBuilder = wrappedBuilder;
            wrappedBuilder.AppendLine()
                .Append( CURLY_BRACKET_OPEN );
            wrappedBuilder.Indent++;
            wrappedBuilder.AppendLine();
            //m_indentedStringBuilder.AppendIndentation();
        }



        public void Dispose()
        {
            if ( m_wrappedBuilder != null )
            {
                m_wrappedBuilder.AppendLine()
                    .Append( CURLY_BRACKET_CLOSE )
                    .AppendLine();

            }
            else if ( m_indentedStringBuilder != null )
            {
                m_indentedStringBuilder.Indent--;
                m_indentedStringBuilder.AppendLine()
                    .Append( CURLY_BRACKET_CLOSE )
                    .AppendLine();
            }
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


    public class IndentedStringBuilder
    {
        private StringBuilder m_wrappedBuilder;
        private int m_indent;

        public int Indent
        {
            get { return m_indent; }
            set 
            { 
                m_indent = value;
            }
        }


        public IndentedStringBuilder()
        {
            m_wrappedBuilder = new StringBuilder();
            m_indent = 0;
        }


        public IndentedStringBuilder Append( string value )
        {
            m_wrappedBuilder.Append( value );
            return this;
        }


        public IndentedStringBuilder Append( int value )
        {
            m_wrappedBuilder.Append( value );
            return this;
        }


        public IndentedStringBuilder Append( char value )
        {
            m_wrappedBuilder.Append( value );
            return this;
        }


        public void AppendIndentation()
        {
            for ( int i = 0; i < m_indent; i++ )
            {
                m_wrappedBuilder.Append( '\t' );
            }
        }


        public IndentedStringBuilder AppendLine()
        {
            m_wrappedBuilder.AppendLine();
            AppendIndentation();
            return this;
        }


        public override string ToString()
        {
            return m_wrappedBuilder.ToString();
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


        public static string WithoutExtension( this string path )
        {
            return path.WithoutExtension( out _ );
        }


        public static string WithoutExtension( this string path, out string extension )
        {
            int extensionDotIndex = path.Length;
            while ( extensionDotIndex-- >= 0 )
            {
                if ( path[extensionDotIndex] == '.' )
                {
                    break;
                }
            }

            if ( extensionDotIndex <= 0 )
            {
                extension = string.Empty;
                return path;
            }

            extension = path.Substring( extensionDotIndex + 1 );
            return path.Substring( 0, extensionDotIndex );
        }


        public static string[] SplitPathAndName( this string path )
        {
            string[] output = new string[2];

            int slashIndex = path.Length;
            while ( slashIndex-- > 0 )
            {
                if ( path[slashIndex] == '/' || path[slashIndex] == '\\' )
                {
                    break;
                }
            }

            if ( slashIndex <= 0 )
            {
                output[0] = string.Empty;
                output[1] = path;
            }
            else
            {
                output[0] = path.Substring( 0, slashIndex );
                output[1] = path.Substring( slashIndex + 1 );
            }

            return output;
        }


        public static string RemoveCharInstances( this string str, char toRemove )
        {
            StringBuilder buidler = new StringBuilder( str.Length );
            foreach ( char c in str )
            {
                if ( c != toRemove )
                {
                    buidler.Append( c );
                }
            }

            return buidler.ToString();
        }
    }
}
