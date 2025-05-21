using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalListSelector<T>
{
    public readonly struct Data
    {
        public readonly string Title;
        public readonly List<T> Source;
        public readonly Action<T> SelectCallback;
        public readonly bool HasSearchBar;
        public readonly Func<string, T, bool> Filter;
        public readonly bool AllowSelectOnItemDoubleClick;
        // view interface
        public readonly Func<VisualElement> MakeItem;
        public readonly Action<VisualElement, int, T> BindItem;
        public readonly Action<VisualElement, int, T> UnbindItem;


        public Data( 
            string title, 
            List<T> source, 
            Action<T> selectCallback, 
            bool hasSearchBar, 
            Func<string, T, bool> filter, 
            bool allowSelectOnItemDoubleClick,
            Func<VisualElement> make,
            Action<VisualElement, int, T> bind,
            Action<VisualElement, int, T> unbind )
        {
            Title = title;
            Source = source;
            SelectCallback = selectCallback;
            HasSearchBar = hasSearchBar;
            Filter = filter;
            AllowSelectOnItemDoubleClick = allowSelectOnItemDoubleClick;
            MakeItem = make;
            BindItem = bind;
            UnbindItem = unbind;
        }
    }


    public static void Open( Data data )
    {
        Window window = Window.Get();
        window.titleContent = new GUIContent( data.Title );
        CreateGUI( window, data );
        window.ShowModal();
    }


    private static List<T> s_filteredList;
    private static void CreateGUI( Window window, Data data )
    {
        VisualElement root = window.rootVisualElement;
        ToolbarSearchField searchBar = null;
        ListView view = new();
        VisualElement footer = new();
        Button cancel = new();
        cancel.text = "Cancel";
        Button select = new();
        select.text = "Select";

        if ( data.HasSearchBar )
        {
            searchBar = new ToolbarSearchField();
            searchBar.RegisterCallback<ChangeEvent<string>>( evt =>
            {
                if ( evt.newValue == evt.previousValue )
                {
                    return;
                }

                List<T> listToFilter = new ( data.Source );
                for ( int index = listToFilter.Count - 1; index >= 0; index-- )
                {
                    if ( !data.Filter( evt.newValue, listToFilter[index] ) )
                    {
                        listToFilter.RemoveAt( index );
                    }
                }

                s_filteredList = listToFilter;
                view.itemsSource = s_filteredList;
            } );

            searchBar.style.flexGrow = 0;
            searchBar.style.width = Length.Auto();

            root.Add( searchBar );
        }

        // callback
        view.makeItem = () =>
        {
            VisualElement element = data.MakeItem();
            if ( data.AllowSelectOnItemDoubleClick )
            {
                element.RegisterCallback<ClickEvent>( evt =>
                {
                    if ( evt.clickCount > 1 )
                    {
                        ConfirmSelection();
                    }
                } );
            }

            return element;
        };
        view.bindItem = ( element, index ) =>
        {
            data.BindItem( element, index, s_filteredList[index] );
        };
        view.unbindItem = ( element, index ) =>
        {
            if ( index < 0 || index >= s_filteredList.Count )
            {
                return;
            }

            data.UnbindItem( element, index, s_filteredList[index] );
        };

        cancel.clicked += window.Close;
        select.clicked += ConfirmSelection;

        s_filteredList = new List<T>( data.Source );
        view.itemsSource = s_filteredList;

        // style
        view.style.flexGrow = 1;
        footer.style.flexGrow = 0;
        footer.style.flexShrink = 0;
        footer.style.flexDirection = FlexDirection.Row;
        footer.style.justifyContent = Justify.SpaceBetween;

        // layout
        footer.Add( cancel );
        footer.Add( select );
        root.Add( view );
        root.Add( footer );

        if ( data.HasSearchBar )
        {
            searchBar.Focus();
        }

        void ConfirmSelection()
        {
            int index = view.selectedIndex;
            if ( index < 0 || index >= s_filteredList.Count )
            {
                return;
            }

            window.Close();
            data.SelectCallback( s_filteredList[index] );
        }
    }


    
}


public class ModalListBuilder<T>
{
    private readonly List<T> m_list;
    private readonly Action<T> m_selectCallback;
    private readonly Func<VisualElement> m_make;
    private readonly Action<VisualElement, int, T> m_bind;

    private Action<VisualElement, int, T> m_unbind;
    private bool m_hasSearchBar;
    private Func<string, T, bool> m_filter;
    private string m_title;
    private bool m_allowSelectOnDoubleClick;

    public ModalListBuilder( List<T> list, Action<T> selectCallback, Func<VisualElement> make, Action<VisualElement, int, T> bind )
    {
        m_list = list;
        m_selectCallback = selectCallback;
        m_make = make;
        m_unbind = ( _, __, ___ ) => {};
        m_bind = bind;
        m_hasSearchBar = false;
        m_filter = null;
        m_title = string.Empty;
        m_allowSelectOnDoubleClick = false;
    }


    public ModalListBuilder<T> WithUnbind( Action<VisualElement, int, T> unbind )
    {
        m_unbind = unbind;
        return this;
    }


    public ModalListBuilder<T> WithTitle( string title )
    {
        m_title = title;
        return this;
    }


    public ModalListBuilder<T> WithSearchBar( Func<string, T, bool> filter )
    {
        m_hasSearchBar = true;
        m_filter = filter;
        return this;
    }


    public ModalListBuilder<T> WithSelectOnDoubleClick()
    {
        m_allowSelectOnDoubleClick = true;
        return this;
    }


    public void Open()
    {
        ModalListSelector<T>.Data data = new(
            m_title,
            m_list,
            m_selectCallback,
            m_hasSearchBar,
            m_filter,
            m_allowSelectOnDoubleClick,
            m_make,
            m_bind,
            m_unbind
            );

        ModalListSelector<T>.Open( data );
    }
}


public class Window : EditorWindow
{
    public static Window Get()
    {
        return GetWindow<Window>();
    }
}