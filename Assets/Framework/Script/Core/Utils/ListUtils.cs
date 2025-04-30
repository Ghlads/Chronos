using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core
{
    public static class List
    {
        public static T FirstOrDefaultNoException<T>( this List<T> list )
        {
            if ( list == null )
            {
                return default;
            }

            return list.Count > 0 ? list[0] : default;
        }


        public static T FirstOrDefaultNoException<T>( this T[] array )
        {
            if ( array == null )
            {
                return default;
            }

            return array.Length > 0 ? array[0] : default;
        }


        public static bool AddUnique<T>( this List<T> list, T element )
        {
            if ( list == null )
            {
                return false;
            }

            if ( list.Contains( element ) )
            {
                return false;
            }

            list.Add( element );
            return true;
        }


        public static List<EnumType> MakeListFromEnum<EnumType>() where EnumType : Enum
        {
            Array array = typeof( EnumType ).GetEnumValues();
            List<EnumType> list = new List<EnumType>( array.Length );
            foreach ( object value in array )
            {
                list.Add( (EnumType)value );
            }
            return list;
        }


        public static void Foreach<T>( this IEnumerable<T> enumerable, Action<T> action )
        {
            foreach ( T item in enumerable )
            {
                action( item );
            }
        }


        public static void Log<T>( this List<T> list )
        {
            foreach( T item in list )
            {
                Debug.Log( item.ToString() );
            }
        }


        public static List<T> NewFiltered<T>( this List<T> list, Func<T,bool> comparator )
        {
            Assert.IsNotNull( comparator );
            Assert.IsNotNull( list );
            List<T> newList = new List<T>( list.Count );
            for ( int index = 0; index < list.Count; index++ )
            {
                if ( comparator( list[index] ) )
                {
                    newList.Add( list[index] );
                }
            }

            return newList;
        }


        public static List<T> Filter<T>( this List<T> list, Func<T, bool> comparator )
        {
            Assert.IsNotNull( comparator );
            if ( list != null )
            {
                for ( int index = list.Count; index >= 0; index-- )
                {
                    if ( comparator( list[index] ) )
                    {
                        list.RemoveAt( index );
                    }
                }
            }

            return list;
        }
    }
}
