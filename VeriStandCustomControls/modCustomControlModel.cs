using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using NationalInstruments.CBSCommon;
using NationalInstruments.CommonModel;
using NationalInstruments.Controls.SourceModel;
using NationalInstruments.Core;
using NationalInstruments.DynamicProperties;
using NationalInstruments.PanelCommon.SourceModel;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Services;
using NationalInstruments.SourceModel;
using NationalInstruments.SourceModel.Persistence;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.SourceModel;
using NationalInstruments.VeriStand.SourceModel.Screen;
using NationalInstruments.VeriStand.SystemStorage;
using static NationalInstruments.Core.ExceptionHelper;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// This class is an implementation of ICustomVeriStandControl. By specifying it as an Export, we are identifying
    /// it as a plugin to VeriStand.
    /// The interface implementation defines how the control should appear in the palette.
    /// </summary>
    [Export(typeof(ICustomVeriStandControl))]
    public class modCustomControlModelExporter : ICustomVeriStandControl
    {
        /// <summary>
        /// MergeScript which defines what to drop on the screen from the palette.  Can be used to set default values on the control
        /// </summary>
        public string Target =>
            "<pf:MergeScript xmlns:pf=\"http://www.ni.com/PlatformFramework\">" +
                "<pf:MergeItem>" +
                    "<modCustomControl xmlns=\"http://www.your-company.com/VeriStandExample\" Width=\"[float]220\" Height=\"[float]175\"/>" +
                "</pf:MergeItem>" +
            "</pf:MergeScript>";

        /// <summary>
        /// Name of the control as it will appear in the palette
        /// </summary>
        public string Name => "Custom User Control (mod)";

        /// <summary>
        /// Path to the image to use in the palette
        /// </summary>
        public string ImagePath => "/NationalInstruments.VeriStand.CustomControlsExamples;component/Resources/ADG_32x32.png";

        /// <summary>
        /// Tool tip to display in the palette
        /// </summary>
        public string ToolTip => "Test user control.";

        /// <summary>
        /// Unique id for the control. The only requirement is that this doesn't overlap with existing controls or other custom controls.
        /// This is used for serialization and the context help system.
        /// </summary>
        public string UniqueId => "modCustomControl";

        /// <summary>
        /// Returns the palette hierarchy for this element. Returning null tells VeriStand to put this in the top level custom controls directory.
        /// </summary>
        public IList<PaletteElementCategory> PaletteHierarchy =>
            new List<PaletteElementCategory>() { new PaletteElementCategory("Custom controls", ImagePath, "adgCustomControls", .1) };
    }

    public class modCustomControlModel : VisualModel,
#if MUTATE2020R4
        IDataEngineStateChangeObserver
#else
        ISubscribeProviderStatusUpdates
