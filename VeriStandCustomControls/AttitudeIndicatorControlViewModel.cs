using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows;
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
    // IChannelControlViewValueAccessor, ICommonConfigurationPaneControl
    public class AttitudeIndicatorControlViewModel : VisualViewModel, IControlContextMenuHelper
    {
        //private readonly NumericPointerChannelControlViewModelImplementation<AttitudeIndicatorControlViewModel, AttitudeIndicatorControlModel> _channelControlViewModelImplementation;
        /// <summary>
        /// Constructs a new instance of the modCustomControlViewModel class
        /// </summary>
        /// <param name="model">The modCustomControlModel associated with this view model.</param>
        // set implementation

        public AttitudeIndicatorControlViewModel(AttitudeIndicatorControlModel model) : base(model)
        {
           // does the implementation handle value changes?
            WeakEventManager<AttitudeIndicatorControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "modCustomControlChannelValueChangedEvent", ModCustomControlValueChangedEventHandler);
        }

        /// <summary>
        /// Called when the view value has changed.
        /// </summary>
        /// <param name="newValue">The view's new value.</param>
        //protected override void OnValueChanged(object newValue)
        //{
        //    OnValueChanged(newValue, PropertyChangeSource.Interactive);
        //}
        /// <summary>
        /// Called when the view value has changed.
        /// </summary>
        /// <param name="newValue">The view's new value.</param>
        /// <param name="source">The source of the value change.</param>
        //private void OnValueChanged(object newValue, PropertyChangeSource source)
        //{
        //    _channelControlViewModelImplementation.OnValueChanged(newValue, source);
        //}


        // FM_note: could be disabled if control is not composite
        /// <summary>
        /// Override resize behavior so the control cannot be resized.  This is done because with composite controls it is a lot of work to get all the individual components
        /// to scale reasonably with respect to each other
        /// </summary>
        public override ResizeDirections ResizeDirections
        {
            get
            {
                return ResizeDirections.All;
            }
        }

        private void ModCustomControlValueChangedEventHandler(object sender, ChannelValueChangedEventArgs e)
        {
            var attitudeIndicatorControl = View.Children.FirstOrDefault().AsFrameworkElement as AttitudeIndicatorControl;
            if (attitudeIndicatorControl != null)
            {
                //FM_note: CustomControlValue is defined in view.
                attitudeIndicatorControl.CustomControlValue = (double)e.ChannelValue;
            }
        }

        /// <summary>
        /// Gets or sets if model value changes should be suppressed on the view.
        /// </summary>
        internal bool SuppressValueChanges { get; set; }

        //protected override FrameworkElement CreateViewForType(Type numericType)
        //{

        //    var view = new modCustomControl(this);
        //    WeakEventManager<modCustomControl, CustomChannelValueChangedEventArgs>.AddHandler(view, "ValueChanged", SetChannelValue);
        //    return view;
        //}
        /// <summary>
        /// Creates the view associated with this view model by initializing a new instance of our custom control class modCustomControl
        /// This is an opportunity to provide callbacks to the view and to hook up event handlers.  In this case we add a value changed event handler so we can
        /// react when the view changes value.
        /// </summary>
        /// <returns>modCustomControl view</returns>
        public override object CreateView()
        {
            var view = new AttitudeIndicatorControl(this);
            WeakEventManager<AttitudeIndicatorControl, CustomChannelValueChangedEventArgs>.AddHandler(view, "ValueChanged", SetChannelValue);
            return view;
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
            //_channelControlViewModelImplementation.ModelPropertyChanged(modelElement, propertyName, transactionItem);
            ((AttitudeIndicatorControlModel)Model).PropertyChanged(modelElement, propertyName, transactionItem);
        }

        /// <summary>
        /// Applies a property value to the view.
        /// </summary>
        /// <param name="identifier">The property to set.</param>
        /// <param name="value">The new value for the property.</param>
        protected override void SetProperty(PropertySymbol identifier, object value)
        {
            bool handled = false;
            //handled = _channelControlViewModelImplementation.SetProperty(identifier, value);
            if (!handled)
            {
                base.SetProperty(identifier, value);
            }
        }
        /// <inheritdoc />
        //public override void Placed(PlacementReleaseEventArgs args)
        //{
        //    base.Placed(args);
        //    _channelControlViewModelImplementation.Placed(args);
        //}

        /// <summary>
        /// Gets the adorners used with this control during a hard selection (left-click).
        /// Currently used to create an adorner that allows browsing to a channel path in a VeriStand SDF.
        /// </summary>
        /// <returns>An enumerable collection of hard select adorners.</returns>
        //public override IEnumerable<Adorner> GetHardSelectAdorners()
        //{
        //    Collection<Adorner> adorners = _channelControlViewModelImplementation.GetHardSelectAdorners(base.GetHardSelectAdorners());
        //    return adorners;
        //}

        /// <inheritdoc />
        //public override IEnumerable<Adorner> GetSoftSelectAdorners()
        //{
        //    Collection<Adorner> adorners = _channelControlViewModelImplementation.GetSoftSelectAdorners(base.GetSoftSelectAdorners());
        //    return adorners;
        //}

        /// <inheritdoc />
        //public override QueryResult<T> QueryService<T>()
        //    => QueryResult<T>.FromObject(_channelControlViewModelImplementation)
        //        .AppendedWith(base.QueryService<T>());

        /// <summary>
        ///  Creates configuration pane content for this control. See comments on
        ///  <see cref="IProvideCommandContent"/> for more information about correct usage of this function.
        /// </summary>
        /// <param name="context">The current display context</param>

        public override void CreateCommandContent(ICommandPresentationContext context)
        {
            base.CreateCommandContent(context);
            //_channelControlViewModelImplementation.CreateCommandContent(context);
            // specify that we are adding things to the configuration pane
            using (context.AddConfigurationPaneContent())
            {
                // First add the group command which lets us know what top level configuration pane group to put the child commands in
                using (context.AddGroup(ConfigurationPaneCommands.BehaviorGroupCommand))
                {
                    // add child commands whose visuals will show up in the specified parent group.
                    context.Add(ControlChannelBrowseCommand);
                }
            }
        }

        /// <inheritdoc/>
        //protected override void OnCreateContextMenu(CreateContextMenuRoutedEventArgs args)
        //{
        //    base.OnCreateContextMenu(args);
        //    _channelControlViewModelImplementation.OnCreateContextMenu(args);
        //}

        /// <summary>
        /// Called when the model this instance was watching has gone to the detached state.
        /// </summary>
        /// <param name="modelElement">The removed model</param>
        /// <returns>Return true to have your observer removed from watching this object</returns>
        //public override bool ModelDetached(Element modelElement)
        //{
        //    _channelControlViewModelImplementation.ModelDetached(modelElement);
        //    return base.ModelDetached(modelElement);
        //}


        // Methods for ConfigurationPane for VisualModel
        private static ChannelPopup _uiSdfBrowsePopup;

        private static IEnumerable<IViewModel> _currentSelection;

        private static bool _settingValue;

        /// <summary>
        /// Command to launch the channel browser.
        /// </summary>
        public static readonly ISelectionCommand ControlChannelBrowseCommand = new ShellSelectionRelayCommand(LaunchControlChannelBrowser, CanLaunchChannelBrowser)
        {
            LabelTitle = "Custom User Control Channel",
            LargeImageSource = ResourceHelpers.LoadImage(typeof(AttitudeIndicatorControlViewModel), "Resources/Browse.png"),
            SmallImageSource = ResourceHelpers.LoadImage(typeof(AttitudeIndicatorControlViewModel), "Resources/Browse_16x16.png"),
            UniqueId = "NI.ChannelCommands:BrowseForCustomControlChannelCommand",
            UIType = UITypeForCommand.Button
        };

        /// <summary>
        /// Command to launch the channel browser.
        /// </summary>
        private static void LaunchControlChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            _settingValue = false;
            LaunchChannelBrowser(parameter, selection, host, site);
        }

        /// <summary>
        /// This can execute method is run periodically by the command to determine whether it should be enabled or disabled.
        /// </summary>
        private static bool CanLaunchChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            return selection.All(s => s.Model is AttitudeIndicatorControlModel);
        }

        private static void LaunchChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            _uiSdfBrowsePopup = new ChannelPopup
            {
                ShowWaveforms = false,
                Name = "UI_SDFBrowsePopup",
                IsOpen = false,
                Placement = Placement.BelowCenter
            };
            // Register the property changed callback for the popup window
            WeakEventManager<ChannelPopup, PropertyChangedEventArgs>.AddHandler(_uiSdfBrowsePopup, "PropertyChanged", ChannelNamePropertyChanged);
            // Show Popup window with Channels.
            _currentSelection = selection.ToList();
            _uiSdfBrowsePopup.PlacementTarget = (UIElement)parameter.AssociatedVisual;
            _uiSdfBrowsePopup.Channel = ((AttitudeIndicatorControlModel)_currentSelection.First().Model).modCustomControlChannel;
            _uiSdfBrowsePopup.ShowSdfBrowser(host, true, false);
        }
        private static void ChannelNamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChannelControlModelPropertyNames.ChannelName)
            {
                foreach (IViewModel selectedViewModel in _currentSelection)
                {
                    var uiModel = (UIModel)selectedViewModel.Model;
                    // we are setting values on the model so start a new transaction. set the purpose of the transaction to user so that it can be undone
                    using (var transaction = uiModel.TransactionManager.BeginTransaction("Set channel", TransactionPurpose.User))
                    {
                        var modCustomControlModel = uiModel as AttitudeIndicatorControlModel;
                        // loop on different channels
                        if (modCustomControlModel != null)
                        {
                            modCustomControlModel.modCustomControlChannel = _uiSdfBrowsePopup.Channel;
                        }
                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the adorners used with this control during a hard selection (left-click).
        /// Currently used to create an adorner that allows browsing to two channel paths
        /// </summary>
        /// <returns>An enumerable collection of hard select adorners.</returns>
        //public override IEnumerable<Adorner> GetHardSelectAdorners()
        //{
        //    var adorners = new Collection<Adorner>();
        //    var toolbar = new FloatingToolBar();

        //    if (Model == null) return adorners;
        //    var control = new TwoChannelAdorner(DesignerNodeHelpers.GetVisualForViewModel(this));
        //    toolbar.ToolBar = control;
        //    adorners.Add(new ControlAdorner(DesignerNodeHelpers.GetVisualForViewModel(this), toolbar, Placement.BelowCenter));
        //    return adorners;
        //}


        /// <summary>
        /// Creates and returns a list of context menu commands for this view model
        /// </summary>
        /// <returns>List of context menu commands for this view model</returns>
        public virtual IEnumerable<ShellCommandInstance> CreateContextMenuCommands()
        {
            var commands = new List<ShellCommandInstance>();
            commands.Add(
                new ShellCommandInstance(SelectChannelsCommand)
                {
                    LabelTitle = "Select Channels In Tree"
                });
            return commands;
        }

        /// <summary>
        /// Gets Unique IDs to be filtered from context menu commands
        /// </summary>
        public virtual IEnumerable<string> FilterContextMenuCommands()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Select the current control channel in the system definition tree
        /// </summary>
        public static readonly ShellSelectionRelayCommand SelectChannelsCommand = new ShellSelectionRelayCommand(SelectChannels, CanSelectChannel)
        {
            LabelTitle = "Select Channels",
            UniqueId = "NI.ChannelCommands:SelectChannelsInSystemDefinitionTree",
            UIType = UITypeForCommand.Button
        };

        private static bool CanSelectChannel(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            return host.GetSharedExportedValue<VeriStandHelper>().IsSystemDefinitionValid;
        }

        private static void SelectChannels(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            IEnumerable<string> controlChannels = selection.Select(item => item.Model)
                .OfType<AttitudeIndicatorControlModel>()
                .Select(model => model.modCustomControlChannel)
                .Where(channel => !string.IsNullOrEmpty(channel))
                .ToList();
            var systemDefinitionPalette = SystemDefinitionPaletteControl.Activate(site);
            systemDefinitionPalette.SelectNodes(controlChannels);
        }

        /// <summary>
        /// Called by the view when a value change occurs.  The view fires this for both duty cycle and frequency value changes and the event args let us
        /// tell which one was fired
        /// </summary>
        /// <param name="sender">sending object - not used</param>
        /// <param name="eventArgs">custom event information telling us which channel changed and what its value is</param>
        private void SetChannelValue(object sender, CustomChannelValueChangedEventArgs eventArgs)
        {
            ((AttitudeIndicatorControlModel)Model).SetChannelValue(eventArgs.ChannelName, (double)eventArgs.ChannelValue);
        }

        #region IChannelControlViewValueAccessor
        public bool CanAddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public void AddChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public void RemoveChildren(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public void UpdatePositions(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public void FinishPlacement(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Adorner> GetDragOverAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Adorner> GetBoundsChangeAdorners(IEnumerable<ReparentingInformation> infos)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region ICommonConfigurationPaneControl

        /// <inheritdoc/>
        public virtual bool HasTextContent => true;

        /// <summary>
        /// Gets or sets the current value displayed in the view.
        /// </summary>
        //public object ViewValue
        //{
        //    get
        //    {
        //        return Helper.GetValue();
        //    }
        //    set
        //    {
        //        Helper.SetValue(value);
        //    }
        //}

        public ReparentingStyle ReparentingStyle => throw new NotImplementedException();

        public SMPoint PastePositionOffset => throw new NotImplementedException();

        public bool ShowHighlightRect => throw new NotImplementedException();

        public SMRect SoftSelectionRectangle => throw new NotImplementedException();

        #endregion
    }
}
