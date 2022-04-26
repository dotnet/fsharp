// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to coordinate the parsing and checking of one or a group of files
module internal FSharp.Compiler.ParseAndCheckInputs

open System
open System.IO
open System.Collections.Generic

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Text.Lexing

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.IO
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TcGlobals

let CanonicalizeFilename fileName =
    let basic = FileSystemUtils.fileNameOfPath fileName
    String.capitalize (try FileSystemUtils.chopExtension basic with _ -> basic)

let IsScript fileName =
    FSharpScriptFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)

// Give a unique name to the different kinds of inputs. Used to correlate signature and implementation files
//   QualFileNameOfModuleName - files with a single module declaration or an anonymous module
let QualFileNameOfModuleName m fileName modname =
    QualifiedNameOfFile(mkSynId m (textOfLid modname + (if IsScript fileName then "$fsx" else "")))

let QualFileNameOfFilename m fileName =
    QualifiedNameOfFile(mkSynId m (CanonicalizeFilename fileName + (if IsScript fileName then "$fsx" else "")))

// Interactive fragments
let ComputeQualifiedNameOfFileFromUniquePath (m, p: string list) =
    QualifiedNameOfFile(mkSynId m (String.concat "_" p))

let QualFileNameOfSpecs fileName specs =
    match specs with
    | [SynModuleOrNamespaceSig(longId = modname; kind = kind; range = m)] when kind.IsModule -> QualFileNameOfModuleName m fileName modname
    | [SynModuleOrNamespaceSig(kind = kind; range = m)] when not kind.IsModule -> QualFileNameOfFilename m fileName
    | _ -> QualFileNameOfFilename (mkRange fileName pos0 pos0) fileName

let QualFileNameOfImpls fileName specs =
    match specs with
    | [SynModuleOrNamespace(longId = modname; kind = kind; range = m)] when kind.IsModule -> QualFileNameOfModuleName m fileName modname
    | [SynModuleOrNamespace(kind = kind; range = m)] when not kind.IsModule -> QualFileNameOfFilename m fileName
    | _ -> QualFileNameOfFilename (mkRange fileName pos0 pos0) fileName

let PrependPathToQualFileName x (QualifiedNameOfFile q) =
    ComputeQualifiedNameOfFileFromUniquePath (q.idRange, pathOfLid x@[q.idText])

let PrependPathToImpl x (SynModuleOrNamespace(longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)) =
    SynModuleOrNamespace(x@longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)

let PrependPathToSpec x (SynModuleOrNamespaceSig(longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)) =
    SynModuleOrNamespaceSig(x@longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)

let PrependPathToInput x inp =
    match inp with
    | ParsedInput.ImplFile (ParsedImplFileInput (b, c, q, d, hd, impls, e, trivia)) ->
        ParsedInput.ImplFile (ParsedImplFileInput (b, c, PrependPathToQualFileName x q, d, hd, List.map (PrependPathToImpl x) impls, e, trivia))

    | ParsedInput.SigFile (ParsedSigFileInput (b, q, d, hd, specs, trivia)) ->
        ParsedInput.SigFile (ParsedSigFileInput (b, PrependPathToQualFileName x q, d, hd, List.map (PrependPathToSpec x) specs, trivia))

let ComputeAnonModuleName check defaultNamespace fileName (m: range) =
    let modname = CanonicalizeFilename fileName
    if check && not (modname |> String.forall (fun c -> Char.IsLetterOrDigit c || c = '_')) then
          if not (fileName.EndsWith("fsx", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith("fsscript", StringComparison.OrdinalIgnoreCase)) then
              warning(Error(FSComp.SR.buildImplicitModuleIsNotLegalIdentifier(modname, (FileSystemUtils.fileNameOfPath fileName)), m))
    let combined =
      match defaultNamespace with
      | None -> modname
      | Some ns -> textOfPath [ns;modname]

    let anonymousModuleNameRange =
        let fileName = m.FileName
        mkRange fileName pos0 pos0
    pathToSynLid anonymousModuleNameRange (splitNamespace combined)

let PostParseModuleImpl (_i, defaultNamespace, isLastCompiland, fileName, impl) =
    match impl with
    | ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)) ->
        let lid =
            match lid with
            | [id] when kind.IsModule && id.idText = MangledGlobalName ->
                error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespace(lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)

    | ParsedImplFileFragment.AnonModule (defs, m)->
        let isLast, isExe = isLastCompiland
        if not (isLast && isExe) && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)) then
            match defs with
            | SynModuleDecl.NestedModule _ :: _ -> errorR(Error(FSComp.SR.noEqualSignAfterModule(), trimRangeToLine m))
            | _ -> errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(), trimRangeToLine m))

        let modname = ComputeAnonModuleName (not (isNil defs)) defaultNamespace fileName (trimRangeToLine m)
        let trivia: SynModuleOrNamespaceTrivia = { ModuleKeyword = None; NamespaceKeyword = None }
        SynModuleOrNamespace(modname, false, SynModuleOrNamespaceKind.AnonModule, defs, PreXmlDoc.Empty, [], None, m, trivia)

    | ParsedImplFileFragment.NamespaceFragment (lid, isRecursive, kind, decls, xmlDoc, attributes, range, trivia)->
        let lid, kind =
            match lid with
            | id :: rest when id.idText = MangledGlobalName ->
                rest, if List.isEmpty rest then SynModuleOrNamespaceKind.GlobalNamespace else kind
            | _ -> lid, kind
        SynModuleOrNamespace(lid, isRecursive, kind, decls, xmlDoc, attributes, None, range, trivia)

