using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

public static class TraceChild
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int AllocateAndCompute(int seed)
    {
        var bytes = new byte[32 * 1024];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(seed = unchecked(seed * 1664525 + 1013904223));
        GC.KeepAlive(bytes);
        return seed;
    }

    public static void Main(string[] args)
    {
        File.WriteAllText(args[0], Environment.ProcessId.ToString());
        var timer = Stopwatch.StartNew();
        int seed = 1;
        while (timer.ElapsedMilliseconds < 2000)
        {
            for (int i = 0; i < 1000; i++)
                seed = AllocateAndCompute(seed);
            GC.Collect(2, GCCollectionMode.Forced, blocking: true, compacting: true);
        }
        GC.KeepAlive(seed);
        if (args.Length > 1 && args[1] == "--wait")
            Console.ReadLine();
    }
}
