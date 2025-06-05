using System.Collections.Generic;
using UnityEditor;
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


#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else // UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
#endif // UNITY_EDITOR
        public static void ConverterRegister()
        {
            ConverterGroups.RegisterGlobalConverter( ( ref bool value ) => value ? new StyleEnum<DisplayStyle>( DisplayStyle.Flex ) : new StyleEnum<DisplayStyle>( DisplayStyle.None ) );

            ConverterGroups.RegisterConverterGroup( ListToStringSlashSeparation() );
            ConverterGroups.RegisterConverterGroup( ToBackground() );
        }


        public static ConverterGroup ListToStringSlashSeparation()
        {
            ConverterGroup listToStringWithSlashSeparatorGroup = new( "ListToStringSlashSeparated" );
            listToStringWithSlashSeparatorGroup.AddConverter( ( ref List<string> source ) => StringUtils.Concat( source, "/" ) );
            return listToStringWithSlashSeparatorGroup;
        }

        public static ConverterGroup ToBackground()
        {
            ConverterGroup toBackground = new( "ToBackground" );
            toBackground.AddConverter( ( ref Sprite source ) => Background.FromSprite( source ) );
            return toBackground;
        }
    }
}
