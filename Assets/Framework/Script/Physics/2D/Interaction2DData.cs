using UnityEngine;

namespace Framework
{
    [System.Serializable]
    public struct Interaction2DData
    {
        public GameObject Source;
        public Collider2D OtherCollider;
        public Collision2D Collision;
    }
}

