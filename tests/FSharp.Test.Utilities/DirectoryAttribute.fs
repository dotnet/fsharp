namespace FSharp.Test

open System
open System.IO
open System.Reflection
open System.Threading.Tasks

open Xunit
open Xunit.v3
open Xunit.Sdk

// TheoryDataRow is in the Xunit namespace
open type Xunit.TheoryDataRow

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open TestFramework

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a directory, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
[<NoComparison; NoEquality>]
type DirectoryAttribute(dir: string) =
    inherit Attribute()
    
    do if String.IsNullOrWhiteSpace(dir) then
            invalidArg "dir" "Directory cannot be null, empty or whitespace only."

    let directoryPath = normalizePathSeparator (Path.GetFullPath(dir))
    let mutable baselineSuffix = ""
    let mutable includes = Array.empty<string>
    
    new([<ParamArray>] dirs: string[]) = DirectoryAttribute(Path.Combine(dirs) : string)
    
    member _.BaselineSuffix with get() = baselineSuffix and set v = baselineSuffix <- v
    member _.Includes with get() = includes and set v = includes <- v

    interface IDataAttribute with
        member _.GetData(_testMethod: MethodInfo, _disposalTracker: DisposalTracker) =
            let data = createCompilationUnitForFiles baselineSuffix directoryPath includes
            let rows = data |> Seq.map (fun row -> Xunit.TheoryDataRow(row) :> Xunit.ITheoryDataRow) |> Seq.toArray :> Collections.Generic.IReadOnlyCollection<_>
            // Use ValueTask constructor for net472 compatibility (ValueTask.FromResult not available)
            ValueTask<Collections.Generic.IReadOnlyCollection<Xunit.ITheoryDataRow>>(rows)
        
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
