using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ProjectList", menuName = "Scriptable/Chronos/ProjectList")]
    public class ProjectList : ScriptableObject
    {
        [SerializeField] private List<ProjectData> m_projectDatas;
    }
}
