//#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <windows.h>
#include <string>
#include <vector>

#include "Hook.h"

typedef DWORD Time;

enum class HookState
{
	Invalid,		// Hooks haven't been created.  Things are uninitialized
	Idle,			// Hooks have been set up but aren't being used for anything
	Cancled,		// Temporary state after a the cancel key is pressed
	Recording,		// Keyboard and mouse events are being recorded
	Playback,		// Keyboard and mouse events are being played back
	AutoClicking	// Spams mouse clicks as fast as possible
};

const char* InputStringElementSeparator = "\t";

static HHOOK m_mouseHook = NULL;
static HHOOK m_keyboardHook = NULL;
static DWORD m_toggleRecordingKey = 0;
static DWORD m_playbackKey = 0;
static DWORD m_cancelPlaybackKey = 0;
static DWORD m_toggleAutoClickKey = 0;

static HookState m_hookState = HookState::Invalid;
static Time m_timeRecordingStarted = 0;
static std::vector<INPUT> m_recordedInput;
static std::string m_recordingString;
static Time m_timePlaybackStarted = 0;
static int m_playbackNextEventIndex = 0;

static bool m_useVirtualKeys = true;
static bool m_useAbsoluteMousePositions = true;
static bool m_useOnlyFinalMouseMoveEvent = false;
static int m_maxPlaybackEventsPerFrame = 100;		// Note: This parameter hasn't been fully tested

static const DWORD INPUT_TYPE_INVALID = 0xFFFFFFFF;

LRESULT CALLBACK MouseProc(_In_  int nCode, _In_  WPARAM wParam, _In_  LPARAM lParam);
LRESULT CALLBACK KeyboardProc(_In_  int nCode, _In_  WPARAM wParam, _In_  LPARAM lParam);
void UpdatePlayback();
void UpdateAutoClicking();

Time GetTime()
{
	// Using GetTickCount instead of GetTickCount64 because this value gets
	// compared to input event timestamps which are 32 bit
	DWORD ticks = GetTickCount();
	return (Time)ticks;
}

std::string GetStringFromInputStruct(const INPUT& input)
{
	std::string result;

	if (input.type == INPUT_KEYBOARD)
	{
		char msg[64];
		sprintf_s(msg, "%d%s%d%s%d%s%d",
			(int)input.type,
			InputStringElementSeparator, (int)input.ki.time,
			InputStringElementSeparator, (int)input.ki.dwFlags,
			InputStringElementSeparator, (int)input.ki.wVk);
		result = msg;
	}
	else if (input.type == INPUT_MOUSE)
	{
		char msg[64];
		sprintf_s(msg, "%d%s%d%s%d%s%d%s%d%s%d",
			(int)input.type,
			InputStringElementSeparator, (int)input.mi.time,
			InputStringElementSeparator, (int)input.mi.dwFlags,
			InputStringElementSeparator, (int)input.mi.mouseData,
			InputStringElementSeparator, (int)input.mi.dx,
			InputStringElementSeparator, (int)input.mi.dy);
		result = msg;
	}

	return result;
}

INPUT GetInputStructFromString(const char* string)
{
	INPUT input;
	ZeroMemory(&input, sizeof(input));

	int inputType = 0;
	int param[5] = {0};
	// First two parameters are always base 10; Remaining could be hex.
	// Using %d for time because leading zeros shouldn't be parsed as octal.
	int numParsed = sscanf_s(string, " %d %d %i %i %i %i", &inputType, &param[0], &param[1], &param[2], &param[3], &param[4]);
	if ((inputType == INPUT_KEYBOARD) && (numParsed == 4))
	{
		input.type = inputType;
		input.ki.time = param[0];
		input.ki.dwFlags = param[1];
		input.ki.wVk = param[2];
	}
	else if (inputType == INPUT_MOUSE && (numParsed == 6))
	{
		input.type = inputType;
		input.mi.time = param[0];
		input.mi.dwFlags = param[1];
		input.mi.mouseData = param[2];
		input.mi.dx = param[3];
		input.mi.dy = param[4];
	}
	else
	{
		input.type = INPUT_TYPE_INVALID;
	}
	return input;
}

