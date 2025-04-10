using Chronos.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using static Chronos.Input.InputControls;

[CreateAssetMenu( fileName = "InputSource", menuName = "Scriptable/Input/Source" )]
public class InputSource : Scriptable.RuntimeScriptableObject, IMovementActions
{
    public bool IsPointerDown => m_inputActions.Movement.PointerDown.IsPressed();

    public Vector2 PointerPosition => m_inputActions.Movement.PointerPosition.ReadValue<Vector2>();


    private InputControls m_inputActions;


    public void Enable()
    {
        m_inputActions.Movement.SetCallbacks( this );
        m_inputActions.Enable();
    }


    public void Disable()
    {
        m_inputActions.Disable();
        m_inputActions.Movement.RemoveCallbacks( this );
    }

    
    public override void RuntimeReset()
    {
        m_inputActions = new InputControls();
    }


    public void OnPointerDown( InputAction.CallbackContext context ) {}
    public void OnPointerPosition( InputAction.CallbackContext context ) {}
}
