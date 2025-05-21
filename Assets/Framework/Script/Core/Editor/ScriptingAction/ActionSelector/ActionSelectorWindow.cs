using Framework.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionSelectorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_visualTreeAsset = default;

    private Button m_cancel;
    private Button m_select;
    private TextField m_filterField;
    private ListView m_list;


    private Action<MethodInfo> m_onSelectCallback;
    private Type m_target;
    private List<MethodInfo> m_methodInfos;
    private List<MethodInfo> m_methodInfosFiltered;

    private static Dictionary<MethodInfo, string> s_methodToDisplayMap = new();

    public static void Open( Type targetType, Action<MethodInfo> onSelectedCallback )
    {
        ActionSelectorWindow window = GetWindow<ActionSelectorWindow>();
        window.titleContent = new GUIContent("ActionSelectorWindow");

        window.m_onSelectCallback = onSelectedCallback;
        window.m_target = targetType;

        window.m_methodInfos = window.m_methodInfosFiltered = GetMethodInfos( window.m_target );
        window.m_list.itemsSource = window.m_methodInfosFiltered;

        window.ShowModal();
    }


    private static List<MethodInfo> GetMethodInfos( Type targetType )
    {
        if ( targetType == null )
        {
            return GetStaticMethodInfos();
        }
        else
        {
            return GetTargetMethodInfos( targetType );
        }
    }


    private static List<MethodInfo> GetTargetMethodInfos( Type targetType )
    {
        List<MethodInfo> result = new List<MethodInfo>( targetType.GetMethods( BindingFlags.Instance | BindingFlags.Public ) );
        for ( int index = 0; index < result.Count; index++ )
        {
            if ( result[index].IsGenericMethod )
            {
                result[index] = result[result.Count - 1];
                result.RemoveAt( result.Count - 1 );
                index--;
            }
        }

        return result;
    }


    private static List<MethodInfo> s_staticList;
    private static List<MethodInfo> GetStaticMethodInfos()
    {
        if ( s_staticList != null )
        {
            return s_staticList;
        }

        s_staticList = new List<MethodInfo>( 100 );// arbitrary big number to prevent a lot of realloc 
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach ( Assembly assembly in assemblies )
        {
            if ( IsEditor( assembly ) )
            {
                continue;
            }

            foreach ( Type type in assembly.GetTypes() )
            {
                if ( !type.IsPublic || type.IsGenericType )
                {
                    continue;
                }

                foreach ( MethodInfo method in type.GetMethods( BindingFlags.Static | BindingFlags.Public ) )
                {
                    if ( method.IsGenericMethod || method.IsGenericMethodDefinition || method.ContainsGenericParameters || method.IsConstructedGenericMethod )
                    {
                        continue;
                    }

                    s_staticList.AddUnique( method );
                }
            }
        }

        return s_staticList;

        bool IsEditor( Assembly assembly )
        {
            if ( assembly.GetName().Name.ToLower().Contains( "editor" ) )
            {
                return true;
            }

            return false;
        }
    }


    public void CreateGUI()
    {
        TemplateContainer instance = m_visualTreeAsset.Instantiate();
        instance.style.flexGrow = 1;
        rootVisualElement.Add( instance );

        m_cancel = instance.Q<Button>( name: "cancel-button" );
        m_select = instance.Q<Button>( name: "select-button" );
        m_list = instance.Q<ListView>();
        m_filterField = instance.Q<TextField>();

        // Callback
        m_list.makeItem = () => new Label();
        m_list.bindItem = BindMethod;
        m_list.RegisterCallback<ClickEvent>( @event =>
        {
            if ( @event.clickCount > 1 )
            {
                SelectHandler( @event );
            }
        } );
        m_cancel.RegisterCallback<ClickEvent>( _ => Close() );
        m_select.RegisterCallback<ClickEvent>( SelectHandler );
        m_filterField.RegisterCallback<ChangeEvent<string>>( FilterChangeHandler );

        m_filterField.Focus();
    }


    private void SelectHandler( ClickEvent @event )
    {
        int index = m_list.selectedIndex;
        if ( index < 0 || index >= m_methodInfosFiltered.Count )
        {
            return;
        }

        m_onSelectCallback?.Invoke( m_methodInfosFiltered[index] );
        Close();
    }


    private void BindMethod( VisualElement element, int index )
    {
        if ( index < 0 || index >= m_methodInfosFiltered.Count )
        {
            return;
        }

        ( element as Label ).text = MethodInfoToString( m_methodInfosFiltered[index] );
    }


    private void FilterChangeHandler( ChangeEvent<string> @event )
    {
        List<MethodInfo> listToSearch = @event.newValue.Length <= @event.previousValue.Length ? m_methodInfos : m_methodInfosFiltered;
        List<MethodInfo> newFilteredList = new List<MethodInfo>(listToSearch.Count);
        foreach ( MethodInfo method in listToSearch )
        {
            if ( MethodInfoToString( method ).ToLower().Contains( @event.newValue.ToLower() ) )
            {
                newFilteredList.Add( method );
            }
        }

        m_methodInfosFiltered = newFilteredList;
        m_list.itemsSource = listToSearch;
    }


    public static string MethodInfoToString( MethodInfo method )
    {
        if ( s_methodToDisplayMap.ContainsKey( method ) )
        {
            return s_methodToDisplayMap[method];
        }

        StringBuilder builder = new StringBuilder();
        builder.Append( method.ReturnType.Beautified().GetPrettyName() )
            .Append( ' ' )
            .Append( method.DeclaringType.GetPrettyName() )
            .Append( '.' )
            .Append( method.Name );
        ParameterInfo[] parametersInfo = method.GetParameters();
        using ( new ParenthesesWrapper( builder, parametersInfo.Length > 0 && parametersInfo[0].ParameterType != typeof( NullStruct ) ) )
        {
            for ( int index = 0; index < parametersInfo.Length - 1; index++ )
            {
                Type type = parametersInfo[index].ParameterType.Beautified();
                if ( type != typeof( NullStruct ) )
                {
                    builder.Append( type.GetPrettyName() ).Append( ", " );
                }
            }

            if ( parametersInfo.Length > 0 )
            {
                Type lastType = parametersInfo[parametersInfo.Length - 1].ParameterType.Beautified();
                if ( lastType != typeof( NullStruct ) )
                {
                    builder.Append( lastType.GetPrettyName() );
                }
            }
        }

        s_methodToDisplayMap.Add( method, builder.ToString() );
        return s_methodToDisplayMap[method];
    }
}
