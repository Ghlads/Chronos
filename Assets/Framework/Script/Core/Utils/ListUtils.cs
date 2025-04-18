using System.Collections.Generic;

namespace Framework
{
    public static class ListUtils
    {
        public static T FirstOrDefaultNoException<T>( this List<T> list )
        {
            if ( list == null )
            {
                return default;
            }

            return list.Count > 0 ? list[0] : default;
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
    }
}
