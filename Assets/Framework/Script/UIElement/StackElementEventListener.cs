using Framework.Core;
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Framework
{
    [UxmlElement]
    public partial class StackElementEventListener : VisualElement
    {
        public const int DELAY_BEFORE_VALID_BINDING_MS = 64;

        // Whole thinks should be an bundling all of this at once but UXML serialization and binding is weird I don't know how to properly handle that yet
        // GainFocus 
        public static readonly BindingId GainFocusCommandProperty = nameof( GainFocusCommand );
        [Header( "Gain Focus" )]
        [UxmlAttribute( "GainFocusCommandArgs" )] private CommandArgs m_gainFocusCommandArgs;

        private Command m_cachedGainFocusCommand;
        [UxmlObjectReference]
        [CreateProperty]
        public Command GainFocusCommand
        {
            get
            {
                if ( m_cachedGainFocusCommand != null )
                {
                    Command outCommand = m_cachedGainFocusCommand;
                    m_cachedGainFocusCommand = null;
                    return outCommand;
                }

                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
                    m_cachedGainFocusCommand = Command.Pool( this, dataSource, m_gainFocusCommandArgs != null ? m_gainFocusCommandArgs.Args : null );
                    NotifyPropertyChanged( GainFocusCommandProperty );
                }
            }
        }

        //LooseFocus
        public static readonly BindingId LooseFocusCommandProperty = nameof( LooseFocusCommand );
        [Header( "Loose Focus" )]
        [UxmlAttribute( "LooseFocusCommandArgs" )] private CommandArgs m_looseFocusCommandArgs;

        private Command m_cachedLooseFocusCommand;
        [UxmlObjectReference]
        [CreateProperty]
        public Command LooseFocusCommand
        {
            get
            {
                if ( m_cachedLooseFocusCommand != null )
                {
                    Command outCommand = m_cachedLooseFocusCommand;
                    m_cachedLooseFocusCommand = null;
                    return outCommand;
                }

                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
                    m_cachedLooseFocusCommand = Command.Pool( this, dataSource, m_looseFocusCommandArgs != null ? m_looseFocusCommandArgs.Args : null );
                    NotifyPropertyChanged( LooseFocusCommandProperty );
                }
            }
        }


        //Open
        public static readonly BindingId OpenCommandProperty = nameof( OpenCommand );
        [Header( "Open" )]
        [UxmlAttribute( "OpenCommandArgs" )] private CommandArgs m_openCommandArgs;

        private Command m_cachedOpenCommand;
        [UxmlObjectReference]
        [CreateProperty]
        public Command OpenCommand
        {
            get
            {
                if ( m_cachedOpenCommand != null )
                {
                    Command outCommand = m_cachedOpenCommand;
                    m_cachedOpenCommand = null;
                    return outCommand;
                }

                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
                    m_cachedOpenCommand = Command.Pool( this, dataSource, m_openCommandArgs != null ? m_openCommandArgs.Args : null );
                    NotifyPropertyChanged( OpenCommandProperty );
                }
            }
        }


        //Close
        public static readonly BindingId CloseCommandProperty = nameof( CloseCommand );
        [Header( "Close" )]
        [UxmlAttribute( "CloseCommandArgs" )] private CommandArgs m_closeCommandArgs ;

        private Command m_cachedCloseCommand;
        [UxmlObjectReference]
        [CreateProperty]
        public Command CloseCommand
        {
            get
            {
                if ( m_cachedCloseCommand != null )
                {
                    Command outCommand = m_cachedCloseCommand;
                    m_cachedCloseCommand = null;
                    return outCommand;
                }

                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
                    m_cachedCloseCommand = Command.Pool( this, dataSource, m_closeCommandArgs != null ? m_closeCommandArgs.Args : null );
                    NotifyPropertyChanged( CloseCommandProperty );
                }
            }
        }


        public StackElementEventListener()
        {
            List<Action> toProcessQueue = new();
            bool isInit = false;
            RegisterCallback<EventStackOpen>( evt =>
            {
                if ( isInit )
                {
                    OpenCommand = Command.Trigger;
                }
                else
                {
                    toProcessQueue.Add( () => OpenCommand = Command.Trigger );
                }
            } );
            RegisterCallback<EventStackGainFocus>( evt => 
            {
                if ( isInit )
                {
                    GainFocusCommand = Command.Trigger;
                }
                else
                {
                    toProcessQueue.Add( () => GainFocusCommand = Command.Trigger );
                }
            } );
            RegisterCallback<EventStackClose>( evt =>
            {
                if ( isInit )
                {
                    CloseCommand = Command.Trigger;
                }
                else
                {
                    toProcessQueue.Add( () => CloseCommand = Command.Trigger );
                }
            } );
            RegisterCallback<EventStackLooseFocus>( evt =>
            {
                if ( isInit )
                {
                    LooseFocusCommand = Command.Trigger;
                }
                else
                {
                    toProcessQueue.Add( () => LooseFocusCommand = Command.Trigger );
                }
            } );

            RegisterCallbackOnce<AttachToPanelEvent>( evt => schedule.Execute( Init ).ExecuteLater( DELAY_BEFORE_VALID_BINDING_MS ) );
            void Init()
            {
                isInit = true;
                foreach ( Action action in toProcessQueue )
                {
                    action();
                }

                toProcessQueue = null;
            }
        }

    }
}
