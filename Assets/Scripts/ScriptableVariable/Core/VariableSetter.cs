using UnityEngine;

namespace Scriptable.Variable
{
    public class VariableSetter<T, U> : MonoBehaviour where T : ScriptableVariable<U>
    {
        [SerializeField] private T m_variable;

        [SerializeField] private U m_value;

        private void Start()
        { 
            m_variable.Value = m_value;
        }
    }
}
