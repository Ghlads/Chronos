using UnityEngine.UIElements;

namespace Framework.Core
{
    public class GenericFoldout : VisualElement
    {
        private readonly Toggle m_toggle;
        private readonly VisualElement m_container;

        private bool m_isBuild = false;
        public override VisualElement contentContainer => m_isBuild ? m_container : this;

        public GenericFoldout( VisualElement headerElement )
        {
            style.flexShrink = 0;
            style.flexGrow = 0;
            m_container = new VisualElement();
            m_container.style.paddingLeft = new Length( 25 );
            m_toggle = new Toggle();
            m_toggle.AddToClassList( "unity-foldout__toggle" );


            VisualElement header = new();
            header.style.flexGrow = 1;

            header.Add( m_toggle );
            header.Add( headerElement );
            Add( header );
            Add( m_container );

            m_isBuild = true;

            header.style.flexDirection = FlexDirection.Row;
            m_toggle.RegisterCallback<ChangeEvent<bool>>( ToggleChangeHandler );
            UpdateVisibility( m_toggle.value );
        } 


        private void ToggleChangeHandler( ChangeEvent<bool> evt )
        {
            UpdateVisibility( evt.newValue );
        }

        private void UpdateVisibility( bool value )
        {
            if ( !value )
            {
                m_container.Hide();
            }
            else
            {
                m_container.Display();
            }
        }
    }
}
