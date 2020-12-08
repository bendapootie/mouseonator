using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

namespace dotNetUi
{
    public partial class Form1 : Form
    {
        [DllImport("MouseHookDLL.dll")] static extern void HK_StartHook();
        [DllImport("MouseHookDLL.dll")] static extern void HK_EndHook();

        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsIdle();
        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsCancled();
        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsRecording();
        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsPlayback();
        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsAutoClicking();
        [DllImport("MouseHookDLL.dll")] static extern Boolean HK_IsRecordingSaved();
        
        [DllImport("MouseHookDLL.dll")] static extern void HK_SetRecordingKey(UInt32 vkCode);
        [DllImport("MouseHookDLL.dll")] static extern void HK_SetPlaybackKey(UInt32 vkCode);
        [DllImport("MouseHookDLL.dll")] static extern void HK_SetCancelPlaybackKey(UInt32 vkCode);
        [DllImport("MouseHookDLL.dll")] static extern void HK_SetAutoClickKey(UInt32 vkCode);

        [DllImport("MouseHookDLL.dll")] static extern void HK_StartRecording();
        [DllImport("MouseHookDLL.dll")] static extern void HK_StopRecording();

        [DllImport("MouseHookDLL.dll")] static extern IntPtr HK_GetRecordingString();
        [DllImport("MouseHookDLL.dll")] static extern UInt32 HK_SetRecordingString(IntPtr stringToSet);

        [DllImport("MouseHookDLL.dll")] static extern void HK_StartPlayback();
        [DllImport("MouseHookDLL.dll")] static extern void HK_CancelPlayback();

        [DllImport("MouseHookDLL.dll")] static extern void HK_StartAutoClicking();
        [DllImport("MouseHookDLL.dll")] static extern void HK_StopAutoClicking();

        [DllImport("MouseHookDLL.dll")] static extern void HK_Update();
        
        const uint INPUT_MOUSE    = 0;
        const uint INPUT_KEYBOARD = 1;
        const uint INPUT_HARDWARE = 2;

        const uint VK_PAUSE = 0x13;

        const uint VK_ESCAPE = 0x1B;

        const uint VK_F1  = 0x70;
        const uint VK_F2  = 0x71;
        const uint VK_F3  = 0x72;
        const uint VK_F4  = 0x73;
        const uint VK_F5  = 0x74;
        const uint VK_F6  = 0x75;
        const uint VK_F7  = 0x76;
        const uint VK_F8  = 0x77;
        const uint VK_F9  = 0x78;
        const uint VK_F10 = 0x79;
        const uint VK_F11 = 0x7A;
        const uint VK_F12 = 0x7B;

        const uint VK_NUMLOCK = 0x90;
        const uint VK_SCROLL = 0x91;

        static string[] InputStringElementSeparators = { "\t", " ", "," };
        static string[] DefaultCandyImagesPath = {
                                                     //"SearchPattern.bmp",
                                                     //"SearchPattern_Pie.bmp",
                                                     //"SearchPattern_Sandwich.bmp"
                                                 };

        bool m_playingScript = false;
        DateTime m_scriptStartTime;
        int m_scriptNextLine = 0;
        DateTime m_scriptTimeToStopAutoClicking;
        bool? m_lastComparison = null;      // nullable boolean can be 'true', 'false', or 'null'
        Dictionary<string, string> m_scriptVariables = new Dictionary<string, string>();
        Dictionary<string, int> m_labelLineNumbers = new Dictionary<string, int>();
        ScriptOutput m_scriptOutput = new ScriptOutput();
        DateTime m_lastScriptOutputUpdateTime;

        List<Bitmap> m_candyImages = new List<Bitmap>();
        DateTime m_nextAutoCandyClickTime;

        class ScriptOutput
        {
            public string String { get { return m_output; } }
            public bool IsDirty() { return m_dirty; }
            public void ClearDirtyFlag() { m_dirty = false; }

            public void Clear()
            {
                m_output = "";
                m_dirty = true;
            }

