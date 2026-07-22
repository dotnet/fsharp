namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Reflection
open FSharp.Compiler
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff

open FSharp.Compiler.Service.Tests.Common

// Internal (not private) so the closure-name-allocator tests can reuse the same
// compile-and-extract harness against real typed trees.
type internal DiffTestHarness() =
    let projectDir =
        let dir = Path.Combine(Path.GetTempPath(), "typed-tree-diff-tests", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(dir) |> ignore
        dir

    let filePath = Path.Combine(projectDir, "Library.fs")
    let dllPath = Path.Combine(projectDir, "Test.dll")
    let projPath = Path.Combine(projectDir, "Test.fsproj")

    let checker =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false,
            useTransparentCompiler = FSharp.Test.CompilerAssertHelpers.UseTransparentCompiler
        )

    static let reflectionFlags =
        BindingFlags.Instance ||| BindingFlags.NonPublic ||| BindingFlags.Public

    static let tryGetTypedImplementationFilesTuple (projectResults: FSharpCheckProjectResults) =
        let resultsType = typeof<FSharpCheckProjectResults>

        match resultsType.GetProperty("TypedImplementationFiles", reflectionFlags) with
        | null ->
            match resultsType.GetMethod("get_TypedImplementationFiles", reflectionFlags) with
            | null -> invalidOp "Could not resolve TypedImplementationFiles reflection accessors."
            | getter -> getter.Invoke(projectResults, [||])
        | property -> property.GetValue(projectResults)

    let args = mkProjectCommandLineArgs(dllPath, [ filePath ])

    let projectOptions =
        { checker.GetProjectOptionsFromCommandLineArgs(projPath, args) with
            SourceFiles = [| filePath |] }

    member _.Rewrite(source: string) =
        File.WriteAllText(filePath, source)
        Range.setTestSource filePath source

    member _.Compile() =
        checker.InvalidateAll()

        let projectResults =
            checker.ParseAndCheckProject(projectOptions)
            |> Async.RunImmediate

        if projectResults.HasCriticalErrors then
            let errors =
                projectResults.Diagnostics
                |> Array.choose (fun diag -> if diag.Severity = FSharpDiagnosticSeverity.Error then Some diag.Message else None)
            failwithf "Compilation failed: %A" errors

        let tupleItems =
            tryGetTypedImplementationFilesTuple projectResults
            |> FSharpValue.GetTupleFields

        let tcGlobals = tupleItems[0] :?> FSharp.Compiler.TcGlobals.TcGlobals
        let implFiles = tupleItems[3] :?> CheckedImplFile list

        let matches (CheckedImplFile(qualifiedNameOfFile = qname)) =
            let text = qname.Text
            String.Equals(text, "Library.fs", StringComparison.Ordinal)
            || String.Equals(text, "Library", StringComparison.Ordinal)
            || String.Equals(text, "Test", StringComparison.Ordinal)

        let implFile =
            match List.tryFind matches implFiles with
            | Some impl -> impl
            // Single-file projects: accept whatever module name the source declares (e.g. a
            // multi-segment `module Library.Sub`), so tests can exercise non-default module paths.
            | None ->
                match implFiles with
                | [ single ] -> single
                | _ -> failwithf "Could not locate Library implementation file. Available files: %A" (implFiles |> List.map (fun (CheckedImplFile(qualifiedNameOfFile = qname)) -> qname.Text))

        tcGlobals, implFile

    /// Diffs with an explicit runtime capability set; addition classification depends on it.
    member _.DiffWith (capabilities: EditAndContinueCapabilities) baseline updated =
        let tcGlobals, baselineImpl = baseline
        let _, updatedImpl = updated
        diffImplementationFile tcGlobals capabilities baselineImpl updatedImpl

    /// Diffs with the conservative baseline-only capability set (the session default).
    member this.Diff baseline updated =
        this.DiffWith EditAndContinueCapabilities.BaselineOnly baseline updated

    interface IDisposable with
        member _.Dispose() =
            try checker.InvalidateAll() with _ -> ()
            try Directory.Delete(projectDir, true) with _ -> ()

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module private Sources =
    let moduleHeader = "module Library\n"

    let functionReturning value =
        $"{moduleHeader}let value () = {value}\n"

    let inlineFunction inlineKeyword =
        $"{moduleHeader}let {inlineKeyword}value x = x\n"

    let unionWithFields fieldText =
        $"{moduleHeader}type DU = | Case of {fieldText}\n"

    let deeplySequentialFunction statementCount terminalValue =
        [
            yield moduleHeader + "let value () ="

            for index in 1..statementCount do
                yield $"    ignore {index}"

            yield $"    {terminalValue}"
        ]
        |> String.concat "\n"

    let replaceBindingBodies replacement (CheckedImplFile(qualifiedName, signature, contents, hasEntryPoint, isScript, anonRecords, namedDebugPoints)) =
        let rec replaceBinding binding =
            match binding with
            | ModuleOrNamespaceBinding.Binding(TBind(var, _, debugPoint)) ->
                ModuleOrNamespaceBinding.Binding(TBind(var, replacement, debugPoint))
            | ModuleOrNamespaceBinding.Module(entity, contents) ->
                ModuleOrNamespaceBinding.Module(entity, replaceContents contents)

        and replaceContents contents =
            match contents with
            | ModuleOrNamespaceContents.TMDefs definitions ->
                definitions
                |> List.map replaceContents
                |> ModuleOrNamespaceContents.TMDefs
            | ModuleOrNamespaceContents.TMDefLet(TBind(var, _, debugPoint), bindingRange) ->
                ModuleOrNamespaceContents.TMDefLet(TBind(var, replacement, debugPoint), bindingRange)
            | ModuleOrNamespaceContents.TMDefRec(isRec, opens, tycons, bindings, definitionRange) ->
                ModuleOrNamespaceContents.TMDefRec(
                    isRec,
                    opens,
                    tycons,
                    bindings |> List.map replaceBinding,
                    definitionRange
                )
            | other -> other

        CheckedImplFile(
            qualifiedName,
            signature,
            replaceContents contents,
            hasEntryPoint,
            isScript,
            anonRecords,
            namedDebugPoints
        )

