using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
	public class SerialPortConfiguration
	{
		public enum ConnectedDeviceTypes
		{
			None,
			GPS
		}

		public string BaudRate { get; set; }
		public string DataBits { get; set; }
		public string Parity { get; set; }
		public string StopBits { get; set; }
		public string Protocol { get; set; }
		public string SendEol { get; set; }
		public string ReceiveEol { get; set; }
		public bool InvertSignals { get; set; }

		public ConnectedDeviceTypes ConnectedDevice { get; set; }

		public SerialPortConfiguration Copy()
		{
			SerialPortConfiguration serialPortConfiguration = new SerialPortConfiguration
			{
				BaudRate = this.BaudRate,
				DataBits = this.DataBits,
				Parity = this.Parity,
				StopBits = this.StopBits,
				Protocol = this.Protocol,
				SendEol = this.SendEol,
				ReceiveEol = this.ReceiveEol,
				InvertSignals = this.InvertSignals,
				ConnectedDevice = this.ConnectedDevice
			};

			return serialPortConfiguration;
		}

		public bool IsEqual(SerialPortConfiguration serialPortConfiguration)
		{
			return (this.BaudRate == serialPortConfiguration.BaudRate &&
				this.DataBits == serialPortConfiguration.DataBits &&
				this.Parity == serialPortConfiguration.Parity &&
				this.StopBits == serialPortConfiguration.StopBits &&
				this.Protocol == serialPortConfiguration.Protocol &&
				this.SendEol == serialPortConfiguration.SendEol &&
				this.ReceiveEol == serialPortConfiguration.ReceiveEol &&
				this.InvertSignals == serialPortConfiguration.InvertSignals &&
				this.ConnectedDevice == serialPortConfiguration.ConnectedDevice);
		}

		public void WriteToXml(XmlTextWriter writer, int serialPort)
		{
			writer.WriteStartElement("SerialPortConfiguration");
			writer.WriteElementString("port", serialPort.ToString());
			writer.WriteElementString("baudRate", BaudRate);
			writer.WriteElementString("dataBits", DataBits);
			writer.WriteElementString("parity", Parity);
			writer.WriteElementString("stopBits", StopBits);
			writer.WriteElementString("protocol", Protocol);
			writer.WriteElementString("sendEol", SendEol);
			writer.WriteElementString("receiveEol", ReceiveEol);
			writer.WriteElementString("invertSignals", InvertSignals.ToString());
			writer.WriteElementString("connectedDevice", ConnectedDevice.ToString());
			writer.WriteEndElement(); // serialPortConfiguration
		}

		public static SerialPortConfiguration ReadXml(XmlReader reader, out int port)
		{
			port = -1;

			SerialPortConfiguration serialPortConfiguration = new SerialPortConfiguration();
			serialPortConfiguration.Protocol = "ASCII";
			serialPortConfiguration.SendEol = "CR";
			serialPortConfiguration.ReceiveEol = "CR";
			serialPortConfiguration.InvertSignals = false;
			serialPortConfiguration.ConnectedDevice = ConnectedDeviceTypes.None;

			while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
			{
				switch (reader.LocalName)
				{
				case "port":
					port = Convert.ToInt32(reader.ReadString());
					break;
				case "baudRate":
					serialPortConfiguration.BaudRate = reader.ReadString();
					break;
				case "dataBits":
					serialPortConfiguration.DataBits = reader.ReadString();
					break;
				case "parity":
					serialPortConfiguration.Parity = reader.ReadString();
					break;
				case "stopBits":
					serialPortConfiguration.StopBits = reader.ReadString();
					break;
				case "protocol":
					serialPortConfiguration.Protocol = reader.ReadString();
					break;
				case "sendEol":
					serialPortConfiguration.SendEol = reader.ReadString();
					break;
				case "receiveEol":
					serialPortConfiguration.ReceiveEol = reader.ReadString();
					break;
				case "invertSignals":
					serialPortConfiguration.InvertSignals = Convert.ToBoolean(reader.ReadString());
					break;
				case "connectedDevice":
					string connectedDeviceSpec = reader.ReadString();
					serialPortConfiguration.ConnectedDevice = GetConnectedDevice(connectedDeviceSpec);
					break;
				}
			}

			return serialPortConfiguration;
		}

		private static ConnectedDeviceTypes GetConnectedDevice(string connectedDeviceSpec)
		{
			int index = 0;
			foreach (string connectedDevice in Enum.GetNames(typeof(ConnectedDeviceTypes)))
			{
				if (connectedDevice == connectedDeviceSpec)
				{
					Array serialHardwareValues = Enum.GetValues(typeof(ConnectedDeviceTypes));
					return (ConnectedDeviceTypes)serialHardwareValues.GetValue(index);
				}
				index++;
			}

			return ConnectedDeviceTypes.None;
		}
	}
}
