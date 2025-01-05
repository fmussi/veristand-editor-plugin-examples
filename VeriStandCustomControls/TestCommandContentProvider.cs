using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using NationalInstruments.Composition;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.DataTypes;
using NationalInstruments.Design;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Command content plugin for this assembly
    /// <see cref="IPushCommandContent"/> for the various entrypoints where commands can be added.
    /// </summary>
    /// <remarks>The LaunchKeyword is to ensure we are loaded directly at application startup,
    /// which is necessary if you want to add to menus.</remarks>
    [ExportPushCommandContent]
    [BindsToKeyword(DocumentEditSite.LaunchKeyword)]
    public class TestCommandContentProvider : PushCommandContent
    {
        /// <summary>
        /// Imported composition host for accessing other plugin components
        /// </summary>
        [Import]
        public ICompositionHost Host { get; set; }

        /// <inheritdoc />
        /// 
        public override void CreateApplicationContent(ICommandPresentationContext context)
        {
            base.CreateApplicationContent(context);
            using (context.AddToolLauncherContent())
            {
                // The Weight determines where in the list of Tool Launcher items the new item will appear. (A lower value is higher in the list.)
                context.Add(new ShellCommandInstance(MyCommand) { Weight = 0.9 }, ToolLauncherContentBuilder.ToolLauncherVisualFactoryInstance);          
            }
        }

        private static readonly ICommandEx MyCommand = new ShellRelayCommand(ExecuteMyCommand, CanExecuteMyCommand)
        {
            UniqueId = "MyCommandId",
            LabelTitle = "My Command Display Name",
            // Relative path to the accompanying image/icon resource within the assembly
            SmallImageSource = ResourceHelpers.LoadImage(typeof(TestCommandContentProvider), "Resources/SystemExplorer_16.png")
        };

        private static void ExecuteMyCommand(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            MessageBox.Show("This is my command's implementation");
        }

        private static bool CanExecuteMyCommand(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            // With this implementation, the command is always enabled in the tool launcher.
            // If conditionally disabling the command is not required, then this CanExecute handler can be removed entirely.
            return true;
        }
    }
}
