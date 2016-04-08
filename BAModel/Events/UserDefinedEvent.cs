using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;
//using System.Windows.Controls;

namespace BAModel
{
    public class UserDefinedEvent : BSEvent
    {
        public string _name = String.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Value = _name;
            }
        }

        public List<UserEvent> UserEvents { get; set; }

//        public RadioButton TBRB { get; set; }
//        public CheckBox TBCB { get; set; }

        public bool Validated { get; set; }

        // this member is only used for tracking changes made during User Variable editing
        public string _originalName = String.Empty;
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        public UserDefinedEvent()
        {
            Name = String.Empty;
            UserEvents = new List<UserEvent>();
            ImageResourceName = "iconUserEvent";
            ImageResourceNameLarge = "iconUserEventLarge";
            ImageResourceSelectedName = "iconUserEventSelected";
        }

        public override object Clone()
        {
            UserDefinedEvent userDefinedEvent = new UserDefinedEvent();
            userDefinedEvent.Name = this.Name;

            List<UserEvent> userEvents = userDefinedEvent.UserEvents;
            foreach (UserEvent userEvent in UserEvents)
            {
                userEvents.Add((UserEvent)userEvent.Clone());
            }
            return userDefinedEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is UserDefinedEvent)) return false;

            UserDefinedEvent userDefinedEvent = (UserDefinedEvent)bsEvent;

            if (userDefinedEvent.Name != this.Name) return false;

            if (userDefinedEvent.UserEvents.Count != this.UserEvents.Count) return false;
            for (int i = 0; i < userDefinedEvent.UserEvents.Count; i++)
            {
                if (!userDefinedEvent.UserEvents[i].IsEqual(this.UserEvents[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override string GetToolTip()
        {
            return Name;
        }

        public override string ToString()
        {
            return "UserDefinedEvent: " + Name;
        }

        public override string Description()
        {
            return "UserDefinedEvent: " + Name;
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            if (!publish)
            {
                writer.WriteStartElement("userDefinedEvent");
                writer.WriteAttributeString("name", Name);
                writer.WriteEndElement(); // UserDefinedEvent
            }
            else
            {
                foreach (UserEvent userEvent in UserEvents)
                {
                    userEvent.WriteToXml(writer, publish);
                }
            }
        }

        public void WriteAllToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("userDefinedEvent");
            writer.WriteAttributeString("name", Name);

            writer.WriteStartElement("userEvents");

            foreach (UserEvent userEvent in UserEvents)
            {
                userEvent.WriteToXml(writer, false);
            }

            writer.WriteEndElement(); // UserEvents

            writer.WriteEndElement(); // UserDefinedEvent
        }

        public static ObservableCollection<UserDefinedEvent> ReadUserDefinedEvents(XmlReader reader)
        {
            ObservableCollection<UserDefinedEvent> userDefinedEvents = new ObservableCollection<UserDefinedEvent>();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userDefinedEvent":
                        UserDefinedEvent userDefinedEvent = ReadXml(reader);
                        userDefinedEvents.Add(userDefinedEvent);
                        break;
                }
            }

            return userDefinedEvents;
        }

        public static UserDefinedEvent ReadXml(XmlReader reader)
        {
            UserDefinedEvent userDefinedEvent = new UserDefinedEvent();

            userDefinedEvent.Name = reader.GetAttribute("name");
            userDefinedEvent.UserEvents = new List<UserEvent>();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userEvents":
                        userDefinedEvent.UserEvents = ReadUserEventsXml(reader);
                        break;
                }
            }

            return userDefinedEvent;
        }

        private static List<UserEvent> ReadUserEventsXml(XmlReader reader)
        {
            List<UserEvent> userEvents = new List<UserEvent>();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userEvent":
                        UserEvent userEvent = UserEvent.ReadXml(reader);
                        userEvents.Add(userEvent);
                        break;
                }
            }

            return userEvents;
        }

        public static UserDefinedEvent MatchUserDefinedEvent(XmlReader reader)
        {
            string userDefinedEventName = reader.GetAttribute("name");

            ObservableCollection<UserDefinedEvent> userDefinedEvents = Sign.CurrentSign.UserDefinedEvents;
            foreach (UserDefinedEvent userDefinedEvent in userDefinedEvents)
            {
                if (userDefinedEvent.Name == userDefinedEventName)
                {
                    UserDefinedEvent matchedUserDefinedEvent = (UserDefinedEvent)userDefinedEvent.Clone();
                    matchedUserDefinedEvent.Validated = true;
                    return matchedUserDefinedEvent;
                }
            }

            // if no match was found and this sign does not contain user defined events, retrieve from global user defined events
            if (!Sign.CurrentSign.UserDefinedEventsFound)
            {
                ObservableCollection<UserDefinedEvent> globalUserDefinedEvents = Window1.GlobalUserDefinedEvents;

                // linear search to get matching user defined event. improve?
                foreach (UserDefinedEvent globalUserDefinedEvent in globalUserDefinedEvents)
                {
                    if (globalUserDefinedEvent.Name == userDefinedEventName)
                    {
                        // add to sign's user defined event list
                        userDefinedEvents.Add(globalUserDefinedEvent);

                        UserDefinedEvent matchedUserDefinedEvent = (UserDefinedEvent)globalUserDefinedEvent.Clone();
                        matchedUserDefinedEvent.Validated = true;
                        return matchedUserDefinedEvent;
                    }
                }
            }

            UserDefinedEvent invalidUserDefinedEvent = new UserDefinedEvent { Name = userDefinedEventName, Validated = false };
            return invalidUserDefinedEvent;
        }

        public bool IsValid()
        {
            return Validated;
        }
    }
}
