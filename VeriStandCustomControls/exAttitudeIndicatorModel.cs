using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Xml.Linq;
using NationalInstruments.CBSCommon;
using NationalInstruments.CommonModel;
using NationalInstruments.Controls.SourceModel;
using NationalInstruments.Core;
using NationalInstruments.DynamicProperties;
using NationalInstruments.Controls.Internal;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Controls;
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
    public class exAttitudeIndicatorModelExporter : ICustomVeriStandControl
    {
        /// <summary>
        /// MergeScript which defines what to drop on the screen from the palette.  Can be used to set default values on the control
        /// </summary>
        public string Target =>
            "<pf:MergeScript xmlns:pf=\"http://www.ni.com/PlatformFramework\">" +
                "<pf:MergeItem>" +
                    "<exAttitudeIndicator xmlns=\"http://www.your-company.com/VeriStandExample\" Width=\"[float]220\" Height=\"[float]175\"/>" +
                "</pf:MergeItem>" +
            "</pf:MergeScript>";

        /// <summary>
        /// Name of the control as it will appear in the palette
        /// </summary>
        public string Name => "Attitude Indicator";

        /// <summary>
        /// Path to the image to use in the palette
        /// </summary>
        public string ImagePath => "/NationalInstruments.VeriStand.CustomControlsExamples;component/Resources/TestIcon_32x32.png";

        /// <summary>
        /// Tool tip to display in the palette
        /// </summary>
        public string ToolTip => "Attitude indicator exported from Industry control assembly.";

        /// <summary>
        /// Unique id for the control. The only requirement is that this doesn't overlap with existing controls or other custom controls.
        /// This is used for serialization and the context help system.
        /// </summary>
        public string UniqueId => "exAttitudeIndicator";

        /// <summary>
        /// Returns the palette hierarchy for this element. Returning null tells VeriStand to put this in the top level custom controls directory.
        /// </summary>
        public IList<PaletteElementCategory> PaletteHierarchy => null;
    }

    /// <summary>
    /// Model class which defines the business logic for the PWM Control.
    /// </summary>
    //public class exAttitudeIndicatorModel : AttitudeModel
    public class exAttitudeIndicatorModel : ChannelNumericTextBoxModel
    {
        /// <summary>
        /// The name to use for serialization of this model.  This name must match the name used in the Target xml in the ICustomVeriStandControl interface
        /// </summary>
        private const string exAttitudeIndicatorName = "exAttitudeIndicator";

        /// <summary>
        /// String used to put errors from this control in their own bucket so code from this model doesn't interfere with the rest of the error
        /// reporting behavior in VeriStand
        /// </summary>
        private const string exAttitudeIndicatorModelErrorString = "exAttitudeIndicatorModelErrors";

        /// <summary>
        /// XML element name, including full namespace, for universal persistence.
        /// </summary>
        public override XName XmlElementName
        {
            get { return XName.Get(exAttitudeIndicatorName, PluginNamespaceSchema.ParsableNamespaceName); }
        }

        /// <summary>
        /// Factory method for creating a new exAttitudeIndicator
        /// </summary>
        /// <param name="info">Information required to create the model, such as the parser.</param>
        /// <returns>A constructed and initialized PulseWidthModulationControlModel instance.</returns>
        [XmlParserFactoryMethod(exAttitudeIndicatorName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static exAttitudeIndicatorModel CreateShifterControlModel(IElementCreateInfo info)
        {
            var model = new exAttitudeIndicatorModel();
#if MUTATE2020
            model.Initialize(info);
#else
            model.Init(info);
#endif
            return model;
        }
    }
}