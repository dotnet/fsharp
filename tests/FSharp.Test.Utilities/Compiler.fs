// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open FSharp.Compiler.SourceCodeServices
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Assert
open FSharp.Test.Utilities.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open NUnit.Framework
open System
open System.Collections.Immutable
open System.IO

module Compiler =

    type TestType =
        | Text of string
        | Path of string
        | Baseline of (string * string)

    type CompilationUnit =
        | FS  of FSharpCompilationSource
        | CS  of CSharpCompilationSource
        | IL  of ILCompilationSource

    and FSharpCompilationSource =
        { Source:         TestType
          Options:        string list
          OutputType:     CompileOutput
          SourceKind:     SourceKind
          Name:           string option
          IgnoreWarnings: bool
          References:     CompilationUnit list }

    and CSharpCompilationSource =
        { Source:          TestType
          LangVersion:     CSharpLanguageVersion
          TargetFramework: TargetFramework
          Name:            string option
          References:      CompilationUnit list }

    and ILCompilationSource =
        { Source:     TestType
          References: CompilationUnit list}

    type ErrorSeverity = Error | Warning

    type ErrorInfo = { Severity:           ErrorSeverity
                       ErrorNumber:        int
                       StartLineAlternate: int
                       StartColumn:        int
                       EndLineAlternate:   int
                       EndColumn:          int
                       Message:            string }

    type Output = { OutputPath: string option
                    Adjust:     int
                    Errors:     ErrorInfo list
                    Warnings:   ErrorInfo list }

    type CompilationResult =
        | Success of Output
        | Failure of Output

    let private defaultOptions : string list = []

    // Not very safe version of reading stuff from file, but we want to fail fast for now if anything goes wrong.
    let private getSource (src: TestType) : string =
        match src with
        | Text t -> t
        | Path p -> System.IO.File.ReadAllText p
        | Baseline (d, f) -> System.IO.File.ReadAllText (System.IO.Path.Combine(d, f))

    let private fsFromString (source: string) (kind: SourceKind) : FSharpCompilationSource =
        match source with
        | null -> failwith "Source cannot be null"
        | _ -> { Source         = Text source;
                 Options        = defaultOptions;
                 OutputType     = Library;
                 SourceKind     = kind;
                 Name           = None;
                 IgnoreWarnings = false;
                 References     = [] }

    let private csFromString (source: string) : CSharpCompilationSource =
        match source with
        | null -> failwith "Source cannot be null"
        | _ -> { Source          = Text source;
                 LangVersion     = CSharpLanguageVersion.CSharp8;
                 TargetFramework = TargetFramework.NetCoreApp30;
                 Name            = None;
                 References      = [] }

    let private fromFSharpErrorInfo (errors: FSharpErrorInfo[]) : (ErrorInfo list * ErrorInfo list) =
        let toErrorInfo (e: FSharpErrorInfo) : ErrorInfo =
            { Severity           = if e.Severity = FSharpErrorSeverity.Warning then Warning else Error;
              ErrorNumber        = e.ErrorNumber;
              StartLineAlternate = e.StartLineAlternate;
              StartColumn        = e.StartColumn;
              EndLineAlternate   = e.EndLineAlternate;
              EndColumn          = e.EndColumn;
              Message            = e.Message }

        errors |> List.ofArray
               |> List.map toErrorInfo
               |> List.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
               |> List.partition  (fun e -> e.Severity = Error)

    let Fsx (source: string) : CompilationUnit =
        fsFromString source Fsx |> FS

    let FSharp (source: string) : CompilationUnit =
        fsFromString source Fs |> FS

    let baseline (dir: string, file: string) : CompilationUnit =
        match (dir, file) with
        | dir, _ when String.IsNullOrWhiteSpace dir -> failwith "Baseline tests directory cannot be null or empty."
        | _, file when String.IsNullOrWhiteSpace file -> failwith "Baseline source file name cannot be null or empty."
        | _ -> { Source         = Baseline (dir, file);
                 Options        = defaultOptions;
                 OutputType     = Library;
                 SourceKind     = Fs;
                 Name           = None;
                 IgnoreWarnings = false;
                 References     = [] } |> FS

    let CSharp (source: string) : CompilationUnit =
        csFromString source |> CS

    let withName (name: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS { src with Name = Some name }
        | CS src -> CS { src with Name = Some name }
        | IL _ -> failwith "IL Compilation cannot be named."

    let withReferences (references: CompilationUnit list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with References = fs.References @ references }
        | CS cs -> CS { cs with References = cs.References @ references }
        | IL _ -> failwith "References are not supported in IL"

    let withOptions (options: string list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with Options = options }
        | _ -> failwith "withOptions is only supported n F#"

    let asLibrary (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputType = CompileOutput.Library }
        | _ -> failwith "TODO: Implement where applicable."

    let asExe (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputType = CompileOutput.Exe }
        | _ -> failwith "TODO: Implement where applicable."

    let ignoreWarnings (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with IgnoreWarnings = true }
        | _ -> failwith "TODO: Implement ignorewarnings for the rest."

    let rec private asMetadataReference reference =
        match reference with
        | CompilationReference (cmpl, _) ->
            let result = compileFSharpCompilation cmpl false
            match result with
            | Failure f ->
                let message = sprintf "Compilation failed (expected to succeed).\n All errors:\n%A" (f.Errors @ f.Warnings)
                failwith message
            | Success s ->
                match s.OutputPath with
                    | None -> failwith "Compilation didn't produce any output!"
                    | Some p -> p |> MetadataReference.CreateFromFile
        | _ -> failwith "Conversion isn't possible"

    and private processReferences (references: CompilationUnit list) =

        let rec loop acc = function
            | [] -> List.rev acc
            | x::xs ->
                match x with
                | FS fs ->
                    let refs = loop [] fs.References
                    let source = getSource fs.Source
                    let name = if Option.isSome fs.Name then fs.Name.Value else null
                    let cmpl = Compilation.Create(source, fs.SourceKind, fs.OutputType, cmplRefs = refs, name = name) |> CompilationReference.CreateFSharp
                    loop (cmpl::acc) xs
                | CS cs ->
                    let refs = loop [] cs.References
                    let source = getSource cs.Source
                    let name = if Option.isSome cs.Name then cs.Name.Value else null
                    let metadataReferences = List.map asMetadataReference refs
                    metadataReferences |> ignore
                    // additionalReferences = ImmutableArray.CreateRange metadataReferences,
                    let cmpl = CompilationUtil.CreateCSharpCompilation(source, cs.LangVersion, cs.TargetFramework, name = name)
                            |> CompilationReference.Create
                    loop (cmpl::acc) xs
                | IL _ -> failwith "TODO: Process references for IL"
        loop [] references

    and private compileFSharpCompilation compilation ignoreWarnings : CompilationResult =

        let ((err: FSharpErrorInfo[], outputFilePath: string), _) = CompilerAssert.CompileRaw(compilation)

        let (errors, warnings) = err |> fromFSharpErrorInfo

        let result = { OutputPath  = None;
                       Adjust      = 0;
                       Warnings    = warnings;
                       Errors      = errors }

        // Treat warnings as errors if "IgnoreWarnings" is false;
        if errors.Length > 0 || (warnings.Length > 0 && not ignoreWarnings) then
            Failure { result with Warnings = warnings;
                                  Errors   = errors }
        else
            Success { result with Warnings   = warnings;
                                  OutputPath = Some outputFilePath }

    and private compileFSharp (fsSource: FSharpCompilationSource) : CompilationResult =

        let source = getSource fsSource.Source
        let sourceKind = fsSource.SourceKind
        let output = fsSource.OutputType
        let options = fsSource.Options |> Array.ofList

        let references = processReferences fsSource.References

        let compilation = Compilation.Create(source, sourceKind, output, options, references)

        compileFSharpCompilation compilation fsSource.IgnoreWarnings

    and private compileCSharpCompilation (compilation: CSharpCompilation) : CompilationResult =

        let outputPath = Path.Combine(Path.GetTempPath(), "FSharpCompilerTests", Path.GetRandomFileName())

        Directory.CreateDirectory(outputPath) |> ignore

        let filename = compilation.AssemblyName
        let output = Path.Combine(outputPath, Path.ChangeExtension(filename, ".dll"))

        let cmplResult = compilation.Emit (output)

        let result = { OutputPath  = None;
                       Adjust      = 0;
                       Warnings    = [];
                       Errors      = [] }

        if cmplResult.Success then
            Success { result with OutputPath  = Some output }
        else
            cmplResult.Diagnostics |> printfn "%A"
            Failure result

    and private compileCSharp (csSource: CSharpCompilationSource) : CompilationResult =

        let source = getSource csSource.Source
        let name = if Option.isSome csSource.Name then csSource.Name.Value else Guid.NewGuid().ToString ()

        let additionalReferences =
            match processReferences csSource.References with
            | [] -> ImmutableArray.Empty
            | r  -> (List.map asMetadataReference r).ToImmutableArray().As<MetadataReference>()

        let references = TargetFrameworkUtil.getReferences csSource.TargetFramework

        let lv =
          match csSource.LangVersion with
            | CSharpLanguageVersion.CSharp8 -> LanguageVersion.CSharp8
            | _ -> LanguageVersion.Default

        let cmpl =
          CSharpCompilation.Create(
            name,
            [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
            references.As<MetadataReference>().AddRange additionalReferences,
            CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))

        cmpl |> compileCSharpCompilation

    and compile (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> compileFSharp fs
        | CS cs -> compileCSharp cs
        | _ -> failwith "TODO"

    let private typecheckFSharpWithBaseline (options: string list) (dir: string) (file: string) : CompilationResult =
        // Since TypecheckWithErrorsAndOptionsAgainsBaseLine throws if doesn't match expected baseline,
        // We return a successfull CompilationResult if it succeeds.
        CompilerAssert.TypeCheckWithErrorsAndOptionsAgainstBaseLine (Array.ofList options) dir file

        Success { OutputPath  = None;
                  Adjust      = 0;
                  Warnings    = [];
                  Errors      = [] }

    let private typecheckFSharpSource (fsSource: FSharpCompilationSource) : CompilationResult =
        let source = getSource fsSource.Source
        let options = fsSource.Options |> Array.ofList

        let (err: FSharpErrorInfo []) = CompilerAssert.TypeCheckWithOptions options source

        let (errors, warnings) = err |> fromFSharpErrorInfo

        let result = { OutputPath  = None;
                       Adjust      = 0;
                       Warnings    = warnings;
                       Errors      = errors }

        // Treat warnings as errors if "IgnoreWarnings" is false;
        if errors.Length > 0 || (warnings.Length > 0 && not fsSource.IgnoreWarnings) then
            Failure { result with Warnings = warnings;
                                  Errors   = errors }
        else
            Success { result with Warnings   = warnings }

    let private typecheckFSharp (fsSource: FSharpCompilationSource) : CompilationResult =
        match fsSource.Source with
        | Baseline (f, d) -> typecheckFSharpWithBaseline fsSource.Options f d
        | _ -> typecheckFSharpSource fsSource

    let typecheck (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> typecheckFSharp fs
        | _ -> failwith "Typecheck only supports F#"

    let run (cResult: CompilationResult ) : unit =
        match cResult with
        | Failure o -> failwith (sprintf "Compilation should be successfull in order to run.\n Errors: %A" (o.Errors @ o.Warnings))
        | Success s ->
            match s.OutputPath with
            | None -> failwith "Compilation didn't produce any output. Unable to run. (did you forget to set output type to Exe?)"
            | Some p -> CompilerAssert.Run p

    let compileAndRun = compile >> run

    let compileExeAndRun = asExe >> compileAndRun

    [<AutoOpen>]
    module Assertions =
        let private getErrorInfo (info: ErrorInfo) : string =
            sprintf "%A %A %A" info.Severity info.ErrorNumber info.Message

        let inline private assertErrorsLength (source: ErrorInfo list) (expected: 'a list) : unit =
            if (List.length source) <> (List.length expected) then
                failwith (sprintf "Expected list of issues differ from compilation result:\nExpected:\n %A\nActual:\n %A" expected (List.map getErrorInfo source))
            ()

        let private assertErrorMessages (source: ErrorInfo list) (expected: string list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: ErrorInfo) -> el.Message = exp) source) then
                    failwith (sprintf "Mismatch in error message, expected '%A' was not found during compilation.\nAll errors:\n%A" exp (List.map getErrorInfo source))
            assertErrorsLength source expected

        let private assertErrorNumbers (source: ErrorInfo list) (expected: int list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: ErrorInfo) -> el.ErrorNumber = exp) source) then
                    failwith (sprintf "Mismatch in ErrorNumber, expected '%A' was not found during compilation.\nAll errors:\n%A" exp (List.map getErrorInfo source))
            assertErrorsLength source expected

        let private assertErrors (what: string) libAdjust (source: ErrorInfo list) (expected: (int * (int * int * int * int) * string) list) : unit =
            let errors =
                source
                |> List.map (fun info -> (info.ErrorNumber, (info.StartLineAlternate - libAdjust, info.StartColumn + 1, info.EndLineAlternate - libAdjust, info.EndColumn + 1), info.Message))

            let inline checkEqual k a b =
                if a <> b then
                    Assert.AreEqual(a, b, sprintf "%s: Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A" what k a b errors)

            // TODO: Check all "categories", collect all results and print alltogether.
            checkEqual "Errors count"  expected.Length errors.Length

            List.zip errors expected
            |> List.iter (fun (actualError, expectedError) ->
                           let (expectedErrorNumber, expectedErrorRange, expectedErrorMsg) = expectedError
                           let (actualErrorNumber, actualErrorRange, actualErrorMsg) = actualError
                           checkEqual "ErrorNumber" expectedErrorNumber actualErrorNumber
                           checkEqual "ErrorRange" expectedErrorRange actualErrorRange
                           checkEqual "Message" expectedErrorMsg actualErrorMsg)
            ()

        let adjust (adjust: int) (result: CompilationResult) : CompilationResult =
            match result with
            | Success s -> Success { s with Adjust = adjust }
            | Failure f -> Failure { f with Adjust = adjust }

        let shouldSucceed (result: CompilationResult) : CompilationResult =
            match result with
            | Success _ -> result
            | Failure r ->
                let message = sprintf "Compilation failed (expected to succeed).\n All errors:\n%A" (r.Errors @ r.Warnings)
                failwith message

        let shouldFail (result: CompilationResult) : CompilationResult =
            match result with
            | Success _ -> failwith "Compilation succeded (expected: Failure)."
            | Failure _ -> result

        let withMessages (messages: string list) (result: CompilationResult) : CompilationResult =
             match result with
             | Success r | Failure r -> assertErrorMessages (r.Warnings @ r.Errors) messages
             result

        let withMessage (message: string) (result: CompilationResult) : CompilationResult =
            withMessages [message] result

        let withErrorMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r -> assertErrorMessages r.Errors messages
            result

        let withErrorMessage (message: string) (result: CompilationResult) : CompilationResult =
            withErrorMessages [message] result

        let withWarningMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r -> assertErrorMessages r.Warnings messages
            result

        let withWarningMessage (message: string) (result: CompilationResult) : CompilationResult =
            withWarningMessages [message] result

        let withWarnings (expectedWarnings: (int * (int * int * int * int) * string) list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertErrors "Warnings" r.Adjust r.Warnings expectedWarnings
            result

        let withWarning (expectedWarning: (int * (int * int * int * int) * string)) (result: CompilationResult) : CompilationResult =
            withWarnings [expectedWarning] result

        let withWarningCodes (expectedWarnings: int list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertErrorNumbers r.Warnings expectedWarnings
            result

        let withWarningCode (expectedWarning: int) (result: CompilationResult) : CompilationResult =
           withWarningCodes [expectedWarning] result

        let withErrors (expectedErrors: (int * (int * int * int * int) * string) list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertErrors "Errors" r.Adjust r.Errors expectedErrors
            result

        let withError (expectedError: (int * (int * int * int * int) * string)) (result: CompilationResult) : CompilationResult =
            withErrors [expectedError] result

        let withErrorCodes (expectedErrors: int list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertErrorNumbers r.Errors expectedErrors
            result

        let withErrorCode (expectedError: int) (result: CompilationResult) : CompilationResult =
            withErrorCodes [expectedError] result

        let withRange = ignore
