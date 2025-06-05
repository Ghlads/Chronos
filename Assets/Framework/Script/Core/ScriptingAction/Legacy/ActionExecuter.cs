using System;
using UnityEngine;

namespace Framework.Core
{
    [Flags]
    public enum MessageType
    {
        Awake = 0,
        OnEnable = 1,
        Start = 1 << 1,
        OnUpdate = 1 << 2,
        OnDisable = 1 << 3,
    }

    public class ActionExecuter : MonoBehaviour
    {
        [SerializeField] private MessageType m_type;
        [SerializeField] private ScriptingAction m_action;


        private void Invoke( MessageType type )
        {
            if ( ( m_type & type ) == type )
            {
                m_action.Invoke();
            }
        }


        private void Awake()
        {
           Invoke( MessageType.Awake );
        }


        private void OnEnable()
        {
            Invoke( MessageType.OnEnable );
        }


        private void Start()
        {
            Invoke( MessageType.Start );
        }


        private void Update()
        {
            Invoke( MessageType.OnUpdate );
        }


        private void OnDisable()
        {
            Invoke( MessageType.OnDisable );
        }
    }
}
