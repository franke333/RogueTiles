using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager to select colors and displaybility of message types of logger
/// </summary>
public class LoggerManager : PersistentSingletonClass<LoggerManager>
{
    [System.Serializable]
    public struct LogTypeInfo{
        public bool show;
        public Color color;
    }
    
    public LogTypeInfo Debug, Info, Warning, Error;
}
