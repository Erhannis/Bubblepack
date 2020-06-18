using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Utils
{
    public static int mod(int x, int m) {
        return (x%m + m)%m;
    }

    public static float sqr(float x) {
        return x * x;
    }


    #region Log once
    private static HashSet<string> logged = new HashSet<string>();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void logOnce(string msg) {
        logged.Add(msg);
        Debug.Log(msg);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void logOnceW(string msg) {
        logged.Add(msg);
        Debug.LogWarning(msg);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void logOnceE(string msg) {
        logged.Add(msg);
        Debug.LogError(msg);
    }
    #endregion Log once
}
