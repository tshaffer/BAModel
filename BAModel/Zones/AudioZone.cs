using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    public class AudioZone : Zone
    {

        public enum AudioOutputSelection
        {
            AnalogAudio,
            USBAudio,
            DigitalAudioStereoPCM,
            DigitalAudioRawAC3,
            OnboardAnalogHDMIMirrowRaw
        }

        public enum AudioModeSelection
        {
            MultichannelSurround,
            MixedDownToStereo,
            NoAudio,
            MonoLeftMixdown,
            MonoRightMixdown
        }

        public enum AudioMappingSelection
        {
            Audio1,
            Audio2,
            Audio3,
            AudioAll
        }

        private AudioOutputSelection _audioOutput;
        private AudioModeSelection _audioMode;
        private AudioMappingSelection _audioMapping;

        public enum AudioOutputType
        {
            PCM,
            PassThrough,
            Multichannel,
            None
        }
        private AudioOutputType _analogOutput;
        private AudioOutputType _analog2Output;
        private AudioOutputType _analog3Output;
        private AudioOutputType _hdmiOutput;
        private AudioOutputType _spdifOutput;
        private AudioOutputType _usbOutput;
        private AudioOutputType _usbOutputA;
        private AudioOutputType _usbOutputB;
        private AudioOutputType _usbOutputC;
        private AudioOutputType _usbOutputD;

        public enum AudioMixMode
        {
            Stereo,
            Left,
            Right
        }
        private AudioMixMode _audioZoneMixMode;

        private string _audioVolume;

        string _minimumVolume;
        string _maximumVolume;

        public AudioZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id, AudioOutputSelection audioOutput, AudioModeSelection audioMode, AudioMappingSelection audioMapping,
            AudioZone.AudioOutputType analogOutput, AudioZone.AudioOutputType analog2Output, AudioZone.AudioOutputType analog3Output, AudioZone.AudioOutputType hdmiOutput, AudioZone.AudioOutputType spdifOutput,
            AudioZone.AudioOutputType usbOutput, AudioZone.AudioOutputType usbOutputA, AudioZone.AudioOutputType usbOutputB, AudioZone.AudioOutputType usbOutputC, AudioZone.AudioOutputType usbOutputD, 
            AudioZone.AudioMixMode audioMixMode,            
            string audioVolume, string minimumVolume, string maximumVolume) :
            base(name, xStart, yStart, width, height, zoneType, id)
        {
            _audioOutput = audioOutput;
            _audioMode = audioMode;
            _audioMapping = audioMapping;

            _analogOutput = analogOutput;
            _analog2Output = analog2Output;
            _analog3Output = analog3Output;
            _hdmiOutput = hdmiOutput;
            _spdifOutput = spdifOutput;
            _usbOutput = usbOutput;
            _usbOutputA = usbOutputA;
            _usbOutputB = usbOutputB;
            _usbOutputC = usbOutputC;
            _usbOutputD = usbOutputD;
            _audioZoneMixMode = audioMixMode;

            _audioVolume = audioVolume;
            _minimumVolume = minimumVolume;
            _maximumVolume = maximumVolume;
        }

        public AudioOutputSelection AudioOutput
        {
            get { return _audioOutput; }
            set { _audioOutput = value; }
        }

        public AudioModeSelection AudioMode
        {
            get { return _audioMode; }
            set { _audioMode = value; }
        }

        public AudioMappingSelection AudioMapping
        {
            get { return _audioMapping; }
            set { _audioMapping = value; }
        }

        public AudioZone.AudioOutputType AnalogOutput
        {
            get { return _analogOutput; }
            set { _analogOutput = value; }
        }

        public AudioZone.AudioOutputType Analog2Output
        {
            get { return _analog2Output; }
            set { _analog2Output = value; }
        }

        public AudioZone.AudioOutputType Analog3Output
        {
            get { return _analog3Output; }
            set { _analog3Output = value; }
        }

        public AudioZone.AudioOutputType HDMIOutput
        {
            get { return _hdmiOutput; }
            set { _hdmiOutput = value; }
        }

        public AudioZone.AudioOutputType SPDIFOutput
        {
            get { return _spdifOutput; }
            set { _spdifOutput = value; }
        }

        public AudioZone.AudioOutputType USBOutput
        {
            get { return _usbOutput; }
            set { _usbOutput = value; }
        }

        public AudioZone.AudioOutputType USBOutputA
        {
            get { return _usbOutputA; }
            set { _usbOutputA = value; }
        }

        public AudioZone.AudioOutputType USBOutputB
        {
            get { return _usbOutputB; }
            set { _usbOutputB = value; }
        }

        public AudioZone.AudioOutputType USBOutputC
        {
            get { return _usbOutputC; }
            set { _usbOutputC = value; }
        }

        public AudioZone.AudioOutputType USBOutputD
        {
            get { return _usbOutputD; }
            set { _usbOutputD = value; }
        }

        public AudioZone.AudioMixMode AudioZoneMixMode
        {
            get { return _audioZoneMixMode; }
            set { _audioZoneMixMode = value; }
        }

        public string AudioVolume
        {
            get { return _audioVolume; }
            set { _audioVolume = value; }
        }

        public string MinimumVolume
        {
            get { return _minimumVolume; }
            set { _minimumVolume = value; }
        }

        public string MaximumVolume
        {
            get { return _maximumVolume; }
            set { _maximumVolume = value; }
        }

        public override int UsedAudioDecoders()
        {
            return 1;
        }

        public override object Clone() // ICloneable implementation
        {
            AudioZone audioZone = new AudioZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID, 
                this.AudioOutput, this.AudioMode, this.AudioMapping,
                this.AnalogOutput, this.Analog2Output, this.Analog3Output, this.HDMIOutput, this.SPDIFOutput,
                this.USBOutput, this.USBOutputA, this.USBOutputB, this.USBOutputC, this.USBOutputD, 
                this.AudioZoneMixMode,
                this.AudioVolume, this.MinimumVolume, this.MaximumVolume);

            return base.Copy(audioZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            AudioZone audioZone = (AudioZone)obj;
            if ((audioZone.AudioOutput != this.AudioOutput) ||
                (audioZone.AudioMode != this.AudioMode) ||
                (audioZone.AudioMapping != this.AudioMapping) ||
                (audioZone.AnalogOutput != this.AnalogOutput) ||
                (audioZone.Analog2Output != this.Analog2Output) ||
                (audioZone.Analog3Output != this.Analog3Output) ||
                (audioZone.HDMIOutput != this.HDMIOutput) ||
                (audioZone.SPDIFOutput != this.SPDIFOutput) ||
                (audioZone.USBOutput != this.USBOutput) ||
                (audioZone.USBOutputA != this.USBOutputA) ||
                (audioZone.USBOutputB != this.USBOutputB) ||
                (audioZone.USBOutputC != this.USBOutputC) ||
                (audioZone.USBOutputD != this.USBOutputD) ||
                (audioZone.AudioZoneMixMode != this.AudioZoneMixMode) ||
                (audioZone.AudioVolume != this.AudioVolume) ||
                (audioZone.MinimumVolume != this.MinimumVolume) ||
                (audioZone.MaximumVolume != this.MaximumVolume)
                )
            {
                return false;
            }
            return base.IsEqual(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

//        protected bool EditAudioParameters(Window1 parent, EditAudioZoneDlg editAudioZoneDlg)
//        {
//            editAudioZoneDlg.Owner = parent;
//
//            editAudioZoneDlg.SetAudioSettings(AudioOutput, AudioMode, AudioMapping);
//            editAudioZoneDlg.AnalogOutput = AnalogOutput;
//            editAudioZoneDlg.Analog2Output = Analog2Output;
//            editAudioZoneDlg.Analog3Output = Analog3Output;
//            editAudioZoneDlg.HDMIOutput = HDMIOutput;
//            editAudioZoneDlg.SPDIFOutput = SPDIFOutput;
//            editAudioZoneDlg.USBOutput = USBOutput;
//            editAudioZoneDlg.USBOutputA = USBOutputA;
//            editAudioZoneDlg.USBOutputB = USBOutputB;
//            editAudioZoneDlg.USBOutputC = USBOutputC;
//            editAudioZoneDlg.USBOutputD = USBOutputD;
//            editAudioZoneDlg.AudioMixMode = AudioZoneMixMode;
//            editAudioZoneDlg.InitialAudioVolume = AudioVolume;
//            editAudioZoneDlg.MinimumVolume = MinimumVolume;
//            editAudioZoneDlg.MaximumVolume = MaximumVolume;
//
//            if (editAudioZoneDlg.ShowDialog() == true)
//            {
//                AudioOutputSelection audioOutput;
//                AudioModeSelection audioMode;
//                AudioMappingSelection audioMapping;
//
//                editAudioZoneDlg.GetAudioSettings(out audioOutput, out audioMode, out audioMapping);
//                AudioOutput = audioOutput; 
//                AudioMode = audioMode;
//                AudioMapping = audioMapping;
//
//                AnalogOutput = editAudioZoneDlg.AnalogOutput;
//                Analog2Output = editAudioZoneDlg.Analog2Output;
//                Analog3Output = editAudioZoneDlg.Analog3Output;
//                HDMIOutput = editAudioZoneDlg.HDMIOutput;
//                SPDIFOutput = editAudioZoneDlg.SPDIFOutput;
//                USBOutput = editAudioZoneDlg.USBOutput;
//                USBOutputA = editAudioZoneDlg.USBOutputA;
//                USBOutputB = editAudioZoneDlg.USBOutputB;
//                USBOutputC = editAudioZoneDlg.USBOutputC;
//                USBOutputD = editAudioZoneDlg.USBOutputD;
//                AudioZoneMixMode = editAudioZoneDlg.AudioMixMode;
//
//                AudioVolume = editAudioZoneDlg.InitialAudioVolume;
//                MinimumVolume = editAudioZoneDlg.MinimumVolume;
//                MaximumVolume = editAudioZoneDlg.MaximumVolume;
//
//                if (UserPreferences.SavePropertiesForAllFuture)
//                {
//                    UserPreferences.InitialAudioVolume = editAudioZoneDlg.InitialAudioVolume;
//
//                    UserPreferences.AudioOutput = BrightAuthorUtils.GetAudioOutputSpec(audioOutput);
//                    UserPreferences.AudioMode = BrightAuthorUtils.GetAudioModeSpec(audioMode);
//                    UserPreferences.AudioMapping = BrightAuthorUtils.GetAudioMappingSpec(audioMapping);
//
//                    BrightAuthorUtils.UpdateAudioUserPreferenceParameters(parent.Sign.Model, AnalogOutput, Analog2Output, Analog3Output, SPDIFOutput,
//                        USBOutput, USBOutputA,USBOutputB, USBOutputC, USBOutputD);
//                    UserPreferences.HDMIOutput = HDMIOutput;
//                    UserPreferences.AudioMixMode = AudioZoneMixMode;
//                }
//
//                return true;
//            }
//
//            return false;
//        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditAudioZoneDlg editAudioZoneDlg = new EditAudioZoneDlg();
//
//            EditAudioParameters(parent, editAudioZoneDlg);
//        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteElementString("audioOutput", BrightAuthorUtils.GetAudioOutputSpec(AudioOutput));
            writer.WriteElementString("audioMode", BrightAuthorUtils.GetAudioModeSpec(AudioMode));
            writer.WriteElementString("audioMapping", BrightAuthorUtils.GetAudioMappingSpec(AudioMapping));
            writer.WriteElementString("analogOutput", AnalogOutput.ToString());
            writer.WriteElementString("analog2Output", Analog2Output.ToString());
            writer.WriteElementString("analog3Output", Analog3Output.ToString());
            writer.WriteElementString("hdmiOutput", HDMIOutput.ToString());
            writer.WriteElementString("spdifOutput", SPDIFOutput.ToString());
            writer.WriteElementString("usbOutput", USBOutput.ToString());
            writer.WriteElementString("usbOutputA", USBOutputA.ToString());
            writer.WriteElementString("usbOutputB", USBOutputB.ToString());
            writer.WriteElementString("usbOutputC", USBOutputC.ToString());
            writer.WriteElementString("usbOutputD", USBOutputD.ToString());
            writer.WriteElementString("audioMixMode", AudioZoneMixMode.ToString());
            writer.WriteElementString("audioVolume", AudioVolume);
            writer.WriteElementString("minimumVolume", MinimumVolume);
            writer.WriteElementString("maximumVolume", MaximumVolume);
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out AudioOutputSelection audioOutput, out AudioModeSelection audioMode, out AudioMappingSelection audioMapping,
            out AudioZone.AudioOutputType analogOutput, out AudioZone.AudioOutputType analog2Output, out AudioZone.AudioOutputType analog3Output, out AudioZone.AudioOutputType hdmiOutput, out AudioZone.AudioOutputType spdifOutput,
            out AudioZone.AudioOutputType usbOutput, out AudioZone.AudioOutputType usbOutputA, out AudioZone.AudioOutputType usbOutputB, out AudioZone.AudioOutputType usbOutputC, out AudioZone.AudioOutputType usbOutputD,
            out AudioZone.AudioMixMode audioMixMode,
            out string audioVolume, out string minimumVolume, out string maximumVolume)
        {
            audioOutput = AudioOutputSelection.AnalogAudio;
            audioMode = AudioModeSelection.MultichannelSurround;
            audioMapping = AudioMappingSelection.Audio1;

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

            audioVolume = UserPreferences.InitialAudioVolume;
            minimumVolume = "0";
            maximumVolume = "100";

            bool updatedAudioParametersFound = false;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                bool parameterFound = AssignAudioXmlValue(reader, ref audioOutput, ref audioMode, ref audioMapping, 
                    ref analogOutput, ref analog2Output, ref analog3Output, ref hdmiOutput, ref spdifOutput,
                    ref usbOutput, ref usbOutputA, ref usbOutputB, ref usbOutputC, ref usbOutputD, 
                    ref audioMixMode,
                    ref audioVolume, ref minimumVolume, ref maximumVolume);

                string localName = reader.LocalName;
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

        protected static bool AssignAudioXmlValue(XmlReader reader, ref AudioOutputSelection audioOutput, ref AudioModeSelection audioMode, ref AudioMappingSelection audioMapping,
            ref AudioZone.AudioOutputType analogOutput, ref AudioZone.AudioOutputType analog2Output, ref AudioZone.AudioOutputType analog3Output, ref AudioZone.AudioOutputType hdmiOutput, ref AudioZone.AudioOutputType spdifOutput,
            ref AudioZone.AudioOutputType usbOutput, ref AudioZone.AudioOutputType usbOutputA, ref AudioZone.AudioOutputType usbOutputB, ref AudioZone.AudioOutputType usbOutputC, ref AudioZone.AudioOutputType usbOutputD, 
            ref AudioZone.AudioMixMode audioMixMode,
            ref string audioVolume, ref string minimumVolume, ref string maximumVolume)
        {
            switch (reader.LocalName)
            {
                case "audioOutput":
                    string audioOutputStr = reader.ReadString();
                    audioOutput = BrightAuthorUtils.GetAudioOutputEnum(audioOutputStr);
                    break;
                case "audioMode":
                    string audioModeStr = reader.ReadString();
                    audioMode = BrightAuthorUtils.GetAudioModeEnum(audioModeStr);
                    break;
                case "audioMapping":
                    string audioMappingStr = reader.ReadString();
                    audioMapping = BrightAuthorUtils.GetAudioMappingEnum(audioMappingStr);
                    break;
                case "analogOutput":
                    string analogOutputStr = reader.ReadString();
                    analogOutput = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), analogOutputStr);
                    break;
                case "analog2Output":
                    string analog2OutputStr = reader.ReadString();
                    analog2Output = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), analog2OutputStr);
                    break;
                case "analog3Output":
                    string analog3OutputStr = reader.ReadString();
                    analog3Output = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), analog3OutputStr);
                    break;
                case "hdmiOutput":
                    string hdmiOutputStr = reader.ReadString();
                    hdmiOutput = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), hdmiOutputStr);
                    break;
                case "spdifOutput":
                    string spdifOutputStr = reader.ReadString();
                    spdifOutput = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), spdifOutputStr);
                    break;
                case "usbOutput":
                    string usbOutputStr = reader.ReadString();
                    usbOutput = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), usbOutputStr);
                    break;
                case "usbOutputA":
                    string usbOutputAStr = reader.ReadString();
                    usbOutputA = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), usbOutputAStr);
                    break;
                case "usbOutputB":
                    string usbOutputBStr = reader.ReadString();
                    usbOutputB = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), usbOutputBStr);
                    break;
                case "usbOutputC":
                    string usbOutputCStr = reader.ReadString();
                    usbOutputC = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), usbOutputCStr);
                    break;
                case "usbOutputD":
                    string usbOutputDStr = reader.ReadString();
                    usbOutputD = (AudioZone.AudioOutputType)Enum.Parse(typeof(AudioZone.AudioOutputType), usbOutputDStr);
                    break;
                case "audioMixMode":
                    string audioMixModeStr = reader.ReadString();
                    audioMixMode = (AudioZone.AudioMixMode)Enum.Parse(typeof(AudioZone.AudioMixMode), audioMixModeStr);
                    break;
                case "audioVolume":
                    audioVolume = reader.ReadString();
                    break;
                case "minimumVolume":
                    minimumVolume = reader.ReadString();
                    break;
                case "maximumVolume":
                    maximumVolume = reader.ReadString();
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void UpdateAudioParameters(AudioZone.AudioOutputSelection audioOutput, AudioZone.AudioModeSelection audioMode,
            ref AudioZone.AudioOutputType analogOutput, ref AudioZone.AudioOutputType analog2Output, ref AudioZone.AudioOutputType analog3Output, ref AudioZone.AudioOutputType hdmiOutput, ref AudioZone.AudioOutputType spdifOutput,
            ref AudioZone.AudioOutputType usbOutput, ref AudioZone.AudioOutputType usbOutputA, ref AudioZone.AudioOutputType usbOutputB, ref AudioZone.AudioOutputType usbOutputC, ref AudioZone.AudioOutputType usbOutputD, 
            ref AudioZone.AudioMixMode audioMixMode)
        {
            analog2Output = AudioZone.AudioOutputType.None;
            analog3Output = AudioZone.AudioOutputType.None;
            usbOutput = AudioZone.AudioOutputType.None;
            usbOutputA = AudioZone.AudioOutputType.None;
            usbOutputB = AudioZone.AudioOutputType.None;
            usbOutputC = AudioZone.AudioOutputType.None;
            usbOutputD = AudioZone.AudioOutputType.None;

            if (audioMode == AudioZone.AudioModeSelection.NoAudio)
            {
                analogOutput = AudioZone.AudioOutputType.None;
                hdmiOutput = AudioZone.AudioOutputType.None;
                spdifOutput = AudioZone.AudioOutputType.None;
                audioMixMode = AudioZone.AudioMixMode.Stereo;
            }
            else
            {
                switch (audioOutput)
                {
                    case AudioZone.AudioOutputSelection.AnalogAudio:
                        analogOutput = AudioZone.AudioOutputType.PCM;
                        hdmiOutput = AudioZone.AudioOutputType.PCM;
                        spdifOutput = AudioZone.AudioOutputType.None;
                        break;
                    case AudioZone.AudioOutputSelection.DigitalAudioRawAC3:
                        analogOutput = AudioZone.AudioOutputType.None;
                        hdmiOutput = AudioZone.AudioOutputType.PassThrough;
                        spdifOutput = AudioZone.AudioOutputType.PassThrough;
                        break;
                    case AudioZone.AudioOutputSelection.DigitalAudioStereoPCM:
                        analogOutput = AudioZone.AudioOutputType.None;
                        hdmiOutput = AudioZone.AudioOutputType.PCM;
                        spdifOutput = AudioZone.AudioOutputType.PCM;
                        break;
                    case AudioZone.AudioOutputSelection.OnboardAnalogHDMIMirrowRaw:
                        analogOutput = AudioZone.AudioOutputType.PCM;
                        hdmiOutput = AudioZone.AudioOutputType.PCM;
                        spdifOutput = AudioZone.AudioOutputType.PCM;
                        break;
                    case AudioZone.AudioOutputSelection.USBAudio:
                        break;
                }
                switch (audioMode)
                {
                    case AudioZone.AudioModeSelection.MixedDownToStereo:
                        audioMixMode = AudioZone.AudioMixMode.Stereo;
                        break;
                    case AudioZone.AudioModeSelection.MonoLeftMixdown:
                        audioMixMode = AudioZone.AudioMixMode.Left;
                        break;
                    case AudioZone.AudioModeSelection.MonoRightMixdown:
                        audioMixMode = AudioZone.AudioMixMode.Right;
                        break;
                    case AudioZone.AudioModeSelection.MultichannelSurround:
                        break;
                }
            }
        }
    }
}
