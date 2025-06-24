using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Game
{
    [UxmlElement]
    public partial class LocalSelectorButton : VisualElement
    {
        public const string USS_CLASS = "local-selector-button";
        public const string ICON_CLASS = USS_CLASS + "__icon";
        public const string BACKGROUND_CLASS = USS_CLASS + "__background";
        public const string HOVER_CLASS = USS_CLASS + "--hover";
        public const string ACTIVE_CLASS = USS_CLASS + "--active";
        public const string CURRENT_LOCAL_CLASS = USS_CLASS + "--current-local";


        private SystemLanguage m_language = SystemLanguage.English;
        [UxmlAttribute]
        public SystemLanguage Language
        {
            get => m_language;
            set
            {
                m_language = value;
                if ( Application.isPlaying )
                {
                    LocalChangeHandler( LocalizationSettings.SelectedLocale );
                }
            }
        }


        private Sprite m_icon = null;
        [UxmlAttribute]
        public Sprite Icon
        {
            get => m_icon;
            set
            {
                m_icon = value;
                if ( m_icon != null )
                {
                    m_iconElement.style.backgroundImage = Background.FromSprite( m_icon );
                }
                else
                {
                    m_iconElement.style.backgroundImage = new Background();
                }
            }
        }


        private readonly VisualElement m_iconElement;

        public LocalSelectorButton()
        {
            VisualElement background = new();
            m_iconElement = new();
            background.Add( m_iconElement );
            Add( background );

            AddToClassList( USS_CLASS );
            background.AddToClassList( BACKGROUND_CLASS );
            m_iconElement.AddToClassList( ICON_CLASS );

            RegisterCallback<ClickEvent>( ClickHandler );
            RegisterCallback<PointerEnterEvent>( evt =>
            {
                AddToClassList( HOVER_CLASS );
            } );
            RegisterCallback<PointerLeaveEvent>( ect =>
            {
                RemoveFromClassList( HOVER_CLASS );
            } );
            RegisterCallback<PointerDownEvent>( evt =>
            {
                AddToClassList( ACTIVE_CLASS );
            } );
            RegisterCallback<PointerUpEvent>( evt =>
            {
                RemoveFromClassList( ACTIVE_CLASS );
            } );

            if ( Application.isPlaying )
            {
                LocalizationSettings.SelectedLocaleChanged += LocalChangeHandler;
                LocalChangeHandler( LocalizationSettings.SelectedLocale );
            }
        }

        
        private void LocalChangeHandler( UnityEngine.Localization.Locale locale )
        {
            if ( locale.Identifier.Code != SystemLanguageConverter.GetSystemLanguageCultureCode( Language ) )
            {
                RemoveFromClassList( CURRENT_LOCAL_CLASS );
            }
            else
            {
                AddToClassList( CURRENT_LOCAL_CLASS );
            }
        }


        private void ClickHandler( ClickEvent clickEvent )
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale( Language );
        }
    }

    // Copy of Localization package code flag internal for some reason
    static class SystemLanguageConverter
    {
        public static string GetSystemLanguageCultureCode( SystemLanguage lang )
        {
            switch ( lang )
            {
                case SystemLanguage.Afrikaans: return "af";
                case SystemLanguage.Arabic: return "ar";
                case SystemLanguage.Basque: return "eu";
                case SystemLanguage.Belarusian: return "be";
                case SystemLanguage.Bulgarian: return "bg";
                case SystemLanguage.Catalan: return "ca";
                case SystemLanguage.Chinese: return "zh-CN";
                case SystemLanguage.ChineseSimplified: return "zh-hans";
                case SystemLanguage.ChineseTraditional: return "zh-hant";
                case SystemLanguage.SerboCroatian: return "hr";
                case SystemLanguage.Czech: return "cs";
                case SystemLanguage.Danish: return "da";
                case SystemLanguage.Dutch: return "nl";
                case SystemLanguage.English: return "en";
                case SystemLanguage.Estonian: return "et";
                case SystemLanguage.Faroese: return "fo";
                case SystemLanguage.Finnish: return "fi";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Greek: return "el";
                case SystemLanguage.Hebrew: return "he";
                case SystemLanguage.Hungarian: return "hu";
                case SystemLanguage.Icelandic: return "is";
                case SystemLanguage.Indonesian: return "id";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.Japanese: return "ja";
                case SystemLanguage.Korean: return "ko";
                case SystemLanguage.Latvian: return "lv";
                case SystemLanguage.Lithuanian: return "lt";
                case SystemLanguage.Norwegian: return "no";
                case SystemLanguage.Polish: return "pl";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.Romanian: return "ro";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.Slovak: return "sk";
                case SystemLanguage.Slovenian: return "sl";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.Swedish: return "sv";
                case SystemLanguage.Thai: return "th";
                case SystemLanguage.Turkish: return "tr";
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Vietnamese: return "vi";
#if UNITY_2022_2_OR_NEWER
                case SystemLanguage.Hindi: return "hi";
#endif
                default: return "";
            }
        }
    }
}
