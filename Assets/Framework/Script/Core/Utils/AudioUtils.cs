using UnityEngine;

namespace Framework.Core
{
    public static class AudioUtils
    {
        public static float LinearToDb( float value )
        {
            if ( value <= 0.0001f )
            {
                return -80f;
            }

            return Mathf.Log10( value ) * 20f;
        }
    }
}
