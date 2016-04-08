using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    class ImageZone : Zone
    {
        private int _imageMode;

        public ImageZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id, int imageMode) :
            base (name, xStart, yStart, width, height, zoneType, id)
        {
            _imageMode = imageMode;
        }

        public int ImageMode
        {
            get { return _imageMode; }
            set { _imageMode = value; }
        }

        public override object Clone()
        {
            ImageZone imageZone = new ImageZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID, this.ImageMode);

            return base.Copy(imageZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            ImageZone imageZone = (ImageZone)obj;
            if (imageZone.ImageMode != this.ImageMode) return false;

            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditImagesZoneDlg editImagesZoneDlg = new EditImagesZoneDlg();
//            editImagesZoneDlg.Owner = parent;
//
//            editImagesZoneDlg.ImageMode = BrightAuthorUtils.GetImageModeSpec(ImageMode);
//
//            if (editImagesZoneDlg.ShowDialog() == true)
//            {
//                ImageMode = BrightAuthorUtils.GetImageModeValue(editImagesZoneDlg.ImageMode);
//
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.ImageMode = editImagesZoneDlg.ImageMode;
//                }
//            }
//
//        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteElementString("imageMode", BrightAuthorUtils.GetImageModeSpec(ImageMode));
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out int imageMode)
        {
            imageMode = 1;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "imageMode":
                        string imageModeStr = reader.ReadString();
                        imageMode = BrightAuthorUtils.GetImageModeValue(imageModeStr);
                        break;
                }
            }
        }

    }
}
