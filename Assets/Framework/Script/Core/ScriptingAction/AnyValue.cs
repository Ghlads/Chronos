using System;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public struct AnyValue
    {
        public enum ValueType
        {
            Bool = 0,
            Int = 1,
            Float = 2,
            Vector2 = 3,
            Vector3 = 4,
            Vector4 = 5,
            Color = 6,
            Rect = 7,
            String = 8,
            Object = 9,
        }


        [SerializeField] private ValueType m_type;
        [SerializeField] private string m_stringValue;
        [SerializeField] private float m_x, m_y, m_z, m_w;
        [SerializeField] private UnityEngine.Object m_objectValue;


        public ValueType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if ( m_type == value )
                {
                    return;
                }

                m_x = m_y = m_z = m_w = 0f;
                m_stringValue = null;
                m_objectValue = null;
                m_type = value;
            }
        }


        public AnyValue( int value )
        {
            m_type = ValueType.Int;
            m_x = value;
            m_y = m_z = m_w = 0f;
            m_stringValue = null;
            m_objectValue = null;
        }


        public AnyValue( string value )
        {
            m_type = ValueType.String;
            m_stringValue = value;
            m_x = m_y = m_z = m_w = 0f;
            m_objectValue = null;
        }


        public AnyValue( float value )
        {
            m_type = ValueType.Float;
            m_stringValue = null;
            m_x = value;
            m_y = m_z = m_w = 0f;
            m_objectValue = null;
        }


        public AnyValue( bool value )
        {
            m_type = ValueType.Bool;
            m_stringValue = null;
            m_x = value ? 1f : 0f;
            m_y = m_z = m_w = 0f;
            m_objectValue = null;
        }


        public AnyValue( Vector2 value )
        {
            m_type = ValueType.Vector2;
            m_stringValue = null;
            m_x = value.x;
            m_y = value.y;  
            m_z = m_w = 0f;
            m_objectValue = null;
        }


        public AnyValue( Vector3 value )
        {
            m_type = ValueType.Vector3;
            m_stringValue = null;
            m_x = value.x;
            m_y = value.y;
            m_z = value.z;
            m_w = 0f;
            m_objectValue = null;
        }


        public AnyValue( Rect value )
        {
            m_type = ValueType.Rect;
            m_stringValue = null;
            m_x = value.x;
            m_y = value.y;
            m_z = value.width;
            m_w = value.height;
            m_objectValue = null;
        }


        public AnyValue( UnityEngine.Object value )
        {
            m_type = ValueType.Object;
            m_stringValue = null;
            m_x = m_y = m_z = m_w = 0f;
            m_objectValue = value;
        }


        public AnyValue( Vector4 value )
        {
            m_type = ValueType.Vector4;
            m_stringValue = null;
            m_x = value.x;
            m_y = value.y;
            m_z = value.z;
            m_w = value.w;
            m_objectValue = null;
        }


        public AnyValue( Color value )
        {
            m_type = ValueType.Color;
            m_stringValue = null;
            m_x = value.r;
            m_y = value.g;
            m_z = value.b;
            m_w = value.a;
            m_objectValue = null;
        }



        public static implicit operator AnyValue( int value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( string value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( float value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( bool value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( Vector2 value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( Vector3 value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( Rect value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( UnityEngine.Object value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( Vector4 value )
        {
            return new AnyValue( value );
        }


        public static implicit operator AnyValue( Color value )
        {
            return new AnyValue( value );
        }


        public static implicit operator bool( AnyValue value )
        {
            return value.Get<bool>();
        }


        public static implicit operator int( AnyValue value )
        {
            return value.Get<int>();
        }


        public static implicit operator string( AnyValue value )
        {
            return value.Get<string>();
        }


        public static implicit operator float( AnyValue value )
        {
            return value.Get<float>();
        }


        public static implicit operator Vector2( AnyValue value )
        {
            return value.Get<Vector2>();
        }


        public static implicit operator Vector3( AnyValue value )
        {
            return value.Get<Vector3>();
        }


        public static implicit operator Rect( AnyValue value )
        {
            return value.Get<Rect>();
        }


        public static implicit operator UnityEngine.Object( AnyValue value )
        {
            return value.Get<UnityEngine.Object>();
        }


        public void Set<T>( T value )
        {
            switch ( m_type )
            {
                case ValueType.Int:
                    m_x = As<int, T>( value );
                    break;
                case ValueType.String:
                    m_stringValue = As<string, T>( value );
                    break;
                case ValueType.Float:
                    m_x = As<float, T>( value );
                    break;
                case ValueType.Bool:
                    m_x = As<bool, T>( value ) ? 1f : 0f;
                    break;
                case ValueType.Vector2:
                    Vector2 v2Value = As<Vector2, T>( value );
                    m_x = v2Value.x;
                    m_y = v2Value.y;
                    break;
                case ValueType.Vector3:
                    Vector3 v3Value = As<Vector3, T>( value );
                    m_x = v3Value.x;
                    m_y = v3Value.y;
                    m_z = v3Value.z;
                    break;
                case ValueType.Rect:
                    Rect rectValue = As<Rect, T>( value );
                    m_x = rectValue.x;
                    m_y = rectValue.y;
                    m_z = rectValue.width;
                    m_w = rectValue.height;
                    break;
                case ValueType.Vector4:
                    Vector4 v4Value = As<Vector4, T>( value );
                    m_x = v4Value.x;
                    m_y = v4Value.y;
                    m_z = v4Value.z;    
                    m_w = v4Value.w;
                    break;
                case ValueType.Color:
                    Color colorValue = As<Color, T>( value );
                    m_x = colorValue.r;
                    m_y = colorValue.g;
                    m_z = colorValue.b;
                    m_w = colorValue.a;
                    break;
                case ValueType.Object:
                    m_objectValue = typeof( T ).InheritsFrom<UnityEngine.Object>() && value is UnityEngine.Object asObject ? asObject : null;
                    break;
            }
        }


        public readonly T Get<T>()
        {
            return m_type switch
            {
                ValueType.Int => As<T, int>( ( int )m_x ),
                ValueType.String => As<T, string>( m_stringValue ),
                ValueType.Float => As<T, float>( m_x ),
                ValueType.Bool => As<T, bool>( m_x != 0f ),
                ValueType.Vector2 => As<T, Vector2>( new Vector2( m_x, m_y ) ),
                ValueType.Vector3 => As<T, Vector3>( new Vector3( m_x, m_y, m_z ) ),
                ValueType.Rect => As<T, Rect>( new Rect( m_x, m_y, m_z, m_w ) ),
                ValueType.Vector4 => As<T, Vector4>( new Vector4( m_x, m_y, m_z, m_w ) ),
                ValueType.Color => As<T, Color>( new Color( m_x, m_y, m_z, m_w ) ),
                ValueType.Object => As<T, UnityEngine.Object>( m_objectValue ),
                _ => default,
            };
        }


        private readonly T As<T, U>( U value )
        {
            if ( typeof( T ) == typeof( object ) )
            {
                if ( value is object obj && obj is T objAsT )
                {
                    return objAsT;
                }
            }

            return typeof( T ).InheritsFrom<U>() && value is T casted ? casted : default;
        }


        public readonly Type GetWrappedSystemType()
        {
            return m_type switch
            {
                ValueType.Float => typeof( float ),
                ValueType.Object => typeof( UnityEngine.Object ),
                ValueType.Vector2 => typeof( Vector2 ),
                ValueType.Vector3 => typeof( Vector3 ),
                ValueType.Vector4 => typeof( Vector4 ),
                ValueType.Color => typeof( Color ),
                ValueType.Int => typeof( int ),
                ValueType.Bool => typeof( bool ),
                ValueType.Rect => typeof( Rect ),
                ValueType.String => typeof( string ),
                _ => typeof( object ),
            };
        }


        public override bool Equals( object obj )
        {
            return obj is AnyValue value && value == this;
        }


        public override int GetHashCode()
        {
            System.HashCode hash = new System.HashCode();
            hash.Add( m_type );
            hash.Add( Type );
            hash.Add( m_stringValue );
            hash.Add( m_x );
            hash.Add( m_y );
            hash.Add( m_z );
            hash.Add( m_w );
            hash.Add( m_objectValue );
            return hash.ToHashCode();
        }


        public static bool operator ==( AnyValue a, AnyValue b )
        {
            if ( a.m_type != b.m_type )
            {
                return false;
            }

            switch ( a.m_type )
            {
                case ValueType.Int:
                    return ( int )a.m_x == ( int )b.m_x;
                case ValueType.String:
                    return a.m_stringValue == b.m_stringValue;
                case ValueType.Float:
                    return a.m_x == b.m_x;
                case ValueType.Bool:
                    return ( a.m_x != 0f ) == ( b.m_x != 0 );
                case ValueType.Vector2:
                    return a.m_x == b.m_x &&
                            a.m_y == b.m_y;
                case ValueType.Vector3:
                    return a.m_x == b.m_x &&
                            a.m_y == b.m_y &&
                            a.m_z == b.m_z;
                case ValueType.Object:
                    return a.m_objectValue == b.m_objectValue;
                case ValueType.Rect:
                case ValueType.Vector4:
                case ValueType.Color:
                    return a.m_x == b.m_x &&
                            a.m_y == b.m_y &&
                            a.m_z == b.m_z &&
                            a.m_w == b.m_w;
                default:
                    return false;
            }
        }


        public static bool operator !=( AnyValue a, AnyValue b )
        {
            return !( a == b );
        }
    }
}
