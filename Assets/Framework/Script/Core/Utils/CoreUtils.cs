using NUnit.Framework;
using UnityEngine;

namespace Framework.Core
{
    public static class CoreUtils
    {
        public static bool Not( bool value )
        {
            return !value;
        }


        public static void Behaviour_Enable( Behaviour behaviour, bool value )
        {
            behaviour.enabled = value;
        }
    }
}
