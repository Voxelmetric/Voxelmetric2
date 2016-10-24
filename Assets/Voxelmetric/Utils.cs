using System.Threading;
using UnityEngine;

public class Utils {

    public static void ProfileCall(ThreadStart threadStart, string sampleName)
    {
        Profiler.BeginSample(sampleName);
        threadStart.DynamicInvoke();
        Profiler.EndSample();
    }
}


