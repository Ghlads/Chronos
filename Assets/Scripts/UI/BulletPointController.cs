using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletPointController : MonoBehaviour
{
    [SerializeField] private Image m_icon;
    [SerializeField] private TextMeshProUGUI m_titleTMP;
    [SerializeField] private TextMeshProUGUI m_contentTMP;

    public void SetData( BulletPointData data )
    {
        m_icon.sprite = data.Icon;
        m_titleTMP.text = data.Title;
        m_contentTMP.text = data.Text;
    }
}
