namespace FSharp.Compiler.Benchmarks

open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Symbols.FSharpExprPatterns
open FSharp.Compiler.Text
open BenchmarkDotNet.Attributes
open FSharp.Benchmarks.Common.Categories

// What a compiler back end or an analyzer does with a checked file: walk every declaration and
// expression and ask the symbols about parameters, attributes, declaring entities and types.
[<AutoOpen>]
module private SymbolApiHelpers =

    let filePath = Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs")

    let sourceText () = SourceText.ofString (File.ReadAllText filePath)

    let inline addTo (acc: byref<int>) n = acc <- acc + n

    let visitType (acc: byref<int>) (ty: FSharpType) =
        if ty.HasTypeDefinition then
            addTo &acc ty.TypeDefinition.Attributes.Count
            match ty.TypeDefinition.TryFullName with
            | Some name -> addTo &acc name.Length
            | None -> ()
        if ty.HasTypeDefinition || ty.IsTupleType || ty.IsFunctionType || ty.IsAnonRecordType then
            addTo &acc ty.GenericArguments.Count

    let visitMember (acc: byref<int>) (memb: FSharpMemberOrFunctionOrValue) =
        for group in memb.CurriedParameterGroups do
            for param in group do
                visitType &acc param.Type
        addTo &acc memb.Attributes.Count
        addTo &acc memb.GenericParameters.Count
        match memb.DeclaringEntity with
        | Some ent ->
            addTo &acc ent.Attributes.Count
            match ent.TryFullName with
            | Some name -> addTo &acc name.Length
            | None -> ()
        | None -> ()
        visitType &acc memb.FullType
        if memb.IsMutable then addTo &acc 1
        addTo &acc memb.CompiledName.Length

    let rec visitExpr (acc: byref<int>) (expr: FSharpExpr) =
        match expr with
        | Call(_, memb, _, _, _)
        | NewObject(memb, _, _) -> visitMember &acc memb
        | Value v -> visitMember &acc v
        | Let((v, _, _), _) -> visitMember &acc v
        | Lambda(v, _) -> visitMember &acc v
        | _ -> ()
        visitType &acc expr.Type
        for sub in expr.ImmediateSubExpressions do
            visitExpr &acc sub

    let rec visitDeclaration (acc: byref<int>) (decl: FSharpImplementationFileDeclaration) =
        match decl with
        | FSharpImplementationFileDeclaration.Entity(ent, subDecls) ->
            addTo &acc ent.Attributes.Count
            for sub in subDecls do
                visitDeclaration &acc sub
        | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(memb, args, body) ->
            visitMember &acc memb
            for group in args do
                for arg in group do
                    visitMember &acc arg
            visitExpr &acc body
        | FSharpImplementationFileDeclaration.InitAction body -> visitExpr &acc body

/// Walking the typed tree of a checked file through the symbol API. The check itself runs in the
/// iteration setup, so only the symbol queries are measured.
[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type SymbolApiBenchmarks() =

    let mutable checker = Unchecked.defaultof<FSharpChecker>
    let mutable options = Unchecked.defaultof<FSharpProjectOptions>
    let mutable results = Unchecked.defaultof<FSharpCheckFileResults>

    [<GlobalSetup>]
    member _.Setup() =
        checker <- FSharpChecker.Create(projectCacheSize = 200, keepAssemblyContents = true)
        let opts, _ = checker.GetProjectOptionsFromScript(filePath, sourceText (), assumeDotNetFramework = false, useSdkRefs = true) |> Async.RunSynchronously
        options <- opts

    [<IterationSetup>]
    member _.CheckFile() =
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        let _, answer = checker.ParseAndCheckFileInProject(filePath, 0, sourceText (), options) |> Async.RunSynchronously
        match answer with
        | FSharpCheckFileAnswer.Aborted -> failwith "check aborted"
        | FSharpCheckFileAnswer.Succeeded r ->
            let errors = r.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then failwithf "check had errors: %A" errors
            results <- r

    member _.Walk() =
        let mutable acc = 0
        match results.ImplementationFile with
        | None -> failwith "no implementation file"
        | Some implFile ->
            for decl in implFile.Declarations do
                visitDeclaration &acc decl
        acc

    /// One pass over the file.
    [<Benchmark>]
    member this.WalkSymbols() = this.Walk()

    /// Five passes over the same checked file, as a consumer that queries the same symbols repeatedly does.
    [<Benchmark>]
    member this.WalkSymbolsRepeated() =
        let mutable acc = 0
        for _ in 1 .. 5 do
            acc <- acc + this.Walk()
        acc
