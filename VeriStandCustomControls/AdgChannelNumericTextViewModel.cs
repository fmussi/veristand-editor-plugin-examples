using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Design;
using NationalInstruments.Controls.SourceModel;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.DynamicProperties;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Controls.ViewModels;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;


namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Defines the behavior of view models whose view is a numeric text box.- \ChannelNumericTextViewModel
    /// </summary>
    public class AdgChannelNumericTextViewModel : NumericTextViewModel, IChannelControlViewValueAccessor
    {
        private readonly NumericTextBoxChannelControlViewModelImplementation<AdgChannelNumericTextViewModel, AdgChannelNumericTextModel>
            _channelControlViewModelImplementation;

        /// <summary>
        /// Initializes a new instance of the AdgAdgChannelNumericTextViewModel class.
        /// </summary>
        /// <param name="model">The model associated with this view model.</param>
        public AdgChannelNumericTextViewModel(AdgChannelNumericTextModel model)
            : base(model)
        {
            _channelControlViewModelImplementation =
                new NumericTextBoxChannelControlViewModelImplementation<AdgChannelNumericTextViewModel, AdgChannelNumericTextModel>(this);
        }

        /// <summary>
        /// Sets the interval duration of the suppression timer.
        /// </summary>
        /// <param name="duration">time between timer ticks</param>
        internal void SetSuppressionTimerDuration(int duration)
        {
            _channelControlViewModelImplementation.SetSuppressionTimerDuration(duration);
        }

        /// <summary>
        ///  Creates ribbon content for the current selection when more than one item is selected.
        ///  The returned list can be RibbonGroups or RibbonTabs.  The groups will be added to
        ///  the default selection tab and the tabs will be added after this. See comments on
        ///  <see cref="IProvideCommandContent"/> for more information about correct usage of this function.
        /// </summary>
        /// <param name="context">The current display context</param>
        public override void CreateCommandContent(ICommandPresentationContext context)
        {
            base.CreateCommandContent(context);
            _channelControlViewModelImplementation.CreateCommandContent(context);
        }

        /// <inheritdoc/>
        protected override void OnCreateContextMenu(CreateContextMenuRoutedEventArgs args)
        {
            base.OnCreateContextMenu(args);
            _channelControlViewModelImplementation.OnCreateContextMenu(args);
        }

        /// <summary>
        /// Called when the view value has changed.
        /// </summary>
        /// <param name="newValue">The view's new value.</param>
        /// <param name="source">The source of the value change.</param>
        public void OnValueChanged(object newValue, PropertyChangeSource source)
        {
            _channelControlViewModelImplementation.OnValueChanged(newValue, source);
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(object newValue)
        {
            OnValueChanged(newValue, PropertyChangeSource.Interactive);
        }

        /// <summary>
        /// Called when the model this instance was watching has gone to the detached state.
        /// </summary>
        /// <param name="modelElement">The removed model</param>
        /// <returns>Return true to have your observer removed from watching this object</returns>
        public override bool ModelDetached(Element modelElement)
        {
            _channelControlViewModelImplementation.ModelDetached(modelElement);
            return base.ModelDetached(modelElement);
        }

        /// <summary>
        /// Gets or sets the current value displayed in the view.
        /// </summary>
        public object ViewValue
        {
            get
            {
                return Helper.GetValue();
            }
            set
            {
                Helper.SetValue(value);
            }
        }

        /// <summary>
        /// Called when a property of the model associated with this view model has changed.
        /// ChannelControlViewModel forwards this change to the associated model so that the model can respond to property changes
        /// that require interactions with the VeriStand gateway
        /// </summary>
        /// <param name="modelElement">The model that changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="transactionItem">The transaction item associated with the change.</param>
        public override void ModelPropertyChanged(Element modelElement, string propertyName, TransactionItem transactionItem)
        {
            base.ModelPropertyChanged(modelElement, propertyName, transactionItem);
            // adjust to font size, LV base class implementation excludes derived classes
            if (propertyName == VisualModel.FontName)
            {
                var control = ProxiedElement as Control;
                if (control != null)
                {
                    control.MinHeight = 0;
                    control.Height = float.NaN;
                    control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    var newMinimumHeight = control.DesiredSize.Height;

                    TransactionManager.TransactViewControlsSizeBehavior("Change control height and minimum height", () =>
                    {
                        Model.Height = (float)newMinimumHeight;
                        Model.MinHeight = (float)newMinimumHeight;
                    });
                }
            }
            if (propertyName == NumericTextBoxModel.ValueFormatterName || propertyName == NumericTextBoxModel.RadixVisibilityName)
            {
                ViewValue = Model.GetChannelControlModel().ChannelValue;
            }
            _channelControlViewModelImplementation.ModelPropertyChanged(modelElement, propertyName, transactionItem);
        }

        /// <summary>
        /// Applies a property value to the view.
        /// </summary>
        /// <param name="identifier">The property to set.</param>
        /// <param name="value">The new value for the property.</param>
        protected override void SetProperty(PropertySymbol identifier, object value)
        {
            // First check the implementation class, then go to base class.
            bool handled = _channelControlViewModelImplementation.SetProperty(identifier, value);
            if (!handled)
            {
                base.SetProperty(identifier, value);
            }
        }

        /// <inheritdoc />
        protected override NumericControlViewHelper CreateHelper(Type valueType, NumericControlViewModel viewModel, FrameworkElement view)
        {
            return (NumericControlViewHelper)Activator.CreateInstance(
                typeof(ChannelNumericTextViewHelper<>).MakeGenericType(valueType),
                viewModel,
                view);
        }

        /// <summary>
        /// The Helper used to set properties on the strongly typed views.
        /// </summary>
        /// <typeparam name="T">The type of the view.</typeparam>
        protected class ChannelNumericTextViewHelper<T> : NumericTextViewHelper<T>
        {
            /// <summary>
            /// The constructor for the ChannelNumericTextViewHelper.
            /// </summary>
            /// <param name="viewModel">The associated ViewModel</param>
            /// <param name="view">The associated View.</param>
            public ChannelNumericTextViewHelper(NumericTextViewModel viewModel, FrameworkElement view)
                : base(viewModel, view)
            {
            }

            /// <inheritdoc />
            protected override NumericTextBoxInteractionModes IndicatorInteractionMode => NumericTextBoxInteractionModes.ReadOnly;
        }

        /// <inheritdoc />
        public override void Placed(PlacementReleaseEventArgs args)
        {
            base.Placed(args);
            _channelControlViewModelImplementation.Placed(args);
        }

        /// <summary>
        /// Gets the adorners used with this control during a hard selection (left-click).
        /// Currently used to create an adorner that allows browsing to a channel path in a VeriStand SDF.
        /// </summary>
        /// <returns>An enumerable collection of hard select adorners.</returns>
        public override IEnumerable<Adorner> GetHardSelectAdorners()
        {
            Collection<Adorner> adorners = _channelControlViewModelImplementation.GetHardSelectAdorners(base.GetHardSelectAdorners());
            return adorners;
        }

        /// <summary>
        /// Gets the adorners used with this control during a soft selection (left-click).
        /// </summary>
        /// <returns>An enumerable collection of hard select adorners.</returns>
        public override IEnumerable<Adorner> GetSoftSelectAdorners()
        {
            Collection<Adorner> adorners = _channelControlViewModelImplementation.GetSoftSelectAdorners(base.GetSoftSelectAdorners());
            return adorners;
        }

        /// <inheritdoc />
        public override QueryResult<T> QueryService<T>()
            => QueryResult<T>.FromObject(_channelControlViewModelImplementation)
                .AppendedWith(base.QueryService<T>());

        bool IDesignerParent.CanAddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        bool IDesignerParent.CanRemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        void IDesignerParent.AddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        void IDesignerParent.RemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        void IDesignerParent.UpdatePositions(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        void IDesignerParent.FinishPlacement(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Adorner> IDesignerParentAdornerProvider.GetDragOverAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Adorner> IDesignerParentAdornerProvider.GetBoundsChangeAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets whether the timer used to suppress value changes after each view update is running.
        /// </summary>
        //internal bool IsSuppressUpdatesOnValueChangeTimerEnabled => _channelControlViewModelImplementation.IsSuppressUpdatesOnValueChangeTimerEnabled;

        ReparentingStyle IDesignerParent.ReparentingStyle => throw new NotImplementedException();

        SMPoint IDesignerParent.PastePositionOffset => throw new NotImplementedException();

        bool IDesignerParentAdornerProvider.ShowHighlightRect => throw new NotImplementedException();

        SMRect ICustomizeSoftSelectionRect.SoftSelectionRectangle => throw new NotImplementedException();
    }
}