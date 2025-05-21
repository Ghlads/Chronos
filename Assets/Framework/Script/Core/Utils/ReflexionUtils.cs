using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace Framework.Core
{
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
            return FindTypesByCondition( t => t.Name == name );
        }


        public static List<Type> FindTypesByFullName( string fullName )
        {
            return FindTypesByCondition( t => t.FullName == fullName );
        }


        public delegate bool FindTypeSignature( Type type );
        public static List<Type> FindTypesByCondition( FindTypeSignature condition )
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany( x => x.GetTypes() )
                .Where( t => condition( t ) )
                .ToList();
        }


        public static Type GetTypeByAssemblyName( string assemblyName )
        {
            if ( string.IsNullOrEmpty( assemblyName ) )
            {
                return null;
            }

            Type result = Type.GetType( assemblyName, throwOnError: false );
            if ( result != null )
            {
                return result;
            }

            Assembly[] asseemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach ( Assembly assembly in asseemblies )
            {
                result = assembly.GetType( assemblyName );
                if( result != null )
                {
                    return result;
                }
            }

            return result;
        }


        public static Type OptimizedGetType( string assemblyFullName, string fullName )
        {
            if ( string.IsNullOrEmpty( fullName ) || string.IsNullOrEmpty( assemblyFullName ) )
            {
                return null;
            }


            foreach ( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                if ( assembly.FullName != assemblyFullName )
                {
                    continue;
                }

                return assembly.GetType( fullName );
            }

            return null;
        }


        public static bool ImplementsInterface<T>( this Type type )
        {
            return ImplementsInterface( type, typeof( T ) );
        }


        public static bool ImplementsInterface( this Type targetType, Type interfaceType )
        {
            if ( !interfaceType.IsInterface )
            {
                return false;
            }

            return interfaceType.IsAssignableFrom( targetType );
        }
    
    
        public static bool HasAnItemInRecovery( this SerializedType[] types )
        {
            if ( types == null || types.Length <= 0 )
            {
                return false;
            }

            foreach ( SerializedType type in types )
            {
                if ( type.IsInRecovery )
                {
                    return true;
                }
            }

            return false;
        }


        public static Type[] ToTypeArray( this SerializedType[] types )
        {
            if ( types == null || types.Length <= 0 )
            {
                return new Type[0];
            }

            Type[] result = new Type[types.Length];
            for ( int index = 0; index < types.Length; index++ )
            {
                result[index] = types[index];
            }

            return result;
        }

        
        public static string ToFullName( this SerializedType[] types, char separator = ',' )
        {
            if( types == null || types.Length <= 0 )
            {
                return string.Empty;
            }

            StringBuilder buidler = new StringBuilder();
            for ( int index = 0; index < types.Length; index++ )
            {
                Type type = types[index];
                buidler.Append( type.FullName ).Append( separator ).Append( ' ' );
            }

            if ( types.Length > 0 )
            {
                buidler.Append( types[types.Length].Type.FullName );
            }

            return buidler.ToString();
        }


        public static FieldInfo GetFieldInHierarchy( this Type type, string fieldName, BindingFlags bindingFlags )
        {
            while ( type != null )
            {
                FieldInfo field = type.GetField( fieldName, bindingFlags );
                if ( field != null )
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }
    }


    public static class PrimitveBeautifier
    {
        private class BeautyType : Type
        {
            Type m_wrapped = null;
            string m_beautyName = null;
            string m_capitalizeBeautyName = null;

            public BeautyType( Type wrapped, string name )
            {
                m_wrapped = wrapped;
                m_beautyName = name;
                m_capitalizeBeautyName = name;
                StringUtils.Capitalize( ref m_capitalizeBeautyName );
            }

            public override Assembly Assembly => m_wrapped.Assembly;

            public override string AssemblyQualifiedName => m_wrapped.AssemblyQualifiedName;

            public override Type BaseType => m_wrapped.BaseType;

            public override string FullName => m_beautyName;

            public override Guid GUID => m_wrapped.GUID;

            public override Module Module => m_wrapped.Module;

            public override string Namespace => m_wrapped.Namespace;

            public override Type UnderlyingSystemType => m_wrapped.UnderlyingSystemType;

            public override string Name => m_capitalizeBeautyName;

            public override ConstructorInfo[] GetConstructors( BindingFlags bindingAttr )
            {
                return m_wrapped.GetConstructors( bindingAttr );
            }

            public override object[] GetCustomAttributes( bool inherit )
            {
                return m_wrapped.GetCustomAttributes( inherit );
            }

            public override object[] GetCustomAttributes( Type attributeType, bool inherit )
            {
                return m_wrapped.GetCustomAttributes( attributeType, inherit );
            }

            public override Type GetElementType()
            {
                return m_wrapped.GetElementType();
            }

            public override EventInfo GetEvent( string name, BindingFlags bindingAttr )
            {
                return m_wrapped.GetEvent( name, bindingAttr );
            }

            public override EventInfo[] GetEvents( BindingFlags bindingAttr )
            {
                return m_wrapped.GetEvents( bindingAttr );
            }

            public override FieldInfo GetField( string name, BindingFlags bindingAttr )
            {
                return m_wrapped.GetField( name, bindingAttr );
            }

            public override FieldInfo[] GetFields( BindingFlags bindingAttr )
            {
                return m_wrapped.GetFields( bindingAttr );
            }

            public override Type GetInterface( string name, bool ignoreCase )
            {
                return m_wrapped.GetInterface( name, ignoreCase );
            }

            public override Type[] GetInterfaces()
            {
                return m_wrapped.GetInterfaces();
            }

            public override MemberInfo[] GetMembers( BindingFlags bindingAttr )
            {
                return m_wrapped.GetMembers( bindingAttr );
            }

            public override MethodInfo[] GetMethods( BindingFlags bindingAttr )
            {
                return null;
            }

            public override Type GetNestedType( string name, BindingFlags bindingAttr )
            {
                return m_wrapped.GetNestedType( name, bindingAttr );
            }

            public override Type[] GetNestedTypes( BindingFlags bindingAttr )
            {
                return m_wrapped.GetNestedTypes( bindingAttr );
            }

            public override PropertyInfo[] GetProperties( BindingFlags bindingAttr )
            {
                return m_wrapped.GetProperties( bindingAttr );
            }

            public override object InvokeMember( string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters )
            {
                return m_wrapped.InvokeMember( name, invokeAttr, binder, target, args, modifiers, culture, namedParameters );
            }

            public override bool IsDefined( Type attributeType, bool inherit )
            {
                return m_wrapped.IsDefined( attributeType, inherit );
            }

            protected override TypeAttributes GetAttributeFlagsImpl()
            {
                return TypeAttributes.Public;
            }

            protected override ConstructorInfo GetConstructorImpl( BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers )
            {
                return null;
            }

            protected override MethodInfo GetMethodImpl( string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers )
            {
                return null;
            }

            protected override PropertyInfo GetPropertyImpl( string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers )
            {
                return null;
            }

            protected override bool HasElementTypeImpl()
            {
                return false;
            }

            protected override bool IsArrayImpl()
            {
                return false;
            }

            protected override bool IsByRefImpl()
            {
                return false;
            }

            protected override bool IsCOMObjectImpl()
            {
                return false;
            }

            protected override bool IsPointerImpl()
            {
                return false;
            }

            protected override bool IsPrimitiveImpl()
            {
                return true;
            }
        }

        private static Dictionary<Type, Type> s_beautifiedType = new Dictionary<Type, Type>() 
        {
            { typeof( float   ), new BeautyType( typeof( float   ), "float"   ) },
            { typeof( int     ), new BeautyType( typeof( int     ), "int"     ) },
            { typeof( double  ), new BeautyType( typeof( double  ), "double"  ) },
            { typeof( string  ), new BeautyType( typeof( string  ), "string"  ) },
            { typeof( bool    ), new BeautyType( typeof( bool    ), "bool"    ) },
            { typeof( byte    ), new BeautyType( typeof( byte    ), "byte"    ) },
            { typeof( sbyte   ), new BeautyType( typeof( sbyte   ), "sbyte"   ) },
            { typeof( char    ), new BeautyType( typeof( char    ), "char"    ) },
            { typeof( decimal ), new BeautyType( typeof( decimal ), "decimal" ) },
            { typeof( uint    ), new BeautyType( typeof( uint    ), "uint"    ) },
            { typeof( long    ), new BeautyType( typeof( long    ), "long"    ) },
            { typeof( ulong   ), new BeautyType( typeof( ulong   ), "ulong"   ) },
            { typeof( short   ), new BeautyType( typeof( short   ), "short"   ) },
            { typeof( ushort  ), new BeautyType( typeof( ushort  ), "ushort"  ) },
            { typeof( object  ), new BeautyType( typeof( object  ), "object"  ) },
        };


        public static Type Beautified( this Type type )
        {
            if ( type == null )
            {
                return null;
            }

            if ( s_beautifiedType.TryGetValue( type, out Type beautified ) )
            {
                return beautified;
            }

            return type;
        }
    }
}
