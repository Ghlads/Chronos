using UnityEngine;
using UnityEngine.UI;
using Scriptable.Event;

[RequireComponent( typeof( Button ) )]
public class ButtonAction : MonoBehaviour
{
    [HideInInspector][SerializeField] private Button m_button;
    [SerializeField] private ScriptableEvent m_event;

    private void OnEnable()
    {
        m_button.onClick.AddListener( OnClickHandler );
    }


    private void OnDisable()
    {
        m_button.onClick.RemoveListener( OnClickHandler );
    }


    public void OnClickHandler()
    {
        if ( m_event != null )
        {
            m_event.Raise();
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if ( m_button == null )
        {
            m_button = GetComponent<Button>();
        }
    }
#endif // UNITY_EDITOR
}
