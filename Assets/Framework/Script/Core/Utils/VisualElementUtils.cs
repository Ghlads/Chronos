using UnityEngine;
using UnityEngine.UIElements;

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


        public static void RegisterUpdate( this VisualElement element, System.Action update )
        {
            if ( Application.isPlaying )
            {
                element.schedule.Execute( update ).Until( () => false );
            }
        }
    }
}
