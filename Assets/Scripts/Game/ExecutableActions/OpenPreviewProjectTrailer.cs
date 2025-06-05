using Framework.Core;
using Game.Generated.Scriptable;
using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class OpenPreviewProjectTrailer : ExecutableAction
    {
        [SerializeField] private ProjectDataVariable m_previewedProject;

        public override void Execute()
        {
            if ( m_previewedProject == null )
            {
                return;
            }

            if ( m_previewedProject.Value != null )
            {
                Application.OpenURL( m_previewedProject.Value.TrailerLink );
            }
        }
    }
}
