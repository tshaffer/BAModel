using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
//using System.Windows.Controls;
using System.IO;
using System.Diagnostics;

namespace BAModel
{
    public class BrightSignCmdMgr
    {
        // logical bits indicating what, if any, parameter validation should be performed
        public const int ValidateCommandNotEmpty = 1;
        public const int ValidateCommandGPIONumber = 2;
        public const int ValidateCommandSingleByte = 4;
        public const int ValidateNonNegativeInteger = 8;
        public const int ValidateNoValidationRequired = 16;
        public const int ValidateCommandByteStream = 32;
        public const int ValidateIRRemoteOut = 64;
        public const int ValidateVariable = 128;

        private static List<BrightSignCmd> commandSetList = new List<BrightSignCmd>();
        public static List<BrightSignCmd> CommandSetList
        {
            get { return commandSetList; }
        }

        private static Dictionary<string, BrightSignCmd> _commandSetByName = new Dictionary<string, BrightSignCmd>();
        public static Dictionary<string, BrightSignCmd> CommandSetByName
        {
            get { return _commandSetByName; }
        }

        private static List<BrightSignCommandMenuEntry> _brightSignCommandMenuEntry = new List<BrightSignCommandMenuEntry>();
        public static List<BrightSignCommandMenuEntry> BrightSignCommandMenuEntryList
        {
            get { return _brightSignCommandMenuEntry; }
        }

        private static Dictionary<string, BrightSignCommandMenuEntry> _brightSignCommandMenuEntriesFromBrightSignCmd = new Dictionary<string, BrightSignCommandMenuEntry>();
        public static Dictionary<string, BrightSignCommandMenuEntry> BrightSignCommandMenuEntriesFromBrightSignCmd
        {
            get { return _brightSignCommandMenuEntriesFromBrightSignCmd; }
        }

        private static void ParseCommandGroups(XmlDocument doc)
        {
            XmlElement docElement = doc.DocumentElement;
            if (docElement.Name != "BrightAuthorCommandGroups")
            {
                Trace.WriteLine("Missing BrightAuthorCommandGroups docElement name");
                return;
            }

            if (!docElement.HasAttributes)
            {
                Trace.WriteLine("docElement has no attributes");
                return;
            }

            if (docElement.Attributes[0].Name != "Version")
            {
                Trace.WriteLine("missing Version attribute");
                return;
            }

            XmlNodeList childNodes = docElement.ChildNodes;
            foreach (XmlNode childNode in childNodes)
            {
                if (childNode.Name == "CommandGroup")
                {
                    BrightSignCommandGroup brightSignCommandGroup = new BrightSignCommandGroup();
                    brightSignCommandGroup.Name = childNode.Attributes[0].Value;

                    foreach (XmlNode node in childNode.ChildNodes)
                    {
                        switch (node.Name)
                        {
                            case "label":
                                brightSignCommandGroup.Label = node.InnerText;
                                string translatedLabel = BrightAuthorUtils.TryGetLocalizedString(brightSignCommandGroup.Label);
                                if (translatedLabel != String.Empty)
                                {
                                    brightSignCommandGroup.Label = translatedLabel;
                                }
                                break;
                            case "requiredFeatures":
                                XmlElement requiredFeaturesElement = node as XmlElement;
                                XmlNodeList features = requiredFeaturesElement.GetElementsByTagName("feature");

                                foreach (XmlElement feature in features)
                                {
                                    string featureName = feature.InnerText;
                                    BrightSignModel.ModelFeature modelFeature = BrightSignModel.GetModelFeature(featureName);
                                    brightSignCommandGroup.RequiredFeatures.Add(modelFeature);
                                }
                                break;
                            case "commandSets":
                                XmlElement commandSetsElement = node as XmlElement;
                                XmlNodeList commandSets = commandSetsElement.GetElementsByTagName("commandSet");

                                foreach (XmlElement commandSet in commandSets)
                                {
                                    string commandSetName = commandSet.Attributes[0].Value;
                                    if (_commandSetByName.ContainsKey(commandSetName))
                                    {
                                        BrightSignCmd brightSignCmd = _commandSetByName[commandSetName];
                                        brightSignCommandGroup.BrightSignCommandSets.Add(brightSignCmd);
                                        BrightSignCommandMenuEntriesFromBrightSignCmd.Add(commandSetName, brightSignCommandGroup);
                                    }
                                }
                                break;
                        }
                    }

                    BrightSignCommandMenuEntryList.Add(brightSignCommandGroup);
                }
                else if (childNode.Name == "CommandSet")
                {
                    BrightSignCommandSet brightSignCommandSet = new BrightSignCommandSet();
                    brightSignCommandSet.Name = childNode.Attributes[0].Value;

                    if (_commandSetByName.ContainsKey(brightSignCommandSet.Name))
                    {
                        brightSignCommandSet.BrightSignCmd = _commandSetByName[brightSignCommandSet.Name];

                        foreach (XmlNode node in childNode.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "label":
                                    brightSignCommandSet.Label = node.InnerText;
                                    string translatedLabel = BrightAuthorUtils.TryGetLocalizedString(brightSignCommandSet.Label);
                                    if (translatedLabel != String.Empty)
                                    {
                                        brightSignCommandSet.Label = translatedLabel;
                                    }
                                    break;
                                case "requiredFeatures":
                                    XmlElement requiredFeaturesElement = node as XmlElement;
                                    XmlNodeList features = requiredFeaturesElement.GetElementsByTagName("feature");

                                    foreach (XmlElement feature in features)
                                    {
                                        string featureName = feature.InnerText;
                                        BrightSignModel.ModelFeature modelFeature = BrightSignModel.GetModelFeature(featureName);
                                        brightSignCommandSet.RequiredFeatures.Add(modelFeature);
                                    }
                                    break;
                            }
                        }

                        BrightSignCommandMenuEntryList.Add(brightSignCommandSet);
                        BrightSignCommandMenuEntriesFromBrightSignCmd.Add(brightSignCommandSet.Name, brightSignCommandSet);

                    }
                }
            }
        }