let PostParseModuleSpec (_i, defaultNamespace, isLastCompiland, fileName, intf) =
    match intf with
    | ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)) ->
        let lid =
            match lid with
            | [id] when kind.IsModule && id.idText = MangledGlobalName ->
                error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespaceSig(lid, isRec, SynModuleOrNamespaceKind.NamedModule, decls, xmlDoc, attribs, access, m, trivia)

    | ParsedSigFileFragment.AnonModule (defs, m) ->
        let isLast, isExe = isLastCompiland
        if not (isLast && isExe) && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)) then
            match defs with
            | SynModuleSigDecl.NestedModule _ :: _ -> errorR(Error(FSComp.SR.noEqualSignAfterModule(), m))
            | _ -> errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(), m))

        let modname = ComputeAnonModuleName (not (isNil defs)) defaultNamespace fileName (trimRangeToLine m)
        let trivia: SynModuleOrNamespaceSigTrivia = { ModuleKeyword = None; NamespaceKeyword = None }
        SynModuleOrNamespaceSig(modname, false, SynModuleOrNamespaceKind.AnonModule, defs, PreXmlDoc.Empty, [], None, m, trivia)

    | ParsedSigFileFragment.NamespaceFragment (lid, isRecursive, kind, decls, xmlDoc, attributes, range, trivia)->
        let lid, kind =
            match lid with
            | id :: rest when id.idText = MangledGlobalName ->
                rest, if List.isEmpty rest then SynModuleOrNamespaceKind.GlobalNamespace else kind
            | _ -> lid, kind
        SynModuleOrNamespaceSig(lid, isRecursive, kind, decls, xmlDoc, attributes, None, range, trivia)

let GetScopedPragmasForInput input =
    match input with
    | ParsedInput.SigFile (ParsedSigFileInput (scopedPragmas=pragmas)) -> pragmas
    | ParsedInput.ImplFile (ParsedImplFileInput (scopedPragmas=pragmas)) -> pragmas

let GetScopedPragmasForHashDirective hd =
    [ match hd with
      | ParsedHashDirective("nowarn", numbers, m) ->
          for s in numbers do
              match s with
              | ParsedHashDirectiveArgument.SourceIdentifier _ -> ()
              | ParsedHashDirectiveArgument.String (s, _, _) ->
                  match GetWarningNumber(m, s) with
                  | None -> ()
                  | Some n -> yield ScopedPragma.WarningOff(m, n)
      | _ -> () ]

let private collectCodeComments (lexbuf: UnicodeLexing.Lexbuf) (tripleSlashComments: range list) =
    [ yield! LexbufCommentStore.GetComments(lexbuf); yield! (List.map CommentTrivia.LineComment tripleSlashComments) ]
    |> List.sortBy (function
        | CommentTrivia.LineComment r
        | CommentTrivia.BlockComment r -> r.StartLine, r.StartColumn)

let PostParseModuleImpls (defaultNamespace, fileName, isLastCompiland, ParsedImplFile (hashDirectives, impls), lexbuf: UnicodeLexing.Lexbuf, tripleSlashComments: range list) =
    match impls |> List.rev |> List.tryPick (function ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(longId = lid)) -> Some lid | _ -> None) with
    | Some lid when impls.Length > 1 ->
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(), rangeOfLid lid))
    | _ ->
        ()
    let impls = impls |> List.mapi (fun i x -> PostParseModuleImpl (i, defaultNamespace, isLastCompiland, fileName, x))
    let qualName = QualFileNameOfImpls fileName impls
    let isScript = IsScript fileName

    let scopedPragmas =
        [ for SynModuleOrNamespace(decls = decls) in impls do
            for d in decls do
                match d with
                | SynModuleDecl.HashDirective (hd, _) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> ()
          for hd in hashDirectives do
              yield! GetScopedPragmasForHashDirective hd ]

    let conditionalDirectives = LexbufIfdefStore.GetTrivia(lexbuf)
    let codeComments = collectCodeComments lexbuf tripleSlashComments
    let trivia: ParsedImplFileInputTrivia = { ConditionalDirectives = conditionalDirectives; CodeComments = codeComments }
    
    ParsedInput.ImplFile (ParsedImplFileInput (fileName, isScript, qualName, scopedPragmas, hashDirectives, impls, isLastCompiland, trivia))

let PostParseModuleSpecs (defaultNamespace, fileName, isLastCompiland, ParsedSigFile (hashDirectives, specs), lexbuf: UnicodeLexing.Lexbuf, tripleSlashComments: range list) =
    match specs |> List.rev |> List.tryPick (function ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(longId = lid)) -> Some lid | _ -> None) with
    | Some lid when specs.Length > 1 ->
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(), rangeOfLid lid))
    | _ ->
        ()

    let specs = specs |> List.mapi (fun i x -> PostParseModuleSpec(i, defaultNamespace, isLastCompiland, fileName, x))
    let qualName = QualFileNameOfSpecs fileName specs
    let scopedPragmas =
        [ for SynModuleOrNamespaceSig(decls = decls) in specs do
            for d in decls do
                match d with
                | SynModuleSigDecl.HashDirective(hd, _) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> ()
          for hd in hashDirectives do
              yield! GetScopedPragmasForHashDirective hd ]

    let conditionalDirectives = LexbufIfdefStore.GetTrivia(lexbuf)
    let codeComments = collectCodeComments lexbuf tripleSlashComments
    let trivia: ParsedSigFileInputTrivia = { ConditionalDirectives = conditionalDirectives; CodeComments = codeComments }

    ParsedInput.SigFile (ParsedSigFileInput (fileName, qualName, scopedPragmas, hashDirectives, specs, trivia))

type ModuleNamesDict = Map<string,Map<string,QualifiedNameOfFile>>

