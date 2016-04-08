using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Drawing;

namespace BAModel
{
    public class BrightSignModel
    {
        public enum ModelType
        {
            HD222,
            HD1022,
            XD232,
            XD1032,
            XD1132,
            HD920,
            HD922,
            HD970,
            HD972,
            A915,
            HD917,
            HD120,
            HD220,
            LS322,
            LS422,
            HD1020,
            AU320,
            XD230,
            XD1030,
            XD1230,
            FK242,
            FK1042,
            FK1142,
            TK1042
        }

        public enum ModelFeature
        {
            GPIO,
            Serial,
            Networking,
            SingleOnBoardAnalogChannel,
            ThreeOnBoardAnalogChannels,
            BP900,
            USBAudio,
            PumaUSBAudio,
            AudioDownmix,
            AudioFeatures3_3,
            SingleAnalogVolumeLimitation,
            HDMI,
            AudioIn,
            BoseUSB,
            IROut,
            VideoStreaming,
            AudioStreaming,
            AudioMixer,
            Images,
            Video,
            Clock,
            USB,
            GraphicsZOrdering,
            VideoZOrdering,
            LiveVideoModule,
            RFTuner,
            HTML5,
            AudioOutputControl,
            NoAudioOutputControl,
            SPDIF,
            MonacoPandoraVolumeUI,
            BLC400,
            AudioDetect,
            ExternalAudioDetect,
            HDMIIn,
            EnhancedSynchronization,
            Pronto,
            ImportTimeCodes,
            ScrollingTicker,
            TenBitColor,
            FourKImages,
            MosaicMode,
            Null
        }

        public ModelType Model { get; set; }

        private Dictionary<ModelFeature, bool> features = new Dictionary<ModelFeature, bool>();
        public Dictionary<ModelFeature, bool> Features
        {
            get { return features; }
        }

        private int _numUSBPorts = 0;
        public int NumberOfUSBPorts
        {
            get { return _numUSBPorts; }
            set { _numUSBPorts = value; }
        }

        public static string GetFamilyFromModel(ModelType modelType)
        {
            switch (modelType)
            {
                case ModelType.HD222:
                case ModelType.HD1022:
                case ModelType.HD922:
                case ModelType.HD972:
                    return "Bobcat";
                case ModelType.HD920:
                case ModelType.HD970:
                case ModelType.A915:
                case ModelType.HD917:
                    return "Puma";
                case ModelType.LS322:
                case ModelType.LS422:
                case ModelType.HD120:
                case ModelType.HD220:
                case ModelType.HD1020:
                case ModelType.AU320:
                    return "Panther";
                case ModelType.XD230:
                case ModelType.XD1030:
                case ModelType.XD1230:
                    return "Cheetah";
                case ModelType.XD232:
                case ModelType.XD1032:
                case ModelType.XD1132:
                    return "Lynx";
                case ModelType.FK242:
                case ModelType.FK1042:
                case ModelType.FK1142:
                case ModelType.TK1042:
                    return "Tiger";
            }
            return "Panther";
        }

