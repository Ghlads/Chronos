using System;
using System.Collections.Generic;
using System.Text;

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


public  static class StringUtils
{
    public static string Concat( IReadOnlyList<string> strings, string separator )
    {
        StringBuilder builder = new StringBuilder();
        for ( int i = 0; i < strings.Count-1; i++ )
        {
            builder.Append( strings[i] );
            builder.Append( separator );
        }

        return builder.Append( strings[strings.Count-1] ).ToString();
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
