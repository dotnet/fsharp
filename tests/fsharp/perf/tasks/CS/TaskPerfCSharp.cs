using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public static class TaskPerfCSharp
{
    public const int BufferSize = 128;
    public const int ManyIterations = 10000;

    public static async Task ManyWriteFileAsync()
    {
        const string path = "tmp";
        var junk = new byte[BufferSize];
        using (var file = File.Create(path))
        {
            for (var i = 1; i <= ManyIterations; i++)
            {
                await file.WriteAsync(junk, 0, junk.Length);
            }
        }
        File.Delete(path);
    }

    public static async Task<int> AsyncTask()
    {
        // This may be a bit unfair on C#, the F# one is doing just Task.Yield
        await Task.Yield();
        return 100;
    }

    public static Task<int> SyncTask()
    {
        return Task.FromResult(100);
    }

    public static async Task<int> TenBindsSync_CSharp()
    {
        var x1 = await SyncTask();
        var x2 = await SyncTask();
        var x3 = await SyncTask();
        var x4 = await SyncTask();
        var x5 = await SyncTask();
        var x6 = await SyncTask();
        var x7 = await SyncTask();
        var x8 = await SyncTask();
        var x9 = await SyncTask();
        var x10 = await SyncTask();
        return x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10;
    }

    public static async Task<int> TenBindsAsync_CSharp()
    {
        var x1 = await AsyncTask();
        var x2 = await AsyncTask();
        var x3 = await AsyncTask();
        var x4 = await AsyncTask();
        var x5 = await AsyncTask();
        var x6 = await AsyncTask();
        var x7 = await AsyncTask();
        var x8 = await AsyncTask();
        var x9 = await AsyncTask();
        var x10 = await AsyncTask();
        return x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10;
    }

    public static async Task<int> SingleSyncTask_CSharp()
    {
        return 1;
    }

}

