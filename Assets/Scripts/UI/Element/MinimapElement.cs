using Framework.Core;
using Framework.Scriptable.Generated;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    [UxmlElement]
    public partial class MinimapElement : VisualElement
    {
        [UxmlAttribute( "MapBounds" )] private RectVariable m_mapBoundsVariable;
        [UxmlAttribute( "Ship" )] private GameObjectVariable m_shipVariable;
        [UxmlAttribute( "MapScale" )][Tooltip( "As 1/value" )] private float m_mapScale = 5.0f;
        [Header( "Island Marker" )]
        [UxmlAttribute( "IslandSet" )] private GameObjectRuntimeSet m_islandsSet;
        [UxmlAttribute( "IslandMarker" )] private VisualTreeAsset m_islandMarkerVisualTreeAsset;

        private readonly List<VisualElement> m_islandsMarkers = new();
        private VisualElement m_markerContainer;

        public MinimapElement() 
        {
            if ( !Application.isPlaying )
            {
                return;
            }

            schedule.Execute( Start ).ExecuteLater( 10 );
            this.RegisterUpdate( Update );// TODO : make an interface for update and start 
        }


        private void Start()
        {
            m_markerContainer = this.Q( name: "minimap-marker-container" );
            m_islandsSet.OnElementAdded += IslandAddedHandler;
            for ( int index = 0; index < m_islandsSet.Count; index++ )
            {
                CreateNewMarker();
            }
        }


        private void CreateNewMarker()
        {
            VisualElement marker = m_islandMarkerVisualTreeAsset != null ? m_islandMarkerVisualTreeAsset.Instantiate() : new VisualElement();
            marker.AddToClassList( "minimap-marker" );
            marker.AddToClassList( "island" );
            m_markerContainer.Add( marker );
            m_islandsMarkers.Add( marker );
        }


        private void IslandAddedHandler( GameObject newElement, int index )
        {
            CreateNewMarker();
        }


        public Vector2 TransformWorldToMapSpace( Vector3 position, Vector3 origin )
        {
            Vector2 offset = position - origin;
            offset.y = -offset.y;
            float offsetMagnitude = offset.magnitude;
            Vector2 offsetDirection = offset / offsetMagnitude;
            return offsetMagnitude * ( 1 / m_mapScale ) * offsetDirection;
        }


        private void Update()
        {
            if ( m_shipVariable.Value == null )
            {
                return;
            }

            Vector3 shipPosition = m_shipVariable.Value.transform.position;
            for ( int index = 0; index < m_islandsMarkers.Count; index++ )
            {
                Vector2 offset = new Vector2( -m_islandsMarkers[index].resolvedStyle.width * 0.5f, -m_islandsMarkers[index].resolvedStyle.height * 0.5f );
                m_islandsMarkers[index].transform.position = offset + TransformWorldToMapSpace( m_islandsSet[index].transform.localPosition, shipPosition );
            }
        }
    }
}
