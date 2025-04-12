using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public static class CoroutineManager
{
    private class InternalCoroutineRunner : MonoBehaviour {}

    private static InternalCoroutineRunner s_instance = null;



    public static Coroutine Run( IEnumerator routine )
    {
        Assert.IsNotNull( s_instance, "CoroutineManager not initialized" );
        return s_instance.StartCoroutine( routine );
    }


    public static void Stop( Coroutine routine )
    {
        if ( routine == null )
        {
            return;
        }

        if ( s_instance == null )
        {
            return;
        }

        s_instance.StopCoroutine( routine );
    }


    public static void Initialize()
    {
        Assert.IsNull( s_instance, "CoroutineManager already initialize");
        GameObject go = new GameObject( "CoroutineManager" );
        s_instance = go.AddComponent<InternalCoroutineRunner>();
        Object.DontDestroyOnLoad( go );
    }


    public static void CleanUp()
    {
        Assert.IsNotNull( s_instance, "CoroutineManager already cleaned up" );
        Object.Destroy( s_instance.gameObject );
    }
}
