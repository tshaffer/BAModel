using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Windows;

namespace BAModel
{
    public class UserEvent : BSEvent
    {
        public string UserEventName { get; set; }
        public string DialogTitle { get; set; }
        public string DialogTitleI18NResource { get; set; }

        public UserEvent()
        {
        }

        public override void WriteBaseInfoToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("userEvent");
            writer.WriteElementString("name", UserEventName);
        }

        public override string ToString()
        {
            return Value;
        }

        public override string Description()
        {
            return Value;
        }

        public static UserEvent ReadXml(XmlReader reader)
        {
            UserEvent userEvent = null;

            string name = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();

                        switch (name)
                        {
                            case "mediaEnd":
                            case "timeout":
                            case "keyboard":
                            case "remote":
                            case "usb":
                            case "synchronize":
                            case "zoneMessage":
                            case "internalSynchronize":
                            case "quietUserEvent":
                            case "loudUserEvent":
                            case "success":
                            case "fail":
                                userEvent = SimpleUserEvent.ReadXml(reader, name);
                                break;
                            case "serial":
                                userEvent = TwoParameterUserEvent.ReadXml(reader, name);
                                break;
                            case "timeClockEvent":
                                userEvent = TimeClockEvent.ReadXml(reader);
                                break;
                            case "auxConnectUserEvent":
                                userEvent = AuxConnectUserEvent.ReadXml(reader);
                                break;
                            case "auxDisconnectUserEvent":
                                userEvent = AuxDisconnectUserEvent.ReadXml(reader);
                                break;
                            case "gpsEvent":
                                userEvent = GPSUserEvent.ReadXml(reader);
                                break;
                            case "audioTimeCodeEvent":
                                userEvent = AudioTimeCodeUserEvent.ReadXml(reader);
                                break;
                            case "videoTimeCodeEvent":
                                userEvent = VideoTimeCodeUserEvent.ReadXml(reader);
                                break;
                            case "rectangularTouchEvent":
                                userEvent = RectangularTouchUserEvent.ReadXml(reader);
                                break;
                            case "gpioUserEvent":
                                userEvent = GPIOUserEvent.ReadXml(reader);
                                break;
                            case "bp900UserEvent":
                                userEvent = BP900UserEvent.ReadXml(reader);
                                break;
                            case "bp900AUserEvent":
                            case "bp900BUserEvent":
                            case "bp900CUserEvent":
                            case "bp200AUserEvent":
                            case "bp200BUserEvent":
                            case "bp200CUserEvent":
                                userEvent = BPUserEvent.ReadXml(reader);
                                break;
                            case "interactiveMenuEnterEvent":
                                userEvent = new InteractiveMenuEnterEvent();
                                break;
                            case "pluginMessageEvent":
                                userEvent = PluginMessageEvent.ReadXml(reader);
                                break;
                            case "udp":
                                userEvent = UDPEvent.ReadXml(reader);
                                break;
                            /*
                            case "rectTouchEvent":
                                userEvent = RectangularTouchUserEvent.ReadXml(reader);
                                break;
                            */
                        }

                        break;
                }
            }

            return userEvent;
        }
    }
}
