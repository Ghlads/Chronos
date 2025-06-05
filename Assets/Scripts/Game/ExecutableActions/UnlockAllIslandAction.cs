using Framework.Core;
using Framework.Scriptable.Generated;
using System;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Game
{
    [Serializable]
    public class UnlockAllIslandAction : ExecutableAction
    {
        [SerializeField] private GameObjectRuntimeSet m_toVisitIsland;
        [SerializeField] private GameObjectRuntimeSet m_visitedIsland;

        public override void Execute()
        {
            foreach ( GameObject go in m_toVisitIsland )
            {
                m_visitedIsland.Add( go );
            }

            m_toVisitIsland.Clear();
        }
    }
}
