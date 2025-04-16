using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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


    public static List<Type> GetTypesInheriting( this Type inType )
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany( x => x.GetTypes() )
            .Where( t => t.InheritsFrom( inType ) )
            .ToList();
    }


    public static string GetSafeName( this Type type )
    {
        return string.IsNullOrEmpty( type.Namespace ) ? type.Name : $"{type.Namespace}.{type.Name}";
    }


    public static List<Type> FindTypesByName( string name )
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany( x => x.GetTypes() )
            .Where( t => t.Name == name )
            .ToList();
    }


    public static bool ImplementsInterface<T>( this Type type )
    {
        return ImplementsInterface( type, typeof(T ) );
    }


    public static bool ImplementsInterface( this Type targetType, Type interfaceType )
    {
        if ( !interfaceType.IsInterface )
        {
            return false;
        }

        return interfaceType.IsAssignableFrom( targetType );
    }
}