            public void AppendLine(string line)
            {
                m_output += line + "\r\n";
                m_dirty = true;
            }

            private string m_output = "";
            private bool m_dirty = true;
        }

        class MouseInput
        {
            uint time;
            uint dwFlags;
            uint mouseData;
            int dx;
            int dy;

            public string GetString()
            {
                object[] parms = { INPUT_MOUSE, time, dwFlags, mouseData, dx, dy };
                string result = string.Format("{0} {1} 0x{2:X2} 0x{3:X2} {4} {5}", parms);
                return result;
            }

            public void Initialize(int[] entries)
            {
                if (entries.Length >= 6)
                {
                    // entries[0] was INPUT_MOUSE
                    time = (uint)entries[1];
                    dwFlags = (uint)entries[2];
                    mouseData = (uint)entries[3];
                    dx = (int)entries[4];
                    dy = (int)entries[5];
                }
                else
                {
                    throw new Exception("Badly formed entry");
                }
            }
        }

        class KeyboardInput
        {
            uint time;
            uint dwFlags;
            uint wVk;

            public string GetString()
            {
                object[] parms = { INPUT_KEYBOARD, time, dwFlags, wVk };
                string result = string.Format("{0} {1} 0x{2:X2} 0x{3:X2} ", parms);
                return result;
            }

            public void Initialize(int[] entries)
            {
                if (entries.Length >= 4)
                {
                    // entries[0] was INPUT_KEYBOARD
                    time = (uint)entries[1];
                    dwFlags = (uint)entries[2];
                    wVk = (uint)entries[3];
                }
                else
                {
                    throw new Exception("Badly formed entry");
                }
            }
        }

        class InputEvent
        {
            MouseInput mi = null;
            KeyboardInput ki = null;

            public string GetString()
            {
                if (mi != null)
                {
                    return mi.GetString();
                }
                else if (ki != null)
                {
                    return ki.GetString();
                }
                else
                {
                    return "---- bad entry ----";
                }
            }

            bool TryParseIntHex(string s, out int result)
            {
                if (s.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                {
                    return int.TryParse(s.Substring("0x".Length), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out result);
                }
                else
                {
                    return int.TryParse(s, out result);
                }
            }

            public void InitializeFromRawString(string raw)
            {
                mi = null;
                ki = null;
                string[] strings = raw.Split(Form1.InputStringElementSeparators, StringSplitOptions.None);
                int[] entries = new int[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    int val;
                    if (!TryParseIntHex(strings[i], out val))
                    {
                        val = 0;
                    }
                    entries[i] = val;
                }
                if (entries.Length > 0)
                {
                    switch ((uint)entries[0])
                    {
                        case Form1.INPUT_MOUSE:
                            mi = new MouseInput();
                            mi.Initialize(entries);
                            break;
                        case Form1.INPUT_KEYBOARD:
                            ki = new KeyboardInput();
                            ki.Initialize(entries);
                            break;
                    }
                }
            }
        }


        private bool m_wasRecording = false;

        public Form1()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            LoadCandyImageFiles(DefaultCandyImagesPath);

            // Start 60hz update timer
            Timer updateTimer = new Timer();
            updateTimer.Interval = (1000 / 60) - 1; // ~60hz
            updateTimer.Tick += TimerUpdate;
            updateTimer.Enabled = true;
            updateTimer.Start();

            HK_StartHook();
            HK_SetRecordingKey(VK_F12);
            HK_SetPlaybackKey(VK_F11);
            HK_SetCancelPlaybackKey(VK_PAUSE);
            HK_SetAutoClickKey(VK_SCROLL);
        }

        private string GetRecordingString()
        {
            IntPtr rawPointer = HK_GetRecordingString();
            string returnString = Marshal.PtrToStringAnsi(rawPointer);
            return returnString;
        }

        private int SetRecordingString(string s)
        {
            IntPtr nativeAnsiString = Marshal.StringToHGlobalAnsi(s);
            UInt32 numInputsProcessed = HK_SetRecordingString(nativeAnsiString);
            Marshal.FreeHGlobal(nativeAnsiString);
            return (int)numInputsProcessed;
        }

