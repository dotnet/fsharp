[<AutoOpen>]
module internal FSharp.Compiler.Service.Tests.Common

open System
open System.Diagnostics
open System.IO
open System.Collections.Generic
open System.Threading.Tasks
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open TestFramework
open FsUnit
open NUnit.Framework
open FSharp.Test.Utilities

type Async with
    static member RunImmediate (computation: Async<'T>, ?cancellationToken ) =
        let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
        let ts = TaskCompletionSource<'T>()
        let task = ts.Task
        Async.StartWithContinuations(
            computation,
            (fun k -> ts.SetResult k),
            (fun exn -> ts.SetException exn),
            (fun _ -> ts.SetCanceled()),
            cancellationToken)
        task.Result

// Create one global interactive checker instance
let checker = FSharpChecker.Create()

type TempFile(ext, contents: string) =
    let tmpFile =  Path.ChangeExtension(tryCreateTemporaryFileName (), ext)
    do FileSystem.OpenFileForWriteShim(tmpFile).Write(contents)

    interface IDisposable with
        member x.Dispose() = try FileSystem.FileDeleteShim tmpFile with _ -> ()
    member x.Name = tmpFile

#nowarn "57"

let getBackgroundParseResultsForScriptText (input: string) =
    use file =  new TempFile("fsx", input)
    let checkOptions, _diagnostics = checker.GetProjectOptionsFromScript(file.Name, SourceText.ofString input) |> Async.RunImmediate
    checker.GetBackgroundParseResultsForFileInProject(file.Name, checkOptions)  |> Async.RunImmediate


let getBackgroundCheckResultsForScriptText (input: string) =
    use file =  new TempFile("fsx", input)
    let checkOptions, _diagnostics = checker.GetProjectOptionsFromScript(file.Name, SourceText.ofString input) |> Async.RunImmediate
    checker.GetBackgroundCheckResultsForFileInProject(file.Name, checkOptions) |> Async.RunImmediate


let sysLib nm =
#if !NETCOREAPP
    if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows
        let programFilesx86Folder = System.Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
        programFilesx86Folder + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\" + nm + ".dll"
    else
#endif
        let sysDir = AppContext.BaseDirectory
        let (++) a b = Path.Combine(a,b)
        sysDir ++ nm + ".dll"

[<AutoOpen>]
module Helpers =
    type DummyType = A | B
    let PathRelativeToTestAssembly p = Path.Combine(Path.GetDirectoryName(Uri(typeof<FSharpChecker>.Assembly.Location).LocalPath), p)

let fsCoreDefaultReference() =
    PathRelativeToTestAssembly "FSharp.Core.dll"

let mkStandardProjectReferences () =
    TargetFrameworkUtil.currentReferences

let mkProjectCommandLineArgsSilent (dllName, fileNames) =
  let args =
    [|  yield "--simpleresolution"
        yield "--noframework"
        yield "--debug:full"
        yield "--define:DEBUG"
#if NETCOREAPP
        yield "--targetprofile:netcore"
        yield "--langversion:preview"
#endif
        yield "--optimize-"
        yield "--out:" + dllName
        yield "--doc:test.xml"
        yield "--warn:3"
        yield "--fullpaths"
        yield "--flaterrors"
        yield "--target:library"
        for x in fileNames do
            yield x
        let references = mkStandardProjectReferences ()
        for r in references do
            yield "-r:" + r
     |]
  args

let mkProjectCommandLineArgs (dllName, fileNames) =
  let args = mkProjectCommandLineArgsSilent (dllName, fileNames)
  printfn "dllName = %A, args = %A" dllName args
  args

#if NETCOREAPP
let mkProjectCommandLineArgsForScript (dllName, fileNames) =
    [|  yield "--simpleresolution"
        yield "--noframework"
        yield "--debug:full"
        yield "--define:DEBUG"
        yield "--targetprofile:netcore"
        yield "--optimize-"
        yield "--out:" + dllName
        yield "--doc:test.xml"
        yield "--warn:3"
        yield "--fullpaths"
        yield "--flaterrors"
        yield "--target:library"
        for x in fileNames do
            yield x
        let references = mkStandardProjectReferences ()
        for r in references do
            yield "-r:" + r
     |]
#endif

let mkTestFileAndOptions source additionalArgs =
    let fileName = Path.ChangeExtension(tryCreateTemporaryFileName (), ".fs")
    let project = tryCreateTemporaryFileName ()
    let dllName = Path.ChangeExtension(project, ".dll")
    let projFileName = Path.ChangeExtension(project, ".fsproj")
    let fileSource1 = "module M"
    FileSystem.OpenFileForWriteShim(fileName).Write(fileSource1)

    let args = Array.append (mkProjectCommandLineArgs (dllName, [fileName])) additionalArgs
    let options = checker.GetProjectOptionsFromCommandLineArgs (projFileName, args)
    fileName, options

let parseAndCheckFile fileName source options =
    match checker.ParseAndCheckFileInProject(fileName, 0, SourceText.ofString source, options) |> Async.RunImmediate with
    | parseResults, FSharpCheckFileAnswer.Succeeded(checkResults) -> parseResults, checkResults
    | _ -> failwithf "Parsing aborted unexpectedly..."

let parseAndCheckScriptWithOptions (file:string, input, opts) =

#if NETCOREAPP
    let projectOptions =
        let path = Path.Combine(Path.GetTempPath(), "tests", Process.GetCurrentProcess().Id.ToString() + "--"+ Guid.NewGuid().ToString())
        try
            if not (Directory.Exists(path)) then
                Directory.CreateDirectory(path) |> ignore

            let fname = Path.Combine(path, Path.GetFileName(file))
            let dllName = Path.ChangeExtension(fname, ".dll")
            let projName = Path.ChangeExtension(fname, ".fsproj")
            let args = mkProjectCommandLineArgsForScript (dllName, [file])
            printfn "file = %A, args = %A" file args
            checker.GetProjectOptionsFromCommandLineArgs (projName, args)

        finally
            if Directory.Exists(path) then
                Directory.Delete(path, true)

#else
    let projectOptions, _diagnostics = checker.GetProjectOptionsFromScript(file, SourceText.ofString input) |> Async.RunImmediate
    //printfn "projectOptions = %A" projectOptions
#endif

    let projectOptions = { projectOptions with OtherOptions = Array.append opts projectOptions.OtherOptions }
    let parseResult, typedRes = checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString input, projectOptions) |> Async.RunImmediate

    // if parseResult.Errors.Length > 0 then
    //     printfn "---> Parse Input = %A" input
    //     printfn "---> Parse Error = %A" parseResult.Errors

    match typedRes with
    | FSharpCheckFileAnswer.Succeeded(res) -> parseResult, res
    | res -> failwithf "Parsing did not finish... (%A)" res

