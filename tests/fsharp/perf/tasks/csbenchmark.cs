using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public enum Operation { WRITE_ASYNC, FROM_RESULT }
public static class RepeatedAsyncWriteCSharp
{
    public const int BufferSize = 128;
    public static int WriteIterations = 10000;
    public const int ExecutionIterations = 50;
    public static Operation Operation = Operation.WRITE_ASYNC;

    private static async Task WriteFile(string path)
    {
        var junk = new byte[BufferSize];
        using (var file = File.Create(path))
        {
            for (var i = 1; i <= WriteIterations; i++)
            {
                switch (Operation)
                {
                    case Operation.WRITE_ASYNC: 
                        await file.WriteAsync(junk, 0, junk.Length);
                        break;
                    case Operation.FROM_RESULT:
                        await Task.FromResult(100);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private static async Task ReadFile(string path)
    {
        var buffer = new byte[BufferSize];
        using (var file = File.OpenRead(path))
        {
            var reading = true;
            while (reading)
            {
                var countRead = await file.ReadAsync(buffer, 0, buffer.Length);
                reading = countRead > 0;
            }
        }
    }

    public static async Task Bench()
    {
        const string tmp = "tmp";
        var sw = new Stopwatch();
        sw.Start();
        for (var i = 1; i <= ExecutionIterations; i++)
        {
            await WriteFile(tmp);
            await ReadFile(tmp);
        }
        sw.Stop();
        File.Delete(tmp);
        Console.WriteLine($"C# methods completed in {sw.ElapsedMilliseconds} ms");
    }
}