        void CancelPlayingScript()
        {
            HK_CancelPlayback();

            m_playingScript = false;
            m_scriptNextLine = 0;
        }

        void LoadRecordingFromFile(string filename)
        {
            string contents = System.IO.File.ReadAllText(filename);
            UpdateRecordingDisplayFromString(contents);

            string recordingText = RecordingText.Text;
            int numInputsProcessed = SetRecordingString(recordingText);
        }

        void LoadRecordingFromString(string contents)
        {
            UpdateRecordingDisplayFromString(contents);

            string recordingText = RecordingText.Text;
            int numInputsProcessed = SetRecordingString(recordingText);
        }

        private void UpdateConstants()
        {
            // Const_Now
            string secondsSinceScriptStarted = string.Format("{0:0.00}", (DateTime.Now - m_scriptStartTime).TotalSeconds);
            SetScriptVariable("Const_Now", secondsSinceScriptStarted);

            // Const_NextLine
            SetScriptVariable("Const_NextLine", m_scriptNextLine.ToString());
        }

        private string[] GetArgsFromScriptCommand(string rawCommandString)
        {
            string[] args = rawCommandString.Split("/".ToCharArray());

            char VariableStartDesignator = '%';
            char VariableEndDesignator = '%';

            // Replace variables with their values
            for (int i = 0; i < args.Length; i++)
            {
                bool lookingForMore = true;
                while (lookingForMore)
                {
                    lookingForMore = false;
                    int startIndex = args[i].IndexOf(VariableStartDesignator);
                    if (startIndex >= 0)
                    {
                        int endIndex = args[i].IndexOf(VariableEndDesignator, startIndex + 1);
                        if (endIndex > startIndex)
                        {
                            string variableName = args[i].Substring(startIndex + 1, endIndex - (startIndex + 1));
                            if (m_scriptVariables.ContainsKey(variableName))
                            {
                                string valueString = m_scriptVariables[variableName];
                                string newString = args[i].Substring(0, startIndex) + valueString + args[i].Substring(endIndex + 1);
                                args[i] = newString;
                                // If the variable was sucessfully replaced, keep looking for more
                                lookingForMore = true;
                            }
                        }
                    }
                }
            }

            return args;
        }

        private void RebuildLabelLineNumbers()
        {
            m_labelLineNumbers.Clear();

            for (int i = 0; i < ScriptTextBox.Lines.Length; i++)
            {
                string[] args = GetArgsFromScriptCommand(ScriptTextBox.Lines[i]);
                if (args.Length >= 2)
                {
                    if (args[0].ToLower() == "label")
                    {
                        if (m_labelLineNumbers.ContainsKey(args[1]))
                        {
                            m_labelLineNumbers[args[1]] = i;
                        }
                        else
                        {
                            m_labelLineNumbers.Add(args[1], i);
                        }
                    }
                }
            }
        }

        private void StartPlayingScript()
        {
            RebuildLabelLineNumbers();

            m_playingScript = true;
            m_scriptStartTime = DateTime.Now;
            m_scriptNextLine = 0;
        }

        bool GetScriptDouble(string paramString, out double param)
        {
            bool success = false;
            double result = 0.0;
            string trimmedString = paramString.Trim();
            if (double.TryParse(trimmedString, out result))
            {
                success = true;
            }
            else
            {
                // trimmedString can't be interpreted as a double, check it as a variable
                if (m_scriptVariables.ContainsKey(trimmedString))
                {
                    success = GetScriptDouble(m_scriptVariables[trimmedString], out result);
                }
            }
            param = result;
            return success;
        }

        string PerformScriptMath(string stringParam1, string op, string stringParam2)
        {
            double param1 = 0.0;
            double param2 = 0.0;
            if (GetScriptDouble(stringParam1, out param1) && GetScriptDouble(stringParam2, out param2))
            {
                switch (op.ToLower())
                {
                    case "+": return (param1 + param2).ToString();
                    case "-": return (param1 - param2).ToString();
                    case "*": return (param1 * param2).ToString();
                    case "/": return (param1 / param2).ToString();
                    case "%": return (param1 % param2).ToString();
                }
            }
            // If anything goes wrong, return null
            return null;
        }

