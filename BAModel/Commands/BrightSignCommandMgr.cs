using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BAModel
{
    class BrightSignCommandMgr
    {
        // logical bits indicating what, if any, parameter validation should be performed
        public const int ValidateCommandNotEmpty = 1;
        public const int ValidateCommandGPIONumber = 2;
        public const int ValidateCommandSingleByte = 4;
        public const int ValidateNonNegativeInteger = 8;
        public const int ValidateNoValidationRequired = 16;
        public const int ValidateCommandByteStream = 32;

        private static Dictionary<string, BrightSignCommand> _transitionCommandDictionary = new Dictionary<string, BrightSignCommand>();
        private static Dictionary<string, BrightSignCommand> _mediaStateCommandDictionary = new Dictionary<string, BrightSignCommand>();

        public static Dictionary<string, BrightSignCommand> TransitionCommandDictionary
        {
            get { return _transitionCommandDictionary; }
        }

        public static Dictionary<string, BrightSignCommand> MediaStateCommandDictionary
        {
            get { return _mediaStateCommandDictionary; }
        }

        public static void InitializeBrightSignCommandMgr()
        {
            BrightSignCommand gpioOnCommand = new BrightSignCommand { Value = "GPIO On", Command = "gpioOnCommand", Parameters = "", ValidationRules = ValidateCommandGPIONumber, ParameterRequired = true };
            BrightSignCommand gpioOffCommand = new BrightSignCommand { Value = "GPIO Off", Command = "gpioOffCommand", Parameters = "", ValidationRules = ValidateCommandGPIONumber, ParameterRequired = true };
            BrightSignCommand gpioSetStateCommand = new BrightSignCommand { Value = "GPIO Set State", Command = "gpioSetStateCommand", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand sendUDPCommand = new BrightSignCommand { Value = "Send UDP", Command = "sendUDPCommand", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand sendUDPBytesCommand = new BrightSignCommand { Value = "Send UDP bytes (comma separated)", Command = "sendUDPBytesCommand", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand sendSerialStringCommand = new BrightSignCommand { Value = "Serial-send string (CR)", Command = "sendSerialStringCommand", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand sendSerialBlockCommand = new BrightSignCommand { Value = "Serial-send string (no CR)", Command = "sendSerialBlockCommand", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand sendSerialByteCommand = new BrightSignCommand { Value = "Serial-send byte", Command = "sendSerialByteCommand", Parameters = "", ValidationRules = ValidateCommandSingleByte, ParameterRequired = true };
            BrightSignCommand sendSerialBytesCommand = new BrightSignCommand { Value = "Serial-send bytes (comma separated)", Command = "sendSerialBytesCommand", Parameters = "", ValidationRules = ValidateCommandByteStream, ParameterRequired = true };
            BrightSignCommand synchronizeCommand = new BrightSignCommand { Value = "Synchronize", Command = "synchronize", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand internalSynchronizeCommand = new BrightSignCommand { Value = "Link Zones", Command = "internalSynchronize", Parameters = "", ValidationRules = 0, ParameterRequired = true };
            BrightSignCommand setVideoVolumeCommand = new BrightSignCommand { Value = "Set Volume (video)", Command = "setVideoVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand incrementVideoVolumeCommand = new BrightSignCommand { Value = "Increment Volume (video)", Command = "incrementVideoVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand decrementVideoVolumeCommand = new BrightSignCommand { Value = "Decrement Volume (video)", Command = "decrementVideoVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand setAudioVolumeCommand = new BrightSignCommand { Value = "Set Volume (audio)", Command = "setAudioVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand incrementAudioVolumeCommand = new BrightSignCommand { Value = "Increment Volume (audio)", Command = "incrementAudioVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand decrementAudioVolumeCommand = new BrightSignCommand { Value = "Decrement Volume (audio)", Command = "decrementAudioVolume", Parameters = "", ValidationRules = ValidateNonNegativeInteger, ParameterRequired = true };
            BrightSignCommand enablePowerSaveModeCommand = new BrightSignCommand { Value = "Enable monitor power save mode", Command = "enablePowerSaveMode", Parameters = "", ValidationRules = ValidateNoValidationRequired, ParameterRequired = false };
            BrightSignCommand disablePowerSaveModeCommand = new BrightSignCommand { Value = "Disable monitor power save mode", Command = "disablePowerSaveMode", Parameters = "", ValidationRules = ValidateNoValidationRequired, ParameterRequired = false };
            BrightSignCommand pauseVideoCommand = new BrightSignCommand { Value = "Pause video", Command = "pauseVideoCommand", Parameters = "", ValidationRules = ValidateNoValidationRequired, ParameterRequired = false };
            BrightSignCommand resumeVideoCommand = new BrightSignCommand { Value = "Resume video", Command = "resumeVideoCommand", Parameters = "", ValidationRules = ValidateNoValidationRequired, ParameterRequired = false };

            _transitionCommandDictionary.Add(sendUDPCommand.Command, sendUDPCommand);
            _transitionCommandDictionary.Add(sendUDPBytesCommand.Command, sendUDPBytesCommand);
            _transitionCommandDictionary.Add(sendSerialStringCommand.Command, sendSerialStringCommand);
            _transitionCommandDictionary.Add(sendSerialBlockCommand.Command, sendSerialBlockCommand);
            _transitionCommandDictionary.Add(sendSerialByteCommand.Command, sendSerialByteCommand);
            _transitionCommandDictionary.Add(sendSerialBytesCommand.Command, sendSerialBytesCommand);
            _transitionCommandDictionary.Add(gpioOnCommand.Command, gpioOnCommand);
            _transitionCommandDictionary.Add(gpioOffCommand.Command, gpioOffCommand);
            _transitionCommandDictionary.Add(gpioSetStateCommand.Command, gpioSetStateCommand);
            _transitionCommandDictionary.Add(synchronizeCommand.Command, synchronizeCommand);
            _transitionCommandDictionary.Add(internalSynchronizeCommand.Command, internalSynchronizeCommand);
            _transitionCommandDictionary.Add(setVideoVolumeCommand.Command, setVideoVolumeCommand);
            _transitionCommandDictionary.Add(incrementVideoVolumeCommand.Command, incrementVideoVolumeCommand);
            _transitionCommandDictionary.Add(decrementVideoVolumeCommand.Command, decrementVideoVolumeCommand);
            _transitionCommandDictionary.Add(setAudioVolumeCommand.Command, setAudioVolumeCommand);
            _transitionCommandDictionary.Add(incrementAudioVolumeCommand.Command, incrementAudioVolumeCommand);
            _transitionCommandDictionary.Add(decrementAudioVolumeCommand.Command, decrementAudioVolumeCommand);
            _transitionCommandDictionary.Add(enablePowerSaveModeCommand.Command, enablePowerSaveModeCommand);
            _transitionCommandDictionary.Add(disablePowerSaveModeCommand.Command, disablePowerSaveModeCommand);
            _transitionCommandDictionary.Add(pauseVideoCommand.Command, pauseVideoCommand);
            _transitionCommandDictionary.Add(resumeVideoCommand.Command, resumeVideoCommand);

            _mediaStateCommandDictionary.Add(sendUDPCommand.Command, sendUDPCommand);
            _mediaStateCommandDictionary.Add(sendUDPBytesCommand.Command, sendUDPBytesCommand);
            _mediaStateCommandDictionary.Add(sendSerialStringCommand.Command, sendSerialStringCommand);
            _mediaStateCommandDictionary.Add(sendSerialBlockCommand.Command, sendSerialBlockCommand);
            _mediaStateCommandDictionary.Add(sendSerialByteCommand.Command, sendSerialByteCommand);
            _mediaStateCommandDictionary.Add(sendSerialBytesCommand.Command, sendSerialBytesCommand);
            _mediaStateCommandDictionary.Add(gpioOnCommand.Command, gpioOnCommand);
            _mediaStateCommandDictionary.Add(gpioOffCommand.Command, gpioOffCommand);
            _mediaStateCommandDictionary.Add(gpioSetStateCommand.Command, gpioSetStateCommand);
            _mediaStateCommandDictionary.Add(setVideoVolumeCommand.Command, setVideoVolumeCommand);
            _mediaStateCommandDictionary.Add(incrementVideoVolumeCommand.Command, incrementVideoVolumeCommand);
            _mediaStateCommandDictionary.Add(decrementVideoVolumeCommand.Command, decrementVideoVolumeCommand);
            _mediaStateCommandDictionary.Add(setAudioVolumeCommand.Command, setAudioVolumeCommand);
            _mediaStateCommandDictionary.Add(incrementAudioVolumeCommand.Command, incrementAudioVolumeCommand);
            _mediaStateCommandDictionary.Add(decrementAudioVolumeCommand.Command, decrementAudioVolumeCommand);
            _mediaStateCommandDictionary.Add(enablePowerSaveModeCommand.Command, enablePowerSaveModeCommand);
            _mediaStateCommandDictionary.Add(disablePowerSaveModeCommand.Command, disablePowerSaveModeCommand);
            _mediaStateCommandDictionary.Add(pauseVideoCommand.Command, pauseVideoCommand);
            _mediaStateCommandDictionary.Add(resumeVideoCommand.Command, resumeVideoCommand);
        }

        public static BrightSignCommand GetBrightSignCommand(string key)
        {
            if (_transitionCommandDictionary.ContainsKey(key))
            {
                return _transitionCommandDictionary[key];
            }
            return null;
        }

    }
}
