using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions.Must;
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
            window.minSize = new Vector2( window.minSize.x, 140 );
            window.maxSize = new Vector2( window.maxSize.x, 141 );
        }

        private EnumFlagsField m_classesSelectionField;
        private Button m_typeSelector;
        private TextField m_namespaceField;
        private TextField m_categoryField;
        private TextField m_outputPathField;
        private Button m_generateButton;
        private ProgressBar m_progressBar;

        private Type m_selectedType = null;

        public void CreateGUI()
        {
            VisualElement treeAsset = m_visualTreeAsset.Instantiate();
            treeAsset.style.flexGrow = 1;
            rootVisualElement.Add( treeAsset );

            // extract element
            m_classesSelectionField = treeAsset.Q<EnumFlagsField>();
            m_typeSelector = treeAsset.Q<VisualElement>( name: "type-selector" ).Q<Button>();
            m_namespaceField = treeAsset.Q<TextField>( name: "namespace" );
            m_categoryField = treeAsset.Q<TextField>( name: "category" );
            m_outputPathField = treeAsset.Q<TextField>( name: "ouput-path" );
            m_generateButton = treeAsset.Q<Button>( name: "generate-button" );
            m_progressBar = treeAsset.Q<ProgressBar>();

            m_classesSelectionField.RegisterCallback<ChangeEvent<Enum>>( ClassesSelectionChangeHandler );
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

        private void ClassesSelectionChangeHandler( ChangeEvent<Enum> evt )
        {
            ScriptableClass classes = ( ScriptableClass )evt.newValue;
            ScriptableClass classesAndDependencies = classes;
            classesAndDependencies.EnsureClassesDependencies();
            if ( classesAndDependencies != classes )
            {
                m_classesSelectionField.value = classesAndDependencies;
            }
        }


        private void TypeSelectorClickHandler( ClickEvent evt )
        {
            TypeSelectorWindow.OpenBrowser( NewTypeSelected );
        }


        private void NewTypeSelected( Type newType )
        {
            m_selectedType = newType;
            m_typeSelector.text = m_selectedType != null ? m_selectedType.Name : "select type";
            m_generatorCache.LastSelectedType = newType;
        }


        private void GenerateButtonClickHandler( ClickEvent evt )
        {
            ScriptableClass classesToGenerate = ( ScriptableClass )( m_classesSelectionField.value );
            string @namespace = m_namespaceField.value;
            string outputPath = m_outputPathField.value;
            string category = m_categoryField.value;

            m_progressBar.style.visibility = Visibility.Visible;
            EnableElement( false );
            ScriptableGeneratorUtils.Generate( classesToGenerate, @namespace, outputPath, category, m_selectedType, this, GenerationEndHandler );
        }

        void IProgress<float>.Report( float value )
        {
            m_progressBar.value = value;
        }


        private void GenerationEndHandler( bool success )
        {
            if ( success )
            {
                Close();
                AssetDatabase.Refresh();
            }
            else
            {
                EnableElement( true );
                m_progressBar.style.visibility = Visibility.Hidden;
            }
        }


        private void EnableElement( bool enable )
        {
            m_classesSelectionField.SetEnabled( enable );
            m_typeSelector.SetEnabled( enable );
            m_namespaceField.SetEnabled( enable );
            m_outputPathField.SetEnabled( enable );
            m_generateButton.SetEnabled( enable );
        }
    }
}
