using System;
using Unity.Properties;
using UnityEngine;

namespace Framework.Core
{
    [CreateAssetMenu(fileName = "ActionContainer", menuName = "Scriptable/Action/Container")]
    public class ActionContainer : ScriptableObject
    {
        [SerializeReference][SerializeField] private ExecutableAction m_action;

        public ExecutableAction Action => m_action;

        [CreateProperty]
        public Command Command
        {
            get
            {
                return Command.Default;
            }
            set
            {
                Action.Execute();
            }
        }
    }


    [Serializable]
    public class ExecutableAction
    {
        public virtual void Execute() { }
    }
}
