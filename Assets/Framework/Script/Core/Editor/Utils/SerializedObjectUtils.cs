using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core.Editor
{
    public static class SerializedObjectUtils
    {
        public static bool Is<T>( this SerializedObject serializedObject ) where T: UnityEngine.Object
        {
            return serializedObject.targetObject.GetType().InheritsFrom<T>();
        }


        public static bool IsIn<T>( this SerializedProperty property ) where T : UnityEngine.Object
        {
            return property.serializedObject.Is<T>();
        }


        public static void ApplyModificationAndUpdate( this SerializedObject serializedObject )
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        public static void ApplyModificationAndUpdate( this SerializedProperty property )
        {
            property.serializedObject.ApplyModificationAndUpdate();
        }


        public static void SetArrayProperties<T>( this SerializedProperty property, T[] array, Action<SerializedProperty, T> setter  )
        {
            Assert.IsTrue( property.isArray );
            property.arraySize = array.Length;
            for ( int index = 0; index < array.Length; index++ )
            {
                setter( property.GetArrayElementAtIndex( index ), array[index] );
            }
        }


        public static void SetArrayProperties<T>( this SerializedProperty property, List<T> list, Action<SerializedProperty, T> setter )
        {
            Assert.IsTrue( property.isArray );
            property.arraySize = list.Count;
            for ( int index = 0; index < list.Count; index++ )
            {
                setter( property.GetArrayElementAtIndex( index ), list[index] );
            }
        }


        public static T[] GetArrayProperties<T>( this SerializedProperty property, Func<SerializedProperty,T> getter )
        {
            Assert.IsTrue ( property.isArray );
            T[] values = new T[property.arraySize];
            for ( int index = 0; index < property.arraySize; index++ )
            {
                values[index] = getter( property.GetArrayElementAtIndex( index ) );
            }

            return values;
        }


        public static string GetAssetPath( this SerializedProperty property )
        {
            return property.serializedObject.GetAssetPath();
        }


        public static string GetAssetPath( this SerializedObject @object )
        {
            if ( @object == null )
            {
                return string.Empty;
            } 


            if ( @object.targetObject is GameObject go )
            {
                return GetPathForGameObject( go );
            }
            else if ( @object.targetObject is UnityEngine.Component component )
            {
                return GetPathForGameObject( component.gameObject );
            }


            return AssetDatabase.GetAssetPath( @object.targetObject );
        }


        public static GameObject[] GetRootObjects( this SerializedProperty property )
        {
            return property.serializedObject.GetRootObjects();
        }


        public static GameObject[] GetRootObjects( this SerializedObject serializedObject )
        {
            if ( serializedObject == null )
            {
                return new GameObject[0];
            }


            if ( serializedObject.targetObject is GameObject go )
            {
                return GetRootObjectsFrom( go );
            }
            else if ( serializedObject.targetObject is UnityEngine.Component component )
            {
                return GetRootObjectsFrom( component.gameObject );
            }


            return new GameObject[0];
        }


        public static string GetPathForGameObject( GameObject go )
        {
            if ( go == null )
            {
                return string.Empty;                
            }

            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if ( prefabStage != null && prefabStage.scene.IsValid() )
            {
                if ( go.scene == prefabStage.scene )
                {
                    return prefabStage.assetPath;
                }
            }

            UnityEngine.SceneManagement.Scene scene = go.scene;
            if ( scene.IsValid() && scene.path != null )
            {
                return scene.path;
            }

            return AssetDatabase.GetAssetPath( go );
        }


        public static GameObject[] GetRootObjectsFrom( GameObject go )
        {
            if ( go == null )
            {
                return new GameObject[0];
            }

            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if ( prefabStage != null && prefabStage.scene.IsValid() )
            {
                if ( go.scene == prefabStage.scene )
                {
                    return new GameObject[1] { prefabStage.prefabContentsRoot };
                }
            }

            UnityEngine.SceneManagement.Scene scene = go.scene;
            if ( scene.IsValid() && scene.path != null )
            {
                return scene.GetRootGameObjects();
            }

            return new GameObject[0];
        }


        public static bool IsPartOfPrefabInstance( this SerializedProperty property )
        {
            if ( property == null || property.serializedObject == null )
            {
                return false;
            }
            try
            {
                return PrefabUtility.GetPrefabInstanceStatus( property.serializedObject.targetObject ) == PrefabInstanceStatus.Connected;
            }
            catch ( Exception )
            {
                return false;
            }
        }


        public static string GetSourcePrefabPath( SerializedProperty property )
        {
            if ( property == null )
            {
                return string.Empty;
            }

            UnityEngine.Object targetObject = property.serializedObject?.targetObject;
            if ( targetObject == null )
            {
                return string.Empty;
            }

            if ( PrefabUtility.GetPrefabInstanceStatus( targetObject ) != PrefabInstanceStatus.Connected )
            {
                return string.Empty;
            }

            UnityEngine.Object prefabSource = PrefabUtility.GetCorrespondingObjectFromSource( targetObject );
            if ( prefabSource == null )
            {
                return string.Empty;
            }

            string path = AssetDatabase.GetAssetPath( prefabSource );
            return path ?? string.Empty;
        }
    }
}
