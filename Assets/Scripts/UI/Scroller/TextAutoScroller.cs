using TMPro;
using UnityEngine;


public enum TextAutoScrollerDirection
{
    Horizontal,
    Vertical,
}


[RequireComponent(typeof( TextMeshProUGUI ))]
[Tooltip("This component will make your text auto scroll to help display what's overflowing your text box, will do nothing if it doesn't overflow /!\\ wil do some computation on start")]
public class TextAutoScroller : MonoBehaviour
{
    [HideInInspector][SerializeField] private TextMeshProUGUI m_affectedTMP;

    [SerializeField] private TextAutoScrollerDirection m_direction;
    [SerializeField] private float m_scrollSpeed = 5.0f;
    [SerializeField] private float m_secondsBetweenLoops = 1.2f;

    private IScrollerModule m_module = null;

    private void Awake()
    {
        m_module = m_direction switch
        {
            TextAutoScrollerDirection.Vertical => new VerticalScrollerModule(),
            TextAutoScrollerDirection.Horizontal => new HorizontalScrollerModule(),
            _ => new NoopScrollerModule(),
        };
    }


    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add( OnTextChange );
        EvaluateAutoScrollNecessity();
    }


    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove( OnTextChange );
        m_module.ResetScrolling();
    }


    private void EvaluateAutoScrollNecessity()
    {
        m_module.ResetScrolling();

        if ( m_module.RequireScroll( new IScrollerModule.Data
        {
            RectTransform = transform as RectTransform,
            TextMeshProUGUI = m_affectedTMP,
            ScrollSpeed = m_scrollSpeed,
            SecondsBetweenLoops = m_secondsBetweenLoops,
        } ) )
        {
            m_module.Scroll();
        }
    }


    private void OnTextChange( Object tmpChanging )
    {
        if ( tmpChanging != m_affectedTMP )
        {
            return;
        }

        EvaluateAutoScrollNecessity();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if ( m_affectedTMP == null )
        {
            m_affectedTMP = GetComponent<TextMeshProUGUI>();
        }
    }
#endif
}
