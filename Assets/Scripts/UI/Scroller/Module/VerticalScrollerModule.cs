
using UnityEngine;

public class VerticalScrollerModule : AxisScrollerModule
{
    protected override void ApplyPosition( float newPos )
    {
        Vector2 pos = Data.RectTransform.anchoredPosition;
        pos.y = newPos;
        Data.RectTransform.anchoredPosition = pos;
    }

    protected override float GetPosition()
    {
        return Data.RectTransform.anchoredPosition.y;
    }

    protected override float GetTotalSize()
    {
        return Data.TextMeshProUGUI.preferredHeight;
    }

    protected override float GetVisibleSize()
    {
        return Data.RectTransform.rect.height;
    }
}