/// Checks if a module name is already given and deduplicates the name if needed.
let DeduplicateModuleName (moduleNamesDict: ModuleNamesDict) fileName (qualNameOfFile: QualifiedNameOfFile) =
    let path = Path.GetDirectoryName fileName
    let path = if FileSystem.IsPathRootedShim path then try FileSystem.GetFullPathShim path with _ -> path else path
    match moduleNamesDict.TryGetValue qualNameOfFile.Text with
    | true, paths ->
        if paths.ContainsKey path then
            paths[path], moduleNamesDict
        else
            let count = paths.Count + 1
            let id = qualNameOfFile.Id
            let qualNameOfFileT = if count = 1 then qualNameOfFile else QualifiedNameOfFile(Ident(id.idText + "___" + count.ToString(), id.idRange))
            let moduleNamesDictT = moduleNamesDict.Add(qualNameOfFile.Text, paths.Add(path, qualNameOfFileT))
            qualNameOfFileT, moduleNamesDictT
    | _ ->
        let moduleNamesDictT = moduleNamesDict.Add(qualNameOfFile.Text, Map.empty.Add(path, qualNameOfFile))
        qualNameOfFile, moduleNamesDictT

/// Checks if a ParsedInput is using a module name that was already given and deduplicates the name if needed.
let DeduplicateParsedInputModuleName (moduleNamesDict: ModuleNamesDict) input =
    match input with
    | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (fileName, isScript, qualNameOfFile, scopedPragmas, hashDirectives, modules, (isLastCompiland, isExe), trivia)) ->
        let qualNameOfFileT, moduleNamesDictT = DeduplicateModuleName moduleNamesDict fileName qualNameOfFile
        let inputT = ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput (fileName, isScript, qualNameOfFileT, scopedPragmas, hashDirectives, modules, (isLastCompiland, isExe), trivia))
        inputT, moduleNamesDictT
    | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (fileName, qualNameOfFile, scopedPragmas, hashDirectives, modules, trivia)) ->
        let qualNameOfFileT, moduleNamesDictT = DeduplicateModuleName moduleNamesDict fileName qualNameOfFile
        let inputT = ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput (fileName, qualNameOfFileT, scopedPragmas, hashDirectives, modules, trivia))
        inputT, moduleNamesDictT

let ParseInput (lexer, diagnosticOptions:FSharpDiagnosticOptions, diagnosticsLogger: DiagnosticsLogger, lexbuf: UnicodeLexing.Lexbuf, defaultNamespace, fileName, isLastCompiland) =
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy file name
    //  - if you have a #line directive, e.g.
    //        # 1000 "Line01.fs"
    //    then it also asserts. But these are edge cases that can be fixed later, e.g. in bug 4651.

    // Delay sending errors and warnings until after the file is parsed. This gives us a chance to scrape the
    // #nowarn declarations for the file
    let delayLogger = CapturingDiagnosticsLogger("Parsing")
    use unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind (fun _ -> delayLogger)
    use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

    let mutable scopedPragmas = []
    try
        let input =
            if mlCompatSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                if lexbuf.SupportsFeature LanguageFeature.MLCompatRevisions then
                    errorR(Error(FSComp.SR.buildInvalidSourceFileExtensionML fileName, rangeStartup))
                else
                    mlCompatWarning (FSComp.SR.buildCompilingExtensionIsForML()) rangeStartup

            // Call the appropriate parser - for signature files or implementation files
            if FSharpImplFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                let impl = Parser.implementationFile lexer lexbuf
                let tripleSlashComments = LexbufLocalXmlDocStore.ReportInvalidXmlDocPositions(lexbuf)
                PostParseModuleImpls (defaultNamespace, fileName, isLastCompiland, impl, lexbuf, tripleSlashComments)
            elif FSharpSigFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                let intfs = Parser.signatureFile lexer lexbuf
                let tripleSlashComments =  LexbufLocalXmlDocStore.ReportInvalidXmlDocPositions(lexbuf)
                PostParseModuleSpecs (defaultNamespace, fileName, isLastCompiland, intfs, lexbuf, tripleSlashComments)
            else
                if lexbuf.SupportsFeature LanguageFeature.MLCompatRevisions then
                    error(Error(FSComp.SR.buildInvalidSourceFileExtensionUpdated fileName, rangeStartup))
                else
                    error(Error(FSComp.SR.buildInvalidSourceFileExtension fileName, rangeStartup))


        scopedPragmas <- GetScopedPragmasForInput input
        input
    finally
        // OK, now commit the errors, since the ScopedPragmas will (hopefully) have been scraped
        let filteringDiagnosticsLogger = GetDiagnosticsLoggerFilteringByScopedPragmas(false, scopedPragmas, diagnosticOptions, diagnosticsLogger)
        delayLogger.CommitDelayedDiagnostics filteringDiagnosticsLogger

type Tokenizer = unit -> Parser.token

// Show all tokens in the stream, for testing purposes
let ShowAllTokensAndExit (shortFilename, tokenizer: Tokenizer, lexbuf: LexBuffer<char>) =
    while true do
        printf "tokenize - getting one token from %s\n" shortFilename
        let t = tokenizer ()
        printf "tokenize - got %s @ %a\n" (Parser.token_to_string t) outputRange lexbuf.LexemeRange
        match t with
        | Parser.EOF _ -> exit 0
        | _ -> ()
        if lexbuf.IsPastEndOfStream then printf "!!! at end of stream\n"

// Test one of the parser entry points, just for testing purposes
let TestInteractionParserAndExit (tokenizer: Tokenizer, lexbuf: LexBuffer<char>) =
    while true do
        match (Parser.interaction (fun _ -> tokenizer ()) lexbuf) with
        | ParsedScriptInteraction.Definitions(l, m) -> printfn "Parsed OK, got %d defs @ %a" l.Length outputRange m
        | ParsedScriptInteraction.HashDirective(_, m) -> printfn "Parsed OK, got hash @ %a" outputRange m
    exit 0

