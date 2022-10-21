using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Primitives;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Design;
using NationalInstruments.Hmi.Core.Screen;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.Shell;
using NationalInstruments.VeriStand.Tools;
using NationalInstruments.Hmi.Core.Controls.ViewModels;


namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// The view model which controls how changes on the view are propagated to the model.
    /// This ViewModel extends directly from VisualViewModel which is the base view model for all PF control view models
    /// This class implementes IControlContextMenuHelper which gives it the ability to provide custom right click menus
    /// </summary>
    public class exAttitudeIndicatorViewModel : ChannelNumericTextViewModel
    {
        /// <summary>
        /// Constructs a new instance of the PulseWidthModulationControlViewModel class
        /// </summary>
        /// <param name="model">The PulseWidthModulationControlModel associated with this view model.</param>
        public exAttitudeIndicatorViewModel(exAttitudeIndicatorModel model)
            : base(model)
        {
            // Register for channel value change events on the model.  The weak event manager is used here since it helps prevent memory leaks associated
            // with registering for events and lets us be less careful about unregistering for these events at a later time.
            //WeakEventManager<PulseWidthModulationControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "DutyCycleChannelValueChangedEvent", DutyCycleValueChangedEventHandler);
            //WeakEventManager<PulseWidthModulationControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "FrequencyChannelValueChangedEvent", FrequencyValueChangedEventHandler);
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

        /// <summary>
        /// Gets or sets if model value changes should be suppressed on the view.
        /// </summary>
        internal bool SuppressValueChanges { get; set; }


        /// <summary>
        /// Creates the view associated with this view model by initializing a new instance of our custom control class exAttitudeIndicator
        /// This is an opportunity to provide callbacks to the view and to hook up event handlers.  In this case we add a value changed event handler so we can
        /// react when the view changes value.
        /// </summary>
        /// <returns>pwmcontrol view</returns>
        public override object CreateView()
        {
            //var view = new ex_AttitudeIndicator();
            var view = new exAttitudeIndicator();
            // numeric controls have a helper which needs to get attached to the view that helps setting values of different types to the control. Since we only have I32
            // for our control type we just hard code that when creating the helper
            //Helper = CreateHelper(typeof(int), this, view);
            return view;
        }

        private void FaultStateChanged(object sender, FaultStateChangeEventArgs e)
        {
            //((exAttitudeIndicatorModel)Model).SetFaultStateAsync(e.FaultState).IgnoreAwait();
        }


    }
}