std::string GetLastErrorAsString()
{
    //Get the error message, if any.
    DWORD errorMessageID = ::GetLastError();
    if(errorMessageID == 0)
        return "No error message has been recorded";

    LPSTR messageBuffer = nullptr;
    size_t size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                 NULL, errorMessageID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL);

    std::string message(messageBuffer, size);

    //Free the buffer.
    LocalFree(messageBuffer);

    return message;
}

//=============================================================================
//
// Start of dll export implementation
//
//-----------------------------------------------------------------------------

extern void CallType HK_StartHook()
{
	m_hookState = HookState::Idle;
	if (m_mouseHook == NULL)
	{
		m_mouseHook = SetWindowsHookEx(WH_MOUSE_LL, MouseProc, GetModuleHandle(NULL), NULL);
		if (m_mouseHook == NULL)
		{
			std::string error = GetLastErrorAsString();
		}
	}
	if (m_keyboardHook == NULL)
	{
		m_keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, GetModuleHandle(NULL), NULL);
		if (m_keyboardHook == NULL)
		{
			std::string error = GetLastErrorAsString();
		}
	}
}

extern void CallType HK_EndHook()
{
	if (m_mouseHook != NULL)
	{
		UnhookWindowsHookEx(m_mouseHook);
		m_mouseHook = NULL;
	}
	if (m_keyboardHook != NULL)
	{
		UnhookWindowsHookEx(m_keyboardHook);
		m_keyboardHook = NULL;
	}
}

extern BOOL CallType HK_IsIdle()
{
	return m_hookState == HookState::Idle;
}

extern BOOL CallType HK_IsCancled()
{
	return m_hookState == HookState::Cancled;
}

extern BOOL CallType HK_IsRecording()
{
	return m_hookState == HookState::Recording;
}

extern BOOL CallType HK_IsPlayback()
{
	return m_hookState == HookState::Playback;
}

extern BOOL CallType HK_IsAutoClicking()
{
	return m_hookState == HookState::AutoClicking;
}

extern BOOL CallType HK_IsRecordingSaved()
{
	// Don't report having a saved recording if we're in the process of creating one.
	// Only report having one if there is recorded input.
	return (HK_IsRecording() == false) && (m_recordedInput.size() > 0);
}

extern void CallType HK_SetRecordingKey(DWORD vkCode)
{
	m_toggleRecordingKey = vkCode;
}

extern void CallType HK_SetPlaybackKey(DWORD vkCode)
{
	m_playbackKey = vkCode;
}

extern void CallType HK_SetCancelPlaybackKey(DWORD vkCode)
{
	m_cancelPlaybackKey = vkCode;
}

extern void CallType HK_SetAutoClickKey(DWORD vkCode)
{
	m_toggleAutoClickKey = vkCode;
}

extern void CallType HK_StartRecording()
{
	m_hookState = HookState::Recording;
	m_timeRecordingStarted = GetTime();
	m_recordedInput.clear();
}

extern void CallType HK_StopRecording()
{
	m_hookState = HookState::Idle;
}

extern const char* CallType HK_GetRecordingString()
{
	m_recordingString = "";
	for (int i = 0; i < (int)m_recordedInput.size(); i++)
	{
		std::string inputString = GetStringFromInputStruct(m_recordedInput[i]);
		m_recordingString += ((i == 0) ? "" : "\r\n") + inputString;
	}
	return m_recordingString.c_str();
}

extern DWORD CallType HK_SetRecordingString(const char* recordingString)
{
	m_recordingString = recordingString;
	m_recordedInput.clear();

	const char* parseLocation = recordingString;
	while (*parseLocation != '\0')
	{
		// skip whitespace
		while (isspace(*parseLocation))
		{
			parseLocation++;
		}
		std::string inputLine = "";
		while ((*parseLocation != '\0') && (*parseLocation != '\n'))
		{
			inputLine += *parseLocation;
			parseLocation++;
		}

		INPUT i = GetInputStructFromString(inputLine.c_str());
		if ((i.type == INPUT_KEYBOARD) || (i.type == INPUT_MOUSE))
		{
			m_recordedInput.push_back(i);
		}
	}
	return (DWORD)m_recordedInput.size();
}

