[<FSharp.Test.RunTestCasesInSequence>]
module FSharp.Compiler.Service.Tests.ScriptDiagnosticsTests

open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

let private closure (files: (string * string) list) (active: string) : FSharpDiagnostic[] =
    let dir = Path.Combine(Path.GetTempPath(), "sdt_" + Guid.NewGuid().ToString("N"))
    Directory.CreateDirectory(dir) |> ignore
    try
        for (name, content) in files do
            File.WriteAllText(Path.Combine(dir, name), content)
        let activePath = Path.Combine(dir, active)
        let source = File.ReadAllText activePath
        let options, _ =
#if NETCOREAPP
            checker.GetProjectOptionsFromScript(activePath, SourceText.ofString source, assumeDotNetFramework = false, useSdkRefs = true) |> Async.RunImmediate
#else
            checker.GetProjectOptionsFromScript(activePath, SourceText.ofString source) |> Async.RunImmediate
#endif
        let results = checker.ParseAndCheckProject(options) |> Async.RunImmediate
        results.Diagnostics
    finally
        try Directory.Delete(dir, true) with _ -> ()

let private distinctDiags (diags: FSharpDiagnostic[]) =
    diags
    |> Array.map (fun d -> formatDiagnostic d, normalizeDiagnosticMessage d)
    |> Array.distinctBy fst

let private closureDump (diags: FSharpDiagnostic[]) =
    distinctDiags diags |> Array.map fst |> String.concat "\n"

let private assertClosureNoDiagnostics (diags: FSharpDiagnostic[]) =
    if diags.Length > 0 then
        failwithf "Expected no diagnostics, but got %d:\n%s" diags.Length (closureDump diags)

let private assertClosureContains (text: string) (diags: FSharpDiagnostic[]) =
    let errors = diags |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    let msgs = distinctDiags errors |> Array.map snd
    if not (msgs |> Array.exists (fun m -> m.Contains text)) then
        failwithf "Expected an ERROR diagnostic containing %A, but got %d:\n%s" text diags.Length (closureDump diags)

let private assertClosureContainsAll (parts: string list) (diags: FSharpDiagnostic[]) =
    let errors = diags |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    let msgs = distinctDiags errors |> Array.map snd
    if not (msgs |> Array.exists (fun m -> parts |> List.forall (fun p -> m.Contains p))) then
        failwithf "Expected a single ERROR diagnostic containing all of %A, but got %d:\n%s" parts diags.Length (closureDump diags)

let private assertClosureWarningContains (text: string) (diags: FSharpDiagnostic[]) =
    let warnings = diags |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Warning)
    let msgs = distinctDiags warnings |> Array.map snd
    if not (msgs |> Array.exists (fun m -> m.Contains text)) then
        failwithf "Expected a WARNING containing %A, but got %d diagnostic(s):\n%s" text diags.Length (closureDump diags)

let private assertClosureExactlyOneContaining (text: string) (diags: FSharpDiagnostic[]) =
    let matching = distinctDiags diags |> Array.filter (fun (_, m) -> m.Contains text)
    if matching.Length <> 1 then
        failwithf "Expected exactly one diagnostic containing %A, but %d of %d matched:\n%s"
            text matching.Length diags.Length (closureDump diags)

let private fooFs = "namespace Namespace\ntype Foo = \n     static member public Property = 0\n"
let private fooFsi = "namespace Namespace\ntype Foo =\n  class\n    static member Property : int\n  end\n"
let private fooFsHidden = "namespace Namespace\ntype Foo = \n     static member public HiddenProperty = 0\n     static member public Property = 0\n"
let private myNamespaceFs = "namespace MyNamespace\n    module MyModule =\n        let x = 1\n"

[<Fact>]
let ``Squiggles.ShowInFsxFiles`` () =
    let _, checkResults = getParseAndCheckResults "open Thing1.Thing2"
    assertDiagnosticsContain "The namespace or module 'Thing1' is not defined" checkResults

[<FactForDESKTOP>]
let ``Hash.RProperSquiggleForNonExistentFile`` () =
    let _, checkResults = getParseAndCheckResults "#r \"NonExistent\" "
    assertDiagnosticsContain "'NonExistent' was not found or is invalid" checkResults

[<FactForDESKTOP>]
let ``Hash.RDoesNotExist.Bug3325`` () =
    let _, checkResults = getParseAndCheckResults "#r \"ThisDLLDoesNotExist\" "
    assertDiagnosticsContain "'ThisDLLDoesNotExist' was not found or is invalid" checkResults

