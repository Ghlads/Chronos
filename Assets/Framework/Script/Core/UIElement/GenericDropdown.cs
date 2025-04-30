using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace Framework.Core
{
    public class GenericDropdown<T> : VisualElement
    {
        public const string ELEMENT_CLASS = "generic-dropdown";
        public const string LIST_VIEW_CLASS = ELEMENT_CLASS + "__list-view";
        public const string LIST_ELEMENT_CLASS = LIST_VIEW_CLASS + "__element";

        public const string ODD_CLASS = "odd";
        public const string EVEN_CLASS = "even";

        //entry
        private readonly ListView m_internalListView;
        private readonly List<T> m_itemList;

        //callback
        private readonly Func<VisualElement> m_makeEntry;
        private readonly Action<T, VisualElement> m_bindEntry;
        private readonly Action<T, VisualElement> m_unbindEntry;

        public event Action<T> OnSelectedChanged;
        public T SelectedValue => m_itemList[m_internalListView.selectedIndex];
        public IReadOnlyList<T> Items => m_itemList;

        public GenericDropdown( List<T> itemList, Func<VisualElement> makeEntry, Action<T, VisualElement> bindEntry, Action<T, VisualElement> unbindEntry )
        {
#if UNITY_EDITOR
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>( "Assets/Framework/Script/Core/Editor/ScriptingAction/USS/ScriptingActionSheet.uss" );
            styleSheets.Add( styleSheet );    
#endif // UNITY_EDITOR

            AddToClassList( ELEMENT_CLASS );
            m_itemList = itemList;
            m_makeEntry = makeEntry;
            m_bindEntry = bindEntry;
            m_unbindEntry = unbindEntry;
            m_internalListView = new ListView(
                m_itemList,
                makeItem: Make,
                bindItem: Bind
                );

            m_internalListView.unbindItem = Unbind;
            m_internalListView.showAddRemoveFooter = false;
            m_internalListView.selectionType = SelectionType.Single;
            m_internalListView.selectedIndicesChanged += SelectedIndicesChangeHandler;
            m_internalListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            m_internalListView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            m_internalListView.AddToClassList( LIST_VIEW_CLASS );
            Add( m_internalListView );
        }


        private void SelectedIndicesChangeHandler( IEnumerable<int> enumerable )
        {
            IEnumerator<int> enumerator = enumerable.GetEnumerator();
            if ( enumerator.MoveNext() )
            {
                OnSelectedChanged?.Invoke( m_itemList[enumerator.Current] );
            }
        }


        public void SetSelected( T newSelected )
        {
            for( int index = 0; index < m_itemList.Count; index++ )
            {
                if ( EqualityComparer<T>.Default.Equals( m_itemList[index], newSelected ) )
                {
                    m_internalListView.selectedIndex = index;
                    return;
                }
            }

            Debug.LogWarning( "[SetSelected] : newSelected isn't in dropdown content list" );
        }


        public void SetSelected( int index )
        {
            if ( index >= 0 && index < m_itemList.Count )
            {
                m_internalListView.selectedIndex = index;
                return;
            }

            Debug.LogWarning( $"[SetSelected] : index {index} isn't in range [0 -> {m_itemList.Count}]" );
        }


        private void Bind( VisualElement element, int index )
        {
            element.AddToClassList( index % 2 == 0 ? EVEN_CLASS : ODD_CLASS );
            m_bindEntry( m_itemList[index], element );
        }


        private void Unbind( VisualElement element, int index )
        {
            element.RemoveFromClassList( index % 2 == 0 ? EVEN_CLASS : ODD_CLASS );
            m_unbindEntry( m_itemList[index], element );
        }


        public VisualElement Make()
        {
            VisualElement element = m_makeEntry();
            element.AddToClassList( LIST_ELEMENT_CLASS );
            return element;
        }

        private const float SAFE_AREA_PIXEL = 55f;
        public void Show( VisualElement caller )
        {
            if ( style.display == DisplayStyle.Flex )
            {
                Debug.LogWarning( "Dropdown already displayed, can't show twice check your logic" );
                return;
            } 

            caller.panel.visualTree.Add( this );
            style.display = DisplayStyle.Flex;
            style.width = caller.resolvedStyle.width;
            Rect worldRect = caller.worldBound;
            style.left = worldRect.xMin;
            style.position = Position.Absolute;

            schedule.Execute( () =>
            {
                float dropdownHeight = resolvedStyle.height;
                float screenHeight = caller.panel.visualTree.worldBound.height;

                bool fitsBelow = worldRect.yMax + dropdownHeight + SAFE_AREA_PIXEL <= screenHeight;

                if ( fitsBelow )
                {
                    style.top = worldRect.yMax;
                }
                else
                {
                    style.top = worldRect.yMin - dropdownHeight;
                }

                VisualElement visualTree = caller.panel.visualTree;
                visualTree.RegisterCallback<PointerDownEvent>( PointerDownHandler, TrickleDown.TrickleDown );
                void PointerDownHandler( PointerDownEvent @event )
                {
                    if ( style.display == DisplayStyle.None )
                    {
                        visualTree.UnregisterCallback<PointerDownEvent>( PointerDownHandler, TrickleDown.TrickleDown );
                        // already hidden
                        return;
                    }

                    if ( !( this.worldBound.Contains( @event.position ) || worldRect.Contains( @event.position ) ) )
                    {
                        visualTree.UnregisterCallback<PointerDownEvent>( PointerDownHandler, TrickleDown.TrickleDown );
                        @event.StopPropagation();
                        Hide( caller );
                    }
                }


#if UNITY_EDITOR
                EditorWindow.windowFocusChanged += FocusChangeHandler;
                void FocusChangeHandler()
                {
                    EditorWindow.windowFocusChanged -= FocusChangeHandler;
                    if ( style.display == DisplayStyle.None )
                    {
                        // already hidden
                        return;
                    }

                    Hide( caller );
                }
                ;
#endif // UNITY_EDITOR
            } ).ExecuteLater( 1 );
        }


        public void Hide( VisualElement caller )
        {
            caller.Add( this );
            style.display = DisplayStyle.None;
        }
    }
}
