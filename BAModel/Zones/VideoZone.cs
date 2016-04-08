using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Windows.Media;
using System.Windows;

namespace BAModel
{
    public class VideoZone : Zone
    {
//        private Window1 _mainWindow = null;
        private int _viewMode;

        private bool _mosaic = false;
        private MosaicDecoder.MaxContentResolution _maxResolution = MosaicDecoder.MaxContentResolution._NotApplicable;

        private string _mosaicDecoderName = String.Empty;
        public string MosaicDecoderName
        {
            get { return _mosaicDecoderName; }
            set { _mosaicDecoderName = value; }
        }

        private AudioZone.AudioOutputSelection _audioOutput;
        private AudioZone.AudioModeSelection _audioMode;
        private AudioZone.AudioMappingSelection _audioMapping;

        private AudioZone.AudioOutputType _analogOutput;
        private AudioZone.AudioOutputType _analog2Output;
        private AudioZone.AudioOutputType _analog3Output;
        private AudioZone.AudioOutputType _hdmiOutput;
        private AudioZone.AudioOutputType _spdifOutput;
        private AudioZone.AudioOutputType _usbOutput;
        private AudioZone.AudioOutputType _usbOutputA;
        private AudioZone.AudioOutputType _usbOutputB;
        private AudioZone.AudioOutputType _usbOutputC;
        private AudioZone.AudioOutputType _usbOutputD;
        private AudioZone.AudioMixMode _audioMixMode;

        string _videoVolume;
        string _audioVolume;

        string _minimumVolume;
        string _maximumVolume;

        string _liveVideoInput;
        string _liveVideoStandard;
        string _brightness;
        string _contrast;
        string _saturation;
        string _hue;

        private bool _zOrderFront = true;

        // pseudo zone used for mosaic mode calculations
        public VideoZone() :
            base ("tmp", 0, 0, 1920, 1080, "", "")
        {
        }

        public VideoZone(string name, int xStart, int yStart, int width, int height, string zoneType, string id,
            int viewMode, AudioZone.AudioOutputSelection audioOutput, AudioZone.AudioModeSelection audioMode, AudioZone.AudioMappingSelection audioMapping,
            AudioZone.AudioOutputType analogOutput, AudioZone.AudioOutputType analog2Output, AudioZone.AudioOutputType analog3Output, AudioZone.AudioOutputType hdmiOutput, AudioZone.AudioOutputType spdifOutput,
            AudioZone.AudioOutputType usbOutput, AudioZone.AudioOutputType usbOutputA, AudioZone.AudioOutputType usbOutputB, AudioZone.AudioOutputType usbOutputC, AudioZone.AudioOutputType usbOutputD, 
            AudioZone.AudioMixMode audioMixMode,
            string videoVolume, string audioVolume, string minimumVolume, string maximumVolume,
            string liveVideoInput, string liveVideoStandard, string brightness, string contrast, string saturation, string hue, bool zOrderFront,
            bool mosaic, MosaicDecoder.MaxContentResolution maxResolution) :
            base(name, xStart, yStart, width, height, zoneType, id)
        {
            _mainWindow = Window1.GetInstance();

            // if mosaic mode, only 'Scale to Fill' (0) is supposed
            if (mosaic)
            {
                _viewMode = 0;
            }
            else
            {
                _viewMode = viewMode;
            }

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
            _audioMixMode = audioMixMode;

            _videoVolume = videoVolume;
            _audioVolume = audioVolume;

            _minimumVolume = minimumVolume;
            _maximumVolume = maximumVolume;

            _liveVideoInput = liveVideoInput;
            _liveVideoStandard = liveVideoStandard;
            _brightness = brightness;
            _contrast = contrast;
            _saturation = saturation;
            _hue = hue;

            _zOrderFront = zOrderFront;

            _mosaic = mosaic;
            _maxResolution = maxResolution;
        }

        public bool Mosaic
        {
            get { return _mosaic; }
            set { _mosaic = value; }
        }

        public MosaicDecoder.MaxContentResolution MaxResolution
        {
            get { return _maxResolution; }
            set { _maxResolution = value; }
        }

