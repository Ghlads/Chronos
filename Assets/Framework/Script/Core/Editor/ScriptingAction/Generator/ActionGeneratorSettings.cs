using Mono.Cecil;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Core.Editor
{
    [CreateAssetMenu(fileName = "ActionGeneratorSettings", menuName = "Editor/Tools/Action/Settings")]
    public class ActionGeneratorSettings : ScriptableObject
    {
        [SerializeField] private string m_generatedFilePath = "Generated/";
        [SerializeField] private string m_namespace = "Generated";
        [SerializeField] private AssemblyDefinitionAsset m_assemblyDefinition = null;
        [SerializeField][HideInInspector] private string jsonCache = string.Empty;


        public string GeneratedFilePath => m_generatedFilePath;
        public string Namespace => m_namespace;
        public AssemblyDefinitionAsset AssemblyDefinition => m_assemblyDefinition;
        public string JsonCache
        {
            get => jsonCache;
            set => jsonCache = value;
        }
    }
}