let parseAndCheckScript (file, input) = parseAndCheckScriptWithOptions (file, input, [| |])
let parseAndCheckScript50 (file, input) = parseAndCheckScriptWithOptions (file, input, [| "--langversion:5.0" |])
let parseAndCheckScriptPreview (file, input) = parseAndCheckScriptWithOptions (file, input, [| "--langversion:preview" |])

let parseSourceCode (name: string, code: string) =
    let location = Path.Combine(Path.GetTempPath(),"test"+string(hash (name, code)))
    try Directory.CreateDirectory(location) |> ignore with _ -> ()
    let filePath = Path.Combine(location, name)
    let dllPath = Path.Combine(location, name + ".dll")
    let args = mkProjectCommandLineArgs(dllPath, [filePath])
    let options, errors = checker.GetParsingOptionsFromCommandLineArgs(List.ofArray args)
    let parseResults = checker.ParseFile(filePath, SourceText.ofString code, options) |> Async.RunImmediate
    parseResults.ParseTree

let matchBraces (name: string, code: string) =
    let location = Path.Combine(Path.GetTempPath(),"test"+string(hash (name, code)))
    try Directory.CreateDirectory(location) |> ignore with _ -> ()
    let filePath = Path.Combine(location, name + ".fs")
    let dllPath = Path.Combine(location, name + ".dll")
    let args = mkProjectCommandLineArgs(dllPath, [filePath])
    let options, errors = checker.GetParsingOptionsFromCommandLineArgs(List.ofArray args)
    let braces = checker.MatchBraces(filePath, SourceText.ofString code, options) |> Async.RunImmediate
    braces


