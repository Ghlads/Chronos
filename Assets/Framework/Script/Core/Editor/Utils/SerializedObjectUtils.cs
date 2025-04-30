using System;
using System.Collections.Generic;
using UnityEditor;
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
    }
}
