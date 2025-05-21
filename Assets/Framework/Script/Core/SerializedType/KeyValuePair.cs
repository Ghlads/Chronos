using System;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public struct KeyValuePair<TKey,TValue>
    {
        public TKey Key;
        public TValue Value;

        public static implicit operator KeyValuePair<TKey, TValue>( System.Collections.Generic.KeyValuePair<TKey, TValue> pair )
        {
            return new KeyValuePair<TKey, TValue> { Key = pair.Key, Value = pair.Value };
        }
    }
}