let getSingleModuleLikeDecl (input: ParsedInput) =
    match input with
    | ParsedInput.ImplFile (ParsedImplFileInput (modules = [ decl ])) -> decl
    | _ -> failwith "Could not get module decls"

let getSingleModuleMemberDecls (input: ParsedInput) =
    let (SynModuleOrNamespace (decls = decls)) = getSingleModuleLikeDecl input
    decls

let getSingleDeclInModule (input: ParsedInput) =
    match getSingleModuleMemberDecls input with
    | [ decl ] -> decl
    | _ -> failwith "Can't get single module member declaration"

let getSingleExprInModule (input: ParsedInput) =
    match getSingleDeclInModule input with
    | SynModuleDecl.Expr (expr, _) -> expr
    | _ -> failwith "Unexpected expression"

let parseSourceCodeAndGetModule (source: string) =
    parseSourceCode ("test.fsx", source) |> getSingleModuleLikeDecl

/// Extract range info
let tups (m: range) = (m.StartLine, m.StartColumn), (m.EndLine, m.EndColumn)

/// Extract range info  and convert to zero-based line  - please don't use this one any more
let tupsZ (m: range) = (m.StartLine-1, m.StartColumn), (m.EndLine-1, m.EndColumn)

let attribsOfSymbolUse (symbolUse: FSharpSymbolUse) =
    [ if symbolUse.IsFromDefinition then yield "defn"
      if symbolUse.IsFromType then yield "type"
      if symbolUse.IsFromAttribute then yield "attribute"
      if symbolUse.IsFromDispatchSlotImplementation then yield "override"
      if symbolUse.IsFromPattern then yield "pattern"
      if symbolUse.IsFromComputationExpression then yield "compexpr" ]

