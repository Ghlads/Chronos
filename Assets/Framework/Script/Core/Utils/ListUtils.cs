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
    }
}
