using System;
using System.Collections.Generic;
using System.Xml;

namespace BAModel
{
    public class AudioTimeCodeUserEvent : TimeCodeUserEvent
    {
        public AudioTimeCodeUserEvent()
        {
            UserEventName = "audioTimeCodeEvent";
            Value = "Audio Time Code";
            DialogTitle = "";
            ImageResourceName = "iconAudioTimeCode";
            ImageResourceNameLarge = "iconAudioTimeCodeLarge";
            ImageResourceSelectedName = "iconAudioTimeCodeSelected";
            ValueI18NResource = "AudioTimeCodeEvent";

            toolTipId = "AudioTimeCodeEventTT";
        }

        public new static AudioTimeCodeUserEvent ReadXml(XmlReader reader)
        {
            TimedBrightSignCmd timedBrightSignCmd = null;

            AudioTimeCodeUserEvent vtcue = new AudioTimeCodeUserEvent();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "audioTimeCodeItem":
                        timedBrightSignCmd = ReadTimeCodeCommandItemXml(reader);
                        vtcue.TimedBrightSignCmds.Add(timedBrightSignCmd);
                        break;
                    case "timedBrightSignCmd":
                        timedBrightSignCmd = TimedBrightSignCmd.ReadXml(reader);
                        vtcue.TimedBrightSignCmds.Add(timedBrightSignCmd);
                        break;
                }
            }
            return vtcue;
        }

        public override object Clone() // ICloneable implementation
        {
            AudioTimeCodeUserEvent tcue = new AudioTimeCodeUserEvent();

            CloneMembers(tcue);

            return tcue;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is AudioTimeCodeUserEvent)) return false;

            AudioTimeCodeUserEvent atcue = bsEvent as AudioTimeCodeUserEvent;

            return MembersAreEqual(atcue);
        }
    }
}
