using Framework.Core;
using Framework.Scriptable.Generated;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    [Serializable]
    public class OpenMenuAction : ExecutableAction
    {
        [SerializeField] private VisualElementVariable m_stackElement;
        [SerializeField] private VisualTreeAsset m_elementToOpen;
        [Space]
        [SerializeField] private StackElement.OpenOptions m_openOptions;

        public override void Execute()
        {
            StackElement stack = m_stackElement.Value as StackElement;
            stack.Open( m_elementToOpen, m_openOptions );
        }
    }
}
