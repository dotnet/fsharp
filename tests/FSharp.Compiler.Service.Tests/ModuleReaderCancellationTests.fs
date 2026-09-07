// Tests utilize caching in module state (checker) and `mutable` state
[<Xunit.TestClass(DisableParallelization = true)>]
module FSharp.Compiler.Service.Tests.ModuleReaderCancellationTests

open System
open System.IO
open System.Reflection
open System.Threading
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Test.Assert
open Internal.Utilities.Library
open FSharp.Compiler.Service.Tests.Common
open Xunit

// Dedicated checker to isolate cancellation tests from the shared checker's state.
let private checker = FSharpChecker.Create(useTransparentCompiler = FSharp.Test.CompilerAssertHelpers.UseTransparentCompiler)

let mutable private cts = new CancellationTokenSource()
let mutable private wasCancelled = false

let runCancelFirstTime f =
    let mutable requestCount = 0
    fun () ->
        if requestCount = 0 then
            cts.Cancel()

        requestCount <- requestCount + 1
        Cancellable.CheckAndThrow()

        f ()


module ModuleReader =
    let subsystemVersion = 4, 0
    let useHighEntropyVA = false
    let metadataVersion = String.Empty
    let flags = 0
    let exportedTypes = mkILExportedTypes []

    let mkCtor () =
        let name = ".ctor"
        let methodAttrs =
            MethodAttributes.Public |||
            MethodAttributes.HideBySig |||
            MethodAttributes.NewSlot |||
            MethodAttributes.SpecialName

        let callingConv = ILCallingConv.Instance
        let parameters = []
        let ret = mkILReturn ILType.Void
        let genericParams = []
        let customAttrs = mkILCustomAttrs []

        let implAttributes = MethodImplAttributes.Managed
        let body = InterruptibleLazy.FromValue MethodBody.NotAvailable
        let securityDecls = emptyILSecurityDecls
        let isEntryPoint = false

        ILMethodDef(name, methodAttrs, implAttributes, callingConv, parameters, ret, body, isEntryPoint, genericParams,
             securityDecls, customAttrs)



type ModuleReader(name, typeDefs, cancelOnModuleAccess) =
    let assemblyName = $"{name}.dll"

    let mkModuleDef =
        let mkModuleDef () =
            let assemblyName = $"{name}.dll"
            let moduleName = name
            let isDll = true

            mkILSimpleModule
                assemblyName moduleName isDll
                ModuleReader.subsystemVersion
                ModuleReader.useHighEntropyVA
                typeDefs
                None None
                ModuleReader.flags
                ModuleReader.exportedTypes
                ""

        if cancelOnModuleAccess then
            runCancelFirstTime mkModuleDef
        else
            mkModuleDef

    member val Timestamp = DateTime.UtcNow
    member val Path = Path.Combine(Path.GetTempPath(), assemblyName)

    interface ILModuleReader with
        member x.ILModuleDef = mkModuleDef ()
        member x.ILAssemblyRefs = []
        member x.Dispose() = ()


type PreTypeDefData =
    { Name: string
      Namespace: string list
      HasCtor: bool
      CancelOnImport: bool }

    member this.TypeDef =
        let methodsDefs =
            if this.HasCtor then
                let mkCtor = runCancelFirstTime (fun _ -> [| ModuleReader.mkCtor () |])
                mkILMethodsComputed mkCtor
            else
                mkILMethods []

        let typeAttributes = TypeAttributes.Public
        ILTypeDef(this.Name, typeAttributes, ILTypeDefLayout.Auto, [], [],
            None, methodsDefs, mkILTypeDefs [], mkILFields [], emptyILMethodImpls, mkILEvents [], mkILProperties [],
            emptyILSecurityDecls, emptyILCustomAttrsStored)

