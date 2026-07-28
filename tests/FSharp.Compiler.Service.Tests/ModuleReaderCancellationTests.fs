// Sequential execution because of shared mutable state.
[<FSharp.Test.RunTestCasesInSequence>]
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

        let callingConv = Callconv(ILThisConvention.Instance, ILArgConvention.Default)
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


// A flat table: each entry pairs the type's namespace with the pre-type-def (all at one level).
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
    referenceReaderProjectWithTypeDefs (mkILTypeDefsComputed getPreTypeDefs) cancelOnModuleAccess options

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


// can only be run explicitly
[<Fact(Skip = "Type shouldn't be imported, see dotnet/fsharp#16166")>]
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


// A namespace split across the metadata (Ns1.Ns2 broken by the intervening Ns1.T2) must merge into
// one namespace on import, and the import order must be preserved. These use the synthetic reader
// above so the split order is under our control (Roslyn can't emit a genuinely split namespace).
let private splitNamespaceTypes =
    [ { Name = "T1"; Namespace = ["Ns1"; "Ns2"]; HasCtor = false; CancelOnImport = false }
      { Name = "T2"; Namespace = ["Ns1"]; HasCtor = false; CancelOnImport = false }
      { Name = "T3"; Namespace = ["Ns1"; "Ns2"]; HasCtor = false; CancelOnImport = false } ]

[<Fact>]
let ``Split namespace - both fragments merge and are accessible`` () =
    // open Ns1.Ns2 must bring in both T1 and T3, even though they are split by Ns1.T2 in the metadata.
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

/// The synthetic reference assembly, analysed as a whole project.
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

    // Depth-first: Ns1's own type T2 first, then the merged Ns1.Ns2 with T1 before T3 (metadata order).
    // Verified to be the same order the pre-ILPreNamespace import produced for this shape - though see
    // the ordering tests below: that parity is a coincidence of this shape's namespace depths.
    (referencedAssembly options).Contents.Entities
    |> Seq.map (fun e -> e.DisplayName, e.AccessPath)
    |> List.ofSeq
    |> shouldEqual [ "T2", "Ns1"; "T1", "Ns1.Ns2"; "T3", "Ns1.Ns2" ]


// ---- Entity order ------------------------------------------------------------------------------
//
// Types at namespace depths 0-3, with sibling types at each depth, two sibling namespaces inside Ns1
// and a second top-level namespace - enough to pin sibling order at every depth for both reader shapes.
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
/// NOTE: this is a deliberate change from the pre-ILPreNamespace import, which bucketed types by
/// prepending into a dictionary and so reversed siblings once per namespace component consumed - its
/// order alternated with depth: "N1.B1, N1.A1" and namespaces "N1.Q, N1.P, N1.N2", but "N1.N2.A2,
/// N1.N2.B2" and then "N1.N2.N3.B3, N1.N2.N3.A3" again.
///
/// That order cannot be kept now that both reader shapes exist: in a grouped tree a type is an item of
/// exactly one level, so it would be reversed once whatever its depth, while a flat table reverses it
/// once per component. Metadata order is the only order the two shapes can agree on - which is why
/// both tests below assert the same list.
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

[<Fact>]
let ``Import order - flat reader keeps metadata order at every depth`` () =
    // A flat table: every entry at one level, carrying its full namespace (the shape custom readers,
    // FSI and static linking produce, and the only shape that existed before ILPreNamespace).
    let typeDefs =
        mkILTypeDefsComputed (fun () -> createPreTypeDefs (List.map mkTypeData orderedTypes))

    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs typeDefs false options

    importedTypeOrder options |> shouldEqual expectedOrder

[<Fact>]
let ``Import order - grouped reader keeps metadata order at every depth`` () =
    // A lazy namespace tree, the shape the default file reader produces. It must import in exactly the
    // same order as the flat table above.
    let typeDefs =
        mkILTypeDefsGroupedComputed
            (fun () -> [| for name, ns in orderedTypes -> struct (ns, mkTypeData (name, ns)) |])
            (fun data -> PreTypeDef data :> ILPreTypeDef)

    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs typeDefs false options

    importedTypeOrder options |> shouldEqual expectedOrder


// ---- Hybrid levels -----------------------------------------------------------------------------
//
// A level that carries BOTH namespaced flat entries and child pre-namespaces, with a namespace name
// coming from both sources. ImportILNamespaceLevel has to merge those into one namespace entity;
// without the merge one of the two same-named entities shadows the other in name resolution and its
// types become unresolvable. No reader produces this shape today - grouped-reader entries carry no
// namespace and flat readers have no pre-namespaces - but the import and ILTypeDefs both support it,
// and addILTypeDef can construct it.
let private hybridTypeDefs () =
    let pre name = PreTypeDef(mkTypeData (name, [])) :> ILPreTypeDef

    let ns2Contents () =
        mkILTypeDefsComputed (fun () -> [| struct ([], pre "TDeepGrouped") |])

    let ns1Contents () =
        mkILTypeDefsAndNamespacesComputed
            (fun () -> [| struct ([], pre "TGrouped") |])
            (fun () -> [| mkILPreNamespaceComputed("Ns2", ns2Contents) |])

    // Ns1 comes from both sources, and so does Ns1.Ns2 (via TDeepFlat's remaining namespace path).
    mkILTypeDefsAndNamespacesComputed
        (fun () -> [| struct ([ "Ns1" ], pre "TFlat"); struct ([ "Ns1"; "Ns2" ], pre "TDeepFlat") |])
        (fun () -> [| mkILPreNamespaceComputed("Ns1", ns1Contents) |])

[<Fact>]
let ``Hybrid level - flat entries and pre-namespaces merge into one namespace`` () =
    let source = """
module Module

open Ns1
open Ns1.Ns2

let _f1 (x: TFlat) = x
let _f2 (x: TGrouped) = x
let _f3 (x: TDeepFlat) = x
let _f4 (x: TDeepGrouped) = x
"""
    let path, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs (hybridTypeDefs ()) false options

    match parseAndCheck path source options with
    | Some results ->
        results.Diagnostics
        |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
        |> Array.map _.Message
        |> shouldEqual [||]
    | None -> failwith "Expecting results"

[<Fact>]
let ``Hybrid level - merged namespace keeps flat entries before pre-namespace contents`` () =
    let _, options = mkTestFileAndOptions [||]
    let options = referenceReaderProjectWithTypeDefs (hybridTypeDefs ()) false options

    importedTypeOrder options
    |> shouldEqual [ "Ns1.TFlat"; "Ns1.TGrouped"; "Ns1.Ns2.TDeepFlat"; "Ns1.Ns2.TDeepGrouped" ]
