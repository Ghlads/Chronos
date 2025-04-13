using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class AxisScrollerModule : IScrollerModule
{
    private IScrollerModule.Data m_data;
    private float m_startPos = 0;
    private float m_scrollEndPos = 0;
    private Coroutine m_scrollingRoutine;
    private WaitForSeconds m_delayBetweenLoops = new( 1 );

    protected IScrollerModule.Data Data => m_data;

    protected abstract float GetPosition();
    protected abstract float GetVisibleSize();
    protected abstract float GetTotalSize();
    protected abstract void ApplyPosition( float newPos );


    public bool RequireScroll( IScrollerModule.Data data )
    {
        m_data = data;
        if ( m_data.RectTransform == null )
        {
            return false;
        }

        if ( m_data.TextMeshProUGUI == null || string.IsNullOrEmpty( m_data.TextMeshProUGUI.text ) )
        {
            return false;
        }

        float visibleSize = GetVisibleSize();
        float totalSize = GetTotalSize();
        float scrollingOffset = totalSize - visibleSize;
        m_startPos = GetPosition();
        m_scrollEndPos = m_startPos - scrollingOffset;
        m_delayBetweenLoops = new WaitForSeconds( m_data.SecondsBetweenLoops );
        return scrollingOffset > 0;
    }


    public void ResetScrolling()
    {
        if ( m_scrollingRoutine == null )
        {
            return;
        }

        CoroutineManager.Stop( m_scrollingRoutine );
        m_scrollingRoutine = null;

        ApplyPosition( m_startPos );
    }


    public void Scroll()
    {
        Assert.IsNull( m_scrollingRoutine );
        m_scrollingRoutine = CoroutineManager.Run( InternalScrolling() );
    }


    private IEnumerator InternalScrolling()
    {
        while ( true )
        {
            float currentPos = GetPosition();

            currentPos -= m_data.ScrollSpeed * Time.deltaTime;
            currentPos = Mathf.Max( currentPos, m_scrollEndPos );

            ApplyPosition( currentPos );
            if ( currentPos <= m_scrollEndPos )
            {
                yield return m_delayBetweenLoops;
                ApplyPosition( m_startPos );
                yield return m_delayBetweenLoops;
            }
            else
            {
                yield return null;
            }
        }
    }
}
