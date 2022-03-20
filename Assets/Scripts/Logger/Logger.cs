using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    private enum MessageType
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Count = 4
    }

    private static string[] _messageStrings = new string[]
    {
        "Debug",
        "Info",
        "Warning",
        "Error"
    };

    private static bool[] _showMessages = new bool[(int)MessageType.Count];
    private static Color[] _messageColors = new Color[(int)MessageType.Count];

    static Log()
    {
        var info = GameObject.Find("LoggerManager").GetComponent<LoggerManager>();
        LoggerManager.LogTypeInfo[] datas = new LoggerManager.LogTypeInfo[(int)MessageType.Count]
            { info.Debug, info.Info, info.Warning, info.Error };
        for (int i = 0; i < datas.Length; i++)
        {
            _showMessages[i] = datas[i].show;
            _messageColors[i] = datas[i].color;
        }
        Info("Correctly Loaded", null);
    }


    private static void Display(string message, GameObject obj, MessageType type)
    {
        int index = (int)type;
        if (!_showMessages[index])
            return;
        string name = obj ? obj.name : "";
        if(_messageColors[index]!=null)
            UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGBA(_messageColors[index])}>{_messageStrings[index]} </color>{name}: {message}");
        else
            UnityEngine.Debug.Log($"{_messageStrings[index]} {name}: {message}");
    }


    public static void Debug(string message, GameObject obj = null)
    {
        Display(message, obj, MessageType.Debug);
    }

    public static void Info(string message, GameObject obj = null)
    {
        Display(message, obj, MessageType.Info);
    }

    public static void Warning(string message, GameObject obj = null)
    {
        Display(message, obj, MessageType.Warning);
    }

    public static void Error(string message, GameObject obj = null)
    {
        Display(message, obj, MessageType.Error);
    }


}
