namespace FSharp.Test

open System
open System.Diagnostics
open System.Linq
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Xunit
open Xunit.Abstractions
open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open TestFramework

//[<AutoOpen>]
//module Extensions =
//    let getCompilation (compilation: CompilationHelper): CompilationUnit =
//        let c = unbox<CompilationHelper> compilation
//        c.Value

type BooleanOptions =
    | True = 1
    | False = 2
    | Both = 3
    | None = 0

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a file, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
[<NoComparison; NoEquality>]
type FileInlineData(filename: string, realsig: BooleanOptions option, optimize: BooleanOptions option, [<CallerFilePath; Optional; DefaultParameterValue("")>]directory: string) =
    inherit DataAttribute()

    let mutable directory: string = directory
    let mutable filename: string = filename
    let mutable optimize: BooleanOptions option = optimize
    let mutable realsig: BooleanOptions option = realsig

    static let computeBoolValues opt =
        match opt with
        | Some BooleanOptions.True -> [|Some true|]
        | Some BooleanOptions.False -> [|Some false|]
        | Some BooleanOptions.Both -> [|Some true; Some false|]
        | _ -> [|None|]

    static let convertToBoxed opt =
        match opt with
        | None -> null
        | Some opt -> box opt

    new (filename: string, [<CallerFilePath; Optional; DefaultParameterValue("")>]directory: string) = FileInlineData(filename, None, None, directory)

    member _.Directory with set v = directory <- v

    member _.Optimize with set v = optimize <- Some v

    member _.Realsig with set v = realsig <- Some v

    override _.GetData _ =

        let getOptions realsig optimize =

            let compilationHelper = CompilationHelper(filename, directory, convertToBoxed realsig, convertToBoxed optimize)
            [| box (compilationHelper) |]

        let results =
            let rsValues = computeBoolValues realsig
            let optValues = computeBoolValues optimize
            [|
                for r in rsValues do
                    for o in optValues do
                        getOptions r o
            |]

        results

// realsig and optimized are boxed so null = not set, true or false = set
and [<NoComparison; NoEquality; AutoOpen>]
    CompilationHelper internal (filename: obj, directory: obj, realsig: obj, optimize: obj) =

    let mutable filename = filename
    let mutable directory = directory
    let mutable realsig = realsig
    let mutable optimize = optimize

    let setRealInternalSignature compilation =
        match realsig with
        | null -> compilation
        | realsig -> compilation |> withRealInternalSignature (unbox realsig)

    let setOptimization compilation =
        match optimize with
        | null -> compilation
        | optimize -> compilation |> withOptimization (unbox optimize)

    static let getBaseline (opt: obj) prefix =
        match opt with
        | :? bool as b when b = true -> Some $"{prefix}On"
        | :? bool as b when b = false -> Some $"{prefix}Off"
        | _ -> None

    static let combineBaselines realsigBsl optimizeBsl =
        match realsigBsl, optimizeBsl with
        | Some rs, Some opt -> Some $"{rs}{opt}"
        | Some rs, None -> Some $"{rs}"
        | None, Some opt -> Some $"{opt}"
        | _ -> None

    new() = CompilationHelper(null, null, null, null)

    static member getCompilation(helper: CompilationHelper) : CompilationUnit =
        helper.Value ()

    member private _.Value(): CompilationUnit =
        let directoryPath =
            let path = string directory
            if File.Exists(path) then
                Path.GetDirectoryName(path)
            else
                path

        let fileName =
            if not (isNull filename) then
                string filename
            else
                ""

        let rsLabel = ".RealInternalSignature"
        let optLabel = ".Optimize"
        let realsigBsl =  (getBaseline realsig  rsLabel)
        let optimizeBsl = (getBaseline optimize optLabel)
        let sourceBaselineSuffix = Option.defaultValue "" (combineBaselines realsigBsl optimizeBsl)

        let baselineSuffixes = [
            yield Option.defaultValue "" (combineBaselines realsigBsl optimizeBsl)              // .RealInternalSignatureOff.OptimizeOff
            yield Option.defaultValue "" (combineBaselines realsigBsl (Some (optLabel)))        // .RealInternalSignatureOff.Optimize
            yield Option.defaultValue "" (combineBaselines (Some rsLabel) optimizeBsl)          // .RealInternalSignature.OptimizeOff
            yield Option.defaultValue "" (combineBaselines (Some rsLabel) (Some optLabel))      // .RealInternalSignature.Optimize
            yield Option.defaultValue "" (combineBaselines realsigBsl None)                     // .RealInternalSignatureOff
            yield Option.defaultValue "" (combineBaselines (Some rsLabel) None)                 // .RealInternalSignature
            yield Option.defaultValue "" (combineBaselines None optimizeBsl)                    // .OptimizeOff
            yield Option.defaultValue "" (combineBaselines None (Some optLabel))                // .Optimize
            yield Option.defaultValue "" (combineBaselines None optimizeBsl)                    // .OptimizeOff
            yield Option.defaultValue "" (combineBaselines None (Some optLabel))                // .Optimize
            yield ""                                                                            //
            ]

        let compilation = createCompilationUnit sourceBaselineSuffix baselineSuffixes directoryPath fileName
        compilation |> setRealInternalSignature |> setOptimization

    override _.ToString(): string =
        let file = $"File: {filename}"
        let realsig =
            match realsig with
            | :? bool as b -> $" realsig: {b}"
            | _ -> ""
        let optimize =
            match optimize with
            | :? bool as b -> $" optimize: {b}"
            | _ -> ""
        file + realsig + optimize

    interface IXunitSerializable with
        member _.Serialize(info: IXunitSerializationInfo) =
            info.AddValue("filename", filename)
            info.AddValue("directory", directory)
            info.AddValue("realsig", realsig)
            info.AddValue("optimize", optimize)

        member _.Deserialize(info: IXunitSerializationInfo) =
            filename <- info.GetValue<string>("filename")
            directory <- info.GetValue<string>("directory")
            realsig <- info.GetValue<obj>("realsig")
            optimize <- info.GetValue<obj>("optimize")
