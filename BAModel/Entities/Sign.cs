using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;


namespace BAModel
{
	public class Sign
	{
		// was in BrightAuthor.xaml.cs
		public enum MonitorOrientation
		{
			Landscape,
			Portrait,
			PortraitBottomOnRight
		}


		public string Name { get; set; }
		public bool IsMosaic { get; set; }
		private string _model;
		private bool _isBackup = false;
		private string _videoMode;
		private MonitorOrientation _monitorOrientation;
		private string _videoConnector;
		private string _backgroundScreenColor;
		private string _rssDownloadSpec;
		private string _language;
		private string[] _gpioConfiguration;

		public bool BP900AConfigureAutomatically { get; set; }
		public bool BP900BConfigureAutomatically { get; set; }
		public bool BP900CConfigureAutomatically { get; set; }
		public bool BP200AConfigureAutomatically { get; set; }
		public bool BP200BConfigureAutomatically { get; set; }
		public bool BP200CConfigureAutomatically { get; set; }
		public int BP900AConfiguration { get; set; }
		public int BP900BConfiguration { get; set; }
		public int BP900CConfiguration { get; set; }
		public int BP200AConfiguration { get; set; }
		public int BP200BConfiguration { get; set; }
		public int BP200CConfiguration { get; set; }

		private SerialPortConfiguration[] serialPortConfigurations = new SerialPortConfiguration[UserPreferences.NUM_SERIAL_PORTS];
		public SerialPortConfiguration[] SerialPortConfigurations
		{
			get
			{
				return serialPortConfigurations;
			}
		}

		public string UDPDestinationAddressType { get; set; }
		public string UDPDestinationAddress { get; set; }
		public string UDPDestinationPort { get; set; }
		public string UDPReceiverPort { get; set; }
		public string FlipCoordinates { get; set; }
		public string TouchCursorDisplayMode { get; set; }

		public bool ForceResolution { get; set; }
		public bool TenBitColorEnabled { get; set; }
		public string MonitorOverscan { get; set; }

		public string CustomAutorun { get; set; }

		public enum DeviceWebPageDisplay
		{
			None,
			Standard,
			Custom
		}
		public DeviceWebPageDisplay SelectedDeviceWebPageDisplay { get; set; }
		public string CustomDeviceWebPageSiteName { get; set; }
		public string CustomDeviceWebPage { get; set; }
		public bool AlphabetizeVariableNames { get; set; }

		public bool DelayScheduleChangeUntilMediaEndEvent { get; set; }

		public bool HTMLEnableJavascriptConsole { get; set; }

		private int _videoWidth;
		private int _videoHeight;
		private ObservableCollection<Zone> _zoneList = new ObservableCollection<Zone>();

		public string Audio1MinVolume { get; set; }
		public string Audio1MaxVolume { get; set; }
		public string Audio2MinVolume { get; set; }
		public string Audio2MaxVolume { get; set; }
		public string Audio3MinVolume { get; set; }
		public string Audio3MaxVolume { get; set; }
		public string USBAMinVolume { get; set; }
		public string USBAMaxVolume { get; set; }
		public string USBBMinVolume { get; set; }
		public string USBBMaxVolume { get; set; }
		public string USBCMinVolume { get; set; }
		public string USBCMaxVolume { get; set; }
		public string USBDMinVolume { get; set; }
		public string USBDMaxVolume { get; set; }
		public string HDMIMinVolume { get; set; }
		public string HDMIMaxVolume { get; set; }
		public string SPDIFMinVolume { get; set; }
		public string SPDIFMaxVolume { get; set; }

		public string TripleUSBPort { get; set; }

		public string AudioInSampleRate { get; set; }

		private List<BoseProductInPresentation> _boseProducts = new List<BoseProductInPresentation>();
		public List<BoseProductInPresentation> BoseProducts
		{
			get { return _boseProducts; }
			set { _boseProducts = value; }
		}

