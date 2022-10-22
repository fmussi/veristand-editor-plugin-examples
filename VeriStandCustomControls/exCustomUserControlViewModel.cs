using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Screen;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.Shell;
using NationalInstruments.VeriStand.Tools;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    public class exCustomUserControlViewModel : VisualViewModel, IControlContextMenuHelper
    {
        /// <summary>
        /// Constructs a new instance of the exCustomUserControlViewModel class
        /// </summary>
        /// <param name="model">The exCustomUserControlModel associated with this view model.</param>

        public exCustomUserControlViewModel(exCustomUserControlModel model) : base(model)
        {
            WeakEventManager<exCustomUserControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "CustomControlChannelValueChangedEvent", CustomControlValueChangedEventHandler);
        }

        /// <summary>
        /// Override resize behavior so the control cannot be resized.  This is done because with composite controls it is a lot of work to get all the individual components
        /// to scale reasonably with respect to each other
        /// </summary>
        public override ResizeDirections ResizeDirections
        {
            get
            {
                return ResizeDirections.None;
            }
        }

        private void CustomControlValueChangedEventHandler(object sender, ChannelValueChangedEventArgs e)
        {

        }

        /// <summary>
        /// Gets or sets if model value changes should be suppressed on the view.
        /// </summary>
        internal bool SuppressValueChanges { get; set; }


    }
}
