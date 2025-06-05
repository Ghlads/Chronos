using System;
using UnityEngine;

namespace Framework.Core
{
    [Serializable]
    public class SavePrefsAction : ExecutableAction
    {
        public override void Execute()
        {
            PlayerPrefs.Save();
        }
    }
}
