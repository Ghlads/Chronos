using Framework.Core;
using Framework.Scriptable.Generated;
using UnityEngine;

namespace Game
{
    public class MapBorder : MonoBehaviour
    {
        [Header( "Corners" )]
        [SerializeField] private RectVariable m_bounds;

        [Header( "DynamicObject" )]
        [SerializeField] private GameObjectRuntimeSet m_nonPhysicObjects;
        [SerializeField] private Rigidbody2DRuntimeSet m_physicObjects;


        private void FixedUpdate()
        {
            foreach ( GameObject go in m_nonPhysicObjects )
            {
                if ( !m_bounds.Value.Contains( go.transform.position ) )
                {
                    Vector3 position = go.transform.position;
                    MathUtils.ConstraintPositionInRectWarpped( ref position, m_bounds.Value );
                    go.transform.position = position;
                }
            }

            foreach ( Rigidbody2D rb in m_physicObjects )
            {
                if ( !m_bounds.Value.Contains( rb.position ) )
                {
                    Vector2 position = rb.position;
                    MathUtils.ConstraintPositionInRectWarpped( ref position, m_bounds.Value );
                    rb.position = position;
                }
            }
        }


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
        }
#endif // UNITY_EDITOR
    }
}
