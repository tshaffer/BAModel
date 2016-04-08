using System;
using System.Xml;

namespace BAModel
{
    public class AuxConnectUserEvent : AuxConnectDisconnectUserEvent
    {
        public AuxConnectUserEvent()
        {
            UserEventName = "auxConnectUserEvent";
            Value = "Aux Connect";
            DialogTitle = "Aux Connect";
            ImageResourceName = "iconAuxConnect";
            ImageResourceNameLarge = "iconAuxConnectLarge";
            ImageResourceSelectedName = "iconAuxConnectSelected";
            ValueI18NResource = "AuxConnectEvent";
        }

        public override string GetToolTip()
        {
            return "Aux Connect";
        }

        public new static AuxConnectUserEvent ReadXml(XmlReader reader)
        {
            AuxConnectUserEvent auxConnectUserEvent = new AuxConnectUserEvent();
            auxConnectUserEvent.AudioConnector = AudioConnectorType.Aux300Audio1;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "audioConnector":
                        string audioConnectorStr = reader.ReadString();
                        auxConnectUserEvent.AudioConnector = (AudioConnectorType)Enum.Parse(typeof(AudioConnectorType), audioConnectorStr);
                        break;
                }
            }

            return auxConnectUserEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            writer.WriteElementString("audioConnector", AudioConnector.ToString());

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            AuxConnectUserEvent auxConnectUserEvent = new AuxConnectUserEvent();
            auxConnectUserEvent.AudioConnector = this.AudioConnector;
            return auxConnectUserEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is AuxConnectUserEvent)) return false;

            return base.IsEqual(bsEvent);
        }

    }
}
