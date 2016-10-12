using System.Threading;
using UnityEngine;

public class Utils {

    public static void ProfileCall(ThreadStart threadStart, string profilerName)
    {
        Profiler.BeginSample(profilerName);
        threadStart.DynamicInvoke();
        Profiler.EndSample();
    }
}
