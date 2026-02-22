namespace FSharp.Test

open System
open System.Diagnostics
open System.Linq
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
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

// realsig and optimized are boxed so null = not set, true or false = set  
// Keeping CompilationHelper as it may be used elsewhere
[<NoComparison; NoEquality; AutoOpen>]
type CompilationHelper internal (filename: obj, directory: obj, realsig: obj, optimize: obj) =

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

    static member private SerializeNullableBool (info: IXunitSerializationInfo) (key: string) (value: obj) =
        let strValue =
            match value with
            | :? bool as b -> if b then "true" else "false"
            | _ -> (null : string)
        info.AddValue(key, strValue)

    static member private DeserializeNullableBool (info: IXunitSerializationInfo) (key: string) : obj =
        match info.GetValue<string>(key) with
        | "true" -> box true
        | "false" -> box false
        | _ -> null

    interface IXunitSerializable with
        member _.Serialize(info: IXunitSerializationInfo) =
            info.AddValue("filename", (if isNull filename then null else string filename))
            info.AddValue("directory", (if isNull directory then null else string directory))
            CompilationHelper.SerializeNullableBool info "realsig" realsig
            CompilationHelper.SerializeNullableBool info "optimize" optimize

        member _.Deserialize(info: IXunitSerializationInfo) =
            filename <- info.GetValue<string>("filename")
            directory <- info.GetValue<string>("directory")
            realsig <- CompilationHelper.DeserializeNullableBool info "realsig"
            optimize <- CompilationHelper.DeserializeNullableBool info "optimize"

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a file, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
[<NoComparison; NoEquality>]
type FileInlineData(filenameArg: string, realsig: BooleanOptions option, optimize: BooleanOptions option, [<CallerFilePath; Optional; DefaultParameterValue("")>]directory: string) =
    inherit DataAttributeBase()
    
    let mutable directory: string = directory
    let mutable filename: string = filenameArg
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

    override _.GetData(_testMethod: MethodInfo, _disposalTracker: DisposalTracker) =
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

        DataAttributeBase.WrapRows(results)
