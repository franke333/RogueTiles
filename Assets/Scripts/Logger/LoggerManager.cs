using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerManager : MonoBehaviour
{
    [System.Serializable]
    public struct LogTypeInfo{
        public bool show;
        public Color color;
    }
    
    public LogTypeInfo Debug, Info, Warning, Error;
}
