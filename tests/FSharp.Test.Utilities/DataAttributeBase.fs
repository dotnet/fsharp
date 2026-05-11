namespace FSharp.Test

open System
open System.Collections.Generic
open System.Reflection
open System.Threading.Tasks
open Xunit
open Xunit.v3
open Xunit.Sdk

/// Provides default implementations for IDataAttribute stub members
/// and a helper to wrap result rows as TheoryDataRow ValueTask.
/// Centralizes boilerplate so that adding new IDataAttribute members
/// only requires updating one place.
[<AbstractClass>]
type DataAttributeBase() =
    inherit Attribute()

    /// Wraps an array of obj[] rows into the ValueTask<IReadOnlyCollection<ITheoryDataRow>> expected by xUnit.
    static member WrapRows(rows: seq<obj[]>) : ValueTask<IReadOnlyCollection<ITheoryDataRow>> =
        let collection =
            rows
            |> Seq.map (fun row -> TheoryDataRow(row) :> ITheoryDataRow)
            |> Seq.toArray :> IReadOnlyCollection<_>
        // Use ValueTask constructor for net472 compatibility (ValueTask.FromResult not available)
        ValueTask<IReadOnlyCollection<ITheoryDataRow>>(collection)

    abstract GetData: testMethod: MethodInfo * disposalTracker: DisposalTracker -> ValueTask<IReadOnlyCollection<ITheoryDataRow>>

    interface IDataAttribute with
        member this.GetData(testMethod, disposalTracker) = this.GetData(testMethod, disposalTracker)
        member _.Explicit = Nullable()
        member _.Label = null
        member _.Skip = null
        member _.SkipType = null
        member _.SkipUnless = null
        member _.SkipWhen = null
        member _.TestDisplayName = null
        member _.Timeout = Nullable()
        member _.Traits = null
        member _.SupportsDiscoveryEnumeration() = true