module TypedTreeDiffTests =

    let private assertRequiredCapabilities expected (result: TypedTreeDiffResult) =
        let actual = result.RequiredCapabilities |> List.map (fun capability -> capability.Name)
        Assert.Equal<string list>(expected, actual)

    [<Fact>]
    let ``unchanged file produces no edits`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "1")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.functionReturning "1")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Empty(result.RudeEdits)
        assertRequiredCapabilities [] result

    [<Fact>]
    let ``rude-edit symbol name is not module-prefix-doubled`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library.Sub\ntype State = { Count: int }\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library.Sub\ntype State = { Count: int; Extra: int }\n")
        let updated = harness.Compile()
        let result = harness.Diff baseline updated

        let names =
            (result.RudeEdits |> List.choose (fun e -> e.Symbol |> Option.map (fun s -> s.QualifiedName)))
            @ (result.SemanticEdits |> List.map (fun e -> e.Symbol.QualifiedName))

        // The State type lives in `module Library.Sub`, so its qualified name is "Library.Sub.State",
        // never the historically module-prefix-doubled "Library.Sub.Library.Sub.State".
        Assert.NotEmpty(names)
        Assert.All(names, fun n -> Assert.DoesNotContain("Library.Sub.Library.Sub", n))
        Assert.Contains("Library.Sub.State", names)

    [<Fact>]
    let ``deep expression identity fails closed before exhausting the process stack`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.deeplySequentialFunction 600 1)
        let baseline = harness.Compile()
        harness.Rewrite(Sources.deeplySequentialFunction 600 2)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.Unsupported, rudeEdit.Kind)

    [<Fact>]
    let ``cyclic expression link fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "1")
        let tcGlobals, baselineImpl = harness.Compile()
        let expressionRef: Expr ref = ref Unchecked.defaultof<Expr>
        let cyclicExpression = Expr.Link expressionRef
        expressionRef.Value <- cyclicExpression
        let updatedImpl = Sources.replaceBindingBodies cyclicExpression baselineImpl

        let result = harness.Diff (tcGlobals, baselineImpl) (tcGlobals, updatedImpl)

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.Unsupported, rudeEdit.Kind)

    [<Fact>]
    let ``method body update produces semantic edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "1")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.functionReturning "2")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Empty(result.RudeEdits)
        let edit = result.SemanticEdits[0]
        Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)
        Assert.Equal("value", edit.Symbol.LogicalName)
        assertRequiredCapabilities [ "Baseline" ] result

    [<Fact>]
    let ``field-backed module value initializer update fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet existingValue = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet existingValue = 2\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.Unsupported, rudeEdit.Kind)
        Assert.Equal(Some "existingValue", rudeEdit.Symbol |> Option.map (fun symbol -> symbol.LogicalName))

    [<Fact>]
    let ``inline annotation change triggers rude edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.inlineFunction "inline ")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.inlineFunction "")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Single(result.RudeEdits) |> ignore
        Assert.Equal(RudeEditKind.InlineChange, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``union layout change triggers rude edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.unionWithFields "int")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.unionWithFields "int * int")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Union layout changes can also perturb synthesized comparer/hash methods.
        // Assert we still classify the user-visible shape change as a rude edit.
        let nonSynthesizedSemanticEdits =
            result.SemanticEdits |> List.filter (fun edit -> not edit.IsSynthesized)
        Assert.Empty(nonSynthesizedSemanticEdits)
        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.TypeLayoutChange)

    [<Fact>]
    let ``generic constraint change triggers rude edit`` () =
        // Test that adding/removing generic constraints is detected as a rude edit
        // (SignatureChange) since constraints affect runtime behavior.
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\nlet identity<'T> (x: 'T) = x\n"
        let updated_source = "module Library\nlet identity<'T when 'T :> System.IDisposable> (x: 'T) = x\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Should produce a rude edit (signature change) not a semantic edit
        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.SignatureChange, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``mutable field change triggers rude edit`` () =
        // Test that toggling mutable on a field is detected as a type layout change
        // since it affects the runtime representation of the type.
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\ntype MyRecord = { Value: int }\n"
        let updated_source = "module Library\ntype MyRecord = { mutable Value: int }\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Should produce a rude edit (type layout change) since mutability affects representation
        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.TypeLayoutChange, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``base class change triggers rude edit`` () =
        // Changing the base type (inherit A() -> inherit B()) leaves the ctor body as an
        // allowed MethodBody edit but shifts TypeDef.Extends. The representation hash must
        // catch it now that it reads tcaug_super.
        use harness = new DiffTestHarness()
        let baseline_source =
            "module Library\ntype A() = class end\ntype B() = class end\ntype C() =\n    inherit A()\n"
        let updated_source =
            "module Library\ntype A() = class end\ntype B() = class end\ntype C() =\n    inherit B()\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.TypeLayoutChange)

    [<Fact>]
    let ``adding a marker interface triggers rude edit`` () =
        // A member-less marker interface adds no field/member edit, so without hashing the
        // interface set it would be silently dropped. The representation hash must catch it.
        use harness = new DiffTestHarness()
        let baseline_source =
            "module Library\ntype IMarker = interface end\ntype C() = class end\n"
        let updated_source =
            "module Library\ntype IMarker = interface end\ntype C() =\n    interface IMarker\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.TypeLayoutChange)

    [<Fact>]
    let ``lambda lowering shape change triggers rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let transform = fun x -> x + 1
    transform 41
"""
        let updated_source = """
module Library
let evaluate () =
    let transform = fun x -> fun y -> x + y
    transform 40 2
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // The structural change is a removed+added occurrence pair; the BaselineOnly
        // diff reports it as NotSupportedByRuntime (the addition needs NewTypeDefinition).
        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``lambda lowering shape change with extra closure layer triggers rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let transform = fun value -> value + 1
    transform 41
"""
        let updated_source = """
module Library
let evaluate () =
    let transform =
        fun value ->
            let capture = value
            fun delta -> capture + delta

    transform 40 2
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.NotSupportedByRuntime)

    [<Fact>]
    let ``state machine lowering shape change falls back to lambda rude edit in structural-only mode`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let runAsync () =
    async {
        return 1
    }
"""
        let updated_source = """