// Report the statistics for testing purposes
let ReportParsingStatistics res =
    let rec flattenSpecs specs =
            specs |> List.collect (function SynModuleSigDecl.NestedModule (moduleDecls=subDecls) -> flattenSpecs subDecls | spec -> [spec])
    let rec flattenDefns specs =
            specs |> List.collect (function SynModuleDecl.NestedModule (decls=subDecls) -> flattenDefns subDecls | defn -> [defn])

    let flattenModSpec (SynModuleOrNamespaceSig(decls = decls)) = flattenSpecs decls
    let flattenModImpl (SynModuleOrNamespace(decls = decls)) = flattenDefns decls
    match res with
    | ParsedInput.SigFile (ParsedSigFileInput (modules = specs)) ->
        printfn "parsing yielded %d specs" (List.collect flattenModSpec specs).Length
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = impls)) ->
        printfn "parsing yielded %d definitions" (List.collect flattenModImpl impls).Length

let EmptyParsedInput(fileName, isLastCompiland) =
    if FSharpSigFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
        ParsedInput.SigFile(
            ParsedSigFileInput(
                fileName,
                QualFileNameOfImpls fileName [],
                [],
                [],
                [],
                { ConditionalDirectives = []; CodeComments = [] }
            )
        )
    else
        ParsedInput.ImplFile(
            ParsedImplFileInput(
                fileName,
                false,
                QualFileNameOfImpls fileName [],
                [],
                [],
                [],
                isLastCompiland,
                { ConditionalDirectives = []; CodeComments = [] }
            )
        )

/// Parse an input, drawing tokens from the LexBuffer
let ParseOneInputLexbuf (tcConfig: TcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger) =
    use unwindbuildphase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
    try

        // Don't report whitespace from lexer
        let skipWhitespaceTokens = true

        // Set up the initial status for indentation-aware processing
        let indentationSyntaxStatus = IndentationAwareSyntaxStatus (tcConfig.ComputeIndentationAwareSyntaxInitialStatus fileName, true)

        // Set up the initial lexer arguments
        let lexargs = mkLexargs (tcConfig.conditionalDefines, indentationSyntaxStatus, lexResourceManager, [], diagnosticsLogger, tcConfig.pathMap)

        // Set up the initial lexer arguments
        let shortFilename = SanitizeFileName fileName tcConfig.implicitIncludeDir

        let input =
            usingLexbufForParsing (lexbuf, fileName) (fun lexbuf ->

                // Set up the LexFilter over the token stream
                let tokenizer,tokenizeOnly =
                    match tcConfig.tokenize with
                    | TokenizeOption.Unfiltered ->
                        (fun () -> Lexer.token lexargs skipWhitespaceTokens lexbuf), true
                    | TokenizeOption.Only ->
                        LexFilter.LexFilter(indentationSyntaxStatus, tcConfig.compilingFSharpCore, Lexer.token lexargs skipWhitespaceTokens, lexbuf).GetToken, true
                    | _ ->
                        LexFilter.LexFilter(indentationSyntaxStatus, tcConfig.compilingFSharpCore, Lexer.token lexargs skipWhitespaceTokens, lexbuf).GetToken, false

                // If '--tokenize' then show the tokens now and exit
                if tokenizeOnly then
                    ShowAllTokensAndExit(shortFilename, tokenizer, lexbuf)

                // Test hook for one of the parser entry points
                if tcConfig.testInteractionParser then
                    TestInteractionParserAndExit (tokenizer, lexbuf)

                // Parse the input
                let res = ParseInput((fun _ -> tokenizer ()), tcConfig.diagnosticsOptions, diagnosticsLogger, lexbuf, None, fileName, isLastCompiland)

                // Report the statistics for testing purposes
                if tcConfig.reportNumDecls then
                    ReportParsingStatistics res

                res
            )
        input

    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

let ValidSuffixes = FSharpSigFileSuffixes@FSharpImplFileSuffixes

let checkInputFile (tcConfig: TcConfig) fileName =
    if List.exists (FileSystemUtils.checkSuffix fileName) ValidSuffixes then
        if not(FileSystem.FileExistsShim fileName) then
            error(Error(FSComp.SR.buildCouldNotFindSourceFile fileName, rangeStartup))
    else
        error(Error(FSComp.SR.buildInvalidSourceFileExtension(SanitizeFileName fileName tcConfig.implicitIncludeDir), rangeStartup))

let parseInputStreamAux (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked, stream: Stream) =
    use reader = stream.GetReader(tcConfig.inputCodePage, retryLocked)

    // Set up the LexBuffer for the file
    let lexbuf = UnicodeLexing.StreamReaderAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, reader)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

let parseInputSourceTextAux (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, sourceText: ISourceText) =
    // Set up the LexBuffer for the file
    let lexbuf = UnicodeLexing.SourceTextAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, sourceText)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

let parseInputFileAux (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked) =
    // Get a stream reader for the file
    use fileStream = FileSystem.OpenFileForReadShim(fileName)
    use reader = fileStream.GetReader(tcConfig.inputCodePage, retryLocked)

    // Set up the LexBuffer for the file
    let lexbuf = UnicodeLexing.StreamReaderAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, reader)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

/// Parse an input from stream
let ParseOneInputStream (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked, stream: Stream) =
    try
       parseInputStreamAux(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked, stream)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Parse an input from source text
let ParseOneInputSourceText (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, sourceText: ISourceText) =
    try
       parseInputSourceTextAux(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, sourceText)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Parse an input from disk
