using Framework.Core;
using Game.Generated.Scriptable;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectData", menuName = "Scriptable/ProjectData")]
public class ProjectData : ScriptableObject
{
    [SerializeField][CreateProperty] private Sprite m_icon;
    [SerializeField][CreateProperty] private Sprite m_splash;
    [SerializeField][CreateProperty] private string m_trailerLink;
    [SerializeField][CreateProperty] private string m_title;
    [SerializeField][CreateProperty] private bool m_needTitleDisplay;
    [Space]
    [SerializeField][CreateProperty] private List<BulletPointData> m_bulletPoints;

    public Sprite Icon => m_icon;
    public Sprite Splash => m_splash;
    public string TrailerLink => m_trailerLink;
    public string Title => m_title;
    public bool NeedTitleDisplay => m_needTitleDisplay;
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