[<FactForDESKTOP>]
let ``ExactlyOneError.Bug4861`` () =
    let _, checkResults = getParseAndCheckResults "//\n#r \"Nonexistent\"\n"
    assertDiagnosticCount 1 checkResults
    assertDiagnosticsContain "Nonexistent" checkResults

[<Fact(Skip = "the FCS script checker does not surface a missing '#load' target as a diagnostic")>]
let ``InvalidHashLoad.ShouldBeASquiggle.Bug3012`` () =
    let diags = closure [ "Test.fsx", "\n#load \"Bar.fs\"\n" ] "Test.fsx"
    assertClosureContains "Bar.fs" diags

[<Fact>]
let ``HashLoad.Added`` () =
    let _, checkResults = getParseAndCheckResults "//#load \"File1.fs\"\nopen MyNamespace.MyModule\nprintfn \"%d\" x\n"
    assertDiagnosticsContain "MyNamespace" checkResults

[<Fact>]
let ``HashR.Removed`` () =
    let _, checkResults = getParseAndCheckResults "#r \"System.Transactions.dll\"\nopen System.Transactions\n"
    assertNoDiagnostics checkResults

[<FactForDESKTOP>]
let ``HashR.AddedIn`` () =
    let _, checkResults = getParseAndCheckResults "//#r \"System.Transactions.dll\"\nopen System.Transactions\n"
    assertDiagnosticsContain "'Transactions' is not defined" checkResults

[<Fact>]
let ``NoError.HashR.DllWithNoPath`` () =
    let _, checkResults = getParseAndCheckResults "\n#r \"System.Transactions.dll\"\nopen System.Transactions"
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoError.HashR.BugDefaultReferenceFileIsAlsoResolved`` () =
    let _, checkResults = getParseAndCheckResults "\n#r \"System\"\n"
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoError.HashR.DoubleReference`` () =
    let _, checkResults = getParseAndCheckResults "\n#r \"System\"\n#r \"System\"\n"
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoError.HashR.ResolveFromGAC`` () =
    let _, checkResults = getParseAndCheckResults "\n#r \"CustomMarshalers\"\n"
    assertNoDiagnostics checkResults

[<Fact>]
let ``NoError.HashR.ResolveFromFullyQualifiedPath`` () =
    let path = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "System.configuration.dll")
    let _, checkResults = getParseAndCheckResults (sprintf "#r @\"%s\"" path)
    assertNoDiagnostics checkResults

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``NoError.HashR.RelativePath1`` () = ()

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``NoError.HashR.RelativePath2`` () = ()

[<FactForDESKTOP>]
let ``NoError.AutomaticImportsForFsxFiles`` () =
    let _, checkResults =
        getParseAndCheckResults
            "\nopen System\nopen System.Xml\nopen System.Drawing\nopen System.Runtime.Remoting\nopen System.Runtime.Serialization.Formatters.Soap\nopen System.Data\nopen System.Drawing\nopen System.Web\nopen System.Web.Services\nopen System.Windows.Forms"
    assertNoDiagnostics checkResults

[<Theory>]
[<InlineData("#r \"JoeBob\"\n")>]
[<InlineData("#I \".\"\n")>]
[<InlineData("#load \"Dooby\"\n")>]
let ``HashDirectivesAreErrors.InNonScriptFiles`` (directive: string) =
    assertDiagnosticsContain "may only be used in F# script files" (checkAsFsFile directive)

[<FactForDESKTOP>]
let ``InvalidHashReference.ShouldBeASquiggle.Bug3012`` () =
    let _, checkResults = getParseAndCheckResults "#r \"Bar.dll\""
    assertDiagnosticsContain "'Bar.dll' was not found or is invalid" checkResults

[<Fact(Skip = "non-FCS: needs a pre-built bin output fixture and the FCS script checker does not surface '#r' resolution as a diagnostic, so assertNoDiagnostics would be vacuous.")>]
let ``ScriptCanReferenceBinDirectoryOutput.Bug3151`` () =
    let _, checkResults = getParseAndCheckResults "#reference @\"bin\\Debug\\testproject.exe\"\n"
    assertNoDiagnostics checkResults

