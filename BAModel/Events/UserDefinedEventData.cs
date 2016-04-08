using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;

namespace BAModel
{
    public class UserDefinedEventData
    {
        public static ObservableCollection<UserDefinedEvent> ReadUserDefinedEvents(string userDefinedEventsPath, bool getPriorVersion)
        {
            ObservableCollection<UserDefinedEvent> userDefinedEvents = new ObservableCollection<UserDefinedEvent>();

            FileStream fs = null;

            try
            {
                if (!File.Exists(userDefinedEventsPath) && getPriorVersion)
                {
                    userDefinedEventsPath = BrightAuthorUtils.GetPriorFileVersion("UserDefinedEvents.xml");
                }

                bool fileExists = userDefinedEventsPath != "";
                if (fileExists)
                {
                    fileExists = File.Exists(userDefinedEventsPath);
                }

                if (fileExists)
                {
                    fs = new FileStream(userDefinedEventsPath, FileMode.Open, FileAccess.Read);
                    XmlReader reader = new XmlTextReader(fs);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case "userDefinedEvents":
                                    userDefinedEvents = UserDefinedEvent.ReadUserDefinedEvents(reader);
                                    break;
                            }
                        }
                    }

                    fs.Close();
                }
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                //App.myTraceListener.Assert(false, ex.ToString());
            }

            return userDefinedEvents;
        }

        public static void WriteUserDefinedEvents(string userDefinedEventsPath, ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            XmlTextWriter writer = new XmlTextWriter(userDefinedEventsPath, System.Text.Encoding.UTF8);

            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteStartElement("BrightAuthorUserDefinedEvents");
            writer.WriteAttributeString("version", "1");

            WriteUserDefinedEventData(writer, userDefinedEvents);

            writer.WriteEndElement(); // BrightAuthorUserDefinedEvents

            writer.WriteEndDocument();

            writer.Close();
        }

        public static void WriteUserDefinedEventData(XmlTextWriter writer, ObservableCollection<UserDefinedEvent> userDefinedEvents)
        {
            writer.WriteStartElement("userDefinedEvents");

            foreach (UserDefinedEvent userDefinedEvent in userDefinedEvents)
            {
                userDefinedEvent.WriteAllToXml(writer);
            }

            writer.WriteFullEndElement(); // UserDefinedEvents
        }
    }
}
