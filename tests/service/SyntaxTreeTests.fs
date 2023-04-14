module Tests.Service.SyntaxTree

open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open NUnit.Framework

let testCasesDir = Path.Combine(__SOURCE_DIRECTORY__, "data", "SyntaxTree")

let allTestCases =
    Directory.EnumerateFiles(testCasesDir, "*.fs?", SearchOption.AllDirectories)
    |> Seq.map (fun f ->
        let fileInfo = FileInfo(f)
        let fileName = Path.Combine(fileInfo.Directory.Name, fileInfo.Name)
        [| fileName :> obj |])
    |> Seq.toArray

[<Literal>]
let RootDirectory = @"/root"

/// <summary>
/// Everytime `__SOURCE_DIRECTORY__` was used in the code, the ast will contain an invalid value and range for it.
/// This should be cleaned up when the test runs during CI/CD.
/// </summary>
/// <remarks>
/// This function is incomplete and does not clean up the entire ParsedInput.
/// A shortcut was made to only support the existing use-cases.
/// </remarks>
let private sanitizeAST (sourceDirectoryValue: string) (ast: ParsedInput) : ParsedInput =
    let isZero (m: range) =
        m.StartLine = 0 && m.StartColumn = 0 && m.EndLine = 0 && m.EndColumn = 0

    // __SOURCE_DIRECTORY__ will contain the evaluated value, so we want to replace it with a stable value instead.
    let mapParsedHashDirective (ParsedHashDirective(ident, args, _) as phd) =
        match args with
        | [ ParsedHashDirectiveArgument.SourceIdentifier("__SOURCE_DIRECTORY__", _, mSourceDirectory) ] ->
            let mZero =
                Range.mkRange mSourceDirectory.FileName (Position.mkPos 0 0) (Position.mkPos 0 0)

            ParsedHashDirective(
                ident,
                [ ParsedHashDirectiveArgument.SourceIdentifier("__SOURCE_DIRECTORY__", sourceDirectoryValue, mZero) ],
                mZero
            )
        | _ -> phd

    let (|SourceDirectoryConstant|_|) (constant: SynConst) =
        match constant with
        | SynConst.SourceIdentifier("__SOURCE_DIRECTORY__", _, mSourceDirectory) ->
            let mZero =
                Range.mkRange mSourceDirectory.FileName (Position.mkPos 0 0) (Position.mkPos 0 0)

            Some(SynConst.SourceIdentifier("__SOURCE_DIRECTORY__", sourceDirectoryValue, mZero), mZero)
        | _ -> None

    let (|SourceDirectoryConstantExpr|_|) (expr: SynExpr) =
        match expr with
        | SynExpr.Const(SourceDirectoryConstant(constant, mZero), _) -> Some(SynExpr.Const(constant, mZero))
        | _ -> None

    let rec mapSynModuleDecl (mdl: SynModuleDecl) =
        match mdl with
        | SynModuleDecl.HashDirective(ParsedHashDirective(range = mZero) as hd, m) ->
            let hd = mapParsedHashDirective hd
            // Only update the range of SynModuleSigDecl.HashDirective if the value was updated.
            let m = if isZero mZero then mZero else m
            SynModuleDecl.HashDirective(hd, m)
        | SynModuleDecl.NestedModule(moduleInfo, isRecursive, decls, isContinuing, range, trivia) ->
            SynModuleDecl.NestedModule(moduleInfo, isRecursive, List.map mapSynModuleDecl decls, isContinuing, range, trivia)
        | SynModuleDecl.Expr(SourceDirectoryConstantExpr(expr), _) -> SynModuleDecl.Expr(expr, expr.Range)
        | _ -> mdl

    let mapSynModuleOrNamespace (SynModuleOrNamespace(longId, isRecursive, kind, decls, xmlDoc, attribs, ao, range, trivia)) =
        SynModuleOrNamespace(longId, isRecursive, kind, List.map mapSynModuleDecl decls, xmlDoc, attribs, ao, range, trivia)

    let rec mapSynModuleDeclSig (msdl: SynModuleSigDecl) =
        match msdl with
        | SynModuleSigDecl.HashDirective(ParsedHashDirective(range = mZero) as hd, m) ->
            let hd = mapParsedHashDirective hd
            // Only update the range of SynModuleSigDecl.HashDirective if the value was updated.
            let m = if isZero mZero then mZero else m
            SynModuleSigDecl.HashDirective(hd, m)
        | SynModuleSigDecl.NestedModule(moduleInfo, isRecursive, decls, range, trivia) ->
            SynModuleSigDecl.NestedModule(moduleInfo, isRecursive, List.map mapSynModuleDeclSig decls, range, trivia)
        | _ -> msdl

    let mapSynModuleOrNamespaceSig (SynModuleOrNamespaceSig(longId, isRecursive, kind, decls, xmlDoc, attribs, ao, range, trivia)) =
        SynModuleOrNamespaceSig(longId, isRecursive, kind, List.map mapSynModuleDeclSig decls, xmlDoc, attribs, ao, range, trivia)

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(fileName,
                                               isScript,
                                               qualifiedNameOfFile,
                                               scopedPragmas,
                                               hashDirectives,
                                               contents,
                                               flags,
                                               trivia,
                                               identifiers)) ->
        ParsedImplFileInput(
            fileName,
            isScript,
            qualifiedNameOfFile,
            scopedPragmas,
            List.map mapParsedHashDirective hashDirectives,
            List.map mapSynModuleOrNamespace contents,
            flags,
            trivia,
            identifiers
        )
        |> ParsedInput.ImplFile
    | ParsedInput.SigFile(ParsedSigFileInput(fileName, qualifiedNameOfFile, scopedPragmas, hashDirectives, contents, trivia, identifiers)) ->
        ParsedSigFileInput(
            fileName,
            qualifiedNameOfFile,
            scopedPragmas,
            List.map mapParsedHashDirective hashDirectives,
            List.map mapSynModuleOrNamespaceSig contents,
            trivia,
            identifiers
        )
        |> ParsedInput.SigFile

