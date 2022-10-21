using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Interaction logic for exAttitudeIndicator.xaml
    /// </summary>
    public partial class exAttitudeIndicator
    {
        private readonly exAttitudeIndicatorViewModel _viewModel;

        /// <summary>
        /// Constructor for exAttitudeIndicator
        /// </summary>
        /// <param name="viewModel">view model associated with this control</param>
        public exAttitudeIndicator(exAttitudeIndicatorViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        /// <summary>
        /// Callback for setting the fault state
        /// </summary>
        public event EventHandler<FaultStateChangeEventArgs> FaultStateChanged;

        /// <summary>
        /// Raises the FaultStateChanged event. Invoked when the channel value changes.
        /// </summary>
        /// <param name="newFaultState">The new fault state.</param>
        protected virtual void OnFaultStateChanged(FaultState newFaultState)
        {
            var faultStateChangedSubscribers = FaultStateChanged;
            if (faultStateChangedSubscribers != null)
            {
                faultStateChangedSubscribers(this, new FaultStateChangeEventArgs(newFaultState));
            }
        }

        /// <summary>
        /// Event that is fired when the value on the control changes
        /// </summary>
        public event EventHandler<CustomChannelValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Raises the ChannelValueChanged event. Invoked when the channel value changes.
        /// </summary>
        /// <param name="channelValue">New channel value</param>
        /// <param name="channelName">Name of the channel that changed</param>
        protected virtual void OnValueChanged(double channelValue, string channelName)
        {
            var channelValueChangedSubscribers = ValueChanged;
            if (channelValueChangedSubscribers != null)
            {
                channelValueChangedSubscribers(this, new CustomChannelValueChangedEventArgs(channelValue, channelName));
            }
        }
    }
}
