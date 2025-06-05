using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public class ExecutableList : ExecutableAction
    {
        [SerializeReference] private List<ExecutableAction> m_actions = new List<ExecutableAction>();

        public override void Execute()
        {
            foreach ( ExecutableAction action in m_actions )
            {
                action.Execute();
            }
        }
    }
}
