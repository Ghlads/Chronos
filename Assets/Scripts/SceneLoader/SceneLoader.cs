using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string m_sceneToLoad = string.Empty;

    [SerializeField] private bool m_loadOnEnable = false;


    private void OnEnable()
    {
        if ( !m_loadOnEnable )
        {
            return;
        }

        LoadScene( new LoadingParams 
        { 
            OnSceneLoadedCallback = null, 
            OnErrorCallback = null, 
            LoadingOperation = null 
        } );
    }


    public void LoadScene( LoadingParams loadingParams )
    {
        if ( string.IsNullOrEmpty( m_sceneToLoad ) )
        {
            Debug.LogWarning( "Scene loader : scene to load was null or empty" );
            return;
        }

        loadingParams.SceneName = m_sceneToLoad;
        SceneManagementUtils.LoadScene( loadingParams );
    }
}
