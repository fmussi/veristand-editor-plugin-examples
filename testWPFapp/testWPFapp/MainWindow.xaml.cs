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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Binding binding = new Binding("Text");
            binding.Source = txtValue;
            //lblValue.SetBinding(TextBlock.TextProperty, binding); 
            //pnlMainGrid.MouseUp += new MouseButtonEventHandler(pnlMainGrid_MouseUp);
        }

        //private void pnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        //}

        //private void btnClickMe_Click(object sender, RoutedEventArgs e)
        //{
        //    string s = null;
        //    try
        //    {
        //        s.Trim();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Handled exception occured: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //    lbResult.Items.Add(this.FindResource("ComboBoxTitle").ToString());
        //}
    }
}
