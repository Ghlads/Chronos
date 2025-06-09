using Framework.Core;
using Framework.Scriptable.Generated;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Game
{
    [UxmlElement]
    public partial class CompassElement : VisualElement
    {
        private GameObjectRuntimeSet m_toVisitIsland;
        [UxmlAttribute]
        public GameObjectRuntimeSet ToVisitIsland
        {
            get => m_toVisitIsland;
            set
            {
                m_toVisitIsland = value;
                if ( m_toVisitIsland != null )
                {
                    m_toVisitIsland.OnCleared += ClearedHandler;
                    m_toVisitIsland.OnElementRemoved += ElementRemovedHandler;
                }
            }
        }


        [UxmlAttribute( "Target" )] private GameObjectVariable m_target;
        [UxmlAttribute( "Origin" )] private GameObjectVariable m_origin;
        [UxmlAttribute( "Damping" )][Range( 0, 359 )] private float m_damping;
        private VisualTreeAsset m_treeAsset;
        [UxmlAttribute]
        private VisualTreeAsset TreeAsset
        {
            get => m_treeAsset;
            set
            {
                m_treeAsset = value;
                if ( m_treeAsset != null )
                {
                    Clear();
                    Add( m_treeAsset.Instantiate() );
                    m_needle = this.Q( name: "compass-needle" );
                    Assert.IsNotNull( m_needle );
                }
            }
        }


        private VisualElement m_needle;

        public CompassElement()
        {
            if ( !Application.isPlaying )
            {
                return;
            }

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


        private void ElementRemovedHandler( GameObject newElement, int index )
        {
            if ( m_toVisitIsland.Count <= 0 )
            {
                this.Hide();
            }
        }


        private void ClearedHandler()
        {
            this.Hide();
        }
    }
}
