using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public enum BPType
    {
        BP900,
        BP200,
        GPIO
    }

    public enum BPIndex
    {
        A,
        B,
        C
    }

    public class BPUserEvent : UserEvent
    {
        private InputControlConfiguration _bpConfiguration = new InputControlPressConfiguration();
        public InputControlConfiguration BPConfiguration
        {
            get { return _bpConfiguration; }
            set { _bpConfiguration = value; }
        }

        public BPType BPType { get; set; }
        public BPIndex BPIndex { get; set; }
        public string ButtonNumber { get; set; }

        static string[,] _userEventNames = { { "bp900AUserEvent", "bp900BUserEvent", "bp900CUserEvent" }, { "bp200AUserEvent", "bp200BUserEvent", "bp200CUserEvent" } };
        static string[,] _values = { { "BP900A Event", "BP900B Event", "BP900C Event" }, { "BP200A Event", "BP200B Event", "BP200C Event" } };
        static string[,] _valueI18NResources = { { "BP900AEventTT", "BP900BEventTT", "BP900CEventTT" }, { "BP200AEventTT", "BP200BEventTT", "BP200CEventTT" } };
        static string[,] _imageResourceNames = { { "iconBP900A", "iconBP900B", "iconBP900C" }, { "iconBP200A", "iconBP200B", "iconBP200C" } };
        static string[,] _imageResourceNamesLarge = { { "iconBP900ALarge", "iconBP900BLarge", "iconBP900CLarge" }, { "iconBP200ALarge", "iconBP200BLarge", "iconBP200CLarge" } };
        static string[,] _imageResourceSelectedNames = { { "iconBP900ASelected", "iconBP900BSelected", "iconBP900CSelected" }, { "iconBP200ASelected", "iconBP200BSelected", "iconBP200CSelected" } };

        static string[,] _iconBPAny = { { "iconBP900AAny", "iconBP900BAny", "iconBP900CAny" }, { "iconBP200AAny", "iconBP200BAny", "iconBP200CAny" } };
        static string[,] _iconBPAnySelected = { { "iconBP900ASelectedAny", "iconBP900BSelectedAny", "iconBP900CSelectedAny" }, { "iconBP200ASelectedAny", "iconBP200BSelectedAny", "iconBP200CSelectedAny" } };

        static string[,] _iconBP0 = { { "iconBP900A1", "iconBP900B1", "iconBP900C1" }, { "iconBP200A1", "iconBP200B1", "iconBP200C1" } };
        static string[,] _iconBP0Selected = { { "iconBP900ASelected1", "iconBP900BSelected1", "iconBP900CSelected1" }, { "iconBP200ASelected1", "iconBP200BSelected1", "iconBP200CSelected1" } };

        static string[,] _iconBP1 = { { "iconBP900A2", "iconBP900B2", "iconBP900C2" }, { "iconBP200A2", "iconBP200B2", "iconBP200C2" } };
        static string[,] _iconBP1Selected = { { "iconBP900ASelected2", "iconBP900BSelected2", "iconBP900CSelected2" }, { "iconBP200ASelected2", "iconBP200BSelected2", "iconBP200CSelected2" } };

        static string[,] _iconBP2 = { { "iconBP900A3", "iconBP900B3", "iconBP900C3" }, { "iconBP200A3", "iconBP200B3", "iconBP200C3" } };
        static string[,] _iconBP2Selected = { { "iconBP900ASelected3", "iconBP900BSelected3", "iconBP900CSelected3" }, { "iconBP200ASelected3", "iconBP200BSelected3", "iconBP200CSelected3" } };

        static string[,] _iconBP3 = { { "iconBP900A4", "iconBP900B4", "iconBP900C4" }, { "iconBP200A4", "iconBP200B4", "iconBP200C4" } };
        static string[,] _iconBP3Selected = { { "iconBP900ASelected4", "iconBP900BSelected4", "iconBP900CSelected4" }, { "iconBP200ASelected4", "iconBP200BSelected4", "iconBP200CSelected4" } };

        static string[,] _iconBP4 = { { "iconBP900A5", "iconBP900B5", "iconBP900C5" }, { "iconBP200A5", "iconBP200B5", "iconBP200C5" } };
        static string[,] _iconBP4Selected = { { "iconBP900ASelected5", "iconBP900BSelected5", "iconBP900CSelected5" }, { "iconBP200ASelected5", "iconBP200BSelected5", "iconBP200CSelected5" } };

        static string[,] _iconBP5 = { { "iconBP900A6", "iconBP900B6", "iconBP900C6" }, { "iconBP200A6", "iconBP200B6", "iconBP200C6" } };
        static string[,] _iconBP5Selected = { { "iconBP900ASelected6", "iconBP900BSelected6", "iconBP900CSelected6" }, { "iconBP200ASelected6", "iconBP200BSelected6", "iconBP200CSelected6" } };

        static string[,] _iconBP6 = { { "iconBP900A7", "iconBP900B7", "iconBP900C7" }, { "iconBP200A7", "iconBP200B7", "iconBP200C7" } };
        static string[,] _iconBP6Selected = { { "iconBP900ASelected7", "iconBP900BSelected7", "iconBP900CSelected7" }, { "iconBP200ASelected7", "iconBP200BSelected7", "iconBP200CSelected7" } };

        static string[,] _iconBP7 = { { "iconBP900A8", "iconBP900B8", "iconBP900C8" }, { "iconBP200A8", "iconBP200B8", "iconBP200C8" } };
        static string[,] _iconBP7Selected = { { "iconBP900ASelected8", "iconBP900BSelected8", "iconBP900CSelected8" }, { "iconBP200ASelected8", "iconBP200BSelected8", "iconBP200CSelected8" } };

        static string[,] _iconBP8 = { { "iconBP900A9", "iconBP900B9", "iconBP900C9" }, { "iconBP200A9", "iconBP200B9", "iconBP200C9" } };
        static string[,] _iconBP8Selected = { { "iconBP900ASelected9", "iconBP900BSelected9", "iconBP900CSelected9" }, { "iconBP200ASelected9", "iconBP200BSelected9", "iconBP200CSelected9" } };

        static string[,] _iconBP9 = { { "iconBP900A10", "iconBP900B10", "iconBP900C10" }, { "iconBP200A10", "iconBP200B10", "iconBP200C10" } };
        static string[,] _iconBP9Selected = { { "iconBP900ASelected10", "iconBP900BSelected10", "iconBP900CSelected10" }, { "iconBP200ASelected10", "iconBP200BSelected10", "iconBP200CSelected10" } };

        static string[,] _iconBP10 = { { "iconBP900A11", "iconBP900B11", "iconBP900C11" }, { "iconBP200A11", "iconBP200B11", "iconBP200C11" } };
        static string[,] _iconBP10Selected = { { "iconBP900ASelected11", "iconBP900BSelected11", "iconBP900CSelected11" }, { "iconBP200ASelected11", "iconBP200BSelected11", "iconBP200CSelected11" } };

        public BPUserEvent(BPType bpType, BPIndex bpIndex)
        {
            BPType = bpType;
            BPIndex = bpIndex;

            UserEventName = _userEventNames[(int)bpType, (int)bpIndex];
            Value = _values[(int)bpType, (int)bpIndex];
            ValueI18NResource = _valueI18NResources[(int)bpType, (int)bpIndex];
            DialogTitle = Value;

            ImageResourceName = _imageResourceNames[(int)bpType, (int)bpIndex];
            ImageResourceNameLarge = _imageResourceNamesLarge[(int)bpType, (int)bpIndex];
            ImageResourceSelectedName = _imageResourceSelectedNames[(int)bpType, (int)bpIndex];

            ButtonNumber = "0";
        }

        public override object Clone()
        {
            BPUserEvent bpUserEvent = new BPUserEvent(BPType, BPIndex);
            bpUserEvent.ButtonNumber = this.ButtonNumber;
            bpUserEvent.BPConfiguration = (InputControlConfiguration)this.BPConfiguration.Clone();
            return bpUserEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is BPUserEvent)) return false;

            BPUserEvent bpUserEvent = (BPUserEvent)bsEvent;

            if (this.BPIndex != bpUserEvent.BPIndex) return false;

            // compare _bpConfiguration
            if (!this.BPConfiguration.IsEqual(bpUserEvent.BPConfiguration)) return false;

            return bpUserEvent.ButtonNumber == this.ButtonNumber;
        }

        public override string Description()
        {
            return Value + ": " + BPType.ToString() + BPIndex.ToString() + ":" + ButtonNumber;
        }

        public override string GetToolTip()
        {
            string tt = BrightAuthorUtils.GetLocalizedString(ValueI18NResource);
            string ttLabel;

            if (ButtonNumber == "-1")
            {
                ttLabel = BrightAuthorUtils.GetLocalizedString("BPAnyButton");
            }
            else
            {
                int buttonNumber = Convert.ToInt16(ButtonNumber);
                buttonNumber++;
                ttLabel = buttonNumber.ToString();
            }

            tt += " " + ttLabel;
            return tt;
        }

        public override void GetIconResourceNames(ref string imageResourceName, ref string imageResourceSelectedName)
        {
            switch (ButtonNumber)
            {
                case "-1":
                    imageResourceName = _iconBPAny[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBPAnySelected[(int)BPType, (int)BPIndex];
                    break;
                case "0":
                    imageResourceName = _iconBP0[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP0Selected[(int)BPType, (int)BPIndex];
                    break;
                case "1":
                    imageResourceName = _iconBP1[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP1Selected[(int)BPType, (int)BPIndex];
                    break;
                case "2":
                    imageResourceName = _iconBP2[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP2Selected[(int)BPType, (int)BPIndex];
                    break;
                case "3":
                    imageResourceName = _iconBP3[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP3Selected[(int)BPType, (int)BPIndex];
                    break;
                case "4":
                    imageResourceName = _iconBP4[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP4Selected[(int)BPType, (int)BPIndex];
                    break;
                case "5":
                    imageResourceName = _iconBP5[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP5Selected[(int)BPType, (int)BPIndex];
                    break;
                case "6":
                    imageResourceName = _iconBP6[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP6Selected[(int)BPType, (int)BPIndex];
                    break;
                case "7":
                    imageResourceName = _iconBP7[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP7Selected[(int)BPType, (int)BPIndex];
                    break;
                case "8":
                    imageResourceName = _iconBP8[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP8Selected[(int)BPType, (int)BPIndex];
                    break;
                case "9":
                    imageResourceName = _iconBP9[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP9Selected[(int)BPType, (int)BPIndex];
                    break;
                case "10":
                    imageResourceName = _iconBP10[(int)BPType, (int)BPIndex];
                    imageResourceSelectedName = _iconBP10Selected[(int)BPType, (int)BPIndex];
                    break;
            }
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            switch (BPType)
            {
                case BPType.BP200:
                    writer.WriteElementString("buttonPanelType", "BP200");
                    break;
                case BPType.BP900:
                    writer.WriteElementString("buttonPanelType", "BP900");
                    break;
            }

            writer.WriteElementString("buttonPanelIndex", ((int)BPIndex).ToString());

            writer.WriteElementString("buttonNumber", ButtonNumber);

            if (_bpConfiguration != null)
            {
                _bpConfiguration.WriteToXml(writer, publish);
            }
            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public new static BPUserEvent ReadXml(XmlReader reader)
        {
            BPUserEvent bpUserEvent = null;

            BPType bpType = BPType.BP900;
            BPIndex bpIndex = BPIndex.A;

            string buttonNumber = String.Empty;
            InputControlConfiguration bpConfiguration = new InputControlPressConfiguration();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                    case "buttonPanelType":
                        string buttonPanelType = reader.ReadString();
                        switch (buttonPanelType)
                        {
                            case "BP200":
                                bpType = BPType.BP200;
                                break;
                            case "BP900":
                                bpType = BPType.BP900;
                                break;
                        }
                        break;
                    case "buttonPanelIndex":
                        string buttonPanelIndex = reader.ReadString();
                        switch (buttonPanelIndex)
                        {
                            case "0":
                                bpIndex = BPIndex.A;
                                break;
                            case "1":
                                bpIndex = BPIndex.B;
                                break;
                            case "2":
                                bpIndex = BPIndex.C;
                                break;
                        }
                        break;
                    case "buttonNumber":
                        buttonNumber = reader.ReadString();
                        break;
                    case "press":
                        break;
                    case "pressContinuous":
                        bpConfiguration = InputControlPressContinuousConfiguration.ReadXml(reader);
                        break;
                }
            }

            bpUserEvent = new BPUserEvent(bpType, bpIndex);
            bpUserEvent.ButtonNumber = buttonNumber;
            bpUserEvent.BPConfiguration = bpConfiguration;

            return bpUserEvent;
        }

    }

    public class InputControlConfiguration
    {
        public virtual void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
        }

        public virtual Object Clone()
        {
            return null;
        }

        public virtual bool IsEqual(InputControlConfiguration bpConfiguration)
        {
            return false;
        }
    }

    public class InputControlPressConfiguration : InputControlConfiguration
    {
        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("press");
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            InputControlPressConfiguration bpPressConfiguration = new InputControlPressConfiguration();
            return bpPressConfiguration;
        }

        public override bool IsEqual(InputControlConfiguration bpConfiguration)
        {
            if (!(bpConfiguration is InputControlPressConfiguration)) return false;

            return true;
        }
    }

    public class InputControlPressContinuousConfiguration : InputControlConfiguration
    {
        public string RepeatInterval { get; set; }

        public static InputControlPressContinuousConfiguration ReadXml(XmlReader reader)
        {
            string repeatInterval = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "repeatInterval":
                        repeatInterval = reader.ReadString();
                        break;
                    case "initialHoldoff":
                        string initialHoldoff = reader.ReadString();
                        break;
                }
            }

            InputControlPressContinuousConfiguration bpPressContinuousConfiguration = new InputControlPressContinuousConfiguration();
            bpPressContinuousConfiguration.RepeatInterval = repeatInterval;

            return bpPressContinuousConfiguration;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("pressContinuous");
            writer.WriteElementString("repeatInterval", RepeatInterval);
            writer.WriteElementString("initialHoldoff", RepeatInterval);
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            InputControlPressContinuousConfiguration bpPressContinuousConfiguration = new InputControlPressContinuousConfiguration();
            bpPressContinuousConfiguration.RepeatInterval = this.RepeatInterval;
            return bpPressContinuousConfiguration;
        }

        public override bool IsEqual(InputControlConfiguration bpConfiguration)
        {
            if (!(bpConfiguration is InputControlPressContinuousConfiguration)) return false;

            InputControlPressContinuousConfiguration bpPressContinuousConfiguration = (InputControlPressContinuousConfiguration)bpConfiguration;
            return bpPressContinuousConfiguration.RepeatInterval == this.RepeatInterval;
        }
    }

    public class InputControlPressAndHoldConfiguration : InputControlConfiguration
    {
        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
        }
    }

    public class InputControlPressMultipleConfiguration : InputControlConfiguration
    {
        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
        }
    }
}
