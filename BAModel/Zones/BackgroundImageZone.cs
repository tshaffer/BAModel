using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    public class BackgroundImageZone : Zone
    {
        public BackgroundImageZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id) :
            base (name, xStart, yStart, width, height, zoneType, id)
        {
        }

        public override object Clone()
        {
            BackgroundImageZone backgroundImageZone = new BackgroundImageZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID);

            return base.Copy(backgroundImageZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
