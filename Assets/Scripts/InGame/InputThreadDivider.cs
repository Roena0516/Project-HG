using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using WindowsInput.Native;
using System.Linq;
using System.Threading;

public class InputThreadDivider : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    //public UnityEvent<KeyDown> OnKeyDown;
    //public UnityEvent<KeyUp> OnKeyUp;
    //public UnityEvent<KeyDownAsync> OnKeyDownAsync;
    //public UnityEvent<KeyUpAsync> OnKeyUpAsync;

    [SerializeField] private LineInputChecker lineInputChecker;
    private SettingsManager settings;

    private VirtualKeyCode[] pollingList;
    private int[] keyStateList;
    private int keyIndex;

    // shared data
    private List<KeyDown> keyDownBuffer;
    private List<KeyDown> keyDownBackBuffer;
    private List<KeyUp> keyUpBuffer;
    private List<KeyUp> keyUpBackBuffer;
    private bool keyDownDirty;
    private bool keyUpDirty;

    public static InputThreadDivider Instance { get; private set; }

    [DllImport("user32.dll")]
    private static extern UInt16 GetAsyncKeyState(Int32 key);

    private void Awake()
    {
        Assert.IsNotNull(lineInputChecker);

        Debug.Log($"메인 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");

        settings = SettingsManager.Instance;
        var keys = settings.settings.KeyBinds;

        pollingList = new VirtualKeyCode[keys.Count];
        int index = 0;
        foreach (string key in keys)
        {
            if (KeyNameToVirtualKeyCode.TryGetValue(key, out var keyCode))
            {
                pollingList[index] = keyCode;
            }
            else
            {
                Debug.LogWarning($"'{key}'는 지원하지 않는 키입니다.");
            }
            index++;
        }

        keyStateList = new int[pollingList.Length];
        keyDownBuffer = new List<KeyDown>(10);
        keyDownBackBuffer = new List<KeyDown>(10);
        keyUpBuffer = new List<KeyUp>(10);
        keyUpBackBuffer = new List<KeyUp>(10);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnChartPlayerStarted()
    {
        foreach (ref int keyState in keyStateList.AsSpan())
        {
            keyState = 0;
        }
        keyIndex = 1;
        keyDownBuffer.Clear();
        keyDownBackBuffer.Clear();
        keyDownDirty = false;
        keyUpBuffer.Clear();
        keyUpBackBuffer.Clear();
        keyUpDirty = false;
    }

    public void OnChartProgress(double progress)
    {
        if (keyDownDirty)
        {
            lock (keyDownBackBuffer)
            {
                var swap = keyDownBuffer;
                keyDownBuffer = keyDownBackBuffer;
                keyDownBackBuffer = swap;
                keyDownDirty = false;
            }

            for (int i = 0; i < keyDownBuffer.Count; i++)
            {
                //OnKeyDown.Invoke(keyDownBuffer[i]);
                var keyDown = keyDownBuffer[i];
                lineInputChecker.EnqueueMainThreadAction(() =>
                    lineInputChecker.DownInput(keyDown.position, keyDown.time));
            }
            keyDownBuffer.Clear();
        }

        if (keyUpDirty)
        {
            lock (keyUpBackBuffer)
            {
                var swap = keyUpBuffer;
                keyUpBuffer = keyUpBackBuffer;
                keyUpBackBuffer = swap;
                keyUpDirty = false;
            }

            for (int i = 0; i < keyUpBuffer.Count; i++)
            {
                //OnKeyUp.Invoke(keyUpBuffer[i]);
                var keyUp = keyUpBuffer[i];
                lineInputChecker.EnqueueMainThreadAction(() =>
                    lineInputChecker.UpInput(keyUp.position, keyUp.time));
            }
            keyUpBuffer.Clear();
        }
    }

    public void OnChartProgressAsync(double progress)
    {
        //Debug.Log($"비동기 입력 폴링 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");

        for (int i = 0; i < pollingList.Length; i++)
        {
            ushort state = GetAsyncKeyState((int)pollingList[i]);
            if (state == 0 && keyStateList[i] != 0)
            {
                // KeyUp
                int keyIndex = keyStateList[i];
                keyStateList[i] = 0;
                //OnKeyUpAsync.Invoke(new KeyUpAsync
                //{
                //    position= i,
                //    time = progress,
                //});

                lock (keyUpBackBuffer)
                {
                    keyUpBackBuffer.Add(new KeyUp
                    {
                        position = i,
                        time = progress,
                    });
                    keyUpDirty = true;
                }
            }
            else if (state == 0x8000 && keyStateList[i] == 0)
            {
                // KeyDown
                int index = keyIndex++;
                keyStateList[i] = index;
                //OnKeyDownAsync.Invoke(new KeyDownAsync
                //{
                //    position = i,
                //    time = progress,
                //});

                lock (keyDownBackBuffer)
                {
                    keyDownBackBuffer.Add(new KeyDown
                    {
                        position = i,
                        time = progress,
                    });
                    keyDownDirty = true;
                }
            }
        }
    }

    //private void OnChartProgress(ChartProgress data) => OnChartProgress(data.progress);
    //private void OnChartPlayerStarted(ChartPlayerStarted data) => OnChartPlayerStarted();
    //private void OnChartProgressAsync(ChartProgressAsync data) => OnChartProgressAsync(data.progress);
    public static readonly Dictionary<string, VirtualKeyCode> KeyNameToVirtualKeyCode = new()
    {
        // 알파벳
        { "A", VirtualKeyCode.VK_A }, { "B", VirtualKeyCode.VK_B }, { "C", VirtualKeyCode.VK_C },
        { "D", VirtualKeyCode.VK_D }, { "E", VirtualKeyCode.VK_E }, { "F", VirtualKeyCode.VK_F },
        { "G", VirtualKeyCode.VK_G }, { "H", VirtualKeyCode.VK_H }, { "I", VirtualKeyCode.VK_I },
        { "J", VirtualKeyCode.VK_J }, { "K", VirtualKeyCode.VK_K }, { "L", VirtualKeyCode.VK_L },
        { "M", VirtualKeyCode.VK_M }, { "N", VirtualKeyCode.VK_N }, { "O", VirtualKeyCode.VK_O },
        { "P", VirtualKeyCode.VK_P }, { "Q", VirtualKeyCode.VK_Q }, { "R", VirtualKeyCode.VK_R },
        { "S", VirtualKeyCode.VK_S }, { "T", VirtualKeyCode.VK_T }, { "U", VirtualKeyCode.VK_U },
        { "V", VirtualKeyCode.VK_V }, { "W", VirtualKeyCode.VK_W }, { "X", VirtualKeyCode.VK_X },
        { "Y", VirtualKeyCode.VK_Y }, { "Z", VirtualKeyCode.VK_Z },

        // 숫자 (상단 숫자열)
        { "0", VirtualKeyCode.VK_0 }, { "1", VirtualKeyCode.VK_1 }, { "2", VirtualKeyCode.VK_2 },
        { "3", VirtualKeyCode.VK_3 }, { "4", VirtualKeyCode.VK_4 }, { "5", VirtualKeyCode.VK_5 },
        { "6", VirtualKeyCode.VK_6 }, { "7", VirtualKeyCode.VK_7 }, { "8", VirtualKeyCode.VK_8 },
        { "9", VirtualKeyCode.VK_9 },

        // 기능키 (F1 ~ F12)
        { "F1", VirtualKeyCode.F1 }, { "F2", VirtualKeyCode.F2 }, { "F3", VirtualKeyCode.F3 },
        { "F4", VirtualKeyCode.F4 }, { "F5", VirtualKeyCode.F5 }, { "F6", VirtualKeyCode.F6 },
        { "F7", VirtualKeyCode.F7 }, { "F8", VirtualKeyCode.F8 }, { "F9", VirtualKeyCode.F9 },
        { "F10", VirtualKeyCode.F10 }, { "F11", VirtualKeyCode.F11 }, { "F12", VirtualKeyCode.F12 },

        // 특수문자 (OEM)
        { "`", VirtualKeyCode.OEM_3 }, { "-", VirtualKeyCode.OEM_MINUS }, { "=", VirtualKeyCode.OEM_PLUS },
        { "[", VirtualKeyCode.OEM_4 }, { "]", VirtualKeyCode.OEM_6 }, { "\\", VirtualKeyCode.OEM_5 },
        { ";", VirtualKeyCode.OEM_1 }, { "'", VirtualKeyCode.OEM_7 },
        { ",", VirtualKeyCode.OEM_COMMA }, { ".", VirtualKeyCode.OEM_PERIOD }, { "/", VirtualKeyCode.OEM_2 },

        // 방향키
        { "UP", VirtualKeyCode.UP }, { "DOWN", VirtualKeyCode.DOWN },
        { "LEFT", VirtualKeyCode.LEFT }, { "RIGHT", VirtualKeyCode.RIGHT },

        // 제어키 / 조합키
        { "ESC", VirtualKeyCode.ESCAPE }, { "TAB", VirtualKeyCode.TAB },
        { "CAPSLOCK", VirtualKeyCode.CAPITAL }, { "LSHIFT", VirtualKeyCode.LSHIFT }, { "RSHIFT", VirtualKeyCode.RSHIFT },
        { "LCTRL", VirtualKeyCode.LCONTROL }, { "RCTRL", VirtualKeyCode.RCONTROL },
        { "LALT", VirtualKeyCode.LMENU }, { "RALT", VirtualKeyCode.RMENU },
        { "SPACE", VirtualKeyCode.SPACE }, { "ENTER", VirtualKeyCode.RETURN },
        { "BACKSPACE", VirtualKeyCode.BACK },

        // 편집 및 이동
        { "INSERT", VirtualKeyCode.INSERT }, { "DELETE", VirtualKeyCode.DELETE },
        { "HOME", VirtualKeyCode.HOME }, { "END", VirtualKeyCode.END },
        { "PAGEUP", VirtualKeyCode.PRIOR }, { "PAGEDOWN", VirtualKeyCode.NEXT },

        // 시스템 키
        { "PRINTSCREEN", VirtualKeyCode.SNAPSHOT }, { "SCROLLLOCK", VirtualKeyCode.SCROLL },
        { "PAUSE", VirtualKeyCode.PAUSE },

        // 숫자패드 (NumPad)
        { "NUM0", VirtualKeyCode.NUMPAD0 }, { "NUM1", VirtualKeyCode.NUMPAD1 },
        { "NUM2", VirtualKeyCode.NUMPAD2 }, { "NUM3", VirtualKeyCode.NUMPAD3 },
        { "NUM4", VirtualKeyCode.NUMPAD4 }, { "NUM5", VirtualKeyCode.NUMPAD5 },
        { "NUM6", VirtualKeyCode.NUMPAD6 }, { "NUM7", VirtualKeyCode.NUMPAD7 },
        { "NUM8", VirtualKeyCode.NUMPAD8 }, { "NUM9", VirtualKeyCode.NUMPAD9 },
        { "NUMMULTIPLY", VirtualKeyCode.MULTIPLY }, { "NUMADD", VirtualKeyCode.ADD },
        { "NUMSUBTRACT", VirtualKeyCode.SUBTRACT }, { "NUMDECIMAL", VirtualKeyCode.DECIMAL },
        { "NUMDIVIDE", VirtualKeyCode.DIVIDE }, { "NUMENTER", VirtualKeyCode.RETURN }
    };

#endif
}


public class KeyDown
{
    public int position { get; set; }
    public double time { get; set; }
}

public class KeyUp
{
    public int position { get; set; }
    public double time { get; set; }
}

public class KeyDownAsync
{
    public int position { get; set; }
    public double time { get; set; }
}

public class KeyUpAsync
{
    public int position { get; set; }
    public double time { get; set; }
}