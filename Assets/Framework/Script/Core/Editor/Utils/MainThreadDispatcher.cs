using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Core.Editor
{
    [InitializeOnLoad]
    public static class MainThreadDispather
    {
        private static readonly Queue<Action> s_actions = new Queue<Action>();

        static MainThreadDispather()
        {
            EditorApplication.update += Update;
        }


        private static void Update()
        {
            lock ( s_actions )
            {
                foreach ( Action action in s_actions )
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch ( Exception e )
                    {
                        Debug.LogException( e );
                    }
                }

                s_actions.Clear();
            }
        }


        public static void Execute( Action action )
        {
            lock ( s_actions )
            {
                s_actions.Enqueue( action );
            }
        }
    }
}
