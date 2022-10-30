using System;
using System.Windows;
using System.Windows.Input;
using NationalInstruments.Controls;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Interaction logic for exCustomUserControl.xaml
    /// </summary>
    public partial class modCustomControl
    {
        private readonly modCustomControlViewModel _viewModel;

        /// <summary>
        /// Constructor for exCustomUserControl
        /// </summary>
        /// <param name="viewModel">view model associated with this control</param>
        public modCustomControl(modCustomControlViewModel viewModel)
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

        public static readonly DependencyProperty CustomControlValueProperty = DependencyProperty.Register("CustomControlValue", typeof(double), typeof(modCustomControl), new FrameworkPropertyMetadata(0.0));

        private void CustomControl_OnValueChanged(object sender, Controls.ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, modCustomControlModel.modCustomControlChannelName);
        }

        private void CustomControl_OnRollChanged(object sender, ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, modCustomControlModel.modCustomControlChannelName);
        }
    }
}
