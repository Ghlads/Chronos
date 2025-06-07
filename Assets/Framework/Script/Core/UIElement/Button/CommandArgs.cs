using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [CreateAssetMenu(fileName = "CommandArgs", menuName = "Scriptable/Core/UI/CommandArgs")]
    public class CommandArgs : ScriptableObject
    {
        [SerializeField] private List<UnityEngine.Object> m_args;
        public List<UnityEngine.Object> Args => m_args;
    }
}
