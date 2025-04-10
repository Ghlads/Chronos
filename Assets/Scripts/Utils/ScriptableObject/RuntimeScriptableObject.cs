using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    public abstract class RuntimeScriptableObject : ScriptableObject
    {
        private static List<RuntimeScriptableObject> s_runtimeScriptableObjects = new List<RuntimeScriptableObject>();

        private void OnEnable()
        {
            s_runtimeScriptableObjects.Add(this);
        }

        private void OnDisable()
        {
            s_runtimeScriptableObjects.Remove(this);
        }

        public abstract void RuntimeReset();

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        private static void ResetRuntimeScriptableObject()
        {
            for( int index = 0; index < s_runtimeScriptableObjects.Count; index++ )
            {
                RuntimeScriptableObject RuntimeObject = s_runtimeScriptableObjects[index];
                if ( RuntimeObject == null )
                {
                    s_runtimeScriptableObjects.RemoveAt( index );
                    index--;
                    continue;
                }

                RuntimeObject.RuntimeReset();
            }
        }
    }
}
