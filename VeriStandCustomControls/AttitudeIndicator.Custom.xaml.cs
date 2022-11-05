using System;
using System.Windows;
using System.Windows.Input;
using NationalInstruments.Controls;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Interaction logic for AttitudeIndicatorControl.xaml
    /// </summary>
    public partial class AttitudeIndicatorControl
    {
        private readonly AttitudeIndicatorControlViewModel _viewModel;

        /// <summary>
        /// Constructor for AttitudeIndicatorControl
        /// </summary>
        /// <param name="viewModel">view model associated with this control</param>
        public AttitudeIndicatorControl(AttitudeIndicatorControlViewModel viewModel)
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

        public double CustomControlValue
        {
            get { return (double)GetValue(CustomControlValueProperty); }
            set { SetValue(CustomControlValueProperty, value); }
        }

        public static readonly DependencyProperty CustomControlValueProperty = DependencyProperty.Register("CustomControlValue", typeof(double), typeof(AttitudeIndicatorControl), new FrameworkPropertyMetadata(0.0));

        //private void CustomControl_OnValueChanged(object sender, ValueChangedEventArgs<double> e)
        //{
        //    OnValueChanged(e.NewValue, ModCustomControlModel.modCustomControlChannelName);
        //}

        private void AttitudeIndicatorControl_OnRollChanged(object sender, ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, AttitudeIndicatorControlModel.attitudeIndicatorControlChannelName);
        }
    }
}
