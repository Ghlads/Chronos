using Framework.Scriptable.Generated;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    public class MenuStackController : MonoBehaviour
    {
        [SerializeField] private VisualElementVariable m_stackVariable;
        [Space]
        [SerializeField] private StackElement.OpenOptions m_openOptions;

        public void OpenElement( VisualTreeAsset visualTreeAsset )
        {
            if ( m_stackVariable.Value is StackElement stack )
            {
                stack.Open( visualTreeAsset, m_openOptions );
            }
        }


        public void Close()
        {
            if ( m_stackVariable.Value is StackElement stack )
            {
                stack.Close();
            }
        }
    }
}
