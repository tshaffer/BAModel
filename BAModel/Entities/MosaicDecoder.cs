using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BAModel
{
    public class VideoZoneResolutionComparer : IComparer<VideoZone>
    {
        public int Compare(VideoZone xVZ, VideoZone yVZ)
        {
            MosaicDecoder.MaxContentResolution x = xVZ.MaxResolution;
            MosaicDecoder.MaxContentResolution y = yVZ.MaxResolution;
            
            int xValue = MosaicDecoder.GetResolutionComparisonValue(x);
            int yValue = MosaicDecoder.GetResolutionComparisonValue(y);
            if (xValue < yValue)
            {
                return 1;
            }
            else if (yValue < xValue)
            {
                return -1;
            }
            return 0;
        }
    }

    public class MosaicDecoderMaxResolutionComparer : IComparer<MosaicDecoderStatus>
    {
        public int Compare(MosaicDecoderStatus xMDS, MosaicDecoderStatus yMDS)
        {
            MosaicDecoder xM = xMDS.MosaicDecoder;
            MosaicDecoder yM = yMDS.MosaicDecoder;

            int xValue = xM.GetResolutionComparisonValue();
            int yValue = yM.GetResolutionComparisonValue();
            if (xValue < yValue)
            {
                return 1;
            }
            else if (yValue < xValue)
            {
                return -1;
            }
            return 0;
        }
    }

    public class MosaicDecoder
    {
        public enum MaxContentResolution
        {
            _NotApplicable,
            _4K,
            _HD,
            _SD,
            _CIF,
            _QCIF
        }

        public static string GetTimesliceModeFromContentResolution(MaxContentResolution contentResolution)
        {
            switch (contentResolution)
            {
                case MosaicDecoder.MaxContentResolution._4K:
                    return "4K";
                case MosaicDecoder.MaxContentResolution._HD:
                    return "HD";
                case MosaicDecoder.MaxContentResolution._SD:
                    return "SD";
                case MosaicDecoder.MaxContentResolution._CIF:
                    return "CIF";
                case MosaicDecoder.MaxContentResolution._QCIF:
                    return "QCIF";
                case MosaicDecoder.MaxContentResolution._NotApplicable:
                    return "N/A";
                default:
                    return "HD";
            }
        }

        public static int GetResolutionComparisonValue(MosaicDecoder.MaxContentResolution maxRes)
        {
            switch (maxRes)
            {
                case MosaicDecoder.MaxContentResolution._4K:
                    return 5;
                case MosaicDecoder.MaxContentResolution._HD:
                    return 4;
                case MosaicDecoder.MaxContentResolution._SD:
                    return 3;
                case MosaicDecoder.MaxContentResolution._CIF:
                    return 2;
                case MosaicDecoder.MaxContentResolution._QCIF:
                    return 1;
                case MosaicDecoder.MaxContentResolution._NotApplicable:
                default:
                    return 0;
            }
        }

        private bool ContainsMaxResolution(MaxContentResolution maxRes)
        {
            if (MaxVideoPlayersByResolution.ContainsKey(maxRes))
            {
                if (MaxVideoPlayersByResolution[maxRes] > 0) return true;
            }
            return false;
        }

        public int GetResolutionComparisonValue()
        {
            if (ContainsMaxResolution(MaxContentResolution._4K)) return GetResolutionComparisonValue(MaxContentResolution._4K);
            if (ContainsMaxResolution(MaxContentResolution._HD)) return GetResolutionComparisonValue(MaxContentResolution._HD);
            if (ContainsMaxResolution(MaxContentResolution._SD)) return GetResolutionComparisonValue(MaxContentResolution._SD);
            if (ContainsMaxResolution(MaxContentResolution._CIF)) return GetResolutionComparisonValue(MaxContentResolution._CIF);
            if (ContainsMaxResolution(MaxContentResolution._QCIF)) return GetResolutionComparisonValue(MaxContentResolution._QCIF);
            return GetResolutionComparisonValue(MaxContentResolution._NotApplicable);
        }

        public MosaicDecoder.MaxContentResolution GetMaxResolution()
        {
            if (ContainsMaxResolution(MaxContentResolution._4K)) return MaxContentResolution._4K;
            if (ContainsMaxResolution(MaxContentResolution._HD)) return MaxContentResolution._HD;
            if (ContainsMaxResolution(MaxContentResolution._SD)) return MaxContentResolution._SD;
            if (ContainsMaxResolution(MaxContentResolution._CIF)) return MaxContentResolution._CIF;
            if (ContainsMaxResolution(MaxContentResolution._QCIF)) return MaxContentResolution._QCIF;
            return MaxContentResolution._4K;
        }

        public static MosaicDecoder.MaxContentResolution GetMaxModelResolution(BrightSignModel model)
        {
            MosaicDecoder.MaxContentResolution maxModelResolution = MosaicDecoder.MaxContentResolution._NotApplicable;
            List<MosaicDecoder> mosaicDecoders = MosaicDecoderMgr.GetMosaicDecoders(BrightSignModelMgr.CurrentBrightSignModel);
            foreach (MosaicDecoder mosaicDecoder in mosaicDecoders)
            {
                MosaicDecoder.MaxContentResolution maxDecoderResolution = mosaicDecoder.GetMaxResolution();
                if (MosaicDecoder.ResolutionGreaterThan(maxDecoderResolution, maxModelResolution))
                {
                    maxModelResolution = maxDecoderResolution;
                }
            }
            return maxModelResolution;
        }

        public string DecoderName { get; set; }

        private Dictionary<MosaicDecoder.MaxContentResolution, int> _maxVideoPlayersByResolution = new Dictionary<MosaicDecoder.MaxContentResolution, int>();
        public Dictionary<MosaicDecoder.MaxContentResolution, int> MaxVideoPlayersByResolution
        {
            get { return _maxVideoPlayersByResolution; }
            set { _maxVideoPlayersByResolution = value; }
        }

        public static bool ResolutionsEqual(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            int xValue = GetResolutionComparisonValue(x);
            int yValue = GetResolutionComparisonValue(y);
            return xValue == yValue;
        }

        public static bool ResolutionGreaterThan(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            int xValue = GetResolutionComparisonValue(x);
            int yValue = GetResolutionComparisonValue(y);
            return xValue > yValue;
        }

        public static bool ResolutionGreatThanOrEqual(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            int xValue = GetResolutionComparisonValue(x);
            int yValue = GetResolutionComparisonValue(y);
            return xValue >= yValue;
        }

    }

    public class MosaicDecoderStatus
    {
        public void MarkUnusable()
        {
            InUse = true;
            SelectedResolution = MosaicDecoder.MaxContentResolution._NotApplicable;
            NumAvailablePlayers = -1;
        }

        public bool MarkedAsUnusable()
        {
            return (InUse == true && SelectedResolution == MosaicDecoder.MaxContentResolution._NotApplicable && NumAvailablePlayers == -1);
        }

        public string GetTimesliceMode()
        {
            string timeSliceMode = "QCIF";

            timeSliceMode = MosaicDecoder.GetTimesliceModeFromContentResolution(SelectedResolution);
            if (timeSliceMode == "N/A") timeSliceMode = "QCIF";

            return timeSliceMode;
        }

        public static bool ResolutionsEqual(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            return MosaicDecoder.ResolutionsEqual(x, y);
        }

        public static bool ResolutionGreaterThan(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            return MosaicDecoder.ResolutionGreaterThan(x, y);
        }

        public static bool ResolutionGreatThanOrEqual(MosaicDecoder.MaxContentResolution x, MosaicDecoder.MaxContentResolution y)
        {
            return MosaicDecoder.ResolutionGreatThanOrEqual(x, y);
        }

        public string DecoderName { get; set; }

        public MosaicDecoder MosaicDecoder { get; set; }

        public bool InUse { get; set; }
        public MosaicDecoder.MaxContentResolution SelectedResolution { get; set; }
        public int NumAvailablePlayers { get; set; }

        private List<VideoZone> _videoZones = new List<VideoZone>();
        public List<VideoZone> VideoZones
        {
            get { return _videoZones; }
            set { _videoZones = value; }
        }
    }

    public class PublishedMosaicDecoder
    {
        public string DecoderName { get; set; }     // as returned from libMedia
        public string TimeSliceMode { get; set; }   // 4K, HD, SD, CIF, QCIF
        public int ZOrder { get; set; }
        public string FriendlyName { get; set; }
        public bool EnableMosaicDeinterlacer { get; set; }

        public void WriteXML(XmlTextWriter writer)
        {
            writer.WriteStartElement("mosaicDecoder");
            writer.WriteElementString("decoderName", DecoderName);
            writer.WriteElementString("timeSliceMode", TimeSliceMode);
            writer.WriteElementString("zOrder", ZOrder.ToString());
            writer.WriteElementString("friendlyName", FriendlyName);
            writer.WriteElementString("enableMosaicDeinterlacer", EnableMosaicDeinterlacer.ToString());
            writer.WriteFullEndElement();
        }
    }

}
