using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework.Core
{
    public static class ActyxRegistry
    {
        private static Dictionary<Hash128, Action<object[]>> s_actionMap = new();

#if !UNITY_EDITOR // safety only for build to help catching error in editor since will have null delegate log error
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterDefaultBehaviour() 
        {
            s_actionMap[default] = args => { };// Safety to prevent null invocation error
        }
#endif // UNITY_EDITOR


        public static Action<object[]> Get( Hash128 id )
        {
            return s_actionMap[id];
        }


        public static void Register( Hash128 id, Action<object[]> action )
        {
            Assert.IsTrue( !s_actionMap.ContainsKey( id ), $"Collision detected for id [{id}]" );
            s_actionMap[id] = action;
        }
    }


    [Serializable]
    public class ID : ISerializationCallbackReceiver
    {
        [SerializeField] private string m_guid;

        public Hash128 Hash { get; private set; }


        public ID() : this( string.Empty )
        {
        }


        public ID ( string guid )
        {
            m_guid = guid;
            OnAfterDeserialize ();
        }


        public void OnAfterDeserialize()
        {
            if ( string.IsNullOrEmpty( m_guid ) )
            {
                m_guid = Guid.NewGuid().ToString();
            }

            Hash = Hash128.Compute( m_guid );
        }


        public void OnBeforeSerialize() {}


        public override string ToString()
        {
            return m_guid;
        }


        public override bool Equals( object obj )
        {
            return obj is ID id &&
                id.m_guid == m_guid;
        }


        public override int GetHashCode()
        {
            return m_guid.GetHashCode();
        }


        public static bool operator ==( ID lhs, ID rhs )
        {
            if ( lhs is null )
            {
                return rhs is null;
            }
            else if ( rhs is null )
            {
                return lhs is null;
            }

            return lhs.Hash == rhs.Hash;
        }


        public static bool operator !=( ID lhs, ID rhs )
        {
            return !( lhs == rhs );
        }

        
        public static implicit operator Hash128( ID id )
        {
            return id.Hash;
        }
    }


    [Serializable]
    public struct ModifierArgs 
    {
        public List<AnyValue> Args;
    }


    [Serializable]
    public class Actyx<T0, T1, T2, T3> : ISerializationCallbackReceiver
    {


        [SerializeField] private ID m_id;
        [SerializeField] private List<ModifierArgs> m_constantArguments = new();
        [NonSerialized] private Action<object[]> m_delegate;
        [NonSerialized] private object[] m_args;
        [NonSerialized] private bool m_isDirty = false;

        public void Invoke( T0 arg0, T1 arg1, T2 arg2, T3 arg3 )
        {
            if ( m_isDirty || m_args == null || m_args.Length < 4 || m_delegate == null )
            {
                m_delegate = ActyxRegistry.Get( m_id );
                m_args = new object[m_constantArguments.Count + 4];
                for ( int index = 0; index < m_constantArguments.Count; index++ )
                {
                    m_args[index + 4] = m_constantArguments[index];
                }
            }

            m_args[0] = arg0;
            m_args[1] = arg1;
            m_args[2] = arg2;
            m_args[3] = arg3;
            m_delegate.Invoke( m_args );
        }


        public void SetDirty()
        {
            m_isDirty = true;
        }


        public void OnAfterDeserialize()
        {
            m_isDirty = true;
        }

        public void OnBeforeSerialize() {}
    }


    [Serializable]
    public class Actyx<T0, T1, T2> : Actyx<T0, T1, T2, Nullable<byte>>
    {
        public void Invoke( T0 arg0, T1 arg1, T2 arg2 )
        {
            Invoke( arg0, arg1, arg2, null );
        }
    }


    [Serializable]
    public class Actyx<T0, T1> : Actyx<T0, T1, Nullable<byte>>
    {
        public void Invoke( T0 arg0, T1 arg1 )
        {
            Invoke( arg0, arg1, null, null );
        }
    }


    [Serializable]
    public class Actyx<T> : Actyx<T, Nullable<byte>>
    {
        public void Invoke( T arg )
        {
            Invoke( arg, null, null, null );
        }
    }


    [Serializable]
    public class Actyx : Actyx<Nullable<byte>>
    {
        public void Invoke()
        {
            Invoke( null, null, null, null );
        }

        public static void NoopAction() {}
    }
}