		private int _imageBufferSpaceUsed;
		public static int ImageBufferSpaceUsed
		{
			get { return CurrentSign._imageBufferSpaceUsed; }
			set { CurrentSign._imageBufferSpaceUsed = value; }
		}

		private Dictionary<string, ImageBufferItem> _imageBufferPool = new Dictionary<string, ImageBufferItem>();
		public static Dictionary<string, ImageBufferItem> ImageBufferPool
		{
			get { return CurrentSign._imageBufferPool; }
		}

		public bool InactivityTimeout { get; set; }
		public string InactivityTime { get; set; }

		private UserVariableSet _userVariableSet = new UserVariableSet();
		public UserVariableSet UserVariableSet
		{
			get { return _userVariableSet; }
			set { _userVariableSet = value; }
		}

		private LiveDataFeedSet _liveDataFeedSet = new LiveDataFeedSet();
		public LiveDataFeedSet LiveDataFeedSet
		{
			get { return _liveDataFeedSet; }
			set { _liveDataFeedSet = value; }
		}

		private Dictionary<int, MediaFeedCustomFields> _mediaFeedsCustomFields = new Dictionary<int, MediaFeedCustomFields>();
		public Dictionary<int, MediaFeedCustomFields> MediaFeedsCustomFields
		{
			get { return _mediaFeedsCustomFields; }
			set { _mediaFeedsCustomFields = value; }
		}

		private ScriptPluginSet _scriptPluginSet = new ScriptPluginSet();
		public ScriptPluginSet ScriptPluginSet
		{
			get { return _scriptPluginSet; }
			set { _scriptPluginSet = value; }
		}

		private HTMLSiteSet _htmlSiteSet = new HTMLSiteSet();
		public HTMLSiteSet HTMLSiteSet
		{
			get { return _htmlSiteSet; }
			set { _htmlSiteSet = value; }
		}

		public bool AutoCreateMediaCounterVariables { get; set; }
		public bool ResetVariablesOnPresentationStart { get; set; }

		public string NetworkedVariablesUpdateInterval { get; set; }

		private PresentationIdentifierSet _presentationIdentifierSet = new PresentationIdentifierSet();
		public PresentationIdentifierSet PresentationIdentifierSet
		{
			get { return _presentationIdentifierSet; }
			set { _presentationIdentifierSet = value; }
		}

		private ObservableCollection<string> _additionalFilesToPublish = new ObservableCollection<string>();
		public ObservableCollection<string> AdditionalFilesToPublish
		{
			get { return _additionalFilesToPublish; }
			set { _additionalFilesToPublish = value; }
		}

		public bool IsStretchedVideoWall { get; set; }
		public int VideoWallNumRows { get; set; }
		public int VideoWallNumColumns { get; set; }
		public int VideoWallRowPosition { get; set; }
		public int VideoWallColumnPosition { get; set; }
		public int BezelWidthPercent { get; set; }
		public int BezelHeightPercent { get; set; }

		public bool EnableEnhancedSynchronization { get; set; }
		public bool DeviceIsSyncMaster { get; set; }
		public string PTPDomain { get; set; }

		private string _rfChannelDataFilePath = String.Empty;
		public string RFChannelDataFilePath
		{
			get { return _rfChannelDataFilePath; }
			set { _rfChannelDataFilePath = value; }
		}

		public enum GraphicsZOrderPosition
		{
			Front,
			Middle,
			Back
		}

		private GraphicsZOrderPosition _graphicsZOrder = GraphicsZOrderPosition.Front;
		public GraphicsZOrderPosition GraphicsZOrder
		{
			get { return _graphicsZOrder; }
			set { _graphicsZOrder = value; }
		}

		private int _bpfVersionNumber = BrightAuthorUtils.CurrentFileVersion;

		private ObservableCollection<UserDefinedEvent> _userDefinedEvents = new ObservableCollection<UserDefinedEvent>();
		//private ObservableCollection<UserDefinedEvent> _userDefinedEvents = null;
		public ObservableCollection<UserDefinedEvent> UserDefinedEvents
		{
			get { return _userDefinedEvents; }
			set { _userDefinedEvents = value; }
		}

