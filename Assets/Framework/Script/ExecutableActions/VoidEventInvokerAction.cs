using Framework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
    public class VoidEventInvokerAction : ExecutableAction
    {
        [SerializeField] private List<Framework.Scriptable.ScriptableEvent> m_events;

        public override void Execute()
        {
            if ( m_events != null )
            {
                foreach ( Scriptable.ScriptableEvent @event in m_events )
                {
                    @event.Raise();
                }
            }
        }
    }
}
