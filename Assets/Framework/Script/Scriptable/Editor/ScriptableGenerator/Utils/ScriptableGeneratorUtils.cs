using Framework.Core;
using Framework.Core.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.Scriptable.Editor
{
    public static class ScriptableGeneratorUtils
    {
        public static int ScriptableClassCount = 6;
        private static IEnumerator s_generationOperation = null;

        public static void Generate( List<CodeTemplateSource> templates, string @namespace, string outputPath, string category, Type targetType, IProgress<float> reporter = null, Action<bool> generationEndCallback = null )
        {
            if ( s_generationOperation != null )
            {
                Debug.LogError( "[Generation fail] : operation already running" );
                generationEndCallback?.Invoke( false );
                return;
            }

            s_generationOperation = GenerateAsync( templates, @namespace, outputPath, category, targetType, reporter, generationEndCallback );
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


        public static IEnumerator GenerateAsync( List<CodeTemplateSource> templates, string @namespace, string outputPath, string category, Type targetType, IProgress<float> reporter = null, Action<bool> generationEndCallback = null )
        {
            if ( string.IsNullOrWhiteSpace( outputPath ) )
            {
                Debug.LogError( "[Generation fail] : empty output path" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            if ( templates == null || templates.Count <= 0 )
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

            targetType = targetType.Beautified();

            reporter.Report( 15.0f );
            yield return null;

            try
            {
                // path can not exist yet we'll create it but we want to catch user input error
                if ( !outputPath.EndsWith( '/' ) )
                {
                    outputPath += '/';
                }

                Directory.Exists( Application.dataPath + "/" + outputPath );// Don't create yet since other thinks can fail
            }
            catch ( Exception ex )
            {
                Debug.LogError( $"[Generation fail] : invalid path | Exception {ex}" );
                generationEndCallback?.Invoke( false );
                yield break;
            }

            List<GenericCodeTemplate> resolvedTemplates = new List<GenericCodeTemplate>( templates.Count );


            foreach ( CodeTemplateSource template in templates )
            {
                AddTemplateAndDependencies( resolvedTemplates, template );
            }

            RemoveExistantTemplates( resolvedTemplates, targetType, @namespace );
            if ( resolvedTemplates.Count <= 0 )
            {
                Debug.LogWarning( "[Generation cancel] : All templates requested already exist. Nothing to generate" );
                generationEndCallback?.Invoke( true );
                yield break;
            }

            reporter.Report( 30.0f );
            yield return null;

            float progress = 30f;
            float step = ( 100f - progress ) / resolvedTemplates.Count;

            bool success = true;
            foreach ( GenericCodeTemplate template in resolvedTemplates )
            {
                success &= GenerateClass( template, targetType, @namespace, outputPath, category );
                progress += step;
                reporter.Report( progress );
                yield return null;
            }

            reporter.Report( 100f );
            generationEndCallback?.Invoke( success );
            yield break;
        }


        public static void RemoveExistantTemplates( List<GenericCodeTemplate> list, Type target, string @namespace )
        {
            for ( int index = 0; index < list.Count; index++ )
            {
                GenericCodeTemplate template = list[index];
                List<Type> types = ReflexionUtils.FindTypesByFullName( $"{@namespace}.{template.GetTypeName( target )}" );
                if ( types == null || types.Count <= 0 )
                {
                    continue;
                }

                // trick to prevent shifting the whole list since order doesn't matter
                list[index] = list[list.Count - 1];
                list.RemoveAt( list.Count - 1 );
                index--;
            }
        }


        public static void AddTemplateAndDependencies( List<GenericCodeTemplate> list, GenericCodeTemplate template )
        {
            if ( !list.AddUnique( template ) )
            {
                return;
            }

            foreach ( GenericCodeTemplate dependency in template.Dependencies )
            {
                AddTemplateAndDependencies( list, dependency );
            }
        }


        public static void AddTemplateAndDependencies( List<GenericCodeTemplate> list, CodeTemplateSource template )
        {
            switch ( template )
            {
                case GenericCodeTemplate genericCodeTemplate:
                    AddTemplateAndDependencies( list, genericCodeTemplate );
                    break;
                case CodeTemplatesPreset codeTemplatesPreset:
                    foreach ( CodeTemplateSource source in codeTemplatesPreset.CodeTemplateSources )
                    {
                        AddTemplateAndDependencies ( list, source );
                    }
                    break;
                default:
                    Debug.LogError( $"[Generation fail] : unsupported template type | {template.GetType()}" );
                    break;
            }
        }


        public static bool GenerateClass( GenericCodeTemplate template, Type target, string @namspace, string outputPath, string category )
        {
            string fileName = template.GetTypeName( target );
            const string EXTENSION = ".generated.cs";
            StringBuilder contentBuilder = new StringBuilder();
            IDisposable wrapper = new NoopDisposable();
            if ( !string.IsNullOrWhiteSpace( @namspace ) )
            {
                contentBuilder.Append( "namespace " ).Append( @namspace );
                wrapper = new CurlyBracketWrapper( contentBuilder );
            }
          
            using ( wrapper )
            {
                contentBuilder.Append( template.GetTypeContent( target, category ) );
            }

            return FileUtils.CreateTextFileAtPath( $"{Application.dataPath}/{outputPath}", fileName, contentBuilder.ToString(), EXTENSION );
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

            if ( type.ImplementsInterface<IGenericScriptable>() )
            {
                return false;
            }

            return type.Name.ToLowerInvariant().Contains( filter );
        }
    }
}
