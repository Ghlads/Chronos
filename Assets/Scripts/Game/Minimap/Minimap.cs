using Framework.Scriptable.Generated;
using System;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

namespace Game
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private RectVariable m_mapBoundsVariable;
        [SerializeField] private GameObjectVariable m_shipVariable;
        [SerializeField][Tooltip( "As 1/value" )] private float m_mapScale = 5.0f;
        [Header("Island Marker")]
        [SerializeField] private GameObjectRuntimeSet m_islandsSet;
        [SerializeField] private RectTransform m_islandMarkerContainer;
        [SerializeField] private GameObject m_islandMarkerPrefab;

        private readonly List<GameObject> m_islandsMarkers = new();

        private void Start()
        {
            m_islandsSet.OnElementAdded += IslandAddedHandler;
            for ( int index = 0; index  < m_islandsSet.Count; index++ )
            {
                CreateNewMarker();
            }
        }


        private void CreateNewMarker()
        {
            GameObject marker = Instantiate( m_islandMarkerPrefab, Vector3.zero, Quaternion.identity, m_islandMarkerContainer );
            m_islandsMarkers.Add( marker );
        }


        private void IslandAddedHandler( GameObject newElement, int index )
        {
            CreateNewMarker();
        }


        public Vector2 TransformWorldToMapSpace( Vector3 position, Vector3 origin )
        {
            Vector2 offset = position - origin;
            float offsetMagnitude = offset.magnitude;
            Vector2 offsetDirection = offset / offsetMagnitude;
            return offsetMagnitude * ( 1 / m_mapScale ) * offsetDirection;
        }


        private void Update()
        {
            Vector3 shipPosition = m_shipVariable.Value.transform.position;
            for ( int index = 0; index < m_islandsMarkers.Count; index++ )
            {
                m_islandsMarkers[index].transform.localPosition = TransformWorldToMapSpace( m_islandsSet[index].transform.localPosition, shipPosition );
            }
        }
    }
}