#endif
    {
        /// <summary>
        /// The name to use for serialization of this model.  This name must match the name used in the Target xml in the ICustomVeriStandControl interface
        /// </summary>
        private const string modCustomControlName = "modCustomControl";
        /// <summary>
        /// String used to put errors from this control in their own bucket so code from this model doesn't interfere with the rest of the error
        /// reporting behavior in VeriStand
        /// </summary>
        private const string modCustomControlModelErrorString = "modCustomControlModelErrors";

        // Duplicate start for other channels
        /// <summary>
        /// Specifies the name of the frequency channel
        /// </summary>
        public const string modCustomControlChannelName = "modCustomControlChannel";

        /// <summary>
        /// Specifies the PropertySymbol for the first registered channel.  Any custom attribute that needs to serialized so that it is saved needs to be a property symbol.
        /// </summary>
        public static readonly PropertySymbol modCustomControlChannelSymbol = ExposePropertySymbol<modCustomControlModel>(modCustomControlChannelName, string.Empty);
        // Duplicate end
        // Xaml generation
        /// <summary>
        /// Provide a xaml generation helper. This is used to help generate xaml for the properties on this control.
        /// </summary>
        public override XamlGenerationHelper XamlGenerationHelper
        {
            get { return new modCustomControlXamlHelper(); }
        }
        /// <summary>
        /// Private class which helps with xaml generation for this model.  For most custom models this should just need to override the control type from the generic XamlGenerationHelper
        /// </summary>
        private class modCustomControlXamlHelper : XamlGenerationHelper
        {
            public override Type ControlType => typeof(modCustomControl);
        }
        /// <summary>
        /// XML element name, including full namespace, for universal persistence.
        /// </summary>
        public override XName XmlElementName
        {
            get { return XName.Get(modCustomControlName, PluginNamespaceSchema.ParsableNamespaceName); }
        }

        /// <summary>
        /// Factory method for creating a new modCustomControl
        /// </summary>
        /// <param name="info">Information required to create the model, such as the parser.</param>
        /// <returns>A constructed and initialized modCustomControlModel instance.</returns>

        [XmlParserFactoryMethod(modCustomControlName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static modCustomControlModel Create(IElementCreateInfo info)
        {
            var model = new modCustomControlModel();
#if MUTATE2020
            model.Initialize(info);
#else
            model.Init(info);
#endif
            return model;
        }

        /// <summary>
        /// Gets the type of the specified property.  This must be implemented for any new properties that get added that need to be serialized.
        /// </summary>
        /// <param name="identifier">The property to get the type of.</param>
        /// <returns>The exact runtime type of the specified property.</returns>

        public override Type GetPropertyType(PropertySymbol identifier)
        {
            switch (identifier.Name)
            {
                case modCustomControlChannelName:
                    return typeof(string);
                default:
                    return base.GetPropertyType(identifier);
            }

        }
        /// <summary>
        /// Gets the default value of the specified property.  This must be implemented for any new properties that get added that need to be serialized.
        /// </summary>
        /// <param name="identifier">The property to get the default value of.</param>
        /// <returns>The default value of the specified property.</returns>
        public override object DefaultValue(PropertySymbol identifier)
        {
            switch (identifier.Name)
            {
                case modCustomControlChannelName:
                    return string.Empty;
                default:
                    return base.DefaultValue(identifier);
            }
        }

        // INotifier must override

        /// <summary>
        ///   Called when VeriStand connects to the gateway. This control should register for the channel value change
        /// events it is interested in when this happens.
        /// </summary>
        /// <returns>Task which can be awaited</returns>
        public async Task OnConnectedAsync()
        {
            // use Host.BeginInvoke to clear error messages when connecting to the gateway.  The error message collection must be interacted with by the UI thread
            // which is why we must use BeginInvoke since OnConnectToGateway is not guaranteed to be called by the UI thread
            Host.BeginInvoke(AsyncTaskPriority.WorkerHigh, () =>
            {
                MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(modCustomControlModelErrorString, this);
            });
            if (!string.IsNullOrEmpty(modCustomControlChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().RegisterTagAsync(modCustomControlChannel, OnModCustomControlChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        private void ReportErrorToModel(Exception ex)
        {
            // Clear any existing errors and then report a new error message.  Use Host.BeginInvoke here since this must occur on the UI thread
            Host.BeginInvoke(AsyncTaskPriority.WorkerHigh, () =>
            {
                MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(
                    modCustomControlModelErrorString,
                    this);
#if MUTATE2021
                this.SafeReportError(PwmControlModelErrorString, null, MessageDescriptor.Empty, ex);
#else
                this.ReportError(modCustomControlModelErrorString, null, MessageDescriptor.Empty, ex);
#endif
            });
        }

        /// <summary>
        /// Called when VeriStand is connecting to the gateway.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task OnConnectingAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when VeriStand is disconnecting from the VeriStand gateway or the control is removed from the application.
        /// The control should unregister from channel value change events when this happens.
        /// </summary>
        /// <returns>Task which can be awaited</returns>

        public async Task OnDisconnectingAsync()
        {
            // use Host.BeginInvoke to clear error messages when connecting to the gateway.  The error message collection must be interacted with by the UI thread
            // which is why we must use BeginInvoke since OnConnectToGateway is not guaranteed to be called by the UI thread
            Host.BeginInvoke(
                AsyncTaskPriority.WorkerHigh,
                () =>
                    MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(
                        modCustomControlModelErrorString,
                        this));
            if (!string.IsNullOrEmpty(modCustomControlChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(modCustomControlChannel, OnModCustomControlChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        /// <summary>
        /// Called when VeriStand becomes disconnected from the gateway.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task OnDisconnectedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnStartAsync()
        {
            return Task.CompletedTask;
        }
        public Task OnShutdownAsync()
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// Called when the view model observes a change in a model property.
        /// Used to reflect model property changes on the VeriStand gateway (when necessary).
        /// </summary>
        /// <param name="modelElement">The model whose property changed. Unused by this method.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="transactionItem">The transaction item associated with the property change.</param>
        public void PropertyChanged(Element modelElement, string propertyName, TransactionItem transactionItem)
        {
            ScreenModel owningScreen = ScreenModel.GetScreen(this);
            // Loop on channel names, if multiple
            HandleChannelChangeAsync(transactionItem, owningScreen, OnModCustomControlChannelValueChange).IgnoreAwait();
        }

        /// <summary>
        /// This method is called when the channel itself is changed
        /// </summary>
        /// <param name="transactionItem">Information about the change in the model</param>
        /// <param name="owningScreen">Screen which owns this control</param>
        /// <param name="channelChangeHandler">The handler to use for registering with the gateway. Since we have two different channels
        /// we must make sure to provide the gateway with the correct handler</param>
        /// 
        private async Task HandleChannelChangeAsync(
            TransactionItem transactionItem,
            ScreenModel owningScreen,
            Action<ITagValue> channelChangeHandler)
        {
            var propertyExpressionTransactionItem =
                transactionItem as PropertyExpressionTransactionItem;
            // Lots of conditions where we ignore the channel change
            if (propertyExpressionTransactionItem == null
                || Host.ActiveRunTimeServiceProvider().Status != RunTimeProviderStatus.Connected
                || Equals(propertyExpressionTransactionItem.OldValue, propertyExpressionTransactionItem.NewValue)
                || owningScreen == null)
            {
                return;
            }

            // The oldValue can be thought of as what the channel value currently is and the newValue is what it will become.
            string oldValue, newValue;

            // transaction state being rolled back means that we are undoing something. for our purposes this reverses the logic for which item in the transaction is the old value and which is the new value
            if (transactionItem.State == TransactionState.RolledBack)
            {
                oldValue = propertyExpressionTransactionItem.NewValue as string;
                newValue = propertyExpressionTransactionItem.OldValue as string;
            }
            else
            {
                oldValue = propertyExpressionTransactionItem.OldValue as string;
                newValue = propertyExpressionTransactionItem.NewValue as string;
            }

            if (CheckForVectorChannels(!string.IsNullOrEmpty(newValue) ? newValue : oldValue))
            {
                return;
            }

            // Depending on whether there are values for old value and new value, either register, unregister, or rebind (which is unregister and register). Rebind is used to avoid
            // some race conditions with registration when the same control is being unregistered and re-registered to something else
            if (!string.IsNullOrEmpty(oldValue))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(oldValue, channelChangeHandler);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
            if (!string.IsNullOrEmpty(newValue))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(newValue, channelChangeHandler);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        /// <summary>
        /// Acquires a read lock on the entire model that the associated element is a part of.
        /// the model includes all elements which share a transaction manager.
        /// </summary>
        /// <returns>Disposable to dispose of to release the read lock.</returns>
        protected IDisposable AcquireReadLock()
        {
            return AcquireModelReadLock();
        }

        private bool CheckForVectorChannels(string channelName)
        {
            BaseNodeType node;
            Host.GetSharedExportedValue<VeriStandHelper>()
                .ActiveSystemDefinition.Root.BaseNodeType.FindNode(
                    StringArrayToNodePathUtils.NodePathToStringArray(channelName), out node);
            ChannelType channel = null;
            if (node is AliasType)
            {
                channel = (node as AliasType).ResolveAliasReference as ChannelType;
            }
            else if (node is ChannelType)
            {
                channel = node as ChannelType;
            }
            if (channel != null && !(channel.RowDim == 1 && channel.ColDim == 1))
            {
                ReportError(modCustomControlModelErrorString, null, MessageDescriptor.Empty, "Cannot Register Vector Channels");
                return true;
            }
            return false;
        }

        // Event handling
        /// <summary>
        /// Occurs when the model has been updated with a new channel value by the VeriStand gateway.
        /// </summary>
        public event EventHandler<ChannelValueChangedEventArgs> modCustomControlChannelValueChangedEvent;

        /// <summary>
        /// Raises the CustomControlChannelValueChangedEvent. Invoked when the channel value changes.
        /// </summary>
        protected virtual void OnModCustomControlChannelValueChangedEvent()
        {
            EventHandler<ChannelValueChangedEventArgs> channelValueChangeSubscribers = modCustomControlChannelValueChangedEvent;
            if (channelValueChangeSubscribers != null)
            {
                channelValueChangeSubscribers(this, new ChannelValueChangedEventArgs(modCustomControlChannelValue));
            }
        }

        /// <summary>
        /// These objects are used as owners for the gateway collator.  The gateway collators only stores one action per owner at a time and sends it to/receives from  the gateway periodically.   This
        /// is limit the rate at which things are sent/received from the gateway to avoid flooding the WCF pipe or falling behind in time.  Since this control has two buckets of things to collate against each other (frequency updates,
        /// and duty cycle updates, we need two owners to keep one controls updates from overwriting the others updates in the collator
        /// </summary>
        private readonly object _modCustomControlChannelCollatorOwner = new object();

        private void OnModCustomControlChannelValueChange(ITagValue value)
        {
            double newChannelValue = (double)value.Value;
            using (AcquireReadLock())
            {
                // The visual parent is null if the item is deleted, this is not null for models that are within a container
                // and are not directly the children of the screen surface.
                if (VisualParent == null)
                {
                    return;
                }
                ScreenModel screenModel = ScreenModel.GetScreen(this);
                // add an action to the collator.  the collator will limit the number of actions coming from the gateway and only
                // process the most recent action. This keeps us from falling behind in time if we can't process the gateway updates as fast as they are received.
                screenModel.FromGatewayActionCollator.AddAction(
                    _modCustomControlChannelCollatorOwner,
                    () =>
                    {
                        using (AcquireReadLock())
                        {
                            // The item could get deleted after the action has been dispatched.
                            if (VisualParent != null)
                            {
                                if (!Equals(modCustomControlChannelValue, newChannelValue))
                                {
                                    modCustomControlChannelValue = newChannelValue;
                                    OnModCustomControlChannelValueChangedEvent();
                                }
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Gets the modCustomControlChannelValue value
        /// </summary>
        public double modCustomControlChannelValue { get; protected set; }
        /// <summary>
        /// Gets or sets the path to the VeriStand channel associated with this control models duty cycle
        /// </summary>
        public string modCustomControlChannel
        {
            get { return ImmediateValueOrDefault<string>(modCustomControlChannelSymbol); }
            set { SetOrReplaceImmediateValue(modCustomControlChannelSymbol, value); }
        }
        public void SetChannelValue(string channelName, double channelValue)
        {
            // set the collator owner to be different for the different channel value change operations so a value change for one of the controls doesn't
            // erase the value change for the other one
            var collatorOwner = _modCustomControlChannelCollatorOwner;
            if (Host.ActiveRunTimeServiceProvider().Status == RunTimeProviderStatus.Connected)
            {
                ScreenModel screenModel = ScreenModel.GetScreen(this);
                if (screenModel != null)
                {
                    // Use the action collator to make sure we are not generating more set channel value calls than we can handle.
                    screenModel.ToGatewayActionCollator.AddAction(collatorOwner, async () =>
                    {
                        try
                        {
                            // set the channel value on the gateway, we are passing in empty labda expressions to the success and failure callbacks in this case
                            // if we wanted to report errors to the user we could add some handling code for the failure case
                            await Host.GetRunTimeService<ITagService>().SetTagValueAsync(modCustomControlChannel, TagFactory.CreateTag(channelValue));
                        }
                        catch (VeriStandException e)
                        {
                            Host.Dispatcher.InvokeIfNecessary(
                                PlatformDispatcher.AsyncOperationAlwaysValid,
#if MUTATE2021
                                () => this.SafeReportError(
#else
                                () => this.ReportError(
#endif
                                    modCustomControlModelErrorString,
                                    null,
                                    MessageDescriptor.Empty,
                                    e));
                        }
                    });
                }
            }
        }
    }
}