type PreTypeDef(data: PreTypeDefData) =
    let typeDef = data.TypeDef
    let getTypeDef =
        if data.CancelOnImport then runCancelFirstTime (fun _ -> typeDef) else (fun _ -> typeDef)

    interface ILPreTypeDef with
        member x.Name = data.Name
        member x.GetTypeDef() = getTypeDef ()


// Entries for a reader with no namespace structure of its own.
let createPreTypeDefs typeData : struct (string list * ILPreTypeDef)[] =
    typeData
    |> Array.ofList
    |> Array.map (fun data -> struct (data.Namespace, PreTypeDef data :> ILPreTypeDef))

let referenceReaderProjectWithTypeDefs (typeDefs: ILTypeDefs) (cancelOnModuleAccess: bool) (options: FSharpProjectOptions) =
    let reader = new ModuleReader("Reference", typeDefs, cancelOnModuleAccess)

    let project = FSharpReferencedProject.ILModuleReference(
        reader.Path, (fun _ -> reader.Timestamp), (fun _ -> reader)
    )

    { options with ReferencedProjects = [| project |]; OtherOptions = Array.append options.OtherOptions [| $"-r:{reader.Path}"|] }

let referenceReaderProject getPreTypeDefs (cancelOnModuleAccess: bool) (options: FSharpProjectOptions) =
    let typeDefs = mkILTypeDefsGroupedComputed getPreTypeDefs (fun () -> Array.empty)
    referenceReaderProjectWithTypeDefs typeDefs cancelOnModuleAccess options

let parseAndCheck path source options =
    cts <- new CancellationTokenSource()
    wasCancelled <- false

    try
        let checkFileAsync = checker.ParseAndCheckFileInProject(path, 0, SourceText.ofString source, options)
        let result =
            match Async.RunSynchronously(checkFileAsync, cancellationToken = cts.Token) with
            | _, FSharpCheckFileAnswer.Aborted -> None
            | _, FSharpCheckFileAnswer.Succeeded results -> Some results

        // AsyncLocal cleanup may not have propagated yet on slower CI platforms (Linux, MacOS).
        if Cancellable.HasCancellationToken then
            System.Threading.Thread.Sleep(200)

        Cancellable.HasCancellationToken |> shouldEqual false
        result

    with :? OperationCanceledException ->
        wasCancelled <- true
        None



let source1 = """
module Module

let t: T = T()
"""

let source2 = """
module Module

open Ns1.Ns2

let t: T = T()
"""


[<Fact>]
let ``CheckAndThrow is not allowed to throw outside of cancellable`` () =
    Assert.Throws<Exception>(fun () -> Cancellable.CheckAndThrow())

[<Fact>]
let ``Type defs 01 - assembly import`` () =
    let source = source1

    let getPreTypeDefs typeData = runCancelFirstTime (fun _ -> createPreTypeDefs typeData)
    let typeDefs = getPreTypeDefs [ { Name = "T"; Namespace = []; HasCtor = false; CancelOnImport = false } ]
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject typeDefs false options

    // First request, should be cancelled inside getPreTypeDefs
    // The cancellation happens in side CombineImportedAssembliesTask, so background builder node fails to be evaluated
    parseAndCheck path source options |> ignore
    wasCancelled |> shouldEqual true

    // Second request, should succeed, with complete analysis
    match parseAndCheck path source options with
    | Some results ->
        wasCancelled |> shouldEqual false

        results.Diagnostics
        |> Array.map (fun e -> e.Message)
        |> shouldEqual [| "No constructors are available for the type 'T'" |]

    | None -> failwith "Expecting results"


[<Fact>]
let ``Type defs 02 - assembly import`` () =
    let source = source1

    let typeDefs = fun _ -> createPreTypeDefs [ { Name = "T"; Namespace = ["Ns"]; HasCtor = false; CancelOnImport = true } ]
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject typeDefs false options

    parseAndCheck path source options |> ignore
    wasCancelled |> shouldEqual false

    match parseAndCheck path source options with
    | Some results ->
        wasCancelled |> shouldEqual false
        results.Diagnostics |> Array.isEmpty |> shouldEqual false
    | None -> failwith "Expecting results"