		private bool _userDefinedEventsFound = false;
		public bool UserDefinedEventsFound
		{
			get { return _userDefinedEventsFound; }
			set { _userDefinedEventsFound = value; }
		}

		public string Model
		{
			get { return _model; }
			set { _model = value; }
		}

		public string VideoMode
		{
			get { return _videoMode; }
			set
			{ 
				_videoMode = value;
				ZoneTemplateData.GetVideoDimensions(_videoMode, out _videoWidth, out _videoHeight);
			}
		}

//		public MonitorOrientation MonitorOrientation
//		{
//			get { return _monitorOrientation; }
//			set { _monitorOrientation = value; }
//		}

		public string VideoConnector
		{
			get { return _videoConnector; }
			set { _videoConnector = value; }
		}

		public int VideoWidth
		{
			get { return _videoWidth; }
		}

		public int VideoHeight
		{
			get { return _videoHeight; }
		}

		public bool IsBackup
		{
			get { return _isBackup; }
			set { _isBackup = value; }
		}

		public string BackgroundScreenColor
		{
			get { return _backgroundScreenColor; }
			set { _backgroundScreenColor = value; }
		}

		public string RSSDownloadSpec
		{
			get { return _rssDownloadSpec; }
			set { _rssDownloadSpec = value; }
		}

		public string Language
		{
			get { return _language; }
			set { _language = value; }
		}

		public string[] GPIOConfiguration
		{
			get { return _gpioConfiguration; }
			set { _gpioConfiguration = value; }
		}

		public int NumberOfGPIOInputs
		{
			get
			{
				int gpioInputCount = 0;
				foreach (string gpioConfig in GPIOConfiguration)
				{
					if (gpioConfig == "input") gpioInputCount++;
				}
				return gpioInputCount;
			}
		}

		public int FirstGPIOInput
		{
			get
			{
				int gpioInput = 0;
				foreach (string gpioConfig in GPIOConfiguration)
				{
					if (gpioConfig == "input") return gpioInput;
					gpioInput++;
				}
				return -1;
			}
		}

		public ObservableCollection<Zone> ZoneList
		{
			get { return _zoneList; }
		}


