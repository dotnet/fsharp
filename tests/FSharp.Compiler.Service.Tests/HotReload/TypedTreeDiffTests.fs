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

// Internal (not private) so the C3 closure-name-allocator tests can reuse the same
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
            | None -> failwithf "Could not locate Library implementation file. Available files: %A" (implFiles |> List.map (fun (CheckedImplFile(qualifiedNameOfFile = qname)) -> qname.Text))

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

    /// The full capability set advertised by a modern CoreCLR runtime; addition tests pass it
    /// explicitly because the diff defaults to the conservative baseline-only set.
    let private allCapabilities =
        EditAndContinueCapabilities.Parse [
            "Baseline"
            "AddMethodToExistingType"
            "AddStaticFieldToExistingType"
            "AddInstanceFieldToExistingType"
            "NewTypeDefinition"
            "ChangeCustomAttributes"
            "UpdateParameters"
            "GenericAddMethodToExistingType"
            "GenericUpdateMethod"
            "GenericAddFieldToExistingType"
            "AddExplicitInterfaceImplementation"
            "AddFieldRva"
        ]

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        let result = harness.DiffWith allCapabilities baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun e -> e.Symbol.LogicalName = "newCounter"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)

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

        let result = harness.DiffWith allCapabilities baseline updated

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

        // Phase B2: instance fields on classes are capability-gated. A [<DefaultValue>]
        // val mutable produces no binding/constructor change, so the addition surfaces as
        // a single TypeDefinition edit (the emitter discovers the new Field row from the
        // fresh compile); the symbol path mirrors the IL type name.
        let result = harness.DiffWith allCapabilities baseline updated

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
        let result = harness.DiffWith allCapabilities baseline updated

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
    // Lambda occurrence model tests (Phase C1)
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
        // body update covers it (Phase C4 emits the new closure TypeDef in the delta).
        let allowed = harness.DiffWith allCapabilities baseline updated

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
        // Additive captures are rude today; the message records that Phase C4 may allow
        // them via the AddInstanceFieldToExistingType capability (Roslyn emits a new
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
