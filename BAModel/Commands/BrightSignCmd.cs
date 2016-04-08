using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
//using System.Windows.Controls;

using System.Text.RegularExpressions;

namespace BAModel
{
    public class BrightSignCmd
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public bool DataEntered { get; set; }

        private bool _customUI = false;
        public bool CustomUI
        {
            get { return _customUI; }
            set { _customUI = value; }
        }

        private bool _stateEntrySupported = true;
        public bool StateEntrySupported
        {
            get { return _stateEntrySupported; }
            set { _stateEntrySupported = value; }
        }

        private List<BSUIElement> uiElements = new List<BSUIElement>();
        public List<BSUIElement> UIElements
        {
            get { return uiElements; }
        }

        private List<BSCommand> commands = new List<BSCommand>();
        public List<BSCommand> Commands
        {
            get { return commands; }
        }

        private List<BrightSignModel.ModelFeature> requiredFeatures = new List<BrightSignModel.ModelFeature>();
        public List<BrightSignModel.ModelFeature> RequiredFeatures
        {
            get { return requiredFeatures; }
        }

        public void Assign(BrightSignCmd brightSignCmd)
        {
            Name = brightSignCmd.Name;
            DataEntered = brightSignCmd.DataEntered;
            CustomUI = brightSignCmd.CustomUI;
            StateEntrySupported = brightSignCmd.StateEntrySupported;

            Label = brightSignCmd.Label;

            RequiredFeatures.Clear();
            foreach (BrightSignModel.ModelFeature requiredFeature in brightSignCmd.RequiredFeatures)
            {
                RequiredFeatures.Add(requiredFeature);
            }

            UIElements.Clear();
            foreach (BSUIElement uiElement in brightSignCmd.UIElements)
            {
                UIElements.Add(uiElement);
            }

            Commands.Clear();
            foreach (BSCommand bsCommand in brightSignCmd.Commands)
            {
                Commands.Add(bsCommand.Clone());
            }
        }

        public BrightSignCmd Clone()
        {
            BrightSignCmd bsc = new BrightSignCmd();
            bsc.Name = this.Name;
            bsc.DataEntered = this.DataEntered;
            bsc.CustomUI = this.CustomUI;
            bsc.StateEntrySupported = this.StateEntrySupported;

            bsc.Label = this.Label;
            foreach (BrightSignModel.ModelFeature requiredFeature in this.RequiredFeatures)
            {
                bsc.RequiredFeatures.Add(requiredFeature); // any reason to clone?
            }
            foreach (BSUIElement uiElement in this.UIElements)
            {
                bsc.UIElements.Add(uiElement); // any reason to clone?
            }

            foreach (BSCommand bsCommand in Commands)
            {
                bsc.Commands.Add(bsCommand.Clone());
            }

            return bsc;
        }

        public bool IsEqual(BrightSignCmd bsc)
        {
            if (bsc.Name != this.Name) return false;
            if (bsc.CustomUI != this.CustomUI) return false;
            if (bsc.Commands.Count != this.Commands.Count) return false;
            for (int i = 0; i < this.Commands.Count; i++ )
            {
                if (!bsc.Commands[i].IsEqual(this.Commands[i])) return false;
            }

            return true;
        }

        public virtual void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("brightSignCmd");

            writer.WriteElementString("name", Name);
            writer.WriteElementString("customUI", CustomUI.ToString());

            foreach (BSCommand command in Commands)
            {
                writer.WriteStartElement("command");

                writer.WriteElementString("name", command.Name);

                foreach (string parameterName in command.Parameters.Keys)
                {
                    BSParameter parameter = command.Parameters[parameterName];

                    writer.WriteStartElement("parameter");

                    writer.WriteElementString("name", parameter.ParameterName);
                    writer.WriteElementString("uiElementName", parameter.UIElementName);
                    parameter.ParameterValue.WriteToXml(writer);
                    writer.WriteElementString("itemName", parameter.ItemName);
                    writer.WriteElementString("validationRule", parameter.ValidationRule.ToString());

                    writer.WriteEndElement(); // parameter
                }

                writer.WriteEndElement(); // command

            }

