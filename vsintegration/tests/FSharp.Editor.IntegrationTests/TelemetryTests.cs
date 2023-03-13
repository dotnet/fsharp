using Microsoft.CodeAnalysis.Testing;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class TelemetryTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task BasicFSharpTelemetry()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;
        var solutionExplorer = TestServices.SolutionExplorer;
        var editor = TestServices.Editor;
        var telemetry = TestServices.Telemetry;

        await using var telemetryChannel = await telemetry.EnableTestTelemetryChannelAsync(TestToken);
        await solutionExplorer.CreateSolutionAsync(nameof(TelemetryTests), TestToken);
        await solutionExplorer.AddProjectAsync("Library", template, TestToken);
        await solutionExplorer.BuildSolutionAsync(TestToken);
        
        var eventName = "vs/projectsystem/cps/loadcpsproject";
        var @event = await telemetryChannel.GetEventAsync(eventName, TestToken);

        var propKey = "VS.ProjectSystem.Cps.Project.Extension";
        Assert.True(@event.Properties.ContainsKey(propKey));

        var propValue = @event.Properties[propKey].ToString();
        Assert.Equal("fsproj", propValue);
    }
}