		public Sign(string name, string model, string videoMode, MonitorOrientation monitorOrientation, string videoConnector, string customAutorun)
		{
			Name = name;

			IsMosaic = false;

			_model = model;
			_videoMode = videoMode;
			_monitorOrientation = monitorOrientation;
			_videoConnector = videoConnector;
			CustomAutorun = customAutorun;

//			SelectedDeviceWebPageDisplay = UserPreferences.DeviceWebPageDisplay;
			CustomDeviceWebPageSiteName = String.Empty;
//			CustomDeviceWebPage = UserPreferences.CustomDeviceWebPage;
//			AlphabetizeVariableNames = UserPreferences.AlphabetizeVariableNames;
//			DelayScheduleChangeUntilMediaEndEvent = UserPreferences.DelayScheduleChangeUntilMediaEndEvent;
			HTMLEnableJavascriptConsole = false;

//			_backgroundScreenColor = UserPreferences.BackgroundScreenColor;

			_rssDownloadSpec = "type|periodic|value|86400";

//			ForceResolution = UserPreferences.ForceResolution;

//			if (BrightSignModelMgr.TenBitColorSupported(_model, _videoMode))
//			{
//				TenBitColorEnabled = UserPreferences.TenBitColorEnabled;
//			}
//			else
//			{
//				TenBitColorEnabled = false;
//			}

//			MonitorOverscan = UserPreferences.MonitorOverscan;

//			_language = UserPreferences.Language;
			_gpioConfiguration = new string[8];
//			string[] userPreferredGPIOConfiguration = UserPreferences.GPIOConfiguration;
//			for (int i = 0; i < _gpioConfiguration.Length; i++)
//			{
//				_gpioConfiguration[i] = userPreferredGPIOConfiguration[i];
//			}

//			BP900AConfigureAutomatically = UserPreferences.BP900AConfigureAutomatically;
//			BP900BConfigureAutomatically = UserPreferences.BP900BConfigureAutomatically;
//			BP900CConfigureAutomatically = UserPreferences.BP900CConfigureAutomatically;
//			BP200AConfigureAutomatically = UserPreferences.BP200AConfigureAutomatically;
//			BP200BConfigureAutomatically = UserPreferences.BP200BConfigureAutomatically;
//			BP200CConfigureAutomatically = UserPreferences.BP200CConfigureAutomatically;
//			BP900AConfiguration = UserPreferences.BP900AConfiguration;
//			BP900BConfiguration = UserPreferences.BP900BConfiguration;
//			BP900CConfiguration = UserPreferences.BP900CConfiguration;
//			BP200AConfiguration = UserPreferences.BP200AConfiguration;
//			BP200BConfiguration = UserPreferences.BP200BConfiguration;
//			BP200CConfiguration = UserPreferences.BP200CConfiguration;
//
//			SerialPortConfiguration[] userPreferencesSerialPortConfigurations = UserPreferences.SerialPortConfigurations;
//			for (int i = 0; i < UserPreferences.NUM_SERIAL_PORTS; i++)
//			{
//				serialPortConfigurations[i] = userPreferencesSerialPortConfigurations[i].Copy();
//			}
//
//			UDPDestinationAddressType = UserPreferences.UDPDestinationAddressType;
//			UDPDestinationAddress = UserPreferences.UDPDestinationAddress;
//			UDPDestinationPort = UserPreferences.UDPDestinationPort;
//			UDPReceiverPort = UserPreferences.UDPReceiverPort;
//			FlipCoordinates = UserPreferences.FlipCoordinates;
//			TouchCursorDisplayMode = UserPreferences.TouchCursorDisplayMode;

//			ZoneTemplateData.GetVideoDimensions(_videoMode, out _videoWidth, out _videoHeight);

			Audio1MinVolume = "0";
			Audio1MaxVolume = "100";
			Audio2MinVolume = "0";
			Audio2MaxVolume = "100";
			Audio3MinVolume = "0";
			Audio3MaxVolume = "100";
			USBAMinVolume = "0";
			USBAMaxVolume = "100";
			USBBMinVolume = "0";
			USBBMaxVolume = "100";
			USBCMinVolume = "0";
			USBCMaxVolume = "100";
			USBDMinVolume = "0";
			USBDMaxVolume = "100";
			HDMIMinVolume = "0";
			HDMIMaxVolume = "100";
			SPDIFMinVolume = "0";
			SPDIFMaxVolume = "100";

//			InactivityTimeout = UserPreferences.InactivityTimeout;
//			InactivityTime = UserPreferences.InactivityTime;
//
//			TripleUSBPort = String.Empty;
//			AudioInSampleRate = UserPreferences.AudioInSampleRate;
//
//			AutoCreateMediaCounterVariables = UserPreferences.AutoCreateMediaCounterVariables;
//			ResetVariablesOnPresentationStart = UserPreferences.ResetVariablesOnPresentationStart;
//			NetworkedVariablesUpdateInterval = UserPreferences.NetworkedVariablesUpdateInterval;

			_imageBufferSpaceUsed = 0;
			_imageBufferPool.Clear();

			IsStretchedVideoWall = false;

			EnableEnhancedSynchronization = false;
			DeviceIsSyncMaster = false;
			PTPDomain = "0";

//			GraphicsZOrder = UserPreferences.GraphicsZOrder;
//			int numberOfVideoPlayers = BrightSignModelMgr.GetNumberOfSupportedVideoPlayers(model);
//			if (numberOfVideoPlayers == 1 && GraphicsZOrder == GraphicsZOrderPosition.Middle)
//			{
//				GraphicsZOrder = GraphicsZOrderPosition.Front;
//			}

		}

