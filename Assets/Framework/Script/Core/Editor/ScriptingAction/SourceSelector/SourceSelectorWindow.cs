using Framework.Core.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SourceSelectorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_visualTreeAsset = default;

    private Button m_cancelButton;
    private Button m_selectButton;
    private ListView m_list;

    private List<ISourceReference> m_references;
    private Action<ISourceReference> m_onSelectCallback;

    public static void OpenSelection( List<ISourceReference> references, Action<ISourceReference> onSelecCallback )
    {
        SourceSelectorWindow window = GetWindow<SourceSelectorWindow>();
        window.titleContent = new GUIContent("SourceSelectorWindow");
        window.m_references = references;
        window.m_onSelectCallback = onSelecCallback;
        window.m_list.itemsSource = window.m_references;
        window.ShowModal();
    }


    public void CreateGUI()
    {
        VisualElement visualTree = m_visualTreeAsset.Instantiate();
        rootVisualElement.Add( visualTree );
        visualTree.style.flexGrow = 1;

        m_cancelButton = visualTree.Q<Button>( name: "cancel-button" );
        m_selectButton = visualTree.Q<Button>( name: "select-button" );
        m_list = visualTree.Q<ListView>();


        m_cancelButton.RegisterCallback<ClickEvent>( _ => Close() );
        m_selectButton.RegisterCallback<ClickEvent>( SelectHandler );
        m_list.RegisterCallback<ClickEvent>( @event =>
        {
            if ( @event.clickCount > 1 )
            {
                SelectHandler( @event );
            }
        } );

        m_list.makeItem = () => new Label();
        m_list.bindItem = ( element, index ) =>
        {
            Label label = element as Label;
            label.text = m_references[index].GetDisplayString();
        };

    }


    private void SelectHandler( ClickEvent evt )
    {
        if ( m_list.selectedIndex < 0 || m_list.selectedIndex >= m_references.Count )
        {
            return;
        }

        m_onSelectCallback?.Invoke( m_references[m_list.selectedIndex] );
        Close();
    }
}