            writer.WriteEndElement(); // brightSignCmd
        }

        public static BrightSignCmd FromBrightSignCommand(BrightSignCommand brightSignCommand)
        {
            BrightSignCmd brightSignCmd = new BrightSignCmd();
            BSCommand bsCommand = new BSCommand();
            BSParameter bsParameter = new BSParameter();

            BSParameter bsPortParameter = new BSParameter();
            bsPortParameter.ParameterName = "port";
            bsPortParameter.UIElementName = "cbPort";
            bsPortParameter.ItemName = "Port";
            bsPortParameter.ValidationRule = -1;

            switch (brightSignCommand.Command)
            {
                case "gpioOnCommand":
                    brightSignCmd.Name = "GPIOOn";
                    bsCommand.Name = "gpioOnCommand";
                    bsParameter.ParameterName = "gpioNumber";
                    bsParameter.UIElementName = "gpioNumber";
                    break;
                case "gpioOffCommand":
                    brightSignCmd.Name = "GPIOOff";
                    bsCommand.Name = "gpioOffCommand";
                    bsParameter.ParameterName = "gpioNumber";
                    bsParameter.UIElementName = "gpioNumber";
                    break;
                case "gpioSetStateCommand":
                    brightSignCmd.Name = "GPIOSetState";
                    bsCommand.Name = "gpioSetStateCommand";
                    bsParameter.ParameterName = "stateValue";
                    bsParameter.UIElementName = "stateValue";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "sendUDPCommand":
                    brightSignCmd.Name = "SendUDP";
                    bsCommand.Name = "sendUDPCommand";
                    bsParameter.ParameterName = "udpString";
                    bsParameter.UIElementName = "udpString";
                    break;
                case "sendUDPBytesCommand":
                    brightSignCmd.Name = "SendUDPBytes";
                    bsCommand.Name = "sendUDPBytesCommand";
                    bsParameter.ParameterName = "byteValues";
                    bsParameter.UIElementName = "byteValues";
                    break;
                case "sendSerialStringCommand":
                    brightSignCmd.Name = "SerialSendStringCR";
                    bsCommand.Name = "sendSerialStringCommand";
                    bsCommand.Parameters.Add(bsPortParameter.ParameterName, bsPortParameter);
                    bsParameter.ParameterName = "serialString";
                    bsParameter.UIElementName = "serialString";
                    break;
                case "sendSerialBlockCommand":
                    brightSignCmd.Name = "SerialSendStringNoCR";
                    bsCommand.Name = "sendSerialBlockCommand";
                    bsCommand.Parameters.Add(bsPortParameter.ParameterName, bsPortParameter);
                    bsParameter.ParameterName = "serialString";
                    bsParameter.UIElementName = "serialString";
                    break;
                case "sendSerialByteCommand":
                    brightSignCmd.Name = "SerialSendByte";
                    bsCommand.Name = "sendSerialByteCommand";
                    bsCommand.Parameters.Add(bsPortParameter.ParameterName, bsPortParameter);
                    bsParameter.ParameterName = "byteValue";
                    bsParameter.UIElementName = "byteValue";
                    break;
                case "sendSerialBytesCommand":
                    brightSignCmd.Name = "SerialSendBytes";
                    bsCommand.Name = "sendSerialByteCommand";
                    bsCommand.Parameters.Add(bsPortParameter.ParameterName, bsPortParameter);
                    bsParameter.ParameterName = "byteValues";
                    bsParameter.UIElementName = "byteValues";
                    break;
                case "synchronize":
                    brightSignCmd.Name = "Synchronize";
                    bsCommand.Name = "synchronize";
                    bsParameter.ParameterName = "synchronizeKeyword";
                    bsParameter.UIElementName = "synchronizeKeyword";
                    break;
                case "internalSynchronize":
                    brightSignCmd.Name = "InternalSynchronize";
                    bsCommand.Name = "internalSynchronize";
                    bsParameter.ParameterName = "synchronizeKeyword";
                    bsParameter.UIElementName = "synchronizeKeyword";
                    break;
                case "setVideoVolume":
                    brightSignCmd.Name = "SetVolume";
                    bsCommand.Name = "setVideoVolume";
                    bsParameter.ParameterName = "volume";
                    bsParameter.UIElementName = "volume";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "incrementVideoVolume":
                    brightSignCmd.Name = "IncrementVolume";
                    bsCommand.Name = "incrementVideoVolume";
                    bsParameter.ParameterName = "volumeDelta";
                    bsParameter.UIElementName = "volumeDelta";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "decrementVideoVolume":
                    brightSignCmd.Name = "DecrementVolume";
                    bsCommand.Name = "decrementVideoVolume";
                    bsParameter.ParameterName = "volumeDelta";
                    bsParameter.UIElementName = "volumeDelta";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "setAudioVolume":
                    brightSignCmd.Name = "SetVolume";
                    bsCommand.Name = "setAudioVolume";
                    bsParameter.ParameterName = "volume";
                    bsParameter.UIElementName = "volume";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "incrementAudioVolume":
                    brightSignCmd.Name = "IncrementVolume";
                    bsCommand.Name = "incrementAudioVolume";
                    bsParameter.ParameterName = "volumeDelta";
                    bsParameter.UIElementName = "volumeDelta";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "decrementAudioVolume":
                    brightSignCmd.Name = "DecrementVolume";
                    bsCommand.Name = "decrementAudioVolume";
                    bsParameter.ParameterName = "volumeDelta";
                    bsParameter.UIElementName = "volumeDelta";
                    bsParameter.ValidationRule = BrightSignCmdMgr.ValidateNonNegativeInteger;
                    break;
                case "enablePowerSaveMode":
                    brightSignCmd.Name = "EnablePowerSaveMode";
                    bsCommand.Name = "enablePowerSaveMode";
                    brightSignCmd.Commands.Add(bsCommand);
                    brightSignCmd.DataEntered = true;
                    return brightSignCmd;
                case "disablePowerSaveMode":
                    brightSignCmd.Name = "DisablePowerSaveMode";
                    bsCommand.Name = "disablePowerSaveMode";
                    brightSignCmd.Commands.Add(bsCommand);
                    brightSignCmd.DataEntered = true;
                    return brightSignCmd;
                case "pauseVideoCommand":
                    brightSignCmd.Name = "PauseVideo";
                    bsCommand.Name = "pauseVideoCommand";
                    brightSignCmd.Commands.Add(bsCommand);
                    brightSignCmd.DataEntered = true;
                    return brightSignCmd;
                case "resumeVideoCommand":
                    brightSignCmd.Name = "ResumeVideo";
                    bsCommand.Name = "resumeVideoCommand";
                    brightSignCmd.Commands.Add(bsCommand);
                    brightSignCmd.DataEntered = true;
                    return brightSignCmd;
                
                default:
                    return null;
            }

            bsParameter.ParameterValue.SetTextValue(brightSignCommand.Parameters);
            bsCommand.Parameters.Add(bsParameter.ParameterName, bsParameter);
            brightSignCmd.Commands.Add(bsCommand);
            brightSignCmd.DataEntered = true;

            return brightSignCmd;
        }

        public static BrightSignCmd ReadXml(XmlReader reader)
        {
            BrightSignCmd brightSignCmd = new BrightSignCmd();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        brightSignCmd.Name = reader.ReadString();
                        break;
                    case "customUI":
                        brightSignCmd.CustomUI = Convert.ToBoolean(reader.ReadString());
                        break;
                    case "command":
                        BSCommand bsCommand = BSCommand.ReadXml(reader);
                        brightSignCmd.Commands.Add(bsCommand);
                        if (bsCommand.Name == "sendBP900Output")
                        {
                            brightSignCmd = ConvertBP900Command(brightSignCmd);
                        }
                        else if (bsCommand.Name == "switchPresentation")
                        {
                            brightSignCmd = ConvertSwitchPresentationCommand(brightSignCmd);
                        }
                        break;
                }
            }

            if (brightSignCmd.Name == "SerialSendStringCR" || brightSignCmd.Name == "SerialSendStringNoCR")
            {
                brightSignCmd.CustomUI = true;
            }

            brightSignCmd.DataEntered = true;

            return brightSignCmd;
        }

        public void ConvertAudioCommand(Sign sign, Zone zone, List<BrightSignCmd> convertedBrightSignCmds)
        {
            BrightSignModel brightSignModel = BrightSignModelMgr.GetBrightSignModel(sign.Model);
            if (brightSignModel != null)
            {
                if (brightSignModel.FeatureIsSupported(BrightSignModel.ModelFeature.AudioOutputControl))
                {
                    if (Name == "SetAudioMixerAnalog" || Name == "SetAudioMixerHDMI" || Name == "SetAudioMixerHDMIMultichannel" || Name == "SetAudioMixerAnalogAndHDMI" || Name == "SetAudioMixerNoAudio")
                    {
                        ConvertSetAudioCommand(sign, zone);
                        convertedBrightSignCmds.Add(this);
                    }

                    if (Name == "SetVolume" || Name == "IncrementVolume" || Name == "DecrementVolume")
                    {
                        ConvertVolumeCommand(sign, zone);
                        convertedBrightSignCmds.Add(this);
                    }

                    if (Name == "MuteAnalogAudio")
                    {
                        ConvertMuteCommand("setAnalogMuteVideo", "true", "false", "false");
                        convertedBrightSignCmds.Add(this);
                    }

                    if (Name == "MuteHDMIAudio")
                    {
                        ConvertMuteCommand("setHDMIMute", "false", "true", "false");
                        convertedBrightSignCmds.Add(this);
                    }

                    if (Name == "MuteDigitalAudio")
                    {
                        ConvertMuteCommand("setSpdifMuteVideo", "false", "true", "true");
                        convertedBrightSignCmds.Add(this);
                    }
                }
            }
        }

        private void ConvertMuteCommand(string oldCommandName, string analogValue, string hdmiValue, string spdifValue)
        {
            BrightSignCmd oldBrightSignCmd = this.Clone();

            foreach (BSCommand oldBSCommand in oldBrightSignCmd.Commands)
            {
                if (oldBSCommand.Name == oldCommandName)
                {
                    Dictionary<string, BSParameter> parameters = oldBSCommand.Parameters;
                    BSParameterValue muteUnmute = parameters["mute"].ParameterValue;
                    string parameterValue = muteUnmute.GetCurrentValue();

                    BrightSignCmd muteUnmuteCmd = null;

                    if (parameterValue == "1") // mute
                    {
                        muteUnmuteCmd = BrightSignCmdMgr.CommandSetByName["MuteAudioOutputs"];
                    }
                    else // unmute
                    {
                        muteUnmuteCmd = BrightSignCmdMgr.CommandSetByName["UnmuteAudioOutputs"];
                    }

                    this.Assign(muteUnmuteCmd);

                    foreach (BSCommand bsCommand in Commands)
                    {
                        foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
                        {
                            string parameterName = kvp.Key;
                            BSParameter parameter = kvp.Value;

                            switch (parameterName)
                            {
                                case "analog":
                                    parameter.ParameterValue.SetTextValue(analogValue);
                                    break;
                                case "hdmi":
                                    parameter.ParameterValue.SetTextValue(hdmiValue);
                                    break;
                                case "spdif":
                                    parameter.ParameterValue.SetTextValue(spdifValue);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void ConvertVolumeCommand(Sign sign, Zone zone)
        {
            BrightSignCmd oldBrightSignCmd = this.Clone();

            BrightSignCmd volumeCmd = null;

            switch (Name)
            {
                case "SetVolume":
                    volumeCmd = BrightSignCmdMgr.CommandSetByName["SetZoneVolume"];
                    break;
                case "IncrementVolume":
                    volumeCmd = BrightSignCmdMgr.CommandSetByName["IncrementZoneVolume"];
                    break;
                case "DecrementVolume":
                    volumeCmd = BrightSignCmdMgr.CommandSetByName["DecrementZoneVolume"];
                    break;
            }

            this.Assign(volumeCmd);

            BSParameterValue volumeParameterValue = null;

            foreach (BSCommand bsCommand in oldBrightSignCmd.Commands)
            {
                Dictionary<string, BSParameter> parameters = bsCommand.Parameters;
                if (bsCommand.Name == "setVideoVolume")
                {
                    volumeParameterValue = parameters["volume"].ParameterValue.Clone();
                }
                else if (bsCommand.Name == "incrementVideoVolume" || bsCommand.Name == "decrementVideoVolume")
                {
                    volumeParameterValue = parameters["volumeDelta"].ParameterValue.Clone();
                }
            }

            foreach (BSCommand bsCommand in Commands)
            {
                foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
                {
                    string parameterName = kvp.Key;
                    BSParameter parameter = kvp.Value;

                    switch (parameterName)
                    {
                        case "zoneId":
                            parameter.ParameterValue.SetTextValue(GetZoneForAudioCommand(sign, zone).ZoneID);
                            break;
                        case "volume":
                            parameter.ParameterValue = volumeParameterValue;
                            break;
                    }
                }
            }
        }

        // Function to convert an old audio command that did not specify the zone to one that does specify the zone
        // Algorithm is as follows
        //      if the current zone is an audio only zone
        //          if the current zone has audio assets
        //              use the current zone
        //          else if there is a video zone
        //              use it
        //          else
        //              use the current zone
        //      else if there is a video zone
        //          use the video zone
        //      else if there is an audio zone
        //          use it
        //      else
        //          use the current zone

        private Zone GetZoneForAudioCommand(Sign sign, Zone zone)
        {
            Zone videoZone = sign.GetExistingVideoZone();
            Zone audioZone = sign.GetExistingAudioZone();

            if (zone.ZoneType == "AudioOnly" || zone.ZoneType == "EnhancedAudio")
            {
                if (!zone.ContainsAudioAsset() && videoZone != null)
                {
                    zone = videoZone;
                }
            }
            else if (videoZone != null)
            {
                zone = videoZone;
            }
            else if (audioZone != null)
            {
                zone = audioZone;
            }

            return zone;
        }

        private void ConvertSetAudioCommand(Sign sign, Zone zone)
        {
            BrightSignCmd oldBrightSignCmd = this.Clone();

            BrightSignCmd setAllAudioOutputsBrightSignCmd = BrightSignCmdMgr.CommandSetByName["SetAllAudioOutputs"];
            this.Assign(setAllAudioOutputsBrightSignCmd);

            Commands.Clear();

            BSCommand existingModeCommand = null;
            foreach (BSCommand bsCommand in oldBrightSignCmd.Commands)
            {
                if (bsCommand.Name == "setAudioModeVideo")
                {
                    existingModeCommand = bsCommand;
                    break;
                }
            }

            string existingModeValue = "stereo";
            if (existingModeCommand != null)
            {
                Dictionary<string, BSParameter> parameters = existingModeCommand.Parameters;
                if (parameters.ContainsKey("mode"))
                {
                    BSParameter parameter = parameters["mode"];
                    BSParameterValue parameterValue = parameter.ParameterValue;
                    List<BSParameterValueItem> parameterValueItems = parameterValue.BSParameterValueItems;
                    BSParameterValueItemText parameterValueItemText = (BSParameterValueItemText)parameterValueItems[0];
                    switch (parameterValueItemText.Value)
                    {
                        case "3":
                            existingModeValue = "left";
                            break;
                        case "4":
                            existingModeValue = "right";
                            break;
                    }
                }
            }

            // SetAllAudioOutputs
            BSCommand setAllAudioOutputsCmd = new BSCommand { Name = "setAllAudioOutputs" };
            Dictionary<string, BSParameter> setAllOutputsParameters = setAllAudioOutputsCmd.Parameters;

            // analog
            BSParameter analogParameter = new BSParameter { ParameterName = "analog" };
            BSParameterValue analogParameterValue = new BSParameterValue();
            BSParameterValueItemText analogParameterValueItemText = new BSParameterValueItemText();

            // HDMI
            BSParameter hdmiParameter = new BSParameter { ParameterName = "hdmi" };
            BSParameterValue hdmiParameterValue = new BSParameterValue();
            BSParameterValueItemText hdmiParameterValueItemText = new BSParameterValueItemText();

            // SPDIF
            BSParameter spdifParameter = new BSParameter { ParameterName = "spdif" };
            BSParameterValue spdifParameterValue = new BSParameterValue();
            BSParameterValueItemText spdifParameterValueItemText = new BSParameterValueItemText();

            // SetAudioMode
            BSCommand setAudioMode = new BSCommand { Name = "setAudioMode" };
            Dictionary<string, BSParameter> setAudioModeParameters = setAudioMode.Parameters;

            BSParameter modeParameter = new BSParameter { ParameterName = "mode" };
            BSParameterValue modeParameterValue = new BSParameterValue();
            BSParameterValueItemText modeParameterValueItemText = new BSParameterValueItemText();

            switch (oldBrightSignCmd.Name)
            {
                case "SetAudioMixerAnalog":
                    analogParameterValueItemText.Value = "pcm";
                    hdmiParameterValueItemText.Value = "none";
                    spdifParameterValueItemText.Value = "none";
                    modeParameterValueItemText.Value = existingModeValue;
                    break;
                case "SetAudioMixerHDMI":
                    analogParameterValueItemText.Value = "none";
                    hdmiParameterValueItemText.Value = "pcm";
                    spdifParameterValueItemText.Value = "pcm";
                    modeParameterValueItemText.Value = existingModeValue;
                    break;
                case "SetAudioMixerHDMIMultichannel":
                    analogParameterValueItemText.Value = "none";
                    hdmiParameterValueItemText.Value = "passthrough";
                    spdifParameterValueItemText.Value = "passthrough";
                    modeParameterValueItemText.Value = "stereo";
                    break;
                case "SetAudioMixerAnalogAndHDMI":
                    analogParameterValueItemText.Value = "pcm";
                    hdmiParameterValueItemText.Value = "passthrough";
                    spdifParameterValueItemText.Value = "passthrough";
                    modeParameterValueItemText.Value = existingModeValue;
                    break;
                case "SetAudioMixerNoAudio":
                    analogParameterValueItemText.Value = "none";
                    hdmiParameterValueItemText.Value = "none";
                    spdifParameterValueItemText.Value = "none";
                    modeParameterValueItemText.Value = "stereo";
                    break;
            }

            // SetAllAudioOutputs

            BSParameter zoneParameter = new BSParameter { ParameterName = "zoneId" };
            zoneParameter.ParameterValue.SetTextValue(GetZoneForAudioCommand(sign, zone).ZoneID);
            setAllOutputsParameters.Add("zoneId", zoneParameter);

            analogParameterValue.BSParameterValueItems.Add(analogParameterValueItemText);
            analogParameter.ParameterValue = analogParameterValue;
            setAllOutputsParameters.Add("analog", analogParameter);

            hdmiParameterValue.BSParameterValueItems.Add(hdmiParameterValueItemText);
            hdmiParameter.ParameterValue = hdmiParameterValue;
            setAllOutputsParameters.Add("hdmi", hdmiParameter);

            spdifParameterValue.BSParameterValueItems.Add(spdifParameterValueItemText);
            spdifParameter.ParameterValue = spdifParameterValue;
            setAllOutputsParameters.Add("spdif", spdifParameter);

            Commands.Add(setAllAudioOutputsCmd);

            // SetAudioMode

            zoneParameter = new BSParameter { ParameterName = "zoneId" };
            zoneParameter.ParameterValue.SetTextValue(GetZoneForAudioCommand(sign, zone).ZoneID);
            setAudioModeParameters.Add("zoneId", zoneParameter);

            modeParameterValue.BSParameterValueItems.Add(modeParameterValueItemText);
            modeParameter.ParameterValue = modeParameterValue;
            setAudioModeParameters.Add("mode", modeParameter);

            Commands.Add(setAudioMode);

            // ConfigureAudioResources
            BSCommand configureResources = new BSCommand { Name = "configureResources" };
            Commands.Add(configureResources);
        }

        private static BrightSignCmd ConvertSwitchPresentationCommand(BrightSignCmd existingSwitchPresentationCmd)
        {
            // need to convert the command if it only has one parameter - presentationName
            if (existingSwitchPresentationCmd.Commands[0].Parameters.Count == 2)
            {
                return existingSwitchPresentationCmd;
            }

            BrightSignCmd switchPresentationCmd = null;

            List<BrightSignCmd> brightSignCmds = BrightSignCmdMgr.CommandSetList;
            foreach (BrightSignCmd brightSignCmd in brightSignCmds)
            {
                if (brightSignCmd.Name == "SwitchPresentation")
                {
                    switchPresentationCmd = brightSignCmd.Clone();
                }
            }
            
            // get presentation name from existing switch presentation command
            string existingPresentationName = String.Empty;
            Dictionary<string, BSParameter> existingParameters = existingSwitchPresentationCmd.Commands[0].Parameters;
            foreach (KeyValuePair<string, BSParameter> kvp in existingParameters)
            {
                BSParameter bsParameter = kvp.Value;
                if (kvp.Key == "presentationName")
                {
                    existingPresentationName = bsParameter.ParameterValue.GetCurrentValue();
                }
            }

            // set values in new switch presentation command
            Dictionary<string, BSParameter> parameters = switchPresentationCmd.Commands[0].Parameters;
            foreach (KeyValuePair<string, BSParameter> kvp in parameters)
            {
                BSParameter bsParameter = kvp.Value;
                switch (kvp.Key)
                {
                    case "presentationName":
                        bsParameter.ParameterValue.SetTextValue(existingPresentationName);
                        break;
                }
            }

            // add presentation to Sign if necessary
            Sign sign = Sign.SignBeingRead;
            PresentationIdentifierSet presentationIdentifierSet = sign.PresentationIdentifierSet;
            Dictionary<string, PresentationIdentifier> presentationIdentifiers = presentationIdentifierSet.PresentationIdentifiers;
            if (!presentationIdentifiers.ContainsKey(existingPresentationName))
            {
                string currentDirectory = BrightAuthorUtils.RetrieveDirectory(BrightAuthorUtils.DirectoryCategories.Presentations);
                string path = System.IO.Path.Combine(currentDirectory, existingPresentationName + ".bpf");
                presentationIdentifiers.Add(existingPresentationName,
                    new PresentationIdentifier
                    {
                        Name = existingPresentationName,
                        PresentationName = existingPresentationName,
                        Path = path
                    }
                    );
            }

            return switchPresentationCmd;
        }

        private static BrightSignCmd ConvertBP900Command(BrightSignCmd bp900Cmd)
        {
            BrightSignCmd bpCmd = null;

            List<BrightSignCmd> brightSignCmds = BrightSignCmdMgr.CommandSetList;
            foreach (BrightSignCmd brightSignCmd in brightSignCmds)
            {
                if (brightSignCmd.Name == "BP900AOutput")
                {
                    bpCmd = brightSignCmd.Clone();

                    BSCommand bpCommand = bpCmd.Commands[0];
                    BSCommand bp900Command = bp900Cmd.Commands[0];

                    Dictionary<string, BSParameter> bpParametersDictionary = bpCommand.Parameters;

                    Dictionary<string, BSParameter> bp900ParametersDictionary = bp900Command.Parameters;

                    BSParameter bpActionParameter = bpParametersDictionary["action"];
                    BSParameter bp900ActionParameter = bp900ParametersDictionary["action"];
                    bpActionParameter.ItemName = bp900ActionParameter.ItemName;
                    bpActionParameter.UIElementName = bp900ActionParameter.UIElementName;
                    bpActionParameter.ValidationRule = bp900ActionParameter.ValidationRule;
                    bpActionParameter.ParameterValue = bp900ActionParameter.ParameterValue.Clone();

                    BSParameter bpButtonNumberParameter = bpParametersDictionary["buttonNumber"];
                    BSParameter bp900ButtonNumberParameter = bp900ParametersDictionary["buttonNumber"];
                    bpButtonNumberParameter.ItemName = bp900ButtonNumberParameter.ItemName;
                    bpButtonNumberParameter.UIElementName = bp900ButtonNumberParameter.UIElementName;
                    bpButtonNumberParameter.ValidationRule = bp900ButtonNumberParameter.ValidationRule;
                    bpButtonNumberParameter.ParameterValue = bp900ButtonNumberParameter.ParameterValue.Clone();
                }
            }
            return bpCmd;
        }

        public bool DebugPortUsed()
        {
            List<BSCommand> bsCommands = Commands;
            foreach (BSCommand bsCommand in bsCommands)
            {
                if ((bsCommand.Name == "sendSerialStringCommand") || (bsCommand.Name == "sendSerialBlockCommand") || (bsCommand.Name == "sendSerialByteCommand") || (bsCommand.Name == "sendSerialBytesCommand"))
                {
                    Dictionary<string, BSParameter> bsParameters = bsCommand.Parameters;
                    foreach (KeyValuePair<string, BSParameter> bsParameter in bsParameters)
                    {
                        if (bsParameter.Key == "port")
                        {
                            BSParameter bsp = bsParameter.Value;
                            string textValue;
                            if (bsp.ParameterValue.IsTextOnlyParameter(out textValue))
                            {
                                if (textValue == "0") return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void GetSerialPortsUsed(SortedList<int, SerialPortConfiguration> usedSerialPorts)
        {
            List<BSCommand> bsCommands = Commands;
            foreach (BSCommand bsCommand in bsCommands)
            {
                if ((bsCommand.Name == "sendSerialStringCommand") || (bsCommand.Name == "sendSerialBlockCommand") || (bsCommand.Name == "sendSerialByteCommand") || (bsCommand.Name == "sendSerialBytesCommand"))
                {
                    Dictionary<string, BSParameter> bsParameters = bsCommand.Parameters;
                    foreach (KeyValuePair<string, BSParameter> bsParameter in bsParameters)
                    {
                        if (bsParameter.Key == "port")
                        {
                            BSParameter bsp = bsParameter.Value;
                            string textValue;
                            if (bsp.ParameterValue.IsTextOnlyParameter(out textValue))
                            {
                                int port = Convert.ToInt16(textValue);
                                if (!usedSerialPorts.ContainsKey(port))
                                {
                                    usedSerialPorts.Add(port, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<UserVariable> GetUserVariablesInUse()
        {
            List<UserVariable> userVariables = new List<UserVariable>();

            foreach (BSCommand bsCommand in Commands)
            {
                bsCommand.GetUserVariablesInUse(userVariables);
            }

            return userVariables;
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            foreach (BSCommand bsCommand in Commands)
            {
                bsCommand.UpdateUserVariables(userVariableSet);
            }
        }

        public string GetScriptPluginsInUse()
        {
            foreach (BSCommand bsCommand in Commands)
            {
                if (bsCommand.Name == "sendPluginMessage")
                {
                    foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
                    {
                        if (kvp.Key == "pluginName")
                        {
                            BSParameter bsParameter = kvp.Value;
                            BSParameterValue bsParameterValue = bsParameter.ParameterValue;
                            return bsParameterValue.GetCurrentValue();
                        }
                    }
                }
            }
            return String.Empty;
        }

        public void UpdateScriptPlugins(ScriptPluginSet scriptPluginSet)
        {
            foreach (BSCommand bsCommand in Commands)
            {
                if (bsCommand.Name == "sendPluginMessage")
                {
                    foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
                    {
                        if (kvp.Key == "pluginName")
                        {
                            BSParameter bsParameter = kvp.Value;
                            BSParameterValue bsParameterValue = bsParameter.ParameterValue;
                            string updatedScriptPluginName = scriptPluginSet.UpdateScriptPluginName(bsParameterValue.GetCurrentValue());
                            bsParameterValue.SetTextValue(updatedScriptPluginName);
                        }
                    }
                }
            }
        }

        public List<PresentationIdentifier> GetPresentationsInUse()
        {
            List<PresentationIdentifier> presentationIdentifiers = new List<PresentationIdentifier>();

            foreach (BSCommand bsCommand in Commands)
            {
                if (bsCommand.Name == "switchPresentation")
                {
                    bsCommand.GetPresentationsInUse(presentationIdentifiers);
                }
            }

            return presentationIdentifiers;
        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            foreach (BSCommand bsCommand in Commands)
            {
                if (bsCommand.Name == "switchPresentation")
                {
                    bsCommand.UpdatePresentationIdentifiers(presentationIdentifierSet);
                }
            }
        }

        public string GetZoneId()
        {
            string zoneId = String.Empty;
            foreach (BSCommand bsCommand in Commands)
            {
                foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
                {
                    string parameterName = kvp.Key;
                    BSParameter bsParameter = kvp.Value;

                    switch (parameterName)
                    {
                        case "zoneId":
                            zoneId = bsParameter.ParameterValue.GetValue();
                            break;
                    }
                }
            }
            return zoneId;
        }
    }

    public class TimedBrightSignCmd : BrightSignCmd
    {
        public string Timeout { get; set; }

        public TimedBrightSignCmd(BrightSignCmd brightSignCmd, string timeout)
        {
            this.Assign(brightSignCmd);
            Timeout = timeout;
        }

        public new TimedBrightSignCmd Clone()
        {
            BrightSignCmd brightSignCmd = (this as BrightSignCmd).Clone();
            TimedBrightSignCmd timedBrightSignCmd = new TimedBrightSignCmd(brightSignCmd, this.Timeout);
            return timedBrightSignCmd;
        }

        public new static TimedBrightSignCmd ReadXml(XmlReader reader)
        {
            string timeout = String.Empty;
            BrightSignCmd brightSignCmd = null;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "timeout":
                        timeout = reader.ReadString();
                        break;
                    case "brightSignCmd":
                        brightSignCmd = BrightSignCmd.ReadXml(reader);
                        break;
                }
            }

            TimedBrightSignCmd timedBrightSignCmd = new TimedBrightSignCmd(brightSignCmd, timeout);

            return timedBrightSignCmd;
        }

        public override void WriteToXml(System.Xml.XmlTextWriter writer)
        {
            writer.WriteStartElement("timedBrightSignCmd");

            writer.WriteElementString("timeout", Timeout);

            base.WriteToXml(writer);

            writer.WriteEndElement(); // timedBrightSignCmd
        }

        public void PublishToXml(System.Xml.XmlTextWriter writer)
        {
            base.WriteToXml(writer);
        }
        
        public bool IsEqual(TimedBrightSignCmd timedBrightSignCmd)
        {
            if (this.Timeout != timedBrightSignCmd.Timeout) return false;

            return base.IsEqual(timedBrightSignCmd);
        }

    }

    public class BSUIElement
    {
        public string Name { get; set; }
        public BSMargin Margin { get; set; }
        public string MinWidth { get; set; }
    }

    public class BSTextBox : BSUIElement
    {
        public string Default { get; set; }
        public string MaxWidth { get; set; }
    }

    public class BSComboBox : BSUIElement
    {
        public string DefaultIndex { get; set; }

        private List<BSComboBoxItem> items = new List<BSComboBoxItem>();
        public List<BSComboBoxItem> Items
        {
            get { return items; }
        }
    }

    public class BSComboBoxItem
    {
        public string Label { get; set; }
        private List<BSComboBoxItemParameter> parameterItems = new List<BSComboBoxItemParameter>();
        public List<BSComboBoxItemParameter> ParameterItems
        {
            get { return parameterItems; }
        }
    }

    public class BSComboBoxItemParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class BSCheckBoxComboBox : BSUIElement
    {
        public string Prompt { get; set; }

        private List<BSCheckBoxComboBoxItem> items = new List<BSCheckBoxComboBoxItem>();
        public List<BSCheckBoxComboBoxItem> Items
        {
            get { return items; }
        }
    }

    public class BSCheckBoxComboBoxItem
    {
        public string Label { get; set; }
        BSCheckBoxComboBoxItemParameter parameterItem = new BSCheckBoxComboBoxItemParameter();
        public BSCheckBoxComboBoxItemParameter ParameterItem
        {
            get { return parameterItem; }
            set { parameterItem = value; }
        }
    }

    public class BSCheckBoxComboBoxItemParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Checked { get; set; }
    }

    public class BSTextBlock : BSUIElement
    {
        public string Text { get; set; }
    }

    public class BSMargin
    {
        public string Left { get; set; }
        public string Top { get; set; }
        public string Right { get; set; }
        public string Bottom { get; set; }

//        public Thickness GetMarginValues()
//        {
//            double left = Convert.ToDouble(Left);
//            double top = Convert.ToDouble(Top);
//            double right = Convert.ToDouble(Right);
//            double bottom = Convert.ToDouble(Bottom);
//            return new Thickness(left, top, right, bottom);
//        }
    }

    public class BSCommand
    {
        public string Name { get; set; }

        private Dictionary<string, BSParameter> parameters = new Dictionary<string, BSParameter>();
        public Dictionary<string, BSParameter> Parameters
        {
            get { return parameters; }
        }

        /*
        private List<BSParameter> parameters = new List<BSParameter>();
        public List<BSParameter> Parameters
        {
            get { return parameters; }
        }
        */

        public bool IsEqual(BSCommand bsCommand)
        {
            if (bsCommand.Name != this.Name) return false;
            if (bsCommand.Parameters.Count != this.Parameters.Count) return false;

            foreach (string parameterName in this.Parameters.Keys)
            {
                if (bsCommand.Parameters.ContainsKey(parameterName))
                {
                    BSParameter bsParameter = bsCommand.Parameters[parameterName];
                    if (!bsParameter.IsEqual(this.Parameters[parameterName])) return false;
                }
            }

            return true;
        }

        public BSCommand Clone()
        {
            BSCommand bsCommand = new BSCommand();
            bsCommand.Name = this.Name;

            foreach (KeyValuePair<string, BSParameter> kvp in this.Parameters)
            {
                BSParameter bsParameter = kvp.Value;
                BSParameter bsParameterNew = bsParameter.Clone();
                bsCommand.Parameters.Add(kvp.Key, bsParameterNew);
            }

            return bsCommand;
        }

        public static BSCommand ReadXml(XmlReader reader)
        {
            BSCommand bsCommand = new BSCommand();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        bsCommand.Name = reader.ReadString();
                        break;
                    case "parameter":
                        BSParameter bsParameter = BSParameter.ReadXml(reader);
                        bsCommand.Parameters.Add(bsParameter.ParameterName, bsParameter);
                        break;
                }
            }

            return bsCommand;
        }

        public void GetUserVariablesInUse(List<UserVariable> userVariables)
        {
            foreach (KeyValuePair<string, BSParameter> kvp in this.Parameters)
            {
                BSParameter bsParameter = kvp.Value;
                bsParameter.GetUserVariablesInUse(userVariables);
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            foreach (KeyValuePair<string, BSParameter> kvp in this.Parameters)
            {
                BSParameter bsParameter = kvp.Value;
                bsParameter.UpdateUserVariables(userVariableSet);
            }
        }

        public void GetPresentationsInUse(List<PresentationIdentifier> presentationIdentifiers)
        {
            string useUserVariable = String.Empty;
            string presentationIdentifierName = String.Empty;

            foreach (KeyValuePair<string, BSParameter> kvp in this.Parameters)
            {
                BSParameter bsParameter = kvp.Value;
                BSParameterValue bsParameterValue = bsParameter.ParameterValue;
                string parameterValue = bsParameterValue.GetCurrentValue();

                switch (bsParameter.ParameterName)
                {
                    case "useUserVariable":
                        useUserVariable = bsParameter.ParameterValue.GetCurrentValue();
                        break;
                    case "presentationName":
                        presentationIdentifierName = bsParameter.ParameterValue.GetCurrentValue();
                        break;
                }
            }

            if (useUserVariable != "true")
            {
                Dictionary<string, PresentationIdentifier> signPresentationIdentifiers = Sign.CurrentSign.PresentationIdentifierSet.PresentationIdentifiers;
                if (signPresentationIdentifiers.ContainsKey(presentationIdentifierName))
                {
                    presentationIdentifiers.Add(signPresentationIdentifiers[presentationIdentifierName]);
                }
            }

        }

        public void UpdatePresentationIdentifiers(PresentationIdentifierSet presentationIdentifierSet)
        {
            string useUserVariable = String.Empty;
            string presentationIdentifierName = String.Empty;
            BSParameterValue presentationIdentifierParameterValue = null;

            foreach (KeyValuePair<string, BSParameter> kvp in this.Parameters)
            {
                BSParameter bsParameter = kvp.Value;
                BSParameterValue bsParameterValue = bsParameter.ParameterValue;
                string parameterValue = bsParameterValue.GetCurrentValue();

                switch (bsParameter.ParameterName)
                {
                    case "useUserVariable":
                        useUserVariable = bsParameter.ParameterValue.GetCurrentValue();
                        break;
                    case "presentationName":
                        presentationIdentifierName = bsParameter.ParameterValue.GetCurrentValue();
                        presentationIdentifierParameterValue = bsParameter.ParameterValue;
                        break;
                }
            }

            if (useUserVariable != "true" && presentationIdentifierParameterValue != null)
            {
                string updatedPresentationIdentifierName = presentationIdentifierSet.UpdatePresentationIdentifierName(presentationIdentifierName);
                presentationIdentifierParameterValue.SetTextValue(updatedPresentationIdentifierName);
            }
        }
    }

    public class BSParameter
    {
        private BSParameterValue _bsParameterValue = new BSParameterValue();

        public string Type { get; set; }
        public string UIElementName { get; set; }
        public string ParameterName { get; set; }
        public BSParameterValue ParameterValue
        {
            get { return _bsParameterValue; }
            set { _bsParameterValue = value; }
        }
        public string ItemName { get; set; }
        public int ValidationRule { get; set; }

        public bool IsEqual(BSParameter bsParameter)
        {
            if (this.ParameterName != bsParameter.ParameterName) return false;
            if (!this.ParameterValue.IsEqual(bsParameter.ParameterValue)) return false;
            return true;
        }

        public BSParameter Clone()
        {
            BSParameter bsParameter = new BSParameter();
            bsParameter.ParameterName = this.ParameterName;
            bsParameter.ParameterValue = this.ParameterValue.Clone();
            bsParameter.Type = this.Type;
            bsParameter.UIElementName = this.UIElementName;
            bsParameter.ItemName = this.ItemName;
            bsParameter.ValidationRule = this.ValidationRule;

            return bsParameter;
        }

        public string Validate()
        {
            string value;
            if (_bsParameterValue.IsTextOnlyParameter(out value))
            {
                if (ValidationRule > 0)
                {
                    if (value.Length == 0)
                    {
                        return BrightAuthorUtils.GetLocalizedString("ParameterCantBeBlank");
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateNonNegativeInteger) != 0)
                    {
                        int numericEntry = 0;
                        bool success = Int32.TryParse(value, out numericEntry);
                        if ((!success) || (numericEntry < 0))
                        {
                            return BrightAuthorUtils.GetLocalizedString("ValueMustBePositiveNumber");
                        }
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateIRRemoteOut) != 0)
                    {
                        string param = value;

                        if (param.StartsWith("b-"))
                        {
                            param = param.Substring(2);
                        }

                        int numericEntry = 0;
                        bool success = Int32.TryParse(param, out numericEntry);
                        if ((!success) || (numericEntry < 0))
                        {
                            return BrightAuthorUtils.GetLocalizedString("ValueMustBePositiveNumber");
                        }
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateCommandGPIONumber) != 0)
                    {
                        int gpioIndex = 0;
                        bool success = Int32.TryParse(value, out gpioIndex);
                        if ((!success) || ((gpioIndex < 0) || (gpioIndex > 7)))
                        {
                            return BrightAuthorUtils.GetLocalizedString("ValidGPIOEntry");
                        }
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateCommandSingleByte) != 0)
                    {
                        int numericEntry = 0;
                        bool success = Int32.TryParse(value, out numericEntry);
                        if (!success)
                        {
                            return BrightAuthorUtils.GetLocalizedString("ValueMustBe0to255");
                        }
                        if ((numericEntry < 0) || (numericEntry > 255))
                        {
                            return BrightAuthorUtils.GetLocalizedString("ValueMustBe0to255");
                        }
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateCommandByteStream) != 0)
                    {
                        string retVal = ValidateCommandByteStream(value);
                        if (retVal != String.Empty)
                        {
                            return retVal;
                        }
                    }

                    if ((ValidationRule & BrightSignCmdMgr.ValidateVariable) != 0)
                    {
                        if (!_bsParameterValue.IsVariableOnlyParameter())
                        {
                            return "Not a variable";
                        }
                    }
                }
            }

            return "";
        }

        public static string ValidateCommandByteStream(string s)
        {
            // verify that all characters are either numbers of a comma symbol.
            // verify that numbers between commas are valid

            string parameter = s.Trim();
            char[] chars = parameter.ToCharArray();
            foreach (char c in chars)
            {
                if ((c != ',') && (c != ' '))
                {
                    if (c < '0' || c > '9')
                    {
                        return BrightAuthorUtils.GetLocalizedString("OnlyNumbersAndCommas");
                    }
                }
            }

            string[] byteValues = parameter.Split(new Char[] { ',' });
            foreach (string byteValue in byteValues)
            {
                string trimmedByteValue = byteValue.Trim();
                string retVal = ValidateByteValue(trimmedByteValue);
                if (retVal != String.Empty)
                {
                    return retVal;
                }
            }

            return String.Empty;
        }

        private static string ValidateByteValue(string byteValue)
        {
            int numericEntry = 0;
            bool success = Int32.TryParse(byteValue, out numericEntry);
            if (!success)
            {
                return BrightAuthorUtils.GetLocalizedString("ValueMustBe0to255");
            }
            if ((numericEntry < 0) || (numericEntry > 255))
            {
                return BrightAuthorUtils.GetLocalizedString("ValueMustBe0to255");
            }
            return "";
        }

        public static BSParameter ReadXml(XmlReader reader)
        {
            BSParameter bsParameter = new BSParameter();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "name":
                        bsParameter.ParameterName = reader.ReadString();
                        break;
                    case "uiElementName":
                        bsParameter.UIElementName = reader.ReadString();
                        break;
                    case "value":
                        bsParameter.ParameterValue.SetTextValue(reader.ReadString());
                        break;
                    case "parameterValue":
                        bsParameter.ParameterValue = BSParameterValue.ReadXml(reader);
                        break;
                    case "itemName":
                        bsParameter.ItemName = reader.ReadString();
                        break;
                    case "validationRule":
                        string validationRuleStr = reader.ReadString();
                        bsParameter.ValidationRule = BrightSignCmdMgr.GetValidationRule(validationRuleStr);
                        break;
                }
            }

            return bsParameter;
        }

        public void GetUserVariablesInUse(List<UserVariable> userVariables)
        {
            _bsParameterValue.GetUserVariablesInUse(userVariables);
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            _bsParameterValue.UpdateUserVariables(userVariableSet);
        }
    }

    public class BSParameterValue
    {
        private List<BSParameterValueItem> _bsParameterValueItems = new List<BSParameterValueItem>();

        public List<BSParameterValueItem> BSParameterValueItems
        {
            get { return _bsParameterValueItems; }
        }

        public BSParameterValue Clone()
        {
            BSParameterValue bsParameterValue = new BSParameterValue();
            foreach (BSParameterValueItem bsParameterItemValue in _bsParameterValueItems)
            {
                BSParameterValueItem newBSParameterValueItem = (BSParameterValueItem)bsParameterItemValue.Clone();
                bsParameterValue.BSParameterValueItems.Add(newBSParameterValueItem);
            }

            return bsParameterValue;
        }

        public bool IsEqual(BSParameterValue bsParameterValue)
        {
            if (this.BSParameterValueItems.Count() != bsParameterValue.BSParameterValueItems.Count) return false;

            for (int i = 0; i < this.BSParameterValueItems.Count(); i++ )
            {
                if (!this.BSParameterValueItems[i].IsEqual(bsParameterValue.BSParameterValueItems[i])) return false;
            }

            return true;
        }

        public string SetValue(string parameterValueSpec)
        {
            BSParameterValueItemText bsParameterValueItemText = null;

            _bsParameterValueItems.Clear();

            Regex r = new Regex("\\$\\$");
            string[] subStrings = r.Split(parameterValueSpec.Trim());

            if (subStrings.Length < 3)
            {
                bsParameterValueItemText = new BSParameterValueItemText();
                bsParameterValueItemText.Value = parameterValueSpec.Trim();
                _bsParameterValueItems.Add(bsParameterValueItemText);
            }
            else
            {
                UserVariableSet userVariableSet = Sign.CurrentSign.UserVariableSet;
                Dictionary<string, UserVariable> userVariables = userVariableSet.UserVariables;

                bool gettingText = true;

                foreach (string subString in subStrings)
                {
                    if (subString != String.Empty)
                    {
                        if (gettingText)
                        {
                            bsParameterValueItemText = new BSParameterValueItemText();
                            bsParameterValueItemText.Value = subString;
                            _bsParameterValueItems.Add(bsParameterValueItemText);
                        }
                        else
                        {
                            string variableName = subString;

                            if (variableName.StartsWith("_"))
                            {
                                BSParameterValueItemMediaCounterVariable bsParameterValueItemMediaCounterVariable =
                                    new BSParameterValueItemMediaCounterVariable
                                    {
                                        FileName = variableName
                                    };
                                _bsParameterValueItems.Add(bsParameterValueItemMediaCounterVariable);
                            }
                            else if (userVariables.ContainsKey(variableName))
                            {
                                UserVariable userVariable = userVariables[variableName];
                                BSParameterValueItemUserVariable bsParameterValueItemUserVariable = new BSParameterValueItemUserVariable();
                                bsParameterValueItemUserVariable.UserVariable = userVariable;
                                _bsParameterValueItems.Add(bsParameterValueItemUserVariable);
                            }
                            else
                            {
                                return "Variable " + variableName + " not found.";
                            }
                        }
                    }

                    gettingText = !gettingText;

                }
            }

            return "";
        }

        public string GetValue()
        {
            string value = String.Empty;

            foreach (BSParameterValueItem bsParameterValueItem in _bsParameterValueItems)
            {
                if (bsParameterValueItem is BSParameterValueItemText)
                {
                    value += (bsParameterValueItem as BSParameterValueItemText).Value;
                }
                else if (bsParameterValueItem is BSParameterValueItemUserVariable)
                {
                    value += String.Concat("$$", (bsParameterValueItem as BSParameterValueItemUserVariable).UserVariable.Name, "$$");
                }
                else if (bsParameterValueItem is BSParameterValueItemMediaCounterVariable)
                {
                    value += String.Concat("$$", (bsParameterValueItem as BSParameterValueItemMediaCounterVariable).FileName, "$$");
                }
            }

            return value;
        }

        public string GetCurrentValue()
        {
            string value = String.Empty;

            foreach (BSParameterValueItem bsParameterValueItem in _bsParameterValueItems)
            {
                if (bsParameterValueItem is BSParameterValueItemText)
                {
                    value += (bsParameterValueItem as BSParameterValueItemText).Value;
                }
                else if (bsParameterValueItem is BSParameterValueItemUserVariable)
                {
                    UserVariable userVariable = (bsParameterValueItem as BSParameterValueItemUserVariable).UserVariable;
                    value += userVariable.DefaultValue;
                }
                else if (bsParameterValueItem is BSParameterValueItemMediaCounterVariable)
                {
                    value += "0";
                }
            }

            return value;
        }

        public void SetTextValue(string textValue)
        {
            _bsParameterValueItems.Clear();
            BSParameterValueItemText bsParameterValueItemText = new BSParameterValueItemText();
            bsParameterValueItemText.Value = textValue;
            _bsParameterValueItems.Add(bsParameterValueItemText);
        }

        public static BSParameterValue ReadXml(XmlReader reader)
        {
            BSParameterValue bsParameterValue = new BSParameterValue();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "parameterValueItemText":
                        BSParameterValueItemText bsParameterValueItemText = BSParameterValueItemText.ReadXml(reader);
                        bsParameterValue._bsParameterValueItems.Add(bsParameterValueItemText);
                        break;
                    case "parameterValueItemUserVariable":
                        BSParameterValueItemVariable bsParameterValueItemUserVariable = BSParameterValueItemUserVariable.ReadXml(reader);
                        bsParameterValue._bsParameterValueItems.Add(bsParameterValueItemUserVariable);
                        break;
                    case "parameterValueItemMediaCounterVariable":
                        BSParameterValueItemVariable bsParameterValueItemMediaCounterVariable = BSParameterValueItemMediaCounterVariable.ReadXml(reader);
                        bsParameterValue._bsParameterValueItems.Add(bsParameterValueItemMediaCounterVariable);
                        break;
                }
            }

            return bsParameterValue;
        }

        public void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("parameterValue");

            foreach (BSParameterValueItem bsParameterValueItem in _bsParameterValueItems)
            {
                bsParameterValueItem.WriteToXml(writer);
            }

            writer.WriteFullEndElement(); // parameterValue
        }

        public void GetUserVariablesInUse(List<UserVariable> userVariables)
        {
            foreach (BSParameterValueItem bsParameterValueItem in _bsParameterValueItems)
            {
                bsParameterValueItem.GetUserVariablesInUse(userVariables);
            }
        }

        public void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            foreach (BSParameterValueItem bsParameterValueItem in _bsParameterValueItems)
            {
                bsParameterValueItem.UpdateUserVariables(userVariableSet);
            }
        }

        public bool IsTextOnlyParameter(out string textValue)
        {
            textValue = String.Empty;

            bool textOnly = false;

            if (BSParameterValueItems.Count == 1 && BSParameterValueItems[0] is BSParameterValueItemText)
            {
                BSParameterValueItemText bsParameterValueItemText = BSParameterValueItems[0] as BSParameterValueItemText;
                textValue = bsParameterValueItemText.Value;
                textOnly = true;
            }

            return textOnly;
        }

        public bool IsVariableOnlyParameter()
        {
            if (BSParameterValueItems.Count == 1 && BSParameterValueItems[0] is BSParameterValueItemVariable)
            {
                return true;
            }
            return false;
        }
    }

    public class BSParameterValueItem
    {
        public virtual void WriteToXml(System.Xml.XmlTextWriter writer)
        {
        }

        public virtual Object Clone()
        {
            return null;
        }

        public virtual bool IsEqual(Object obj)
        {
            return false;
        }

        public virtual void GetUserVariablesInUse(List<UserVariable> userVariables)
        {
        }

        public virtual void UpdateUserVariables(UserVariableSet userVariableSet)
        {
        }
    }

    public class BSParameterValueItemVariable : BSParameterValueItem
    {
        //public virtual void WriteToXml(System.Xml.XmlTextWriter writer)
        //{
        //}

        //public virtual Object Clone()
        //{
        //    return null;
        //}

        //public virtual bool IsEqual(Object obj)
        //{
        //    return false;
        //}
    }

    public class BSParameterValueItemUserVariable : BSParameterValueItemVariable
    {
        private UserVariable _userVariable = new UserVariable();
        public UserVariable UserVariable
        {
            get { return _userVariable; }
            set { _userVariable = value; }
        }

        public override object Clone()
        {
            BSParameterValueItemUserVariable bsParameterValueItemUserVariable = new BSParameterValueItemUserVariable();
            bsParameterValueItemUserVariable.UserVariable = (UserVariable)this.UserVariable.Clone();
            return bsParameterValueItemUserVariable;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            BSParameterValueItemUserVariable bsParameterValueItemUserVariable = (BSParameterValueItemUserVariable)obj;

            return (bsParameterValueItemUserVariable.UserVariable.IsEqual(this.UserVariable));
        }

        public override string ToString()
        {
            return String.Format("$${0}$$",_userVariable.Name);
        }

        public static BSParameterValueItemUserVariable ReadXml(XmlReader reader)
        {
            BSParameterValueItemUserVariable bsParameterValueItemUserVariable = new BSParameterValueItemUserVariable();

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "userVariable":
                        bsParameterValueItemUserVariable.UserVariable = UserVariable.ReadXml(reader);
                        break;
                }
            }

            return bsParameterValueItemUserVariable;
        }

        public override void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("parameterValueItemUserVariable");
            _userVariable.WriteToXml(writer);
            writer.WriteEndElement(); // parameterValueItemUserVariable
        }

        public override void GetUserVariablesInUse(List<UserVariable> userVariables)
        {
            userVariables.Add(_userVariable);
        }

        public override void UpdateUserVariables(UserVariableSet userVariableSet)
        {
            _userVariable.Name = userVariableSet.UpdateUserVariableName(_userVariable.Name);
        }
    }

    public class BSParameterValueItemMediaCounterVariable : BSParameterValueItemVariable
    {
        public string FileName { get; set; }

        public override object Clone()
        {
            BSParameterValueItemMediaCounterVariable bsParameterValueItemMediaCounterVariable = new BSParameterValueItemMediaCounterVariable();
            bsParameterValueItemMediaCounterVariable.FileName = this.FileName;
            return bsParameterValueItemMediaCounterVariable;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            BSParameterValueItemMediaCounterVariable bsParameterValueItemMediaCounterVariable = (BSParameterValueItemMediaCounterVariable)obj;
            if (bsParameterValueItemMediaCounterVariable.FileName != this.FileName) return false;
            return true;
        }

        public override string ToString()
        {
            return String.Format("Counter: {0}", FileName);
        }

        public override void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("parameterValueItemMediaCounterVariable");
            writer.WriteElementString("fileName", FileName);
            writer.WriteEndElement(); // textItemSpec
        }

        public static BSParameterValueItemMediaCounterVariable ReadXml(XmlReader reader)
        {
            string fileName = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "fileName":
                        fileName = reader.ReadString();
                        break;
                }
            }

            BSParameterValueItemMediaCounterVariable bsParameterValueItemMediaCounterVariable = new BSParameterValueItemMediaCounterVariable { FileName = fileName };
            return bsParameterValueItemMediaCounterVariable;
        }
    }


    public class BSParameterValueItemText : BSParameterValueItem
    {
        private string _value = String.Empty;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override object Clone()
        {
            BSParameterValueItemText bsParameterValueItemText = new BSParameterValueItemText();
            bsParameterValueItemText.Value = this.Value;
            return bsParameterValueItemText;
        }

        public override bool IsEqual(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            BSParameterValueItemText bsParameterValueItemText = (BSParameterValueItemText)obj;

            return (bsParameterValueItemText.Value.ToString() == this.Value.ToString());
        }

        public override string ToString()
        {
            return Value;
        }

        public static BSParameterValueItemText ReadXml(XmlReader reader)
        {
            string value = String.Empty;

            while (reader.Read() && (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Whitespace))
            {
                switch (reader.LocalName)
                {
                    case "value":
                        value = reader.ReadString();
                        break;
                }
            }

            BSParameterValueItemText bsParameterValueItemText = new BSParameterValueItemText();
            bsParameterValueItemText.Value = value;

            return bsParameterValueItemText;
        }


        public override void WriteToXml(XmlTextWriter writer)
        {
            writer.WriteStartElement("parameterValueItemText");
            writer.WriteElementString("value", Value);
            writer.WriteEndElement(); // parameterValueItemText
        }
    }
}
