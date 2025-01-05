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
using NationalInstruments.VeriStand.ServiceModel;

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
                // Add sample command
                //context.Add(new ShellCommandInstance(MyCommand) { Weight = 0.9 }, ToolLauncherContentBuilder.ToolLauncherVisualFactoryInstance);
                context.Add(new ShellCommandInstance(VsDebugToolCommand) { Weight = 0.9 }, ToolLauncherContentBuilder.ToolLauncherVisualFactoryInstance);
            }
        }
#region SampleCommand
        // sample method for ICommandEx
        private static readonly ICommandEx MyCommand = new ShellRelayCommand(ExecuteMyCommand, CanExecuteMyCommand)
        {
            UniqueId = "MyCommandId",
            LabelTitle = "My Command Display Name",
            // Relative path to the accompanying image/icon resource within the assembly
            SmallImageSource = ResourceHelpers.LoadImage(typeof(TestCommandContentProvider), "Resources/ViewConsole.png")
        };

        // sample method for Execute
        private static void ExecuteMyCommand(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            MessageBox.Show("This is my command's implementation");
        }

        // sample methof for CanExecute
        private static bool CanExecuteMyCommand(ICommandParameter parameter, ICompositionHost host, DocumentEditSite site)
        {
            // With this implementation, the command is always enabled in the tool launcher.
            // If conditionally disabling the command is not required, then this CanExecute handler can be removed entirely.
            return true;
        }
        #endregion
 
        public static readonly ICommandEx VsDebugToolCommand = new ShellRelayCommand((p, host, s) => host.LaunchLegacyToolAsync(_vsDebugToolPath, null).IgnoreAwait())
        {
            UniqueId = "NI.CustomTools:VsDebugTool",
            LabelTitle = "Custom Device Debug Tool",
            SmallImageSource = ResourceHelpers.LoadImage(typeof(TestCommandContentProvider), "Resources/Workspace_16x16.png")
        };

        //private static readonly string _vsDebugToolPath = "C:\\AzDo\\itmussif-vsdevtools\\Builds\\vsCdLogViewer\\vsCdLogViewer_Main(SysLog).vi";
        //move back from Project Explorer.exe\\National Instruments\\Workspace\\Tools to point to <Program Files>\\Veristand <year> folder
        private static readonly string _vsDebugToolPath = "..\\..\\..\\..\\vsCdLogViewer\\vsCdLogViewer_Main(SysLog).vi";
    }
}
