using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Linq;
using NationalInstruments.CBSCommon;
using NationalInstruments.Controls.SourceModel;
using NationalInstruments.Core;
using NationalInstruments.DynamicProperties;
using NationalInstruments.Hmi.Core.Screen;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.PanelCommon.SourceModel;
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
    public class AdgChannelNumericTextModelExporter : ICustomVeriStandControl
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
        public string Name => "Adg Channel Numeric TextBox";

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
        public string UniqueId => "AdgChannelNumericTextBox";

        /// <summary>
        /// Returns the palette hierarchy for this element. Returning null tells VeriStand to put this in the top level custom controls directory.
        /// </summary>
        public IList<PaletteElementCategory> PaletteHierarchy =>
            new List<PaletteElementCategory>() { new PaletteElementCategory("Custom controls", ImagePath, "adgCustomControls", .1) };
    }
    /// <summary>
    /// Model for NumericTextBoxModel controls that bind to channels.
    /// </summary>
    [ExposeAttachedProperties(typeof(NumericChannelControlModelImplementation<AdgChannelNumericTextModel>), PluginNamespaceSchema.ParsableNamespaceName)]
    public class AdgChannelNumericTextModel : NumericTextBoxModel, ICommonConfigurationPaneControl
    {
        private const string NumericTextBoxName = "modCustomControl2";

        private readonly NumericChannelControlModelImplementation<AdgChannelNumericTextModel> _channelControlModelImplementation;

        /// <summary>Initializes a new instance of the <see cref="AdgChannelNumericTexModel"/> class.</summary>
        protected AdgChannelNumericTextModel()
        {
            _channelControlModelImplementation = new NumericChannelControlModelImplementation<AdgChannelNumericTextModel>(this);
        }

        /// <inheritdoc/>
        public override XName XmlElementName => XName.Get(NumericTextBoxName, PluginNamespaceSchema.ParsableNamespaceName);

        /// <inheritdoc/>
        public override IEnumerable<ParsableNamespaceVersionCompatibility> ParsableNamespaceVersionCompatibility =>
            base.ParsableNamespaceVersionCompatibility.Concat(
                new ParsableNamespaceVersionCompatibility(PluginNamespaceSchema.ParsableNamespaceName, NamespaceSchema.VersionZero)
                    .ToEnumerable());

        /// <summary>Creates an <see cref="AdgChannelNumericTextModel"/>.</summary>
        /// <param name="info">An <see cref="IElementCreateInfo"/> instance.</param>
        /// <returns>An <see cref="AdgChannelNumericTextModel"/>.</returns>
        [XmlParserFactoryMethod(NumericTextBoxName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static AdgChannelNumericTextModel CreateAdgChannelNumericTextModel(IElementCreateInfo info)
        {
            var model = new AdgChannelNumericTextModel();
            model.Initialize(info);
            return model;
        }

        /// <summary>
        /// Get access to the <see cref="INumericChannelControlModel"/>
        /// </summary>
        public INumericChannelControlModel NumericChannelControlModel => _channelControlModelImplementation;

        /// <summary>
        /// Connected state, tracked for testing
        /// </summary>
        //internal bool Connected => _channelControlModelImplementation.Connected;

        /// <inheritdoc/>
        protected override void OnDeleting()
        {
            _channelControlModelImplementation.OnDeleting();
        }

        /// <inheritdoc/>
        public override Type GetPropertyType(PropertySymbol identifier)
        {
            Type type;
            return _channelControlModelImplementation.GetPropertyType(identifier, out type) ? type : base.GetPropertyType(identifier);
        }

        /// <summary>
        /// Gets the default value of the specified property.
        /// </summary>
        /// <param name="identifier">The property to get the default value of.</param>
        /// <returns>The default value of the specified property.</returns>
        public override object DefaultValue(PropertySymbol identifier)
        {
            object defaultValue;
            return _channelControlModelImplementation.GetDefaultValue(identifier, out defaultValue) ? defaultValue : base.DefaultValue(identifier);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            IPanelLabel label = this.ToPanelControl().Label;
            return label == null ? base.ToString() : label.Text;
        }

        #region QueryService

        /// <inheritdoc />
        public override QueryResult<T> QueryService<T>()
        {
            if (typeof(T) == typeof(ISearchable))
            {
                var searchableService = base.QueryService<ISearchable>().FirstOrDefault() ?? new DefaultSearchableService(this);
                _channelControlModelImplementation.UpdateISearchable(searchableService);
                return new QueryResult<T>(searchableService as T);
            }
            var result = QueryResult<T>.FromPair(null, _channelControlModelImplementation);
            if (result.Any())
            {
                return result;
            }
            return base.QueryService<T>();
        }

        #endregion QueryService

        #region ICommonConfigurationPaneControl

        /// <inheritdoc/>
        public virtual bool HasTextContent => true;

        #endregion ICommonConfigurationPaneControl
    }
}