        private static void InitializeCommandGroups()
        {
            XmlDocument doc = new XmlDocument();

            Uri newUri = new Uri("templates/BrightSignCommandGroups.xml", UriKind.Relative);
            string brightSignCommandGroupsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, newUri.ToString());
            
            try
            {
                doc.Load(brightSignCommandGroupsPath);
            }
            catch (Exception)
            {
            }

            ParseCommandGroups(doc);
        }

        private static void ParseCommands(XmlDocument doc)
        {
            XmlElement docElement = doc.DocumentElement;
            if (docElement.Name != "BrightAuthorCommands")
            {
                Trace.WriteLine("Missing BrightAuthorCommands docElement name");
                return;
            }

            if (!docElement.HasAttributes)
            {
                Trace.WriteLine("docElement has no attributes");
                return;
            }

            if (docElement.Attributes[0].Name != "Version")
            {
                Trace.WriteLine("missing Version attribute");
                return;
            }

            if (docElement.Attributes.Count > 1)
            {
                if (docElement.Attributes[1].Name == "Mode")
                {
                    if (docElement.Attributes[1].Value.ToLower() == "replace")
                    {
                        commandSetList.Clear();
                    }
                }
            }

            XmlNodeList commandSets = doc.GetElementsByTagName("CommandSet");

            foreach (XmlElement commandSetItem in commandSets)
            {
                BrightSignCmd commandSet = new BrightSignCmd();

                // check existence first
                commandSet.Name = commandSetItem.Attributes[0].Value;

                commandSet.CustomUI = false;
                XmlNode customUINode = commandSetItem.Attributes.GetNamedItem("CustomUI");
                if (customUINode != null && customUINode.Value.ToLower() == "true")
                {
                    commandSet.CustomUI = true;
                }

                commandSet.StateEntrySupported = true;
                XmlNode stateEntrySupportedNode = commandSetItem.Attributes.GetNamedItem("StateEntrySupported");
                if (stateEntrySupportedNode != null && stateEntrySupportedNode.Value.ToLower() == "false")
                {
                    commandSet.StateEntrySupported = false;
                }

                commandSet.DataEntered = false;

                XmlNodeList requiredFeatures = commandSetItem.GetElementsByTagName("requiredFeatures");
                if (requiredFeatures.Count == 1)
                {
                    XmlElement requiredFeaturesElement = requiredFeatures[0] as XmlElement;
                    XmlNodeList features = requiredFeaturesElement.GetElementsByTagName("feature");

                    foreach (XmlElement feature in features)
                    {
                        string featureName = feature.InnerText;
                        BrightSignModel.ModelFeature modelFeature = BrightSignModel.GetModelFeature(featureName);
                        commandSet.RequiredFeatures.Add(modelFeature);
                    }
                }

                XmlNodeList uiList = commandSetItem.GetElementsByTagName("UI");
                //if (uiList.Count != 1)
                //{
                //    Trace.WriteLine("uiList.Count != 1");
                //    continue;
                //}

                if (uiList.Count == 1)
                {
                    XmlElement uiElement = uiList[0] as XmlElement;
                    if (uiElement.Attributes.Count == 1)
                    {
                        commandSet.Label = uiElement.Attributes[0].Value;
                        string translatedLabel = BrightAuthorUtils.TryGetLocalizedString(commandSet.Label);
                        if (translatedLabel != String.Empty)
                        {
                            commandSet.Label = translatedLabel;
                        }
                    }
                    else
                    {
                        Trace.WriteLine("ui label not present");
                        continue;
                    }

                    XmlElement uiElements = uiElement.FirstChild as XmlElement;

                    if (uiElements != null && uiElements.Name == "UIElements")
                    {
                        XmlNodeList uiElementItems = uiElements.ChildNodes;

                        foreach (XmlElement uiControl in uiElementItems)
                        {
                            BSUIElement bsUIElement = null;

                            string controlType = uiControl.Name;

                            switch (controlType)
                            {
                                case "TextBox":
                                    bsUIElement = new BSTextBox { Default = String.Empty, MaxWidth = "150" };
                                    break;
                                case "TextBlock":
                                    bsUIElement = new BSTextBlock();
                                    break;
                                case "ComboBox":
                                    bsUIElement = new BSComboBox();
                                    break;
                                case "CheckBoxComboBox":
                                    bsUIElement = new BSCheckBoxComboBox();
                                    break;
                                default:
                                    break;
                            }

                            XmlNodeList controlAttributeList = uiControl.ChildNodes;

                            foreach (XmlElement controlAttribute in controlAttributeList)
                            {
                                string attributeType = controlAttribute.Name;

                                switch (attributeType)
                                {
                                    case "Name":
                                        string controlName = controlAttribute.InnerText;
                                        bsUIElement.Name = controlName;
                                        break;
                                    case "MinWidth":
                                        string minWidth = controlAttribute.InnerText;
                                        bsUIElement.MinWidth = minWidth;
                                        break;
                                    case "MaxWidth":
                                        if (bsUIElement is BSTextBox)
                                        {
                                            string maxWidth = controlAttribute.InnerText;
                                            (bsUIElement as BSTextBox).MaxWidth = maxWidth;
                                        }
                                        break;
                                    case "Text":
                                        string text = controlAttribute.InnerText;
                                        if (bsUIElement is BSTextBlock)
                                        {
                                            BSTextBlock bsTextBlock = bsUIElement as BSTextBlock;
                                            bsTextBlock.Text = text;
                                            string translatedTextBlock = BrightAuthorUtils.TryGetLocalizedString(text);
                                            if (translatedTextBlock != String.Empty)
                                            {
                                                bsTextBlock.Text = translatedTextBlock;
                                            }
                                        }
                                        break;
                                    case "Margin":
                                        string marginLeft = "0";
                                        string marginTop = "0";
                                        string marginRight = "0";
                                        string marginBottom = "0";
                                        XmlNodeList marginValueList = controlAttribute.ChildNodes;
                                        foreach (XmlElement marginValue in marginValueList)
                                        {
                                            switch (marginValue.Name)
                                            {
                                                case "Left":
                                                    marginLeft = marginValue.InnerText;
                                                    break;
                                                case "Top":
                                                    marginTop = marginValue.InnerText;
                                                    break;
                                                case "Right":
                                                    marginRight = marginValue.InnerText;
                                                    break;
                                                case "Bottom":
                                                    marginBottom = marginValue.InnerText;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        BSMargin bsMargin = new BSMargin();
                                        bsMargin.Left = marginLeft;
                                        bsMargin.Top = marginTop;
                                        bsMargin.Right = marginRight;
                                        bsMargin.Bottom = marginBottom;
                                        bsUIElement.Margin = bsMargin;
                                        break;
                                    case "Item":
                                        if (bsUIElement is BSComboBox)
                                        {
                                            BSComboBox bsComboBox = bsUIElement as BSComboBox;
                                            List<BSComboBoxItem> bsComboBoxItems = bsComboBox.Items;

                                            string itemLabel = String.Empty;
                                            string parameterName = String.Empty;
                                            string parameterValue = String.Empty;
                                            if (controlAttribute.Attributes.Count == 1)
                                            {
                                                itemLabel = controlAttribute.Attributes[0].Value;
                                                string translatedItemLabel = BrightAuthorUtils.TryGetLocalizedString(itemLabel);
                                                if (translatedItemLabel != String.Empty)
                                                {
                                                    itemLabel = translatedItemLabel;
                                                }
                                                BSComboBoxItem bscbi = new BSComboBoxItem();
                                                bscbi.Label = itemLabel;

                                                List<BSComboBoxItemParameter> bscbipList = bscbi.ParameterItems;

                                                XmlNodeList itemParameters = controlAttribute.ChildNodes;
                                                foreach (XmlElement itemParameter in itemParameters)
                                                {
                                                    if (itemParameter.Name == "Parameter")
                                                    {
                                                        XmlNode itemParameterNode = itemParameter as XmlNode;
                                                        foreach (XmlElement itemParameterNodeData in itemParameterNode)
                                                        {
                                                            switch (itemParameterNodeData.Name)
                                                            {
                                                                case "Name":
                                                                    parameterName = itemParameterNodeData.InnerText;
                                                                    break;
                                                                case "Value":
                                                                    parameterValue = itemParameterNodeData.InnerText;
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                        BSComboBoxItemParameter bscbip = new BSComboBoxItemParameter
                                                        {
                                                            Name = parameterName,
                                                            Value = parameterValue
                                                        };
                                                        bscbipList.Add(bscbip);
                                                    }
                                                }
                                                bsComboBoxItems.Add(bscbi);
                                            }
                                        }
                                        else if (bsUIElement is BSCheckBoxComboBox)
                                        {
                                            BSCheckBoxComboBox bsComboBox = bsUIElement as BSCheckBoxComboBox;
                                            List<BSCheckBoxComboBoxItem> bsComboBoxItems = bsComboBox.Items;

                                            string itemLabel = String.Empty;
                                            string parameterName = String.Empty;
                                            string parameterValue = String.Empty;
                                            if (controlAttribute.Attributes.Count == 1)
                                            {
                                                itemLabel = controlAttribute.Attributes[0].Value;
                                                string translatedItemLabel = BrightAuthorUtils.TryGetLocalizedString(itemLabel);
                                                if (translatedItemLabel != String.Empty)
                                                {
                                                    itemLabel = translatedItemLabel;
                                                }
                                                BSCheckBoxComboBoxItem bscbi = new BSCheckBoxComboBoxItem();
                                                bscbi.Label = itemLabel;

                                                // List<BSCheckBoxComboBoxItemParameter> bscbipList = bscbi.ParameterItems;

                                                XmlNodeList itemParameters = controlAttribute.ChildNodes;
                                                if (itemParameters.Count != 1)
                                                {
                                                    Trace.WriteLine("only one parameter permitted per item in a CheckBoxComboBox");
                                                    continue;
                                                }
                                                XmlElement itemParameter = (XmlElement)itemParameters[0];

                                                if (itemParameter.Name == "Parameter")
                                                {
                                                    XmlNode itemParameterNode = itemParameter as XmlNode;
                                                    foreach (XmlElement itemParameterNodeData in itemParameterNode)
                                                    {
                                                        switch (itemParameterNodeData.Name)
                                                        {
                                                            case "Name":
                                                                parameterName = itemParameterNodeData.InnerText;
                                                                break;
                                                            case "Value":
                                                                parameterValue = itemParameterNodeData.InnerText;
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                    }
                                                    BSCheckBoxComboBoxItemParameter bscbip = new BSCheckBoxComboBoxItemParameter
                                                    {
                                                        Name = parameterName,
                                                        Value = parameterValue
                                                    };
                                                    bscbi.ParameterItem = bscbip;
                                                    bsComboBoxItems.Add(bscbi);
                                                }
                                                else
                                                {
                                                    Trace.WriteLine("parameter not named Parameter");
                                                    continue;
                                                }
                                            }
                                        }
                                        break;
                                    case "Default":
                                        string defaultValue = controlAttribute.InnerText;
                                        if (bsUIElement is BSTextBox)
                                        {
                                            BSTextBox bsTextBox = bsUIElement as BSTextBox;
                                            bsTextBox.Default = defaultValue;
                                        }
                                        break;
                                    case "DefaultIndex":
                                        string defaultIndex = controlAttribute.InnerText;
                                        if (bsUIElement is BSComboBox)
                                        {
                                            BSComboBox bsComboBox = bsUIElement as BSComboBox;
                                            bsComboBox.DefaultIndex = defaultIndex;
                                        }
                                        break;
                                    case "Prompt":
                                        string prompt = controlAttribute.InnerText;
                                        if (bsUIElement is BSCheckBoxComboBox)
                                        {
                                            BSCheckBoxComboBox bsCheckBoxComboBox = bsUIElement as BSCheckBoxComboBox;
                                            string translatedPrompt = BrightAuthorUtils.TryGetLocalizedString(prompt);
                                            if (translatedPrompt != String.Empty)
                                            {
                                                prompt = translatedPrompt;
                                            }
                                            bsCheckBoxComboBox.Prompt = prompt;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            commandSet.UIElements.Add(bsUIElement);

                        }
                    }
                }

                XmlNodeList commandList = commandSetItem.GetElementsByTagName("Command");
                //if (commandList.Count < 1)
                //{
                //    Trace.WriteLine("commandList.Count < 1");
                //    continue;
                //}

                if (commandList.Count > 0)
                {
                    foreach (XmlElement commandItem in commandList)
                    {
                        BSCommand command = new BSCommand();

                        if (commandItem.Attributes.Count == 1)
                        {
                            string commandName = commandItem.Attributes[0].Value;
                            command.Name = commandName;
                        }
                        else
                        {
                            Trace.WriteLine("command name not present");
                            continue;
                        }

                        XmlNodeList parameterHeader = commandItem.ChildNodes;
                        if (parameterHeader.Count == 1 && parameterHeader[0].Name == "Parameters")
                        {
                            XmlElement parameters = parameterHeader[0] as XmlElement;

                            XmlNodeList parameterList = parameters.GetElementsByTagName("Parameter");

                            foreach (XmlElement parameter in parameterList)
                            {
                                BSParameter bsParameter = new BSParameter();

                                string parameterType = String.Empty;
                                string uiElementName = String.Empty;
                                string parameterName = String.Empty;
                                string value = String.Empty;
                                string itemName = String.Empty;
                                int validationRule = -1;

                                XmlNodeList parameterAttributes = parameter.ChildNodes;
                                foreach (XmlElement parameterAttribute in parameterAttributes)
                                {
                                    switch (parameterAttribute.Name)
                                    {
                                        case "Type":
                                            parameterType = parameterAttribute.InnerText;
                                            break;
                                        case "UIElementName":
                                            uiElementName = parameterAttribute.InnerText;
                                            break;
                                        case "ParameterName":
                                            parameterName = parameterAttribute.InnerText;
                                            break;
                                        case "Value":
                                            value = parameterAttribute.InnerText;
                                            break;
                                        case "ItemName":
                                            itemName = parameterAttribute.InnerText;
                                            break;
                                        case "ValidationRule":
                                            string validationRuleStr = parameterAttribute.InnerText;
                                            validationRule = GetValidationRule(validationRuleStr);
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                bsParameter.Type = parameterType;
                                bsParameter.UIElementName = uiElementName;
                                bsParameter.ParameterName = parameterName;
                                bsParameter.ParameterValue.SetTextValue(value);
                                bsParameter.ItemName = itemName;
                                bsParameter.ValidationRule = validationRule;

                                command.Parameters.Add(bsParameter.ParameterName, bsParameter);
                            }
                        }

                        commandSet.Commands.Add(command);

                    }
                }

                commandSetList.Add(commandSet);

                _commandSetByName.Add(commandSet.Name, commandSet);
            }
        }

        private static void InitializeCommandSets()
        {
            commandSetList.Clear();

            XmlDocument doc = new XmlDocument();

            // get the system commands
            //            System.IO.Stream s = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/BrightAuthor;component/Templates/BrightSignCommands.xml")).Stream;
            //            doc.Load(s);

            Uri newUri = new Uri("templates/BrightSignCommands.xml", UriKind.Relative);
            string brightSignCommandPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, newUri.ToString());

            try
            {
                doc.Load(brightSignCommandPath);
            }
            catch (Exception)
            {
            }

            ParseCommands(doc);

            // look for user installed commands
            try
            {
                string commandsPath = Path.Combine(System.Windows.Forms.Application.LocalUserAppDataPath, "commands");

                if (Directory.Exists(commandsPath))
                {
                    string[] commandFiles = Directory.GetFiles(commandsPath);
                    foreach (string commandFile in commandFiles)
                    {
                        string extension = System.IO.Path.GetExtension(commandFile).ToLower();
                        if (extension == ".xml")
                        {
                            StreamReader sr = new StreamReader(commandFile);
                            doc = new XmlDocument();
                            doc.Load(sr);
                            ParseCommands(doc);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                App.myTraceListener.Assert(false, ex.ToString());
            }
        }

        public static void Initialize()
        {
            InitializeCommandSets();

            InitializeCommandGroups();
        }

        public static int GetValidationRule(string validationRuleStr)
        {
            switch (validationRuleStr)
            {
                case "ValidateNonNegativeInteger":
                    return ValidateNonNegativeInteger;
                case "ValidateCommandSingleByte":
                    return ValidateCommandSingleByte;
                case "ValidateCommandByteStream":
                    return ValidateCommandByteStream;
                case "ValidateIRRemoteOut":
                    return ValidateIRRemoteOut;
                case "ValidateVariable":
                    return ValidateVariable;
                default:
                    return -1;
            }
        }

        private static bool CommandParametersMatch(string uiElementName, BSComboBoxItemParameter parameterItem, BrightSignCmd BrightSignCmd)
        {
            // look for the match in one of the BrightSignCmd parameters
            List<BSCommand> commands = BrightSignCmd.Commands;
            foreach (BSCommand command in commands)
            {
                foreach (KeyValuePair<string, BSParameter> kvp in command.Parameters)
                {
                    BSParameter parameter = kvp.Value;
                    if (parameter.UIElementName == uiElementName)
                    {
                        string parameterValue = parameter.ParameterValue.GetValue();
                        if ((parameterItem.Name == parameter.ItemName) && (parameterItem.Value == parameterValue))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

//        public static void SetCommandParameter(StackPanel spCommandParameters, string elementName, string elementValue)
//        {
//            foreach (FrameworkElement frameworkElement in spCommandParameters.Children)
//            {
//                if (frameworkElement.Name == elementName)
//                {
//                    switch (frameworkElement.GetType().Name)
//                    {
//                        case "TextBox":
//                            TextBox tb = frameworkElement as TextBox;
//                            tb.Text = elementValue;
//                            break;
//                    }
//                }
//            }
//        }

//        public static void UpdateCommandParameters(StackPanel spCommandParameters, BrightSignCmd brightSignCmd, bool clearUI)
//        {
//            TextBlock tbl = null;
//            TextBox tb = null;
//
//            if (clearUI) spCommandParameters.Children.Clear();
//
//            // find the appropriate commandSet based on the brightSignCmd
//            BrightSignCmd commandSet = null;
//            foreach (BrightSignCmd bsc in commandSetList)
//            {
//                if (bsc.Name == brightSignCmd.Name)
//                {
//                    commandSet = bsc;
//                    break;
//                }
//            }
//            if (commandSet == null) return;
//
//            //BrightSignCmd commandSet = commandSetList[commandIndex];
//            List<BSUIElement> uiElements = commandSet.UIElements;
//            foreach (BSUIElement uiElement in uiElements)
//            {
//                if (uiElement is BSTextBlock)
//                {
//                    BSTextBlock bsTextBlock = uiElement as BSTextBlock;
//                    tbl = new TextBlock();
//                    tbl.Name = bsTextBlock.Name;
//                    tbl.Text = bsTextBlock.Text;
//                    tbl.VerticalAlignment = VerticalAlignment.Center;
//                    tbl.Margin = bsTextBlock.Margin.GetMarginValues();
//                    spCommandParameters.Children.Add(tbl);
//                }
//                else if (uiElement is BSCheckBoxComboBox)
//                {
//                    BSCheckBoxComboBox bsCheckBoxComboBox = uiElement as BSCheckBoxComboBox;
//                    ComboBox cb = new ComboBox();
//                    cb.Name = bsCheckBoxComboBox.Name;
//                    cb.SelectedIndex = 0;
//                    cb.IsDropDownOpen = false;
//                    cb.Margin = bsCheckBoxComboBox.Margin.GetMarginValues();
//
//                    List<BSCheckBoxComboBoxItem> items = bsCheckBoxComboBox.Items;
//
//                    // identify this as a CheckBoxComboBox
//                    cb.Tag = "CheckBoxComboBox";
//
//                    // top line is the prompt
//                    ComboBoxItem cbi = new ComboBoxItem();
//                    cbi.Content = bsCheckBoxComboBox.Prompt;
//                    cb.Items.Add(cbi);
//
//                    foreach (BSCheckBoxComboBoxItem item in items)
//                    {
//                        StackPanel sp = new StackPanel();
//                        sp.Orientation = Orientation.Horizontal;
//                        CheckBox checkBox = new CheckBox();
//                        checkBox.VerticalAlignment = VerticalAlignment.Center;
//                        checkBox.Margin = new Thickness(0, 0, 6, 0);
//                        sp.Children.Add(checkBox);
//                        TextBlock tbLabel = new TextBlock();
//                        tbLabel.Text = item.Label;
//                        sp.Children.Add(tbLabel);
//                        cbi = new ComboBoxItem();
//                        cbi.Tag = item.ParameterItem;
//                        cbi.Content = sp;
//                        cb.Items.Add(cbi);
//                    }
//                    spCommandParameters.Children.Add(cb);
//
//                    if (brightSignCmd.DataEntered)
//                    {
//                        // Current CheckBoxComboBox rules:
//                        // There can be multiple commands in the BrightSignCmd, however each command must reference the CheckBoxComboBox (see below where only the first command
//                        // is grabbed). Within that command, the correct parameter will be found and matched with the CheckBoxComboBox bit fields.
//                        // The code will need to be enhanced if a command set is required that includes commands where some use the CheckBoxComboBox and others are either fixed or
//                        // use other UIElements.
//                        // Additional restriction - each item within a CheckBoxComboBox can only have a single parameter. This could be enforced in code with some work.
//
//                        BSParameter parameter = null;
//                        BSCommand bsCommand = brightSignCmd.Commands[0];
//                        foreach (KeyValuePair<string, BSParameter> kvp in bsCommand.Parameters)
//                        {
//                            parameter = kvp.Value;
//                            if (parameter.UIElementName == uiElement.Name) break;
//                        }
//                        string compositeValueStr = parameter.ParameterValue.GetValue();
//                        int compositeValue = Convert.ToInt32(compositeValueStr);
//
//                        int index = 1;
//                        foreach (BSCheckBoxComboBoxItem item in items)
//                        {
//                            BSCheckBoxComboBoxItemParameter uiParameter = item.ParameterItem;
//                            // check against the value
//                            if ((Convert.ToInt32(uiParameter.Value) & compositeValue) != 0)
//                            {
//                                cbi = (ComboBoxItem)cb.Items[index];
//                                if (cbi.Content is StackPanel)
//                                {
//                                    StackPanel sp = cbi.Content as StackPanel;
//                                    UIElementCollection elements = sp.Children;
//                                    if (elements[0] is CheckBox)
//                                    {
//                                        CheckBox checkBox = elements[0] as CheckBox;
//                                        checkBox.IsChecked = true;
//                                    }
//                                }
//                            }
//                            index++;
//                        }
//                    }
//                }
//                else if (uiElement is BSComboBox)
//                {
//                    BSComboBox bsComboBox = uiElement as BSComboBox;
//                    ComboBox cb = new ComboBox();
//                    cb.Name = bsComboBox.Name;
//                    cb.SelectedIndex = Convert.ToInt32(bsComboBox.DefaultIndex);
//                    cb.IsDropDownOpen = false;
//                    cb.Margin = bsComboBox.Margin.GetMarginValues();
//                    cb.VerticalAlignment = VerticalAlignment.Center;
//
//                    List<BSComboBoxItem> items = bsComboBox.Items;
//                    foreach (BSComboBoxItem item in items)
//                    {
//                        ComboBoxItem cbi = new ComboBoxItem();
//                        cbi.Tag = item.ParameterItems;
//                        cbi.Content = item.Label;
//                        cb.Items.Add(cbi);
//                    }
//                    spCommandParameters.Children.Add(cb);
//
//                    if (brightSignCmd.DataEntered)
//                    {
//                        // need to match the command parameters in BrightSignCmd with the parameters in one of the items in the comboBox
//                        bool matchFound = true;
//                        int selectedIndex = 0;
//                        foreach (BSComboBoxItem item in items)
//                        {
//                            // each parameter associated with this combo box item must match associated parameters in this BrightSignCmd
//                            // for each parameter in this combo box item
//                            // is there a match with a parameter in the command set?
//                            // if yes, continue with the next parameter associated with this combo box item
//                            // if not, go to the next combo box item
//                            List<BSComboBoxItemParameter> parameterItems = item.ParameterItems;
//                            foreach (BSComboBoxItemParameter parameterItem in parameterItems)
//                            {
//                                if (!CommandParametersMatch(uiElement.Name, parameterItem, brightSignCmd))
//                                {
//                                    matchFound = false;
//                                    break;
//                                }
//                                else
//                                {
//                                    // go onto the next parameterItem
//                                    matchFound = true;
//                                    continue;
//                                }
//                            }
//                            if (matchFound)
//                            {
//                                cb.SelectedIndex = selectedIndex;
//                                break;
//                            }
//                            selectedIndex++;
//                        }
//                    }
//                }
//                else if (uiElement is BSTextBox)
//                {
//                    BSTextBox bsTextBox = uiElement as BSTextBox;
//                    tb = new TextBox();
//                    tb.Name = bsTextBox.Name;
//                    tb.MinWidth = Convert.ToDouble(bsTextBox.MinWidth);
//                    tb.MaxWidth = Convert.ToDouble(bsTextBox.MaxWidth);
//                    tb.VerticalAlignment = VerticalAlignment.Center;
//                    tb.Text = bsTextBox.Default;
//                    spCommandParameters.Children.Add(tb);
//
//                    if (brightSignCmd.DataEntered)
//                    {
//                        List<BSCommand> commands = brightSignCmd.Commands;
//                        foreach (BSCommand command in commands)
//                        {
//                            foreach (KeyValuePair<string, BSParameter> kvp in command.Parameters)
//                            {
//                                BSParameter parameter = kvp.Value;
//                                if (parameter.UIElementName == uiElement.Name)
//                                {
//                                    tb.Text = parameter.ParameterValue.GetValue();
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public static string GetCommandSet(StackPanel spCommandParameters, BrightSignCmd commandSet, out BrightSignCmd selectedCommandSet)
//        {
//            selectedCommandSet = new BrightSignCmd { Name = commandSet.Name, DataEntered = true };
//
//            List<BSCommand> commands = commandSet.Commands;
//            foreach (BSCommand command in commands)
//            {
//                BSCommand selectedCommand = new BSCommand();
//
//                string commandName = command.Name;
//                selectedCommand.Name = commandName;
//
//                // Console.WriteLine("Command: " + commandName);
//
//                foreach (KeyValuePair<string, BSParameter> kvp in command.Parameters)
//                {
//                    BSParameter parameter = kvp.Value;
//
//                    BSParameter selectedParameter = new BSParameter();
//
//                    string parameterType = parameter.Type;
//                    string parameterName = parameter.ParameterName;
//                    string itemName = parameter.ItemName;
//
//                    // Console.WriteLine("Parameter name: " + parameterName);
//
//                    selectedParameter.ParameterName = parameterName;
//
//                    if (parameterType == "entry")
//                    {
//                        string uiElementName = parameter.UIElementName;
//                        selectedParameter.UIElementName = uiElementName;
//                        selectedParameter.ValidationRule = parameter.ValidationRule;
//
//                        DependencyObject depObj = LogicalTreeHelper.FindLogicalNode(spCommandParameters, uiElementName);
//
//                        string parameterValue = String.Empty;
//                        if (depObj is TextBox)
//                        {
//                            string errorMsg = String.Empty;
//
//                            parameterValue = ((TextBox)depObj).Text;
//                            // Console.WriteLine("Parameter value: " + parameterValue);
//
//                            errorMsg = selectedParameter.ParameterValue.SetValue(parameterValue);
//                            if (errorMsg != "") return errorMsg;
//
//                            errorMsg = selectedParameter.Validate();
//                            if (errorMsg != "") return errorMsg;
//                        }
//                        else if (depObj is ComboBox)
//                        {
//                            ComboBox cb = depObj as ComboBox;
//                            if (cb.Tag != null && cb.Tag is String && (string)cb.Tag == "CheckBoxComboBox")
//                            {
//                                int selectedValue = 0;
//
//                                int numItems = cb.Items.Count;
//                                for (int i = 1; i < numItems; i++)
//                                {
//                                    ComboBoxItem cbi = (ComboBoxItem)cb.Items[i];
//                                    if (cbi.Content is StackPanel)
//                                    {
//                                        StackPanel sp = cbi.Content as StackPanel;
//                                        UIElementCollection spChildren = sp.Children;
//                                        if (spChildren[0] is CheckBox)
//                                        {
//                                            CheckBox cbChecked = spChildren[0] as CheckBox;
//                                            if ((bool)cbChecked.IsChecked)
//                                            {
//                                                BSCheckBoxComboBoxItemParameter bscbcbip = (BSCheckBoxComboBoxItemParameter)cbi.Tag;
//                                                int itemValue = Convert.ToInt32(bscbcbip.Value);
//                                                selectedValue += itemValue;
//                                            }
//                                        }
//                                    }
//                                }
//                                selectedParameter.ParameterValue.SetTextValue(selectedValue.ToString());
//                                selectedParameter.ItemName = itemName;
//                            }
//                            else
//                            {
//                                int selectedIndex = cb.SelectedIndex;
//                                ComboBoxItem cbi = (ComboBoxItem)cb.Items[selectedIndex];
//                                List<BSComboBoxItemParameter> parameterItems = (List<BSComboBoxItemParameter>)cbi.Tag;
//
//                                foreach (BSComboBoxItemParameter parameterItem in parameterItems)
//                                {
//                                    if (parameterItem.Name == itemName)
//                                    {
//                                        // Console.WriteLine("Parameter value: " + parameterItem.Value);
//                                        selectedParameter.ParameterValue.SetTextValue(parameterItem.Value);
//                                        selectedParameter.ItemName = itemName;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        // Console.WriteLine("Parameter value: " + parameter.Value);
//                        selectedParameter.ParameterValue = parameter.ParameterValue.Clone();
//                    }
//
//                    selectedCommand.Parameters.Add(selectedParameter.ParameterName, selectedParameter);
//                }
//
//                selectedCommandSet.Commands.Add(selectedCommand);
//            }
//
//            return String.Empty;
//        }

//        public static string GetCommandSets(StackPanel spCommand, out BrightSignCmd selectedCommandSet)
//        {
//            BrightSignCmd commandSet = null;
//            selectedCommandSet = null;
//
//            ComboBox cbCommandNames = (ComboBox)LogicalTreeHelper.FindLogicalNode(spCommand, "cbCommandNames");
//            int selectedCommandIndex = cbCommandNames.SelectedIndex;
//            ComboBoxItem selectedCBI = (ComboBoxItem)cbCommandNames.Items[selectedCommandIndex];
//            BrightSignCmd brightSignCmd = (BrightSignCmd)selectedCBI.Tag;
//
//            List<BrightSignCmd> commandSetList = BrightSignCmdMgr.CommandSetList;
//            foreach (BrightSignCmd masterBrightSignCmd in commandSetList)
//            {
//                if (masterBrightSignCmd.Name == brightSignCmd.Name)
//                {
//                    commandSet = masterBrightSignCmd;
//                }
//            }
//            if (commandSet == null) return String.Empty;
//
//            selectedCommandSet = new BrightSignCmd { Name = commandSet.Name, DataEntered = true };
//
//            List<BSCommand> commands = commandSet.Commands;
//            foreach (BSCommand command in commands)
//            {
//                BSCommand selectedCommand = new BSCommand();
//
//                string commandName = command.Name;
//                selectedCommand.Name = commandName;
//
//                // Console.WriteLine("Command: " + commandName);
//
//                foreach (KeyValuePair<string, BSParameter> kvp in command.Parameters)
//                {
//                    BSParameter parameter = kvp.Value;
//
//                    BSParameter selectedParameter = new BSParameter();
//
//                    string parameterType = parameter.Type;
//                    string parameterName = parameter.ParameterName;
//                    string itemName = parameter.ItemName;
//
//                    // Console.WriteLine("Parameter name: " + parameterName);
//
//                    selectedParameter.ParameterName = parameterName;
//
//                    if (parameterType == "entry")
//                    {
//                        string uiElementName = parameter.UIElementName;
//                        selectedParameter.UIElementName = uiElementName;
//                        selectedParameter.ValidationRule = parameter.ValidationRule;
//
//                        StackPanel spCommandParameters = (StackPanel)LogicalTreeHelper.FindLogicalNode(spCommand, "spCommandParameters");
//
//                        DependencyObject depObj = LogicalTreeHelper.FindLogicalNode(spCommandParameters, uiElementName);
//
//                        string parameterValue = String.Empty;
//                        if (depObj is TextBox)
//                        {
//                            string errorMsg = String.Empty;
//
//                            parameterValue = ((TextBox)depObj).Text;
//                            // Console.WriteLine("Parameter value: " + parameterValue);
//
//                            errorMsg = selectedParameter.ParameterValue.SetValue(parameterValue);
//                            if (errorMsg != "") return errorMsg;
//
//                            errorMsg = selectedParameter.Validate();
//                            if (errorMsg != "") return errorMsg;
//                        }
//                        else if (depObj is ComboBox)
//                        {
//                            ComboBox cb = depObj as ComboBox;
//                            if (cb.Tag != null && cb.Tag is String && (string)cb.Tag == "CheckBoxComboBox")
//                            {
//                                int selectedValue = 0;
//
//                                int numItems = cb.Items.Count;
//                                for (int i = 1; i < numItems; i++)
//                                {
//                                    ComboBoxItem cbi = (ComboBoxItem)cb.Items[i];
//                                    if (cbi.Content is StackPanel)
//                                    {
//                                        StackPanel sp = cbi.Content as StackPanel;
//                                        UIElementCollection spChildren = sp.Children;
//                                        if (spChildren[0] is CheckBox)
//                                        {
//                                            CheckBox cbChecked = spChildren[0] as CheckBox;
//                                            if ((bool)cbChecked.IsChecked)
//                                            {
//                                                BSCheckBoxComboBoxItemParameter bscbcbip = (BSCheckBoxComboBoxItemParameter)cbi.Tag;
//                                                int itemValue = Convert.ToInt32(bscbcbip.Value);
//                                                selectedValue += itemValue;
//                                            }
//                                        }
//                                    }
//                                }
//                                selectedParameter.ParameterValue.SetTextValue(selectedValue.ToString());
//                                selectedParameter.ItemName = itemName;
//                            }
//                            else
//                            {
//                                int selectedIndex = cb.SelectedIndex;
//                                ComboBoxItem cbi = (ComboBoxItem)cb.Items[selectedIndex];
//                                List<BSComboBoxItemParameter> parameterItems = (List<BSComboBoxItemParameter>)cbi.Tag;
//
//                                foreach (BSComboBoxItemParameter parameterItem in parameterItems)
//                                {
//                                    if (parameterItem.Name == itemName)
//                                    {
//                                        // Console.WriteLine("Parameter value: " + parameterItem.Value);
//                                        selectedParameter.ParameterValue.SetTextValue(parameterItem.Value);
//                                        selectedParameter.ItemName = itemName;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else
//                    {
//                        // Console.WriteLine("Parameter value: " + parameter.Value);
//                        selectedParameter.ParameterValue = parameter.ParameterValue.Clone();
//                    }
//
//                    selectedCommand.Parameters.Add(selectedParameter.ParameterName, selectedParameter);
//                }
//
//                selectedCommandSet.Commands.Add(selectedCommand);
//            }
//
//            return String.Empty;
//        }

        //public static string GetBrightSignCmds(StackPanel spCommands, ref List<BrightSignCmd> selectedCommandSets)
        //{
        //    selectedCommandSets.Clear();

        //    foreach (StackPanel spCommand in spCommands.Children)
        //    {
        //        BrightSignCmd selectedCommandSet = null;
        //        string errorMsg = GetCommandSets(spCommand, out selectedCommandSet);
        //        if (errorMsg != String.Empty) return errorMsg;
        //        selectedCommandSets.Add(selectedCommandSet);
        //    }

        //    return String.Empty;
        //}

        public static bool IsCommandSupported(BrightSignCmd brightSignCmd, bool stateEntry)
        {
            foreach (BrightSignModel.ModelFeature requiredFeature in brightSignCmd.RequiredFeatures)
            {
                bool featureSupported = BrightSignModelMgr.CurrentModelSupportsFeature(requiredFeature);
                if (!featureSupported) return false;
            }

            if (stateEntry && !brightSignCmd.StateEntrySupported) return false;

            return true;
        }
    }
}