extern void CallType HK_StartPlayback()
{
	m_hookState = HookState::Playback;
	m_timePlaybackStarted = GetTime();
	m_playbackNextEventIndex = 0;
}

extern void CallType HK_CancelPlayback()
{
	if (m_hookState != HookState::Idle)
	{
		m_hookState = HookState::Cancled;
	}
}

extern void CallType HK_StartAutoClicking()
{
	m_hookState = HookState::AutoClicking;
}

extern void CallType HK_StopAutoClicking()
{
	m_hookState = HookState::Idle;
}


extern void CallType HK_Update()
{
	if (HK_IsCancled())
	{
		// Only stay in the Cancled state for one update
		m_hookState = HookState::Idle;
	}
	if (HK_IsPlayback())
	{
		UpdatePlayback();
	}
	if (HK_IsAutoClicking())
	{
		UpdateAutoClicking();
	}
}

//-----------------------------------------------------------------------------
//
// End of dll export implementation
//
//=============================================================================

void UpdatePlayback()
{
	// If all events have been processed, the playback is done
	if (m_playbackNextEventIndex >= (int)m_recordedInput.size())
	{
		m_hookState = HookState::Idle;
		return;
	}

	Time now = GetTime();
	Time playbackElapsedTime = now - m_timePlaybackStarted;
	int eventsPlayedThisFrame = 0;
	bool done = false;
	while ((m_playbackNextEventIndex < (int)m_recordedInput.size()) && (eventsPlayedThisFrame < m_maxPlaybackEventsPerFrame) && !done)
	{
		INPUT& recordedInput = m_recordedInput[m_playbackNextEventIndex];
		INPUT inputToSend = recordedInput;

		DWORD eventTime = -1;
		switch (recordedInput.type)
		{
		case INPUT_KEYBOARD:
			eventTime = recordedInput.ki.time;
			inputToSend.ki.time = now;
			break;
		case INPUT_MOUSE:
			eventTime = recordedInput.mi.time;
			inputToSend.mi.time = now;
			break;
		default: break;		// Unsupported input type; Ignore for now
		}

		if (eventTime == -1)
		{
			// Unsupported input type; Ignore for now
			m_playbackNextEventIndex++;
		}
		else if (eventTime < playbackElapsedTime)
		{
			// Send input to system
			UINT numSent = SendInput(1, &inputToSend, sizeof(inputToSend));
			m_playbackNextEventIndex++;
		}
		else
		{
			// Not enough time has passed.  Done updating for now, check again later.
			done = true;
		}

		eventsPlayedThisFrame++;
	}
}

void UpdateAutoClicking()
{
	static Time m_lastAutoClickTime = 0;
	static Time m_minAutoClickDelay = 1000 / 100;	// Just in case, don't send more than 100 clicks per second

	Time now = GetTime();
	Time nextClickTime = m_lastAutoClickTime + m_minAutoClickDelay;
	if (now >= nextClickTime)
	{
		// Perform an auto-click
		INPUT input;
		ZeroMemory(&input, sizeof(input));

		input.type = INPUT_MOUSE;
		input.mi.time = now;

		// Send L-mouse button down
		input.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		UINT numSent = SendInput(1, &input, sizeof(input));

		// Send L-mouse button up
		input.mi.dwFlags = MOUSEEVENTF_LEFTUP;
		numSent = SendInput(1, &input, sizeof(input));

		m_lastAutoClickTime = now;
	}
}

