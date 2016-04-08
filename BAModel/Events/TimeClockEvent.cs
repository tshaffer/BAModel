using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class TimeClockEvent : UserEvent
    {
        public TimeClockEventSpecification TimeClockEventSpec { get; set; }

        public TimeClockEvent()
        {
            UserEventName = "timeClockEvent";
            Value = "Time/Clock";
            DialogTitle = "";
            ImageResourceName = "iconTimeClock";
            ImageResourceNameLarge = "iconTimeClockLarge";
            ImageResourceSelectedName = "iconTimeClockSelected";
            ValueI18NResource = "TimeClockEvent";
        }

        public override string GetToolTip()
        {
            return BrightAuthorUtils.GetLocalizedString("TimeClockEventDlg");
        }

        public override string Description()
        {
            return Value + ": " + TimeClockEventSpec.Description();
        }

        public new static TimeClockEvent ReadXml(XmlReader reader)
        {
            TimeClockEvent timeClockEvent = new TimeClockEvent();
            TimeClockEventSpecification timeClockEventSpecification = null;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "timeClockDateTime":
                        timeClockEventSpecification = TimeClockDateTime.ReadXml(reader);
                        break;
                    case "timeClockDateTimeByUserVariable":
                        timeClockEventSpecification = TimeClockDateTimeByUserVariable.ReadXml(reader);
                        break;
                    case "timeClockDailyOnce":
                        timeClockEventSpecification = TimeClockDailyOnce.ReadXml(reader);
                        break;
                    case "timeClockDailyPeriodic":
                        timeClockEventSpecification = TimeClockDailyPeriodic.ReadXml(reader);
                        break;
                }
            }

            timeClockEvent.TimeClockEventSpec = timeClockEventSpecification;

            return timeClockEvent;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            base.WriteBaseInfoToXml(writer, publish);

            writer.WriteStartElement("timeClockEvent");
            TimeClockEventSpec.WriteToXml(writer, publish);
            writer.WriteEndElement(); // textItem

            // close out element that was written in base class - yuck!
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            TimeClockEvent timeClockEvent = new TimeClockEvent();

            timeClockEvent.TimeClockEventSpec = (TimeClockEventSpecification)this.TimeClockEventSpec.Clone();

            return timeClockEvent;
        }

        public override bool IsEqual(BSEvent bsEvent)
        {
            if (!(bsEvent is TimeClockEvent)) return false;

            TimeClockEvent timeClockEvent = (TimeClockEvent)bsEvent;

            if (!timeClockEvent.TimeClockEventSpec.IsEqual(this.TimeClockEventSpec)) return false;
            return true;
        }
    }

    public abstract class TimeClockEventSpecification
    {
        public abstract void WriteToXml(System.Xml.XmlTextWriter writer, bool publish);

        public abstract Object Clone();

        public abstract bool IsEqual(Object obj);

        public abstract string Description();

    }

    public class TimeClockDateTimeByUserVariable : TimeClockEventSpecification
    {
        public string UserVariableName { get; set; }

        public override object Clone()
        {
            TimeClockDateTimeByUserVariable timeClockDateTimeByUserVariable = new TimeClockDateTimeByUserVariable();
            timeClockDateTimeByUserVariable.UserVariableName = this.UserVariableName;
            return timeClockDateTimeByUserVariable;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TimeClockDateTimeByUserVariable timeClockDateTimeByUserVariable = (TimeClockDateTimeByUserVariable)obj;
            if (!timeClockDateTimeByUserVariable.UserVariableName.Equals(this.UserVariableName)) return false;
            return true;
        }

        public override string Description()
        {
            return "Variable: " + UserVariableName;
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("timeClockDateTimeByUserVariable");
            writer.WriteElementString("userVariableName", UserVariableName);
            writer.WriteEndElement(); // timeClockDateTimeByUserVariable
        }

        public static TimeClockDateTimeByUserVariable ReadXml(XmlReader reader)
        {
            string userVariableName = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userVariableName":
                        userVariableName = reader.ReadString();
                        break;
                }
            }

            TimeClockDateTimeByUserVariable timeClockDateTimeByUserVariable = new TimeClockDateTimeByUserVariable { UserVariableName = userVariableName };
            return timeClockDateTimeByUserVariable;
        }
    }

    public class TimeClockDateTime : TimeClockEventSpecification
    {
        public DateTime DateTime { get; set; }

        public override object Clone()
        {
            TimeClockDateTime timeClockDateTime = new TimeClockDateTime();
            timeClockDateTime.DateTime = this.DateTime;
            return timeClockDateTime;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TimeClockDateTime timeClockDateTime = (TimeClockDateTime)obj;
            if (!timeClockDateTime.DateTime.Equals(this.DateTime)) return false;
            return true;
        }

        public override string Description()
        {
            return "DateTime: " + DateTime.ToString("s");
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("timeClockDateTime");
            writer.WriteElementString("dateTime", DateTime.ToString("s"));
            writer.WriteEndElement(); // timeClockDateTime
        }

        public static TimeClockDateTime ReadXml(XmlReader reader)
        {
            string dateTimeStr = String.Empty;
            DateTime dateTime = DateTime.Now;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "dateTime":
                        dateTimeStr = reader.ReadString();
                        bool ok = DateTime.TryParse(dateTimeStr, out dateTime);
                        break;
                }
            }

            TimeClockDateTime timeClockDateTime = new TimeClockDateTime { DateTime = dateTime };
            return timeClockDateTime;
        }
    }

    public abstract class TimeClockDaily : TimeClockEventSpecification
    {
        public int DaysOfWeek { get; set; }

        public abstract override Object Clone();

        protected void CopyMembers(TimeClockDaily timeClockDaily)
        {
            timeClockDaily.DaysOfWeek = this.DaysOfWeek;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TimeClockDaily timeClockDaily = (TimeClockDaily)obj;
            if (timeClockDaily.DaysOfWeek != this.DaysOfWeek) return false;

            return true;
        }

        public override string Description()
        {
            return "DaysOfWeek: " + DaysOfWeek.ToString();
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteElementString("daysOfWeek", DaysOfWeek.ToString());
        }
    }

    public class TimeClockDailyOnce : TimeClockDaily
    {
        public int EventTime { get; set; }

        public override object Clone()
        {
            TimeClockDailyOnce timeClockDailyOnce = new TimeClockDailyOnce();
            timeClockDailyOnce.EventTime = this.EventTime;
            base.CopyMembers(timeClockDailyOnce);
            return timeClockDailyOnce;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TimeClockDailyOnce timeClockDailyOnce = (TimeClockDailyOnce)obj;
            if (timeClockDailyOnce.EventTime != this.EventTime) return false;

            return base.IsEqual(timeClockDailyOnce);
        }

        public override string Description()
        {
            return "DailyOnce: " + EventTime.ToString();
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("timeClockDailyOnce");
            base.WriteToXml(writer, publish);
            writer.WriteElementString("eventTime", EventTime.ToString());
            writer.WriteEndElement(); // timeClockDailyOnce
        }

        public static TimeClockDailyOnce ReadXml(XmlReader reader)
        {
            string daysOfWeekStr = String.Empty;
            int daysOfWeek = 0;

            string eventTimeStr = String.Empty;
            int eventTime = 0;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "eventTime":
                        eventTimeStr = reader.ReadString();
                        eventTime = Convert.ToInt16(eventTimeStr);
                        break;
                    case "daysOfWeek":
                        daysOfWeekStr = reader.ReadString();
                        daysOfWeek = Convert.ToInt32(daysOfWeekStr);
                        break;
                }
            }

            TimeClockDailyOnce timeClockDailyOnce = new TimeClockDailyOnce { DaysOfWeek = daysOfWeek, EventTime = eventTime };
            return timeClockDailyOnce;
        }
    }

    public class TimeClockDailyPeriodic : TimeClockDaily
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int IntervalTime { get; set; }

        public override object Clone()
        {
            TimeClockDailyPeriodic timeClockDailyPeriodic = new TimeClockDailyPeriodic();
            timeClockDailyPeriodic.StartTime = this.StartTime;
            timeClockDailyPeriodic.EndTime = this.EndTime;
            timeClockDailyPeriodic.IntervalTime = this.IntervalTime;
            base.CopyMembers(timeClockDailyPeriodic);
            return timeClockDailyPeriodic;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            TimeClockDailyPeriodic timeClockDailyPeriodic = (TimeClockDailyPeriodic)obj;
            if (timeClockDailyPeriodic.StartTime != this.StartTime) return false;
            if (timeClockDailyPeriodic.EndTime != this.EndTime) return false;
            if (timeClockDailyPeriodic.IntervalTime != this.IntervalTime) return false;

            return base.IsEqual(timeClockDailyPeriodic);
        }

        public override string Description()
        {
            return "DailyPeriodic, start: " + StartTime.ToString() + ", end: " + EndTime.ToString() + ", interval: " + IntervalTime.ToString();
        }

        public override void WriteToXml(XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("timeClockDailyPeriodic");
            base.WriteToXml(writer, publish);
            writer.WriteElementString("startTime", StartTime.ToString());
            writer.WriteElementString("endTime", EndTime.ToString());
            writer.WriteElementString("intervalTime", IntervalTime.ToString());
            writer.WriteEndElement(); // timeClockDailyPeriodic
        }

        public static TimeClockDailyPeriodic ReadXml(XmlReader reader)
        {
            int daysOfWeek = 0;
            int startTime = 0;
            int endTime = 0;
            int intervalTime = 0;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "daysOfWeek":
                        string daysOfWeekStr = reader.ReadString();
                        daysOfWeek = Convert.ToInt32(daysOfWeekStr);
                        break;
                    case "startTime":
                        string startTimeStr = reader.ReadString();
                        startTime = Convert.ToInt16(startTimeStr);
                        break;
                    case "endTime":
                        string endTimeStr = reader.ReadString();
                        endTime = Convert.ToInt16(endTimeStr);
                        break;
                    case "intervalTime":
                        string intervalTimeStr = reader.ReadString();
                        intervalTime = Convert.ToInt16(intervalTimeStr);
                        break;
                }
            }

            TimeClockDailyPeriodic timeClockDailyPeriodic = new TimeClockDailyPeriodic { DaysOfWeek = daysOfWeek, StartTime = startTime, EndTime = endTime, IntervalTime = intervalTime };
            return timeClockDailyPeriodic;
        }
    }

    //public class TimeClockDaily : TimeClockEventSpecification
    //{
    //    public int EventTime { get; set; }

    //    public override object Clone()
    //    {
    //        TimeClockDaily timeClockDaily = new TimeClockDaily();
    //        timeClockDaily.EventTime = this.EventTime;
    //        return timeClockDaily;
    //    }

    //    public override bool IsEqual(Object obj)
    //    {
    //        //Check for null and compare run-time types.
    //        if (obj == null || GetType() != obj.GetType()) return false;

    //        TimeClockDaily timeClockDaily = (TimeClockDaily)obj;
    //        if (timeClockDaily.EventTime != this.EventTime) return false;
    //        return true;
    //    }

    //    public override void WriteToXml(XmlTextWriter writer, bool publish)
    //    {
    //        writer.WriteStartElement("timeClockDaily");
    //        writer.WriteElementString("eventTime", EventTime.ToString());
    //        writer.WriteEndElement(); // timeClockDaily
    //    }

    //    public static TimeClockDaily ReadXml(XmlReader reader)
    //    {
    //        string eventTimeStr = String.Empty;
    //        int eventTime = 0;

    //        while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
    //        {
    //            switch (reader.LocalName)
    //            {
    //                case "eventTime":
    //                    eventTimeStr = reader.ReadString();
    //                    eventTime = Convert.ToInt16(eventTimeStr);
    //                    break;
    //            }
    //        }

    //        TimeClockDaily timeClockDaily = new TimeClockDaily { EventTime = eventTime };
    //        return timeClockDaily;
    //    }

    //}


    //public class TimeClockPeriodic : TimeClockEventSpecification
    //{
    //    public int IntervalTime { get; set; }

    //    public override object Clone()
    //    {
    //        TimeClockPeriodic timeClockPeriodic = new TimeClockPeriodic();
    //        timeClockPeriodic.IntervalTime = this.IntervalTime;
    //        return timeClockPeriodic;
    //    }

    //    public override bool IsEqual(Object obj)
    //    {
    //        //Check for null and compare run-time types.
    //        if (obj == null || GetType() != obj.GetType()) return false;

    //        TimeClockPeriodic timeClockPeriodic = (TimeClockPeriodic)obj;
    //        if (timeClockPeriodic.IntervalTime != this.IntervalTime) return false;
    //        return true;
    //    }

    //    public override void WriteToXml(XmlTextWriter writer, bool publish)
    //    {
    //        writer.WriteStartElement("timeClockPeriodic");
    //        writer.WriteElementString("intervalTime", IntervalTime.ToString());
    //        writer.WriteEndElement(); // timeClockPeriodic
    //    }

    //    public static TimeClockPeriodic ReadXml(XmlReader reader)
    //    {
    //        string intervalTimeStr = String.Empty;
    //        int intervalTime = 0;

    //        while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
    //        {
    //            switch (reader.LocalName)
    //            {
    //                case "intervalTime":
    //                    intervalTimeStr = reader.ReadString();
    //                    intervalTime = Convert.ToInt16(intervalTimeStr);
    //                    break;
    //            }
    //        }

    //        TimeClockPeriodic timeClockPeriodic = new TimeClockPeriodic { IntervalTime = intervalTime };
    //        return timeClockPeriodic;
    //    }
    //}


}