        public static ModelFeature GetModelFeature(string featureName)
        {
            switch (featureName)
            {
                case "gpio":
                    return ModelFeature.GPIO;
                case "serial":
                    return ModelFeature.Serial;
                case "networking":
                    return ModelFeature.Networking;
                case "singleOnBoardAnalogChannel":
                    return ModelFeature.SingleOnBoardAnalogChannel;
                case "threeOnBoardAnalogChannels":
                    return ModelFeature.ThreeOnBoardAnalogChannels;
                case "singleAnalogVolumeLimitation":
                    return ModelFeature.SingleAnalogVolumeLimitation;
                case "bp900":
                    return ModelFeature.BP900;
                case "usbAudio":
                    return ModelFeature.USBAudio;
                case "pumaUSBAudio":
                    return ModelFeature.PumaUSBAudio;
                case "audioDownmix":
                    return ModelFeature.AudioDownmix;
                case "audioFeatures3_3":
                    return ModelFeature.AudioFeatures3_3;
                case "HDMI":
                    return ModelFeature.HDMI;
                case "AudioIn":
                    return ModelFeature.AudioIn;
                case "boseUSB":
                    return ModelFeature.BoseUSB;
                case "irOut":
                    return ModelFeature.IROut;
                case "videoStreaming":
                    return ModelFeature.VideoStreaming;
                case "audioStreaming":
                    return ModelFeature.AudioStreaming;
                case "audioMixer":
                    return ModelFeature.AudioMixer;
                case "images":
                    return ModelFeature.Images;
                case "video":
                    return ModelFeature.Video;
                case "clock":
                    return ModelFeature.Clock;
                case "usb":
                    return ModelFeature.USB;
                case "atscTuner":
                    return ModelFeature.RFTuner;
                case "html5":
                    return ModelFeature.HTML5;
                case "audioOutputControl":
                    return ModelFeature.AudioOutputControl;
                case "noAudioOutputControl":
                    return ModelFeature.NoAudioOutputControl;
                case "spdif":
                    return ModelFeature.SPDIF;
                case "monacoPandoraVolumeUI":
                    return ModelFeature.MonacoPandoraVolumeUI;
                case "blc400":
                    return ModelFeature.BLC400;
                case "audioDetect":
                    return ModelFeature.AudioDetect;
                case "externalAudioDetect":
                    return ModelFeature.ExternalAudioDetect;
                case "hdmiIn":
                    return ModelFeature.HDMIIn;
                case "enhancedSynchronization":
                    return ModelFeature.EnhancedSynchronization;
                case "pronto":
                    return ModelFeature.Pronto;
                case "importTimeCodes":
                    return ModelFeature.ImportTimeCodes;
                case "scrollingTicker":
                    return ModelFeature.ScrollingTicker;
                case "tenBitColor":
                    return ModelFeature.TenBitColor;
                case "mosaicMode":
                    return ModelFeature.MosaicMode;
                default:
                    return ModelFeature.Null;
            }
        }

        public bool FeatureIsSupported(ModelFeature modelFeature)
        {
            if (features.ContainsKey(modelFeature)) return true;
            return false;
        }

        public bool ZoneTypeSupported(string zoneType)
        {
            switch (zoneType)
            {
                case "VideoOrImages":
                    return FeatureIsSupported(ModelFeature.Images) && FeatureIsSupported(ModelFeature.Video);
                case "BackgroundImage":
                case "VideoOnly":
                    return FeatureIsSupported(ModelFeature.Video);
                case "Images":
                case "Ticker":
                    return FeatureIsSupported(ModelFeature.Images);
                case "AudioOnly":
                    return true;
                case "EnhancedAudio":
                    return FeatureIsSupported(ModelFeature.AudioMixer);
                case "Clock":
                    return FeatureIsSupported(ModelFeature.Clock);
            }
            return false;
        }

//        public Size MaximumImageDimensions
//        {
//            get
//            {
//                Size maxSize = new Size(1920, 1080);
//                //switch (Model)
//                //{
//                //    case ModelType.A913:
//                //    case ModelType.A933:
//                //        maxSize = new Size(640, 480);
//                //        break;
//                //}
//
//                return maxSize;
//            }
//        }

        public int MaximumImageSize
        {
            get
            {
                return 1920 * 1280;
            }
        }

        public int MaximumImageSizeVideoPlayer
        {
            get
            {
                switch (Model)
                {
                    case ModelType.FK1042:
                    case ModelType.FK1142:
                    case ModelType.FK242:
                        return 3840 * 2160;
                    default:
                        return 1920 * 1080;
                }
            }
        }