[<Fact(Skip = "the FCS script checker does not report a '#reference' resolution error")>]
let ``HashReferenceAgainstNonAssemblyExe`` () =
    let path = Path.Combine(Environment.GetEnvironmentVariable("windir"), "notepad.exe")
    let _, checkResults = getParseAndCheckResults (sprintf "#reference @\"%s\"\n" path)
    assertDiagnosticsContain "was not found or is invalid" checkResults

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.UnitsOfMeasure.SmokeTest1`` () = ()

[<Fact>]
let ``ScriptClosure.TransitiveLoad1`` () =
    closure
        [ "File1.fs", fooFs
          "Script2.fsx", "#load \"File1.fs\"\n"
          "Script1.fsx", "#load \"Script2.fsx\"\nNamespace.Foo.Property\n" ]
        "Script1.fsx"
    |> assertClosureNoDiagnostics

[<Fact>]
let ``ScriptClosure.TransitiveLoad2`` () =
    closure
        [ "File1.fs", fooFs
          "Script2.fsx", "#load \"File1.fs\"\n"
          "Script1.fsx", "#load \"Script2.fsx\"\nNamespace.Foo.NonExistingProperty\n" ]
        "Script1.fsx"
    |> assertClosureExactlyOneContaining "NonExistingProperty"

[<Fact>]
let ``HashLoad.Removed`` () =
    closure
        [ "File1.fs", myNamespaceFs
          "File2.fsx", "#load \"File1.fs\"\nopen MyNamespace.MyModule\nprintfn \"%d\" x\n" ]
        "File2.fsx"
    |> assertClosureNoDiagnostics

[<Fact>]
let ``NoError.ScriptClosure.TransitiveLoad16`` () =
    closure
        [ "ThisProject.fsx", "#nowarn \"44\"\n"
          "Script1.fsx", "#load \"ThisProject.fsx\"\n[<System.Obsolete(\"x\")>]\nlet fn x = 0\nlet y = fn 1\n" ]
        "Script1.fsx"
    |> assertClosureExactlyOneContaining "This construct is deprecated. x"

[<Fact>]
let ``NoError.HashLoad.Simple`` () =
    closure
        [ "File1.fs", myNamespaceFs
          "File2.fsx", "#load \"File1.fs\"\nopen MyNamespace.MyModule\nprintfn \"%d\" x\n" ]
        "File2.fsx"
    |> assertClosureNoDiagnostics

[<Fact>]
let ``NoWarn.OnLoadedFile.Bug4837`` () =
    closure
        [ "File1.fs", "module File1Module\nlet x = System.DateTime.Now - System.DateTime.Now\nx.Add(x) |> ignore\n"
          "File2.fsx", "#load \"File1.fs\"\n" ]
        "File2.fsx"
    |> assertClosureNoDiagnostics

[<Fact>]
let ``ExactlyOneError.ScriptClosure.TransitiveLoad15`` () =
    closure
        [ "File2.fs", "namespace Namespace\ntype Type() =\n    static member Property = 0\n"
          "File1.fs", "#load \"File2.fs\"\nnamespace File2Namespace\n"
          "Script1.fsx", "#load \"File1.fs\"\nNamespace.Type.Property\n" ]
        "Script1.fsx"
    |> assertClosureExactlyOneContaining "Namespace"

[<Fact>]
let ``ScriptClosure.TransitiveLoad14`` () =
    closure
        [ "Script2.fsx", "#load \"Script1.fsx\"\n#r \"NonExisting\"\n"
          "Script1.fsx", "#load \"Script2.fsx\"\n#r \"System\"\n" ]
        "Script1.fsx"
    |> assertClosureNoDiagnostics

[<Fact>]
let ``HashLoadedFileWithErrors.Bug3149`` () =
    closure
        [ "File1.fs", "module File1\nDogChow\n"
          "File2.fsx", "#load @\"File1.fs\"\n" ]
        "File2.fsx"
    |> assertClosureContains "DogChow"

[<Fact>]
let ``HashLoadedFileWithWarnings.Bug3149`` () =
    closure
        [ "File1.fs", "module File1Module\ntype WarningHere<'a> = static member X() = 0\nlet y = WarningHere.X\n"
          "File2.fsx", "#load @\"File1.fs\"\n" ]
        "File2.fsx"
    |> assertClosureWarningContains "WarningHere"

[<Fact>]
let ``HashLoadedFileWithErrors.Bug3652`` () =
    closure
        [ "File1.fs", "module File1\nlet a = 1 + \"\"\nlet c = new obj()\nlet b = c.foo()\n"
          "File2.fsx", "#load @\"File1.fs\"\n" ]
        "File2.fsx"
    |> assertClosureContainsAll [ "'string'"; "'int'" ]

