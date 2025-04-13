using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDisplayController : MonoBehaviour
{
    [SerializeField] private BulletPointController m_bulletPointPrefab;
    [Space]
    [SerializeField] private Button m_backButton;
    [Space]
    [Header( "Details Container" )]
    [SerializeField] private Image m_splash;
    [SerializeField] private Button m_trailerButton;
    [SerializeField] private TextMeshProUGUI m_titleTMP;
    [SerializeField] private TextMeshProUGUI m_platformesTMP;
    [SerializeField] private TextMeshProUGUI m_technologiesTMP;
    [SerializeField] private Transform m_bulletPointsContainer;

    [Space]
    [SerializeField] private ProjectDataScriptableEvent m_projectDataEvent;

    private ProjectData m_projectData;

    private List<BulletPointController> m_bulletPoints = new();

    private void Start()
    {
        m_trailerButton.onClick.AddListener( OpenTrailerURL );
    }


    private void OnEnable()
    {
        m_projectDataEvent.AddListener( SetProject );
    }


    private void OnDisable()
    {
        m_projectDataEvent.RemoveListener( SetProject );
    }


    public void SetProject( ProjectData projectData )
    {
        m_projectData = projectData;
        m_splash.sprite = m_projectData.Splash;
        m_titleTMP.text = $"Title : {m_projectData.Title}";
        m_platformesTMP.text = $"Platformes : {StringUtils.Concat( m_projectData.Platformes, "/ " )}";
        m_technologiesTMP.text = $"Technologies : {StringUtils.Concat( m_projectData.Techonologies, " / " )}";

        foreach ( BulletPointController bulletPoint in m_bulletPoints )
        {
            Destroy( bulletPoint.gameObject );
        }

        foreach ( BulletPointData point in m_projectData.BulletPoints )
        {
            BulletPointController bulletPoint = Instantiate( m_bulletPointPrefab, m_bulletPointsContainer );
            bulletPoint.SetData( point );
        }
    }


    private void OpenTrailerURL()
    {
        Application.OpenURL( m_projectData.TrailerLink );
    }
}
