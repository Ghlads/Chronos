using Framework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scriptable
{

    public interface IRuntimeSet<T> : IList<T>, IBaseRuntimeSet
    {
        public delegate void ChangeEventSignature( T oldValue, T newValue, int index );
        public delegate void MoveElementSignature( T newElement, int index );

        public event ChangeEventSignature OnElementChanged;
        public event MoveElementSignature OnElementAdded;
        public event MoveElementSignature OnElementRemoved;
    }


    public interface IBaseRuntimeSet
    {
        public delegate void ClearSignature();
        public event ClearSignature OnCleared;
    }

    public enum RemoveMode
    {
        Classic, //will remove element at index and collapse list to fill the hole
        LimitShrinkingMovement, //will replace index element to remove with last one and remove last element
    }

    public abstract class RuntimeSet<T> : RuntimeScriptableObject, IRuntimeSet<T>
    {
        [SerializeField] private List<T> m_values = new List<T>();
        [SerializeField] private RemoveMode m_mode = RemoveMode.Classic;

        public event IRuntimeSet<T>.ChangeEventSignature OnElementChanged;
        public event IRuntimeSet<T>.MoveElementSignature OnElementAdded;
        public event IRuntimeSet<T>.MoveElementSignature OnElementRemoved;
        public event IBaseRuntimeSet.ClearSignature OnCleared;

        public T this[int index] 
        { 
            get => m_values[index]; 
            set => m_values[index] = value; 
        }


        public int Count => m_values.Count;


        public bool IsReadOnly => false;


        public void AddUnique( T item )
        {
            if ( m_values.Contains( item ) )
            {
                return;
            }

            Add( item );
        }


        public void Add( T item )
        {
            m_values.Add( item );
            OnElementAdded?.Invoke( item, m_values.Count - 1 );
        }


        public void Clear()
        {
            m_values.Clear();
            OnCleared?.Invoke();
        }


        public bool Contains( T item )
        {
            return m_values.Contains( item );
        }


        public void CopyTo( T[] array, int arrayIndex )
        {
            m_values.CopyTo( array, arrayIndex );
        }


        public IEnumerator<T> GetEnumerator()
        {
            return m_values.GetEnumerator();
        }


        public int IndexOf( T item )
        {
            return m_values.IndexOf( item );
        }


        public void Insert( int index, T item )
        {
            m_values.Insert( index, item );
            OnElementAdded?.Invoke( item, index );
        }


        public bool Remove( T item )
        {
            bool result = false;
            int index = m_values.IndexOf( item );
            if ( index != -1 )
            {
                result = true;
                RemoveAt( index );
            }

            return result;
        }


        public void RemoveItem( T item )
        {
            Remove( item );
        }


        public void RemoveAt( int index )
        {
            if ( index < 0 || index >= m_values.Count )
            {
                Debug.LogError( $"Remove index out of range [0->{m_values.Count}] : index{index}" );
                return;
            }
            T previous = m_values[index];
            switch ( m_mode )
            {
                case RemoveMode.Classic:
                    m_values.RemoveAt( index );
                    OnElementRemoved?.Invoke( previous, index );
                    while ( index <= m_values.Count )
                    {
                        OnElementChanged?.Invoke( previous, m_values[index], index );
                        previous = m_values.Count > index + 1 ? m_values[index + 1] : default;
                        index++;
                    }
                    break;
                case RemoveMode.LimitShrinkingMovement:
                    OnElementRemoved?.Invoke( previous , index );
                    m_values[index] = m_values[m_values.Count - 1];
                    m_values.RemoveAt( m_values.Count - 1 );
                    OnElementChanged( previous, m_values[index], index );
                    break;
                default:
                    Debug.LogWarning( $"Unhandled mode [{m_mode}] for RemoveAt" );
                    break;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public override void RuntimeReset()
        {
            m_values.Clear();
            OnCleared?.Invoke();// for safety but should have no listener at this point
        }
    }
}
