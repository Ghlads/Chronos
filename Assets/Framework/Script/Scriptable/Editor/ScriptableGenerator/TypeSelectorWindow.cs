using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework.Scriptable.Editor
{
    public class TypeSelectorWindow : EditorWindow
    {
        private Action<Type> m_selectedTypeCallback;
        private TextField m_typeNameField;
        private ListView m_listView;
        private Button m_selectButton;
        private Button m_cancelButton;

        private List<Type> m_matchingTypes = new();

        public static void OpenBrowser( Action<Type> selectedTypeCallback )
        {
            TypeSelectorWindow window = GetWindow<TypeSelectorWindow>();
            window.titleContent = new GUIContent( "Type Selector" );
            window.m_selectedTypeCallback = selectedTypeCallback;
            window.ShowModalUtility();
        }


        public void CreateGUI()
        {
            m_typeNameField = new TextField();
            m_typeNameField.label = string.Empty;
            m_typeNameField.RegisterCallback<ChangeEvent<string>>( FilterChangeHandler );

            rootVisualElement.Add( m_typeNameField );

            m_listView = new ListView();
            m_listView.makeItem = MakeTypeEntry;
            m_listView.bindItem = BindTypeEntry;
            m_listView.itemsSource = m_matchingTypes;
            m_listView.selectionChanged += SelectionChangeHandler;

            rootVisualElement.Add( m_listView );

            m_selectButton = new Button();
            m_selectButton.text = "Select";
            m_selectButton.SetEnabled( false );
            m_selectButton.RegisterCallback<ClickEvent>( SelectClickHandler );

            m_cancelButton = new Button();
            m_cancelButton.text = "Cancel";
            m_cancelButton.RegisterCallback<ClickEvent>( CancelClickHandler );

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.justifyContent = Justify.SpaceBetween;
            container.Add( m_cancelButton );
            container.Add( m_selectButton );

            rootVisualElement.Add( container );
        }


        private void SelectionChangeHandler( IEnumerable<object> enumerable )
        {
            m_selectButton.SetEnabled( true );
        }


        private void CancelClickHandler( ClickEvent evt )
        {
            Close();
        }


        private void SelectClickHandler( ClickEvent evt )
        {
            if ( m_listView.selectedItem == null )
            {
                return;
            }

            m_selectedTypeCallback?.Invoke( ( Type )m_listView.selectedItem );
            Close();
        }


        private void BindTypeEntry( VisualElement element, int index )
        {
            Label label = element as Label;
            label.text = m_matchingTypes[index].GetSafeName();
        }


        private VisualElement MakeTypeEntry()
        {
            return new Label();
        }


        private void FilterChangeHandler( ChangeEvent<string> evt )
        {
            m_matchingTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany( x => x.GetTypes() )
                            .Where( type => IsTypeEligible( type, evt.newValue ) )
                            .Distinct()
                            .Take( 30 )
                            .ToList();

            m_selectButton.SetEnabled( false );
            m_listView.itemsSource = m_matchingTypes;
            m_listView.Rebuild();
        }


        private bool IsTypeEligible( Type type, string filter )
        {
            if ( !type.IsPublic )
            {
                return false;   
            }

            if ( type.IsAbstract )
            {
                return false;
            }

            if ( type.IsInterface )
            {
                return false;
            }

            if ( type.IsGenericType )
            {
                return false;
            }

            if ( !( type.IsPrimitive || 
                type.Attributes.HasFlag( System.Reflection.TypeAttributes.Serializable ) || 
                type.InheritsFrom<UnityEngine.Object>() ) )
            {
                return false;
            }

            return type.Name.ToLowerInvariant().Contains( filter );
        }
    }
}
