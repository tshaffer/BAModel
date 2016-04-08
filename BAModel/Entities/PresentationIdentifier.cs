using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace BAModel
{
    public class PresentationIdentifier : INotifyPropertyChanged
    {
        public string Name { get; set; }

        // this member is only used for tracking changes made during editing
        public string _originalName = String.Empty;
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        private string _presentationName = String.Empty;
        public string PresentationName
        {
            get { return _presentationName; }
            set { _presentationName = value; }
        }

        private string _path = String.Empty;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                this.OnPropertyChanged("Path");
            }
        }

        public PresentationIdentifier Clone()
        {
            PresentationIdentifier presentationIdentifier = new PresentationIdentifier();
            presentationIdentifier.Name = this.Name;
            presentationIdentifier.PresentationName = this.PresentationName;
            presentationIdentifier.Path = this.Path;

            return presentationIdentifier;
        }

        public virtual bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            PresentationIdentifier presentationIdentifier = (PresentationIdentifier)obj;

            return (this.Name == presentationIdentifier.Name) &&
                (this.PresentationName == presentationIdentifier.PresentationName) &&
                (this.Path == presentationIdentifier.Path);
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("presentationIdentifier");
            writer.WriteElementString("name", Name);
            writer.WriteElementString("presentationName", PresentationName);

            if (publish)
            {
                writer.WriteElementString("path", System.IO.Path.GetFileNameWithoutExtension(Path));
            }
            else
            {
                writer.WriteElementString("path", Path);
            }
            writer.WriteFullEndElement(); // presentationIdentifier
        }

        public static PresentationIdentifier ReadXml(XmlReader reader)
        {
            PresentationIdentifier presentationIdentifier = new PresentationIdentifier();

            string name = String.Empty;
            string presentationName = String.Empty;
            string path = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "presentationName":
                        presentationName = reader.ReadString();
                        break;
                    case "path":
                        path = reader.ReadString();
                        break;
                }
            }

            presentationIdentifier.Name = name;
            presentationIdentifier.PresentationName = presentationName;
            presentationIdentifier.Path = path;

            // for earlier 3.7 versions
            if (presentationName == String.Empty)
            {
                presentationIdentifier.PresentationName = System.IO.Path.GetFileNameWithoutExtension(path);

            }
            return presentationIdentifier;
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

    public class PresentationIdentifierSet
    {
        private Dictionary<string, PresentationIdentifier> _presentationIdentifiers = new Dictionary<string, PresentationIdentifier>();
        public Dictionary<string, PresentationIdentifier> PresentationIdentifiers
        {
            get { return _presentationIdentifiers; }
        }

        public PresentationIdentifierSet Clone()
        {
            PresentationIdentifierSet presentationIdentifierSet = new PresentationIdentifierSet();
            Dictionary<string, PresentationIdentifier> newPresentationIdentifiers = presentationIdentifierSet.PresentationIdentifiers;

            Dictionary<string, PresentationIdentifier> presentationIdentifiers = this.PresentationIdentifiers;
            foreach (KeyValuePair<string, PresentationIdentifier> kvp in presentationIdentifiers)
            {
                newPresentationIdentifiers.Add(kvp.Key, kvp.Value.Clone());
            }

            return presentationIdentifierSet;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            PresentationIdentifierSet presentationIdentifierSet = (PresentationIdentifierSet)obj;

            if (presentationIdentifierSet.PresentationIdentifiers.Count != this.PresentationIdentifiers.Count) return false;

            foreach (KeyValuePair<string, PresentationIdentifier> kvp in presentationIdentifierSet.PresentationIdentifiers)
            {
                if (!this.PresentationIdentifiers.ContainsKey(kvp.Key)) return false;
                if (!presentationIdentifierSet.PresentationIdentifiers[kvp.Key].IsEqual(this.PresentationIdentifiers[kvp.Key])) return false;
            }

            return true;
        }

        // after editing user variables, check to see if the user variable name has changed; if yes, return new value
        public string UpdatePresentationIdentifierName(string originalPresentationIdentifierName)
        {
            foreach (KeyValuePair<string, PresentationIdentifier> kvp in PresentationIdentifiers)
            {
                PresentationIdentifier presentationIdentifier = kvp.Value;
                if (presentationIdentifier.OriginalName == originalPresentationIdentifierName)
                {
                    return presentationIdentifier.Name;
                }
            }

            // error??
            return originalPresentationIdentifierName;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("presentationIdentifiers");

            foreach (KeyValuePair<string, PresentationIdentifier> kvp in PresentationIdentifiers)
            {
                kvp.Value.WriteToXml(writer, publish);
            }

            writer.WriteFullEndElement(); // presentationIdentifier
        }

        public static PresentationIdentifierSet ReadXml(XmlReader reader)
        {
            PresentationIdentifierSet presentationIdentifierSet = new PresentationIdentifierSet();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "presentationIdentifier":
                        PresentationIdentifier presentationIdentifier = PresentationIdentifier.ReadXml(reader);
                        presentationIdentifierSet.PresentationIdentifiers.Add(presentationIdentifier.Name, presentationIdentifier);
                        break;
                }
            }

            return presentationIdentifierSet;
        }
    }
}
