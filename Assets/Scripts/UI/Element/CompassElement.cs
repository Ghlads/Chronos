using Framework.Core;
using Framework.Scriptable.Generated;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[UxmlElement]
public partial class CompassElement : VisualElement
{
    [UxmlAttribute( "Target" )] private GameObjectVariable m_target;
    [UxmlAttribute( "Origin" )] private GameObjectVariable m_origin;
    [UxmlAttribute( "Damping" )][Range( 0, 359 )] private float m_damping;

    private VisualElement m_needle;

    public CompassElement()
    {
        if ( !Application.isPlaying )
        {
            return;
        }

        schedule.Execute( () =>
        {
            m_needle = this.Q( name: "compass-root" );
            Assert.IsNotNull( m_needle );
        } ).ExecuteLater( 10 );
        this.RegisterUpdate( Update );
    }

    private void Update()
    {
        if ( m_target.Value == null || m_origin.Value == null )
        {
            return;
        }

        if ( m_needle == null || m_needle.transform == null )
        {
            return;
        }

        Vector3 directionToTarget = ( m_target.Value.transform.position - m_origin.Value.transform.position );
        if ( directionToTarget.sqrMagnitude < .0001f )
        {
            return;
        }

        directionToTarget.Normalize();
        directionToTarget.x = -directionToTarget.x;
        float targetAngle = MathUtils.GetAngleRadBetween( directionToTarget, Vector3.up, Axis.Z ) * Mathf.Rad2Deg;
        if ( targetAngle < 0f )
        {
            targetAngle += 360f;
        }

        float currentAngle = m_needle.transform.rotation.eulerAngles.z;
        float newAngle = Mathf.LerpAngle( currentAngle, targetAngle, m_damping * Time.deltaTime );
        m_needle.transform.rotation = Quaternion.Euler( 0, 0, newAngle );
    }
}
