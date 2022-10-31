using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using NationalInstruments.Composition;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Design;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Controls.SourceModel;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.DynamicProperties;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Controls.ViewModels;
using NationalInstruments.Hmi.Core.Screen;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.Shell;
using NationalInstruments.VeriStand.Tools;
//using NationalInstruments.PanelCommon.Design;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    //    public class modCustomControlViewModel : PanelControlViewModel
    // VisualViewModel
    public class modCustomControlViewModel : GaugeViewModel, IChannelControlViewValueAccessor, ICommonConfigurationPaneControl
    {
        /// <summary>
        /// Constructs a new instance of the modCustomControlViewModel class
        /// </summary>
        /// <param name="model">The modCustomControlModel associated with this view model.</param>
        // set implementation
        private readonly NumericPointerChannelControlViewModelImplementation<modCustomControlViewModel, modCustomControlModel> _channelControlViewModelImplementation;

        public modCustomControlViewModel(modCustomControlModel model) : base(model)
        {
            _channelControlViewModelImplementation = new NumericPointerChannelControlViewModelImplementation<modCustomControlViewModel, modCustomControlModel>(this);
            //WeakEventManager<modCustomControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "modCustomControlChannelValueChangedEvent", modCustomControlValueChangedEventHandler);
        }

        /// <summary>
        /// Called when the view value has changed.
        /// </summary>
        /// <param name="newValue">The view's new value.</param>

        protected override void OnValueChanged(object newValue)
        {
            OnValueChanged(newValue, PropertyChangeSource.Interactive);
        }

        /// <summary>
        /// Called when the view value has changed.
        /// </summary>
        /// <param name="newValue">The view's new value.</param>
        /// <param name="source">The source of the value change.</param>
        private void OnValueChanged(object newValue, PropertyChangeSource source)
        {
            _channelControlViewModelImplementation.OnValueChanged(newValue, source);
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

        protected override void OnCreateContextMenu(CreateContextMenuRoutedEventArgs args)
        {
            base.OnCreateContextMenu(args);
            _channelControlViewModelImplementation.OnCreateContextMenu(args);
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
            _channelControlViewModelImplementation.ModelPropertyChanged(modelElement, propertyName, transactionItem);
        }

        /// <summary>
        /// Applies a property value to the view.
        /// </summary>
        /// <param name="identifier">The property to set.</param>
        /// <param name="value">The new value for the property.</param>
        protected override void SetProperty(PropertySymbol identifier, object value)
        {
            bool handled = _channelControlViewModelImplementation.SetProperty(identifier, value);
            if (!handled)
            {
                base.SetProperty(identifier, value);
            }
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

        /// <inheritdoc />
        public override IEnumerable<Adorner> GetSoftSelectAdorners()
        {
            Collection<Adorner> adorners = _channelControlViewModelImplementation.GetSoftSelectAdorners(base.GetSoftSelectAdorners());
            return adorners;
        }

        /// <inheritdoc />
        public override QueryResult<T> QueryService<T>()
            => QueryResult<T>.FromObject(_channelControlViewModelImplementation)
                .AppendedWith(base.QueryService<T>());

        #region ICommonConfigurationPaneControl

        /// <inheritdoc/>
        public virtual bool HasTextContent => true;

        #endregion
        public bool CanAddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public void AddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public void UpdatePositions(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public void FinishPlacement(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Adorner> GetDragOverAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Adorner> GetBoundsChangeAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new System.NotImplementedException();
        }

        protected override void InvertScale(NumericPointerModel numericPointerModel)
        {
            throw new NotImplementedException();
        }

        protected override FrameworkElement CreateViewForType(Type numericType)
        {
            var view = new modCustomControl(this);
            return view;
        }
        public ReparentingStyle ReparentingStyle => throw new System.NotImplementedException();

        public SMPoint PastePositionOffset => throw new System.NotImplementedException();

        public bool ShowHighlightRect => throw new System.NotImplementedException();

        public SMRect SoftSelectionRectangle => throw new System.NotImplementedException();

    }
}