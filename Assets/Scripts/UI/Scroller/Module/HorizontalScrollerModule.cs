using UnityEngine;

public class HorizontalScrollerModule : AxisScrollerModule
{
    protected override void ApplyPosition( float newPos )
    {
        Vector2 pos = Data.RectTransform.anchoredPosition;
        pos.x = newPos;
        Data.RectTransform.anchoredPosition = pos;
    }

    protected override float GetPosition()
    {
        return Data.RectTransform.anchoredPosition.x;
    }

    protected override float GetTotalSize()
    {
        return Data.TextMeshProUGUI.preferredWidth;
    }

    protected override float GetVisibleSize()
    {
        return Data.RectTransform.rect.width;
    }
}
