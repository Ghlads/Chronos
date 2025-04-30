using Framework.Core;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Button ) )]
public class ButtonAction : MonoBehaviour
{
    [HideInInspector][SerializeField] private Button m_button;
    [SerializeField] private ScriptingAction m_event;

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
            m_event.Invoke();
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