		public Sign Copy()
		{
			Sign sign = new Sign(this.Name, this.Model, this.VideoMode, this.MonitorOrientation, this.VideoConnector, this.CustomAutorun);
			sign._videoWidth = this._videoWidth;
			sign._videoHeight = this._videoHeight;
			sign.BackgroundScreenColor = this.BackgroundScreenColor;

			sign.SelectedDeviceWebPageDisplay = this.SelectedDeviceWebPageDisplay;
			sign.CustomDeviceWebPageSiteName = this.CustomDeviceWebPageSiteName;
			sign.CustomDeviceWebPage = this.CustomDeviceWebPage;
			sign.AlphabetizeVariableNames = this.AlphabetizeVariableNames;
			sign.DelayScheduleChangeUntilMediaEndEvent = DelayScheduleChangeUntilMediaEndEvent;
			sign.HTMLEnableJavascriptConsole = this.HTMLEnableJavascriptConsole;

			sign.RSSDownloadSpec = this.RSSDownloadSpec;

			sign.ForceResolution = this.ForceResolution;
			sign.TenBitColorEnabled = this.TenBitColorEnabled;
			sign.MonitorOverscan = this.MonitorOverscan;

			sign.Language = this.Language;

			for (int i = 0; i < this._gpioConfiguration.Length; i++)
			{
				sign._gpioConfiguration[i] = this._gpioConfiguration[i];
			}

			sign.BP900AConfigureAutomatically = this.BP900AConfigureAutomatically;
			sign.BP900BConfigureAutomatically = this.BP900BConfigureAutomatically;
			sign.BP900CConfigureAutomatically = this.BP900CConfigureAutomatically;
			sign.BP200AConfigureAutomatically = this.BP200AConfigureAutomatically;
			sign.BP200BConfigureAutomatically = this.BP200BConfigureAutomatically;
			sign.BP200CConfigureAutomatically = this.BP200CConfigureAutomatically;
			sign.BP900AConfiguration = this.BP900AConfiguration;
			sign.BP900BConfiguration = this.BP900BConfiguration;
			sign.BP900CConfiguration = this.BP900CConfiguration;
			sign.BP200AConfiguration = this.BP200AConfiguration;
			sign.BP200BConfiguration = this.BP200BConfiguration;
			sign.BP200CConfiguration = this.BP200CConfiguration;

			foreach (Zone zone in ZoneList)
			{
				sign.ZoneList.Add((Zone)zone.Clone());
			}

			for (int i = 0; i < UserPreferences.NUM_SERIAL_PORTS; i++)
			{
				sign.serialPortConfigurations[i] = this.SerialPortConfigurations[i].Copy();
			}

			sign.UDPDestinationAddressType = this.UDPDestinationAddressType;
			sign.UDPDestinationAddress = this.UDPDestinationAddress;
			sign.UDPDestinationPort = this.UDPDestinationPort;
			sign.UDPReceiverPort = this.UDPReceiverPort;
			sign.FlipCoordinates = this.FlipCoordinates;
			sign.TouchCursorDisplayMode = this.TouchCursorDisplayMode;

			sign.Audio1MinVolume = this.Audio1MinVolume;
			sign.Audio1MaxVolume = this.Audio1MaxVolume;
			sign.Audio2MinVolume = this.Audio2MinVolume;
			sign.Audio2MaxVolume = this.Audio2MaxVolume;
			sign.Audio3MinVolume = this.Audio3MinVolume;
			sign.Audio3MaxVolume = this.Audio3MaxVolume;
			sign.USBAMinVolume = this.USBAMinVolume;
			sign.USBAMaxVolume = this.USBAMaxVolume;
			sign.USBBMinVolume = this.USBBMinVolume;
			sign.USBBMaxVolume = this.USBBMaxVolume;
			sign.USBCMinVolume = this.USBCMinVolume;
			sign.USBCMaxVolume = this.USBCMaxVolume;
			sign.USBDMinVolume = this.USBDMinVolume;
			sign.USBDMaxVolume = this.USBDMaxVolume;
			sign.HDMIMinVolume = this.HDMIMinVolume;
			sign.HDMIMaxVolume = this.HDMIMaxVolume;
			sign.SPDIFMinVolume = this.SPDIFMinVolume;
			sign.SPDIFMaxVolume = this.SPDIFMaxVolume;

			sign.InactivityTimeout = this.InactivityTimeout;
			sign.InactivityTime = this.InactivityTime;

			sign.AutoCreateMediaCounterVariables = this.AutoCreateMediaCounterVariables;
			sign.ResetVariablesOnPresentationStart = this.ResetVariablesOnPresentationStart;

			sign.TripleUSBPort = this.TripleUSBPort;
			sign.AudioInSampleRate = this.AudioInSampleRate;

			foreach (BoseProductInPresentation bpip in this.BoseProducts)
			{
				sign.BoseProducts.Add(bpip);
			}

			foreach (KeyValuePair<string, ImageBufferItem> kvp in this._imageBufferPool)
			{
				string filePath = kvp.Key;
				ImageBufferItem ibi = kvp.Value;

				if (!sign._imageBufferPool.ContainsKey(filePath))
				{
					ImageBufferItem signIbi = new ImageBufferItem { FileName = ibi.FileName, FileSize = ibi.FileSize };
					sign._imageBufferPool.Add(filePath, signIbi);
				}
			}
			sign._imageBufferSpaceUsed = this._imageBufferSpaceUsed;

			foreach (UserDefinedEvent userDefinedEvent in this.UserDefinedEvents)
			{
				UserDefinedEvent newUserDefinedEvent = (UserDefinedEvent)userDefinedEvent.Clone();
				sign.UserDefinedEvents.Add(newUserDefinedEvent);
			}

			sign.UserVariableSet = this.UserVariableSet.Clone();
			sign.LiveDataFeedSet = this.LiveDataFeedSet.Clone();
			sign.HTMLSiteSet = this.HTMLSiteSet.Clone();
			sign.PresentationIdentifierSet = this.PresentationIdentifierSet.Clone();
			sign.ScriptPluginSet = this.ScriptPluginSet.Clone();

			sign.AdditionalFilesToPublish = new ObservableCollection<string>();
			foreach (string additionalFileToPublish in this.AdditionalFilesToPublish)
			{
				sign.AdditionalFilesToPublish.Add(additionalFileToPublish);
			}

			sign.EnableEnhancedSynchronization = this.EnableEnhancedSynchronization;
			sign.DeviceIsSyncMaster = this.DeviceIsSyncMaster;
			sign.PTPDomain = this.PTPDomain;

			sign.RFChannelDataFilePath = this.RFChannelDataFilePath;

			sign.GraphicsZOrder = this.GraphicsZOrder;

			sign.IsMosaic = this.IsMosaic;

			return sign;
		}


	}

	public class ImageBufferItem
	{
		public string FileName { get; set; }
		public int FileSize { get; set; }

		public bool IsEqual(Object obj)
		{
			//Check for null and compare run-time types.
			if (obj == null || GetType() != obj.GetType()) return false;

			ImageBufferItem ibi = (ImageBufferItem)obj;

			return ibi.FileName == this.FileName && ibi.FileSize == this.FileSize;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class DuplicateFileList
	{
		private List<string> _filePaths = new List<string>();

		public string FileName { get; set; }
		public List<string> FilePaths
		{
			get
			{
				return _filePaths;
			}
		}
	}

	public class UserVariableInUse
	{
		public UserVariable UserVariable { get; set; }
		public List<string> StateNames { get; set; }
	}

	public class PresentationInUse
	{
		public PresentationIdentifier Presentation { get; set; }
		public List<string> StateNames { get; set; }
	}


}

