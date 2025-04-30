using Framework;
using UnityEngine;

namespace Game
{
    public static class MooringUtils
    {
        public static void MoorFromInteractionData( Interaction2DData data )
        {
            if ( !data.IsTrigger() )
            {
                return;
            }

            if ( !data.OtherCollider.TryGetComponent( out MoorableComponent moorable ) )
            {
                return;
            }

            if ( data.OtherCollider.TryGetComponent( out Rigidbody2D rigidbody ) )
            {
                rigidbody.linearVelocity = Vector2.zero;
            }

            Vector3 moorPointPosition = data.Source.transform.position;
            moorable.Moor( moorPointPosition );
        }
    }
}
