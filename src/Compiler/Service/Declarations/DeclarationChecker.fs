// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis.Declarations

open System.Collections.Generic
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

[<AbstractClass; Sealed>]
type DeclarationChecker =

    static member SymbolKey(symbol: FSharpSymbol) : string option = DeclarationSymbolKeys.symbolKey symbol

    static member SignatureLocation
        (ctx: ProjectContext, signature: FileSignature, key: string, conformed: bool, ct: CancellationToken)
        : range option =
        DeclarationSymbolKeys.signatureLocation ctx signature key conformed ct

    static member SignatureUses(ctx: ProjectContext, signature: FileSignature) : (struct (range * bool * FSharpSymbol))[] =
        DeclarationSymbolKeys.signatureUses ctx signature

    static member DefinitionLocation(check: FSharpCheckFileResults, key: string, ct: CancellationToken) : range option =
        DeclarationSymbolKeys.definitionLocation check key ct

    static member CreateProject(options: FSharpProjectOptions) : Async<ProjectContext> =
        DeclarationProjectCheck.createProject options

    static member CreateProject
        (options: FSharpProjectOptions, references: IReadOnlyDictionary<string, ProjectAssembly>)
        : Async<ProjectContext> =
        DeclarationProjectCheck.createProjectWithReferences options references

    static member FailingProjectAssembly(message: string) : ProjectAssembly =
        DeclarationProjectCheck.failingProjectAssembly message

    static member ProjectAssembly(ctx: ProjectContext, files: FileResult list, signatures: FileSignature list) : ProjectAssembly =
        DeclarationProjectCheck.projectAssembly ctx files signatures

    static member Parse(ctx: ProjectContext, fileName: string, source: string) : FSharpParseFileResults =
        DeclarationSyntax.parse ctx fileName source

    static member CountDeclarations(parse: FSharpParseFileResults) : int =
        DeclarationSyntax.countDeclarations parse

    static member Split(parse: FSharpParseFileResults) : ParsedDeclaration[] = DeclarationSyntax.split parse

    static member Content(decl: ParsedDeclaration) : DeclarationContent = DeclarationSyntax.content decl

    static member CheckSignatureFile(ctx: ProjectContext, before: FileSignature list, parse: FSharpParseFileResults) : FileSignature =
        DeclarationProjectCheck.checkSignatureFile ctx before parse

    static member AccessPathTexts(signature: FileSignature) : (string * string)[] =
        DeclarationProjectCheck.accessPathTexts signature

    static member BeginFile
        (ctx: ProjectContext, before: FileSignature list, parse: FSharpParseFileResults)
        : FileStart * DeclarationEnvironment =
        DeclarationProjectCheck.beginFile ctx before parse

    static member FileDependencies(ctx: ProjectContext, parses: FSharpParseFileResults[]) : int[][] =
        DeclarationSyntax.fileDependencies ctx parses

    static member CheckFile
        (ctx: ProjectContext, before: FileSignature list, parse: FSharpParseFileResults, ?fingerprint: bool)
        : FileResult =
        DeclarationProjectCheck.checkFile ctx before parse fingerprint

    static member AfterUndoHook
        with get () = DeclarationStep.afterUndoHook
        and set value = DeclarationStep.afterUndoHook <- value

    static member CheckDeclaration
        (start: FileStart, envBefore: DeclarationEnvironment, decl: ParsedDeclaration, ct: CancellationToken)
        : DeclarationResult =
        DeclarationStep.checkDeclaration start envBefore decl ct

    static member WindowOpen(start: FileStart, results: DeclarationResult[], ct: CancellationToken) : bool =
        DeclarationStep.windowOpen start results ct

    static member PrintsNow(start: FileStart, results: DeclarationResult[]) : string[] = DeclarationStep.printsNow start results

    static member Shift(result: DeclarationResult, decl: ParsedDeclaration) : DeclarationResult voption = DeclarationStep.shift result decl

    static member ExtendEnv(envBefore: DeclarationEnvironment, result: DeclarationResult) : DeclarationEnvironment =
        DeclarationStep.extendEnv envBefore result

    static member ExtendEnvAdHoc(envBefore: DeclarationEnvironment, result: DeclarationResult) : DeclarationEnvironment =
        DeclarationStep.extendEnvAdHoc envBefore result

    static member CheckDeclarationAdHoc
        (start: FileStart, envBefore: DeclarationEnvironment, decl: ParsedDeclaration, ct: CancellationToken)
        : DeclarationResult =
        DeclarationStep.checkDeclarationAdHoc start envBefore decl ct

    static member AnsweringDeclarations
        (start: FileStart, results: DeclarationResult[], parse: FSharpParseFileResults, line: int, column: int)
        : struct (int[] * int * bool) =
        DeclarationFinish.answeringDeclarations start results parse line column

    static member ViewResults(start: FileStart, results: DeclarationResult[], parse: FSharpParseFileResults) : FSharpCheckFileResults =
        DeclarationFinish.viewResults start results parse

    static member FinishFile(start: FileStart, results: DeclarationResult[], parse: FSharpParseFileResults) : FileResult =
        DeclarationFinish.finishFile start results parse
