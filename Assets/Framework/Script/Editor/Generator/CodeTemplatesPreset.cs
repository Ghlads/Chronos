using System.Collections.Generic;
using UnityEngine;

namespace Framework.Editor
{
    [CreateAssetMenu( fileName = "TemplatesPreset", menuName = "Editor/Tool/Generator/TemplatesPreset" )]
    public class CodeTemplatesPreset : CodeTemplateSource
    {
        [SerializeField] private List<CodeTemplateSource> m_codeTemplateSources = new List<CodeTemplateSource>();

        public List<CodeTemplateSource> CodeTemplateSources => m_codeTemplateSources;
    }
}
