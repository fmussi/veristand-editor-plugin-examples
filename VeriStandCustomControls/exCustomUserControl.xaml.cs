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
        public exCustomUserControl()
        {
            InitializeComponent();
        }

        public double CustomTankValue
        {
            get { return (double)GetValue(CustomTankValueProperty); }
            set { SetValue(CustomTankValueProperty, value); }
        }

        public static readonly DependencyProperty CustomTankValueProperty = DependencyProperty.Register("CustomTankValue", typeof(double), typeof(exCustomUserControl), new FrameworkPropertyMetadata(0.0));

        private void CustomTank_OnValueChanged(object sender, Controls.ValueChangedEventArgs<double> e)
        {

        }
    }
}
