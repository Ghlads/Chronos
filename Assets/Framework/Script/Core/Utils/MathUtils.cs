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

    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public enum Space
    {
        Local = 0,
        World = 1
    }

    public static class MathUtils
    {
        public static float RaiseExponant( float value, Exponant exponant )
        {
            float result = value;
            for ( int _ = 0; _ < ( int )exponant; _++ )
            {
                result *= value;
            }

            return result;
        }


        public static int RaiseExponant( int value, Exponant exponant )
        {
            int result = value;
            for ( int _ = 0; _ < ( int )exponant; _++ )
            {
                result *= value;
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


        public static Vector3 ToVector3( this Vector2 v )
        {
            return new Vector3( v.x, v.y, 0 );
        }


        public static float GetAngleRadBetween( Vector3 a, Vector3 b, Axis axis )
        {
            float angleA = 0;
            float angleB = 0;
            switch ( axis )
            {
                case Axis.X:
                    angleA = Mathf.Atan2( a.z, a.y );
                    angleB = Mathf.Atan2( b.z, b.y );   
                    break;
                case Axis.Y:
                    angleA = Mathf.Atan2( a.x, a.z );
                    angleB = Mathf.Atan2( b.x, b.z );
                    break;
                case Axis.Z:
                    angleA = Mathf.Atan2( a.y, a.x );
                    angleB = Mathf.Atan2( b.y, b.x );
                    break;
            }


            return angleA - angleB;
        }


        /// <summary>
        /// Will constraint position x and y torect bound and apply their current offset on the other side of the bounds
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        public static void ConstraintPositionInRectWarpped( ref Vector3 position, Rect rect )
        {
            if ( position.x < rect.xMin )
            {
                float delta = rect.xMin - position.x;
                position.x = rect.xMax - delta;
            }
            else if ( position.x > rect.xMax )
            {
                float delta = position.x - rect.xMax;
                position.x = rect.xMin + delta;
            }

            if ( position.y < rect.yMin )
            {
                float delta = rect.yMin - position.y;
                position.y = rect.yMax - delta;
            }
            else if ( position.y > rect.yMax )
            {
                float delta = position.y - rect.yMax;
                position.y = rect.yMin + delta;
            }
        }


        /// <summary>
        /// Will constraint position x and y torect bound and apply their current offset on the other side of the bounds
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rect"></param>
        public static void ConstraintPositionInRectWarpped( ref Vector2 position, Rect rect )
        {
            if ( position.x < rect.xMin )
            {
                float delta = rect.xMin - position.x;
                position.x = rect.xMax - delta;
            }
            else if ( position.x > rect.xMax )
            {
                float delta = position.x - rect.xMax;
                position.x = rect.xMin + delta;
            }

            if ( position.y < rect.yMin )
            {
                float delta = rect.yMin - position.y;
                position.y = rect.yMax - delta;
            }
            else if ( position.y > rect.yMax )
            {
                float delta = position.y - rect.yMax;
                position.y = rect.yMin + delta;
            }
        }


        public static void Clamp( this Rect rect, ref Vector2 vec )
        {
            vec = rect.Clamp( vec );
        }


        public static Vector2 Clamp( this Rect rect, Vector2 vec )
        {
            Vector2 clamped = new Vector2();
            clamped.x = Mathf.Clamp( vec.x, rect.xMin, rect.xMax );
            clamped.y = Mathf.Clamp( vec.y, rect.yMin, rect.yMax );
            return clamped;
        }

        /// <summary/>
        /// <param name="rect"></param>
        /// <param name="offset">Support negative values</param>
        /// <returns><paramref name="rect"/> offseted by <paramref name="offset"/></returns>
        public static Rect Resize( this Rect rect, float offset )
        {
            Rect result = new Rect( rect );
            result.min -= Vector2.one * offset;
            result.max += Vector2.one * offset;
            return result;
        }
    }
}