let attribsOfSymbol (symbol: FSharpSymbol) =
    [ match symbol with
        | :? FSharpField as v ->
            yield "field"
            if v.IsCompilerGenerated then yield "compgen"
            if v.IsDefaultValue then yield "default"
            if v.IsMutable then yield "mutable"
            if v.IsVolatile then yield "volatile"
            if v.IsStatic then yield "static"
            if v.IsLiteral then yield sprintf "%A" v.LiteralValue.Value
            if v.IsAnonRecordField then
                let info, tys, i = v.AnonRecordFieldDetails
                yield "anon(" + string i + ", [" + info.Assembly.QualifiedName + "/" + String.concat "+" info.EnclosingCompiledTypeNames + "/" + info.CompiledName + "]" + String.concat "," info.SortedFieldNames + ")"


        | :? FSharpEntity as v ->
            v.TryFullName |> ignore // check there is no failure here
            if v.IsNamespace then yield "namespace"
            if v.IsFSharpModule then yield "module"
            if v.IsByRef then yield "byref"
            if v.IsClass then yield "class"
            if v.IsDelegate then yield "delegate"
            if v.IsEnum then yield "enum"
            if v.IsFSharpAbbreviation then yield "abbrev"
            if v.IsFSharpExceptionDeclaration then yield "exn"
            if v.IsFSharpRecord then yield "record"
            if v.IsFSharpUnion then yield "union"
            if v.IsInterface then yield "interface"
            if v.IsMeasure then yield "measure"
#if !NO_TYPEPROVIDERS
            if v.IsProvided then yield "provided"
            if v.IsStaticInstantiation then yield "staticinst"
            if v.IsProvidedAndErased then yield "erased"
            if v.IsProvidedAndGenerated then yield "generated"
#endif
            if v.IsUnresolved then yield "unresolved"
            if v.IsValueType then yield "valuetype"

        | :? FSharpActivePatternCase as v ->
            yield sprintf "apatcase%d" v.Index

        | :? FSharpMemberOrFunctionOrValue as v ->
            if v.IsActivePattern then yield "apat"
            if v.IsDispatchSlot then yield "slot"
            if v.IsModuleValueOrMember && not v.IsMember then yield "val"
            if v.IsMember then yield "member"
            if v.IsProperty then yield "prop"
            if v.IsExtensionMember then yield "extmem"
            if v.IsPropertyGetterMethod then yield "getter"
            if v.IsPropertySetterMethod then yield "setter"
            if v.IsEvent then yield "event"
            if v.EventForFSharpProperty.IsSome then yield "clievent"
            if v.IsEventAddMethod then yield "add"
            if v.IsEventRemoveMethod then yield "remove"
            if v.IsTypeFunction then yield "typefun"
            if v.IsCompilerGenerated then yield "compgen"
            if v.IsImplicitConstructor then yield "ctor"
            if v.IsMutable then yield "mutable"
            if v.IsOverrideOrExplicitInterfaceImplementation then yield "overridemem"
            if v.IsInstanceMember && not v.IsInstanceMemberInCompiledCode && not v.IsExtensionMember then yield "funky"
            if v.IsExplicitInterfaceImplementation then yield "intfmem"
//            if v.IsConstructorThisValue then yield "ctorthis"
//            if v.IsMemberThisValue then yield "this"
//            if v.LiteralValue.IsSome then yield "literal"
        | _ -> () ]

let rec allSymbolsInEntities compGen (entities: IList<FSharpEntity>) =
    [ for entity in entities do
          yield (entity :> FSharpSymbol)
          for gp in entity.GenericParameters do
            if compGen || not gp.IsCompilerGenerated then
             yield (gp :> FSharpSymbol)
          for x in entity.MembersFunctionsAndValues do
             if compGen || not x.IsCompilerGenerated then
               yield (x :> FSharpSymbol)
             for gp in x.GenericParameters do
              if compGen || not gp.IsCompilerGenerated then
               yield (gp :> FSharpSymbol)
          for x in entity.UnionCases do
             yield (x :> FSharpSymbol)
             for f in x.Fields do
                 if compGen || not f.IsCompilerGenerated then
                     yield (f :> FSharpSymbol)
          for x in entity.FSharpFields do
             if compGen || not x.IsCompilerGenerated then
                 yield (x :> FSharpSymbol)
          yield! allSymbolsInEntities compGen entity.NestedEntities ]


let getParseResults (source: string) =
    parseSourceCode("Test.fsx", source)

let getParseResultsOfSignatureFile (source: string) =
    parseSourceCode("Test.fsi", source)

let getParseAndCheckResults (source: string) =
    parseAndCheckScript("Test.fsx", source)

let getParseAndCheckResultsOfSignatureFile (source: string) =
    parseAndCheckScript("Test.fsi", source)

let getParseAndCheckResultsPreview (source: string) =
    parseAndCheckScriptPreview("Test.fsx", source)

let getParseAndCheckResults50 (source: string) =
    parseAndCheckScript50("Test.fsx", source)


let inline dumpDiagnostics (results: FSharpCheckFileResults) =
    results.Diagnostics
    |> Array.map (fun e ->
        let message =
            e.Message.Split('\n')
            |> Array.map (fun s -> s.Trim())
            |> String.concat " "
        sprintf "%s: %s" (e.Range.ToShortString()) message)
    |> List.ofArray

let getSymbolUses (results: FSharpCheckFileResults) =
    results.GetAllUsesOfAllSymbolsInFile()

