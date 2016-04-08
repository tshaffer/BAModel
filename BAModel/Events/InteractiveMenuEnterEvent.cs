using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class InteractiveMenuEnterEvent : UserEvent
    {
        public InteractiveMenuEnterEvent()
        {
            UserEventName = "interactiveMenuEnterEvent";
            Value = "Enter";
            DialogTitle = "";
            ImageResourceName = "iconInteractiveMenu";
            ImageResourceNameLarge = "iconRemoteLarge"; // unused
            ImageResourceSelectedName = "iconInteractiveMenuSelected";
        }

        public override object Clone()
        {
            return new InteractiveMenuEnterEvent();
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            return true;
        }

        public override string GetToolTip()
        {
            return BrightAuthorUtils.GetLocalizedString("MenuEnter");
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }
    }
}
