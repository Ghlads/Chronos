using Framework.Scriptable.Generated;
using UnityEngine;

namespace Game
{
    public class MapBorder : MonoBehaviour
    {
        [Header( "Corners" )]
        [SerializeField] private RectVariable m_bounds;
        [SerializeField] private FloatVariable m_deadSeaSize;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            if ( m_bounds == null )
            {
                return;
            }

            Rect rect = m_bounds.Default;
            Vector2 origin = new(){ x = rect.x, y = rect.y };

            Gizmos.DrawLine( origin, origin + new Vector2( rect.width, 0 ) );
            Gizmos.DrawLine( origin + new Vector2( rect.width, 0 ), origin + new Vector2( rect.width, rect.height ) );
            Gizmos.DrawLine( origin + new Vector2( rect.width, rect.height ), origin + new Vector2( 0, rect.height ) );
            Gizmos.DrawLine( origin + new Vector2( 0, rect.height ), origin );

            Gizmos.color = Color.red;
            Vector2 offset = Vector2.one * ( m_deadSeaSize?.Default ?? 0 );
            rect.min -= offset;
            rect.max += offset;
            origin -= offset;

            Gizmos.DrawLine( origin, origin + new Vector2( rect.width, 0 ) );
            Gizmos.DrawLine( origin + new Vector2( rect.width, 0 ), origin + new Vector2( rect.width, rect.height ) );
            Gizmos.DrawLine( origin + new Vector2( rect.width, rect.height ), origin + new Vector2( 0, rect.height ) );
            Gizmos.DrawLine( origin + new Vector2( 0, rect.height ), origin );
        }
#endif // UNITY_EDITOR
    }
}
