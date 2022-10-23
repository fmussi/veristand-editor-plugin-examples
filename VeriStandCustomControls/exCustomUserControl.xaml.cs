using System;
using System.Windows;
using System.Windows.Input;
using NationalInstruments.Controls;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Interaction logic for exCustomUserControl.xaml
    /// </summary>
    public partial class exCustomUserControl
    {
        private readonly exCustomUserControlViewModel _viewModel;

        /// <summary>
        /// Constructor for exCustomUserControl
        /// </summary>
        /// <param name="viewModel">view model associated with this control</param>
        public exCustomUserControl(exCustomUserControlViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
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

        public double CustomTankValue
        {
            get { return (double)GetValue(CustomTankValueProperty); }
            set { SetValue(CustomTankValueProperty, value); }
        }

        public static readonly DependencyProperty CustomTankValueProperty = DependencyProperty.Register("CustomTankValue", typeof(double), typeof(exCustomUserControl), new FrameworkPropertyMetadata(0.0));

        private void CustomTank_OnValueChanged(object sender, Controls.ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, exCustomUserControlModel.CustomUserControlChannelName);
        }
    }
}
