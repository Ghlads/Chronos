using UnityEngine;

namespace Framework.Scriptable
{
    public class VariableSetter<T, U> : MonoBehaviour where T : ScriptableVariable<U>
    {
        [SerializeField] private T m_variable;

        [SerializeField] private U m_value;

        private void OnEnable()
        { 
            m_variable.Value = m_value;
        }
    }
}
