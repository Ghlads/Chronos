using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{

    public class ScriptableClassGenerator : EditorWindow, IProgress<float>
    {
        [SerializeField] private VisualTreeAsset m_visualTreeAsset = default;
        [SerializeField] private GeneratorCache m_generatorCache;

        [MenuItem( "Tools/Generator/ScriptableClass" )]
        public static void ShowGenerator()
        {
            ScriptableClassGenerator window = GetWindow<ScriptableClassGenerator>();
            window.titleContent = new GUIContent( "ScriptableClassGenerator" );
        }

        private PropertyField m_templatesField;
        private Button m_typeSelector;
        private TextField m_namespaceField;
        private TextField m_categoryField;
        private TextField m_outputPathField;
        private Button m_generateButton;
        private ProgressBar m_progressBar;

        private Type m_selectedType;

        public void CreateGUI()
        {
            VisualElement treeAsset = m_visualTreeAsset.Instantiate();
            treeAsset.style.flexGrow = 1;
            rootVisualElement.Add( treeAsset );

            SerializedObject obj = new SerializedObject( m_generatorCache );

            // extract element
            m_templatesField = treeAsset.Q<PropertyField>();
            m_typeSelector = treeAsset.Q<VisualElement>( name: "type-selector" ).Q<Button>();
            m_namespaceField = treeAsset.Q<TextField>( name: "namespace" );
            m_categoryField = treeAsset.Q<TextField>( name: "category" );
            m_outputPathField = treeAsset.Q<TextField>( name: "ouput-path" );
            m_generateButton = treeAsset.Q<Button>( name: "generate-button" );
            m_progressBar = treeAsset.Q<ProgressBar>();

            m_templatesField.BindProperty( obj.FindProperty( "m_lastSelectedTemplates" ) );
            m_typeSelector.RegisterCallback<ClickEvent>( TypeSelectorClickHandler );
            m_namespaceField.RegisterCallback<ChangeEvent<string>>( NamespaceChangeHandler );
            m_categoryField.RegisterCallback<ChangeEvent<string>>( CategoryChangeHandler );
            m_outputPathField.RegisterCallback<ChangeEvent<string>>( OutputChangeHandler );
            m_generateButton.RegisterCallback<ClickEvent>( GenerateButtonClickHandler );


            m_namespaceField.value = m_generatorCache.LastNamespace;
            m_categoryField.value = m_generatorCache.LastCategory;
            m_outputPathField.value = m_generatorCache.LastOutputPath;
            NewTypeSelected( m_generatorCache.LastSelectedType );
        }


        private void OutputChangeHandler( ChangeEvent<string> evt )
        {
            m_generatorCache.LastOutputPath = evt.newValue;
        }


        private void CategoryChangeHandler( ChangeEvent<string> evt )
        {
            m_generatorCache.LastCategory = evt.newValue;
        }


        private void NamespaceChangeHandler( ChangeEvent<string> evt )
        {
            m_generatorCache.LastNamespace = evt.newValue;
        }


        private void TypeSelectorClickHandler( ClickEvent evt )
        {
            TypeSelectorWindow.OpenBrowser( NewTypeSelected );
        }


        private void NewTypeSelected( Type newType )
        {
            m_selectedType = newType;
            m_generatorCache.LastSelectedType = newType;
            m_typeSelector.text = m_selectedType != null ? m_selectedType.Name : "select type";
        }


        private void GenerateButtonClickHandler( ClickEvent evt )
        {
            m_progressBar.style.visibility = Visibility.Visible;
            EnableElement( false );
            ScriptableGeneratorUtils.Generate( m_generatorCache.LastSelectedTemplates, m_namespaceField.value, m_outputPathField.value, m_categoryField.value, m_selectedType, this, GenerationEndHandler );
        }


        void IProgress<float>.Report( float value )
        {
            m_progressBar.value = value;
        }


        private void GenerationEndHandler( bool success )
        {
            if ( success )
            {
                AssetDatabase.Refresh();
                Close();
                ShowGenerator();
                Debug.Log( "[Generation] success" );
            }
            else
            {
                Debug.LogError( "[Generation] failure, see causes above" );
            }

            EnableElement( true );
            m_progressBar.style.visibility = Visibility.Hidden;
        }


        private void EnableElement( bool enable )
        {
            m_templatesField.SetEnabled( enable );
            m_typeSelector.SetEnabled( enable );
            m_namespaceField.SetEnabled( enable );
            m_outputPathField.SetEnabled( enable );
            m_generateButton.SetEnabled( enable );
        }
    }
}
