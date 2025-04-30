using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [System.Serializable]
    public class InterfaceReference<T, U> where T : class where U : UnityEngine.Object
    {
        [SerializeField] private U m_rawObject;

        public U GetRaw()
        {
            return m_rawObject;
        }


        public T Get()
        {
            return m_rawObject as T;
        }


        public override bool Equals( object obj )
        {
            return obj is InterfaceReference<T, U> reference &&
                    EqualityComparer<U>.Default.Equals( m_rawObject, reference.m_rawObject );
        }


        public override int GetHashCode()
        {
            return HashCode.Combine( m_rawObject );
        }


        public static implicit operator T( InterfaceReference<T,U> reference )
        {
            return reference.Get();
        }


        public static bool operator ==( InterfaceReference<T, U> lhs, InterfaceReference<T, U> rhs )
        {
            if ( lhs is null || lhs.Get() == null )
            {
                if ( rhs is null || rhs.Get() == null )
                {
                    return true;
                }

                return false;
            }

            if ( rhs is null || rhs.Get() == null )
            {
                if ( lhs is null || lhs.Get() == null )
                {
                    return true;
                }

                return false;
            }

            return lhs.Get() == rhs.Get();
        }


        public static bool operator !=( InterfaceReference<T, U> lhs, InterfaceReference<T, U> rhs )
        {
            return !( lhs == rhs );
        }
    }


    [System.Serializable]
    public class InterfaceReference<T> : InterfaceReference<T, UnityEngine.Object> where T : class {}
}
