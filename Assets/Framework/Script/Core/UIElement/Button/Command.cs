using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Framework.Core
{
    /// <summary>
    /// This class is used in a setter to trigger an event
    /// Made to be used in command data source way
    /// Bind a command to source on your UI and assign your property to <see cref = "Trigger"/> from VisualElement
    /// <example>
    /// <code>
    /// // Obviously you need to bind your Command property between your element and your data source. You can reverse the command direction if you inverse this exemple ( data source triggering a command on an element )
    /// public class MyVisualElementWithCommand : VisualElement
    /// {
    ///     public static readonly BindingId CommandProperty = nameof( Command );
    ///     
    ///     [UxmlObject][CreateProperty]
    ///     public Command Command
    ///     {
    ///         get => Command.Default;
    ///         set
    ///         {
    ///             if ( value == Command.Trigger )
    ///             {
    ///                 NotifyPropertyChanged( CommandProperty );
    ///             }
    ///         }
    ///     }
    ///     
    ///     ...
    ///     
    ///     public void MyFunctionTriggeringMyCommand()
    ///     {
    ///         Command = Command.Trigger;
    ///     }
    /// }
    ///   
    /// public class MyDataSource
    /// {
    ///     [CreateProperty]
    ///     public Command DoSomethingCommand
    ///     {
    ///         get => Command.Default;
    ///         set
    ///         {
    ///             // Do something here
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [UxmlObject]
    public partial class Command : IDisposable
    {
        private readonly static List<Command> s_pool = new List<Command>(25);

        public static Command Pool(VisualElement element = null, object datasource = null, List<UnityEngine.Object> additionalParams = null )
        {
            Command pooled = s_pool.Count <= 0 ? new() : s_pool.PopLast();
            pooled.ElementSource = element;
            pooled.AdditionalParams = additionalParams;
            pooled.DataSource = datasource;
            return pooled;
        }

        public void Dispose()
        {
            ElementSource = null;
            DataSource = null;
            AdditionalParams = null;
            s_pool.Add(this);
        }

        public readonly static Command Default = new Command();
        public readonly static Command Trigger = new Command();

        public VisualElement ElementSource;
        public object DataSource;
        public List<UnityEngine.Object> AdditionalParams;
    }
}
