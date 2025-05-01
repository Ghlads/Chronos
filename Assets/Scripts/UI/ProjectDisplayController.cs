using Framework.Core;
using Game.Generated.Scriptable;
using NUnit.Framework;
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
    [SerializeField] private GameObject m_titleContainer;
    [SerializeField] private TextMeshProUGUI m_titleTMP;
    [SerializeField] private TextMeshProUGUI m_platformesTMP;
    [SerializeField] private TextMeshProUGUI m_technologiesTMP;
    [SerializeField] private Transform m_bulletPointsContainer;

    [Space]
    [SerializeField] private ProjectDataEvent m_projectDataEvent;

    private ProjectData m_projectData;

    private List<BulletPointController> m_bulletPoints = new();

    private void Start()
    {
        m_trailerButton.onClick.AddListener( OpenTrailerURL );
        m_projectDataEvent.AddListener( SetProject );
    }


    private void OnDestroy()
    {
        m_projectDataEvent.RemoveListener( SetProject );
    }


    public void SetProject( ProjectData projectData )
    {
        m_projectData = projectData;
        m_splash.sprite = m_projectData.Splash;

        m_titleContainer.SetActive( m_projectData.NeedTitleDisplay );
        m_titleTMP.text = m_projectData.Title;
        m_platformesTMP.text = StringUtils.Concat( m_projectData.Platformes, "/ " );
        m_technologiesTMP.text = StringUtils.Concat( m_projectData.Techonologies, " / " );

        for ( int index = m_bulletPoints.Count - 1; index >= 0; index-- )
        {
            Destroy( m_bulletPoints[ index ].gameObject );
            m_bulletPoints.RemoveAt( index );
        }

        Assert.IsEmpty( m_bulletPoints );
        foreach ( BulletPointData point in m_projectData.BulletPoints )
        {
            BulletPointController bulletPoint = Instantiate( m_bulletPointPrefab, m_bulletPointsContainer );
            m_bulletPoints.Add( bulletPoint );
            bulletPoint.SetData( point );
        }
    }


    private void OpenTrailerURL()
    {
        Application.OpenURL( m_projectData.TrailerLink );
    }
}
