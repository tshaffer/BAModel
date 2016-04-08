using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

namespace BAModel
{
    public class UserVariable : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }

        public bool Networked { get; set; }

        public string LiveDataFeedName { get; set; }

        private string _systemVariable = String.Empty;
        public string SystemVariable
        {
            get { return _systemVariable; }
            set { _systemVariable = value; }
        }

        public enum UserVariableAccess
        {
            Private,
            Shared
        }
        private UserVariableAccess _access = UserVariableAccess.Private;
        public UserVariableAccess Access
        {
            get { return _access; }
            set { _access = value; }
        }

        // this member is only used for tracking changes made during User Variable editing
        public string _originalName = String.Empty;
        public string OriginalName 
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        // this member is only used in Presentation Properties to indicate whether a user variable is in use or not - its value cannot be depended on outside of that dialog box
        private bool _inUse = false;
        public bool InUse
        {
            get { return _inUse; }
            set
            {
                _inUse = value;
                this.OnPropertyChanged("InUse");
            }
        }

        public string GetUserVariableUrlValue()
        {
            string url = String.Empty;

            LiveDataFeedSet liveDataFeedSet = Sign.CurrentSign.LiveDataFeedSet;

            LiveDataFeed liveDataFeed = liveDataFeedSet.GetLiveDataFeed(LiveDataFeedName);
            if (liveDataFeed != null)
            {
                url = liveDataFeed.GetUrlValue();
            }

            return url;
        }

        public UserVariable Clone()
        {
            UserVariable userVariable = new UserVariable();
            userVariable.Name = this.Name;
            userVariable.DefaultValue = this.DefaultValue;
            userVariable.Networked = this.Networked;
            userVariable.LiveDataFeedName = this.LiveDataFeedName;
            userVariable.SystemVariable = this.SystemVariable;
            userVariable.Access = this.Access;

            return userVariable;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            UserVariable userVariable = (UserVariable)obj;

            return (
                (this.Name == userVariable.Name) &&
                (this.DefaultValue == userVariable.DefaultValue) &&
                (this.Networked == userVariable.Networked) &&
                (this.LiveDataFeedName == userVariable.LiveDataFeedName) &&
                (this.SystemVariable == userVariable.SystemVariable) &&
                (this.Access == userVariable.Access)
                );
        }

        public void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("userVariable");
            writer.WriteElementString("name", Name);
            writer.WriteElementString("defaultValue", DefaultValue);
            writer.WriteElementString("networked", Networked.ToString());
            if (Networked)
            {
                writer.WriteElementString("liveDataFeedName", LiveDataFeedName);
            }
            else
            {
                writer.WriteElementString("liveDataFeedName", "");
            }
            writer.WriteElementString("systemVariable", SystemVariable);
            writer.WriteElementString("access", Access.ToString());
            writer.WriteFullEndElement(); // userVariable
        }

        public static UserVariable ReadXml(XmlReader reader)
        {
            UserVariable userVariable = new UserVariable();

            string name = String.Empty;
            string defaultValue = String.Empty;
            bool networked = false;
            string liveDataFeedName = String.Empty;
            string url = String.Empty;
            string systemVariable = String.Empty;
            UserVariableAccess access = UserVariableAccess.Private;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "defaultValue":
                        defaultValue = reader.ReadString();
                        break;
                    case "networked":
                        networked = Boolean.Parse(reader.ReadString());
                        break;
                    case "url": // compatibility - previous versions of text feeds included the live text data url
                    case "liveDataFeedName":
                        liveDataFeedName = reader.ReadString();
                        break;
                    case "systemVariable":
                        systemVariable = reader.ReadString();
                        break;
                    case "access":
                        access = (UserVariableAccess)Enum.Parse(typeof(UserVariableAccess), reader.ReadString());
                        break;
                }
            }

            userVariable.Name = name;
            userVariable.DefaultValue = defaultValue;
            userVariable.Networked = networked;
            userVariable.LiveDataFeedName = liveDataFeedName;
            userVariable.SystemVariable = systemVariable;
            userVariable.Access = access;

            return userVariable;
        }

        public void ResolveLiveDataFeed(LiveDataFeedSet liveDataFeedSet)
        {
            if (LiveDataFeedName != String.Empty)
            {
                LiveDataFeed liveDataFeed = liveDataFeedSet.GetLiveDataFeed(LiveDataFeedName);
                if (liveDataFeed == null)
                {
                    BSParameterValue bsParameterValue = new BSParameterValue();
                    bsParameterValue.SetTextValue(LiveDataFeedName);

                    liveDataFeed = new LiveDataFeed
                    {
                        Name = LiveDataFeedName,
                        Url = bsParameterValue,
                        LiveBSNDataFeed = null,
                        DataFeedUse = LiveDataFeed.DataFeedUsage.Text,
                        PluginFilePath = String.Empty,
                        ParserFunctionName = String.Empty,
                        UpdateInterval = UserPreferences.LiveTextRSSUpdateInterval
                    };
                    liveDataFeedSet.AddLiveDataFeed(liveDataFeed);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(
                    this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }

    public class UserVariableSet
    {
        private Dictionary<string, UserVariable> _userVariables = new Dictionary<string, UserVariable>();
        public Dictionary<string, UserVariable> UserVariables
        {
            get { return _userVariables; }
        }

        public UserVariableSet Clone()
        {
            UserVariableSet userVariableSet = new UserVariableSet();
            Dictionary<string, UserVariable> newUserVariables = userVariableSet.UserVariables;

            Dictionary<string, UserVariable> userVariables = this.UserVariables;
            foreach (KeyValuePair<string, UserVariable> kvp in userVariables)
            {
                newUserVariables.Add(kvp.Key, kvp.Value.Clone());
            }

            return userVariableSet;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            UserVariableSet userVariableSet = (UserVariableSet)obj;

            if (userVariableSet.UserVariables.Count != this.UserVariables.Count) return false;

            foreach (KeyValuePair<string, UserVariable> kvp in userVariableSet.UserVariables)
            {
                if (!this.UserVariables.ContainsKey(kvp.Key)) return false;
                if (!userVariableSet.UserVariables[kvp.Key].IsEqual(this.UserVariables[kvp.Key])) return false;
            }

            return true;
        }

        // after editing user variables, check to see if the user variable name has changed; if yes, return new value
        public string UpdateUserVariableName(string originalUserVariableName)
        {
            foreach (KeyValuePair<string, UserVariable> kvp in UserVariables)
            {
                UserVariable userVariable = kvp.Value;
                if (userVariable.OriginalName == originalUserVariableName)
                {
                    return userVariable.Name;
                }
            }

            // error??
            return originalUserVariableName;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish, bool autoCreateMediaCounterVariables)
        {
            writer.WriteStartElement("userVariables");

            foreach (KeyValuePair<string, UserVariable> kvp in UserVariables)
            {
                kvp.Value.WriteToXml(writer);
            }

            if (publish && autoCreateMediaCounterVariables)
            {
                // create user variables for all media items in the current presentation
                Dictionary<string, string> _allMediaFiles = Sign.CurrentSign.GetUniqueMediaFiles();
                foreach (KeyValuePair<string, string> kvp in _allMediaFiles)
                {
                    UserVariable userVariable = new UserVariable
                    {
                        Name = kvp.Key,
                        DefaultValue = "0",
                        Networked = false,
                        LiveDataFeedName = String.Empty,
                        SystemVariable = String.Empty
                    };
                    userVariable.WriteToXml(writer);
                }
            }

            writer.WriteFullEndElement(); // userVariables
        }

        public static UserVariableSet ReadXml(XmlReader reader)
        {
            UserVariableSet userVariableSet = new UserVariableSet();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userVariable":
                        UserVariable userVariable = UserVariable.ReadXml(reader);
                        userVariableSet.UserVariables.Add(userVariable.Name, userVariable);
                        break;
                }
            }

            return userVariableSet;
        }
    }
}
