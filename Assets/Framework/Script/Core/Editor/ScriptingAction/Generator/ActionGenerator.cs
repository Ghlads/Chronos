using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    public class ActionAssetHook : AssetModificationProcessor
    {
        [InitializeOnLoadMethod]
        public static void RegisterWatcher()
        {
            EditorSceneManager.sceneOpened += SceneOpenHandler;
            PrefabStage.prefabStageOpened += PrefabStageOpenHandler;
            AssemblyReloadEvents.beforeAssemblyReload += ActionGenerator.AssemblyPreReloadHandler;
            AssemblyReloadEvents.afterAssemblyReload += ActionGenerator.AssemblyPostReloadHandler;

            for ( int index = 0; index < EditorSceneManager.sceneCount; index++ )
            {
                UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetSceneAt( index );
                if ( scene.isLoaded )
                {
                    SceneOpenHandler( scene, OpenSceneMode.Single );
                }
            }

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if ( prefabStage != null )
            {
                PrefabStageOpenHandler( prefabStage );
            }
        }


        private static void PrefabStageOpenHandler( PrefabStage stage )
        {
            ActionGenerator.OpenAsset( stage.assetPath, new GameObject[]{ stage.prefabContentsRoot } );
        }


        private static void SceneOpenHandler( UnityEngine.SceneManagement.Scene scene, OpenSceneMode _ )
        {
            ActionGenerator.OpenAsset( scene.path, scene.GetRootGameObjects() );
        }


        public static string[] OnWillSaveAssets( string[] assets )
        {
            foreach ( string asset in assets )
            {
                ActionGenerator.SaveAsset( asset );
            }

            return assets;
        }
    }


    public static class ActionGenerator
    {
        private struct ActionPriorityPair
        {
            public int Priority;
            public Action Action;
        }

        private static readonly List<ActionPriorityPair> s_waitForSettingsValidQueue = new List<ActionPriorityPair>();

        private static ActionGeneratorSettings s_settings;
        public static ActionGeneratorSettings Settings
        {
            get 
            {
                if ( s_settings != null )
                {
                    return s_settings;
                }

                s_settings = Resources.Load<ActionGeneratorSettings>( "ActionGeneratorSettings" );
                if ( s_settings != null )
                {
                    foreach ( ActionPriorityPair pair in s_waitForSettingsValidQueue )
                    {
                        pair.Action?.Invoke();
                    }

                    s_waitForSettingsValidQueue.Clear();
                }

                return s_settings;
            }
        }


        private static void InsertByPriority( ActionPriorityPair pair )
        {
            for ( int index = 0; index < s_waitForSettingsValidQueue.Count; index++ )
            {
                if ( s_waitForSettingsValidQueue[index].Priority > pair.Priority )
                {
                    s_waitForSettingsValidQueue.Insert( index, pair );
                    return;
                }
            }

            s_waitForSettingsValidQueue.Add( pair );
        }


        private static readonly Dictionary<string, AssetContext> s_schemaMaps = new();

        [Serializable]
        public struct PathContextPair
        {
            public string Path;
            public AssetContext AssetContext;
        }


        [Serializable]
        public class GeneratorContext
        {
            public List<PathContextPair> PathContexts;
        }


        public const string RECOMPILE_TEMP_JSON_KEY = "RECOMPILE_TEMP_JSON_KEY";
        public static void AssemblyPreReloadHandler()
        {
            GeneratorContext context = new GeneratorContext();
            context.PathContexts = new( s_schemaMaps.Count );
            foreach ( KeyValuePair<string, AssetContext> pair in s_schemaMaps )
            {
                context.PathContexts.Add( new() { Path = pair.Key, AssetContext = pair.Value } );
            }

            string json = JsonUtility.ToJson( context, true );
            EditorPrefs.SetString( RECOMPILE_TEMP_JSON_KEY, json );
        }


        public static void AssemblyPostReloadHandler()
        {
            if ( EditorPrefs.HasKey( RECOMPILE_TEMP_JSON_KEY ) )
            {
                try
                {
                    GeneratorContext context = JsonUtility.FromJson<GeneratorContext>( EditorPrefs.GetString( RECOMPILE_TEMP_JSON_KEY ) );
                    if ( context != null )
                    {
                        foreach ( PathContextPair pair in context.PathContexts )
                        {
                            s_schemaMaps[pair.Path] = pair.AssetContext;
                        }
                    }
                }
                catch ( Exception ) { }
         
                EditorPrefs.DeleteKey( RECOMPILE_TEMP_JSON_KEY );
            }
        }


        public static void GetOrCreateSchema( string assetPath, SerializedProperty property, GameObject[] rootObjects, ID ID, Action<ActionSchema> callback )
        {
            if ( Settings == null )// legacy behaviour should be removed
            {
                InsertByPriority( new ActionPriorityPair() { 
                    Priority = 1,
                    Action = () =>
                    {
                        Logic( assetPath, property, rootObjects, ID, callback );
                    }
                } );
                return;
            }

            Logic( assetPath, property, rootObjects, ID, callback );
            static void Logic( string assetPath, SerializedProperty property, GameObject[] rootObjects, ID ID, Action<ActionSchema> callback )
            {
                Assert.IsNotNull( callback );
                if ( RetrieveActionSchema( assetPath, property, ID, callback ) )
                {
                    return;
                }

                OpenAsset( assetPath, rootObjects );
                if ( RetrieveActionSchema( assetPath, property, ID, callback ) )
                {
                    return;
                }

                Debug.LogError( $"Asset at path [{assetPath}] wasn't open for edition" );
                callback.Invoke( null );
            }
        }


        private static bool RetrieveActionSchema( string assetPath, SerializedProperty property, ID ID, Action<ActionSchema> callback )
        {
            if ( s_schemaMaps.TryGetValue( assetPath, out AssetContext context ) )
            {
                foreach ( AssetSchema.IDSchemaPair pair in context.AssetSchema.IDSchemaPairs )
                {
                    if ( pair.ID == ID )
                    {
                        callback.Invoke( pair.Action );
                        return true;
                    }
                }

                string prefabPath = SerializedObjectUtils.GetSourcePrefabPath( property );
                if ( !string.IsNullOrEmpty( prefabPath ) )
                {
                    TryGetRootGameObjects( prefabPath, out List<GameObject> list );
                    OpenAsset( prefabPath, list.ToArray() );// ineficiant but first editor pass
                    if ( RetrieveActionSchema( prefabPath, property, ID, schema =>
                    {
                        context.AssetSchema.IDSchemaPairs.AddUnique( new AssetSchema.IDSchemaPair() { ID = ID, Action = schema } );
                        callback.Invoke( context.AssetSchema.IDSchemaPairs.Last().Action );
                    } ) )
                    {
                        return true;
                    }
                }

                context.AssetSchema.IDSchemaPairs.AddUnique( new AssetSchema.IDSchemaPair() { ID = ID, Action = new() } );
                callback.Invoke( context.AssetSchema.IDSchemaPairs.Last().Action );
                return true;
            }

            return false;
        }


        public static string ConvertAssetPathToGeneratedFilesPath( string assetPath )
        {
            if ( assetPath == null || assetPath.Length <= 7 )
            {
                return string.Empty;
            }

            if ( assetPath.StartsWith( "Assets/" ) )
            {
                assetPath = assetPath.Substring( 7 );
            }

            return "Assets/" + Settings.GeneratedFilePath + assetPath;
        }


        private static string ConvertAssetPathToExtensionPath( string assetPath, string extension )
        {
            if ( assetPath == null || assetPath.Length <= 7 )
            {
                return string.Empty;
            }

            if ( assetPath.StartsWith( "Assets/" ) )
            {
                assetPath = assetPath.Substring( 7 );
            }

            return "Assets/" + Settings.GeneratedFilePath + assetPath.WithoutExtension() + extension;
        }


        public static void OpenAsset( string assetPath, GameObject[] rootObjects )
        {
            if ( string.IsNullOrEmpty( assetPath ) )
            {
                return;
            }

            if ( Settings == null )
            {
                InsertByPriority( new ActionPriorityPair()
                {
                    Priority = 0,
                    Action = () =>
                    {
                        Logic( assetPath, rootObjects );
                    }
                } );

                return;
            }

            Logic( assetPath, rootObjects );

            static void Logic( string assetPath, GameObject[] rootObjects )
            {
                Assert.IsNotNull( rootObjects );
                string[] splitPathName = assetPath.SplitPathAndName();
                string generatedFilesPath = ConvertAssetPathToGeneratedFilesPath( splitPathName[0] );
                string jsonPath = generatedFilesPath + "/" + splitPathName[1] + ".json";
                TextAsset json = null;
                try
                {
                    json = AssetDatabase.LoadAssetAtPath<TextAsset>( jsonPath );
                }
                catch ( Exception e )
                {
                    Debug.LogException( e );
                }

                AssetSchema schema = new();
                if ( json != null )
                {
                    try
                    {
                        schema = JsonUtility.FromJson<AssetSchema>( json.text );
                    }
                    catch ( Exception e )
                    {
                        Debug.LogException( e );
                    }
                }

                AssetContext context = new AssetContext()
                {
                    AssetSchema = schema,
                    AssetPath = splitPathName[0],
                    AssetName = splitPathName[1],
                    GeneratedFilesPath = generatedFilesPath,
                    RootObjects = new( rootObjects )
                };
                s_schemaMaps[assetPath] = context;
            }
        }


        public static void OverrideActionForID( string path, ID previousID, ID newID )
        {
            if ( !s_schemaMaps.TryGetValue( path, out AssetContext map ) )
            {
                return;
            }

            for ( int index = 0; index < map.AssetSchema.IDSchemaPairs.Count; index++ )
            {
                if ( map.AssetSchema.IDSchemaPairs[index].ID == previousID )
                {
                    ActionSchema schema = map.AssetSchema.IDSchemaPairs[index].Action;
                    map.AssetSchema.IDSchemaPairs[index] = new AssetSchema.IDSchemaPair() { ID = newID, Action = schema };
                    Debug.Log( $"action with ID {previousID} was overriden with {newID}" );
                    return;
                }
            }

            Debug.Log( $"No action with ID {previousID} to override" );
        }


        public static List<KeyValuePair<ID, SerializedProperty>> ExtractAllActyxIDs( List<GameObject> rootObjects )
        {
            List<KeyValuePair<ID, SerializedProperty>> foundIds = new List<KeyValuePair<ID, SerializedProperty>>();

            foreach ( GameObject root in rootObjects )
            {
                if ( root == null )
                {
                    continue;
                }

                Component[] components = root.GetComponentsInChildren<Component>( true );
                foreach ( Component component in components )
                {
                    if ( component == null )
                    {
                        continue;
                    }

                    ScanObject( new SerializedObject( component ), null, component, foundIds, new HashSet<object>() );
                }
            }

            return foundIds;
        }


        private static void ScanObject( SerializedObject owner, SerializedProperty parentProperty, object obj, List<KeyValuePair<ID, SerializedProperty>> output, HashSet<object> visited )
        {
            if ( obj == null || visited.Contains( obj ) )
            {
                return;
            }

            visited.Add( obj );

            Type objType = obj.GetType();
            FieldInfo[] fields = objType.GetFields( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );

            foreach ( FieldInfo field in fields )
            {
                if ( !IsSerializedField( field ) )
                {
                    continue;
                }

                object value;
                try
                {
                    value = field.GetValue( obj );
                }
                catch
                {
                    continue;
                }

                if ( value == null || value is null )
                {
                    continue;
                }

                Type fieldType = field.FieldType;

                if ( IsActyxType( fieldType ) )
                {
                    FieldInfo idField = fieldType.GetFieldInHierarchy( "m_id", BindingFlags.Instance | BindingFlags.NonPublic );
                    if ( idField != null )
                    {
                        ID id = idField.GetValue( value ) as ID;
                        if ( id != null )
                        {
                            output.Add( new() { Key = id, Value = FindProperty( "m_id" ) } );
                        }
                    }
                }
                else if ( typeof( IEnumerable ).IsAssignableFrom( fieldType ) && fieldType != typeof( string ) )
                {
                    try
                    {
                        SerializedProperty enumerableProperty = FindProperty( field.Name );
                        
                        int index = 0;
                        foreach ( object item in ( IEnumerable )value )
                        {
                            if ( item != null )
                            {
                                ScanObject( owner, enumerableProperty != null && enumerableProperty.isArray ? enumerableProperty.GetArrayElementAtIndex( index ) : parentProperty, item, output, visited );
                            }

                            index++;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if ( !fieldType.IsPrimitive && !fieldType.IsEnum && !fieldType.IsValueType && fieldType != typeof( string ) )
                {
                    ScanObject( owner, FindProperty( field.Name ), value, output, visited );
                }
            }


            SerializedProperty FindProperty( string name )
            {
                return parentProperty != null ? parentProperty.FindPropertyRelative( name ) : owner.FindProperty( name );
            }
        }


        private static bool IsSerializedField( FieldInfo field )
        {
            return field.IsPublic || Attribute.IsDefined( field, typeof( SerializeField ) );
        }


        private static bool IsActyxType( Type type )
        {
            return type.InheritsFrom( typeof( Actyx<,,,> ) );
        }


        public static bool TryGetRootGameObjects( string assetPath, out List<GameObject> roots )
        {
            roots = new List<GameObject>();

            if ( string.IsNullOrEmpty( assetPath ) )
            {
                Debug.Log( "empty path" );
                return false;
            }

            if ( assetPath.EndsWith( ".prefab" ) )
            {
                GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>( assetPath );
                if ( prefabRoot != null )
                {
                    roots.Add( prefabRoot );
                    return true;
                }
            }
            else if ( assetPath.EndsWith( ".unity" ) )
            {
                UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetSceneByPath( assetPath );

                if ( scene.IsValid() && scene.isLoaded )
                {
                    roots.AddRange( scene.GetRootGameObjects() );
                    return true;
                }
                else
                {
                    UnityEngine.SceneManagement.Scene tempScene = EditorSceneManager.OpenScene( assetPath, OpenSceneMode.Additive );
                    if ( !tempScene.IsValid() )
                    {
                        Debug.Log( "temp scene invalid" );
                        return false;
                    }

                    roots.AddRange( tempScene.GetRootGameObjects() );

                    EditorSceneManager.CloseScene( tempScene, true );
                    return true;
                }
            }

            Debug.Log( $"Unsupported asset type : {assetPath}" );
            return false;
        }


        public static void SaveAsset( string assetPath )
        {
            if ( s_schemaMaps.TryGetValue( assetPath, out AssetContext context ) )
            {
                s_schemaMaps.Remove( assetPath );
                if ( context.RootObjects == null && !TryGetRootGameObjects( context.AssetPath, out context.RootObjects ) )
                {
                    Debug.LogError( "couldn't get root objects" );
                }

                List<KeyValuePair<ID, SerializedProperty>> assetIDsAndProperties = ExtractAllActyxIDs( context.RootObjects );
                foreach( KeyValuePair<ID, SerializedProperty> pair in assetIDsAndProperties )
                {
                    if ( AlreadyPresent( pair.Key, context.AssetSchema.IDSchemaPairs ) )
                    {
                        continue;
                    }

                    if ( pair.Value != null )
                    {
                        if ( pair.Value.IsPartOfPrefabInstance() && !pair.Value.prefabOverride )
                        {
                            continue;
                        }
                    }

                    context.AssetSchema.IDSchemaPairs.AddUnique( new AssetSchema.IDSchemaPair() { ID = pair.Key, Action = new ActionSchema() } );
                }

                if ( assetIDsAndProperties.Count <= 0 )
                {
                    context.AssetSchema.IDSchemaPairs.Clear();// all action were removed
                }

                SetupActionForGeneration( context.AssetSchema.IDSchemaPairs, assetIDsAndProperties );
                Generate( context );
            }


            bool AlreadyPresent( ID id, List<AssetSchema.IDSchemaPair> pairs )
            {
                foreach ( AssetSchema.IDSchemaPair pair in context.AssetSchema.IDSchemaPairs )
                {
                    if ( pair.ID == id )
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        public static void SetupActionForGeneration( List<AssetSchema.IDSchemaPair> pairs, List<KeyValuePair<ID, SerializedProperty>> sceneIDs )
        {
            List<ID> ids = new List<ID>( sceneIDs.Count );
            foreach ( KeyValuePair<ID, SerializedProperty> scenePair in sceneIDs )
            {
                ids.Add( scenePair.Key );
            }

            for ( int index = 0; index < pairs.Count; index++ )
            {
                AssetSchema.IDSchemaPair pair = pairs[index];
                ActionSchema schema = pair.Action;
                if ( schema.IsOverridable && !schema.IsOverridden )
                {
                    pairs.SwapRemoveAtIndex( index );
                    continue;
                }

                if ( !ids.Contains( pair.ID ) )
                {
                    pairs.SwapRemoveAtIndex( index );
                }
            }
        }


        [Serializable]
        public class AssetContext
        {
            public AssetSchema AssetSchema;
            public string GeneratedFilesPath;
            public string AssetPath;
            public string AssetName;
            [NonSerialized] public List<GameObject> RootObjects;
        }


        public static void Generate( AssetContext context )
        {
            AsyncGenerate( context );
        }


        private static async void AsyncGenerate( AssetContext context )
        {
            await GenerateJSON( context );
            await GenerateCSharp( context );
            MainThreadDispather.Execute( () =>
            {
                if ( Settings.AssemblyDefinition != null )
                {
                    string referenceName = Settings.AssemblyDefinition.name;
                    string asmrefJson = $"{{\n  \"reference\": \"GUID:{AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( Settings.AssemblyDefinition ) )}\"\n}}";

                    string asmrefPath = Path.Combine( Application.dataPath + "/" + Settings.GeneratedFilePath, "GeneratedAssemblyReference.asmref" );
                    File.WriteAllText( asmrefPath, asmrefJson );
                }

                AssetDatabase.Refresh();
            } );
        }


        private static Task GenerateJSON( AssetContext context )
        {
            try
            {
                string path = Application.dataPath.Substring( 0, Application.dataPath.Length - "Assets".Length ) + context.GeneratedFilesPath;
                string json = EditorJsonUtility.ToJson( context.AssetSchema, prettyPrint: true );
                Directory.CreateDirectory( path );
                File.WriteAllText( path + '/' + context.AssetName + ".json", json );
            }
            catch ( Exception ex )
            {
                Debug.LogException( ex );
            }

            return Task.CompletedTask;
        }


        private static Task GenerateCSharp( AssetContext context )
        {
            try
            {
                string path = Application.dataPath.Substring( 0, Application.dataPath.Length - "Assets".Length ) + context.GeneratedFilesPath;
                string cs = ConvertContextToCSharpCode( context );
                Directory.CreateDirectory( path );
                File.WriteAllText( path + '/' + context.AssetName + ".generated.cs", cs );
            }
            catch ( Exception ex )
            {
                Debug.LogException( ex );
            }

            return Task.CompletedTask;
        }

        
        private static string ConvertContextToCSharpCode( AssetContext context )
        {
            IndentedStringBuilder builder = new IndentedStringBuilder();
            builder.Append( "// this file is auto-generated, all manual edit will be lost on the next generation" ).AppendLine();
            builder.Append( "// if you want to edit those function, do so through the inspector of the matching action" ).AppendLine();
            builder.Append( "namespace " ).Append( Settings.Namespace );
            using ( new CurlyBracketWrapper( builder ) )
            {
                builder.Append( "public static class " ).Append( context.AssetName.WithoutExtension() ).Append( "GameplayAction" );
                using ( new CurlyBracketWrapper( builder ) )
                {

                    foreach ( AssetSchema.IDSchemaPair pair in context.AssetSchema.IDSchemaPairs )
                    {
                        builder.AppendAction( pair );
                    }

                    WriteRegistrationMethod( context, builder );
                }
            }

            return builder.ToString();
        }


        private static IndentedStringBuilder AppendAction( this IndentedStringBuilder builder, AssetSchema.IDSchemaPair pair )
        {
            builder.Append( "public static void Action" ).Append( pair.ID.ToString().RemoveCharInstances( '-' ) ).Append( "( object[] args )" );
            using ( new CurlyBracketWrapper( builder ) )
            {

                for ( int modifierIndex = 0; modifierIndex < pair.Action.ModifierSchemas.Count; modifierIndex++ )
                {
                    ModifierSchema modifier = pair.Action.ModifierSchemas[modifierIndex];
                    if ( !modifier.IsLight )
                    {
                        if ( modifier.Method.Method.ReturnType != typeof( void ) )
                        {
                            builder.Append( $"{modifier.Method.Method.ReturnType.Beautified().FullName} var{modifierIndex} = " );
                        }

                        builder.AppendModifier( pair.Action, modifier ).Append( ";" ).AppendLine();
                    }
                }
            }

            return builder.AppendLine().AppendLine();
        }


        private static IndentedStringBuilder AppendModifier( this IndentedStringBuilder builder, ActionSchema action, ModifierSchema modifier )
        {
            return builder.Append( $"{modifier.Method.Method.DeclaringType.FullName}.{modifier.Method.Method.Name}( " )
                        .AppendParameters( action, modifier ).Append( " )" );
        }


        private static IndentedStringBuilder AppendParameters( this IndentedStringBuilder builder, ActionSchema action, ModifierSchema modifier )
        {
            for ( int parameterIndex = 0; parameterIndex < modifier.Parameters.Count - 1; parameterIndex++ )
            {
                builder.AppendParameter( modifier.Index, action, modifier.Parameters[parameterIndex] ).Append( ", " );
            }

            if ( modifier.Parameters.Count > 0 )
            {
                builder.AppendParameter( modifier.Index, action, modifier.Parameters[modifier.Parameters.Count - 1] );
            }

            return builder;
        }


        private static IndentedStringBuilder AppendParameter( this IndentedStringBuilder builder, int owningModifierIndex, ActionSchema action, ParameterSchema parameter )
        {
            builder.AppendCastToType( parameter.ExpectedType.Beautified() );
            switch ( parameter.Source )
            {
                case ParameterSchema.Sources.Const:
                    string getType;
                    if ( parameter.ExpectedType.InheritsFrom<UnityEngine.Object>() )
                    {
                        getType = $"{nameof( UnityEngine )}.{nameof( UnityEngine.Object )}";
                    }
                    else
                    {
                        getType = parameter.ExpectedType.Beautified().FullName;
                    }

                    builder.Append( $"( ( {nameof( Framework )}.{nameof( Framework.Core )}.{nameof( Framework.Core.AnyValue )} )( ( {nameof( Framework )}.{nameof( Framework.Core )}.{nameof( Framework.Core.ModifierArgs )} )args[" )
                        .Append( "4 + " ).Append( owningModifierIndex )
                        .Append( "] ).Args[").Append( parameter.ConstIndex ).Append("] ).Get<" ).Append( getType ).Append( ">()" );
                    break;
                case ParameterSchema.Sources.Return:
                    ModifierSchema modifier = action.ModifierSchemas[parameter.Index];
                    if ( modifier.IsLight )
                    {
                        builder.AppendModifier( action, modifier );
                    }
                    else
                    {
                        builder.Append( "var" ).Append( parameter.Index );
                    }
                    break;
                case ParameterSchema.Sources.Input:
                    builder.Append( "args[" ).Append( parameter.Index ).Append( "]" );
                    break;
                default:
                    builder.Append( "default" );
                    break;
            }

            return builder;
        }


        private static IndentedStringBuilder AppendCastToType( this IndentedStringBuilder builder, Type type )
        {
            builder.Append( "( " ).Append( type.FullName ).Append( " )" );
            return builder;
        }


        private static void WriteRegistrationMethod( AssetContext context, IndentedStringBuilder builder )
        {
            builder.Append( "[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSplashScreen)]" )
                                    .AppendLine()
                                    .Append( "public static void Register()" );
            using ( new CurlyBracketWrapper( builder ) )
            {
                foreach ( AssetSchema.IDSchemaPair pair in context.AssetSchema.IDSchemaPairs )
                {
                    builder.Append( $"Framework.Core.{nameof( ActyxRegistry )}.{nameof( ActyxRegistry.Register )}( new Framework.Core.ID( \"{pair.ID}\" ), Action{pair.ID.ToString().RemoveCharInstances( '-' )} );" ).AppendLine();
                }
            }
        }


        [InitializeOnLoadMethod]
        private static void ConvertersRegistration()
        {
            ConverterGroups.RegisterGlobalConverter( ( ref Enum value ) => ( ParameterSchema.Sources )value );
            ConverterGroups.RegisterGlobalConverter( ( ref ParameterSchema schema ) =>
            {
                if ( schema.Index == -1 )
                {
                    return "No valid source";
                }

                return $"{schema.Source} : [{schema.Index}]";
            } );
            ConverterGroups.RegisterGlobalConverter( ( ref MethodSchema schema ) => ActionSelectorWindow.MethodInfoToString( schema.Method ) );
            ConverterGroups.RegisterGlobalConverter( ( ref ActionSchema schema ) => ScriptingActionField.DisplayNameAndTypeToString( schema.ActionProperty, schema.ActionTypes ) );
        }
    }


    [Serializable]
    public class AssetSchema
    {
        [Serializable]
        public struct IDSchemaPair
        {
            public ID ID;
            public ActionSchema Action;
        }

        [SerializeField] private List<IDSchemaPair> m_idSchemaPairs = new List<IDSchemaPair>();

        public List<IDSchemaPair> IDSchemaPairs => m_idSchemaPairs;
    }


    [Serializable]
    public class ActionSchema
    {
        [SerializeField] private List<ModifierSchema> m_modifierSchemas = new();
        [SerializeField] private string m_name;
        [SerializeField] private List<TypeSchema> m_inputsType = new();
        [SerializeField] private bool m_isOverridable = false;
        [SerializeField] private bool m_isOverridden = false;
        private SerializedProperty m_actionProperty;
        private Type m_actionTypes;

        public SerializedProperty ActionProperty
        {
            get { return m_actionProperty; }
            set 
            { 
                m_actionProperty = value;
                if ( value != null )
                {
                    m_name = value.displayName;
                    m_isOverridable = value.IsPartOfPrefabInstance();
                    m_isOverridden = m_isOverridable && value.prefabOverride;
                }
                else
                {
                    m_name = string.Empty;
                    m_isOverridable = false;
                    m_isOverridden = false;
                }
            }
        }

        public Type ActionTypes
        {
            get { return m_actionTypes; }
            set { m_actionTypes = value; }
        }


        public bool IsOverridden
        {
            get
            {
                if ( m_isOverridden )
                {
                    return true;
                }

                try
                {
                    m_isOverridden = IsOverridable && m_actionProperty != null && m_actionProperty.prefabOverride;
                }
                catch ( Exception ) {}
                return m_isOverridden;
            }
            set { m_isOverridden = value; }
        }


        public bool IsOverridable
        {
            get
            {
                if ( m_isOverridable )
                {
                    return true;
                }

                try
                {
                    m_isOverridable = m_actionProperty != null && m_actionProperty.IsPartOfPrefabInstance();
                }
                catch ( Exception ) {}
                return m_isOverridable;
            }
            set { m_isOverridable = value; }
        }


        public List<ModifierSchema> ModifierSchemas => m_modifierSchemas;
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }
        public List<TypeSchema> InputTypes => m_inputsType;
    }


    [Serializable]
    public class ModifierSchema
    {
        [SerializeField] private MethodSchema m_method = new();
        [SerializeField] private List<ParameterSchema> m_parameters = new();
        [SerializeField] private bool m_isLight = false;
        [SerializeField] private int m_index = -1;

        public MethodSchema Method => m_method;
        public List<ParameterSchema> Parameters => m_parameters;
        public bool IsLight
        {
            get => m_isLight;
            set => m_isLight = value;
        }


        public int Index
        {
            get => m_index;
            set => m_index = value;
        }
    }


    [Serializable]
    public class ParameterSchema
    {
        public enum Sources
        {
            Const = 0,
            Return = 1,
            Input = 2,
        }


        [SerializeField] private Sources m_sources = Sources.Const;
        [SerializeField] private int m_index = -1;
        [SerializeField] private int m_constIndex = -1;
        [SerializeField] private TypeSchema m_expectedType = new();

        [CreateProperty] public Sources Source
        {
            get => m_sources;
            set => m_sources = value;
        }


        [CreateProperty] public Type ExpectedType
        {
            get => m_expectedType.Type;
            set => m_expectedType.Type = value;
        }


        [CreateProperty] public int Index
        {
            get => m_index;
            set => m_index = value;
        }

        public int ConstIndex
        {
            get => m_constIndex;
            set => m_constIndex = value;
        }

        [CreateProperty] public string Name { get; set; } = string.Empty;
        public SerializedProperty AnyValueProperty { get; set; }
    }


    [Serializable]
    public class MethodSchema
    {
        public const BindingFlags STATIC = BindingFlags.Static | BindingFlags.Public;
        public const BindingFlags INSTANCE = BindingFlags.Instance | BindingFlags.Public;

        [SerializeField] private TypeSchema m_declaringTypeSchema = new();
        [SerializeField] private string m_methodName = string.Empty;
        [SerializeField] private List<TypeSchema> m_parametersTypeSchema = new();
        [SerializeField] private bool m_isStatic = false;
        [NonSerialized] private MethodInfo m_method;


        public List<TypeSchema> ParametersTypeSchema => m_parametersTypeSchema;


        public MethodInfo Method
        {
            get 
            {
                if ( m_method == null )
                {
                    if ( m_declaringTypeSchema.Type == null )
                    {
                        m_declaringTypeSchema.Type = typeof( Actyx );
                        m_methodName = nameof( Actyx.NoopAction );
                        m_isStatic = true;
                        m_parametersTypeSchema.Clear();
                    }

                    m_method = m_declaringTypeSchema.Type.GetMethod( 
                        m_methodName, 
                        m_isStatic ? STATIC : INSTANCE, 
                        null, 
                        TypeSchema.ToTypeArray( m_parametersTypeSchema ), 
                        null );
                }


                return m_method; 
            }
            set 
            { 
                m_method = value;
                m_methodName = value.Name;
                m_isStatic = value.IsStatic;
                m_declaringTypeSchema.Type = value.DeclaringType;
                m_parametersTypeSchema = TypeSchema.FromParameterArray( value.GetParameters() );
            }
        }
    }


    [Serializable]
    public class TypeSchema
    {
        [SerializeField] private string m_declaringAssemblyFullName = string.Empty;
        [SerializeField] private string m_fullName = string.Empty;
        [NonSerialized] private Type m_type = null;

        public TypeSchema() : this( null ) {}


        public TypeSchema( Type type )
        {
            Type = type;
        }


        public Type Type 
        {
            get
            {
                if ( m_type == null )
                {
                    m_type = ReflexionUtils.OptimizedGetType( m_declaringAssemblyFullName, m_fullName );
                }

                return m_type;
            }
            set
            {
                m_type = value;
                if ( m_type != null )
                {
                    m_declaringAssemblyFullName = value.Assembly.FullName;
                    m_fullName = value.FullName;
                }
                else
                {
                    m_declaringAssemblyFullName = string.Empty;
                    m_fullName = string.Empty;
                }
            }
        }


        public static Type[] ToTypeArray( List<TypeSchema> types )
        {
            if ( types == null )
            {
                return null;
            }

            Type[] result = new Type[types.Count];
            for ( int index = 0; index < types.Count; index++ )
            {
                result[index] = types[index].Type;
            }

            return result;
        }


        public static List<TypeSchema> FromParameterArray( ParameterInfo[] infos )
        {
            List<TypeSchema> typeSchemas = new List<TypeSchema>( infos.Length );
            foreach ( ParameterInfo type in infos )
            {
                typeSchemas.Add( new TypeSchema( type.ParameterType ) );
            }

            return typeSchemas;
        }
    }
}
