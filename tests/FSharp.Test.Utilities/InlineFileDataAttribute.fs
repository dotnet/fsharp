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
type FileInlineData (directory: string, filename: string) =
    inherit DataAttribute()

    let mutable optimize: BooleanOptions option = None
    let mutable realsig: BooleanOptions option = None

    static let computeBoolValues opt =
        match opt with
        | Some BooleanOptions.True -> [|Some true|]
        | Some BooleanOptions.False -> [|Some false|]
        | Some BooleanOptions.Both -> [|Some true; Some false|]
        | _ -> [|None|]

    static let convertToNullableBool (opt: bool option): obj =
        match opt with
        | None -> null
        | Some opt -> box opt

    member _.Optimize with set v = optimize <- Some v

    member _.Realsig with set v = realsig <- Some v

    override _.GetData _ =

        let getOptions realsig optimize =

            let compilationHelper = CompilationHelper(filename, directory, convertToNullableBool realsig, convertToNullableBool optimize)
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
and CompilationHelper internal (filename: string, directory: string, realsig: obj, optimize: obj) =

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

    new() = CompilationHelper("", "", null, null)

    // Define the implicit conversion operator
    static member op_Implicit(helper: CompilationHelper) : CompilationUnit =
        helper.Value ()

    member _.Value(): CompilationUnit =
        let frame = StackTrace().GetFrame(1) // Get the calling method's frame
        let methodInfo = frame.GetMethod() :?> MethodInfo

        let directoryAttribute = DirectoryAttribute(directory)
        directoryAttribute.Includes <- [| filename |]

        let realsigBsl =  (getBaseline realsig  ".RealInternalSignature")
        let optimizeBsl = (getBaseline optimize ".Optimize")

        match combineBaselines realsigBsl optimizeBsl with
        | Some baseline -> directoryAttribute.BaselineSuffix <- baseline
        | _ -> ()

        let results = directoryAttribute.GetData methodInfo
        if results.Count() <> 1 then failwith $"directoryAttribute returned incorrect number of results requires only 1: actual: {results.Count()}"
        let arguments = results.First()
        if arguments.Count() <> 1 then failwith $"directoryAttribute returned incorrect number of arguments requires only 1: actual: {arguments.Count()}"
        let compilation = arguments.First()

        match compilation with
        | :? CompilationUnit as cu -> cu |> setRealInternalSignature |> setOptimization
        | _ -> failwith "No compilation created"

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
