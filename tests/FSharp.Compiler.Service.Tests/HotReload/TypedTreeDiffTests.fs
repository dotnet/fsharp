namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Reflection
open FSharp.Compiler
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff

open FSharp.Compiler.Service.Tests.Common

type private DiffTestHarness() =
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
            | None -> failwithf "Could not locate Library implementation file. Available files: %A" (implFiles |> List.map (fun (CheckedImplFile(qualifiedNameOfFile = qname)) -> qname.Text))

        tcGlobals, implFile

    member _.Diff baseline updated =
        let tcGlobals, baselineImpl = baseline
        let _, updatedImpl = updated
        diffImplementationFile tcGlobals baselineImpl updatedImpl

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

module TypedTreeDiffTests =

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
        Assert.Equal(SemanticEditKind.MethodBody, result.SemanticEdits[0].Kind)

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

        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, result.RudeEdits[0].Kind)

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

        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.LambdaShapeChange)

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

        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.LambdaShapeChange, result.RudeEdits[0].Kind)

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
        Assert.Contains(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.LambdaShapeChange)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.DeclarationAdded)
        Assert.DoesNotContain(result.RudeEdits, fun rude -> rude.Kind = RudeEditKind.DeclarationRemoved)

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

        let result = harness.Diff baseline updated

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

        let result = harness.Diff baseline updated

        // Adding a static method should produce an Insert semantic edit
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

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

        let result = harness.Diff baseline updated

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

        let result = harness.Diff baseline updated

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

        let result = harness.Diff baseline updated

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

        let result = harness.Diff baseline updated

        Assert.NotEmpty(result.RudeEdits)
        let hasExplicitInterfaceRudeEdit =
            result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.InsertExplicitInterface)
        Assert.True(
            hasExplicitInterfaceRudeEdit,
            "Expected InsertExplicitInterface rude edit for adding explicit interface implementation"
        )

    [<Fact>]
    let ``adding module-level value produces rude edit`` () =
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

        let result = harness.Diff baseline updated

        // Adding a module-level value should still produce a rude edit
        Assert.NotEmpty(result.RudeEdits)
        Assert.Equal(RudeEditKind.DeclarationAdded, result.RudeEdits[0].Kind)

    // =========================================================================
    // Property Addition Tests
    // =========================================================================

    [<Fact>]
    let ``adding auto-property to class produces rude edit due to backing field`` () =
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

        let result = harness.Diff baseline updated

        // Adding an auto-property creates a backing field, which changes type layout
        // This is correctly detected as a rude edit (TypeLayoutChange)
        Assert.NotEmpty(result.RudeEdits)
        let layoutChange = result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.TypeLayoutChange)
        Assert.True(layoutChange, "Expected TypeLayoutChange rude edit for auto-property addition")

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

        let result = harness.Diff baseline updated

        // Adding a readonly property should produce an Insert semantic edit
        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore
        Assert.Equal(SemanticEditKind.Insert, result.SemanticEdits[0].Kind)

    // =========================================================================
    // Event Addition Tests
    // =========================================================================

    [<Fact>]
    let ``adding event with backing field produces rude edit due to type layout change`` () =
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

        // Adding an event with a backing field (let newEvent = ...) adds a field to the class,
        // which changes the type layout. This is correctly detected as a TypeLayoutChange rude edit.
        Assert.NotEmpty(result.RudeEdits)
        let hasLayoutChange = result.RudeEdits |> List.exists (fun e -> e.Kind = RudeEditKind.TypeLayoutChange)
        Assert.True(hasLayoutChange, "Expected TypeLayoutChange rude edit for event backing field")
