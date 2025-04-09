using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneLoadingError
{
    InvalidSceneName = 0,
    OperationAlreadyRunning = 1,
    OperationStopped = 2,
}


public enum UnloadingSceneResult
{
    Success = 0,
    NoCurrentScene = 1,
    OperationAlreadyRunning = 2,
    OperationStopped = 3,
}


public enum OperationStopResultState
{
    NoOperation = 0,
    BeforeCurrentSceneUnloaded = 1,
    NextCurrentSceneLoaded = 2,
    NoCurrentSceneLoaded = 3,
}


public struct LoadingParams
{
    public string SceneName;
    public Action OnSceneLoadedCallback;
    public Action<SceneLoadingError> OnErrorCallback;
    public Func<IEnumerator> LoadingOperation;
}


public static class SceneManagementUtils
{
    private static string s_currentScene = string.Empty;
    public const string LOADING_SCENE_NAME = "LoadingScene";

    private static Coroutine s_currentOperation = null;

    private static bool s_stopRecoverable = false;
    private static bool s_stopUnrecoverable = false;

    private static Action<OperationStopResultState> s_stopOperationResultListener = null;

    public static void LoadScene( LoadingParams loadingParams )
    {
        if ( s_currentOperation != null )
        {
            loadingParams.OnErrorCallback?.Invoke( SceneLoadingError.OperationAlreadyRunning );
            return;
        }

        s_currentOperation = CoroutineManager.Run( LoadSceneAsync( loadingParams ) );
    }


    public static void UnloadCurrentScene( Action<UnloadingSceneResult> onUnloadCurrentSceneCallback )
    {
        if ( s_currentOperation != null )
        {
            onUnloadCurrentSceneCallback?.Invoke( UnloadingSceneResult.OperationAlreadyRunning );
            return;
        }

        s_currentOperation = CoroutineManager.Run( UnloadCurrentSceneAsync( onUnloadCurrentSceneCallback ) );
    }


    public static void StopCurrentOperation( Action<OperationStopResultState> stopOperationResult )
    {
        if ( s_currentOperation == null )
        {
            stopOperationResult?.Invoke( OperationStopResultState.NoOperation );
            return;
        }

        s_stopOperationResultListener += stopOperationResult;
        s_stopRecoverable = true;
    }


    public static void StopCurrentOperationUnrecoverable()
    {
        if ( s_currentOperation == null )
        {
            return;
        }

        s_stopUnrecoverable = true;
    }


    private static void InvokeStopOperationListener( OperationStopResultState result )
    {
        s_stopOperationResultListener?.Invoke( result );
        s_stopOperationResultListener = null;
    }


    private static IEnumerator LoadSceneAsync( LoadingParams loadingParams )
    {
        Coroutine internalRoutine = CoroutineManager.Run( Internal() );

        while ( internalRoutine != null )
        {
            if ( s_stopUnrecoverable )
            {
                CoroutineManager.Stop( internalRoutine );
                internalRoutine = null;
                s_currentOperation = null;
                s_stopRecoverable = false;
                s_stopUnrecoverable = false;
                yield break;
            }

            yield return null;
        }

        s_currentOperation = null;

        IEnumerator Internal()
        {
            Scene nextScene = SceneManager.GetSceneByName( loadingParams.SceneName );
            if ( !nextScene.IsValid() )
            {
                loadingParams.OnErrorCallback?.Invoke( SceneLoadingError.InvalidSceneName );
            }

            yield return SceneManager.LoadSceneAsync( LOADING_SCENE_NAME, LoadSceneMode.Additive );
            if ( s_stopRecoverable )
            {
                yield return SceneManager.UnloadSceneAsync( LOADING_SCENE_NAME );
                loadingParams.OnErrorCallback?.Invoke( SceneLoadingError.OperationStopped );
                InvokeStopOperationListener( OperationStopResultState.BeforeCurrentSceneUnloaded );
                internalRoutine = null;
                s_stopRecoverable = false;
                yield break;
            }

            if ( loadingParams.LoadingOperation != null )
            {
                IEnumerator loadingOperation = loadingParams.LoadingOperation();
                while ( loadingOperation.MoveNext() )
                {
                    if ( s_stopRecoverable )
                    {
                        loadingOperation.Reset();
                        yield return SceneManager.UnloadSceneAsync( LOADING_SCENE_NAME );
                        loadingParams.OnErrorCallback?.Invoke( SceneLoadingError.OperationStopped );
                        InvokeStopOperationListener( OperationStopResultState.BeforeCurrentSceneUnloaded );
                        internalRoutine = null;
                        s_stopRecoverable = false;
                        yield break;
                    }

                    yield return null;
                }
            }


            if ( !string.IsNullOrEmpty( s_currentScene ) )
            {
                yield return SceneManager.UnloadSceneAsync( s_currentScene );
            }


            yield return SceneManager.LoadSceneAsync( loadingParams.SceneName, LoadSceneMode.Additive );
            s_currentScene = loadingParams.SceneName;
            yield return SceneManager.UnloadSceneAsync( LOADING_SCENE_NAME );
            loadingParams.OnSceneLoadedCallback?.Invoke();

            if ( s_stopRecoverable )
            {
                InvokeStopOperationListener( OperationStopResultState.NextCurrentSceneLoaded );
                s_stopRecoverable = false;
            }
            internalRoutine = null;
        }
    }


    private static IEnumerator UnloadCurrentSceneAsync( Action<UnloadingSceneResult> onUnloadCurrentSceneCallback )
    {
        Coroutine internalRoutine = CoroutineManager.Run( Internal() );

        while ( internalRoutine != null )
        {
            if ( s_stopUnrecoverable )
            {
                CoroutineManager.Stop( internalRoutine );
                internalRoutine = null;
                s_currentOperation = null;
                s_stopRecoverable = false;
                s_stopUnrecoverable = false;
                yield break;
            }
            yield return null;
        }
        
        s_currentOperation = null;
        IEnumerator Internal()
        {
            if ( string.IsNullOrEmpty( s_currentScene ) )
            {
                onUnloadCurrentSceneCallback?.Invoke( UnloadingSceneResult.NoCurrentScene );
                yield break;
            }

            yield return SceneManager.LoadSceneAsync( LOADING_SCENE_NAME, LoadSceneMode.Additive );
            if ( s_stopRecoverable )
            {
                yield return SceneManager.UnloadSceneAsync( LOADING_SCENE_NAME );
                onUnloadCurrentSceneCallback?.Invoke( UnloadingSceneResult.OperationStopped );
                InvokeStopOperationListener( OperationStopResultState.BeforeCurrentSceneUnloaded );
                internalRoutine = null;
                s_stopRecoverable = false;
                yield break;
            }

            yield return SceneManager.UnloadSceneAsync( s_currentScene );
            yield return SceneManager.UnloadSceneAsync( LOADING_SCENE_NAME );
            if ( s_stopRecoverable )
            {
                onUnloadCurrentSceneCallback?.Invoke( UnloadingSceneResult.OperationStopped );
                InvokeStopOperationListener( OperationStopResultState.NoCurrentSceneLoaded );
                internalRoutine = null;
                s_stopRecoverable = false;
                yield break;
            }

            internalRoutine = null;
        }
    }
}