module Library
let runAsync () =
    async {
        let! value = async { return 1 }
        return value
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // F# async lowers to closure chains: the let! rewrite adds/removes occurrences,
        // which the BaselineOnly diff reports as NotSupportedByRuntime (NewTypeDefinition).
        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``state machine lowering shape change with async resource scope falls back to lambda rude edit in structural-only mode`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let runAsync () =
    async {
        return 1
    }
"""
        let updated_source = """
module Library
let runAsync () =
    async {
        use reader = new System.IO.StringReader("42")
        return reader.Read()
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.NotEmpty(result.RudeEdits)
        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.NotSupportedByRuntime)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.DeclarationAdded)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.DeclarationRemoved)

    [<Fact>]
    let ``task body edit with stable resume points is a method body update`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        return value + 1
    }
"""
        let updated_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        return value + 2
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // The resumable step sequence (Delay/Bind/Return calls) is unchanged: the edit
        // is a plain MoveNext body update, not a state machine shape change.
        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun e -> e.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    let ``task await addition triggers state machine shape rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        return value + 1
    }
"""
        let updated_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        let! extra = Task.FromResult 2
        return value + extra
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // A new `let!` is a new resume point: F# task state machines are structs whose
        // awaiter/hoisted field layout cannot change, so this is rude (C# parity:
        // ChangingStateMachineShape).
        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``task await removal triggers state machine shape rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        let! extra = Task.FromResult 2
        return value + extra
    }
