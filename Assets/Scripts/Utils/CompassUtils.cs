using Framework.Scriptable;
using UnityEngine;

namespace Game
{
    public static class CompassUtils
    {
        public static GameObject FindClosest( IRuntimeSet<GameObject> set, Transform from )
        {
            if ( set == null || set.Count <= 0 || from == null )
            {
                return null;
            }

            GameObject closest = null;
            float closestSqrDistance = float.MaxValue;
            foreach ( GameObject go in set )
            {
                if ( go == null )
                {
                    continue;
                }

                Vector3 distance = go.transform.position - from.position;
                if ( distance.sqrMagnitude < closestSqrDistance )
                {
                    closest = go;
                    closestSqrDistance = distance.sqrMagnitude;
                }
            }

            return closest;
        }
    }
}
