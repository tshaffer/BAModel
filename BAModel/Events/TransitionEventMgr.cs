using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAModel
{
    class TransitionEventMgr
    {
        private static Dictionary<string, SimpleUserEvent> _userEventDictionary = new Dictionary<string, SimpleUserEvent>();

        public static Dictionary<string, SimpleUserEvent> UserEventDictionary
        {
            get { return _userEventDictionary; }
        }

        public static void InitializeTransitionEventMgr()
        {
            SimpleUserEvent timeoutUserEvent = new SimpleUserEvent {
                ImageResourceName = "iconTimeout",
                ImageResourceNameLarge = "iconTimeoutLarge",
                ImageResourceSelectedName = "iconTimeoutSelected",
                DialogTitle = "Timeout Event",
                Value = "Timeout", 
                UserEventName = "timeout", 
                Parameter = "3", 
                ParameterRequired = true, 
                Prompt = "Specify timeout (seconds)",
                ValidationRules = EventDlg.ValidateNumberGreaterThanZero,
                ValueI18NResource = "TimeoutEvent",
                DialogTitleI18NResource = "TimeoutEventDlg",
                PromptI18NResource = "TimeoutEventPrompt"
            };
            
            SimpleUserEvent mediaEndUserEvent = new SimpleUserEvent { 
                ImageResourceName = "iconVideoEnd",
                ImageResourceNameLarge = "iconVideoEndLarge",
                ImageResourceSelectedName = "iconVideoEndSelected",
                DialogTitle = "Media End Event", 
                Value = "Media End", 
                UserEventName = "mediaEnd", 
                Parameter = "", 
                ParameterRequired = false, 
                Prompt = "",
                ValidationRules = 0,
                ValueI18NResource = "MediaEndEvent",
                DialogTitleI18NResource = "MediaEndEventDlg",
                PromptI18NResource = ""
            };

            SimpleUserEvent quietUserEvent = null;
            SimpleUserEvent loudUserEvent = null;
            if ((BrightSignModelMgr.Model960Enabled) || (BrightSignModelMgr.Model913Enabled) || (BrightSignModelMgr.Model933Enabled) || (BrightSignModelMgr.Model910Enabled))
            {
                quietUserEvent = new SimpleUserEvent
                {
                    ImageResourceName = "iconQuiet",
                    ImageResourceNameLarge = "iconQuietLarge",
                    ImageResourceSelectedName = "iconQuietSelected",
                    DialogTitle = "Quiet Event",
                    Value = "Quiet Event",
                    UserEventName = "quietUserEvent",
                    Parameter = "",
                    ParameterRequired = false,
                    Prompt = "",
                    ValidationRules = 0,
                    ValueI18NResource = "QuietEvent",
                    DialogTitleI18NResource = "QuietEventDlg",
                    PromptI18NResource = ""
                };
                loudUserEvent = new SimpleUserEvent
                {
                    ImageResourceName = "iconLoud",
                    ImageResourceNameLarge = "iconLoudLarge",
                    ImageResourceSelectedName = "iconLoudSelected",
                    DialogTitle = "Loud Event",
                    Value = "Loud Event",
                    UserEventName = "loudUserEvent",
                    Parameter = "",
                    ParameterRequired = false,
                    Prompt = "",
                    ValidationRules = 0,
                    ValueI18NResource = "LoudEvent",
                    DialogTitleI18NResource = "LoudEventDlg",
                    PromptI18NResource = ""
                };
            }

            TwoParameterUserEvent serialUserEvent = new TwoParameterUserEvent
            { 
                ImageResourceName = "iconSerial",
                ImageResourceNameLarge = "iconSerialLarge",
                ImageResourceSelectedName = "iconSerialSelected",
                DialogTitle = "Serial Input Event", 
                Value = "Serial Input", 
                UserEventName = "serial", 
                Parameter = "0", 
                ParameterRequired = true, 
                Prompt = "Specify port",
                ValidationRules = EventDlg.ValidateSerialPort,
                Parameter2 = "", 
                Parameter2Required = true, 
                Prompt2 = "Specify serial input",
                ValidationRules2 = 0,
                ValueI18NResource = "SerialEvent",
                DialogTitleI18NResource = "SerialEventDlg",
                PromptI18NResource = "SpecifyPort",
                Prompt2I18NResource = "SerialEventPrompt"
            };

            SimpleUserEvent keyboardUserEvent = new SimpleUserEvent {
                ImageResourceName = "iconKeyboard",
                ImageResourceNameLarge = "iconKeyboardLarge",
                ImageResourceSelectedName = "iconKeyboardSelected",
                DialogTitle = "Keyboard Input Event", 
                Value = "Keyboard Input", 
                UserEventName = "keyboard", 
                Parameter = "", 
                ParameterRequired = true, 
                Prompt = "Specify keyboard input",
                ValidationRules = EventDlg.ValidateKeyboardCommand,
                ValueI18NResource = "KeyboardEvent",
                DialogTitleI18NResource = "KeyboardEventDlg",
                PromptI18NResource = "KeyboardEventPrompt"
            };

            SimpleUserEvent remoteUserEvent = new SimpleUserEvent { 
                ImageResourceName = "iconRemote",
                ImageResourceNameLarge = "iconRemoteLarge",
                ImageResourceSelectedName = "iconRemoteSelected",
                DialogTitle = "Remote Input Event", 
                Value = "Remote Input", 
                UserEventName = "remote", 
                Parameter = "", 
                ParameterRequired = true, 
                Prompt = "Specify remote input",
                ValidationRules = EventDlg.ValidateRemoteCommand,
                ValueI18NResource = "RemoteEvent",
                DialogTitleI18NResource = "RemoteEventDlg",
                PromptI18NResource = "RemoteEventPrompt"
            };

            SimpleUserEvent usbStringUserEvent = new SimpleUserEvent { 
                ImageResourceName = "iconUsb",
                ImageResourceNameLarge = "iconUsbLarge",
                ImageResourceSelectedName = "iconUsbSelected",
                DialogTitle = "USB Input Event", 
                Value = "USB Input", 
                UserEventName = "usb", 
                Parameter = "", 
                ParameterRequired = true, 
                Prompt = "Specify USB input",
                ValidationRules = 0,
                ValueI18NResource = "USBEvent",
                DialogTitleI18NResource = "USBEventDlg",
                PromptI18NResource = "USBEventPrompt"
            };

            SimpleUserEvent synchronizeUserEvent = new SimpleUserEvent { 
                ImageResourceName = "iconSynchronize",
                ImageResourceNameLarge = "iconSynchronizeLarge",
                ImageResourceSelectedName = "iconSynchronizeSelected",
                DialogTitle = "Synchronize Event", 
                Value = "Synchronize", 
                UserEventName = "synchronize", 
                Parameter = "", 
                ParameterRequired = true, 
                Prompt = "Specify synchronization keyword",
                ValidationRules = 0,
                ValueI18NResource = "SynchronizeEvent",
                DialogTitleI18NResource = "SynchronizeEventDlg",
                PromptI18NResource = "SynchronizeEventPrompt"
            };

            SimpleUserEvent zoneMessageUserEvent = new SimpleUserEvent
            {
                ImageResourceName = "iconZoneMessage",
                ImageResourceNameLarge = "iconZoneMessageLarge",
                ImageResourceSelectedName = "iconZoneMessageSelected",
                DialogTitle = "Zone Message Event",
                Value = "Zone Message",
                UserEventName = "zoneMessage",
                Parameter = "",
                ParameterRequired = true,
                Prompt = "Specify zone message",
                ValidationRules = 0,
                ValueI18NResource = "ZoneMessageEvent",
                DialogTitleI18NResource = "ZoneMessageEventDlg",
                PromptI18NResource = "ZoneMessageEventPrompt"
            };

            SimpleUserEvent internalSynchronizeUserEvent = new SimpleUserEvent
            {
                ImageResourceName = "iconInternalSynchronize",
                ImageResourceNameLarge = "iconInternalSynchronizeLarge",
                ImageResourceSelectedName = "iconInternalSynchronizeSelected",
                DialogTitle = "Link Zones Event",
                Value = "Link Zones",
                UserEventName = "internalSynchronize",
                Parameter = "",
                ParameterRequired = true,
                Prompt = "Specify link keyword",
                ValidationRules = 0,
                ValueI18NResource = "LinkZonesEvent",
                DialogTitleI18NResource = "LinkZonesEventDlg",
                PromptI18NResource = "LinkZonesEventPrompt"
            };

            SimpleUserEvent successUserEvent = null;
            SimpleUserEvent failUserEvent = null;
            if ((BrightSignModelMgr.Model960Enabled) || (BrightSignModelMgr.Model913Enabled) || (BrightSignModelMgr.Model933Enabled) || (BrightSignModelMgr.Model910Enabled))
            {
                successUserEvent = new SimpleUserEvent
                {
                    ImageResourceName = "iconSuccess",
                    ImageResourceNameLarge = "iconSuccessLarge",
                    ImageResourceSelectedName = "iconSuccessSelected",
                    DialogTitle = "Success Event",
                    Value = "Success",
                    UserEventName = "success",
                    Parameter = "",
                    ParameterRequired = false,
                    Prompt = "",
                    ValidationRules = 0,
                    ValueI18NResource = "SuccessEvent",
                    DialogTitleI18NResource = "SuccessEventDlg",
                    PromptI18NResource = ""
                };

                failUserEvent = new SimpleUserEvent
                {
                    ImageResourceName = "iconFail",
                    ImageResourceNameLarge = "iconFailLarge",
                    ImageResourceSelectedName = "iconFailSelected",
                    DialogTitle = "Fail Event",
                    Value = "Fail",
                    UserEventName = "fail",
                    Parameter = "",
                    ParameterRequired = false,
                    Prompt = "",
                    ValidationRules = 0,
                    ValueI18NResource = "FailEvent",
                    DialogTitleI18NResource = "FailEventDlg",
                    PromptI18NResource = ""
                };
            }

            _userEventDictionary.Add(timeoutUserEvent.UserEventName, timeoutUserEvent);
            _userEventDictionary.Add(mediaEndUserEvent.UserEventName, mediaEndUserEvent);
            if ((quietUserEvent != null) && (loudUserEvent != null))
            {
                _userEventDictionary.Add(quietUserEvent.UserEventName, quietUserEvent);
                _userEventDictionary.Add(loudUserEvent.UserEventName, loudUserEvent);
            }
            _userEventDictionary.Add(serialUserEvent.UserEventName, serialUserEvent);
            _userEventDictionary.Add(keyboardUserEvent.UserEventName, keyboardUserEvent);
            _userEventDictionary.Add(remoteUserEvent.UserEventName, remoteUserEvent);
            _userEventDictionary.Add(synchronizeUserEvent.UserEventName, synchronizeUserEvent);
            _userEventDictionary.Add(zoneMessageUserEvent.UserEventName, zoneMessageUserEvent);
            _userEventDictionary.Add(internalSynchronizeUserEvent.UserEventName, internalSynchronizeUserEvent);
            _userEventDictionary.Add(usbStringUserEvent.UserEventName, usbStringUserEvent);
            if ((successUserEvent != null) && (failUserEvent != null))
            {
                _userEventDictionary.Add(successUserEvent.UserEventName, successUserEvent);
                _userEventDictionary.Add(failUserEvent.UserEventName, failUserEvent);
            }
        }

        public static SimpleUserEvent GetSimpleUserEvent(string key)
        {
            if (_userEventDictionary.ContainsKey(key))
            {
                return _userEventDictionary[key];
            }
            return null;
        }

    }
}
