using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class BSEvent
    {
        public string Value { get; set; }
        public string ValueI18NResource { get; set; }
        public string ImageResourceName { get; set; }
        public string ImageResourceSelectedName { get; set; }
        public string ImageResourceNameLarge { get; set; }

        public virtual Object Clone()
        {
            return null;
        }

        public virtual bool IsEqual(BSEvent bsEvent)
        {
            return false;
        }

        public virtual void GetIconResourceNames(ref string imageResourceName, ref string imageResourceSelectedName)
        {
            imageResourceName = ImageResourceName;
            imageResourceSelectedName = ImageResourceSelectedName;
        }

        public virtual string Description()
        {
            return String.Empty;
        }

        public virtual string GetToolTip()
        {
            return String.Empty;
        }

        public virtual void WriteBaseInfoToXml(XmlTextWriter writer, bool publish)
        {
        }

        public virtual void WriteToXml(XmlTextWriter writer, bool publish)
        {
        }

        public virtual void Publish(List<PublishFile> publishFiles)
        {
        }

        public virtual void FindBrokenMediaLinks(List<Object> brokenLinks)
        {
        }

        public virtual void ReplaceMediaFiles(Dictionary<string, string> replacementFiles)
        {
        }

        public virtual void ConvertAudioCommand(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
        }
    }
}
