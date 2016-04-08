using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace BAModel
{
    public class UDPEvent : UserEvent
    {
        public string Parameter { get; set; }
        public string Label { get; set; }
        public bool Export { get; set; }

        public UDPEvent()
        {
            UserEventName = "udp";
            Value = "UDP Input";
            DialogTitle = BrightAuthorUtils.GetLocalizedString("UDPEventDlg");
            ImageResourceName = "iconEthernet";
            ImageResourceNameLarge = "iconEthernetLarge";
            ImageResourceSelectedName = "iconEthernetSelected";
            ValueI18NResource = "UDPEvent";
        }

        public override string GetToolTip()
        {
            string tt = BrightAuthorUtils.GetLocalizedString(ValueI18NResource);
            string parameter = Parameter;
            tt += " " + parameter;            
            return tt;
        }

        public override string Description()
        {
            return Value + ": " + Parameter;
        }

        public new static UDPEvent ReadXml(XmlReader reader)
        {
            UDPEvent udpEvent = new UDPEvent();
            udpEvent.Parameter = String.Empty;
            udpEvent.Label = String.Empty;
            udpEvent.Export = true;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                        udpEvent.Parameter = reader.ReadString();
                        break;
                    case "label":
                        udpEvent.Label = reader.ReadString();
                        break;
                    case "export":
                        udpEvent.Export = UserPreferences.ConvertStringToBool(reader.ReadString());
                        break;
                }
            }
            // what if parameter is empty?
            if (udpEvent.Label == String.Empty) udpEvent.Label = udpEvent.Parameter;
            return udpEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            string wildcardDesignator = "<any>";
            string parameter = Parameter;
            if (publish)
            {
                int index = Parameter.IndexOf(wildcardDesignator);
                if (index >= 0)
                {
                    string sub1 = Parameter.Substring(0, index);
                    string sub2 = (Parameter.Length - wildcardDesignator.Length > index) ? Parameter.Substring(index + wildcardDesignator.Length) : String.Empty;
                    parameter = Regex.Escape(sub1) + "(.*)" + Regex.Escape(sub2);
                }
            }
            writer.WriteElementString("parameter", parameter);

            writer.WriteElementString("label", Label);
            writer.WriteElementString("export", UserPreferences.ConvertBoolToString(Export));

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            UDPEvent udpEvent = new UDPEvent();
            udpEvent.Parameter = Parameter;
            udpEvent.Label = Label;
            udpEvent.Export = Export;
            return udpEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is UDPEvent)) return false;

            UDPEvent udpEvent = (UDPEvent)bsEvent;

            if ((udpEvent.Parameter != this.Parameter) ||
                (udpEvent.Label != this.Label) ||
                (udpEvent.Export != this.Export))
            {
                return false;
            }

            return true;
        }
    }
}
