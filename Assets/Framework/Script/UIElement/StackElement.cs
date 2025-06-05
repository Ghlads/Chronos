using Framework.Core;
using Framework.Scriptable;
using Framework.Scriptable.Generated;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Framework
{
    [UxmlElement]
    public partial class StackElement : VisualElement
    {
        [Serializable]
        public class OpenOptions
        {
            public bool HidePrevious = false;
        }

        private ScriptableEvent m_backEvent;
        [UxmlAttribute]
        private ScriptableEvent BackEvent
        {
            get { return m_backEvent; }
            set 
            { 
                m_backEvent = value;
                if ( !Application.isPlaying )
                {
                    return;
                }

                if ( m_backEvent != null )
                {
                    m_backEvent.AddListener( Close );
                }
            }
        }


        private VisualTreeAsset m_rootElementAsset;
        [UxmlAttribute]
        private VisualTreeAsset RootElementAsset
        {
            get { return m_rootElementAsset; }
            set 
            { 
                m_rootElementAsset = value; 
                if ( m_rootElementAsset != null && m_stack.Count <= 0 )
                {
                    PushElement( m_rootElementAsset.Instantiate() );
                }
            }
        }


        private VisualElementVariable m_variable;
        [UxmlAttribute]
        private VisualElementVariable Variable
        {
            get { return m_variable; }
            set 
            { 
                m_variable = value; 
                if ( m_variable != null )
                {
                    m_variable.Value = this;
                }
            }
        }


        private readonly Stack<VisualElement> m_stack = new Stack<VisualElement>();
        public VisualElement ActiveElement => m_stack.Count > 0 ? m_stack.Peek() : null;


        public StackElement()
        {
            
        }


        ~StackElement()
        {
            if ( BackEvent != null )
            {
                BackEvent.RemoveListener( Close );
            }
        }


        public void Open( VisualTreeAsset element, OpenOptions options = null )
        {
            Assert.IsTrue( m_stack.Count > 0 );
            ActiveElement.SendEvent( EventStackLooseFocus.GetPooled( ActiveElement ) );
            if ( options.HidePrevious )
            {
                ActiveElement.Hide();
            }
            PushElement( element.Instantiate() );
        }


        private void PushElement( VisualElement element )
        {
            element.style.position = Position.Absolute;
            element.style.width = Length.Percent( 100 );
            element.style.height = Length.Percent( 100 );
            Add( element );
            m_stack.Push( element );
            element.SendEvent( EventStackOpen.GetPooled( element ) );
        }


        public void Close()
        {
            if ( m_stack.Count <= 0 )
            {
                Debug.LogWarning( "Last menu was closed, should not happen unless you know what you do" );
                return;
            }

            ActiveElement.SendEvent( EventStackClose.GetPooled( ActiveElement ) );
            Remove( m_stack.Pop() );
            ActiveElement.SendEvent( EventStackGainFocus.GetPooled( ActiveElement ) );
            ActiveElement.Display();
        }
    }


    public class EventStackBase<T> : EventBase<T>  where T : EventStackBase<T>, new()
    {
        public static T GetPooled( IEventHandler target )
        {
            T @event = EventBase<T>.GetPooled();
            @event.target = target;
            return @event;
        }
    }
    public class EventStackLooseFocus : EventStackBase<EventStackLooseFocus> {}
    public class EventStackGainFocus : EventStackBase<EventStackGainFocus> {}
    public class EventStackClose : EventStackBase<EventStackClose> {}
    public class EventStackOpen : EventStackBase<EventStackOpen> {}
}

