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

            moorable.Moor( data.Source.transform );
        }
    }
}
