using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace BAModel
{
    public class SimpleUserEvent : UserEvent
    {
        public string Parameter { get; set; }
        public bool ParameterRequired { get; set; }
        public string Prompt { get; set; }
        public string PromptI18NResource { get; set; }
        public int ValidationRules { get; set; }

        public SimpleUserEvent()
        {
        }

        public override object Clone()
        {
            return new SimpleUserEvent
            {
                UserEventName = this.UserEventName,
                Value = this.Value,
                Parameter = this.Parameter,
                ParameterRequired = this.ParameterRequired,
                Prompt = this.Prompt,
                DialogTitle = this.DialogTitle,
                ImageResourceName = this.ImageResourceName,
                ImageResourceSelectedName = this.ImageResourceSelectedName,
                ImageResourceNameLarge = this.ImageResourceNameLarge,
                ValidationRules = this.ValidationRules,
                ValueI18NResource = this.ValueI18NResource,
                DialogTitleI18NResource = this.DialogTitleI18NResource,
                PromptI18NResource = this.PromptI18NResource
            };
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is SimpleUserEvent)) return false;

            SimpleUserEvent simpleUserEvent = (SimpleUserEvent)bsEvent;
            return simpleUserEvent.UserEventName == this.UserEventName && simpleUserEvent.Parameter == this.Parameter;
        }

        public override string Description()
        {
            string desc = Value;
            if (ParameterRequired)
            {
                desc += ": ";
                desc += Parameter;
            }
            return desc;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            // If validation rule specifies this is a number, format it properly for the BrightSign
            //  when publishing (bug 19560)
            string paramString = Parameter;
            if (publish && (ValidationRules & EventDlg.ValidateNumberGreaterThanZero) != 0)
            {
                double value;
                if (Double.TryParse(Parameter, out value))
                {
                    paramString = BrightAuthorUtils.GetDoubleAsEnglishString(value);
                }
            }

            writer.WriteStartElement("parameters");
            writer.WriteElementString("parameter", paramString);
            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public static SimpleUserEvent ReadXml(XmlReader reader, string userEventName)
        {
            SimpleUserEvent simpleUserEvent = null;
            string parameter = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                        parameter = reader.ReadString();
                        break;
                }
            }

            SimpleUserEvent simpleUserEventTemplate = (SimpleUserEvent)TransitionEventMgr.GetSimpleUserEvent(userEventName);
            if (simpleUserEventTemplate != null)
            {
                simpleUserEvent = (SimpleUserEvent)simpleUserEventTemplate.Clone();
                simpleUserEvent.Parameter = parameter;
            }
            return simpleUserEvent;
        }

        public override string GetToolTip()
        {
            string tt = BrightAuthorUtils.GetLocalizedString(ValueI18NResource);
            if (ParameterRequired)
            {
                string parameter = Parameter;
                tt += " " + parameter;
            }
            return tt;
        }

        public override void GetIconResourceNames(ref string imageResourceName, ref string imageResourceSelectedName)
        {
            imageResourceName = ImageResourceName;
            imageResourceSelectedName = ImageResourceSelectedName;
        }
    }

    public class TwoParameterUserEvent : SimpleUserEvent
    {
        public string Parameter2 { get; set; }
        public bool Parameter2Required { get; set; }
        public string Prompt2 { get; set; }
        public int ValidationRules2 { get; set; }
        public string Prompt2I18NResource { get; set; }

        public TwoParameterUserEvent()
        {
        }

        public override object Clone()
        {
            TwoParameterUserEvent userEvent = new TwoParameterUserEvent
            {
                UserEventName = this.UserEventName,
                Value = this.Value,
                Parameter = this.Parameter,
                ParameterRequired = this.ParameterRequired,
                Prompt = this.Prompt,
                DialogTitle = this.DialogTitle,
                ImageResourceName = this.ImageResourceName,
                ImageResourceSelectedName = this.ImageResourceSelectedName,
                ImageResourceNameLarge = this.ImageResourceNameLarge,
                ValidationRules = this.ValidationRules,
                Parameter2 = this.Parameter2,
                Parameter2Required = this.Parameter2Required,
                Prompt2 = this.Prompt2,
                ValidationRules2 = this.ValidationRules2,
                ValueI18NResource = this.ValueI18NResource,
                DialogTitleI18NResource = this.DialogTitleI18NResource,
                PromptI18NResource = this.PromptI18NResource,
                Prompt2I18NResource = this.Prompt2I18NResource
            };

            return userEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is TwoParameterUserEvent)) return false;

            TwoParameterUserEvent twoParameterUserEvent = (TwoParameterUserEvent)bsEvent;
            return twoParameterUserEvent.UserEventName == this.UserEventName && twoParameterUserEvent.Parameter == this.Parameter && twoParameterUserEvent.Parameter2 == this.Parameter2;
        }

        public override string Description()
        {
            string desc = base.Description();
            if (Parameter2Required)
            {
                desc += ", ";
                desc += Parameter2;
            }
            return desc;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("parameters");
            writer.WriteElementString("parameter", Parameter);

            // hack for serial
            string wildcardDesignator = "<*>";
            string parameter2 = Parameter2;
            if (publish && UserEventName == "serial")
            {
                int index = Parameter2.IndexOf(wildcardDesignator);
                if (index >= 0)
                {
                    string sub1 = Parameter2.Substring(0, index);
                    string sub2 = (Parameter2.Length - wildcardDesignator.Length > index) ? Parameter2.Substring(index + wildcardDesignator.Length) : String.Empty;
                    parameter2 = Regex.Escape(sub1) + "(.*)" + Regex.Escape(sub2);
                }
            }
            writer.WriteElementString("parameter2", parameter2);
            
            writer.WriteEndElement(); // parameters

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public static new TwoParameterUserEvent ReadXml(XmlReader reader, string userEventName)
        {
            TwoParameterUserEvent twoParameterUserEvent = null;
            string parameter = "";
            string parameter2 = "";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameters":
                        break;
                    case "parameter":
                        parameter = reader.ReadString();
                        break;
                    case "parameter2":
                        parameter2 = reader.ReadString();
                        break;
                }
            }

            SimpleUserEvent simpleUserEventTemplate = (SimpleUserEvent)TransitionEventMgr.GetSimpleUserEvent(userEventName);
            if (simpleUserEventTemplate != null)
            {
                if (simpleUserEventTemplate is TwoParameterUserEvent)
                {
                    TwoParameterUserEvent twoParameterUserEventTemplate = simpleUserEventTemplate as TwoParameterUserEvent;
                    twoParameterUserEvent = (TwoParameterUserEvent)twoParameterUserEventTemplate.Clone();

                    twoParameterUserEvent.Parameter = parameter;
                    twoParameterUserEvent.Parameter2 = parameter2;

                    // hack to deal with compatibility issue
                    if (twoParameterUserEvent.UserEventName == "serial")
                    {
                        if (parameter2 == String.Empty)
                        {
                            twoParameterUserEvent.Parameter = "0";
                            twoParameterUserEvent.Parameter2 = parameter;
                        }
                    }
                }
            }
            return twoParameterUserEvent;
        }
    }

}
