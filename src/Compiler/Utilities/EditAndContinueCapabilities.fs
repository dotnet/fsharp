// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.EditAndContinue

open System.Collections.Immutable

/// <summary>
/// Identifies a single edit-and-continue capability advertised by the runtime.
/// Mirrors Roslyn's <c>EditAndContinueCapabilities</c> flags enum
/// (roslyn: src/Features/Core/Portable/EditAndContinue/EditAndContinueCapabilities.cs).
/// </summary>
[<RequireQualifiedAccess>]
type internal EditAndContinueCapability =
    /// <summary>Edit and continue is generally available with the set of capabilities that Mono 6, .NET Framework and .NET 5 have in common.</summary>
    | Baseline
    /// <summary>Adding a static or instance method to an existing type.</summary>
    | AddMethodToExistingType
    /// <summary>Adding a static field to an existing type.</summary>
    | AddStaticFieldToExistingType
    /// <summary>Adding an instance field to an existing type.</summary>
    | AddInstanceFieldToExistingType
    /// <summary>Creating a new type definition.</summary>
    | NewTypeDefinition
    /// <summary>Adding, updating and deleting of custom attributes (as distinct from pseudo-custom attributes).</summary>
    | ChangeCustomAttributes
    /// <summary>Whether the runtime supports updating the Param table, and hence related edits (e.g. parameter renames).</summary>
    | UpdateParameters
    /// <summary>Adding a static or instance method, property or event to an existing type (without backing fields), such that the method and/or the type are generic.</summary>
    | GenericAddMethodToExistingType
    /// <summary>Updating an existing static or instance method, property or event (without backing fields) that is generic and/or contained in a generic type.</summary>
    | GenericUpdateMethod
    /// <summary>Adding a static or instance field to an existing generic type.</summary>
    | GenericAddFieldToExistingType
    /// <summary>The runtime supports adding rows to the MethodImpl table.</summary>
    | AddExplicitInterfaceImplementation
    /// <summary>The runtime supports adding FieldRva table entries.</summary>
    | AddFieldRva

    /// <summary>The capability name as advertised by the runtime; matches the Roslyn enum member name exactly.</summary>
    member this.Name =
        match this with
        | EditAndContinueCapability.Baseline -> "Baseline"
        | EditAndContinueCapability.AddMethodToExistingType -> "AddMethodToExistingType"
        | EditAndContinueCapability.AddStaticFieldToExistingType -> "AddStaticFieldToExistingType"
        | EditAndContinueCapability.AddInstanceFieldToExistingType -> "AddInstanceFieldToExistingType"
        | EditAndContinueCapability.NewTypeDefinition -> "NewTypeDefinition"
        | EditAndContinueCapability.ChangeCustomAttributes -> "ChangeCustomAttributes"
        | EditAndContinueCapability.UpdateParameters -> "UpdateParameters"
        | EditAndContinueCapability.GenericAddMethodToExistingType -> "GenericAddMethodToExistingType"
        | EditAndContinueCapability.GenericUpdateMethod -> "GenericUpdateMethod"
        | EditAndContinueCapability.GenericAddFieldToExistingType -> "GenericAddFieldToExistingType"
        | EditAndContinueCapability.AddExplicitInterfaceImplementation -> "AddExplicitInterfaceImplementation"
        | EditAndContinueCapability.AddFieldRva -> "AddFieldRva"

