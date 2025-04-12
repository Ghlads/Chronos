using System.Collections.Generic;
using System.Text;

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


}
