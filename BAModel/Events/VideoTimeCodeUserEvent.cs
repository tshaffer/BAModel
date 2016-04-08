using System;
using System.Collections.Generic;
using System.Xml;

namespace BAModel
{
    public class VideoTimeCodeUserEvent : TimeCodeUserEvent
    {
        public VideoTimeCodeUserEvent()
        {
            UserEventName = "videoTimeCodeEvent";
            Value = "Video Time Code";
            DialogTitle = "";
            ImageResourceName = "iconTimeCode";
            ImageResourceNameLarge = "iconTimeCodeLarge";
            ImageResourceSelectedName = "iconTimeCodeSelected";
            ValueI18NResource = "VideoTimeCodeEvent";

            toolTipId = "VideoTimeCodeEventTT";
        }

        public new static VideoTimeCodeUserEvent ReadXml(XmlReader reader)
        {
            TimedBrightSignCmd timedBrightSignCmd = null;

            VideoTimeCodeUserEvent vtcue = new VideoTimeCodeUserEvent();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "videoTimeCodeItem":
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
            VideoTimeCodeUserEvent tcue = new VideoTimeCodeUserEvent();

            CloneMembers(tcue);

            return tcue;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is VideoTimeCodeUserEvent)) return false;

            VideoTimeCodeUserEvent vtcue = bsEvent as VideoTimeCodeUserEvent;

            return MembersAreEqual(vtcue);
        }
    }
}