        public bool ZOrderFront
        {
            get { return _zOrderFront; }
            set { _zOrderFront = value; }
        }

        public int ViewMode
        {
            get { return _viewMode; }
            set { _viewMode = value; }
        }

        public AudioZone.AudioOutputSelection AudioOutput
        {
            get { return _audioOutput; }
            set { _audioOutput = value; }
        }

        public AudioZone.AudioModeSelection AudioMode
        {
            get { return _audioMode; }
            set { _audioMode = value; }
        }

        public AudioZone.AudioMappingSelection AudioMapping
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

        public AudioZone.AudioMixMode AudioMixMode
        {
            get { return _audioMixMode; }
            set { _audioMixMode = value; }
        }

        public string VideoVolume
        {
            get { return _videoVolume; }
            set { _videoVolume = value; }
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

        public string LiveVideoInput
        {
            get { return _liveVideoInput; }
            set { _liveVideoInput = value; }
        }

        public string LiveVideoStandard
        {
            get { return _liveVideoStandard; }
            set { _liveVideoStandard = value; }
        }

        public string Brightness
        {
            get { return _brightness; }
            set { _brightness = value; }
        }

        public string Contrast
        {
            get { return _contrast; }
            set { _contrast = value; }
        }

        public string Saturation
        {
            get { return _saturation; }
            set { _saturation = value; }
        }

        public string Hue
        {
            get { return _hue; }
            set { _hue = value; }
        }

        public override int UsedAudioDecoders()
        {
            //return (AudioMode == AudioZone.AudioModeSelection.NoAudio) ? 0 : 1;

            if ((AnalogOutput != AudioZone.AudioOutputType.None) ||
                 (Analog2Output != AudioZone.AudioOutputType.None) ||
                 (Analog3Output != AudioZone.AudioOutputType.None) ||
                 (USBOutput != AudioZone.AudioOutputType.None) ||
                 (USBOutputA != AudioZone.AudioOutputType.None) ||
                 (USBOutputB != AudioZone.AudioOutputType.None) ||
                 (USBOutputC != AudioZone.AudioOutputType.None) ||
                 (HDMIOutput != AudioZone.AudioOutputType.None) ||
                 (SPDIFOutput != AudioZone.AudioOutputType.None)
                )
            {
                return 1;
            }
            return 0;
        }

        public override object Clone() // ICloneable implementation
        {
            VideoZone VideoZone = new VideoZone(this.Name, this.X, this.Y, this.Width, this.Height, this.ZoneType, this.ZoneID,
                this.ViewMode, this.AudioOutput, this.AudioMode, this.AudioMapping,
                this.AnalogOutput, this.Analog2Output, this.Analog3Output, this.HDMIOutput, this.SPDIFOutput,
                this.USBOutput, this.USBOutputA, this.USBOutputB, this.USBOutputC, this.USBOutputD, 
                this.AudioMixMode,
                this.VideoVolume, this.AudioVolume, this.MinimumVolume, this.MaximumVolume,
                this.LiveVideoInput, this.LiveVideoStandard, this.Brightness, this.Contrast, this.Saturation, this.Hue, this.ZOrderFront,
                this.Mosaic, this.MaxResolution);

            return base.Copy(VideoZone);
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            VideoZone VideoZone = (VideoZone)obj;
            if ((VideoZone.ViewMode != this.ViewMode) ||
                 (VideoZone.AudioOutput != this.AudioOutput) ||
                 (VideoZone.AudioMode != this.AudioMode) ||
                 (VideoZone.AudioMapping != this.AudioMapping) ||
                 (VideoZone.AnalogOutput != this.AnalogOutput) ||
                 (VideoZone.Analog2Output != this.Analog2Output) ||
                 (VideoZone.Analog3Output != this.Analog3Output) ||
                 (VideoZone.HDMIOutput != this.HDMIOutput) ||
                 (VideoZone.SPDIFOutput != this.SPDIFOutput) ||
                 (VideoZone.USBOutput != this.USBOutput) ||
                 (VideoZone.USBOutputA != this.USBOutputA) ||
                 (VideoZone.USBOutputB != this.USBOutputB) ||
                 (VideoZone.USBOutputC != this.USBOutputC) ||
                 (VideoZone.USBOutputD != this.USBOutputD) ||
                 (VideoZone.AudioMixMode != this.AudioMixMode) ||
                 (VideoZone.VideoVolume != this.VideoVolume) ||
                 (VideoZone.AudioVolume != this.AudioVolume) ||
                 (VideoZone.MinimumVolume != this.MinimumVolume) ||
                 (VideoZone.MaximumVolume != this.MaximumVolume) ||
                 (VideoZone.LiveVideoInput != this.LiveVideoInput) ||
                 (VideoZone.LiveVideoStandard != this.LiveVideoStandard) ||
                 (VideoZone.Brightness != this.Brightness) ||
                 (VideoZone.Contrast != this.Contrast) ||
                 (VideoZone.Saturation != this.Saturation) ||
                 (VideoZone.Hue != this.Hue) ||
                 (VideoZone.ZOrderFront != this.ZOrderFront) ||
                 (VideoZone.Mosaic != this.Mosaic) ||
                 (VideoZone.MaxResolution != this.MaxResolution)
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

//        protected void AssignVideoParameters(EditVideoImagesZoneDlg editVideoImagesZoneDlg)
//        {
//            editVideoImagesZoneDlg.ViewMode = BrightAuthorUtils.GetViewModeSpec(ViewMode);
//            editVideoImagesZoneDlg.SetAudioSettings(AudioOutput, AudioMode, AudioMapping);
//            editVideoImagesZoneDlg.AnalogOutput = AnalogOutput;
//            editVideoImagesZoneDlg.Analog2Output = Analog2Output;
//            editVideoImagesZoneDlg.Analog3Output = Analog3Output;
//            editVideoImagesZoneDlg.HDMIOutput = HDMIOutput;
//            editVideoImagesZoneDlg.SPDIFOutput = SPDIFOutput;
//            editVideoImagesZoneDlg.USBOutput = USBOutput;
//            editVideoImagesZoneDlg.USBOutputA = USBOutputA;
//            editVideoImagesZoneDlg.USBOutputB = USBOutputB;
//            editVideoImagesZoneDlg.USBOutputC = USBOutputC;
//            editVideoImagesZoneDlg.USBOutputD = USBOutputD;
//            editVideoImagesZoneDlg.AudioMixMode = AudioMixMode;
//            editVideoImagesZoneDlg.LiveVideoInput = LiveVideoInput;
//            editVideoImagesZoneDlg.LiveVideoStandard = LiveVideoStandard;
//            editVideoImagesZoneDlg.InitialVideoVolume = VideoVolume;
//            editVideoImagesZoneDlg.InitialAudioVolume = AudioVolume;
//            editVideoImagesZoneDlg.MinimumVolume = MinimumVolume;
//            editVideoImagesZoneDlg.MaximumVolume = MaximumVolume;
//            editVideoImagesZoneDlg.Brightness = Brightness;
//            editVideoImagesZoneDlg.Contrast = Contrast;
//            editVideoImagesZoneDlg.Saturation = Saturation;
//            editVideoImagesZoneDlg.Hue = Hue;
//            editVideoImagesZoneDlg.MaxContentResolution = MaxResolution;
//        }

//        protected void RetrieveVideoParameters(EditVideoImagesZoneDlg editVideoImagesZoneDlg, bool saveUserPreferences)
//        {
//            ViewMode = BrightAuthorUtils.GetViewModeValue(editVideoImagesZoneDlg.ViewMode);
//
//            AudioZone.AudioOutputSelection audioOutput;
//            AudioZone.AudioModeSelection audioMode;
//            AudioZone.AudioMappingSelection audioMapping;
//
//            editVideoImagesZoneDlg.GetAudioSettings(out audioOutput, out audioMode, out audioMapping);
//            AudioOutput = audioOutput;
//            AudioMode = audioMode;
//            AudioMapping = audioMapping;
//
//            AnalogOutput = editVideoImagesZoneDlg.AnalogOutput;
//            Analog2Output = editVideoImagesZoneDlg.Analog2Output;
//            Analog3Output = editVideoImagesZoneDlg.Analog3Output;
//
//            HDMIOutput = editVideoImagesZoneDlg.HDMIOutput;
//            SPDIFOutput = editVideoImagesZoneDlg.SPDIFOutput;
//            USBOutput = editVideoImagesZoneDlg.USBOutput;
//            USBOutputA = editVideoImagesZoneDlg.USBOutputA;
//            USBOutputB = editVideoImagesZoneDlg.USBOutputB;
//            USBOutputC = editVideoImagesZoneDlg.USBOutputC;
//            USBOutputD = editVideoImagesZoneDlg.USBOutputD;
//            AudioMixMode = editVideoImagesZoneDlg.AudioMixMode;
//
//            VideoVolume = editVideoImagesZoneDlg.InitialVideoVolume;
//            AudioVolume = editVideoImagesZoneDlg.InitialAudioVolume;
//            MinimumVolume = editVideoImagesZoneDlg.MinimumVolume;
//            MaximumVolume = editVideoImagesZoneDlg.MaximumVolume;
//            LiveVideoInput = editVideoImagesZoneDlg.LiveVideoInput;
//            LiveVideoStandard = editVideoImagesZoneDlg.LiveVideoStandard;
//            Brightness = editVideoImagesZoneDlg.Brightness;
//            Contrast = editVideoImagesZoneDlg.Contrast;
//            Saturation = editVideoImagesZoneDlg.Saturation;
//            Hue = editVideoImagesZoneDlg.Hue;
//
//            if (saveUserPreferences)
//            {
//                UserPreferences.ViewMode = editVideoImagesZoneDlg.ViewMode;
//
//                UserPreferences.InitialVideoVolume = editVideoImagesZoneDlg.InitialVideoVolume;
//                UserPreferences.InitialAudioVolume = editVideoImagesZoneDlg.InitialAudioVolume;
//
//                UserPreferences.AudioOutput = BrightAuthorUtils.GetAudioOutputSpec(audioOutput);
//                UserPreferences.AudioMode = BrightAuthorUtils.GetAudioModeSpec(audioMode);
//                UserPreferences.AudioMapping = BrightAuthorUtils.GetAudioMappingSpec(audioMapping);
//
//                BrightAuthorUtils.UpdateAudioUserPreferenceParameters(_mainWindow.Sign.Model, AnalogOutput, Analog2Output, Analog3Output, SPDIFOutput,
//                    USBOutput, USBOutputA, USBOutputB, USBOutputC, USBOutputD);
//                UserPreferences.HDMIOutput = HDMIOutput;
//                UserPreferences.AudioMixMode = AudioMixMode;
//
//                UserPreferences.LiveVideoInput = LiveVideoInput;
//                UserPreferences.LiveVideoStandard = LiveVideoStandard;
//                UserPreferences.Brightness = Brightness;
//                UserPreferences.Contrast = Contrast;
//                UserPreferences.Saturation = Saturation;
//                UserPreferences.Hue = Hue;
//            }
//        }

//        public override void EditZoneParameters(Window1 parent)
//        {
//            EditVideoImagesZoneDlg editVideoImagesZoneDlg = new EditVideoImagesZoneDlg(false, !Mosaic, Mosaic);
//            editVideoImagesZoneDlg.Owner = parent;
//
//            AssignVideoParameters(editVideoImagesZoneDlg);
//
//            if (editVideoImagesZoneDlg.ShowDialog() == true)
//            {
//                RetrieveVideoParameters(editVideoImagesZoneDlg, UserPreferences.SavePropertiesForAllFuture);
//            }
//        }

        public override void WriteZoneSpecificDataToXml(XmlTextWriter writer, bool publish, Sign sign)
        {
            writer.WriteElementString("viewMode", BrightAuthorUtils.GetViewModeSpec(ViewMode));
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
            writer.WriteElementString("audioMixMode", AudioMixMode.ToString());
            writer.WriteElementString("videoVolume", VideoVolume);
            writer.WriteElementString("audioVolume", AudioVolume);
            writer.WriteElementString("minimumVolume", MinimumVolume);
            writer.WriteElementString("maximumVolume", MaximumVolume);
            writer.WriteElementString("liveVideoInput", LiveVideoInput);
            writer.WriteElementString("liveVideoStandard", LiveVideoStandard);
            writer.WriteElementString("brightness", Brightness);
            writer.WriteElementString("contrast", Contrast);
            writer.WriteElementString("saturation", Saturation);
            writer.WriteElementString("hue", Hue);
            writer.WriteElementString("zOrderFront", ZOrderFront.ToString());
            writer.WriteElementString("mosaic", Mosaic.ToString());
            writer.WriteElementString("maxContentResolution", MaxResolution.ToString());

            if (publish && Mosaic)
            {
                writer.WriteElementString("mosaicDecoderName", MosaicDecoderName);
            }
        }

        protected static bool ReadVideoZoneSpecificDataXml(XmlReader reader, ref int viewMode, ref AudioZone.AudioOutputSelection audioOutput, ref AudioZone.AudioModeSelection audioMode, ref AudioZone.AudioMappingSelection audioMapping,
            ref AudioZone.AudioOutputType analogOutput, ref AudioZone.AudioOutputType analog2Output, ref AudioZone.AudioOutputType analog3Output, ref AudioZone.AudioOutputType hdmiOutput, ref AudioZone.AudioOutputType spdifOutput,
            ref AudioZone.AudioOutputType usbOutput, ref AudioZone.AudioOutputType usbOutputA, ref AudioZone.AudioOutputType usbOutputB, ref AudioZone.AudioOutputType usbOutputC, ref AudioZone.AudioOutputType usbOutputD, 
            ref AudioZone.AudioMixMode audioMixMode, 
            ref string videoVolume, ref string audioVolume, ref string minimumVolume, ref string maximumVolume,
            ref string liveVideoInput, ref string liveVideoStandard, ref string brightness, ref string contrast, ref string saturation, ref string hue,
            ref bool zOrderFront, ref bool mosaic, ref MosaicDecoder.MaxContentResolution maxResolution)
        {
            switch (reader.LocalName)
            {
                case "viewMode":
                    string viewModeStr = reader.ReadString();
                    viewMode = BrightAuthorUtils.GetViewModeValue(viewModeStr);
                    break;
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
                case "videoVolume":
                    videoVolume = reader.ReadString();
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
                case "liveVideoInput":
                    liveVideoInput = reader.ReadString();
                    break;
                case "liveVideoStandard":
                    liveVideoStandard = reader.ReadString();
                    break;
                case "brightness":
                    brightness = reader.ReadString();
                    break;
                case "contrast":
                    contrast = reader.ReadString();
                    break;
                case "saturation":
                    saturation = reader.ReadString();
                    break;
                case "hue":
                    hue = reader.ReadString();
                    break;
                case "zOrderFront":
                    zOrderFront = Convert.ToBoolean(reader.ReadString());
                    break;
                case "mosaic":
                    mosaic = Convert.ToBoolean(reader.ReadString());
                    break;
                case "maxContentResolution":
                    maxResolution = (MosaicDecoder.MaxContentResolution)Enum.Parse(typeof(MosaicDecoder.MaxContentResolution), reader.ReadString());
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static void ReadZoneSpecificDataXml(XmlReader reader, out int viewMode, out AudioZone.AudioOutputSelection audioOutput, out AudioZone.AudioModeSelection audioMode, out AudioZone.AudioMappingSelection audioMapping,
            out AudioZone.AudioOutputType analogOutput, out AudioZone.AudioOutputType analog2Output, out AudioZone.AudioOutputType analog3Output, out AudioZone.AudioOutputType hdmiOutput, out AudioZone.AudioOutputType spdifOutput,
            out AudioZone.AudioOutputType usbOutput, out AudioZone.AudioOutputType usbOutputA, out AudioZone.AudioOutputType usbOutputB, out AudioZone.AudioOutputType usbOutputC, out AudioZone.AudioOutputType usbOutputD, 
            out AudioZone.AudioMixMode audioMixMode, 
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
