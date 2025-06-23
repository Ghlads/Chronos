using UnityEngine;

namespace Framework.Core
{
    public class ToggleGameObjectTween : TweenAction
    {
        [Header( "Toggle GameObject" )]
        [SerializeField] private GameObject m_target;
        [SerializeField] private float m_threshold = .5f;
        [SerializeField] private bool m_enableStateWhenGreaterEqual = true;
        [SerializeField] private bool m_enableStateWhenLess = false;

        protected override void InternalTween( float t )
        {
            m_target.SetActive( t >= m_threshold ? m_enableStateWhenGreaterEqual : m_enableStateWhenLess );
        }
    }
}
