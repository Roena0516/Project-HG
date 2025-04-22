using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using WindowsInput.Native;

public class InputThreadDivider : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    //public UnityEvent<KeyDown> OnKeyDown;
    //public UnityEvent<KeyUp> OnKeyUp;
    //public UnityEvent<KeyDownAsync> OnKeyDownAsync;
    //public UnityEvent<KeyUpAsync> OnKeyUpAsync;

    [SerializeField] private LineInputChecker lineInputChecker;

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

        pollingList = new VirtualKeyCode[4]
        {
                VirtualKeyCode.VK_D,
                VirtualKeyCode.VK_F,
                VirtualKeyCode.VK_J,
                VirtualKeyCode.VK_L,
        };
        keyStateList = new int[4];
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
                lineInputChecker.DownInput(keyDownBuffer[i].position, keyDownBuffer[i].time);
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
                lineInputChecker.UpInput(keyDownBuffer[i].position, keyDownBuffer[i].time);
            }
            keyUpBuffer.Clear();
        }
    }

    public void OnChartProgressAsync(double progress)
    {
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