void RecordMouseEvent(_In_  int nCode, _In_  WPARAM wParam, _In_  LPARAM lParam)
{
	// wParam == WM_LBUTTONDOWN, WM_LBUTTONUP, WM_MOUSEMOVE, WM_MOUSEWHEEL, WM_MOUSEHWHEEL, WM_RBUTTONDOWN, or WM_RBUTTONUP.
	MSLLHOOKSTRUCT& mouseEventData = *(MSLLHOOKSTRUCT*)lParam;

	INPUT input;
	ZeroMemory(&input, sizeof(input));
	input.type = INPUT_MOUSE;

	input.mi.time = mouseEventData.time - m_timeRecordingStarted;	// Make the time value recorded 0-based

	switch (wParam)
	{
	case WM_LBUTTONDOWN:
		input.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		break;
	case WM_LBUTTONUP:
		input.mi.dwFlags = MOUSEEVENTF_LEFTUP;
		break;
	case WM_MOUSEMOVE:
		if (m_useAbsoluteMousePositions)
		{
			input.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
			// Absolute mouse positions are normalized to the range [0..65535]
			input.mi.dx = (mouseEventData.pt.x * 65536)/GetSystemMetrics(SM_CXSCREEN);
			input.mi.dy = (mouseEventData.pt.y * 65536)/GetSystemMetrics(SM_CYSCREEN);
		}
		else
		{
			input.mi.dwFlags = MOUSEEVENTF_MOVE;
			// Note: This hasn't been tested to work for relative movement and I doubt it works
			input.mi.dx = mouseEventData.pt.x;
			input.mi.dy = mouseEventData.pt.y;
		}
		break;
	case WM_MOUSEWHEEL:
		// If the message is WM_MOUSEWHEEL, the high-order word of mouseData is the wheel delta.
		input.mi.dwFlags = MOUSEEVENTF_WHEEL;
		input.mi.mouseData = ((int)(mouseEventData.mouseData)) >> 16;	// Make sure mouseData gets properly sign extended
		break;
	case WM_MOUSEHWHEEL:
		input.mi.dwFlags = MOUSEEVENTF_HWHEEL;
		input.mi.mouseData = ((int)(mouseEventData.mouseData)) >> 16;	// Make sure mouseData gets properly sign extended
		break;
	case WM_RBUTTONDOWN:
		input.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
		break;
	case WM_RBUTTONUP:
		input.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
		break;
	case WM_XBUTTONDOWN:
	case WM_XBUTTONUP:
	case WM_XBUTTONDBLCLK:
	case WM_NCXBUTTONDOWN:
	case WM_NCXBUTTONUP:
	case WM_NCXBUTTONDBLCLK:
		OutputDebugString(L"One of these got called\n");
		break;
	default:
		// TODO: Error?
		return;
	}

	// If we only want the final mouse move event, check if the last thing in
	// the recorded list is a move.  If so, remove it before adding the new one.
	if (m_useOnlyFinalMouseMoveEvent && (m_recordedInput.size() > 0))
	{
		// Check if the most recent input was similar to the current one
		const INPUT& back = m_recordedInput.back();
		if ((back.type == input.type) && (back.mi.dwFlags == input.mi.dwFlags))
		{
			if (input.mi.dwFlags == (MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE))
			{
				// Absolute mouse movement; Remove the obsolete entry at the end of the list.
				m_recordedInput.pop_back();
			}
			else if (input.mi.dwFlags == MOUSEEVENTF_MOVE)
			{
				// Note: This code path hasn't been tested
				// Relative mouse movement; Add the previous movement delta to the current delta and remove the old entry.
				input.mi.dx += back.mi.dx;
				input.mi.dy += back.mi.dy;
				m_recordedInput.pop_back();
			}
		}
	}
	m_recordedInput.push_back(input);
}

