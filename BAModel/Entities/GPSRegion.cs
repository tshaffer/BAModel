using System;
using System.Xml;

namespace BAModel
{
    public class GPSRegion
    {
        public double Radius { get; set; }
        public bool RadiusUnitsInMiles { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GPSRegion()
        {
            Radius = 1.0;
            RadiusUnitsInMiles = true;
            Latitude = 0;
            Longitude = 0;
        }

        public static GPSRegion ReadXml(XmlReader reader)
        {
            GPSRegion gpsRegion = new GPSRegion();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "radius":
                        gpsRegion.Radius = BrightAuthorUtils.SetDoubleFromEnglishString(reader.ReadString());
                        break;
                    case "radiusUnitsInMiles":
                        gpsRegion.RadiusUnitsInMiles = UserPreferences.ConvertStringToBool(reader.ReadString());
                        break;
                    case "latitude":
                        gpsRegion.Latitude = BrightAuthorUtils.SetDoubleFromEnglishString(reader.ReadString());
                        break;
                    case "longitude":
                        gpsRegion.Longitude = BrightAuthorUtils.SetDoubleFromEnglishString(reader.ReadString());
                        break;
                }
            }

            return gpsRegion;
        }

        public void WriteToXml(System.Xml.XmlTextWriter writer, bool publish)
        {
            writer.WriteStartElement("gpsRegion");

            if (publish)
            {
                double radiusInFeet;
                if (RadiusUnitsInMiles)
                {
                    radiusInFeet = Radius * 5280;
                }
                else
                {
                    radiusInFeet = Radius * 3280.8399;
                }
                writer.WriteElementString("radiusInFeet", BrightAuthorUtils.GetDoubleAsEnglishString(radiusInFeet));
            }
            else
            {
                writer.WriteElementString("radius", BrightAuthorUtils.GetDoubleAsEnglishString(Radius));
                writer.WriteElementString("radiusUnitsInMiles", RadiusUnitsInMiles.ToString());
            }
            writer.WriteElementString("latitude", BrightAuthorUtils.GetDoubleAsEnglishString(Latitude));
            writer.WriteElementString("longitude", BrightAuthorUtils.GetDoubleAsEnglishString(Longitude));

            writer.WriteEndElement(); // gpsRegion
        }

        public GPSRegion Clone()
        {
            GPSRegion gpsRegion = new GPSRegion();
            gpsRegion.Radius = this.Radius;
            gpsRegion.RadiusUnitsInMiles = this.RadiusUnitsInMiles;
            gpsRegion.Latitude = this.Latitude;
            gpsRegion.Longitude = this.Longitude;
            return gpsRegion;
        }

        public bool IsEqual(Object o)
        {
            if (!(o is GPSRegion)) return false;

            GPSRegion gpsRegion = (GPSRegion)o;

            if (
                 (gpsRegion.Radius != this.Radius) ||
                 (gpsRegion.RadiusUnitsInMiles != this.RadiusUnitsInMiles) ||
                 (gpsRegion.Latitude != this.Latitude) ||
                 (gpsRegion.Longitude != this.Longitude))
            {
                return false;
            }

            return true;
        }

        public void SetRadiusFromString(string txtRadius)
        {
            Radius = Convert.ToDouble(txtRadius);
        }

        public string GetRadiusAsString()
        {
            return Radius.ToString();
        }

        public void SetLatitudeFromString(string txtLatitude)
        {
            Latitude = Convert.ToDouble(txtLatitude);
        }

        public string GetLatitudeAsString()
        {
            return Latitude.ToString();
        }

        public void SetLongitudeFromString(string txtLongitude)
        {
            Longitude = Convert.ToDouble(txtLongitude);
        }

        public string GetLongitudeAsString()
        {
            return Longitude.ToString();
        }
    }
}