"""
        let updated_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! value = Task.FromResult 1
        return value + 2
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``task await reorder triggers state machine shape rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! first = Task.FromResult 1
        let! second = Task.FromResult 2
        return first - second
    }
"""
        let updated_source = """
module Library
open System.Threading.Tasks
let runTask () =
    task {
        let! second = Task.FromResult 2
        let! first = Task.FromResult 1
        return first - second
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Same step count, but resume-point state numbers are positional: an in-flight
        // frame suspended at state 1 would resume into the other await's continuation.
        // The reorder surfaces as continuation capture churn inside a resumable member.
        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``plain method gaining a while loop is a method body update`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    1
"""
        let updated_source = """
module Library
let evaluate () =
    let mutable acc = 0
    let mutable i = 0
    while i < 2 do
        acc <- acc + 1
        i <- i + 1
    acc
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Plain control flow lowers to ordinary IL in the method body: while/for/try
        // additions are NOT state machine evidence.
        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun e -> e.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    let ``async body edit gaining inner try with is not a state machine rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let runAsync () =
    async {
        let! value = async { return 1 }
        return value + 1
    }
"""
        let updated_source = """
module Library
let runAsync () =
    async {
        let! value = async { return 1 }
        let safe = (try value with _ -> 0)
        return safe + 1
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // async lowers to closure chains, not resumable state machines: an inner
        // (expression-level) try/with is a closure body edit.
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``query lowering shape change falls back to lambda rude edit in structural-only mode`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open Microsoft.FSharp.Linq

let queryValues () =
    query {
        for x in [1..5] do
        select x
    }
    |> Seq.toList
"""
        let updated_source = """
module Library
open Microsoft.FSharp.Linq

let queryValues () =
    query {
        for x in [1..5] do
        where (x > 2)
        select x
    }
    |> Seq.toList
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, result.RudeEdits[0].Kind)

    [<Fact>]
    let ``query lowering shape change with sort clause falls back to lambda rude edit in structural-only mode`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open Microsoft.FSharp.Linq

let queryValues () =
    query {
        for x in [1..5] do
        select x
    }
    |> Seq.toList
"""
        let updated_source = """
module Library
open Microsoft.FSharp.Linq

let queryValues () =
    query {
        for x in [1..5] do
        sortByDescending x
        select x
    }
    |> Seq.toList
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.LambdaShapeChange)

    [<Fact>]
    let ``query-like member names without query lowering do not trigger query rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library

type QueryLike() =
    member _.Where(x: int) = x

let evaluate () =
    let q = QueryLike()
    q.Where(41)
"""
        let updated_source = """
module Library

type QueryLike() =
    member _.Where(x: int) = x + 1

let evaluate () =
    let q = QueryLike()
    q.Where(41)
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.QueryExpressionShapeChange)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``state-machine-like member names without lowered shape do not trigger state-machine rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library

type WorkflowLike() =
    member _.Bind(x: int) = x

let run () =
    let workflow = WorkflowLike()
    workflow.Bind(41)
"""
        let updated_source = """
module Library

type WorkflowLike() =
    member _.Bind(x: int) = x + 1

let run () =
    let workflow = WorkflowLike()
    workflow.Bind(41)
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.QueryExpressionShapeChange)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    // =========================================================================
    // Method Addition Tests
    // Following Roslyn patterns for Edit and Continue restrictions
    // =========================================================================

    /// The full capability set a maximally-capable runtime advertises; addition tests pass it
    /// explicitly because the diff defaults to the conservative baseline-only set.
    let private allCapabilities = EditAndContinueCapabilities.All

    [<Fact>]
    let ``adding instance method to class produces semantic edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.Existing() = 1
    member this.NewMethod() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        // Adding a non-virtual instance method should produce an Insert semantic edit
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

    [<Fact>]
    let ``adding static method to class produces semantic edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    static member Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    static member Existing() = 1
    static member NewStaticMethod() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        // Adding a static method should produce an Insert semantic edit
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

    [<Fact>]
    let ``adding method with AddMethodToExistingType capability produces semantic edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.Existing() = 1
    member this.NewMethod() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // The single capability gating method additions is enough; no other capability is required.
        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

    [<Fact>]
    let ``adding method without AddMethodToExistingType capability produces NotSupportedByRuntime rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.Existing() = 1
    member this.NewMethod() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Session default: the runtime did not advertise AddMethodToExistingType.
        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        // The message must name the missing runtime capability so hosts can surface it.
        Assert.Contains("AddMethodToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding virtual method produces rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.Existing() = 1
    abstract member NewVirtual : unit -> int
    default this.NewVirtual() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Always rude even when the runtime advertises every capability.
        let result = harness.DiffWith allCapabilities baseline updated

        // Adding a virtual method should produce a rude edit
        Assert.NotEmpty(result.RudeEdits)
        // At least one should be InsertVirtual
        let hasVirtualRudeEdit = result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.InsertVirtual)
        Assert.True(hasVirtualRudeEdit, "Expected InsertVirtual rude edit for adding virtual method")

    [<Fact>]
    let ``adding override method produces rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type BaseClass() =
    abstract member Method : unit -> int
    default this.Method() = 1

type DerivedClass() =
    inherit BaseClass()
"""
        let updated_source = """
module Library
type BaseClass() =
    abstract member Method : unit -> int
    default this.Method() = 1

type DerivedClass() =
    inherit BaseClass()
    override this.Method() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        // Adding an override method should produce a rude edit
        Assert.NotEmpty(result.RudeEdits)
        let hasVirtualRudeEdit = result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.InsertVirtual)
        Assert.True(hasVirtualRudeEdit, "Expected InsertVirtual rude edit for adding override method")

    [<Fact>]
    let ``adding operator produces rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyNumber(value: int) =
    member this.Value = value
"""
        let updated_source = """
module Library
type MyNumber(value: int) =
    member this.Value = value
    static member (+) (a: MyNumber, b: MyNumber) = MyNumber(a.Value + b.Value)
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        // Adding an operator should produce a rude edit
        Assert.NotEmpty(result.RudeEdits)
        let hasOperatorRudeEdit = result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.InsertOperator)
        Assert.True(hasOperatorRudeEdit, "Expected InsertOperator rude edit for adding operator")

    [<Fact>]
    let ``adding explicit interface implementation produces explicit-interface rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library

type IFoo =
    abstract member Compute : unit -> int

type MyClass() =
    member _.Existing() = 1
"""
        let updated_source = """
module Library

type IFoo =
    abstract member Compute : unit -> int

type MyClass() =
    member _.Existing() = 1
    interface IFoo with
        member _.Compute() = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        Assert.NotEmpty(result.RudeEdits)
        let hasExplicitInterfaceRudeEdit =
            result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.InsertExplicitInterface)
        Assert.True(
            hasExplicitInterfaceRudeEdit,
            "Expected InsertExplicitInterface rude edit for adding explicit interface implementation"
        )

    [<Fact>]
    let ``adding module-level value with capabilities produces insert edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existingValue = 1
"""
        let updated_source = """
module Library
let existingValue = 1
let newValue = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddStaticFieldToExistingType"; "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        // Module-level values lower to static field + accessor on the module type; with the
        // static-field and method capabilities the addition is an Insert edit (the
        // capability-less rejection is covered by the dedicated gating tests below).
        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun e -> e.Symbol.LogicalName = "newValue"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)

    // =========================================================================
    // Property Addition Tests
    // =========================================================================

    [<Fact>]
    let ``adding auto-property to class with capabilities produces accessor inserts and field edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member val ExistingProp = 1 with get, set
"""
        let updated_source = """
module Library
type MyClass() =
    member val ExistingProp = 1 with get, set
    member val NewProp = 42 with get, set
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        // An auto-property lowers to get_/set_ accessor methods plus a backing instance
        // field initialized in the primary constructor. With the method and instance-field
        // capabilities the addition is fully supported: accessor Insert edits, a
        // constructor body update, and a TypeDefinition edit for the grown field table.
        Assert.Empty(result.RudeEdits)

        let insertNames =
            result.SemanticEdits
            |> List.filter (fun e -> e.Kind = SemanticEditKind.Insert)
            |> List.map (fun e -> e.Symbol.LogicalName)
            |> List.sort

        Assert.Equal<string list>([ "get_NewProp"; "set_NewProp" ], insertNames)

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.TypeDefinition && e.Symbol.LogicalName = "MyClass")

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.MethodBody && e.Symbol.LogicalName = ".ctor")

    [<Fact>]
    let ``adding auto-property without instance field capability names the missing capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member val ExistingProp = 1 with get, set
"""
        let updated_source = """
module Library
type MyClass() =
    member val ExistingProp = 1 with get, set
    member val NewProp = 42 with get, set
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Accessor methods are allowed, but the backing field needs the instance-field
        // capability: the entity-level field diff must name it.
        let methodOnly = EditAndContinueCapabilities.Parse [ "Baseline"; "AddMethodToExistingType" ]
        let result = harness.DiffWith methodOnly baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("AddInstanceFieldToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding readonly property to class produces semantic edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.ExistingProp = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.ExistingProp = 1
    member this.NewReadonlyProp = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        // Adding a readonly property should produce an Insert semantic edit
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

    // =========================================================================
    // Event Addition Tests
    // =========================================================================

    [<Fact>]
    let ``adding event with backing field at baseline capabilities names missing capabilities`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
"""
        let updated_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    let newEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
    [<CLIEvent>]
    member this.NewEvent = newEvent.Publish
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Adding an event with a backing field (let newEvent = ...) needs both the method
        // capability (add_/remove_ accessors) and the instance-field capability (the
        // backing field on the class). At baseline-only capabilities every part of the
        // addition is NotSupportedByRuntime, naming the missing capabilities.
        Assert.NotEmpty(result.RudeEdits)
        Assert.All(result.RudeEdits, fun e -> Assert.Equal(RudeEditKind.NotSupportedByRuntime, e.Kind))

        let mentionsFieldCapability =
            result.RudeEdits
            |> List.exists (fun e -> e.Message.Contains("AddInstanceFieldToExistingType"))

        Assert.True(mentionsFieldCapability, "Expected the backing-field addition to name AddInstanceFieldToExistingType")

    [<Fact>]
    let ``adding mutable module value with field and method capabilities produces insert edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let mutable newCounter = 0
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddStaticFieldToExistingType"; "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun e -> e.Symbol.LogicalName = "newCounter"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)
        assertRequiredCapabilities
            [
                "Baseline"
                "AddMethodToExistingType"
                "AddStaticFieldToExistingType"
            ]
            result

    [<Fact>]
    let ``adding mutable module value without static field capability names the missing capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let mutable newCounter = 0
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Method additions allowed, static fields not: the value addition must report the
        // field capability as the missing one.
        let methodOnly = EditAndContinueCapabilities.Parse [ "Baseline"; "AddMethodToExistingType" ]
        let result = harness.DiffWith methodOnly baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("AddStaticFieldToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding immutable module value with capabilities produces insert edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let newValue = System.DateTime.Now.Ticks
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddStaticFieldToExistingType"; "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun e -> e.Symbol.LogicalName = "newValue"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)

    [<Fact>]
    let ``adding module function requires only the method capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let newFunction (x: int) = x + 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Module functions lower to static methods: AddMethodToExistingType alone suffices.
        let methodOnly = EditAndContinueCapabilities.Parse [ "Baseline"; "AddMethodToExistingType" ]
        let result = harness.DiffWith methodOnly baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun e -> e.Symbol.LogicalName = "newFunction"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)

    [<Fact>]
    let ``adding module function without method capability produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let newFunction (x: int) = x + 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("AddMethodToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding instance field to class with capability produces type definition edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    [<DefaultValue>] val mutable NewField: int
    member this.Existing() = 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Instance fields on classes are capability-gated. A [<DefaultValue>]
        // val mutable produces no binding/constructor change, so the addition surfaces as
        // a single TypeDefinition edit (the emitter discovers the new Field row from the
        // fresh compile); the symbol path mirrors the IL type name.
        let capabilities = EditAndContinueCapabilities.Parse [ "AddInstanceFieldToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits)
        Assert.Equal(SemanticEditKind.TypeDefinition, edit.Kind)
        Assert.Equal("MyClass", edit.Symbol.LogicalName)

    [<Fact>]
    let ``adding instance field without capability names the missing capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    [<DefaultValue>] val mutable NewField: int
    member this.Existing() = 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("AddInstanceFieldToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``removing a middle overload reports only the removed declaration`` () =
        use harness = new DiffTestHarness()

        let baselineSource =
            """module Library
type C() =
    member _.M(value: bool) = if value then 1 else 0
    member _.M(value: int) = value
    member _.M(value: string) = value.Length
"""

        let updatedSource =
            """module Library
type C() =
    member _.M(value: bool) = if value then 1 else 0
    member _.M(value: string) = value.Length
"""

        harness.Rewrite(baselineSource)
        let baseline = harness.Compile()
        harness.Rewrite(updatedSource)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.DeclarationRemoved, rudeEdit.Kind)
        Assert.Equal(Some "M", rudeEdit.Symbol |> Option.map (fun symbol -> symbol.LogicalName))

    [<Fact>]
    let ``adding let-bound instance field to class with capability pairs constructor update`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing() = 1
"""
        let updated_source = """
module Library
type MyClass() =
    let mutable total = 41
    member this.Existing() = 1 + total
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // A `let mutable` class field folds its initializer into the primary constructor:
        // the diff pairs the constructor (and any member reading the field) as MethodBody
        // updates plus the TypeDefinition edit for the grown field table.
        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.TypeDefinition && e.Symbol.LogicalName = "MyClass")

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.MethodBody && e.Symbol.LogicalName = ".ctor")

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.MethodBody && e.Symbol.LogicalName = "Existing")

    [<Fact>]
    let ``adding field to struct stays rude even with all capabilities`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
[<Struct>]
type MyStruct =
    val mutable A: int
"""
        let updated_source = """
module Library
[<Struct>]
type MyStruct =
    val mutable A: int
    val mutable B: int
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Struct layouts are immutable under hot reload (runtime restriction, identical
        // for C#): the addition stays a TypeLayoutChange regardless of capabilities.
        let result = harness.DiffWith allCapabilities baseline updated

        Assert.Contains(result.RudeEdits, fun e -> e.Kind = RudeEditKind.TypeLayoutChange)

    // =========================================================================
    // Lambda occurrence model tests
    // Structured occurrence extraction, old/new alignment, and capture
    // compatibility classification in the typed-tree diff.
    // =========================================================================

    /// All lambda edits across all members of a diff result.
    let private allLambdaEdits (result: TypedTreeDiffResult) =
        result.LambdaEdits |> List.collect (fun memberEdits -> memberEdits.Edits)

    [<Fact>]
    let ``body edit inside lambda still produces method body edit`` () =
        // Regression pin: a pure body edit within an unchanged lambda set must remain an
        // ordinary MethodBody edit, now carrying a structured BodyEdited occurrence pair.
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let transform = fun x -> x + 1
    transform 41
"""
        let updated_source = """
module Library
let evaluate () =
    let transform = fun x -> x + 2
    transform 41
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.MethodBody, result.SemanticEdits[0].Kind)

        let lambdaEdit = Assert.Single(allLambdaEdits result)

        match lambdaEdit with
        | LambdaEdit.BodyEdited (baselineOcc, updatedOcc) ->
            Assert.Equal(0, baselineOcc.Id.Ordinal)
            Assert.Equal(0, updatedOcc.Id.Ordinal)
            Assert.Equal(baselineOcc.CurriedArity, updatedOcc.CurriedArity)
        | other -> failwithf "Expected BodyEdited, got %A" other

    [<Fact>]
    let ``adding a lambda produces structured rude edit naming the added ordinal`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let double = fun x -> x * 2
    double 21
"""
        let updated_source = """
module Library
let evaluate () =
    let double = fun x -> x * 2
    let offset = fun x -> x + 1
    double 21 + offset 0
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // BaselineOnly capabilities: the addition is a supported EDIT KIND but exceeds
        // the runtime's capabilities (C# parity: RudeEditKind.NotSupportedByRuntime
        // naming the missing capability — the new closure class needs NewTypeDefinition).
        let result = harness.Diff baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("NewTypeDefinition", rudeEdit.Message)
        Assert.Contains("1 lambda added at ordinal 1", rudeEdit.Message)

        // The structured payload names the added occurrence; the surviving occurrence is
        // aligned, not reported as removed+added.
        let addedEdit = Assert.Single(allLambdaEdits result)

        match addedEdit with
        | LambdaEdit.Added updatedOcc ->
            Assert.Equal(1, updatedOcc.Id.Ordinal)
            Assert.Empty(updatedOcc.Id.ParentChain)
        | other -> failwithf "Expected Added, got %A" other

        // With the new-type and method capabilities the same edit is allowed: the member
        // body update covers it (the delta emitter emits the new closure TypeDef).
        let allowedCapabilities = EditAndContinueCapabilities.Parse [ "NewTypeDefinition"; "AddMethodToExistingType" ]
        let allowed = harness.DiffWith allowedCapabilities baseline updated

        Assert.Empty(allowed.RudeEdits)
        Assert.Single(allowed.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.MethodBody, allowed.SemanticEdits[0].Kind)

    [<Fact>]
    let ``removing a lambda produces structured rude edit naming the removed ordinal`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let double = fun x -> x * 2
    let offset = fun x -> x + 1
    double 21 + offset 0
"""
        let updated_source = """
module Library
let evaluate () =
    let double = fun x -> x * 2
    double 21
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // Removed-only lambda sets need no new metadata (C# parity: deleted lambda bodies
        // just become unreachable; the baseline closure class stays in place, unused), so
        // the member body update covers the edit even at Baseline capabilities.
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.MethodBody, result.SemanticEdits[0].Kind)

        let removedEdit = Assert.Single(allLambdaEdits result)

        match removedEdit with
        | LambdaEdit.Removed baselineOcc -> Assert.Equal(1, baselineOcc.Id.Ordinal)
        | other -> failwithf "Expected Removed, got %A" other

    [<Fact>]
    let ``reordering distinguishable lambdas aligns occurrences without rude edit`` () =
        // Reordering does not change the lambda set: the LCS alignment matches the
        // surviving occurrences instead of reporting a spurious removed+added pair.
        // (Two IDENTICAL occurrences that are reordered align positionally by construction;
        // that is intentional, since indistinguishable closures are interchangeable.)
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate (flag: bool) =
    let single = fun x -> x + 1
    let curried = fun x y -> x + y
    if flag then single 1 else curried 1 2
"""
        let updated_source = """
module Library
let evaluate (flag: bool) =
    let curried = fun x y -> x + y
    let single = fun x -> x + 1
    if flag then single 1 else curried 1 2
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        // No rude edit: the lambda set is unchanged. (Any body-level edit would surface as
        // an ordinary MethodBody edit; the whole-body hash treats adjacent independent lets
        // as order-insensitive, so a pure swap may produce no semantic edit at all.)
        Assert.Empty(result.RudeEdits)

        for edit in result.SemanticEdits do
            Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)

        // Both occurrences survive the reordering: nothing may be classified added/removed.
        let edits = allLambdaEdits result

        Assert.DoesNotContain(
            edits,
            fun edit ->
                match edit with
                | LambdaEdit.Added _
                | LambdaEdit.Removed _ -> true
                | _ -> false
        )

    [<Fact>]
    let ``nested lambda addition keeps outer occurrence matched`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let outer = fun x -> x + 1
    outer 1
"""
        let updated_source = """
module Library
let evaluate () =
    let outer = fun x ->
        let inner = fun y -> y + x
        inner x + 1
    outer 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("1 lambda added at ordinal 1", rudeEdit.Message)

        let edits = allLambdaEdits result

        // The outer occurrence is matched (its body changed), never removed.
        Assert.DoesNotContain(
            edits,
            fun edit ->
                match edit with
                | LambdaEdit.Removed _ -> true
                | _ -> false
        )

        // The inner occurrence is Added with the outer occurrence in its parent chain.
        let added =
            edits
            |> List.pick (function
                | LambdaEdit.Added occ -> Some occ
                | _ -> None)

        Assert.Equal(1, added.Id.Ordinal)
        Assert.Equal<int list>([ 0 ], added.Id.ParentChain)

    [<Fact>]
    let ``capture rename produces capture-set rude edit`` () =
        // C# parity: RenamingCapturedVariable is a permanent rude edit.
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let seed = 10
    let transform = fun x -> x + seed
    transform 1
"""
        let updated_source = """
module Library
let evaluate () =
    let start = 10
    let transform = fun x -> x + start
    transform 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, rudeEdit.Kind)
        Assert.Contains("RenamingCapturedVariable", rudeEdit.Message)
        Assert.Contains("'seed'", rudeEdit.Message)
        Assert.Contains("'start'", rudeEdit.Message)

        let lambdaEdit = Assert.Single(allLambdaEdits result)

        match lambdaEdit with
        | LambdaEdit.CaptureSetChanged (_, _, changes) ->
            let change = Assert.Single(changes)

            match change with
            | CaptureSetChange.Renamed (oldName, newName, _) ->
                Assert.Equal("seed", oldName)
                Assert.Equal("start", newName)
            | other -> failwithf "Expected Renamed, got %A" other
        | other -> failwithf "Expected CaptureSetChanged, got %A" other

    [<Fact>]
    let ``capture type change produces capture-set rude edit`` () =
        // C# parity: ChangingCapturedVariableType is a permanent rude edit.
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let seed = 10
    let transform = fun (x: int) -> x + seed
    transform 1
"""
        let updated_source = """
module Library
let evaluate () =
    let seed = 10L
    let transform = fun (x: int) -> x + int seed
    transform 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, rudeEdit.Kind)
        Assert.Contains("ChangingCapturedVariableType", rudeEdit.Message)
        Assert.Contains("'seed'", rudeEdit.Message)

        let lambdaEdit = Assert.Single(allLambdaEdits result)

        match lambdaEdit with
        | LambdaEdit.CaptureSetChanged (_, _, changes) ->
            let change = Assert.Single(changes)

            match change with
            | CaptureSetChange.TypeChanged (name, _, _) -> Assert.Equal("seed", name)
            | other -> failwithf "Expected TypeChanged, got %A" other
        | other -> failwithf "Expected CaptureSetChanged, got %A" other

    [<Fact>]
    let ``capturing an additional value produces capture-set rude edit`` () =
        // Additive captures are rude today; the message records that they may become
        // applicable via the AddInstanceFieldToExistingType capability (Roslyn emits a new
        // closure field for them).
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let evaluate () =
    let first = 1
    let transform = fun x -> x + first
    transform 1
"""
        let updated_source = """
module Library
let evaluate () =
    let first = 1
    let second = 2
    let transform = fun x -> x + first + second
    transform 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, rudeEdit.Kind)
        Assert.Contains("'second'", rudeEdit.Message)
        Assert.Contains("AddInstanceFieldToExistingType", rudeEdit.Message)

        let lambdaEdit = Assert.Single(allLambdaEdits result)

        match lambdaEdit with
        | LambdaEdit.CaptureSetChanged (_, _, changes) ->
            let change = Assert.Single(changes)

            match change with
            | CaptureSetChange.CaptureAdded capture -> Assert.Equal("second", capture.LogicalName)
            | other -> failwithf "Expected CaptureAdded, got %A" other
        | other -> failwithf "Expected CaptureSetChanged, got %A" other

    [<Fact>]
    let ``lambda inside added function stays an insert edit`` () =
        // Lambdas inside newly added members are part of the Insert edit, not lambda edits
        // on existing members.
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let existing () = 1
"""
        let updated_source = """
module Library
let existing () = 1
let added (values: int list) = values |> List.map (fun v -> v + 1)
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits)
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)
        Assert.Empty(allLambdaEdits result)

    // =========================================================================
    // Member addition coverage consolidation (field/property/event negative gating)
    // =========================================================================

    [<Fact>]
    let ``adding readonly property without method capability names the missing capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type MyClass() =
    member this.Existing = 1
"""
        let updated_source = """
module Library
type MyClass() =
    member this.Existing = 1
    member this.NewReadonly = 42
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // A getter-only property is a pure accessor-method addition (no backing field):
        // only the method capability gates it, and its absence must be named.
        let fieldOnly =
            EditAndContinueCapabilities.Parse [ "Baseline"; "AddInstanceFieldToExistingType" ]

        let result = harness.DiffWith fieldOnly baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("AddMethodToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding CLIEvent without method capability names the missing capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
"""
        let updated_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    let newEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
    [<CLIEvent>]
    member this.NewEvent = newEvent.Publish
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // The backing field is allowed (field capability present); the add_/remove_
        // accessor additions must name the missing method capability.
        let fieldOnly =
            EditAndContinueCapabilities.Parse [ "Baseline"; "AddInstanceFieldToExistingType" ]

        let result = harness.DiffWith fieldOnly baseline updated

        Assert.NotEmpty(result.RudeEdits)
        Assert.All(result.RudeEdits, fun e -> Assert.Equal(RudeEditKind.NotSupportedByRuntime, e.Kind))

        Assert.Contains(
            result.RudeEdits,
            fun e -> e.Message.Contains("AddMethodToExistingType"))

    [<Fact>]
    let ``adding CLIEvent with capabilities produces accessor inserts and field edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
"""
        let updated_source = """
module Library
open System

type MyClass() =
    let existingEvent = Event<EventHandler, EventArgs>()
    let newEvent = Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.ExistingEvent = existingEvent.Publish
    [<CLIEvent>]
    member this.NewEvent = newEvent.Publish
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType"; "AddInstanceFieldToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)

        let insertNames =
            result.SemanticEdits
            |> List.filter (fun e -> e.Kind = SemanticEditKind.Insert)
            |> List.map (fun e -> e.Symbol.LogicalName)
            |> List.sort

        // add_/remove_ accessors plus the typed-tree get_NewEvent PropertyGet symbol
        // (which has no IL counterpart for [<CLIEvent>] members and is ignored by the
        // delta builder); the backing field surfaces as the TypeDefinition edit.
        Assert.Equal<string list>([ "add_NewEvent"; "get_NewEvent"; "remove_NewEvent" ], insertNames)

        Assert.Contains(
            result.SemanticEdits,
            fun e -> e.Kind = SemanticEditKind.TypeDefinition && e.Symbol.LogicalName = "MyClass")

    // =========================================================================
    // Generic edit gating
    // Roslyn parity (AbstractEditAndContinueAnalyzer): updating a member that is
    // generic or declared in a generic type requires GenericUpdateMethod
    // (RudeEditKind.UpdatingGenericNotSupportedByRuntime); additions in a generic
    // context additionally require GenericAddMethodToExistingType /
    // GenericAddFieldToExistingType.
    // =========================================================================

    [<Fact>]
    let ``generic function body edit without GenericUpdateMethod produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\nlet identity<'T> (x: 'T) = x\n"
        let updated_source = "module Library\nlet identity<'T> (x: 'T) = let y = x in y\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // Session default (BaselineOnly): the runtime did not advertise GenericUpdateMethod.
        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("GenericUpdateMethod", rudeEdit.Message)

    [<Fact>]
    let ``generic function body edit with GenericUpdateMethod produces method body edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\nlet identity<'T> (x: 'T) = x\n"
        let updated_source = "module Library\nlet identity<'T> (x: 'T) = let y = x in y\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "GenericUpdateMethod" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits)
        Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)

    [<Fact>]
    let ``auto-generalized function body edit without GenericUpdateMethod produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        // No explicit typars: automatic generalization still compiles a generic method.
        let baseline_source = "module Library\nlet pair x y = (x, y)\n"
        let updated_source = "module Library\nlet pair x y = let t = (x, y) in t\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(
            result.RudeEdits,
            fun rude -> rude.Kind = RudeEditKind.NotSupportedByRuntime && rude.Message.Contains "GenericUpdateMethod"
        )

    [<Fact>]
    let ``generic class member body edit without GenericUpdateMethod produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Describe() = "Hello " + value.ToString()
"""
        let updated_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Describe() = "Welcome " + value.ToString()
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("GenericUpdateMethod", rudeEdit.Message)

    [<Fact>]
    let ``generic class member body edit with GenericUpdateMethod produces method body edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Describe() = "Hello " + value.ToString()
"""
        let updated_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Describe() = "Welcome " + value.ToString()
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "GenericUpdateMethod" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits)
        Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)

    [<Fact>]
    let ``non-generic body edit needs no GenericUpdateMethod capability`` () =
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\nlet value () = 1\n"
        let updated_source = "module Library\nlet value () = 2\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // BaselineOnly still applies plain method-body edits of non-generic members.
        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits)
        Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)

    [<Fact>]
    let ``adding generic function without GenericAddMethodToExistingType produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = "module Library\nlet value () = 1\n"
        let updated_source = "module Library\nlet value () = 1\nlet pair x y = (x, y)\n"

        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        // AddMethodToExistingType alone is not enough for a generic method.
        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("GenericAddMethodToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding method to generic class without GenericAddMethodToExistingType produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Get() = value
"""
        let updated_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Get() = value
    member _.GetAgain() = value
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "AddMethodToExistingType" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.NotSupportedByRuntime, rudeEdit.Kind)
        Assert.Contains("GenericAddMethodToExistingType", rudeEdit.Message)

    [<Fact>]
    let ``adding field to generic class without GenericAddFieldToExistingType produces NotSupportedByRuntime`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
