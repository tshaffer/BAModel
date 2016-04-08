using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAModel
{
    public class BrightSignCommandMenuEntry
    {
        public string Name { get; set; }
        public string Label { get; set; }
        private List<BrightSignModel.ModelFeature> requiredFeatures = new List<BrightSignModel.ModelFeature>();
        public List<BrightSignModel.ModelFeature> RequiredFeatures
        {
            get { return requiredFeatures; }
        }
    }

    public class BrightSignCommandGroup : BrightSignCommandMenuEntry
    {
        private List<BrightSignCmd> _brightSignCommandSets = new List<BrightSignCmd>();
        public List<BrightSignCmd> BrightSignCommandSets
        {
            get { return _brightSignCommandSets; }
        }
    }

    public class BrightSignCommandSet : BrightSignCommandMenuEntry
    {
        public BrightSignCmd BrightSignCmd { get; set; }
    }
}