[<Fact>]
let ``Type defs 03 - type import`` () =
    let source = source2

    let typeDefs = fun _ -> createPreTypeDefs [ { Name = "T"; Namespace = ["Ns1"; "Ns2"]; HasCtor = false; CancelOnImport = true } ]
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject typeDefs false options

    // First request, should be cancelled inside GetTypeDef
    // This shouldn't be cached due to InterruptibleLazy
    parseAndCheck path source options |> ignore
    wasCancelled |> shouldEqual true

    // Second request, should succeed, with complete analysis
    match parseAndCheck path source options with
    | Some results ->
        wasCancelled |> shouldEqual false

        results.Diagnostics
        |> Array.map (fun e -> e.Message)
        |> shouldEqual [| "No constructors are available for the type 'T'" |]

    | None -> failwith "Expecting results"


[<Fact>]
let ``Type defs 04 - ctor import`` () =
    let source = source1

    let typeDefs = fun _ -> createPreTypeDefs [ { Name = "T"; Namespace = []; HasCtor = true; CancelOnImport = false } ]
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject typeDefs false options

    // First request, should be cancelled inside ILMethodDefs
    // This shouldn't be cached due to InterruptibleLazy
    parseAndCheck path source options |> ignore
    wasCancelled |> shouldEqual true

    // Second request, should succeed, with complete analysis
    match parseAndCheck path source options with
    | Some results ->
        wasCancelled |> shouldEqual false
        results.Diagnostics |> Array.isEmpty |> shouldEqual true

    | None -> failwith "Expecting results"

[<Fact>]
let ``Module def 01 - assembly import`` () =
    let source = source1

    let getPreTypeDefs typeData = fun _ -> createPreTypeDefs typeData
    let typeDefs = getPreTypeDefs [ { Name = "T"; Namespace = []; HasCtor = false; CancelOnImport = false } ]
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject typeDefs true options

    // First request, should be cancelled inside getPreTypeDefs
    // The cancellation happens in side CombineImportedAssembliesTask, so background builder node fails to be evaluated
    parseAndCheck path source options |> ignore
    wasCancelled |> shouldEqual true

    // Second request, should succeed, with complete analysis
    match parseAndCheck path source options with
    | Some results ->
        wasCancelled |> shouldEqual false

        results.Diagnostics
        |> Array.map _.Message
        |> shouldEqual [| "No constructors are available for the type 'T'" |]

    | None -> failwith "Expecting results"


// A namespace split across the metadata must merge into one on import, in metadata order. Synthetic,
// since Roslyn can't emit a genuinely split namespace.
let private splitNamespaceTypes =
    [ { Name = "T1"; Namespace = ["Ns1"; "Ns2"]; HasCtor = false; CancelOnImport = false }
      { Name = "T2"; Namespace = ["Ns1"]; HasCtor = false; CancelOnImport = false }
      { Name = "T3"; Namespace = ["Ns1"; "Ns2"]; HasCtor = false; CancelOnImport = false } ]

[<Fact>]
let ``Split namespace - both fragments merge and are accessible`` () =
    // Both T1 and T3, though split by Ns1.T2 in the metadata.
    let source = """
module Module

open Ns1
open Ns1.Ns2

let _f1 (x: T1) = x
let _f2 (x: T2) = x
let _f3 (x: T3) = x
"""
    let getPreTypeDefs _ = createPreTypeDefs splitNamespaceTypes
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject getPreTypeDefs false options

    match parseAndCheck path source options with
    | Some results ->
        results.Diagnostics
        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
        |> Array.map _.Message
        |> shouldEqual [||]
    | None -> failwith "Expecting results"

let private referencedAssembly (options: FSharpProjectOptions) =
    let results = checker.ParseAndCheckProject(options) |> Async.RunSynchronously

    results.ProjectContext.GetReferencedAssemblies()
    |> List.find (fun a -> a.SimpleName.StartsWith "Reference")

