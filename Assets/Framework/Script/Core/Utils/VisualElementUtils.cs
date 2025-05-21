using UnityEngine;
using UnityEngine.UIElements;
using static Codice.Client.Common.WebApi.WebApiEndpoints;

namespace Framework.Core
{
    public static class VisualElementUtils
    {
        public static void Hide( this VisualElement element )
        {
            element.style.display = DisplayStyle.None;
        }


        public static void Display( this VisualElement element )
        {
            element.style.display = DisplayStyle.Flex;
        }


        public static void SetDisplayMode( this VisualElement element, bool value )
        {
            if ( value )
            {
                element.Display();
            }
            else
            {
                element.Hide();
            }
        }


        public static void AddFoldoutLogic( this Toggle toggle, VisualElement contentContainerElement )
        {
            toggle.RegisterCallback<ChangeEvent<bool>>( ( @event ) =>
            {
                contentContainerElement.SetDisplayMode( @event.newValue );
            } );

            contentContainerElement.SetDisplayMode( toggle.value );
        }
    }
}
