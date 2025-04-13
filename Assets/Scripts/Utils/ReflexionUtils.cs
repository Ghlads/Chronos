using System;
using System.Linq;

public static class ReflexionUtils
{
    public static string GetPrettyName( this Type type )
    {
        if ( type == null )
        {
            return "null";
        }

        if ( !type.IsGenericType )
        {
            return type.Name;
        }

        string name = type.Name.Split( '`' )[0];
        string genericArguments = string.Join( ", ", type.GetGenericArguments().Select( t => t.Name ) );
        return $"{name}<{genericArguments}>";
    }


    public static Type ResolveGenericType( this Type type )
    {
        if ( !type.IsGenericType )
        {
            return type;
        }

        return type.GetGenericTypeDefinition();
    }


    public static bool InheritsFrom<T>( this Type inType )
    {
        return inType.InheritsFrom( typeof( T ) );
    }


    public static bool InheritsFrom( this Type inType, Type inBaseType )
    {
        Type type = inType;
        Type c = inBaseType;
        while ( type != null )
        {
            if ( type == c || type.ResolveGenericType() == c )
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }


    public static Type GetGenericInstanceOf<T>( this Type inType )
    {
        return inType.GetGenericInstanceOf( typeof( T ) );
    }


    public static Type GetGenericInstanceOf( this Type inType, Type generic )
    {
        Type type = inType;
        Type c = generic;
        while ( type != null )
        {
            if ( type == c || type.ResolveGenericType() == c )
            {
                return type;
            }

            type = type.BaseType;
        }

        return type;
    }
}
