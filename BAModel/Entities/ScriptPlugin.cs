using System;
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;

namespace BAModel
{
    public class ScriptPlugin : INotifyPropertyChanged
    {
        public string Name { get; set; }

        // this member is only used for tracking changes made during HTML site editing
        public string _originalName = String.Empty;
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
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

        public ScriptPlugin Clone()
        {
            ScriptPlugin scriptPlugin = new ScriptPlugin();
            scriptPlugin.Name = this.Name;
            scriptPlugin.Path = this.Path;

            return scriptPlugin;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            ScriptPlugin scriptPlugin = (ScriptPlugin)obj;

            return (
                (this.Name == scriptPlugin.Name) &&
                (this.Path == scriptPlugin.Path)
                );
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("scriptPlugin");
            writer.WriteElementString("name", Name);

            if (!publish)
            {
                writer.WriteElementString("path", Path);
            }

            writer.WriteFullEndElement(); // scriptPlugin
        }

        public static ScriptPlugin ReadXml(XmlReader reader)
        {
            ScriptPlugin scriptPlugin = new ScriptPlugin();

            string name = String.Empty;
            string path = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        name = reader.ReadString();
                        break;
                    case "path":
                        path = reader.ReadString();
                        break;
                }
            }

            scriptPlugin.Name = name;
            scriptPlugin.Path = path;

            return scriptPlugin;
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

    public class ScriptPluginSet
    {
        private List<ScriptPlugin> _scriptPlugins = new List<ScriptPlugin>();
        public List<ScriptPlugin> ScriptPlugins
        {
            get { return _scriptPlugins; }
        }

        public ScriptPlugin GetScriptPlugin(string scriptPluginName)
        {
            foreach (ScriptPlugin scriptPlugin in ScriptPlugins)
            {
                if (scriptPlugin.Name == scriptPluginName)
                {
                    return scriptPlugin;
                }
            }
            return null;
        }

        public ScriptPluginSet Clone()
        {
            ScriptPluginSet scriptPluginSet = new ScriptPluginSet();

            List<ScriptPlugin> scriptPlugins = this.ScriptPlugins;
            foreach (ScriptPlugin scriptPlugin in scriptPlugins)
            {
                scriptPluginSet.ScriptPlugins.Add(scriptPlugin.Clone());
            }

            return scriptPluginSet;
        }

        public bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            ScriptPluginSet scriptPluginSet = (ScriptPluginSet)obj;

            if (scriptPluginSet.ScriptPlugins.Count != this.ScriptPlugins.Count) return false;

            for (int i = 0; i < scriptPluginSet.ScriptPlugins.Count; i++)
            {
                if (!scriptPluginSet.ScriptPlugins[i].IsEqual(this.ScriptPlugins[i])) return false;
            }

            return true;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("scriptPlugins");

            foreach (ScriptPlugin scriptPlugin in _scriptPlugins)
            {
                scriptPlugin.WriteToXml(writer, publish);
            }

            writer.WriteFullEndElement(); // scriptPlugins
        }

        public static ScriptPluginSet ReadXml(XmlReader reader)
        {
            ScriptPluginSet scriptPluginSet = new ScriptPluginSet();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "scriptPlugin":
                        ScriptPlugin scriptPlugin = ScriptPlugin.ReadXml(reader);
                        scriptPluginSet.ScriptPlugins.Add(scriptPlugin);
                        break;
                }
            }

            return scriptPluginSet;
        }

        // after editing script plugins, check to see if the script plugin name has changed; if yes, return new value
        public string UpdateScriptPluginName(string originalScriptPluginName)
        {
            foreach (ScriptPlugin scriptPlugin in ScriptPlugins)
            {
                if (scriptPlugin.OriginalName == originalScriptPluginName)
                {
                    return scriptPlugin.Name;
                }
            }

            // error??
            return originalScriptPluginName;
        }
    }
}
