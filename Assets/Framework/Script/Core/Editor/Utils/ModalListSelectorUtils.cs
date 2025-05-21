using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Codice.Client.Common.WebApi.WebApiEndpoints;

namespace Framework.Core.Editor
{
    public static class ModalListSelectorUtils
    {
        private static Dictionary<MethodInfo, string> s_methodToDisplayMap = new();
        public static string MethodInfoToString( MethodInfo method )
        {
            if ( s_methodToDisplayMap.ContainsKey( method ) )
            {
                return s_methodToDisplayMap[method];
            }

            StringBuilder builder = new StringBuilder();
            builder.Append( method.ReturnType.Beautified().GetPrettyName() )
                .Append( ' ' )
                .Append( method.DeclaringType.GetPrettyName() )
                .Append( '.' )
                .Append( method.Name );
            ParameterInfo[] parametersInfo = method.GetParameters();
            using ( new ParenthesesWrapper( builder, parametersInfo.Length > 0 && parametersInfo[0].ParameterType != typeof( NullStruct ) ) )
            {
                for ( int index = 0; index < parametersInfo.Length - 1; index++ )
                {
                    Type type = parametersInfo[index].ParameterType.Beautified();
                    if ( type != typeof( NullStruct ) )
                    {
                        builder.Append( type.GetPrettyName() ).Append( ", " );
                    }
                }

                if ( parametersInfo.Length > 0 )
                {
                    Type lastType = parametersInfo[parametersInfo.Length - 1].ParameterType.Beautified();
                    if ( lastType != typeof( NullStruct ) )
                    {
                        builder.Append( lastType.GetPrettyName() );
                    }
                }
            }

            s_methodToDisplayMap.Add( method, builder.ToString() );
            return s_methodToDisplayMap[method];
        }


        private static List<MethodInfo> s_staticList;
        public static List<MethodInfo> GetStaticMethodInfos()
        {
            if ( s_staticList != null )
            {
                return s_staticList;
            }

            s_staticList = new List<MethodInfo>( 100 );// arbitrary big number to prevent a lot of realloc 
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach ( Assembly assembly in assemblies )
            {
                if ( IsEditor( assembly ) )
                {
                    continue;
                }

                foreach ( Type type in assembly.GetTypes() )
                {
                    if ( !type.IsPublic || type.IsGenericType )
                    {
                        continue;
                    }

                    foreach ( MethodInfo method in type.GetMethods( BindingFlags.Static | BindingFlags.Public ) )
                    {
                        if ( method.IsGenericMethod || method.IsGenericMethodDefinition || method.ContainsGenericParameters || method.IsConstructedGenericMethod )
                        {
                            continue;
                        }

                        s_staticList.AddUnique( method );
                    }
                }
            }

            return s_staticList;

            bool IsEditor( Assembly assembly )
            {
                if ( assembly.GetName().Name.ToLower().Contains( "editor" ) )
                {
                    return true;
                }

                return false;
            }
        }


        public static void OpenActionSelectorWindow( Action<MethodInfo> callback )
        {
            new ModalListBuilder<MethodInfo>(
                GetStaticMethodInfos(),
                callback,
                () => new Label(),
                ( element, _, info ) =>
                {
                    ( element as Label ).text = MethodInfoToString( info );
                }
                ).WithSelectOnDoubleClick().WithTitle( "Action Selector Window" )
                .WithSearchBar( ( filter, info ) =>
                {
                    return MethodInfoToString( info ).ToLower().Contains( filter.ToLower() );
                } ).Open();
        }


        public static void OpenSourceIndexSelectorWindow( ParameterSchema.Sources source, List<int> validIndex, Action<int> callback )
        {
            new ModalListBuilder<int>(
                validIndex,
                callback,
                () => new Label(),
                ( element, _, index ) =>
                {
                    ( element as Label ).text = $"{source} : [{index}]";
                }
                ).WithSelectOnDoubleClick().WithTitle( "Action Selector Window" )
                .Open();
        }
    }
}
