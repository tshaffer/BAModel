using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    class VideoOrImagesZone : VideoZone
    {
        private int _imageMode;

        public VideoOrImagesZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id,
            int viewMode, AudioZone.AudioOutputSelection audioOutput, AudioZone.AudioModeSelection audioMode, AudioZone.AudioMappingSelection audioMapping,
            AudioZone.AudioOutputType analogOutput, AudioZone.AudioOutputType analog2Output, AudioZone.AudioOutputType analog3Output, AudioZone.AudioOutputType hdmiOutput, AudioZone.AudioOutputType spdifOutput,
            AudioZone.AudioOutputType usbOutput, AudioZone.AudioOutputType usbOutputA, AudioZone.AudioOutputType usbOutputB, AudioZone.AudioOutputType usbOutputC, AudioZone.AudioOutputType usbOutputD, 
            AudioZone.AudioMixMode audioMixMode,
            int imageMode,
            string videoVolume, string audioVolume, string minimumVolume, string maximumVolume, string liveVideoInput, 
            string liveVideoStandard, string brightness, string contrast, string saturation, string hue, bool zOrderFront,
            bool mosaic, MosaicDecoder.MaxContentResolution maxResolution) :
            base(name, xStart, yStart, width, height, zoneType, id, viewMode, audioOutput, audioMode, audioMapping,
            analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
            usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD,
            audioMixMode,
            videoVolume, audioVolume, minimumVolume,  maximumVolume,
            liveVideoInput, liveVideoStandard, brightness, contrast, saturation, hue,
            zOrderFront, mosaic, maxResolution)
        {
            _imageMode = imageMode;
        }

        public int ImageMode
        {
            get { return _imageMode; }
            set { _imageMode = value; }
        }

        public override object Clone() // ICloneable implementation
        {
            VideoOrImagesZone videoOrImagesZone = new VideoOrImagesZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID,
                this.ViewMode, this.AudioOutput, this.AudioMode, this.AudioMapping,
                this.AnalogOutput, this.Analog2Output, this.Analog3Output, this.HDMIOutput, this.SPDIFOutput,
                this.USBOutput, this.USBOutputA, this.USBOutputB, this.USBOutputC, this.USBOutputD, 
                this.AudioMixMode,
                this.ImageMode,
                this.VideoVolume, this.AudioVolume, this.MinimumVolume, this.MaximumVolume,
                this.LiveVideoInput, this.LiveVideoStandard, this.Brightness, this.Contrast, this.Saturation, this.Hue, this.ZOrderFront,
                this.Mosaic, this.MaxResolution);

            return base.Copy(videoOrImagesZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            VideoOrImagesZone videoOrImagesZone = (VideoOrImagesZone)obj;
            if ((videoOrImagesZone.ImageMode != this.ImageMode))
            {
                return false;
            }
            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditVideoImagesZoneDlg editVideoImagesZoneDlg = new EditVideoImagesZoneDlg(true, !Mosaic, Mosaic);
//            editVideoImagesZoneDlg.Owner = parent;
//
//            AssignVideoParameters(editVideoImagesZoneDlg);
//            editVideoImagesZoneDlg.ImageMode = BrightAuthorUtils.GetImageModeSpec(ImageMode);
//
//            if (editVideoImagesZoneDlg.ShowDialog() == true)
//            {
//                RetrieveVideoParameters(editVideoImagesZoneDlg, UserPreferences.SavePropertiesForAllFuture);
//
//                ImageMode = BrightAuthorUtils.GetImageModeValue(editVideoImagesZoneDlg.ImageMode);
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.ImageMode = editVideoImagesZoneDlg.ImageMode;
//                }
//            }
//        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            base.WriteZoneSpecificDataToXml(writer, publish, sign);
            writer.WriteElementString("imageMode", BrightAuthorUtils.GetImageModeSpec(ImageMode));
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out int viewMode, out AudioZone.AudioOutputSelection audioOutput, out AudioZone.AudioModeSelection audioMode,
            out AudioZone.AudioMappingSelection audioMapping,
            out AudioZone.AudioOutputType analogOutput, out AudioZone.AudioOutputType analog2Output, out AudioZone.AudioOutputType analog3Output, out AudioZone.AudioOutputType hdmiOutput, out AudioZone.AudioOutputType spdifOutput,
            out AudioZone.AudioOutputType usbOutput, out AudioZone.AudioOutputType usbOutputA, out AudioZone.AudioOutputType usbOutputB, out AudioZone.AudioOutputType usbOutputC, out AudioZone.AudioOutputType usbOutputD, 
            out AudioZone.AudioMixMode audioMixMode, 
            out int imageMode,
            out string videoVolume, out string audioVolume, out string minimumVolume, out string maximumVolume,
            out string liveVideoInput, out string liveVideoStandard, out string brightness, out string contrast, out string saturation, out string hue,
            out bool zOrderFront, out bool mosaic, out MosaicDecoder.MaxContentResolution maxResolution)
        {
            viewMode = 1;
            audioOutput = AudioZone.AudioOutputSelection.AnalogAudio;
            audioMode = AudioZone.AudioModeSelection.MultichannelSurround;
            audioMapping = AudioZone.AudioMappingSelection.Audio1;
            
            analogOutput = AudioZone.AudioOutputType.PCM;
            analog2Output = AudioZone.AudioOutputType.None;
            analog3Output = AudioZone.AudioOutputType.None;
            hdmiOutput = AudioZone.AudioOutputType.PCM;
            spdifOutput = AudioZone.AudioOutputType.PCM;
            usbOutput = AudioZone.AudioOutputType.None;
            usbOutputA = AudioZone.AudioOutputType.None;
            usbOutputB = AudioZone.AudioOutputType.None;
            usbOutputC = AudioZone.AudioOutputType.None;
            usbOutputD = AudioZone.AudioOutputType.None;
            audioMixMode = AudioZone.AudioMixMode.Stereo;

            imageMode = 1;

            videoVolume = UserPreferences.InitialVideoVolume;
            audioVolume = UserPreferences.InitialAudioVolume;
            minimumVolume = "0";
            maximumVolume = "100";

            liveVideoInput = UserPreferences.LiveVideoInput;
            liveVideoStandard = UserPreferences.LiveVideoStandard;
            brightness = UserPreferences.Brightness;
            contrast = UserPreferences.Contrast;
            saturation = UserPreferences.Saturation;
            hue = UserPreferences.Hue;

            zOrderFront = true;

            mosaic = false;
            maxResolution = MosaicDecoder.MaxContentResolution._NotApplicable;

            bool updatedAudioParametersFound = false;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                bool parameterFound = ReadVideoZoneSpecificDataXml(reader, ref viewMode, ref audioOutput, ref audioMode, ref audioMapping,
                    ref analogOutput, ref analog2Output, ref analog3Output, ref hdmiOutput, ref spdifOutput,
                    ref usbOutput, ref usbOutputA, ref usbOutputB, ref usbOutputC, ref usbOutputD, 
                    ref audioMixMode,
                    ref videoVolume, ref audioVolume, ref minimumVolume, ref maximumVolume,
                    ref liveVideoInput, ref liveVideoStandard, ref brightness, ref contrast, ref saturation, ref hue, ref zOrderFront,
                    ref mosaic, ref maxResolution);

                string localName = reader.LocalName;

                if ((!parameterFound) && (localName == "imageMode"))
                {
                    string imageModeStr = reader.ReadString();
                    imageMode = BrightAuthorUtils.GetImageModeValue(imageModeStr);
                }
                if (parameterFound && ((localName == "analogOutput") || (localName == "analog2Output") || (localName == "analog3Output")))
                {
                    updatedAudioParametersFound = true;
                }
            }

            if (!updatedAudioParametersFound)
            {
                AudioZone.UpdateAudioParameters(audioOutput, audioMode,
                    ref analogOutput, ref analog2Output, ref analog3Output, ref hdmiOutput, ref spdifOutput,
                    ref usbOutput, ref usbOutputA, ref usbOutputB, ref usbOutputC, ref usbOutputD,
                    ref audioMixMode);
            }
        }
    }
}
