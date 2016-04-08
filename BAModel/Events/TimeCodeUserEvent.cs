using System;
using System.Collections.Generic;
using System.Xml;

namespace BAModel
{
    public class TimeCodeUserEvent : UserEvent
    {
        protected List<TimedBrightSignCmd> _timedBrightSignCmds = new List<TimedBrightSignCmd>();

        protected string toolTipId = String.Empty;

        public TimeCodeUserEvent()
        {
        }

        public List<TimedBrightSignCmd> TimedBrightSignCmds
        {
            get { return _timedBrightSignCmds; }
            set { _timedBrightSignCmds = value; }
        }

        public override string GetToolTip()
        {
            return BrightAuthorUtils.GetLocalizedString(toolTipId);
        }

        public static TimedBrightSignCmd ReadTimeCodeCommandItemXml(XmlReader reader)
        {
            string timeout = String.Empty;
            BrightSignCmd brightSignCmd = null;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "timeout":
                        timeout = reader.ReadString();
                        break;
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        break;
                    case "brightSignCommand":
                        BrightSignCommand brightSignCommand = BrightSignCommand.ReadXml(reader);
                        brightSignCmd = BrightSignCmd.FromBrightSignCommand(brightSignCommand);
                        break;
                }
            }

            TimedBrightSignCmd timedBrightSignCmd = new TimedBrightSignCmd(brightSignCmd, timeout);
            return timedBrightSignCmd;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            if (!publish)
            {
                base.WriteBaseInfoToXml(writer, publish);

                writer.WriteStartElement("parameters");

                foreach (TimedBrightSignCmd timedBrightSignCmd in TimedBrightSignCmds)
                {
                    timedBrightSignCmd.WriteToXml(writer);
                }

                writer.WriteFullEndElement(); // parameters

                // close out element that was written in base class - yuck!
                writer.WriteEndElement();
            }
        }

        public void CloneMembers(TimeCodeUserEvent tcue) // ICloneable implementation
        {
            tcue._timedBrightSignCmds = new List<TimedBrightSignCmd>();
            foreach (TimedBrightSignCmd timedBrightSignCmd in this.TimedBrightSignCmds)
            {
                tcue._timedBrightSignCmds.Add(timedBrightSignCmd.Clone());
            }
        }

        public bool MembersAreEqual(TimeCodeUserEvent tcue)
        {
            if (tcue.TimedBrightSignCmds.Count != this.TimedBrightSignCmds.Count) return false;
            for (int i = 0; i < TimedBrightSignCmds.Count; i++)
            {
                if (!this.TimedBrightSignCmds[i].IsEqual(tcue.TimedBrightSignCmds[i])) return false;
            }

            return true;
        }

        public override void ConvertAudioCommand(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
            foreach (TimedBrightSignCmd timedBrightSignCmd in TimedBrightSignCmds)
            {
                timedBrightSignCmd.ConvertAudioCommand(sign, zone, convertedBrightSignCmds);
            }
        }
    }
}
