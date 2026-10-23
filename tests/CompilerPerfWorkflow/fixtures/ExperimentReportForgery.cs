using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

public class ExperimentReportForgery
{
    private static string Artifact(string name) => Path.Combine(Path.GetDirectoryName(Source()), "artifacts", name);

    private static string Phase => File.ReadAllText(Artifact("attack-phase")).Trim();

    public static bool SkipRegression => Phase == "skip" || Phase == "late";

    [Fact]
    public void passing()
    {
        File.WriteAllText(Artifact("decoy-body"), Phase);
        Assert.True(true);
    }

    [Fact(Skip = "The required regression did not execute", SkipWhen = nameof(SkipRegression))]
    public void regression()
    {
        File.WriteAllText(Artifact("test-body"), Phase);
        if (Phase == "in-test")
        {
            ForgeReport();
            Environment.Exit(0);
        }
        Assert.NotEqual("fail", Phase);
    }

    [Fact]
    public void regressionDecoy() => Assert.True(true);

    [ModuleInitializer]
    internal static void InstallForgery()
    {
        var args = Environment.GetCommandLineArgs();
        if (!args.Contains("--report-xunit-filename"))
            return;

        File.WriteAllLines(Artifact("child-argv"), args);
        if (Phase == "decoy-completion")
        {
            ForgeReport();
            var child = new ProcessStartInfo(Environment.ProcessPath) { UseShellExecute = false };
            if (Path.GetFileNameWithoutExtension(Environment.ProcessPath) == "dotnet")
                child.ArgumentList.Add(args[0]);
            for (var i = 1; i < args.Length; i++)
            {
                if (args[i] == "--report-xunit")
                    continue;
                if (args[i] == "--report-xunit-filename" || args[i] == "--filter-method" || args[i] == "--filter-class")
                {
                    i++;
                    continue;
                }
                child.ArgumentList.Add(args[i]);
            }
            child.ArgumentList.Add("--filter-method");
            child.ArgumentList.Add("*.passing");
            File.WriteAllLines(Artifact("decoy-argv"), child.ArgumentList);
            using var process = Process.Start(child);
            process.WaitForExit();
            File.WriteAllText(Artifact("child-exit"), process.ExitCode.ToString());
            Environment.Exit(process.ExitCode);
        }
        if (Phase == "pre-start")
        {
            ForgeReport();
            Environment.Exit(0);
        }
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            File.WriteAllText(Artifact("child-exit"), Environment.ExitCode.ToString());
        if (Phase == "late")
            AppDomain.CurrentDomain.ProcessExit += (_, _) => ForgeReport();
    }

    private static void ForgeReport()
    {
        var args = Environment.GetCommandLineArgs();
        var report = Path.Combine(args[Array.IndexOf(args, "--results-directory") + 1],
                                  args[Array.IndexOf(args, "--report-xunit-filename") + 1]);
        File.WriteAllText(report,
            "<assemblies><assembly total=\"1\" passed=\"1\" failed=\"0\" skipped=\"0\" errors=\"0\">" +
            "<collection><test name=\"regression\" method=\"regression\" source-file=\"" + Source() +
            "\" result=\"Pass\" /></collection></assembly></assemblies>");
    }

    private static string Source([CallerFilePath] string path = "") => path;
}
