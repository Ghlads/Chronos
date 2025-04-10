using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum Exponant
{
    Constant = 0,
    Linear = 1,
    Quadratic = 2,
    Cubic = 3,
    Quartic = 4,
    Quintic = 5,
}

public static class Operation
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
}