/// The imported types in entity order, each as "Namespace.Path.TypeName".
let private importedTypeOrder (options: FSharpProjectOptions) =
    (referencedAssembly options).Contents.Entities
    |> Seq.map (fun e -> if e.AccessPath = "global" then e.DisplayName else $"{e.AccessPath}.{e.DisplayName}")
    |> List.ofSeq

[<Fact>]
let ``Split namespace - import order is preserved`` () =
    let getPreTypeDefs _ = createPreTypeDefs splitNamespaceTypes
    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProject getPreTypeDefs false options

    // Depth-first, T1 before T3 despite the split. Matches the old import for this shape by coincidence
    // of its depths - see the ordering tests below.
    (referencedAssembly options).Contents.Entities
    |> Seq.map (fun e -> e.DisplayName, e.AccessPath)
    |> List.ofSeq
    |> shouldEqual [ "T2", "Ns1"; "T1", "Ns1.Ns2"; "T3", "Ns1.Ns2" ]


// ---- Entity order ------------------------------------------------------------------------------
//
// Depths 0-3 with siblings at each - enough to pin sibling order at every depth for both reader shapes.
let private orderedTypes =
    [ "G0", []
      "G1", []
      "A1", [ "N1" ]
      "B1", [ "N1" ]
      "A2", [ "N1"; "N2" ]
      "B2", [ "N1"; "N2" ]
      "A3", [ "N1"; "N2"; "N3" ]
      "B3", [ "N1"; "N2"; "N3" ]
      "PA", [ "N1"; "P" ]
      "QA", [ "N1"; "Q" ]
      "C1", [ "M1" ] ]

let private mkTypeData (name, ns) =
    { Name = name; Namespace = ns; HasCtor = false; CancelOnImport = false }

/// Metadata order at every depth, types before child namespaces.
///
/// A deliberate change: the old import reversed siblings once per namespace component consumed, so its
/// order alternated with depth. A grouped tree reverses a type once whatever its depth and a flat table
/// once per component, so metadata order is the only one both shapes can agree on - hence one list here.
let private expectedOrder =
    [ "G0"
      "G1"
      "N1.A1"
      "N1.B1"
      "N1.N2.A2"
      "N1.N2.B2"
      "N1.N2.N3.A3"
      "N1.N2.N3.B3"
      "N1.P.PA"
      "N1.Q.QA"
      "M1.C1" ]

/// The same types as a hand-built tree: what a reader whose own store knows its namespaces hands over.
let rec private mkPreNamespace name depth (types: (string * string list) list) =
    let ownTypes, nested = types |> List.partition (fun (_, ns) -> List.length ns = depth)

    mkILPreNamespaceComputed(
        name,
        (fun () -> [| for t in ownTypes -> PreTypeDef(mkTypeData t) :> ILPreTypeDef |]),
        (fun () ->
            [| for name, group in List.groupBy (fun (_, ns) -> List.item depth ns) nested ->
                   mkPreNamespace name (depth + 1) group |])
    )

let private mkNamespaceTree depth types =
    mkILTypeDefsOfNamespace (mkPreNamespace "" depth types)

[<Fact>]
let ``Import order - grouped entries keep metadata order at every depth`` () =
    // Namespaced entries grouped by the reader: what a metadata table, FSI and static linking produce.
    let typeDefs =
        mkILTypeDefsGroupedComputed
            (fun () -> createPreTypeDefs (List.map mkTypeData orderedTypes))
            (fun () -> Array.empty)

    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs typeDefs false options

    importedTypeOrder options |> shouldEqual expectedOrder

[<Fact>]
let ``Import order - a hand-built namespace tree imports the same`` () =
    // The two ways of handing over the same types must import identically.
    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs (mkNamespaceTree 0 orderedTypes) false options

    importedTypeOrder options |> shouldEqual expectedOrder
