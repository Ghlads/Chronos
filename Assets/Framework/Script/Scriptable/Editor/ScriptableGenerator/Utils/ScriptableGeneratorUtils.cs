using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Scriptable.Editor
{
    [Flags]
    public enum ScriptableClass
    {
        None,
        Variable = 1,
        Event = 1 << 1,
        VariableInjector = 1 << 2,
        EventInjector = 1 << 3,
        VariableReference = 1 << 4,
        EventReference = 1 << 5,
    }


    public static class ScriptableGeneratorUtils
    {
        public static int ScriptableClassCount = 6;
        private static IEnumerator s_generationOperation = null;

        public static void Generate( ScriptableClass classes, string @namespace, string outputPath, string category, Type targetType, IProgress<float> reporter = null, Action<bool> generationEndCallback = null )
        {
            if ( s_generationOperation != null )
            {
                Debug.LogError( "[Generation fail] : operation already running" );
                generationEndCallback?.Invoke( false );
                return;
            }

            s_generationOperation = GenerateAsync( classes, @namespace, outputPath, category, targetType, reporter, generationEndCallback );
            EditorApplication.update += GeneratorUpdate;
        }


        private static void GeneratorUpdate()
        {
            if ( s_generationOperation.MoveNext() )
            {
                return;
            }

            s_generationOperation = null;
            EditorApplication.update -= GeneratorUpdate;
        }


        public static IEnumerator GenerateAsync( ScriptableClass classes, string @namespace, string outputPath, string category, Type targetType, IProgress<float> reporter = null, Action<bool> generationEndCallback = null )
        {
            if ( string.IsNullOrWhiteSpace( outputPath ) )
            {
                Debug.LogError( "[Generation fail] : empty output path" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            if ( classes == ScriptableClass.None )
            {
                Debug.LogError( "[Generation fail] : no classes requseted" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            if ( targetType == null )
            {
                Debug.LogError( "[Generation fail] : null type" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            reporter.Report( 15.0f );
            yield return null;

            try
            {
                // path can not exist yet we'll create it but we want to catch user input error
                if ( outputPath[outputPath.Length - 1] != '/' )
                {
                    outputPath += '/';
                }

                Directory.Exists( Application.dataPath + "/" + outputPath );
            }
            catch ( Exception ex )
            {
                Debug.LogError( $"[Generation fail] : invalid path | Exception {ex}" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            classes.EnsureClassesDependencies();
            classes.RemoveExistantClasses( targetType );
            if ( classes == ScriptableClass.None )
            {
                Debug.LogWarning( "[Generation cancel] : All classes requested already exist. Nothing to generate" );
                generationEndCallback?.Invoke( true );
                yield break;
            }

            reporter.Report( 30.0f );
            yield return null;

            float progress = 30f;
            float step = ( 100f - progress ) / ScriptableGeneratorUtils.ScriptableClassCount;
            TypeData data = ( TypeData )targetType;
            for ( int index = 0; index < ScriptableGeneratorUtils.ScriptableClassCount; index++ )
            {
                ScriptableClass currentClassTested = ( ScriptableClass )( 1 << index );
                if ( classes.HasClassFlag( currentClassTested ) )
                {
                    GenerateClass(
                        currentClassTested,
                        outputPath,
                        GetClassName( currentClassTested, data ),
                        GenerateClassContent( currentClassTested, data, @namespace, category ) );
                }

                progress += step;
                reporter.Report( progress );
            }

            reporter.Report( 100f );

            generationEndCallback?.Invoke( true );
            yield break;
        }


        public static void GenerateClass( ScriptableClass toGenerate, string outputPath, string className, string content )
        {
            try
            {
                Directory.CreateDirectory( $"{Application.dataPath}/{outputPath}" );
            }
            catch ( Exception e )
            { 
                Debug.LogError( $"[Generatrion fail] : Couldn't create directory at path {Application.dataPath}/{outputPath} | Exception {e}" );
            }

            try
            {
                File.WriteAllText( $"{Application.dataPath}/{outputPath}{className}.generated.cs", content );
            }
            catch ( Exception e )
            {
                Debug.LogError( $"[Generatrion fail] : Couldn't generate {toGenerate} | Exception {e}" );
            }
        }


        public static string GenerateClassContent( ScriptableClass target, TypeData data, string @namespace, string category )
        {
            switch ( target )
            {
                case ScriptableClass.Variable:
                    return Templates.Variable.GenerateClassContent(
                        data,
                        @namespace,
                        category );

                case ScriptableClass.Event:
                    return Templates.Event.GenerateClassContent(
                        data,
                        @namespace,
                        category );

                case ScriptableClass.VariableInjector:
                    return Templates.VariableInjector.GenerateClassContent(
                        data,
                        new TypeData( GetClassName( ScriptableClass.Variable, data ), @namespace ),
                        @namespace );

                case ScriptableClass.EventInjector:
                    return Templates.EventInjector.GenerateClassContent(
                        data,
                        new TypeData( GetClassName( ScriptableClass.Event, data ), @namespace ),
                        @namespace );

                case ScriptableClass.VariableReference:
                    return Templates.VariableReference.GenerateClassContent(
                        data,
                        new TypeData( GetClassName( ScriptableClass.Variable, data ), @namespace ),
                        new TypeData( GetClassName( ScriptableClass.VariableInjector, data ), @namespace ),
                        @namespace );

                case ScriptableClass.EventReference:
                    return Templates.EventReference.GenerateClassContent(
                        data,
                        new TypeData( GetClassName( ScriptableClass.Event, data ), @namespace ),
                        new TypeData( GetClassName( ScriptableClass.EventInjector, data ), @namespace ),
                        @namespace );

                case ScriptableClass.None:
                default:
                    Debug.LogError( $"Couldn't get content for class {target}" );
                    return string.Empty;
            }
        }


        public static string GetClassName( ScriptableClass target, TypeData data )
        {
            switch ( target )
            {
                case ScriptableClass.Variable:
                    return Templates.Variable.GetClassName( data );
                case ScriptableClass.Event:
                    return Templates.Event.GetClassName( data );
                case ScriptableClass.VariableInjector:
                    return Templates.VariableInjector.GetClassName( data );
                case ScriptableClass.EventInjector:
                    return Templates.EventInjector.GetClassName( data );
                case ScriptableClass.VariableReference:
                    return Templates.VariableReference.GetClassName( data );
                case ScriptableClass.EventReference:
                    return Templates.EventReference.GetClassName( data );
                case ScriptableClass.None:
                default:
                    Debug.LogError( $"Couldn't get name for class {target}" );
                    return string.Empty;
            }
        }


        public static void EnsureClassesDependencies( this ref ScriptableClass classes )
        {
            if ( classes.HasClassFlag( ScriptableClass.EventReference ) )
            {
                classes |= ScriptableClass.EventInjector;
            }

            if ( classes.HasClassFlag( ScriptableClass.VariableReference ) )
            {
                classes |= ScriptableClass.VariableInjector;
            }

            if ( classes.HasClassFlag( ScriptableClass.EventInjector ) )
            {
                classes |= ScriptableClass.Event;
            }

            if ( classes.HasClassFlag( ScriptableClass.VariableInjector ) )
            {
                classes |= ScriptableClass.Variable;
            }
        }


        public static bool HasClassFlag( this ScriptableClass classes, ScriptableClass flag )
        {
            return ( classes & flag ) == flag;
        }


        public static void RemoveExistantClasses( this ref ScriptableClass classes, Type type )
        {
            RemoveExistantVariableClasses( ref classes, type );
            RemoveExistantEventClasses( ref classes, type );
        }


        public static bool TryGetTypeAndRemoveFromFlagIfExistant( ref ScriptableClass classes, ScriptableClass toCheck, string typeName, Type genericSubType, out Type concreteType )
        {
            concreteType = null;
            if ( !classes.HasClassFlag( toCheck ) )
            {
                return false;
            }

            List<Type> types = ReflexionUtils.FindTypesByName( typeName );
            if ( types == null || types.Count <= 0 )
            {
                return false;
            }

            foreach ( Type t in types )
            {
                if ( t.InheritsFrom( genericSubType ) )
                {
                    classes &= ~toCheck;
                    return true;
                }
            }

            return false;
        }


        public static void RemoveExistantVariableClasses( ref ScriptableClass classes, Type type )
        {
            TypeData typeData = ( TypeData )type;
            if ( !TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes,
                toCheck: ScriptableClass.Variable,
                typeName: Templates.Variable.GetClassName( typeData ),
                genericSubType: typeof( ScriptableVariable<> ).MakeGenericType( type ),
                concreteType: out Type variableConcreteType ) )
            {
                return;
            }

            if ( !TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes,
                toCheck: ScriptableClass.VariableInjector,
                typeName: Templates.VariableInjector.GetClassName( typeData ),
                genericSubType: typeof( RuntimeVariableInjector<,> ).MakeGenericType( type, variableConcreteType ),
                concreteType: out Type injectorConcreteType ) )
            {
                return;
            }

            TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes, 
                toCheck: ScriptableClass.VariableReference,
                typeName: Templates.VariableReference.GetClassName( typeData ),
                genericSubType: typeof( ScriptableVariableReference<,,> ).MakeGenericType( type, variableConcreteType, injectorConcreteType ),
                concreteType: out Type _ );
        }


        public static void RemoveExistantEventClasses( ref ScriptableClass classes, Type type )
        {
            TypeData typeData = ( TypeData )type;
            if ( !TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes,
                toCheck: ScriptableClass.Event,
                typeName: Templates.Event.GetClassName( typeData ),
                genericSubType: typeof( ScriptableEvent<> ).MakeGenericType( type ),
                concreteType: out Type eventConcreteType ) )
            {
                return;
            }

            if ( !TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes,
                toCheck: ScriptableClass.EventInjector,
                typeName: Templates.EventInjector.GetClassName( typeData ),
                genericSubType: typeof( RuntimeEventInjector<,> ).MakeGenericType( type, eventConcreteType ),
                concreteType: out Type injectorConcreteType ) )
            {
                return;
            }

            TryGetTypeAndRemoveFromFlagIfExistant(
                classes: ref classes,
                toCheck: ScriptableClass.EventReference,
                typeName: Templates.EventReference.GetClassName( typeData ),
                genericSubType: typeof( ScriptableEventReference<,,> ).MakeGenericType( type, eventConcreteType, injectorConcreteType ),
                concreteType: out Type _ );
        }


        public static bool IsTypeEligible( Type type, string filter )
        {
            if ( !type.IsPublic )
            {
                return false;
            }

            if ( type.IsAbstract )
            {
                return false;
            }

            if ( type.IsInterface )
            {
                return false;
            }

            if ( type.IsGenericType )
            {
                return false;
            }

            if ( !( type.IsPrimitive ||
                type.Attributes.HasFlag( System.Reflection.TypeAttributes.Serializable ) ||
                type.InheritsFrom<UnityEngine.Object>() ) )
            {
                return false;
            }

            if ( type.ImplementsInterface<IGenericScriptable>() )
            {
                return false;
            }

            return type.Name.ToLowerInvariant().Contains( filter );
        }
    }
}