void RecordKeyboardEvent(_In_  int nCode, _In_  WPARAM wParam, _In_  LPARAM lParam)
{
	// wParam == WM_KEYDOWN, WM_KEYUP, WM_SYSKEYDOWN, or WM_SYSKEYUP.
	KBDLLHOOKSTRUCT& keyboardEventData = *(KBDLLHOOKSTRUCT*)lParam;

	// Ignore the recording toggle key...
	if (m_toggleRecordingKey == keyboardEventData.vkCode)
	{
		return;
	}

	INPUT input;
	ZeroMemory(&input, sizeof(input));
	input.type = INPUT_KEYBOARD;

	//const DWORD LLKHF_LOWER_IL_INJECTED = 0x0002;

	bool extended = (keyboardEventData.flags & LLKHF_EXTENDED) != 0;
	bool lowerInjected = (keyboardEventData.flags & LLKHF_LOWER_IL_INJECTED) != 0;
	bool injected = (keyboardEventData.flags & LLKHF_INJECTED) != 0;
	bool altDown = (keyboardEventData.flags & LLKHF_ALTDOWN) != 0;
	bool keyUp = (keyboardEventData.flags & LLKHF_UP) != 0;

	input.ki.wVk = m_useVirtualKeys ? (WORD)keyboardEventData.vkCode : 0;		// Only used if processing virtual keys
	input.ki.wScan = m_useVirtualKeys ? 0 : (WORD)keyboardEventData.scanCode;	// Only used if processing hardware codes
	input.ki.dwFlags =
		(extended ? KEYEVENTF_EXTENDEDKEY : 0) |		// is an extended key?
		(keyUp ? KEYEVENTF_KEYUP : 0) |					// is the event a keydown or keyup?
		(m_useVirtualKeys ? 0 : KEYEVENTF_SCANCODE);	// process virtual key or hardware code?
	input.ki.time = keyboardEventData.time - m_timeRecordingStarted;	// Make the time value recorded 0-based
	input.ki.dwExtraInfo = NULL;		// keyboardEventData may have extra info, but holding onto that pointer would be unsafe

	m_recordedInput.push_back(input);
}

LRESULT CALLBACK MouseProc(
  _In_  int nCode,
  _In_  WPARAM wParam,
  _In_  LPARAM lParam
)
{
	wchar_t* eventName = L"???";
	switch (wParam)
	{
	case WM_LBUTTONDOWN: eventName = L"L-Down"; break;
	case WM_LBUTTONUP: eventName = L"L-Up"; break;
	case WM_MOUSEMOVE: eventName = L"Move"; break;
	case WM_MOUSEWHEEL: eventName = L"Wheel"; break;
	case WM_RBUTTONDOWN: eventName = L"R-Down"; break;
	case WM_RBUTTONUP: eventName = L"R-Up"; break;
	}
	MSLLHOOKSTRUCT& mouseData = *(MSLLHOOKSTRUCT*)lParam;

	if (HK_IsRecording())
	{
		RecordMouseEvent(nCode, wParam, lParam);
	}

	return CallNextHookEx(m_mouseHook, nCode, wParam, lParam);
}

LRESULT CALLBACK KeyboardProc(
  _In_  int nCode,
  _In_  WPARAM wParam,
  _In_  LPARAM lParam
)
{
	wchar_t* eventName = L"???";
	switch (wParam)
	{
	case WM_KEYDOWN: eventName = L"Down"; break;
	case WM_KEYUP: eventName = L"Up"; break;
	case WM_SYSKEYDOWN: eventName = L"Down(sys)"; break;
	case WM_SYSKEYUP: eventName = L"Up(sys)"; break;
	}
	KBDLLHOOKSTRUCT& keyboardData = *(KBDLLHOOKSTRUCT*)lParam;

	// Check for user defined shortcuts
	if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
	{
		if (keyboardData.vkCode == m_toggleRecordingKey)
		{
			// Toggle recording state
			HK_IsRecording() ? HK_StopRecording() : HK_StartRecording();
		}
		else if (keyboardData.vkCode == m_playbackKey)
		{
			HK_StartPlayback();
		}
		else if (keyboardData.vkCode == m_cancelPlaybackKey)
		{
			if (HK_IsPlayback() || HK_IsAutoClicking())
			{
				HK_CancelPlayback();
			}
		}
		else if (keyboardData.vkCode == m_toggleAutoClickKey)
		{
			// Toggle AutoClick State
			HK_IsAutoClicking() ? HK_StopAutoClicking() : HK_StartAutoClicking();
		}
	}

	if (HK_IsRecording())
	{
		RecordKeyboardEvent(nCode, wParam, lParam);
	}

	return CallNextHookEx(m_keyboardHook, nCode, wParam, lParam);
}
