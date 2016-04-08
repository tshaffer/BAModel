using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class EnhancedAudioZone : AudioZone
    {
        private int _fadeLength;

        public EnhancedAudioZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id, AudioZone.AudioOutputSelection audioOutput, AudioZone.AudioModeSelection audioMode, AudioZone.AudioMappingSelection audioMapping,
            AudioZone.AudioOutputType analogOutput, AudioZone.AudioOutputType analog2Output, AudioZone.AudioOutputType analog3Output, AudioZone.AudioOutputType hdmiOutput, AudioZone.AudioOutputType spdifOutput,
            AudioZone.AudioOutputType usbOutput, AudioZone.AudioOutputType usbOutputA, AudioZone.AudioOutputType usbOutputB, AudioZone.AudioOutputType usbOutputC, AudioZone.AudioOutputType usbOutputD, 
            AudioZone.AudioMixMode audioMixMode,            
            string audioVolume, string minimumVolume, string maximumVolume, int fadeLength) :
            base(name, xStart, yStart, width, height, zoneType, id, audioOutput, audioMode, audioMapping, 
            analogOutput, analog2Output, analog3Output, hdmiOutput, spdifOutput,
            usbOutput, usbOutputA, usbOutputB, usbOutputC, usbOutputD, 
            audioMixMode,
            audioVolume, minimumVolume, maximumVolume)
        {
            _fadeLength = fadeLength;
        }

        public int FadeLength
        {
            get { return _fadeLength; }
            set { _fadeLength = value; }
        }

        public override object Clone() // ICloneable implementation
        {
            EnhancedAudioZone enhancedAudioZone = new EnhancedAudioZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID, this.AudioOutput, this.AudioMode, this.AudioMapping,
                this.AnalogOutput, this.Analog2Output, this.Analog3Output, this.HDMIOutput, this.SPDIFOutput,
                this.USBOutput, this.USBOutputA, this.USBOutputB, this.USBOutputC, this.USBOutputD, 
                this.AudioZoneMixMode,
                this.AudioVolume, this.MinimumVolume, this.MaximumVolume, this.FadeLength);

            return base.Copy(enhancedAudioZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            EnhancedAudioZone enhancedAudioZone = (EnhancedAudioZone)obj;
            if (enhancedAudioZone.FadeLength != this.FadeLength) return false;
            return base.IsEqual(obj);
        }

        public override int UsedAudioDecoders()
        {
            return 2;
        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            base.WriteZoneSpecificDataToXml(writer, publish, sign);
            writer.WriteElementString("fadeLength", FadeLength.ToString());
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out AudioZone.AudioOutputSelection audioOutput, out AudioZone.AudioModeSelection audioMode, out AudioZone.AudioMappingSelection audioMapping,
            out AudioZone.AudioOutputType analogOutput, out AudioZone.AudioOutputType analog2Output, out AudioZone.AudioOutputType analog3Output, out AudioZone.AudioOutputType hdmiOutput, out AudioZone.AudioOutputType spdifOutput,
            out AudioZone.AudioOutputType usbOutput, out AudioZone.AudioOutputType usbOutputA, out AudioZone.AudioOutputType usbOutputB, out AudioZone.AudioOutputType usbOutputC, out AudioZone.AudioOutputType usbOutputD, 
            out AudioZone.AudioMixMode audioMixMode,
            out string audioVolume, out string minimumVolume, out string maximumVolume, out int fadeLength)
        {
            audioOutput = AudioZone.AudioOutputSelection.AnalogAudio;
            audioMode = AudioZone.AudioModeSelection.MultichannelSurround;
            audioMapping = AudioZone.AudioMappingSelection.Audio1;

            analogOutput = AudioOutputType.PCM;
            analog2Output = AudioOutputType.None;
            analog3Output = AudioOutputType.None;
            hdmiOutput = AudioOutputType.PCM;
            spdifOutput = AudioOutputType.PCM;
            usbOutput = AudioOutputType.None;
            usbOutputA = AudioOutputType.None;
            usbOutputB = AudioOutputType.None;
            usbOutputC = AudioOutputType.None;
            usbOutputD = AudioOutputType.None;
            audioMixMode = AudioMixMode.Stereo;

            fadeLength = 0;

            audioVolume = UserPreferences.InitialAudioVolume;
            minimumVolume = "0";
            maximumVolume = "100";

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                AssignAudioXmlValue(reader, ref audioOutput, ref audioMode, ref audioMapping,
                    ref analogOutput, ref analog2Output, ref analog3Output, ref hdmiOutput, ref spdifOutput,
                    ref usbOutput, ref usbOutputA, ref usbOutputB, ref usbOutputC, ref usbOutputD, 
                    ref audioMixMode,
                    ref audioVolume, ref minimumVolume, ref maximumVolume);

                if (reader.LocalName == "fadeLength")
                {
                    string fadeLengthStr = reader.ReadString();
                    fadeLength = Convert.ToInt32(fadeLengthStr);
                }
            }
        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditAudioZoneDlg editAudioZoneDlg = new EditAudioZoneDlg(true);
//            editAudioZoneDlg.FadeLength = FadeLength;
//
//            bool changesMade = EditAudioParameters(parent, editAudioZoneDlg);
//            if (changesMade)
//            {
//                FadeLength = editAudioZoneDlg.FadeLength;
//
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.FadeLength = FadeLength;
//                }
//            }
//        }
    }
}