        public string MaxVideoResolution
        {
            get
            {
                switch (Model)
                {
                    case ModelType.FK1042:
                    case ModelType.FK1142:
                    case ModelType.FK242:
                        return "4K";
                    default:
                        return "HD";
                }
            }
        }
    }

    public class BrightSignModelMgr
    {
        private static BrightSignModel currentBrightSignModel = null;
        private static BrightSignModel hd222 = null;
        private static BrightSignModel hd1022 = null;
        private static BrightSignModel xd232 = null;
        private static BrightSignModel xd1032 = null;
        private static BrightSignModel xd1132 = null;
        private static BrightSignModel a915 = null;
        private static BrightSignModel hd917 = null;
        private static BrightSignModel hd920 = null;
        private static BrightSignModel hd970 = null;
        private static BrightSignModel hd922 = null;
        private static BrightSignModel hd972 = null;
        private static BrightSignModel ls322 = null;
        private static BrightSignModel ls422 = null;
        private static BrightSignModel hd120 = null;
        private static BrightSignModel hd220 = null;
        private static BrightSignModel hd1020 = null;
        private static BrightSignModel au320 = null;
        private static BrightSignModel xd230 = null;
        private static BrightSignModel xd1030 = null;
        private static BrightSignModel xd1230 = null;
        private static BrightSignModel fk242 = null;
        private static BrightSignModel fk1042 = null;
        private static BrightSignModel fk1142 = null;
        private static BrightSignModel tk1042 = null;

        public static bool Model910Enabled { get; set; }
        public static bool Model913Enabled { get; set; }
        public static bool Model933Enabled { get; set; }
        public static bool Model960Enabled { get; set; }

        public static void InitializeBrightSignModelMgr()
        {
            Dictionary<BrightSignModel.ModelFeature, bool> features = null;

            // HD120
            hd120 = new BrightSignModel { Model = BrightSignModel.ModelType.HD120 };
            features = hd120.Features;
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);

            // HD220
            hd220 = new BrightSignModel { Model = BrightSignModel.ModelType.HD220 };
            features = hd220.Features;
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);

