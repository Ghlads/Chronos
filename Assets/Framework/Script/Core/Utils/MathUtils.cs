using UnityEngine;

namespace Framework.Core
{
    public enum Exponant
    {
        Constant = 0,
        Linear = 1,
        Quadratic = 2,
        Cubic = 3,
        Quartic = 4,
        Quintic = 5,
    }

    public static class MathUtils
    {
        public static float RaiseExponant( float value, Exponant exponant )
        {
            float result = 0.0f;
            for ( int _ = 0; _ < ( int )exponant; _++ )
            {
                result += value;
            }

            return result;
        }


        public static int RaiseExponant( int value, Exponant exponant )
        {
            int result = 0;
            for ( int _ = 0; _ < ( int )exponant; _++ )
            {
                result += value;
            }

            return result;
        }


        public static bool IsNearlyZero( this float toTest, float epsilon = 1E-6f )
        {
            return Mathf.Abs( toTest ) < epsilon;
        }


        public static bool IsNearlyZero( this Vector2 toTest, float epsilon = 1E-6f )
        {
            return toTest.x.IsNearlyZero( epsilon ) && toTest.y.IsNearlyZero( epsilon );
        }


        public static bool IsNearlyZero( this Vector3 toTest, float epsilon = 1E-6f )
        {
            return toTest.x.IsNearlyZero( epsilon ) && toTest.y.IsNearlyZero( epsilon ) && toTest.z.IsNearlyZero( epsilon );
        }


        public static void RotateFromDegree( this ref Vector2 vector, float degree )
        {
            RotateFromRadian( ref vector, Mathf.Deg2Rad * degree );
        }


        public static void RotateFromRadian( this ref Vector2 vector, float radians )
        {
            float x = vector.x;
            float y = vector.y;
            float cos = Mathf.Cos( radians );
            float sin = Mathf.Sin( radians );
            vector.x = ( x * cos ) - ( y * sin );
            vector.y = ( x * sin ) + ( y * cos );
        }


        public static float InverseLerpUnclamp( float a, float b, float t )
        {
            return ( t - a ) / ( b - a );
        }


        public static bool IsAlmostEqual( float a, float b, float epsilon = 1E-6F )
        {
            return Mathf.Abs( a - b ) <= epsilon;
        }


        public static bool Vector3Equal( Vector3 a, Vector3 b, float epsilon = 1E-6F )
        {
            return IsAlmostEqual( a.x, b.x, epsilon ) &&
                   IsAlmostEqual( a.y, b.y, epsilon ) &&
                   IsAlmostEqual( a.z, b.z, epsilon );
        }


        public static bool Vector2Equal( Vector2 a, Vector2 b, float epsilon = 1E-6F )
        {
            return IsAlmostEqual( a.x, b.x, epsilon ) &&
                   IsAlmostEqual( a.y, b.y, epsilon );
        }
    }
}
