using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.SourceModel;
using NationalInstruments.VeriStand.Shell;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Interaction logic for SingleChannelAdorner.xaml.  This class implements IModelObserver which lets it watch a model for property changes (and other things but in this case we just care about property changes)
    /// </summary>
    public partial class SingleChannelAdorner : IModelObserver
    {
        /// <summary>
        /// Constructs a new instance of the SingleChannelAdorner class
        /// </summary>
        /// <param name="adornedElement">The visual element this adorner is attached to.</param>
        public SingleChannelAdorner(PlatformVisual adornedElement)
        {
            InitializeComponent();
            // set the data context for the control to itself. This lets us bind to the commands by name instead of having to use a more advanced binding
            DataContext = this;
            Model = DesignerNodeHelpers.GetModelForVisual(adornedElement) as VisualModel;
            // set up the commands which are used by the adorner buttonsf
            LoadFirstPopupCommand = new RelayCommand(ExecuteLoadFirstPopup);
            // Hook up event handlers.  We set up event handlers for when keys are pressed on the adorner as well as when the properties change on either of the channel
            // selection popups
            WeakEventManager<UIElement, KeyEventArgs>.AddHandler(this, "KeyDown", HandleKeyDown);
            WeakEventManager<UIElement, KeyEventArgs>.AddHandler(this, "KeyUp", HandleKeyUp);
            WeakEventManager<ChannelPopup, PropertyChangedEventArgs>.AddHandler(FirstUiSdfPopup, "PropertyChanged", FirstChannelPropertyChanged);
            // add this adorner as an observer of the model
            Model.AddObserver(this);
            FirstTextControl.Text = ((ChannelCompassModel)Model).channelCompassChannel;
        }

        /// <summary>
        /// the model that is adorned
        /// </summary>
        protected VisualModel Model { get; set; }
        private void FirstChannelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChannelControlModelPropertyNames.ChannelName)
            {
                using (
                    var transaction = Model.TransactionManager.BeginTransaction(
                        "Set channel",
                        TransactionPurpose.User))
                {
                    ((ChannelCompassModel)Model).channelCompassChannel = FirstUiSdfPopup.Channel;
                    transaction.Commit();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1720:Identifier 'obj' contains type name", Justification = "Moving to roslyn")]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1720:Identifier 'obj' contains type name", Justification = "Moving to roslyn")]
        private void ExecuteLoadFirstPopup(object obj)
        {
            // open the first popup, showing only writeable channels
            FirstUiSdfPopup.ShowWaveforms = false;
            FirstUiSdfPopup.ShowSdfBrowser(Model.Host, true, true);
        }

        /// <summary>
        ///     Command for launching the first popup
        /// </summary>
        public ICommand LoadFirstPopupCommand { get; private set; }

        /// <summary>
        /// List of keys to ignore when pressed over the adorner
        /// </summary>
        private static List<Key> _keysToEat = new List<Key>()
        {
            Key.Up, Key.Down, Key.Left, Key.Right, Key.PageDown, Key.PageUp, Key.Home, Key.End
        };

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (_keysToEat.Contains(e.Key))
            {
                e.Handled = true;
            }
            if (e.Key == Key.Tab)
            {
                var designer = DesignerEditControl.GetDesigner(this);
                designer?.EditControl.Focus();
                e.Handled = true;
            }
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (_keysToEat.Contains(e.Key))
            {
                e.Handled = true;
            }
        }

        #region IModelObserver
        /// <summary>
        /// Handles changes to the model properties. Specifically, this method looks for changes in the channel names of the PWMControlModel and pushing
        /// them to the adorner text
        /// </summary>
        /// <param name="modelElement">The model element that changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="transactionItem">The current transaction.</param>
        public void ModelPropertyChanged(Element modelElement, string propertyName, TransactionItem transactionItem)
        {
            var propertyExpressionTransactionItem = transactionItem as PropertyExpressionTransactionItem;
            if (propertyName == ChannelCompassModel.channelCompassChannelName && propertyExpressionTransactionItem != null)
            {
                FirstTextControl.Text = propertyExpressionTransactionItem.NewValue as string;
            }
        }

        /// <summary>
        /// Called when a new component is added; required for IModelObserver but unused by SingleChannelAdorner
        /// </summary>
        /// <param name="owner">Unused as ChannelBoundValueAdorner does not monitor when components are added.</param>
        /// <param name="newComponent">Unused as ChannelBoundValueAdorner does not monitor when components are added.></param>
        /// <param name="insertedAtIndex">Unused as ChannelBoundValueAdorner does not monitor when components are added.</param>
        /// <param name="transactionItem">Unused as ChannelBoundValueAdorner does not monitor when components are added.</param>
        public void ComponentAdded(Element owner, Element newComponent, int insertedAtIndex, TransactionItem transactionItem)
        {
        }

        /// <summary>
        /// Called when a component is removed; required for IModelObserver but unused by SingleChannelAdorner
        /// </summary>
        /// <param name="owner">Unused as ChannelBoundValueAdorner does not monitor when components are removed.</param>
        /// <param name="oldComponent">Unused as ChannelBoundValueAdorner does not monitor when components are removed.</param>
        /// <param name="transactionItem">Unused as ChannelBoundValueAdorner does not monitor when components are removed.</param>
        public void ComponentRemoved(Element owner, Element oldComponent, TransactionItem transactionItem)
        {
        }

        /// <summary>
        /// Called when the observed model has gone to the detached state.
        /// </summary>
        /// <param name="modelElement">The removed model</param>
        /// <returns>Returns true to have this adorner removed from watching the model.</returns>
        public bool ModelDetached(Element modelElement)
        {
            return true;
        }
        #endregion
    }
}