/// <summary>
/// Immutable set of edit-and-continue capabilities negotiated for a hot reload session.
/// Runtime-provided capability strings are parsed exactly once at the public session boundary
/// (<see cref="Parse"/>); all downstream classification consults this typed model so capability
/// checks are never string-based.
/// </summary>
[<Sealed>]
type internal EditAndContinueCapabilities private (capabilities: ImmutableHashSet<EditAndContinueCapability>) =

    static let baselineOnly =
        EditAndContinueCapabilities(ImmutableHashSet.Create EditAndContinueCapability.Baseline)

    // Keep this set, Name, and Parse aligned with the capability cases.
    static let all =
        EditAndContinueCapabilities(
            ImmutableHashSet.CreateRange
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
        )

    static let allNames = all.CapabilityNames |> Seq.toList

    /// <summary>Returns <c>true</c> when the runtime advertised the given capability.</summary>
    member _.Supports(capability: EditAndContinueCapability) = capabilities.Contains capability

    /// <summary>
    /// Returns <c>true</c> when the session carries no capabilities beyond <see cref="EditAndContinueCapability.Baseline"/>,
    /// i.e. only method-body updates of non-generic methods can be applied.
    /// </summary>
    member _.IsBaselineOnly =
        capabilities.Count = 1
        && capabilities.Contains EditAndContinueCapability.Baseline

    /// <summary>The names of the negotiated capabilities, in stable order, for tracing and diagnostics.</summary>
    member _.CapabilityNames =
        capabilities |> Seq.sort |> Seq.map _.Name |> ImmutableArray.CreateRange

    /// <summary>Renders the negotiated capability names for tracing output.</summary>
    override this.ToString() = String.concat " " this.CapabilityNames

    /// <summary>Structural equality over the underlying capability set.</summary>
    override _.Equals(other: obj) =
        match other with
        | :? EditAndContinueCapabilities as other -> capabilities.SetEquals other.Capabilities
        | _ -> false

    /// <summary>Hash code consistent with structural equality.</summary>
    override _.GetHashCode() =
        capabilities |> Seq.fold (fun acc capability -> acc ^^^ hash capability) 0

    member private _.Capabilities = capabilities

    /// <summary>
    /// Conservative default used when a host does not negotiate runtime capabilities:
    /// only baseline edit-and-continue (method-body updates) is assumed to be supported.
    /// </summary>
    static member BaselineOnly = baselineOnly

    /// <summary>Every capability a maximally-capable runtime can advertise (all
    /// <see cref="EditAndContinueCapability"/> cases). Single source of truth (listed once above,
    /// kept in step with the cases); intended for tests and tooling that assume full runtime support.</summary>
    static member All = all

    /// <summary>The capability name strings of <see cref="All"/>, for hosts and tests that negotiate
    /// capabilities as strings (for example <c>CreateHotReloadSession</c>).</summary>
    static member AllNames: string list = allNames

    /// <summary>
    /// Parses runtime-provided capability names into the typed model. Each input string is a single
    /// capability name; matching is exact (case-sensitive) and unknown names are ignored for forward
    /// compatibility, mirroring Roslyn's <c>EditAndContinueCapabilitiesParser.Parse</c>. A session always
    /// carries at least <see cref="EditAndContinueCapability.Baseline"/>: when no recognized capability
    /// is present the result is <see cref="BaselineOnly"/>, so a capability-less session is unrepresentable.
    /// </summary>
    static member Parse(capabilityNames: string seq) =
        let parsed =
            (ImmutableHashSet<EditAndContinueCapability>.Empty, capabilityNames)
            ||> Seq.fold (fun (acc: ImmutableHashSet<EditAndContinueCapability>) name ->
                match name with
                | "Baseline" -> acc.Add EditAndContinueCapability.Baseline
                | "AddMethodToExistingType" -> acc.Add EditAndContinueCapability.AddMethodToExistingType
                | "AddStaticFieldToExistingType" -> acc.Add EditAndContinueCapability.AddStaticFieldToExistingType
                | "AddInstanceFieldToExistingType" -> acc.Add EditAndContinueCapability.AddInstanceFieldToExistingType
                | "NewTypeDefinition" -> acc.Add EditAndContinueCapability.NewTypeDefinition
                | "ChangeCustomAttributes" -> acc.Add EditAndContinueCapability.ChangeCustomAttributes
                | "UpdateParameters" -> acc.Add EditAndContinueCapability.UpdateParameters
                | "GenericAddMethodToExistingType" -> acc.Add EditAndContinueCapability.GenericAddMethodToExistingType
                | "GenericUpdateMethod" -> acc.Add EditAndContinueCapability.GenericUpdateMethod
                | "GenericAddFieldToExistingType" -> acc.Add EditAndContinueCapability.GenericAddFieldToExistingType
                | "AddExplicitInterfaceImplementation" -> acc.Add EditAndContinueCapability.AddExplicitInterfaceImplementation
                | "AddFieldRva" -> acc.Add EditAndContinueCapability.AddFieldRva
                // Aggregate name accepted by Roslyn so runtimes can advertise broader capabilities with one word.
                | "AddDefinitionToExistingType" ->
                    acc
                        .Add(EditAndContinueCapability.AddMethodToExistingType)
                        .Add(EditAndContinueCapability.AddStaticFieldToExistingType)
                        .Add(EditAndContinueCapability.AddInstanceFieldToExistingType)
                // Unknown capability words are ignored so newer runtimes keep working with older compilers.
                | _ -> acc)

        if parsed.IsEmpty then
            baselineOnly
        else
            EditAndContinueCapabilities(parsed.Add EditAndContinueCapability.Baseline)