let parseSourceCode (name: string, code: string) =
    let location = Path.Combine(RootDirectory, name).Replace("\\", "/")

    let parseResults =
        checker.ParseFile(
            location,
            SourceText.ofString code,
            { FSharpParsingOptions.Default with
                SourceFiles = [| location |]
                IsExe = true }
        )
        |> Async.RunImmediate

    let tree = parseResults.ParseTree
    let sourceDirectoryValue = $"{RootDirectory}/{FileInfo(location).Directory.Name}"
    sanitizeAST sourceDirectoryValue tree, parseResults.Diagnostics

/// Asserts the parsed untyped tree matches the expected baseline.
///
/// To update a baseline:
///     CMD: set TEST_UPDATE_BSL=1 & dotnet test --filter "ParseFile"
///     PowerShell: $env:TEST_UPDATE_BSL = "1" ; dotnet test --filter "ParseFile"
///     Linux/macOS: export TEST_UPDATE_BSL=1 & dotnet test --filter "ParseFile"
///
/// Assuming your current directory is tests/FSharp.Compiler.Service.Tests
[<TestCaseSource(nameof allTestCases)>]
let ParseFile fileName =
    let fullPath = Path.Combine(testCasesDir, fileName)
    let contents = File.ReadAllText fullPath
    let ast, diagnostics = parseSourceCode (fileName, contents)
    let normalize (s: string) = s.Replace("\r", "")
    let actual =
        if Array.isEmpty diagnostics then
            $"%A{ast}"
        else
            let diagnostics =
                diagnostics
                |> Array.map (fun d ->
                    let severity =
                        match d.Severity with
                        | FSharpDiagnosticSeverity.Warning -> "warning"
                        | FSharpDiagnosticSeverity.Error -> "error"
                        | FSharpDiagnosticSeverity.Info -> "info"
                        | FSharpDiagnosticSeverity.Hidden -> "hidden"

                    $"(%d{d.StartLine},%d{d.StartColumn})-(%d{d.EndLine},%d{d.EndColumn}) %s{d.Subcategory} %s{severity} %s{d.Message}"
                )
                |> String.concat "\n"
            $"%A{ast}\n\n%s{diagnostics}"
        |> normalize
        |> sprintf "%s\n"
    let bslPath = $"{fullPath}.bsl"

    let expected =
        if File.Exists bslPath then
            File.ReadAllText bslPath |> normalize
        else
            "No baseline was found"

    let testUpdateBSLEnv = System.Environment.GetEnvironmentVariable("TEST_UPDATE_BSL")

    if not (isNull testUpdateBSLEnv) && testUpdateBSLEnv.Trim() = "1" then
        File.WriteAllText(bslPath, actual)
    elif expected <> actual then
        File.WriteAllText($"{fullPath}.tmp", actual)

    Assert.AreEqual(expected, actual)