type Container<'T>(value: 'T) =
    member _.Get() = value
"""
        let updated_source = """
module Library
type Container<'T>(value: 'T) =
    [<DefaultValue>] val mutable Extra: int
    member _.Get() = value
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let capabilities =
            EditAndContinueCapabilities.Parse [ "AddInstanceFieldToExistingType"; "GenericUpdateMethod" ]

        let result = harness.DiffWith capabilities baseline updated

        Assert.Contains(
            result.RudeEdits,
            fun rude ->
                rude.Kind = RudeEditKind.NotSupportedByRuntime
                && rude.Message.Contains "GenericAddFieldToExistingType"
        )

    // --- Parity characterization: structural edits to NON-struct-state-machine CEs ---
    // task {} lowers to a struct resumable state machine, so adding/removing a step is a
    // StateMachineShapeChange rude edit (see the task tests above). async/seq/inner functions do
    // NOT use struct resumable code, so the same structural edit should classify differently (and,
    // we expect, more permissively, matching C#). These tests pin that distinction. The positive
    // (Empty rude edits) assertions encode the expected C#-parity outcome; if F# is stricter than
    // predicted, the failure documents exactly where.

    [<Fact>]
    let ``async let-bang addition is not a struct state machine rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let runAsync () =
    async {
        let! value = async { return 1 }
        return value + 1
    }
