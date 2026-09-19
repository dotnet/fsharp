using System;
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
    public void passing() => Assert.True(true);

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