        private bool? ScriptCompare(string stringParam1, string comparison, string stringParam2)
        {
            double param1 = 0.0;
            double param2 = 0.0;
            if (GetScriptDouble(stringParam1, out param1) && GetScriptDouble(stringParam2, out param2))
            {
                string op = comparison.ToLower();
                switch (op)
                {
                    case "<": return param1 < param2;
                    case "<=": return param1 <= param2;
                    case ">": return param1 > param2;
                    case ">=": return param1 >= param2;
                    case "==": return param1 == param2;
                    case "!=": return param1 == param2;
                }
            }
            // If anything goes wrong, return null
            return null;
        }

        // Returns 'true' if a valid next instruction was found
        bool GetLineNumberFromJumpParam(string jumpParam, out int nextInstruction)
        {
            bool success = false;
            int lineNumber = 0;
            if (int.TryParse(jumpParam, out lineNumber))
            {
                // Parameter was an int, return that number
                success = true;
            }
            else
            {
                // Parameter isn't an int, try to find it as a label
                if (m_labelLineNumbers.ContainsKey(jumpParam))
                {
                    // Found a label
                    lineNumber = m_labelLineNumbers[jumpParam];
                    success = true;
                }
            }
            // jumpParam wasn't a valid int or label, return false to indicate failure
            nextInstruction = lineNumber;
            return success;
        }

        void SetScriptVariable(string variableName, string value)
        {
            if (m_scriptVariables.ContainsKey(variableName))
            {
                m_scriptVariables[variableName] = value;
            }
            else
            {
                m_scriptVariables.Add(variableName, value);
            }
        }

