using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class BoseProduct
    {
        public enum BoseTapProtocol
        {
            Serial,
            CDC,
            HID
        }

        public string ProductName { get; set; }
        public string DisplayName { get; set; }
        public string ProductClass { get; set; }
        public string TransportType { get; set; }
        public string BaudRate { get; set; }
        public string DataBits { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public string FlowControl { get; set; }
        public string Protocol { get; set; }
        public string SendEol { get; set; }
        public string ReceiveEol { get; set; }
        public bool InvertSignals { get; set; }

        private string _usbAudioInterfaceIndex = "";
        public string USBAudioInterfaceIndex
        {
            get { return _usbAudioInterfaceIndex; }
            set { _usbAudioInterfaceIndex = value; }
        }

        private string _usbTapInterfaceIndex = "";
        public string USBTapInterfaceIndex
        {
            get { return _usbTapInterfaceIndex; }
            set { _usbTapInterfaceIndex = value; }
        }

        private BoseTapProtocol _tapProtocol = BoseTapProtocol.Serial;
        public BoseTapProtocol TapProtocol
        {
            get { return _tapProtocol; }
            set { _tapProtocol = value; }
        }

        public BoseProduct()
        {
        }

        public static BoseProduct ReadXml(XmlElement element)
        {
            string productName = String.Empty;
            string displayName = String.Empty;
            string productClass = String.Empty;
            string transportType = String.Empty;
            string baudRate = String.Empty;
            string dataBits = String.Empty;
            string parity = String.Empty;
            string stopBits = String.Empty;
            string flowControl = String.Empty;
            string protocol = String.Empty;
            string sendEol = "CR";
            string receiveEol = "CR";
            bool invertSignals = false;
            string usbAudioInterfaceIndex = "";
            string usbTapInterfaceIndex = "";
            BoseTapProtocol tapProtocol = BoseTapProtocol.Serial;

            XmlNodeList productNames = element.GetElementsByTagName("productName");
            if (productNames.Count == 1)
            {
                XmlElement productNameXML = productNames[0] as XmlElement;
                productName = productNameXML.InnerText;
            }

            XmlNodeList displayNames = element.GetElementsByTagName("displayName");
            if (displayNames.Count == 1)
            {
                XmlElement displayNameXML = displayNames[0] as XmlElement;
                displayName = displayNameXML.InnerText;
            }

            XmlNodeList productClasses = element.GetElementsByTagName("productClass");
            if (productClasses.Count == 1)
            {
                XmlElement productClassXML = productClasses[0] as XmlElement;
                productClass = productClassXML.InnerText;
            }

            XmlNodeList usbAudioInterfaceIndexList = element.GetElementsByTagName("usbAudioInterfaceIndex");
            if (usbAudioInterfaceIndexList.Count == 1)
            {
                XmlElement usbAudioInterfaceXML = usbAudioInterfaceIndexList[0] as XmlElement;
                usbAudioInterfaceIndex = usbAudioInterfaceXML.InnerText;
            }

            XmlNodeList usbTapInterfaceIndexList = element.GetElementsByTagName("usbTapInterfaceIndex");
            if (usbTapInterfaceIndexList.Count == 1)
            {
                XmlElement usbTapInterfaceIndexXML = usbTapInterfaceIndexList[0] as XmlElement;
                usbTapInterfaceIndex = usbTapInterfaceIndexXML.InnerText;
            }

            XmlNodeList tapProtocolList = element.GetElementsByTagName("tapProtocol");
            if (tapProtocolList.Count == 1)
            {
                XmlElement tapProtocolXML = tapProtocolList[0] as XmlElement;
                tapProtocol = (BoseTapProtocol)Enum.Parse(typeof(BoseTapProtocol), (string)tapProtocolXML.InnerText);
            }

            XmlNodeList transportList = element.GetElementsByTagName("transport");
            if (transportList.Count != 1)
            {
                return null;
            }

            XmlElement transportElement = transportList[0] as XmlElement;
            if (transportElement.Attributes.Count == 1)
            {
                transportType = transportElement.Attributes[0].Value;
                if (transportType == "Serial-ASCII")
                {
                    protocol = "ASCII";
                }
                else if (transportType == "Serial-Binary")
                {
                    protocol = "Binary";
                }
            }
            else
            {
                return null;
            }


            XmlNodeList transportItems = transportElement.ChildNodes;
            foreach (XmlElement transportItem in transportItems)
            {
                switch (transportItem.Name)
                {
                    case "baudRate":
                        baudRate = transportItem.InnerText;
                        break;
                    case "dataBits":
                        dataBits = transportItem.InnerText;
                        break;
                    case "parity":
                        parity = transportItem.InnerText;
                        break;
                    case "stopBits":
                        stopBits = transportItem.InnerText;
                        break;
                    case "flowControl":
                        flowControl = transportItem.InnerText;
                        break;
                    case "sendEOL":
                        sendEol = transportItem.InnerText;
                        break;
                    case "receiveEOL":
                        receiveEol = transportItem.InnerText;
                        break;
                    case "invertSignals":
                        invertSignals = Convert.ToBoolean(transportItem.InnerText);
                        break;
                    default:
                        break;
                }
            }

            BoseProduct boseProduct = new BoseProduct
            {
                ProductName = productName,
                DisplayName = displayName,
                ProductClass = productClass,
                TransportType = transportType,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                FlowControl = flowControl,
                Protocol = protocol,
                SendEol = sendEol,
                ReceiveEol = receiveEol,
                InvertSignals = invertSignals,
                TapProtocol = tapProtocol,
                USBAudioInterfaceIndex = usbAudioInterfaceIndex,
                USBTapInterfaceIndex = usbTapInterfaceIndex
            };

            return boseProduct;
        }

        public bool USBHIDCommunication()
        {
            return (_tapProtocol == BoseTapProtocol.HID);
        }
    }
}
