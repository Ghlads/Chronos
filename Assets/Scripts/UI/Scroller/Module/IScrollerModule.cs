using TMPro;
using UnityEngine;

public interface IScrollerModule
{
    public struct Data
    {
        public RectTransform RectTransform;
        public TextMeshProUGUI TextMeshProUGUI;
        public float ScrollSpeed;
        public float SecondsBetweenLoops; 
    } 

    public bool RequireScroll( Data data );

    public void Scroll();

    public void ResetScrolling();
}


public class NoopScrollerModule : IScrollerModule
{
    public bool RequireScroll( IScrollerModule.Data data )
    {
        return false;
    }

    public void ResetScrolling()
    {
    }

    public void Scroll()
    {
    }
}