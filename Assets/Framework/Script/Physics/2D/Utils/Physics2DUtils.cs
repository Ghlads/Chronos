using UnityEngine;

namespace Framework
{
    public static class Physics2DUtils
    {
        public static Interaction2DData CreateTriggerEvent( GameObject source, Collider2D collider )
        {
            return new Interaction2DData() { Source = source, OtherCollider = collider };
        }


        public static Interaction2DData CreateCollisionEvent( GameObject source, Collision2D collision )
        {
            return new Interaction2DData() { Source = source, Collision = collision };
        }


        public static bool IsTrigger( this Interaction2DData data )
        {
            return data.Collision == null;
        }


        public static bool IsCollision( this Interaction2DData data )
        {
            return !data.IsTrigger();
        }
    }
}
