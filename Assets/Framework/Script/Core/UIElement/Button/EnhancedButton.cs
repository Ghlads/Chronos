using Unity.Properties;
using UnityEngine.UIElements;

namespace Framework.Core
{
    [UxmlElement]
    public partial class EnhancedButton : Button
    {
        public static readonly BindingId CommandProperty = nameof( Command );

        [UxmlObjectReference]
        [CreateProperty]
        public Command Command
        {
            get
            {
                return Command.Default;
            }
            set
            {
                if ( Command.Trigger == value )
                {
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