let ParseOneInputFile (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked) =
    try
       checkInputFile tcConfig fileName
       parseInputFileAux(tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Parse multiple input files from disk
let ParseInputFiles (tcConfig: TcConfig, lexResourceManager, sourceFiles, diagnosticsLogger: DiagnosticsLogger, exiter: Exiter, createDiagnosticsLogger: Exiter -> CapturingDiagnosticsLogger, retryLocked) =
    try
        let isLastCompiland, isExe = sourceFiles |> tcConfig.ComputeCanContainEntryPoint
        let sourceFiles = isLastCompiland |> List.zip sourceFiles |> Array.ofList

        if tcConfig.concurrentBuild then
            let mutable exitCode = 0
            let delayedExiter =
                { new Exiter with
                    member _.Exit n = exitCode <- n; raise StopProcessing }

            // Check input files and create delayed error loggers before we try to parallel parse.
            let delayedDiagnosticsLoggers =
                sourceFiles
                |> Array.map (fun (fileName, _) ->
                    checkInputFile tcConfig fileName
                    createDiagnosticsLogger(delayedExiter)
                )

            let results =
                try
                    try
                        sourceFiles
                        |> ArrayParallel.mapi (fun i (fileName, isLastCompiland) ->
                            let delayedDiagnosticsLogger = delayedDiagnosticsLoggers[i]

                            let directoryName = Path.GetDirectoryName fileName
                            let input = parseInputFileAux(tcConfig, lexResourceManager, fileName, (isLastCompiland, isExe), delayedDiagnosticsLogger, retryLocked)
                            (input, directoryName)
                        )
                    finally
                        delayedDiagnosticsLoggers
                        |> Array.iter (fun delayedDiagnosticsLogger ->
                            delayedDiagnosticsLogger.CommitDelayedDiagnostics diagnosticsLogger
                        )
                with
                | StopProcessing ->
                    exiter.Exit exitCode

            results
            |> List.ofArray
        else
            sourceFiles
            |> Array.map (fun (fileName, isLastCompiland) ->
                let directoryName = Path.GetDirectoryName fileName
                let input = ParseOneInputFile(tcConfig, lexResourceManager, fileName, (isLastCompiland, isExe), diagnosticsLogger, retryLocked)
                (input, directoryName))
            |> List.ofArray

    with e ->
        errorRecoveryNoRange e
        exiter.Exit 1

let ProcessMetaCommandsFromInput
     (nowarnF: 'state -> range * string -> 'state,
      hashReferenceF: 'state -> range * string * Directive -> 'state,
      loadSourceF: 'state -> range * string -> unit)
     (tcConfig:TcConfigBuilder,
      inp: ParsedInput,
      pathOfMetaCommandSource,
      state0) =

    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

    let canHaveScriptMetaCommands =
        match inp with
        | ParsedInput.SigFile _ -> false
        | ParsedInput.ImplFile (ParsedImplFileInput (isScript = isScript)) -> isScript

    let ProcessDependencyManagerDirective directive args m state =
        if not canHaveScriptMetaCommands then
            errorR(HashReferenceNotAllowedInNonScript m)

        match args with
        | [path] ->
            let p =
                if String.IsNullOrWhiteSpace(path) then ""
                else path

            hashReferenceF state (m, p, directive)

        | _ ->
            errorR(Error(FSComp.SR.buildInvalidHashrDirective(), m))
            state

    let ProcessMetaCommand state hash =
        let mutable matchedm = range0
        try
            match hash with
            | ParsedHashDirective("I", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR(HashIncludeNotAllowedInNonScript m)
                match args with
                | [path] ->
                    matchedm <- m
                    tcConfig.AddIncludePath(m, path, pathOfMetaCommandSource)
                    state
                | _ ->
                    errorR(Error(FSComp.SR.buildInvalidHashIDirective(), m))
                    state
            | ParsedHashDirective("nowarn", ParsedHashDirectiveArguments numbers,m) ->
                List.fold (fun state d -> nowarnF state (m,d)) state numbers

            | ParsedHashDirective(("reference" | "r"), ParsedHashDirectiveArguments args, m) ->
                matchedm<-m
                ProcessDependencyManagerDirective Directive.Resolution args m state

            | ParsedHashDirective("i", ParsedHashDirectiveArguments args, m) ->
                matchedm<-m
                ProcessDependencyManagerDirective Directive.Include args m state

            | ParsedHashDirective("load", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR(HashDirectiveNotAllowedInNonScript m)
                match args with
                | _ :: _ ->
                   matchedm<-m
                   args |> List.iter (fun path -> loadSourceF state (m, path))
                | _ ->
                   errorR(Error(FSComp.SR.buildInvalidHashloadDirective(), m))
                state
            | ParsedHashDirective("time", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR(HashDirectiveNotAllowedInNonScript m)
                match args with
                | [] ->
                     ()
                | ["on" | "off"] ->
                    ()
                | _ ->
                    errorR(Error(FSComp.SR.buildInvalidHashtimeDirective(), m))
                state

            | _ ->

                (* warning(Error("This meta-command has been ignored", m)) *)
                state
        with e -> errorRecovery e matchedm; state

    let rec WarnOnIgnoredSpecDecls decls =
        decls |> List.iter (fun d ->
            match d with
            | SynModuleSigDecl.HashDirective (_, m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(), m))
            | SynModuleSigDecl.NestedModule (moduleDecls=subDecls) -> WarnOnIgnoredSpecDecls subDecls
            | _ -> ())

    let rec WarnOnIgnoredImplDecls decls =
        decls |> List.iter (fun d ->
            match d with
            | SynModuleDecl.HashDirective (_, m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(), m))
            | SynModuleDecl.NestedModule (decls=subDecls) -> WarnOnIgnoredImplDecls subDecls
            | _ -> ())

    let ProcessMetaCommandsFromModuleSpec state (SynModuleOrNamespaceSig(decls = decls)) =
        List.fold (fun s d ->
            match d with
            | SynModuleSigDecl.HashDirective (h, _) -> ProcessMetaCommand s h
            | SynModuleSigDecl.NestedModule (moduleDecls=subDecls) -> WarnOnIgnoredSpecDecls subDecls; s
            | _ -> s)
         state
         decls

    let ProcessMetaCommandsFromModuleImpl state (SynModuleOrNamespace(decls = decls)) =
        List.fold (fun s d ->
            match d with
            | SynModuleDecl.HashDirective (h, _) -> ProcessMetaCommand s h
            | SynModuleDecl.NestedModule (decls=subDecls) -> WarnOnIgnoredImplDecls subDecls; s
            | _ -> s)
         state
         decls

    match inp with
    | ParsedInput.SigFile (ParsedSigFileInput (hashDirectives = hashDirectives; modules = specs)) ->
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleSpec state specs
        state
    | ParsedInput.ImplFile (ParsedImplFileInput (hashDirectives = hashDirectives; modules = impls)) ->
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleImpl state impls
        state

let ApplyNoWarnsToTcConfig (tcConfig: TcConfig, inp: ParsedInput, pathOfMetaCommandSource) =
    // Clone
    let tcConfigB = tcConfig.CloneToBuilder()
    let addNoWarn = fun () (m,s) -> tcConfigB.TurnWarningOff(m, s)
    let addReference = fun () (_m, _s, _) -> ()
    let addLoadedSource = fun () (_m, _s) -> ()
    ProcessMetaCommandsFromInput
        (addNoWarn, addReference, addLoadedSource)
        (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate=false)

let ApplyMetaCommandsFromInputToTcConfig (tcConfig: TcConfig, inp: ParsedInput, pathOfMetaCommandSource, dependencyProvider) =
    // Clone
    let tcConfigB = tcConfig.CloneToBuilder()
    let getWarningNumber = fun () _ -> ()
    let addReferenceDirective = fun () (m, path, directive) -> tcConfigB.AddReferenceDirective(dependencyProvider, m, path, directive)
    let addLoadedSource = fun () (m,s) -> tcConfigB.AddLoadedSource(m,s,pathOfMetaCommandSource)
    ProcessMetaCommandsFromInput
        (getWarningNumber, addReferenceDirective, addLoadedSource)
        (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate=false)

/// Build the initial type checking environment
let GetInitialTcEnv (assemblyName: string, initm: range, tcConfig: TcConfig, tcImports: TcImports, tcGlobals) =
    let initm = initm.StartRange

    let ccus =
        tcImports.GetImportedAssemblies()
        |> List.map (fun asm -> asm.FSharpViewOfMetadata, asm.AssemblyAutoOpenAttributes, asm.AssemblyInternalsVisibleToAttributes)

    let amap = tcImports.GetImportMap()

    let openDecls0, tcEnv = CreateInitialTcEnv(tcGlobals, amap, initm, assemblyName, ccus)

    if tcConfig.checkOverflow then
        try 
            let checkOperatorsModule = pathToSynLid initm (splitNamespace CoreOperatorsCheckedName)
            let tcEnv, openDecls1 = TcOpenModuleOrNamespaceDecl TcResultsSink.NoSink tcGlobals amap initm tcEnv (checkOperatorsModule, initm)
            tcEnv, openDecls0 @ openDecls1
        with e ->
            errorRecovery e initm
            tcEnv, openDecls0
    else
        tcEnv, openDecls0

/// Inject faults into checking
let CheckSimulateException(tcConfig: TcConfig) =
    match tcConfig.simulateException with
    | Some("tc-oom") -> raise(OutOfMemoryException())
    | Some("tc-an") -> raise(ArgumentNullException("simulated"))
    | Some("tc-invop") -> raise(InvalidOperationException())
    | Some("tc-av") -> raise(AccessViolationException())
    | Some("tc-nfn") -> raise(NotFiniteNumberException())
    | Some("tc-aor") -> raise(ArgumentOutOfRangeException())
    | Some("tc-dv0") -> raise(DivideByZeroException())
    | Some("tc-oe") -> raise(OverflowException())
    | Some("tc-atmm") -> raise(ArrayTypeMismatchException())
    | Some("tc-bif") -> raise(BadImageFormatException())
    | Some("tc-knf") -> raise(KeyNotFoundException())
    | Some("tc-ior") -> raise(IndexOutOfRangeException())
    | Some("tc-ic") -> raise(InvalidCastException())
    | Some("tc-ip") -> raise(InvalidProgramException())
    | Some("tc-ma") -> raise(MemberAccessException())
    | Some("tc-ni") -> raise(NotImplementedException())
    | Some("tc-nr") -> raise(NullReferenceException())
    | Some("tc-oc") -> raise(OperationCanceledException())
    | Some("tc-fail") -> failwith "simulated"
    | _ -> ()

//----------------------------------------------------------------------------
// Type-check sets of files
//--------------------------------------------------------------------------

type RootSigs = Zmap<QualifiedNameOfFile, ModuleOrNamespaceType>

type RootImpls = Zset<QualifiedNameOfFile >

let qnameOrder = Order.orderBy (fun (q: QualifiedNameOfFile) -> q.Text)

type TcState =
    {
      tcsCcu: CcuThunk
      tcsCcuType: ModuleOrNamespace
      tcsNiceNameGen: NiceNameGenerator
      tcsTcSigEnv: TcEnv
      tcsTcImplEnv: TcEnv
      tcsCreatesGeneratedProvidedTypes: bool
      tcsRootSigs: RootSigs
      tcsRootImpls: RootImpls
      tcsCcuSig: ModuleOrNamespaceType
      
      /// The collected open declarations implied by '/checked' flag and processing F# interactive fragments that have an implied module.
      tcsImplicitOpenDeclarations: OpenDeclaration list
    }

    member x.NiceNameGenerator = x.tcsNiceNameGen

    member x.TcEnvFromSignatures = x.tcsTcSigEnv

    member x.TcEnvFromImpls = x.tcsTcImplEnv

    member x.Ccu = x.tcsCcu

    member x.CreatesGeneratedProvidedTypes = x.tcsCreatesGeneratedProvidedTypes

    // Assem(a.fsi + b.fsi + c.fsi) (after checking implementation file )
    member x.CcuType = x.tcsCcuType

    // a.fsi + b.fsi + c.fsi (after checking implementation file for c.fs)
    member x.CcuSig = x.tcsCcuSig

    member x.NextStateAfterIncrementalFragment tcEnvAtEndOfLastInput =
        { x with tcsTcSigEnv = tcEnvAtEndOfLastInput
                 tcsTcImplEnv = tcEnvAtEndOfLastInput }


/// Create the initial type checking state for compiling an assembly
let GetInitialTcState(m, ccuName, tcConfig: TcConfig, tcGlobals, tcImports: TcImports, niceNameGen, tcEnv0, openDecls0) =
    ignore tcImports

    // Create a ccu to hold all the results of compilation
    let ccuContents = Construct.NewCcuContents ILScopeRef.Local m ccuName (Construct.NewEmptyModuleOrNamespaceType Namespace)

    let ccuData: CcuData =
        { IsFSharp=true
          UsesFSharp20PlusQuotations=false
#if !NO_TYPEPROVIDERS
          InvalidateEvent=(Event<_>()).Publish
          IsProviderGenerated = false
          ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
          TryGetILModuleDef = (fun () -> None)
          FileName=None
          Stamp = newStamp()
          QualifiedName= None
          SourceCodeDirectory = tcConfig.implicitIncludeDir
          ILScopeRef=ILScopeRef.Local
          Contents=ccuContents
          MemberSignatureEquality= typeEquivAux EraseAll tcGlobals
          TypeForwarders= Dictionary<_,_>(0, HashIdentity.Structural)
          XmlDocumentationInfo = None }

    let ccu = CcuThunk.Create(ccuName, ccuData)

    // OK, is this is the FSharp.Core CCU then fix it up.
    if tcConfig.compilingFSharpCore then
        tcGlobals.fslibCcu.Fixup ccu

    { tcsCcu= ccu
      tcsCcuType=ccuContents
      tcsNiceNameGen=niceNameGen
      tcsTcSigEnv=tcEnv0
      tcsTcImplEnv=tcEnv0
      tcsCreatesGeneratedProvidedTypes=false
      tcsRootSigs = Zmap.empty qnameOrder
      tcsRootImpls = Zset.empty qnameOrder
      tcsCcuSig = Construct.NewEmptyModuleOrNamespaceType Namespace 
      tcsImplicitOpenDeclarations = openDecls0
    }

/// Dummy typed impl file that contains no definitions and is not used for emitting any kind of assembly.
let CreateEmptyDummyImplFile qualNameOfFile sigTy =
    CheckedImplFile.CheckedImplFile(qualNameOfFile, [], sigTy, ModuleOrNamespaceContents.TMDefs [], false, false, StampMap [], Map.empty)

/// Typecheck a single file (or interactive entry into F# Interactive)
let CheckOneInput
    (
        checkForErrors,
        tcConfig: TcConfig,
        tcImports: TcImports,
        tcGlobals,
        prefixPathOpt,
        tcSink,
        tcState: TcState,
        inp: ParsedInput,
        skipImplIfSigExists: bool
    ) =

    cancellable {
        try
          CheckSimulateException tcConfig

          let m = inp.Range
          let amap = tcImports.GetImportMap()
          match inp with
          | ParsedInput.SigFile (ParsedSigFileInput (qualifiedNameOfFile = qualNameOfFile) as file) ->

              // Check if we've seen this top module signature before.
              if Zmap.mem qualNameOfFile tcState.tcsRootSigs then
                  errorR(Error(FSComp.SR.buildSignatureAlreadySpecified(qualNameOfFile.Text), m.StartRange))

              // Check if the implementation came first in compilation order
              if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                  errorR(Error(FSComp.SR.buildImplementationAlreadyGivenDetail(qualNameOfFile.Text), m))

              let conditionalDefines =
                  if tcConfig.noConditionalErasure then None else Some tcConfig.conditionalDefines

              // Typecheck the signature file
              let! tcEnv, sigFileType, createsGeneratedProvidedTypes =
                  CheckOneSigFile (tcGlobals, tcState.tcsNiceNameGen, amap, tcState.tcsCcu, checkForErrors, conditionalDefines, tcSink, tcConfig.internalTestSpanStackReferring) tcState.tcsTcSigEnv file

              let rootSigs = Zmap.add qualNameOfFile sigFileType tcState.tcsRootSigs

              // Add the signature to the signature env (unless it had an explicit signature)
              let ccuSigForFile = CombineCcuContentFragments m [sigFileType; tcState.tcsCcuSig]

              // Open the prefixPath for fsi.exe
              let tcEnv, _openDecls1 =
                  match prefixPathOpt with
                  | None -> tcEnv, []
                  | Some prefixPath ->
                      let m = qualNameOfFile.Range
                      TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcEnv (prefixPath, m)

              let tcState =
                   { tcState with
                        tcsTcSigEnv=tcEnv
                        tcsTcImplEnv=tcState.tcsTcImplEnv
                        tcsRootSigs=rootSigs
                        tcsCreatesGeneratedProvidedTypes=tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes}

              return (tcEnv, EmptyTopAttrs, None, ccuSigForFile), tcState

          | ParsedInput.ImplFile (ParsedImplFileInput (qualifiedNameOfFile = qualNameOfFile) as file) ->

              // Check if we've got an interface for this fragment
              let rootSigOpt = tcState.tcsRootSigs.TryFind qualNameOfFile

              // Check if we've already seen an implementation for this fragment
              if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                  errorR(Error(FSComp.SR.buildImplementationAlreadyGiven(qualNameOfFile.Text), m))

              let tcImplEnv = tcState.tcsTcImplEnv

              let conditionalDefines =
                  if tcConfig.noConditionalErasure then None else Some tcConfig.conditionalDefines

              let hadSig = rootSigOpt.IsSome

              // Typecheck the implementation file
              let typeCheckOne =
                  if skipImplIfSigExists && hadSig then
                    (EmptyTopAttrs, CreateEmptyDummyImplFile qualNameOfFile rootSigOpt.Value, Unchecked.defaultof<_>, tcImplEnv, false)
                    |> Cancellable.ret
                  else
                    CheckOneImplFile (tcGlobals, tcState.tcsNiceNameGen, amap, tcState.tcsCcu, tcState.tcsImplicitOpenDeclarations, checkForErrors, conditionalDefines, tcSink, tcConfig.internalTestSpanStackReferring, tcImplEnv, rootSigOpt, file)

              let! topAttrs, implFile, _implFileHiddenType, tcEnvAtEnd, createsGeneratedProvidedTypes = typeCheckOne

              let implFileSigType = implFile.Signature

              let rootImpls = Zset.add qualNameOfFile tcState.tcsRootImpls

              // Only add it to the environment if it didn't have a signature
              let m = qualNameOfFile.Range

              // Add the implementation as to the implementation env
              let tcImplEnv = AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcImplEnv implFileSigType

              // Add the implementation as to the signature env (unless it had an explicit signature)
              let tcSigEnv =
                  if hadSig then tcState.tcsTcSigEnv
                  else AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcState.tcsTcSigEnv implFileSigType

              // Open the prefixPath for fsi.exe (tcImplEnv)
              let tcImplEnv, openDecls =
                  match prefixPathOpt with
                  | Some prefixPath -> TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcImplEnv (prefixPath, m)
                  | _ -> tcImplEnv, []

              // Open the prefixPath for fsi.exe (tcSigEnv)
              let tcSigEnv, _ =
                  match prefixPathOpt with
                  | Some prefixPath when not hadSig -> TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcSigEnv (prefixPath, m)
                  | _ -> tcSigEnv, []

              let ccuSigForFile = CombineCcuContentFragments m [implFileSigType; tcState.tcsCcuSig]

              let tcState =
                   { tcState with
                        tcsTcSigEnv=tcSigEnv
                        tcsTcImplEnv=tcImplEnv
                        tcsRootImpls=rootImpls
                        tcsCcuSig=ccuSigForFile
                        tcsCreatesGeneratedProvidedTypes=tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                        tcsImplicitOpenDeclarations = tcState.tcsImplicitOpenDeclarations @ openDecls
                    }
              return (tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile), tcState

        with e ->
            errorRecovery e range0
            return (tcState.TcEnvFromSignatures, EmptyTopAttrs, None, tcState.tcsCcuSig), tcState
    }

/// Typecheck a single file (or interactive entry into F# Interactive)
let TypeCheckOneInputEntry (ctok, checkForErrors, tcConfig:TcConfig, tcImports, tcGlobals, prefixPathOpt) tcState inp =
    // 'use' ensures that the warning handler is restored at the end
    use unwindEL = PushDiagnosticsLoggerPhaseUntilUnwind(fun oldLogger -> GetDiagnosticsLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput inp, tcConfig.diagnosticsOptions, oldLogger) )
    use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck

    RequireCompilationThread ctok
    CheckOneInput (checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, TcResultsSink.NoSink, tcState, inp, false)
        |> Cancellable.runWithoutCancellation

/// Finish checking multiple files (or one interactive entry into F# Interactive)
let CheckMultipleInputsFinish(results, tcState: TcState) =
    let tcEnvsAtEndFile, topAttrs, implFiles, ccuSigsForFiles = List.unzip4 results
    let topAttrs = List.foldBack CombineTopAttrs topAttrs EmptyTopAttrs
    let implFiles = List.choose id implFiles
    // This is the environment required by fsi.exe when incrementally adding definitions
    let tcEnvAtEndOfLastFile = (match tcEnvsAtEndFile with h :: _ -> h | _ -> tcState.TcEnvFromSignatures)
    (tcEnvAtEndOfLastFile, topAttrs, implFiles, ccuSigsForFiles), tcState

let CheckOneInputAndFinish(checkForErrors, tcConfig: TcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input) =
    cancellable {
        Logger.LogBlockStart LogCompilerFunctionId.CompileOps_TypeCheckOneInputAndFinishEventually
        let! results, tcState = CheckOneInput(checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input, false)
        let result = CheckMultipleInputsFinish([results], tcState)
        Logger.LogBlockStop LogCompilerFunctionId.CompileOps_TypeCheckOneInputAndFinishEventually
        return result
    }

let CheckClosedInputSetFinish (declaredImpls: CheckedImplFile list, tcState) =
    // Latest contents to the CCU
    let ccuContents = Construct.NewCcuContents ILScopeRef.Local range0 tcState.tcsCcu.AssemblyName tcState.tcsCcuSig

    // Check all interfaces have implementations
    tcState.tcsRootSigs |> Zmap.iter (fun qualNameOfFile _ ->
      if not (Zset.contains qualNameOfFile tcState.tcsRootImpls) then
        errorR(Error(FSComp.SR.buildSignatureWithoutImplementation(qualNameOfFile.Text), qualNameOfFile.Range)))

    tcState, declaredImpls, ccuContents

let CheckClosedInputSet (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcState, inputs) =
    // tcEnvAtEndOfLastFile is the environment required by fsi.exe when incrementally adding definitions
    let results, tcState = (tcState, inputs) ||> List.mapFold (TypeCheckOneInputEntry (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt))
    let (tcEnvAtEndOfLastFile, topAttrs, implFiles, _), tcState = CheckMultipleInputsFinish(results, tcState)
    let tcState, declaredImpls, ccuContents = CheckClosedInputSetFinish (implFiles, tcState)
    tcState.Ccu.Deref.Contents <- ccuContents
    tcState, topAttrs, declaredImpls, tcEnvAtEndOfLastFile
