using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public static class TaskPerf
{
    public const int BufferSize = 128;
    public const int WriteIterations = 10000;

    private static async Task ManyWriteFile(string path)
    {
        const string tmp = "tmp";
        var junk = new byte[BufferSize];
        using (var file = File.Create(path))
        {
            for (var i = 1; i <= WriteIterations; i++)
            {
                await file.WriteAsync(junk, 0, junk.Length);
            }
        }
        File.Delete(tmp);
    }

}