        // Returns 'true' if more commands can be immediately processed
        // Returns 'false' if we should wait until the next tick to process more commands
        bool ProcessScriptCommand(string command, int lineNumber)
        {
            /*
                Var/Num/0
                Var/AutoClickDuration/600 
                Label/Start 
                Recording/Level Up and Buy Upgrades.mou
                Recording/Advance Level and Toggle Progression Mode.mou
                Recording/UseSkills.mou
                Recording/MoveToCenter.mou
                AutoClick/%AutoClickDuration%
                Math/Num/Num/+/1
                Compare/%Num%/</10
                JumpIf/Start
                Jump/End
                Label/End
             */
            bool canProcessMoreCommands = true;     // most commands can be processed on the same update

            UpdateConstants();
            string[] args = GetArgsFromScriptCommand(command);

            LineNumberDisplay.Value = lineNumber;
            string debugOutputLine = lineNumber.ToString();
            foreach (string arg in args)
            {
                debugOutputLine += "/" + arg;
                string trimmedArg = arg.Trim();
                if (m_scriptVariables.ContainsKey(trimmedArg))
                {
                    string value = m_scriptVariables[trimmedArg];
                    debugOutputLine += "(" + value + ")";
                }
            }
            string timeString = string.Format("{0:0.00}", (DateTime.Now - m_scriptStartTime).TotalSeconds);
            m_scriptOutput.AppendLine(timeString + " - " + debugOutputLine);

            try
            {
                if (args.Length >= 2)
                {
                    switch (args[0].ToLower())
                    {
                        case "var":
                            string val = (args.Length >= 3) ? args[2] : "";
                            SetScriptVariable(args[1], val);
                            break;
                        case "label":
                            // No processing needs to be done for labels
                            break;
                        case "math":
                            if (args.Length >= 5)
                            {
                                string computedValue = PerformScriptMath(args[2], args[3], args[4]);
                                SetScriptVariable(args[1], computedValue);
                            }
                            break;
                        case "compare":
                            if (args.Length >= 4)
                            {
                                m_lastComparison = ScriptCompare(args[1], args[2], args[3]);
                            }
                            break;
                        case "recording":
                            LoadRecordingFromFile(args[1]);
                            HK_StartPlayback();
                            canProcessMoreCommands = false;
                            break;
                        case "autoclick":
                            double autoClickDurationInSeconds = 0.0;
                            if (GetScriptDouble(args[1], out autoClickDurationInSeconds))
                            {
                                HK_StartAutoClicking();
                                m_scriptTimeToStopAutoClicking = DateTime.Now.AddSeconds(autoClickDurationInSeconds);
                            }
                            canProcessMoreCommands = false;
                            break;
                        case "jump":
                            {
                                int nextInstruction = 0;
                                if (GetLineNumberFromJumpParam(args[1], out nextInstruction))
                                {
                                    m_scriptNextLine = nextInstruction;
                                }
                            }
                            break;
                        case "jumpif":
                            if (m_lastComparison == true)
                            {
                                int nextInstruction = 0;
                                if (GetLineNumberFromJumpParam(args[1], out nextInstruction))
                                {
                                    m_scriptNextLine = nextInstruction;
                                }
                            }
                            break;
                        case "clickcandy":
                            List<SubImageSearchResult> candy = FindCandy();
                            List<Point> pointsToClick = GetScreenPositionsFromSubImageSearchResults(candy);
                            bool startedAnAction = PerformOneOffMouseClicks(pointsToClick);
                            canProcessMoreCommands = (startedAnAction == false);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                string outputMessage = "===============================================================================\r\n";
                outputMessage += "Exception while processing script!\r\n";
                outputMessage += "Line = " + lineNumber + "\r\n";
                outputMessage += "Command = " + debugOutputLine + "\r\n";
                outputMessage += e.Message + "\r\n";
                outputMessage += "===============================================================================\r\n";
                m_scriptOutput.AppendLine(outputMessage);
                canProcessMoreCommands = false;
                CancelPlayingScript();
            }

            return canProcessMoreCommands;
        }

        void UpdateScriptPlayback()
        {
            if (HK_IsIdle())
            {
                bool runNextLine = true;
                while (runNextLine)
                {
                    // Default to only processing one line per tick
                    runNextLine = false;

                    // Start the next action in the script
                    if (m_scriptNextLine >= ScriptTextBox.Lines.Length)
                    {
                        CancelPlayingScript();
                    }
                    else
                    {
                        int executionLine = m_scriptNextLine;
                        string command = ScriptTextBox.Lines[executionLine];
                        m_scriptNextLine++;
                        runNextLine = ProcessScriptCommand(command, executionLine);
                    }
                }
            }
            else if (HK_IsAutoClicking())
            {
                if (DateTime.Now >= m_scriptTimeToStopAutoClicking)
                {
                    HK_StopAutoClicking();
                }
            }
        }

        void TimerUpdate(object sender, EventArgs e)
        {
            bool wasCancled = HK_IsCancled();

            HK_Update();

            string scriptText = "";
            if (m_playingScript)
            {
                if (wasCancled)
                {
                    CancelPlayingScript();
                }
                else
                {
                    scriptText = "Script - ";
                    UpdateScriptPlayback();
                }
            }

            bool isRecording = false;
            string statusString = "Idle";
            if (HK_IsRecording())
            {
                statusString = "Recording...";
                isRecording = true;
            }
            else if (HK_IsPlayback())
            {
                statusString = "Playback...";
            }
            else if (HK_IsAutoClicking())
            {
                statusString = "Auto Clicking...";
                DateTime now = DateTime.Now;
                TimeSpan period = new TimeSpan(0, 0, 0, (int)AutoClickCandyPeriod.Value, 0);

                // Check if we just started auto-clicking
                if (StatusLabel.Text.Contains(statusString) == false)
                {
                    m_nextAutoCandyClickTime = now + period;
                }

                if (AutoClickCandyCheckBox.Checked && (now > m_nextAutoCandyClickTime))
                {
                    List<SubImageSearchResult> candy = FindCandy();
                    List<Point> pointsToClick = GetScreenPositionsFromSubImageSearchResults(candy);
                    PerformOneOffMouseClicks(pointsToClick);
                    m_nextAutoCandyClickTime = now + period;
                }
            }
            StatusLabel.Text = scriptText + statusString;

            if ((isRecording == false) && (m_wasRecording == true))
            {
                string rawRecordingString = GetRecordingString();
                UpdateRecordingDisplayFromString(rawRecordingString);
            }
            m_wasRecording = isRecording;

            TryUpdateScriptOutput();
        }

        private void TryUpdateScriptOutput()
        {
            if (m_scriptOutput.IsDirty())
            {
                // Limit how frequently the output window is updated
                TimeSpan m_scriptOutputUpdateFrequency = new TimeSpan(0, 0, 0, 1, 0);   // 1 second
                TimeSpan timeSinceLastUpdate = DateTime.Now - m_lastScriptOutputUpdateTime;
                if (timeSinceLastUpdate > m_scriptOutputUpdateFrequency)
                {
                    OutputTextBox.Text = m_scriptOutput.String;
                    m_scriptOutput.ClearDirtyFlag();
                    m_lastScriptOutputUpdateTime = DateTime.Now;
                }
            }
        }

        private void UpdateRecordingDisplayFromString(string rawRecordingString)
        {
            List<InputEvent> events = GetAllEventsFromRawString(rawRecordingString);
            string[] lines = new string[events.Count];
            for (int i = 0; i < events.Count; i++)
            {
                lines[i] = events[i].GetString();
            }
            RecordingText.Lines = lines;
        }

        private List<InputEvent> GetAllEventsFromRawString(string rawRecordingString)
        {
            List<InputEvent> events = new List<InputEvent>();
            string[] lines = rawRecordingString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                InputEvent newEvent = new InputEvent();
                newEvent.InitializeFromRawString(lines[i]);
                events.Add(newEvent);
            }
            return events;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            HK_EndHook();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog.Filter = "Mouseonator Files|*.mou|All Files|*.*";
            saveFileDialog.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = saveFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                string contents = RecordingText.Text;
                System.IO.File.WriteAllText(filename, contents);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog.Filter = "Mouseonator Files|*.mou|All Files|*.*";
            openFileDialog.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadRecordingFromFile(openFileDialog.FileName);
            }
        }

