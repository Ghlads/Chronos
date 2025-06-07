using Framework.Core;
using Game.Generated.Scriptable;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectData", menuName = "Scriptable/ProjectData")]
public class ProjectData : ScriptableObject
{
    [SerializeField] private Sprite m_icon;
    [SerializeField] private Sprite m_splash;
    [SerializeField] private string m_trailerLink;
    [SerializeField] private string m_title;
    [SerializeField] private bool m_needTitleDisplay;
    [SerializeField] private List<string> m_platformes;
    [Space]
    [SerializeField] private List<string> m_technologies;
    [Space]
    [SerializeField] private List<BulletPointData> m_bulletPoints;

    public Sprite Icon => m_icon;
    public Sprite Splash => m_splash;
    public string TrailerLink => m_trailerLink;
    public string Title => m_title;
    public bool NeedTitleDisplay => m_needTitleDisplay;
    public IReadOnlyList<string> Platformes => m_platformes;
    public IReadOnlyList<string> Techonologies => m_technologies;
    public IReadOnlyList<BulletPointData> BulletPoints => m_bulletPoints;

    [CreateProperty]
    public Command OpenTrailer
    {
        get => Command.Default;
        set
        {
            Application.OpenURL( TrailerLink );
        }
    }


    [CreateProperty]
    public Command PreviewProject
    {
        get => Command.Default;
        set
        {
            ProjectDataEvent @event = value.AdditionalParams[0] as ProjectDataEvent;
            @event.Raise( this );
        }
    }
}


[System.Serializable]
public struct BulletPointData
{
    public Sprite Icon;
    public string Title;
    [TextArea]
    public string Text;
}