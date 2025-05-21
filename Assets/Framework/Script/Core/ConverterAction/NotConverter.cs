using UnityEngine;
using UnityEngine.Events;

namespace Framework.Core
{
    public class NotConverter : MonoBehaviour
    {
        [SerializeField] private UnityEvent<bool> m_onConvert;

        public void Raise( bool value )
        {
            m_onConvert.Invoke( !value );
        }
    }
}
