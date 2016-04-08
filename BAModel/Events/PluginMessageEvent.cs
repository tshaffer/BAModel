using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace BAModel
{
    public class PluginMessageEvent : UserEvent
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public PluginMessageEvent()
        {
            UserEventName = "pluginMessageEvent";
            Value = "Plugin Message";
            DialogTitle = "";
            ImageResourceName = "iconPluginMessage";
            ImageResourceNameLarge = "iconPluginMessageLarge";
            ImageResourceSelectedName = "iconPluginMessageSelected";
            ValueI18NResource = "PluginMessageEvent";
        }

        public override string GetToolTip()
        {
            string msg = "Plugin Message";
            if (!String.IsNullOrEmpty(Message))
            {
                msg = msg + " " + Message;
            }
            return msg;
        }

        public override string Description()
        {
            return Value + ": pluginName = " + Name + ", message = " + Message;
        }

        public new static PluginMessageEvent ReadXml(XmlReader reader)
        {
            PluginMessageEvent pluginMessageEvent = new PluginMessageEvent();
            pluginMessageEvent.Name = String.Empty;
            pluginMessageEvent.Message = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "name":
                        pluginMessageEvent.Name = reader.ReadString();
                        break;
                    case "message":
                        pluginMessageEvent.Message = reader.ReadString();
                        break;
                }
            }

            return pluginMessageEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            writer.WriteElementString("name", Name);

            string messageToPublish = Message;

            if (publish)
            {
                // check for wildcard in message
                string wildcardDesignator = "<*>";
                int index = messageToPublish.IndexOf(wildcardDesignator);
                if (index >= 0)
                {
                    string sub1 = messageToPublish.Substring(0, index);
                    string sub2 = (messageToPublish.Length - wildcardDesignator.Length > index) ? messageToPublish.Substring(index + wildcardDesignator.Length) : String.Empty;
                    messageToPublish = Regex.Escape(sub1) + "(.*)" + Regex.Escape(sub2);
                }
            }

            writer.WriteElementString("message", messageToPublish);

            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            PluginMessageEvent pluginMessageEvent = new PluginMessageEvent();
            pluginMessageEvent.Name = this.Name;
            pluginMessageEvent.Message = this.Message;
            return pluginMessageEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is PluginMessageEvent)) return false;

            PluginMessageEvent pluginMessageEvent = (PluginMessageEvent)bsEvent;

            if ((pluginMessageEvent.Name != this.Name) ||
                (pluginMessageEvent.Message != this.Message))
            {
                return false;
            }

            return true;
        }

    }
}