        private void UpdateRecordingButton_Click(object sender, EventArgs e)
        {
            string s = RecordingText.Text;
            int numInputsProcessed = SetRecordingString(s);
            System.Windows.Forms.MessageBox.Show("Num lines updated - " + numInputsProcessed);
        }

        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            // Cancel and playback, just in case
            HK_CancelPlayback();

            m_scriptOutput.Clear();
            StartPlayingScript();
        }

        private void LoadScriptButton_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog.Filter = "Script Files|*.txt|All Files|*.*";
            openFileDialog.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string contents = System.IO.File.ReadAllText(openFileDialog.FileName);
                ScriptTextBox.Text = contents;
            }
        }

        class SubImageSearchResult
        {
            public SubImageSearchResult(int x, int y, double _score, Bitmap _searchImage)
            {
                pos = new Point(x, y);
                score = _score;
                searchImage = _searchImage;
            }

            public Point pos = new Point(0, 0);
            public double score = 0.0f;
            public Bitmap searchImage = null;
        }

        public static byte[] GetByteArrayFromBitmap(Bitmap img)
        {
            byte[] byteArray = new byte[img.Width * img.Height * 3];
            int index = 0;
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    Color c = img.GetPixel(x, y);
                    byteArray[index + 0] = c.R;
                    byteArray[index + 1] = c.G;
                    byteArray[index + 2] = c.B;
                    index += 3;
                }
            }

            return byteArray;
        }

        List<SubImageSearchResult> SubImageSearch(Bitmap searchPattern, Bitmap image)
        {
            List<SubImageSearchResult> matchingLocations = new List<SubImageSearchResult>();

            byte[] imageBytes = GetByteArrayFromBitmap(image);
            byte[] searchBytes = GetByteArrayFromBitmap(searchPattern);

            int imageWidth = image.Width;
            int imageHeight = image.Height;
            int imageStride = imageWidth * 3;
            int searchWidth = searchPattern.Width;
            int searchHeight = searchPattern.Height;
            int searchStride = searchWidth * 3;

            double highestNonMatchingScore = 0.0;
            Point highestNonMatchingPos = new Point(0, 0);

            double matchingThreshold = (double)AutoClickCandyMatchPercent.Value / 100.0;
            Int64 numPatternPixels = searchWidth * searchHeight;
            Int64 totalDeltaMax = numPatternPixels * 256 * 3;
            Int64 totalDeltaThreshold = (Int64)(totalDeltaMax * (1.0 - matchingThreshold));
            for (int y = 0; y < imageHeight - searchHeight; y++)
            {
                for (int x = 0; x < imageWidth - searchWidth; x++)
                {
                    Int64 totalDelta = 0;
                    for (int j = 0; j < searchHeight; j++)
                    {
                        for (int i = 0; i < searchWidth; i++)
                        {
                            int baseImageIndex = (y + j) * imageStride + (x + i) * 3;
                            int baseSearchIndex = j * searchStride + i * 3;
                            int delta = 
                                Math.Abs((int)imageBytes[baseImageIndex] - (int)searchBytes[baseSearchIndex]) +
                                Math.Abs((int)imageBytes[baseImageIndex + 1] - (int)searchBytes[baseSearchIndex + 1]) +
                                Math.Abs((int)imageBytes[baseImageIndex + 2] - (int)searchBytes[baseSearchIndex + 2]);
                            totalDelta += delta;
                        }
                        if (totalDelta > totalDeltaThreshold)
                        {
                            break;
                        }
                    }

                    // Check if total differences are within tolerance
                    if (totalDelta <= totalDeltaThreshold)
                    {
                        double subImageScore = 1.0 - ((double)totalDelta / (double)totalDeltaMax);

                        bool duplicate = false;
                        for (int i = 0; i < matchingLocations.Count; i++)
                        {
                            if ((Math.Abs(matchingLocations[i].pos.X - x) < (searchWidth / 2)) ||
                                (Math.Abs(matchingLocations[i].pos.Y - y) < (searchHeight / 2)))
                            {
                                if (subImageScore > matchingLocations[i].score)
                                {
                                    matchingLocations[i].score = subImageScore;
                                    matchingLocations[i].pos.X = x;
                                    matchingLocations[i].pos.Y = y;
                                }
                                duplicate = true;
                            }
                        }

                        if (duplicate == false)
                        {
                            matchingLocations.Add(new SubImageSearchResult(x, y, subImageScore, searchPattern));
                        }
                    }
                    else
                    {
                        double subImageScore = 1.0 - ((double)totalDelta / (double)totalDeltaMax);
                        if (subImageScore > highestNonMatchingScore)
                        {
                            highestNonMatchingScore = subImageScore;
                            highestNonMatchingPos = new Point(x, y);
                        }
                    }
                }
            }

            return matchingLocations;
        }

        // Returns 'true' if an action sequence was started
        // Returns 'false' if this call did nothing
        private bool PerformOneOffMouseClicks(List<Point> pointsToClick)
        {
            // Early-out if no points were passed in
            if (pointsToClick == null || pointsToClick.Count == 0)
            {
                return false;
            }

            // TODO: This likely won't work with multiple monitors
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            string text = "";
            int timeMs = 0;
            const int msDelayBeforeMove = 10;
            const int msDelayBeforeClick = 100;
            const int msDelayBeforeRelease = 10;

            foreach (Point point in pointsToClick)
            {
                int mouseX = (65535 * point.X) / screenWidth;
                int mouseY = (65535 * point.Y) / screenHeight;
                timeMs += msDelayBeforeMove;
                text += string.Format("0 {0} 0x8001 0x00 {1} {2}\r\n", timeMs, mouseX, mouseY);
                timeMs += msDelayBeforeClick;
                text += string.Format("0 {0} 0x02 0x00 0 0\r\n", timeMs);
                timeMs += msDelayBeforeRelease;
                text += string.Format("0 {0} 0x04 0x00 0 0\r\n", timeMs);
            }
            /*
            string text = string.Format(
                "0 10 0x8001 0x00 {0} {1}\r\n" +		// Move mouse
                "0 100 0x02 0x00 0 0\r\n" +					// l-mouse down
                "0 120 0x04 0x00 0 0\r\n",					// l-mouse up
                (65535 * x) / screenWidth,
                (65535 * y) / screenHeight);
            */
            LoadRecordingFromString(text);
            HK_StartPlayback();
            return true;
        }

        private List<SubImageSearchResult> FindCandy()
        {
            List<SubImageSearchResult> matches = new List<SubImageSearchResult>();

            if (m_candyImages.Count > 0)
            {
                // Grab the screen into a bitmap
                using (Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    // Create a graphics object from the bitmap
                    Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    // Take the screenshot from the upper left corner to the right bottom corner
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

                    foreach (Bitmap searchImage in m_candyImages)
                    {
                        SubImageSearchResult bestMatch = null;
                        List<SubImageSearchResult> potentialMatches = SubImageSearch(searchImage, bmpScreenshot);
                        foreach (SubImageSearchResult potentialMatch in potentialMatches)
                        {
                            if (bestMatch == null || potentialMatch.score > bestMatch.score)
                            {
                                bestMatch = potentialMatch;
                            }
                        }
                        if (bestMatch != null)
                        {
                            matches.Add(bestMatch);
                        }
                    }
                }
            }

            return matches;
        }

        List<Point> GetScreenPositionsFromSubImageSearchResults(List<SubImageSearchResult> results)
        {
            List<Point> points = new List<Point>();
            foreach (SubImageSearchResult result in results)
            {
                Bitmap searchImage = result.searchImage;
                int x = result.pos.X + (searchImage.Width / 2);
                int y = result.pos.Y + (searchImage.Height / 2);
                points.Add(new Point(x, y));
            }
            return points;
        }

        private void ScreenShotButton_Click(object sender, EventArgs e)
        {
            List<SubImageSearchResult> candy = FindCandy();
            List<Point> pointsToClick = GetScreenPositionsFromSubImageSearchResults(candy);
            PerformOneOffMouseClicks(pointsToClick);
        }

        private void LoadCandyImageButton_Click(object sender, EventArgs eventArgs)
        {
            // Dispose of old candy image
            for (int i = CandyImagesPanel.Controls.Count - 1; i >= 0; i--)
            {
                CandyImagesPanel.Controls[i].Dispose();
            }

            CandyImagesPanel.Controls.Clear();

            foreach (Bitmap candy in m_candyImages)
            {
                candy.Dispose();
            }
            m_candyImages.Clear();

            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog.Filter = "Images|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All Files|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                LoadCandyImageFiles(openFileDialog.FileNames);
            }
        }

        void LoadCandyImageFiles(string[] files)
        {
            foreach (string file in files)
            {
                try
                {
                    Bitmap bmp = Image.FromFile(file) as Bitmap;
                    m_candyImages.Add(bmp);
                }
                catch (Exception e)
                {
                    e.ToString();
                    System.Windows.Forms.MessageBox.Show("Failed to load file as image\n" + file);
                }
            }

            AddCandyPicturesToPanel(CandyImagesPanel, m_candyImages);
        }

        private static void AddCandyPicturesToPanel(Panel panel, List<Bitmap> images)
        {
            foreach (Bitmap bmp in images)
            {
                PictureBox newBox = new PictureBox();
                newBox.Image = bmp;
                newBox.SizeMode = PictureBoxSizeMode.Zoom;
                newBox.Dock = DockStyle.Left;
                newBox.Width = 32;
                newBox.BorderStyle = BorderStyle.FixedSingle;
                panel.Controls.Add(newBox);
            }
        }
    }
}
