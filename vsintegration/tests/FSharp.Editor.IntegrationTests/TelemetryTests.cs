using Microsoft.CodeAnalysis.Testing;
using System.Threading.Tasks;
using Xunit;

namespace FSharp.Editor.IntegrationTests;

public class TelemetryTests : AbstractIntegrationTest
{
    [IdeFact]
    public async Task BasicFSharpTelemetry()
    {
        var template = WellKnownProjectTemplates.FSharpNetCoreClassLibrary;

        await using var telemetryChannel = await Telemetry.EnableTestTelemetryChannelAsync(TestToken);
        await SolutionExplorer.CreateSingleProjectSolutionAsync(template, TestToken);
        await SolutionExplorer.BuildSolutionAsync(TestToken);
        
        var eventName = "vs/projectsystem/cps/loadcpsproject";
        var @event = await telemetryChannel.GetEventAsync(eventName, TestToken);

        var propKey = "VS.ProjectSystem.Cps.Project.Extension";
        Assert.True(@event.Properties.ContainsKey(propKey));

        var propValue = @event.Properties[propKey].ToString();
        Assert.Equal("fsproj", propValue);
    }
}
