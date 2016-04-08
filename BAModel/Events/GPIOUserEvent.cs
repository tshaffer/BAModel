using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class GPIOUserEvent : UserEvent
    {
        public string ButtonNumber { get; set; }

        public string ButtonDirection { get; set; }

        private InputControlConfiguration _inputConfiguration = new InputControlPressConfiguration();
        public InputControlConfiguration InputConfiguration
        {
            get { return _inputConfiguration; }
            set { _inputConfiguration = value; }
        }

        public GPIOUserEvent()
        {
            ButtonDirection = "down";
            UserEventName = "gpioUserEvent";
            Value = "GPIO Input";
            DialogTitle = "GPIO Event";
            ImageResourceName = "iconButton";
            ImageResourceNameLarge = "iconButtonLarge";
            ImageResourceSelectedName = "iconButtonSelected";
            ValueI18NResource = "GPIOEvent";

            ButtonNumber = "0";
        }

        public override object Clone()
        {
            GPIOUserEvent gpioUserEvent = new GPIOUserEvent();
            gpioUserEvent.ButtonNumber = this.ButtonNumber;
            gpioUserEvent.ButtonDirection = this.ButtonDirection;
            gpioUserEvent.InputConfiguration = (InputControlConfiguration)this.InputConfiguration.Clone();
            return gpioUserEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is GPIOUserEvent)) return false;

            GPIOUserEvent gpioUserEvent = (GPIOUserEvent)bsEvent;

            // compare _bpConfiguration
            if (!this.InputConfiguration.IsEqual(gpioUserEvent.InputConfiguration)) return false;

            return gpioUserEvent.ButtonNumber == this.ButtonNumber &&
                   gpioUserEvent.ButtonDirection == this.ButtonDirection;
        }

        public override string Description()
        {
            return Value + ": " + ButtonNumber;
        }

        public override string GetToolTip()
        {
            if (ButtonDirection == "up")
            {
                return BrightAuthorUtils.GetLocalizedString("GPIOUpEvent") + " " + ButtonNumber;
            }
            else
            {
                return BrightAuthorUtils.GetLocalizedString("GPIODownEvent") + " " + ButtonNumber;
            }
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");

            writer.WriteElementString("buttonNumber", ButtonNumber);
            writer.WriteElementString("buttonDirection", ButtonDirection);

            if (_inputConfiguration != null)
            {
                _inputConfiguration.WriteToXml(writer, publish);
            }
            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public new static GPIOUserEvent ReadXml(XmlReader reader)
        {
            GPIOUserEvent gpioUserEvent = null;

            string buttonNumber = String.Empty;
            string buttonDirection = "down";
            InputControlConfiguration inputConfiguration = new InputControlPressConfiguration();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                    case "buttonNumber":
                        buttonNumber = reader.ReadString();
                        break;
                    case "buttonDirection":
                        buttonDirection = reader.ReadString();
                        break;
                    case "press":
                        break;
                    case "pressContinuous":
                        inputConfiguration = InputControlPressContinuousConfiguration.ReadXml(reader);
                        break;
                }
            }

            gpioUserEvent = new GPIOUserEvent();
            gpioUserEvent.ButtonNumber = buttonNumber;
            gpioUserEvent.ButtonDirection = buttonDirection;
            gpioUserEvent.InputConfiguration = inputConfiguration;

            return gpioUserEvent;
        }

        public override void GetIconResourceNames(ref string imageResourceName, ref string imageResourceSelectedName)
        {
            switch (ButtonNumber)
            {
                case "0":
                    imageResourceName = "iconButton0";
                    imageResourceSelectedName = "iconButtonSelected0";
                    break;
                case "1":
                    imageResourceName = "iconButton1";
                    imageResourceSelectedName = "iconButtonSelected1";
                    break;
                case "2":
                    imageResourceName = "iconButton2";
                    imageResourceSelectedName = "iconButtonSelected2";
                    break;
                case "3":
                    imageResourceName = "iconButton3";
                    imageResourceSelectedName = "iconButtonSelected3";
                    break;
                case "4":
                    imageResourceName = "iconButton4";
                    imageResourceSelectedName = "iconButtonSelected4";
                    break;
                case "5":
                    imageResourceName = "iconButton5";
                    imageResourceSelectedName = "iconButtonSelected5";
                    break;
                case "6":
                    imageResourceName = "iconButton6";
                    imageResourceSelectedName = "iconButtonSelected6";
                    break;
                case "7":
                    imageResourceName = "iconButton7";
                    imageResourceSelectedName = "iconButtonSelected7";
                    break;
            }
        }
    }
}
