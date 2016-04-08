using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class BP900UserEvent : UserEvent
    {
        public new static BPUserEvent ReadXml(XmlReader reader)
        {
            BPUserEvent bpUserEvent = null;

            string buttonNumber = String.Empty;
            InputControlConfiguration bpConfiguration = new InputControlPressConfiguration();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                    case "buttonNumber":
                        buttonNumber = reader.ReadString();
                        break;
                    case "press":
                        break;
                    case "pressContinuous":
                        bpConfiguration = InputControlPressContinuousConfiguration.ReadXml(reader);
                        break;
                }
            }

            bpUserEvent = new BPUserEvent(BPType.BP900, BPIndex.A);
            bpUserEvent.ButtonNumber = buttonNumber;
            bpUserEvent.BPConfiguration = bpConfiguration;

            return bpUserEvent;
        }
    }
}
