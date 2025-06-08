using Framework.Core;
using Framework.Scriptable.Generated;
using System;
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

        private GameObjectRuntimeSet m_islandsSet;
        [UxmlAttribute]
        private GameObjectRuntimeSet IslandSet
        {
            get => m_islandsSet;
            set
            {
                if ( m_islandsSet != null )
                {
                    m_islandsSet.OnElementAdded -= IslandAddedHandler;
                }

                m_islandsSet = value;
                if ( m_islandsSet != null )
                {
                    m_islandsSet.OnElementAdded += IslandAddedHandler;
                    if ( m_treeAsset != null )
                    {
                        for ( int index = 0; index < m_islandsSet.Count; index++ )
                        {
                            CreateNewMarker();
                        }
                    }
                }
            }
        }

        [UxmlAttribute( "IslandMarker" )] private VisualTreeAsset m_islandMarkerVisualTreeAsset;
        private VisualTreeAsset m_treeAsset;
        [UxmlAttribute]
        private VisualTreeAsset TreeAsset
        {
            get { return m_treeAsset; }
            set 
            {
                m_treeAsset = value;
                if ( m_treeAsset != null )
                {
                    Clear();
                    Add( m_treeAsset.Instantiate() );
                    m_markerContainer = this.Q( name: "minimap-marker-container" );
                    foreach ( VisualElement element in m_islandsMarkers )
                    {
                        m_markerContainer.Add( element );
                    }
                }
            }
        }

        private readonly List<VisualElement> m_islandsMarkers = new();
        private VisualElement m_markerContainer;

        public MinimapElement() 
        {
            if ( !Application.isPlaying )
            {
                return;
            }

            this.RegisterUpdate( Update );// TODO : make an interface for update and start 
        }


        ~MinimapElement()
        {
            if ( m_islandsSet != null )
            {
                m_islandsSet.OnElementAdded -= IslandAddedHandler;
            }
        }


        private void CreateNewMarker()
        {
            VisualElement marker = m_islandMarkerVisualTreeAsset != null ? m_islandMarkerVisualTreeAsset.Instantiate() : new VisualElement();
            marker.AddToClassList( "minimap-marker" );
            marker.AddToClassList( "island" );
            if ( m_markerContainer != null )
            {
                m_markerContainer.Add( marker );
            }
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
                if ( m_islandsMarkers[index] == null || m_islandsMarkers[index].transform == null )
                {
                    Debug.LogWarning( "Null marker or transform" );
                    continue;
                }

                if ( m_islandsMarkers[index].panel == null )
                {
                    Debug.LogWarning( "Marker not attached yet" );
                    continue;
                }

                Vector2 offset = new Vector2( -m_islandsMarkers[index].resolvedStyle.width * 0.5f, -m_islandsMarkers[index].resolvedStyle.height * 0.5f );
                m_islandsMarkers[index].transform.position = offset + TransformWorldToMapSpace( m_islandsSet[index].transform.localPosition, shipPosition );
            }
        }
    }
}
 