"""
        let updated_source = """
module Library
let runAsync () =
    async {
        let! value = async { return 1 }
        let! extra = async { return 2 }
        return value + extra
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        // async lowers to continuation closures, not a struct resumable state machine: adding a
        // let! adds a continuation lambda (allowed with NewTypeDefinition/AddMethodToExistingType)
        // and edits the prior continuation body. Unlike task, this should NOT be rude.
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)
        Assert.Empty(result.RudeEdits)

    [<Fact>]
    let ``compiled type identity distinguishes equal display names`` () =
        use harness = new DiffTestHarness()

        let baselineSource = """
module Library
module A =
    type C() = class end
module B =
    type C() = class end
let value (x: A.C) = x
"""

        let updatedSource = baselineSource.Replace("(x: A.C)", "(x: B.C)")
        harness.Rewrite(baselineSource)
        let baseline = harness.Compile()
        harness.Rewrite(updatedSource)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.SignatureChange)

    [<Fact>]
    let ``match decision-tree change produces method body edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet value x = match x with | 0 -> 1 | _ -> 2\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet value x = match x with | 1 -> 1 | _ -> 2\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun edit -> edit.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    let ``body identity does not trust colliding diagnostic hashes`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "\"mvaxuhep\"")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.functionReturning "\"erytcfml\"")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun edit -> edit.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    let ``member accessibility change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype C() = member private _.M() = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype C() = member _.M() = 1\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.SignatureChange)

    [<Fact>]
    let ``type parameter constraint change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype I<'T when 'T :> System.IDisposable> = interface end\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype I<'T when 'T :> System.IComparable> = interface end\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.TypeLayoutChange)

    [<Fact>]
    let ``parameter attribute change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet value ([<System.Runtime.InteropServices.In>] x: int) = x\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet value ([<System.Runtime.InteropServices.Out>] x: int) = x\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.Unsupported)

    [<Fact>]
    let ``method generic arity excludes enclosing type parameters`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype C<'T>() = member _.M<'U>(x: 'U) = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype C<'T>() = member _.M<'U>(x: 'U) = 2\n")
        let updated = harness.Compile()

        let capabilities = EditAndContinueCapabilities.Parse [ "GenericUpdateMethod" ]
        let result = harness.DiffWith capabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun edit -> edit.Symbol.LogicalName = "M"))
        Assert.Equal(Some 1, edit.Symbol.GenericArity)
        assertRequiredCapabilities [ "Baseline"; "GenericUpdateMethod" ] result

    [<Fact>]
    let ``seq yield addition is not a struct state machine rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let numbers () =
    seq {
        yield 1
        yield 2
    }
