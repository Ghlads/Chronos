using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace Framework.Core
{
    public static class ScriptingActionUtils
    {
        public static List<Expression> BuildArguments( IReadOnlyList<ParameterReference> parameters, IReadOnlyList<Expression> variables, ParameterExpression[] inputs )
        {
            List<Expression> arguments = new List<Expression>( parameters.Count );
            for ( int index = 0; index < parameters.Count; index++ )
            {
                arguments.Add( null );
                ParameterReference parameter = parameters[index];
                switch ( parameter.Source )
                {
                    case ParameterReference.SourceType.Return:
                        arguments[index] = variables[parameter.ReturnValueIndex];
                        break;
                    case ParameterReference.SourceType.Input:
                        arguments[index] = inputs[parameter.ReturnValueIndex];
                        break;
                    default:
                        Expression argument;
                        object rawValue = parameter.Value.Get<object>();
                        Type type = parameter.Value.GetWrappedSystemType();
                        if ( rawValue == null )
                        {
                            if ( parameter.ExpectedType.IsValueType && Nullable.GetUnderlyingType( parameter.ExpectedType ) == null )
                            {
                                argument = Expression.Default( parameter.ExpectedType );
                            }
                            else
                            {
                                argument = Expression.Constant( null, parameter.ExpectedType );
                            }
                        }
                        else if ( !parameter.ExpectedType.IsAssignableFrom( type ) )
                        {
                            argument = Expression.Convert( Expression.Constant( rawValue, type ), parameter.ExpectedType );
                        }
                        else
                        {
                            argument = Expression.Constant( rawValue, parameter.ExpectedType );
                        }
                        arguments[index] = argument;
                        break;
                }
            }

            return arguments;
        }


        public static void BuildModifier( ModifierReference modifier, List<Expression> variables, List<Expression> body, ParameterExpression[] inputs )
        {
            MethodCallExpression call = modifier.CreateExpression( variables, inputs );
            if ( call.Type == typeof( void ) )
            {
                body.Add( call );
                return;
            }

            Expression result;
            if ( modifier.IsLight )
            {
                result = call;
            }
            else
            {
                result = Expression.Variable( call.Type, $"{modifier.MethodInfo.Name}_result" );
                BinaryExpression cacheAssignation = Expression.Assign( result, call );
                body.Add( cacheAssignation );
            }
            variables.Add( result );
        }
    }
}