            // HD222
            hd222 = new BrightSignModel { Model = BrightSignModel.ModelType.HD222 };
            features = hd222.Features;
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // LS322
            ls322 = new BrightSignModel { Model = BrightSignModel.ModelType.LS422 };
            features = ls322.Features;
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);

            // LS422
            ls422 = new BrightSignModel { Model = BrightSignModel.ModelType.LS422 };
            features = ls422.Features;
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);

            // HD1020
            hd1020 = new BrightSignModel { Model = BrightSignModel.ModelType.HD1020 };
            features = hd1020.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);

            // HD1022
            hd1022 = new BrightSignModel { Model = BrightSignModel.ModelType.HD1022 };
            features = hd1022.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // XD230
            xd230 = new BrightSignModel { Model = BrightSignModel.ModelType.XD230 };
            features = xd230.Features;
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);

            // XD232
            xd232 = new BrightSignModel { Model = BrightSignModel.ModelType.XD232 };
            features = xd232.Features;
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // XD1030
            xd1030 = new BrightSignModel { Model = BrightSignModel.ModelType.XD1030 };
            features = xd1030.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);

            // XD1032
            xd1032 = new BrightSignModel { Model = BrightSignModel.ModelType.XD1032 };
            features = xd1032.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // XD1230
            xd1230 = new BrightSignModel { Model = BrightSignModel.ModelType.XD1230 };
            features = xd1230.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.RFTuner, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.HDMIIn, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);

            // XD1132
            xd1132 = new BrightSignModel { Model = BrightSignModel.ModelType.XD1132 };
            features = xd1132.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            //features.Add(BrightSignModel.ModelFeature.AudioFeatures3_3, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.HDMIIn, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // 4K242
            fk242 = new BrightSignModel { Model = BrightSignModel.ModelType.FK242 };
            features = fk242.Features;
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.TenBitColor, true);
            features.Add(BrightSignModel.ModelFeature.FourKImages, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // 4K1042
            fk1042 = new BrightSignModel { Model = BrightSignModel.ModelType.FK1042 };
            features = fk1042.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.TenBitColor, true);
            features.Add(BrightSignModel.ModelFeature.FourKImages, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // 4K1142
            fk1142 = new BrightSignModel { Model = BrightSignModel.ModelType.FK1142 };
            features = fk1142.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.HDMIIn, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            features.Add(BrightSignModel.ModelFeature.TenBitColor, true);
            features.Add(BrightSignModel.ModelFeature.FourKImages, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // 2K1042
            tk1042 = new BrightSignModel { Model = BrightSignModel.ModelType.TK1042 };
            features = tk1042.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.VideoZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ScrollingTicker, true);
            //features.Add(BrightSignModel.ModelFeature.TenBitColor, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);


            // A915
            a915 = new BrightSignModel { Model = BrightSignModel.ModelType.A915 };
            a915.NumberOfUSBPorts = 2;
            features = a915.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            //features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            //features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            //features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);

            // HD917
            hd917 = new BrightSignModel { Model = BrightSignModel.ModelType.HD917 };
            hd917.NumberOfUSBPorts = 2;
            features = hd917.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            //features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            //features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            //features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);


            // HD920
            hd920 = new BrightSignModel { Model = BrightSignModel.ModelType.HD920 };
            hd920.NumberOfUSBPorts = 2;
            features = hd920.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);

            // HD922
            hd922 = new BrightSignModel { Model = BrightSignModel.ModelType.HD922 };
            hd922.NumberOfUSBPorts = 2;
            features = hd922.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // HD970
            hd970 = new BrightSignModel { Model = BrightSignModel.ModelType.HD970 };
            hd970.NumberOfUSBPorts = 4;
            features = hd970.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.ThreeOnBoardAnalogChannels, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);

            // HD972
            hd972 = new BrightSignModel { Model = BrightSignModel.ModelType.HD972 };
            hd972.NumberOfUSBPorts = 4;
            features = hd972.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.GPIO, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.ThreeOnBoardAnalogChannels, true);
            features.Add(BrightSignModel.ModelFeature.HDMI, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.VideoStreaming, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.Images, true);
            features.Add(BrightSignModel.ModelFeature.Video, true);
            features.Add(BrightSignModel.ModelFeature.Clock, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.BoseUSB, true);
            features.Add(BrightSignModel.ModelFeature.IROut, true);
            features.Add(BrightSignModel.ModelFeature.AudioIn, true);
            features.Add(BrightSignModel.ModelFeature.PumaUSBAudio, true);
            features.Add(BrightSignModel.ModelFeature.BLC400, true);
            features.Add(BrightSignModel.ModelFeature.SPDIF, true);
            features.Add(BrightSignModel.ModelFeature.AudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.ExternalAudioDetect, true);
            features.Add(BrightSignModel.ModelFeature.EnhancedSynchronization, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
            features.Add(BrightSignModel.ModelFeature.ImportTimeCodes, true);
            features.Add(BrightSignModel.ModelFeature.HTML5, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.GraphicsZOrdering, true);
            features.Add(BrightSignModel.ModelFeature.MosaicMode, true);

            // AU320
            au320 = new BrightSignModel { Model = BrightSignModel.ModelType.AU320 };
            features = au320.Features;
            features.Add(BrightSignModel.ModelFeature.Serial, true);
            features.Add(BrightSignModel.ModelFeature.Networking, true);
            features.Add(BrightSignModel.ModelFeature.BP900, true);
            features.Add(BrightSignModel.ModelFeature.SingleAnalogVolumeLimitation, true);
            features.Add(BrightSignModel.ModelFeature.AudioMixer, true);
            features.Add(BrightSignModel.ModelFeature.AudioOutputControl, true);
            features.Add(BrightSignModel.ModelFeature.AudioStreaming, true);
            features.Add(BrightSignModel.ModelFeature.USB, true);
            features.Add(BrightSignModel.ModelFeature.Pronto, true);
        }

        public static BrightSignModel CurrentBrightSignModel
        {
            get { return currentBrightSignModel; }
        }

        public static void SetCurrentBrightSignModel(BrightSignModel.ModelType modelType)
        {
            switch (modelType)
            {
                case BrightSignModel.ModelType.HD120:
                    currentBrightSignModel = hd120;
                    break;
                case BrightSignModel.ModelType.HD220:
                    currentBrightSignModel = hd220;
                    break;
                case BrightSignModel.ModelType.HD222:
                    currentBrightSignModel = hd222;
                    break;
                case BrightSignModel.ModelType.XD230:
                    currentBrightSignModel = xd230;
                    break;
                case BrightSignModel.ModelType.XD232:
                    currentBrightSignModel = xd232;
                    break;
                case BrightSignModel.ModelType.HD1020:
                    currentBrightSignModel = hd1020;
                    break;
                case BrightSignModel.ModelType.HD1022:
                    currentBrightSignModel = hd1022;
                    break;
                case BrightSignModel.ModelType.XD1030:
                    currentBrightSignModel = xd1030;
                    break;
                case BrightSignModel.ModelType.XD1032:
                    currentBrightSignModel = xd1032;
                    break;
                case BrightSignModel.ModelType.XD1132:
                    currentBrightSignModel = xd1132;
                    break;
                case BrightSignModel.ModelType.XD1230:
                    currentBrightSignModel = xd1230;
                    break;
                case BrightSignModel.ModelType.FK242:
                    currentBrightSignModel = fk242;
                    break;
                case BrightSignModel.ModelType.FK1042:
                    currentBrightSignModel = fk1042;
                    break;
                case BrightSignModel.ModelType.FK1142:
                    currentBrightSignModel = fk1142;
                    break;
                case BrightSignModel.ModelType.A915:
                    currentBrightSignModel = a915;
                    break;
                case BrightSignModel.ModelType.HD917:
                    currentBrightSignModel = hd917;
                    break;
                case BrightSignModel.ModelType.HD920:
                    currentBrightSignModel = hd920;
                    break;
                case BrightSignModel.ModelType.HD922:
                    currentBrightSignModel = hd922;
                    break;
                case BrightSignModel.ModelType.HD970:
                    currentBrightSignModel = hd970;
                    break;
                case BrightSignModel.ModelType.HD972:
                    currentBrightSignModel = hd972;
                    break;
                case BrightSignModel.ModelType.AU320:
                    currentBrightSignModel = au320;
                    break;
                default:
                    currentBrightSignModel = hd120;
                    break;
            }
        }

        public static void SetCurrentBrightSignModel(string modelType)
        {
            switch (modelType)
            {
                case "HD120":
                    currentBrightSignModel = hd120;
                    break;
                case "HD220":
                    currentBrightSignModel = hd220;
                    break;
                case "HD222":
                    currentBrightSignModel = hd222;
                    break;
                case "XD230":
                    currentBrightSignModel = xd230;
                    break;
                case "XD232":
                    currentBrightSignModel = xd232;
                    break;
                case "LS322":
                    currentBrightSignModel = ls322;
                    break;
                case "LS422":
                    currentBrightSignModel = ls422;
                    break;
                case "HD1020":
                    currentBrightSignModel = hd1020;
                    break;
                case "HD1022":
                    currentBrightSignModel = hd1022;
                    break;
                case "XD1030":
                    currentBrightSignModel = xd1030;
                    break;
                case "XD1032":
                    currentBrightSignModel = xd1032;
                    break;
                case "XD1132":
                    currentBrightSignModel = xd1132;
                    break;
                case "XD1230":
                    currentBrightSignModel = xd1230;
                    break;
                case "4K242":
                    currentBrightSignModel = fk242;
                    break;
                case "4K1042":
                    currentBrightSignModel = fk1042;
                    break;
                case "4K1142":
                    currentBrightSignModel = fk1142;
                    break;
                case "2K1042":
                    currentBrightSignModel = tk1042;
                    break;
                case "HD920":
                    currentBrightSignModel = hd920;
                    break;
                case "HD922":
                    currentBrightSignModel = hd922;
                    break;
                case "HD970":
                    currentBrightSignModel = hd970;
                    break;
                case "HD972":
                    currentBrightSignModel = hd972;
                    break;
                case "A915":
                    currentBrightSignModel = a915;
                    break;
                case "HD917":
                    currentBrightSignModel = hd917;
                    break;
                case "AU320":
                    currentBrightSignModel = au320;
                    break;
                default:
                    currentBrightSignModel = hd120;
                    break;
            }
        }

        // allow Bose devices to use more than 2 decoders - not clear what the real limit is...
        public static int GetNumberOfSupportedAudioDecoders(string modelType)
        {
            int numAllowedAudioDecoders = 3;
            //switch (modelType)
            //{
            //    case "HD910":
            //    case "HD912":
            //    case "HD960":
            //    case "HD962":
            //    case "A913":
            //    case "A933":
            //        numAllowedAudioDecoders = 6; // random number > 2
            //        break;
            //    default:
            //        break;
            //}

            return numAllowedAudioDecoders;
        }

        public static int GetNumberOfSupportedVideoPlayers(string modelType)
        {
            int numAllowedVideoDecoders = 1;
            switch (modelType)
            {
                case "XD230":
                case "XD232":
                case "XD1030":
                case "XD1032":
                case "XD1132":
                case "XD1230":
                case "4K242":
                case "4K1042":
                case "4K1142":
                case "2K1042":
                    numAllowedVideoDecoders = 2;
                    break;
                default:
                    break;
            }

            return numAllowedVideoDecoders;
        }

        public static BrightSignModel GetBrightSignModel(string modelType)
        {
            switch (modelType)
            {
                case "HD120":
                    return hd120;
                case "HD220":
                    return hd220;
                case "HD222":
                    return hd222;
                case "XD230":
                    return xd230;
                case "XD232":
                    return xd232;
                case "HD1020":
                    return hd1020;
                case "HD1022":
                    return hd1022;
                case "LS322":
                    return ls322;
                case "LS422":
                    return ls422;
                case "XD1030":
                    return xd1030;
                case "XD1032":
                    return xd1032;
                case "XD1132":
                    return xd1132;
                case "XD1230":
                    return xd1230;
                case "4K242":
                    return fk242;
                case "4K1042":
                    return fk1042;
                case "4K1142":
                    return fk1142;
                case "2K1042":
                    return tk1042;
                case "HD920":
                    return hd920;
                case "HD922":
                    return hd922;
                case "HD970":
                    return hd970;
                case "HD972":
                    return hd972;
                case "A915":
                    return a915;
                case "AU320":
                    return au320;
                default:
                    return hd120;
            }
        }

        public static bool CurrentModelSupportsFeature(BrightSignModel.ModelFeature feature)
        {
            if (currentBrightSignModel == null) return false;
            return currentBrightSignModel.FeatureIsSupported(feature);
        }

        private static bool TenBitColorSupported(string resolution)
        {
            switch (resolution)
            {
                case "3840x2160x60p":
                case "3840x2160x59.94p":
                case "3840x2160x50p":
                    return true;
            }
            return false;
        }

        public static bool TenBitColorSupported(string modelType, string resolution)
        {
            BrightSignModel brightSignModel = GetBrightSignModel(modelType);
            if (!brightSignModel.FeatureIsSupported(BrightSignModel.ModelFeature.TenBitColor)) return false;
            return TenBitColorSupported(resolution);
        }
    }
}
