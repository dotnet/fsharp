namespace FSharp.Compiler.Service.Tests.HotReload

open Xunit
open FSharp.Compiler.EditAndContinue

module EditAndContinueCapabilitiesTests =

    let private allCapabilityCases =
        [
            EditAndContinueCapability.Baseline
            EditAndContinueCapability.AddMethodToExistingType
            EditAndContinueCapability.AddStaticFieldToExistingType
            EditAndContinueCapability.AddInstanceFieldToExistingType
            EditAndContinueCapability.NewTypeDefinition
            EditAndContinueCapability.ChangeCustomAttributes
            EditAndContinueCapability.UpdateParameters
            EditAndContinueCapability.GenericAddMethodToExistingType
            EditAndContinueCapability.GenericUpdateMethod
            EditAndContinueCapability.GenericAddFieldToExistingType
            EditAndContinueCapability.AddExplicitInterfaceImplementation
            EditAndContinueCapability.AddFieldRva
        ]

    [<Fact>]
    let ``parse recognizes every known capability name`` () =
        let names = allCapabilityCases |> List.map (fun capability -> capability.Name)
        let capabilities = EditAndContinueCapabilities.Parse names

        for capability in allCapabilityCases do
            Assert.True(capabilities.Supports capability, $"Expected '{capability.Name}' to be supported")

        Assert.False(capabilities.IsBaselineOnly)

    [<Fact>]
    let ``parse ignores unknown capability names for forward compatibility`` () =
        let capabilities =
            EditAndContinueCapabilities.Parse [ "Baseline"; "SomeFutureCapability"; "AddMethodToExistingType" ]

        Assert.True(capabilities.Supports EditAndContinueCapability.Baseline)
        Assert.True(capabilities.Supports EditAndContinueCapability.AddMethodToExistingType)
        Assert.False(capabilities.Supports EditAndContinueCapability.NewTypeDefinition)

    [<Fact>]
    let ``parse of empty input yields baseline-only capabilities`` () =
        let capabilities = EditAndContinueCapabilities.Parse []

        Assert.True(capabilities.IsBaselineOnly)
        Assert.True(capabilities.Supports EditAndContinueCapability.Baseline)
        Assert.False(capabilities.Supports EditAndContinueCapability.AddMethodToExistingType)
        Assert.Equal<EditAndContinueCapabilities>(EditAndContinueCapabilities.BaselineOnly, capabilities)

    [<Fact>]
    let ``parse of only unknown names yields baseline-only capabilities`` () =
        let capabilities = EditAndContinueCapabilities.Parse [ "NotARealCapability" ]

        Assert.True(capabilities.IsBaselineOnly)

    [<Fact>]
    let ``capability name matching is exact and case-sensitive`` () =
        let capabilities = EditAndContinueCapabilities.Parse [ "addmethodtoexistingtype"; " AddMethodToExistingType" ]

        Assert.False(capabilities.Supports EditAndContinueCapability.AddMethodToExistingType)
        Assert.True(capabilities.IsBaselineOnly)

    [<Fact>]
    let ``parse expands the AddDefinitionToExistingType aggregate like Roslyn`` () =
        let capabilities = EditAndContinueCapabilities.Parse [ "AddDefinitionToExistingType" ]

        Assert.True(capabilities.Supports EditAndContinueCapability.AddMethodToExistingType)
        Assert.True(capabilities.Supports EditAndContinueCapability.AddStaticFieldToExistingType)
        Assert.True(capabilities.Supports EditAndContinueCapability.AddInstanceFieldToExistingType)
        Assert.False(capabilities.Supports EditAndContinueCapability.NewTypeDefinition)

    [<Fact>]
    let ``baseline-only default supports baseline edits only`` () =
        let capabilities = EditAndContinueCapabilities.BaselineOnly

        Assert.True(capabilities.IsBaselineOnly)
        Assert.True(capabilities.Supports EditAndContinueCapability.Baseline)

        for capability in allCapabilityCases do
            if capability <> EditAndContinueCapability.Baseline then
                Assert.False(capabilities.Supports capability, $"Did not expect '{capability.Name}' to be supported")

    [<Fact>]
    let ``capability names round-trip through the typed model`` () =
        let names = [ "AddMethodToExistingType"; "Baseline" ]
        let capabilities = EditAndContinueCapabilities.Parse names

        Assert.Equal<string list>([ "Baseline"; "AddMethodToExistingType" ], capabilities.CapabilityNames)
