using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class EntryPoint : MonoBehaviour
{
    private static bool s_coreSystemInitialized = false;


    [SerializeField] private SceneLoader m_loader;

    private void Start()
    {
        m_loader.LoadScene( new LoadingParams { LoadingOperation = CoreSystemFence } );
    }


    private IEnumerator CoreSystemFence()
    {
        while ( !s_coreSystemInitialized )
        {
            yield return null;
        }
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Boot()
    {
        CoroutineManager.Initialize();
        CoroutineManager.Run( Initialize() );
    }


    private static IEnumerator Initialize()
    {
        s_coreSystemInitialized = true;
        SystemLanguage language = Application.systemLanguage;
        LocalizationSettings.SelectedLocale = language switch
        {
            SystemLanguage.French => LocalizationSettings.AvailableLocales.GetLocale( SystemLanguage.French ),
            _ => LocalizationSettings.AvailableLocales.GetLocale( SystemLanguage.English ),
        };

        yield break;
    }


    private void CleanUp()
    {
        s_coreSystemInitialized = false;
        CoroutineManager.CleanUp();
    }
}
