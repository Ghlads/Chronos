using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Core.Editor
{
    public static class AnyValueUtils
    {
        public readonly static List<Type> ValidTypeForAnyValue = new()
        {
            typeof( bool ),
            typeof( float ),
            typeof( int ),
            typeof( string ),
            typeof( Vector2 ),
            typeof( Vector3 ),
            typeof( Vector4 ),
            typeof( Rect ),
            typeof( Color ),
            typeof( UnityEngine.Object ),
        };


        private readonly static Dictionary<Type, AnyValue.ValueType> s_typeToEnumMap = new()
        {
            { typeof( bool ), AnyValue.ValueType.Bool },
            { typeof( float ), AnyValue.ValueType.Float },
            { typeof( int ), AnyValue.ValueType.Int },
            { typeof( string ), AnyValue.ValueType.String },
            { typeof( Vector2 ), AnyValue.ValueType.Vector2 },
            { typeof( Vector3 ), AnyValue.ValueType.Vector3 },
            { typeof(Vector4), AnyValue.ValueType.Vector4 },
            { typeof(Rect), AnyValue.ValueType.Rect },
            { typeof(Color), AnyValue.ValueType.Color },
            { typeof( UnityEngine.Object ), AnyValue.ValueType.Object }
        };


        public static AnyValue.ValueType TypeToEnum( Type type )
        {
            if ( type.InheritsFrom<UnityEngine.Object>() )
            {
                return AnyValue.ValueType.Object;
            }

            if ( s_typeToEnumMap.TryGetValue( type, out AnyValue.ValueType outType ) )
            {
                return outType;
            }

            return AnyValue.ValueType.Bool;
        }


        public static bool IsTypeSupported( Type type )
        {
            return s_typeToEnumMap.ContainsKey( type ) || type.InheritsFrom<UnityEngine.Object>();
        }


        public static SerializedProperty GetXProperty( this SerializedProperty anyValueProperty )
        {
            return anyValueProperty.FindPropertyRelative( "m_x" );
        }


        public static SerializedProperty GetYProperty( this SerializedProperty anyValueProperty )
        {
            return anyValueProperty.FindPropertyRelative( "m_y" );
        }


        public static SerializedProperty GetZProperty( this SerializedProperty anyValueProperty )
        {
            return anyValueProperty.FindPropertyRelative( "m_z" );
        }


        public static SerializedProperty GetWProperty( this SerializedProperty anyValueProperty )
        {
            return anyValueProperty.FindPropertyRelative( "m_w" );
        }


        // Setter
        public static void SetBool( SerializedProperty anyValueProperty, bool value )
        {
            SetBoolToProperty( anyValueProperty.GetXProperty(), value );
        }


        public static void SetBoolToProperty( SerializedProperty xProperty, bool value )
        {
            xProperty.floatValue = value ? 1f : 0f;
        }


        public static void SetInt( SerializedProperty anyValueProperty, int value )
        {
            SetIntToProperty( anyValueProperty.GetXProperty(), value );
        }


        public static void SetIntToProperty( SerializedProperty xProperty, int value )
        {
            xProperty.floatValue = MathF.Truncate( value );
        }


        public static void SetFloat( SerializedProperty anyValueProperty, float value )
        {
            SetFloatToProperty( anyValueProperty.GetXProperty(), value );
        }


        public static void SetFloatToProperty( SerializedProperty xProperty, float value )
        {
            xProperty.floatValue = value;
        }


        public static void SetString( SerializedProperty anyValueProperty, string value )
        {
            SetStringToProperty( anyValueProperty.FindPropertyRelative( "m_stringValue" ), value );
        }


        public static void SetStringToProperty( SerializedProperty stringProperty, string value )
        {
            stringProperty.stringValue = value;
        }


        public static void SetObject( SerializedProperty anyValueProperty, UnityEngine.Object value )
        {
            SetObjectToProperty( anyValueProperty.FindPropertyRelative( "m_objectValue" ), value );
        }


        public static void SetObjectToProperty( SerializedProperty objectProperty, UnityEngine.Object value )
        {
            objectProperty.objectReferenceValue = value;
        }


        public static void SetVector2( SerializedProperty anyValueProperty, Vector2 value )
        {
            SetVector2ToProperties( anyValueProperty.GetXProperty(), anyValueProperty.GetYProperty(), value );
        }


        public static void SetVector2ToProperties( SerializedProperty xProperty, SerializedProperty yProperty, Vector2 value )
        {
            xProperty.floatValue = value.x;
            yProperty.floatValue = value.y;
        }


        public static void SetVector3( SerializedProperty anyValueProperty, Vector3 value )
        {
            SetVector3ToProperties( 
                anyValueProperty.GetXProperty(), 
                anyValueProperty.GetYProperty(), 
                anyValueProperty.GetZProperty(), 
                value );
        }


        public static void SetVector3ToProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, Vector3 value )
        {
            xProperty.floatValue = value.x;
            yProperty.floatValue = value.y;
            zProperty.floatValue = value.z;
        }


        public static void SetVector4( SerializedProperty anyValueProperty, Vector4 value )
        {
            SetVector4ToProperties( 
                anyValueProperty.GetXProperty(), 
                anyValueProperty.GetYProperty(), 
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty(),
                value );
        }


        public static void SetVector4ToProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty, Vector4 value )
        {
            xProperty.floatValue = value.x;
            yProperty.floatValue = value.y;
            zProperty.floatValue = value.z;
            wProperty.floatValue = value.w;
        }


        public static void SetRect( SerializedProperty anyValueProperty, Rect value )
        {
            SetRectToProperties(
                anyValueProperty.GetXProperty(),
                anyValueProperty.GetYProperty(),
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty(),
                value );
        }


        public static void SetRectToProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty, Rect value )
        {
            xProperty.floatValue = value.x;
            yProperty.floatValue = value.y;
            zProperty.floatValue = value.width;
            wProperty.floatValue = value.height;
        }


        public static void SetColor( SerializedProperty anyValueProperty, Color value )
        {
            SetColorToProperties(
                anyValueProperty.GetXProperty(),
                anyValueProperty.GetYProperty(),
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty(),
                value );
        }


        public static void SetColorToProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty, Color value )
        {
            xProperty.floatValue = value.r;
            yProperty.floatValue = value.g;
            zProperty.floatValue = value.b;
            wProperty.floatValue = value.a;
        }
        // Setter


        // Getter
        public static bool GetBool( SerializedProperty anyValueProperty )
        {
            return GetFloat( anyValueProperty ) != 0f;
        }


        public static bool GetBoolFromProperty( SerializedProperty xProperty )
        {
            return xProperty.floatValue != 0f;
        }


        public static float GetFloat( SerializedProperty anyValueProperty )
        {
            return GetFloatFromProperty( anyValueProperty.GetXProperty() );
        }


        public static float GetFloatFromProperty( SerializedProperty xProperty )
        {
            return xProperty.floatValue;
        }


        public static int GetInt( SerializedProperty anyValueProperty )
        {
            return (int)GetFloat( anyValueProperty );
        }


        public static int GetIntFromProperty( SerializedProperty xProperty )
        {
            return ( int )xProperty.floatValue;
        }


        public static string GetString( SerializedProperty anyValueProperty )
        {
            return GetStringFromProperty( anyValueProperty.FindPropertyRelative( "m_stringValue" ) );
        }


        public static string GetStringFromProperty( SerializedProperty stringProperty )
        {
            return stringProperty.stringValue;
        }


        public static T GetObject<T>( SerializedProperty anyValueProperty ) where T : UnityEngine.Object
        {
            return GetObjectFromProperty<T>( anyValueProperty.FindPropertyRelative( "m_objectValue" ) );
        }


        public static T GetObjectFromProperty<T>( SerializedProperty objectProperty ) where T : UnityEngine.Object
        {
            return objectProperty.objectReferenceValue as T;
        }


        public static Vector2 GetVector2( SerializedProperty anyValueProperty )
        {
            return GetVector2FromProperties( anyValueProperty.GetXProperty(), anyValueProperty.GetYProperty() );
        }


        public static Vector2 GetVector2FromProperties( SerializedProperty xProperty, SerializedProperty yProperty )
        {
            return new Vector2() 
            {
                x = xProperty.floatValue,
                y = yProperty.floatValue
            };
        }


        public static Vector3 GetVector3( SerializedProperty anyValueProperty )
        {
            return GetVector3FromProperties( 
                anyValueProperty.GetXProperty(), 
                anyValueProperty.GetYProperty(), 
                anyValueProperty.GetZProperty() );
        }


        public static Vector3 GetVector3FromProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty )
        {
            return new Vector3()
            {
                x = xProperty.floatValue,
                y = yProperty.floatValue,
                z = zProperty.floatValue
            };
        }


        public static Vector4 GetVector4( SerializedProperty anyValueProperty )
        {
            return GetVector4FromProperties(
                anyValueProperty.GetXProperty(),
                anyValueProperty.GetYProperty(),
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty() );
        }


        public static Vector4 GetVector4FromProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty )
        {
            return new Vector4()
            {
                x = xProperty.floatValue,
                y = yProperty.floatValue,
                z = zProperty.floatValue,
                w = wProperty.floatValue
            };
        }


        public static Rect GetRect( SerializedProperty anyValueProperty )
        {
            return GetRectFromProperties(
                anyValueProperty.GetXProperty(),
                anyValueProperty.GetYProperty(),
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty() );
        }


        public static Rect GetRectFromProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty )
        {
            return new Rect()
            {
                x = xProperty.floatValue,
                y = yProperty.floatValue,
                width = zProperty.floatValue,
                height = wProperty.floatValue
            };
        }


        public static Color GetColor( SerializedProperty anyValueProperty )
        {
            return GetColorFromProperties(
                anyValueProperty.GetXProperty(),
                anyValueProperty.GetYProperty(),
                anyValueProperty.GetZProperty(),
                anyValueProperty.GetWProperty() );
        }


        public static Color GetColorFromProperties( SerializedProperty xProperty, SerializedProperty yProperty, SerializedProperty zProperty, SerializedProperty wProperty )
        {
            return new Color()
            {
                r = xProperty.floatValue,
                g = yProperty.floatValue,
                b = zProperty.floatValue,
                a = wProperty.floatValue
            };
        }
        // Getter
    }
}
