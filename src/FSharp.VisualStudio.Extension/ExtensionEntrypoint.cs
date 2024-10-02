namespace FSharp.VisualStudio.Extension;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;
using Extension = Microsoft.VisualStudio.Extensibility.Extension;

/// <summary>
/// Extension entrypoint for the VisualStudio.Extensibility extension.
/// </summary>
[VisualStudioContribution]
internal class ExtensionEntrypoint : Extension
{

    

    /// <inheritdoc/>
    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "FSharp.VisualStudio.Extension.4fd40904-7bdd-40b0-82ab-588cbee624d1",
                version: this.ExtensionAssemblyVersion,
                publisherName: "Publisher name",
                displayName: "FSharp.VisualStudio.Extension",
                description: "Extension description"),
    };

    /// <inheritdoc />
    protected override void InitializeServices(IServiceCollection serviceCollection)
    {
        base.InitializeServices(serviceCollection);

        // You can configure dependency injection here by adding services to the serviceCollection.
    }
}
