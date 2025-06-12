using System.Collections.Generic;
using UnityEngine;

namespace Framework.Scriptable
{
    public interface IRuntimeScriptabelObject
    {
        public void RuntimeReset();
    }


    public abstract class RuntimeScriptableObject : ScriptableObject, IRuntimeScriptabelObject
    {
        private static List<IRuntimeScriptabelObject> s_runtimeScriptableObjects = new List<IRuntimeScriptabelObject>();

        private void OnEnable()
        {
            Register( this );
            RuntimeReset();
        }

        private void OnDisable()
        {
            Unregister( this );
        }

        public abstract void RuntimeReset();

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        private static void ResetRuntimeScriptableObject()
        {
            for( int index = 0; index < s_runtimeScriptableObjects.Count; index++ )
            {
                IRuntimeScriptabelObject RuntimeObject = s_runtimeScriptableObjects[index];
                if ( RuntimeObject == null )
                {
                    s_runtimeScriptableObjects.RemoveAt( index );
                    index--;
                    continue;
                }

                RuntimeObject.RuntimeReset();
            }
        }


        public static void Register( IRuntimeScriptabelObject runtimeObject )
        {
            s_runtimeScriptableObjects.Add( runtimeObject );
        }


        public static void Unregister( IRuntimeScriptabelObject runtimeObject )
        { 
            s_runtimeScriptableObjects.Remove( runtimeObject );
        }
    }
}
