using System.ComponentModel;
using System.Xml;
using System;
using System.Collections.Generic;

namespace BAModel
{
    public class ConditionalTarget : INotifyPropertyChanged
    {
        private string _variableName = String.Empty;
        private string _variableType = String.Empty;
        private string _operator = "EQ";
        private MediaState _targetMediaState = null;
        private string _targetMediaStateName = String.Empty;
        private bool _targetMediaStateIsPreviousState = false;
        private List<BrightSignCmd> _brightSignCmds = new List<BrightSignCmd>();

        public string VariableName
        {
            get { return _variableName; }
            set
            {
                _variableName = value;
                this.OnPropertyChanged("VariableName");
            }
        }

        public string VariableType
        {
            get { return _variableType; }
            set
            {
                _variableType = value;
            }
        }

        public string Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                this.OnPropertyChanged("Operator");
            }
        }

        private BSParameterValue _variableValue = new BSParameterValue();
        public BSParameterValue VariableValue
        {
            get { return _variableValue; }
            set
            {
                _variableValue = value;
                this.OnPropertyChanged("VariableValue");
            }
        }

        private BSParameterValue _variableValue2 = new BSParameterValue();
        public BSParameterValue VariableValue2
        {
            get { return _variableValue2; }
            set
            {
                _variableValue2 = value;
                this.OnPropertyChanged("VariableValue2");
            }
        }

        public MediaState TargetMediaState
        {
            get { return _targetMediaState; }
            set
            {
                _targetMediaState = value;
                this.OnPropertyChanged("TargetMediaState");
            }
        }

        public string TargetMediaStateName
        {
            get { return _targetMediaStateName; }
            set
            {
                _targetMediaStateName = value;
            }
        }

        public bool TargetMediaStateIsPreviousState
        {
            get { return _targetMediaStateIsPreviousState; }
            set
            {
                _targetMediaStateIsPreviousState = value;
                this.OnPropertyChanged("TargetMediaStateIsPreviousState");
            }
        }

        public List<BrightSignCmd> BrightSignCmds
        {
            get { return _brightSignCmds; }
            set { _brightSignCmds = value; }
        }

        public ConditionalTarget Clone()
        {
            ConditionalTarget conditionalTarget = new ConditionalTarget
            {
                VariableName = this.VariableName,
                VariableType = this.VariableType,
                Operator = this.Operator,
                VariableValue = this.VariableValue.Clone(),
                VariableValue2 = this.VariableValue2.Clone(),
                TargetMediaState = this.TargetMediaState,
                TargetMediaStateIsPreviousState = this.TargetMediaStateIsPreviousState
            };

            List<BrightSignCmd> brightSignCmds = new List<BrightSignCmd>();
            foreach (BrightSignCmd brightSignCmd in this.BrightSignCmds)
            {
                brightSignCmds.Add(brightSignCmd.Clone());
            }
            conditionalTarget.BrightSignCmds = brightSignCmds;

            return conditionalTarget;
        }

        public bool IsEqual(ConditionalTarget conditionalTarget)
        {
            if (VariableName != conditionalTarget.VariableName) return false;

            if (VariableType != conditionalTarget.VariableType) return false;

            if (Operator != conditionalTarget.Operator) return false;

            if (!VariableValue.IsEqual(conditionalTarget.VariableValue)) return false;

            if (!VariableValue2.IsEqual(conditionalTarget.VariableValue2)) return false;

            if (TargetMediaState != null && conditionalTarget.TargetMediaState != null)
            {
                if (TargetMediaState.Name != conditionalTarget.TargetMediaState.Name) return false;
            }
            else if (TargetMediaState != null || conditionalTarget.TargetMediaState != null)
            {
                return false;
            }

            if (TargetMediaStateIsPreviousState != conditionalTarget.TargetMediaStateIsPreviousState) return false;

            List<BrightSignCmd> thisBrightSignCmds = this.BrightSignCmds;
            List<BrightSignCmd> transitionBrightSignCmds = conditionalTarget.BrightSignCmds;

            if (thisBrightSignCmds.Count != transitionBrightSignCmds.Count) return false;

            int index = 0;
            foreach (BrightSignCmd bsc in thisBrightSignCmds)
            {
                if (!bsc.IsEqual(transitionBrightSignCmds[index])) return false;
                index++;
            }
            return true;
        }

        public static ConditionalTarget ReadXml(XmlReader reader)
        {
            BrightSignCmd brightSignCmd = null;

            ConditionalTarget conditionalTarget = new ConditionalTarget();
            conditionalTarget.TargetMediaStateIsPreviousState = false;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "variableName":
                        conditionalTarget.VariableName = reader.ReadString();
                        break;
                    case "variableType":
                        conditionalTarget.VariableType = reader.ReadString();
                        break;
                    case "operator":
                        conditionalTarget.Operator = reader.ReadString();
                        break;
                    case "variableValue":
                        string variableValue = reader.ReadString();
                        BSParameterValue pv = new BSParameterValue();
                        pv.SetTextValue(variableValue);
                        conditionalTarget.VariableValue = pv;
                        break;
                    case "variableValueSpec":
                        conditionalTarget.VariableValue = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                    case "variableValue2":
                        string variableValue2 = reader.ReadString();
                        BSParameterValue pv2 = new BSParameterValue();
                        pv2.SetTextValue(variableValue2);
                        conditionalTarget.VariableValue2 = pv2;
                        break;
                    case "variableValue2Spec":
                        conditionalTarget.VariableValue2 = BrightAuthorUtils.ReadBSParameterValue(reader);
                        break;
                    case "targetMediaState":
                        conditionalTarget.TargetMediaStateName = reader.ReadString();
                        break;
                    case "targetIsPreviousState":
                        string val = reader.ReadString();
                        if (val.ToLower() == "yes")
                        {
                            conditionalTarget.TargetMediaStateIsPreviousState = true;
                        }
                        break;
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        conditionalTarget.BrightSignCmds.Add(brightSignCmd);
                        break;
                }
            }

            return conditionalTarget;
        }

        public void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("conditionalTarget");

            writer.WriteElementString("variableName", VariableName);

            writer.WriteElementString("variableType", VariableType);

            writer.WriteElementString("operator", Operator);

            writer.WriteStartElement("variableValueSpec");
            VariableValue.WriteToXml(writer);
            writer.WriteFullEndElement(); // variableValueSpec

            writer.WriteStartElement("variableValue2Spec");
            VariableValue2.WriteToXml(writer);
            writer.WriteFullEndElement(); // variableValue2Spec

            if (TargetMediaState != null)
            {
                writer.WriteElementString("targetMediaState", TargetMediaState.Name);
            }
            else
            {
                writer.WriteElementString("targetMediaState", "");
            }

            if (TargetMediaStateIsPreviousState)
            {
                writer.WriteElementString("targetIsPreviousState", "yes");
            }

            foreach (BrightSignCmd brightSignCmd in BrightSignCmds)
            {
                brightSignCmd.WriteToXml(writer);
            }

            writer.WriteEndElement(); // conditionalTarget
        }

        public void ConvertAudioCommands(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
            foreach (BrightSignCmd brightSignCmd in BrightSignCmds)
            {
                brightSignCmd.ConvertAudioCommand(sign, zone, convertedBrightSignCmds);
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
}
