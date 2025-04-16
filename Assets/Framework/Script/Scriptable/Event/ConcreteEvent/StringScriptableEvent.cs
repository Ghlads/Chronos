using UnityEngine;

namespace Framework.Scriptable
{
    [CreateAssetMenu( fileName = "StringEvent", menuName = "Scriptable/Event/Primitive/String" )]
    public class StringEvent : ScriptableEvent<string> { }
}