let getSymbolUsesFromSource (source: string) =
    let _, typeCheckResults = getParseAndCheckResults source
    typeCheckResults.GetAllUsesOfAllSymbolsInFile()

let getSymbols (symbolUses: seq<FSharpSymbolUse>) =
    symbolUses |> Seq.map (fun symbolUse -> symbolUse.Symbol)

let getSymbolName (symbol: FSharpSymbol) =
    match symbol with
    | :? FSharpMemberOrFunctionOrValue as mfv -> Some mfv.LogicalName
    | :? FSharpEntity as entity -> Some entity.LogicalName
    | :? FSharpGenericParameter as genericParameter -> Some genericParameter.Name
    | :? FSharpParameter as parameter -> parameter.Name
    | :? FSharpStaticParameter as staticParameter -> Some staticParameter.Name
    | :? FSharpActivePatternCase as activePatternCase -> Some activePatternCase.Name
    | :? FSharpUnionCase as unionCase -> Some unionCase.Name
    | :? FSharpField as field -> Some field.Name
    | _ -> None

let getSymbolFullName (symbol: FSharpSymbol) =
    match symbol with
    | :? FSharpMemberOrFunctionOrValue as mfv -> Some mfv.FullName
    | :? FSharpEntity as entity -> entity.TryFullName
    | :? FSharpGenericParameter as genericParameter -> Some genericParameter.FullName
    | :? FSharpParameter as parameter -> Some parameter.FullName
    | :? FSharpStaticParameter as staticParameter -> Some staticParameter.FullName
    | :? FSharpActivePatternCase as activePatternCase -> Some activePatternCase.FullName
    | :? FSharpUnionCase as unioncase -> Some unioncase.FullName
    | :? FSharpField as field -> Some field.FullName
    | _ -> None

let assertContainsSymbolWithName name source =
    getSymbols source
    |> Seq.choose getSymbolName
    |> Seq.contains name
    |> shouldEqual true

let assertContainsSymbolsWithNames (names: string list) source =
    let symbolNames =
        getSymbols source
        |> Seq.choose getSymbolName

    for name in names do
        symbolNames
        |> Seq.contains name
        |> shouldEqual true

let assertHasSymbolUsages (names: string list) (results: FSharpCheckFileResults) =
    let symbolNames =
        getSymbolUses results
        |> getSymbols
        |> Seq.choose getSymbolName
        |> set

    for name in names do
        Assert.That(Set.contains name symbolNames, name)

let findSymbolUseByName (name: string) (results: FSharpCheckFileResults) =
    getSymbolUses results
    |> Seq.find (fun symbolUse ->
        match getSymbolName symbolUse.Symbol with
        | Some symbolName -> symbolName = name
        | _ -> false)

let findSymbolByName (name: string) (results: FSharpCheckFileResults) =
    let symbolUse = findSymbolUseByName name results
    symbolUse.Symbol

let findSymbolUse (evaluateSymbol:FSharpSymbolUse->bool) (results: FSharpCheckFileResults) =
    let symbolUses = getSymbolUses results
    symbolUses |> Seq.find (fun symbolUse -> evaluateSymbol symbolUse)

let taggedTextToString (tts: TaggedText[]) =
    tts |> Array.map (fun tt -> tt.Text) |> String.concat ""

let getRangeCoords (r: range) =
    (r.StartLine, r.StartColumn), (r.EndLine, r.EndColumn)

let coreLibAssemblyName =
#if NETCOREAPP
    "System.Runtime"
#else
    "mscorlib"
#endif

let assertRange
    (expectedStartLine: int, expectedStartColumn: int)
    (expectedEndLine: int, expectedEndColumn: int)
    (actualRange: range)
    : unit =
    Assert.AreEqual(Position.mkPos expectedStartLine expectedStartColumn, actualRange.Start)
    Assert.AreEqual(Position.mkPos expectedEndLine expectedEndColumn, actualRange.End)

