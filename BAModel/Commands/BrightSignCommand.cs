using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;

namespace BAModel
{
    public class BrightSignCommand : INotifyPropertyChanged
    {
        private string _command = "";
        private string _parameters = "";

        public string Value { get; set; }
        public int ValidationRules { get; set; }

        private bool _parameterRequired = false;

        public string Command
        {
            get { return _command; }
            set
            {
                _command = value;
                BrightSignCommand bscTemplate = BrightSignCommandMgr.GetBrightSignCommand(_command);
                if (bscTemplate == null) return;

                Value = bscTemplate.Value;
                Parameters = bscTemplate.Parameters;

                this.OnPropertyChanged("Command");
                this.OnPropertyChanged("Value");
                this.OnPropertyChanged("Parameters");
            }
        }

        public BrightSignCommand Clone() // ICloneable implementation
        {
            BrightSignCommand bsc = new BrightSignCommand();
            bsc.Command = this.Command;
            bsc.Parameters = this.Parameters;
            bsc.Value = this.Value;
            bsc.ValidationRules = this.ValidationRules;
            bsc.ParameterRequired = this.ParameterRequired;

            return bsc;
        }

        public string Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                this.OnPropertyChanged("Parameters");
            }
        }

        public bool ParameterRequired
        {
            get { return _parameterRequired; }
            set
            {
                _parameterRequired = value;
                this.OnPropertyChanged("ParameterRequired");
            }
        }

        public static BrightSignCommand ReadXml(XmlReader reader)
        {
            BrightSignCommand brightSignCommand = new BrightSignCommand();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "command":
                        brightSignCommand.Command = reader.ReadString();
                        break;
                    case "parameters":
                        brightSignCommand.Parameters = reader.ReadString();
                        break;
                }
            }

            BrightSignCommand bscTemplate = BrightSignCommandMgr.GetBrightSignCommand(brightSignCommand.Command);
            brightSignCommand.ParameterRequired = bscTemplate.ParameterRequired;

            return brightSignCommand;
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
}
