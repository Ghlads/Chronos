using Unity.Properties;
using UnityEngine.UIElements;

namespace Framework.Core
{
    [UxmlElement]
    public partial class EnhancedButton : Button
    {
        public static readonly BindingId CommandProperty = nameof( Command );

        [UxmlAttribute( "CommandArgs" )] private CommandArgs m_commandArgs;

        private Command m_cachedCommand;
        [UxmlObjectReference]
        [CreateProperty]
        public Command Command
        {
            get
            {
                if ( m_cachedCommand != null)
                {
                    Command outCommand = m_cachedCommand;
                    m_cachedCommand = null;
                    return outCommand;
                }

                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
                    m_cachedCommand = Command.Pool( this, dataSource, m_commandArgs != null ? m_commandArgs.Args : null );
                    NotifyPropertyChanged( CommandProperty );
                }
            }
        }

        public EnhancedButton() 
        {
            clicked += ClickHandler;
        }

        private void ClickHandler()
        {
            Command = Command.Trigger;
        }
    }
}
