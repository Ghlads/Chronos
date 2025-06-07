using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    [UxmlElement]
    public partial class EnhancedListView : ListView
    {
        private PropertyPath m_itemsSourcePath;
        [UxmlAttribute]
        public string ItemsSource
        {
            get => m_itemsSourcePath.ToString();
            set
            {
                m_itemsSourcePath = new PropertyPath( value );
                if ( m_itemsSourcePath.IsEmpty || dataSource == null )
                {
                    return;
                }

                ResolveItemsSource();
            }
        }

        public void ResolveItemsSource()
        {
            if ( PropertyContainer.TryGetValue( dataSource, PropertyPath.Combine( dataSourcePath, m_itemsSourcePath ), out object outValue ) )
            {
                if ( outValue != null && outValue is IList list )
                {
                    itemsSource = list;
                    return;
                }

                Debug.LogError( $"Invalid path for {dataSourcePath} + {m_itemsSourcePath}" );
            }
        }

        public EnhancedListView()
        {
            bindItem = Bind;
            unbindItem = Unbind;

            schedule.Execute( () => ResolveItemsSource() ).Until( () => itemsSource != null );
        }


        private void Bind( VisualElement element, int index )
        {
            element.dataSource = itemsSource[index];
        }


        private void Unbind( VisualElement element, int index )
        {
            element.dataSource = null;
        }
    }
}
