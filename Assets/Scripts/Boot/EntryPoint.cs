using System.Collections;
using UnityEngine;

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
        yield break;
    }


    private void CleanUp()
    {
        s_coreSystemInitialized = false;
        CoroutineManager.CleanUp();
    }
}