"""
        let updated_source = """
module Library
let numbers () =
    seq {
        yield 1
        yield 2
        yield 3
    }
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        // seq {} lowers via the sequence-expression -> generated IEnumerable object path
        // (ConvertSequenceExprToObject), not resumable struct code. Adding a yield must not be a
        // StateMachineShapeChange; characterizing whether it lands as an allowed body update or a
        // synthesized-declaration change is the point of this test.
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)

    [<Fact>]
    let ``adding an inner function is not a struct state machine rude edit`` () =
        use harness = new DiffTestHarness()
        let baseline_source = """
module Library
let compute (x: int) =
    x + 1
"""
        let updated_source = """
module Library
let compute (x: int) =
    let helper y = y * 2
    helper x + 1
"""
        harness.Rewrite(baseline_source)
        let baseline = harness.Compile()
        harness.Rewrite(updated_source)
        let updated = harness.Compile()

        let result = harness.DiffWith allCapabilities baseline updated

        // An inner function lowers to a closure/local method, the same family as a lambda: adding
        // one should be an additive (allowed) edit with full capabilities, never a state machine
        // shape change.
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.StateMachineShapeChange)
        Assert.Empty(result.RudeEdits)

    [<Fact>]
    let ``attribute and parameter updates report their runtime capabilities`` () =
        use harness = new DiffTestHarness()

        harness.Rewrite(
            "module Library\n[<System.Obsolete(\"old\")>]\nlet value (oldName: int) = oldName + 1\n"
        )

        let baseline = harness.Compile()

        harness.Rewrite(
            "module Library\n[<System.Obsolete(\"new\")>]\nlet value (newName: int) = newName + 1\n"
        )

        let updated = harness.Compile()
        let result = harness.DiffWith allCapabilities baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore

        assertRequiredCapabilities
            [
                "Baseline"
                "ChangeCustomAttributes"
                "UpdateParameters"
            ]
            result
