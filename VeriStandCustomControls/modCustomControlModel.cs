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

    [ExposeAttachedProperties(
    typeof(NumericChannelControlModelImplementation<modCustomControlModel>),
    PluginNamespaceSchema.ParsableNamespaceName)]
    public class modCustomControlModel : GaugeModel
    {
        private const string modCustomControlName = "modCustomControl";

        private readonly NumericChannelControlModelImplementation<modCustomControlModel> _channelControlModelImplementation;

        protected modCustomControlModel()
        {
            _channelControlModelImplementation = new NumericChannelControlModelImplementation<modCustomControlModel>(this);
        }
        /// <summary>
        /// Provide a xaml generation helper. This is used to 
        /// help generate xaml for the properties on this control.
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

        public override XName XmlElementName => XName.Get(modCustomControlName, PluginNamespaceSchema.ParsableNamespaceName);

        public override IEnumerable<ParsableNamespaceVersionCompatibility> ParsableNamespaceVersionCompatibility =>
            base.ParsableNamespaceVersionCompatibility.Concat(
                new ParsableNamespaceVersionCompatibility(ConfigurationBasedSoftwareNamespaceSchema.ParsableNamespaceName, NamespaceSchema.VersionZero)
                    .ToEnumerable());

        [XmlParserFactoryMethod(modCustomControlName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static modCustomControlModel CreateModCustomControlMdel(IElementCreateInfo info)
        {
            var model = new modCustomControlModel();
            model.Initialize(info);
            return model;
        }

        protected override void OnDeleting()
        {
            _channelControlModelImplementation.OnDeleting();
        }

        public override Type GetPropertyType(PropertySymbol identifier)
        {
            Type type;
            return _channelControlModelImplementation.GetPropertyType(identifier, out type) ? type : base.GetPropertyType(identifier);
        }

        public override object DefaultValue(PropertySymbol identifier)
        {
            object defaultValue;
            return _channelControlModelImplementation.GetDefaultValue(identifier, out defaultValue) ? defaultValue : base.DefaultValue(identifier);
        }

        public override string ToString()
        {
            IPanelLabel label = this.ToPanelControl().Label;
            return label == null ? base.ToString() : label.Text;
        }

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
    }
}