[<Theory>]
[<InlineData 1123>]
[<InlineData 1139>]
let ``ScriptClosure.TransitiveLoad3and4`` (caseId: int) =
    let member', expected =
        match caseId with
        | 1123 -> "Property", null
        | _ -> "NonExistingProperty", "NonExistingProperty"
    let files =
        [ "File1.fs", fooFs
          "Script2.fsx", "#load \"Script1.fsx\"\n#load \"File1.fs\"\n"
          "Script1.fsx", sprintf "#load \"Script2.fsx\"\n#load \"File1.fs\"\nNamespace.Foo.%s\n" member' ]
    let diags = closure files "Script1.fsx"
    if isNull expected then assertClosureNoDiagnostics diags
    else assertClosureExactlyOneContaining expected diags

[<Theory>]
[<InlineData 1124>]
[<InlineData 1140>]
let ``ScriptClosure.TransitiveLoad9and5`` (caseId: int) =
    let member', expected =
        match caseId with
        | 1124 -> "Property", null
        | _ -> "HiddenProperty", "HiddenProperty"
    let files =
        [ "File1.fsi", fooFsi
          "File1.fs", fooFsHidden
          "Script1.fsx", sprintf "#load \"File1.fsi\"\n#load \"File1.fs\"\nNamespace.Foo.%s\n" member' ]
    let diags = closure files "Script1.fsx"
    if isNull expected then assertClosureNoDiagnostics diags
    else assertClosureExactlyOneContaining expected diags

[<Theory>]
[<InlineData 1125>]
[<InlineData 1127>]
[<InlineData 1141>]
[<InlineData 1143>]
let ``ScriptClosure.TransitiveLoad10_12_6_8`` (caseId: int) =
    let member', expected =
        match caseId with
        | 1125 | 1127 -> "Property", null
        | _ -> "HiddenProperty", "HiddenProperty"
    let script1, script2 =
        match caseId with
        | 1125 | 1141 ->
            "#load \"File1.fsi\"\n#load \"File1.fs\"\n",
            sprintf "#load \"Script1.fsx\"\nNamespace.Foo.%s\n" member'
        | _ ->
            "#load \"File1.fs\"\n",
            sprintf "#load \"File1.fsi\"\n#load \"Script1.fsx\"\nNamespace.Foo.%s\n" member'
    let files =
        [ "File1.fsi", fooFsi
          "File1.fs", fooFsHidden
          "Script1.fsx", script1
          "Script2.fsx", script2 ]
    let diags = closure files "Script2.fsx"
    if isNull expected then assertClosureNoDiagnostics diags
    else assertClosureExactlyOneContaining expected diags

[<Theory>]
[<InlineData 1126>]
[<InlineData 1142>]
let ``ScriptClosure.TransitiveLoad11and7`` (caseId: int) =
    let member', expected =
        match caseId with
        | 1126 -> "Property", null
        | _ -> "HiddenProperty", "HiddenProperty"
    let files =
        [ "File1.fsi", fooFsi
          "File1.fs", fooFsHidden
          "Script1.fsx", "#load \"File1.fsi\"\n"
          "Script2.fsx", sprintf "#load \"Script1.fsx\"\n#load \"File1.fs\"\nNamespace.Foo.%s\n" member' ]
    let diags = closure files "Script2.fsx"
    if isNull expected then assertClosureNoDiagnostics diags
    else assertClosureExactlyOneContaining expected diags

[<Fact>]
let ``Fsx.SyntheticTokens`` () =
    let _, checkResults = getParseAndCheckResults "#r \"\"\n#reference \"\"\n#load \"\"\n#line 52\n#nowarn 72\n"
    assertDiagnosticsContain "is not a valid assembly name" checkResults
    assertDiagnosticsContain "is not a valid filename" checkResults
    let errors = checkResults.Diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
    Assert.Empty(errors)

[<Theory>]
[<InlineData("#reference \"\n#reference \"Hello There\"\n")>]
[<InlineData("#r \"\n# \"Hello There\"\n")>]
[<InlineData("#load \"\n#load \"Hello There\"\n")>]
let ``Fsx.UnclosedHashReferenceOrLoad`` (source: string) =
    let _, checkResults = getParseAndCheckResults source
    assertDiagnosticsContain "End of file in string begun" checkResults
