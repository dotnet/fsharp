namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Reflection
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.Text
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
        {
            checker.GetProjectOptionsFromCommandLineArgs(projPath, args) with
                SourceFiles = [| filePath |]
        }

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
                |> Array.choose (fun diag ->
                    if diag.Severity = FSharpDiagnosticSeverity.Error then
                        Some diag.Message
                    else
                        None)

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
            | None ->
                match implFiles with
                | [ single ] -> single
                | _ ->
                    let names =
                        implFiles
                        |> List.map (fun (CheckedImplFile(qualifiedNameOfFile = qname)) -> qname.Text)

                    failwithf "Could not locate Library implementation file. Available files: %A" names

        tcGlobals, implFile

    member _.Diff baseline updated =
        let tcGlobals, baselineImpl = baseline
        let _, updatedImpl = updated
        diffImplementationFile tcGlobals EditAndContinueCapabilities.All baselineImpl updatedImpl

    interface IDisposable with
        member _.Dispose() =
            try
                checker.InvalidateAll()
            with _ ->
                ()

            try
                Directory.Delete(projectDir, true)
            with _ ->
                ()

module private Sources =
    let moduleHeader = "module Library\n"

    let functionReturning value =
        $"{moduleHeader}let value () = {value}\n"

type TypedTreeDiffTests() =

    let assertRequiredCapabilities expected (result: TypedTreeDiffResult) =
        let actual = result.RequiredCapabilities |> List.map (fun capability -> capability.Name)
        Assert.Equal<string list>(expected, actual)

    [<Fact>]
    member _.``unchanged file produces no edits`` () =
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
    member _.``reference-equal implementation file uses fast path`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "1")
        let baseline = harness.Compile()

        let result = harness.Diff baseline baseline

        Assert.Empty(result.SemanticEdits)
        Assert.Empty(result.RudeEdits)

    [<Fact>]
    member _.``method body update produces semantic edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "1")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.functionReturning "2")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        let edit = Assert.Single(result.SemanticEdits)
        Assert.Empty(result.RudeEdits)
        Assert.Equal(SemanticEditKind.MethodBody, edit.Kind)
        Assert.Equal("value", edit.Symbol.LogicalName)
        assertRequiredCapabilities [ "Baseline" ] result

    [<Fact>]
    member _.``signature change produces rude edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet value (x: int) = x\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet value (x: string) = x.Length\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.SignatureChange, rudeEdit.Kind)

    [<Fact>]
    member _.``adding module function produces insert edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet existing () = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet existing () = 1\nlet added () = 2\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun edit -> edit.Symbol.LogicalName = "added"))
        Assert.Equal(SemanticEditKind.Insert, edit.Kind)
        assertRequiredCapabilities
            [
                "Baseline"
                "AddMethodToExistingType"
                "AddStaticFieldToExistingType"
            ]
            result

    [<Fact>]
    member _.``deleting module function produces declaration-removed rude edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet kept () = 1\nlet removed () = 2\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet kept () = 1\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        let rudeEdit = Assert.Single(result.RudeEdits)
        Assert.Equal(RudeEditKind.DeclarationRemoved, rudeEdit.Kind)
        Assert.Equal(Some "removed", rudeEdit.Symbol |> Option.map (fun symbol -> symbol.LogicalName))

    [<Fact>]
    member _.``record layout change produces type-layout rude edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype State = { Count: int }\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype State = { Count: int; Extra: int }\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(
            result.RudeEdits,
            fun rudeEdit ->
                rudeEdit.Kind = RudeEditKind.TypeLayoutChange
                && (rudeEdit.Symbol |> Option.exists (fun symbol -> symbol.LogicalName = "State"))
        )

    [<Fact>]
    member _.``logical-name arity keeps generic and non-generic entities distinct`` () =
        use harness = new DiffTestHarness()

        let baselineSource = """
module Library
type Expr = | NonGeneric of int
type Expr<'T> = | Generic of value: 'T
"""

        let updatedSource = """
module Library
type Expr = | NonGeneric of int
type Expr<'T> = | Generic of value: 'T * extra: 'T option
"""

        harness.Rewrite(baselineSource)
        let baseline = harness.Compile()
        harness.Rewrite(updatedSource)
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Contains(
            result.RudeEdits,
            fun rudeEdit ->
                rudeEdit.Kind = RudeEditKind.TypeLayoutChange
                && (rudeEdit.Symbol |> Option.exists (fun symbol -> symbol.LogicalName = "Expr`1"))
        )

        Assert.DoesNotContain(
            result.RudeEdits,
            fun rudeEdit ->
                rudeEdit.Kind = RudeEditKind.TypeLayoutChange
                && (rudeEdit.Symbol |> Option.exists (fun symbol -> symbol.LogicalName = "Expr"))
        )

    [<Fact>]
    member _.``compiled type identity distinguishes equal display names`` () =
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
    member _.``match decision-tree change produces method body edit`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet value x = match x with | 0 -> 1 | _ -> 2\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet value x = match x with | 1 -> 1 | _ -> 2\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun edit -> edit.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    member _.``body identity does not trust colliding diagnostic hashes`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite(Sources.functionReturning "\"mvaxuhep\"")
        let baseline = harness.Compile()
        harness.Rewrite(Sources.functionReturning "\"erytcfml\"")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Contains(result.SemanticEdits, fun edit -> edit.Kind = SemanticEditKind.MethodBody)

    [<Fact>]
    member _.``member accessibility change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype C() = member private _.M() = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype C() = member _.M() = 1\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.SignatureChange)

    [<Fact>]
    member _.``type parameter constraint change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype I<'T when 'T :> System.IDisposable> = interface end\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype I<'T when 'T :> System.IComparable> = interface end\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.TypeLayoutChange)

    [<Fact>]
    member _.``parameter attribute change fails closed`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\nlet value ([<System.Runtime.InteropServices.In>] x: int) = x\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\nlet value ([<System.Runtime.InteropServices.Out>] x: int) = x\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.SemanticEdits)
        Assert.Contains(result.RudeEdits, fun edit -> edit.Kind = RudeEditKind.Unsupported)

    [<Fact>]
    member _.``method generic arity excludes enclosing type parameters`` () =
        use harness = new DiffTestHarness()
        harness.Rewrite("module Library\ntype C<'T>() = member _.M<'U>(x: 'U) = 1\n")
        let baseline = harness.Compile()
        harness.Rewrite("module Library\ntype C<'T>() = member _.M<'U>(x: 'U) = 2\n")
        let updated = harness.Compile()

        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        let edit = Assert.Single(result.SemanticEdits |> List.filter (fun edit -> edit.Symbol.LogicalName = "M"))
        Assert.Equal(Some 1, edit.Symbol.GenericArity)
        assertRequiredCapabilities [ "Baseline"; "GenericUpdateMethod" ] result

    [<Fact>]
    member _.``attribute and parameter updates report their runtime capabilities`` () =
        use harness = new DiffTestHarness()

        harness.Rewrite(
            "module Library\n[<System.Obsolete(\"old\")>]\nlet value (oldName: int) = oldName + 1\n"
        )

        let baseline = harness.Compile()

        harness.Rewrite(
            "module Library\n[<System.Obsolete(\"new\")>]\nlet value (newName: int) = newName + 1\n"
        )

        let updated = harness.Compile()
        let result = harness.Diff baseline updated

        Assert.Empty(result.RudeEdits)
        Assert.Single(result.SemanticEdits) |> ignore

        assertRequiredCapabilities
            [
                "Baseline"
                "ChangeCustomAttributes"
                "UpdateParameters"
            ]
            result
