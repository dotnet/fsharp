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

module rec Compiler =

    type TestType =
        | Text of string
        | Path of string
        | Baseline of (string * string)

    type CompilationUnit =
        | FS  of FSharpCompilationSource
        | CS  of CSharpCompilationSource
        | IL  of ILCompilationSource

    type FSharpCompilationSource =
        { Source:         TestType
          Options:        string list
          OutputType:     CompileOutput
          SourceKind:     SourceKind
          Name:           string option
          IgnoreWarnings: bool
          References:     CompilationUnit list }

    type CSharpCompilationSource =
        { Source:          TestType
          LangVersion:     CSharpLanguageVersion
          TargetFramework: TargetFramework
          Name:            string option
          References:      CompilationUnit list }

    type ILCompilationSource =
        { Source:     TestType
          References: CompilationUnit list  }

    type ErrorType = Error of int | Warning of int

    type Line = Line of int
    type Col = Col of int

    type Range =
          { StartLine:   int
            StartColumn: int
            EndLine:     int
            EndColumn:   int }

    type ErrorInfo =
        { Error:   ErrorType
          Range:   Range
          Message: string }

    type Output =
        { OutputPath: string option
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
        | _ ->
            { Source         = Text source
              Options        = defaultOptions
              OutputType     = Library
              SourceKind     = kind
              Name           = None
              IgnoreWarnings = false
              References     = [] }

    let private csFromString (source: string) : CSharpCompilationSource =
        match source with
        | null -> failwith "Source cannot be null"
        | _ ->
            { Source          = Text source
              LangVersion     = CSharpLanguageVersion.CSharp8
              TargetFramework = TargetFramework.NetCoreApp30
              Name            = None
              References      = [] }

    let private fromFSharpErrorInfo (errors: FSharpErrorInfo[]) : (ErrorInfo list * ErrorInfo list) =
        let toErrorInfo (e: FSharpErrorInfo) : ErrorInfo =
            let errorNumber = e.ErrorNumber
            let severity = e.Severity

            let error = if severity = FSharpErrorSeverity.Warning then Warning errorNumber else Error errorNumber

            { Error   = error
              Range   =
                  { StartLine   = e.StartLineAlternate
                    StartColumn = e.StartColumn
                    EndLine     = e.EndLineAlternate
                    EndColumn   = e.EndColumn }
              Message = e.Message }

        errors
        |> List.ofArray
        |> List.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
        |> List.map toErrorInfo
        |> List.partition  (fun e -> match e.Error with Error _ -> true | _ -> false)

    let private adjustRange (range: Range) (adjust: int) : Range =
        { range with
                StartLine   = range.StartLine   - adjust
                StartColumn = range.StartColumn + 1
                EndLine     = range.EndLine     - adjust
                EndColumn   = range.EndColumn   + 1 }

    let Fsx (source: string) : CompilationUnit =
        fsFromString source SourceKind.Fsx |> FS

    let FSharp (source: string) : CompilationUnit =
        fsFromString source SourceKind.Fs |> FS

    let baseline (dir: string, file: string) : CompilationUnit =
        match (dir, file) with
        | dir, _ when String.IsNullOrWhiteSpace dir -> failwith "Baseline tests directory cannot be null or empty."
        | _, file when String.IsNullOrWhiteSpace file -> failwith "Baseline source file name cannot be null or empty."
        | _ ->
            { Source         = Baseline (dir, file)
              Options        = defaultOptions
              OutputType     = Library
              SourceKind     = Fs
              Name           = None
              IgnoreWarnings = false
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

    let private processReferences (references: CompilationUnit list) =
        let rec loop acc = function
            | [] -> List.rev acc
            | x::xs ->
                match x with
                | FS fs ->
                    let refs = loop [] fs.References
                    let source = getSource fs.Source
                    let name = defaultArg fs.Name null
                    let cmpl = Compilation.Create(source, fs.SourceKind, fs.OutputType, cmplRefs = refs, name = name) |> CompilationReference.CreateFSharp
                    loop (cmpl::acc) xs
                | CS cs ->
                    let refs = loop [] cs.References
                    let source = getSource cs.Source
                    let name = defaultArg cs.Name null
                    let metadataReferences = List.map asMetadataReference refs
                    let cmpl = CompilationUtil.CreateCSharpCompilation(source, cs.LangVersion, cs.TargetFramework, additionalReferences = metadataReferences.ToImmutableArray().As<MetadataReference>(), name = name)
                            |> CompilationReference.Create
                    loop (cmpl::acc) xs
                | IL _ -> failwith "TODO: Process references for IL"
        loop [] references

    let private compileFSharpCompilation compilation ignoreWarnings : CompilationResult =

        let ((err: FSharpErrorInfo[], outputFilePath: string), _) = CompilerAssert.CompileRaw(compilation)

        let (errors, warnings) = err |> fromFSharpErrorInfo

        let result =
            { OutputPath  = None
              Adjust      = 0
              Warnings    = warnings
              Errors      = errors }

        // Treat warnings as errors if "IgnoreWarnings" is false
        if errors.Length > 0 || (warnings.Length > 0 && not ignoreWarnings) then
            Failure { result with Warnings = warnings
                                  Errors   = errors }
        else
            Success { result with Warnings   = warnings
                                  OutputPath = Some outputFilePath }

    let private compileFSharp (fsSource: FSharpCompilationSource) : CompilationResult =

        let source = getSource fsSource.Source
        let sourceKind = fsSource.SourceKind
        let output = fsSource.OutputType
        let options = fsSource.Options |> Array.ofList

        let references = processReferences fsSource.References

        let compilation = Compilation.Create(source, sourceKind, output, options, references)

        compileFSharpCompilation compilation fsSource.IgnoreWarnings

    let private compileCSharpCompilation (compilation: CSharpCompilation) : CompilationResult =

        let outputPath = Path.Combine(Path.GetTempPath(), "FSharpCompilerTests", Path.GetRandomFileName())

        Directory.CreateDirectory(outputPath) |> ignore

        let filename = compilation.AssemblyName
        let output = Path.Combine(outputPath, Path.ChangeExtension(filename, ".dll"))

        let cmplResult = compilation.Emit (output)

        let result =
            { OutputPath  = None
              Adjust      = 0
              Warnings    = []
              Errors      = [] }

        if cmplResult.Success then
            Success { result with OutputPath  = Some output }
        else
            Failure result

    let private compileCSharp (csSource: CSharpCompilationSource) : CompilationResult =

        let source = getSource csSource.Source
        let name = defaultArg csSource.Name (Guid.NewGuid().ToString ())

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

    let compile (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> compileFSharp fs
        | CS cs -> compileCSharp cs
        | _ -> failwith "TODO"

    let private typecheckFSharpWithBaseline (options: string list) (dir: string) (file: string) : CompilationResult =
        // Since TypecheckWithErrorsAndOptionsAgainsBaseLine throws if doesn't match expected baseline,
        // We return a successfull CompilationResult if it succeeds.
        CompilerAssert.TypeCheckWithErrorsAndOptionsAgainstBaseLine (Array.ofList options) dir file

        Success
            { OutputPath  = None
              Adjust      = 0
              Warnings    = []
              Errors      = [] }

    let private typecheckFSharpSource (fsSource: FSharpCompilationSource) : CompilationResult =
        let source = getSource fsSource.Source
        let options = fsSource.Options |> Array.ofList

        let (err: FSharpErrorInfo []) = CompilerAssert.TypeCheckWithOptions options source

        let (errors, warnings) = err |> fromFSharpErrorInfo

        let result =
            { OutputPath  = None
              Adjust      = 0
              Warnings    = warnings
              Errors      = errors }

        // Treat warnings as errors if "IgnoreWarnings" is false;
        if errors.Length > 0 || (warnings.Length > 0 && not fsSource.IgnoreWarnings) then
            Failure { result with Warnings = warnings
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
        let private getErrorNumber (error: ErrorType) : int =
            match error with
            | Error e | Warning e -> e

        let private getErrorInfo (info: ErrorInfo) : string =
            sprintf "%A %A" info.Error info.Message

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
                if not (List.exists (fun (el: ErrorInfo) -> (getErrorNumber el.Error) = exp) source) then
                    failwith (sprintf "Mismatch in ErrorNumber, expected '%A' was not found during compilation.\nAll errors:\n%A" exp (List.map getErrorInfo source))
            assertErrorsLength source expected

        let private assertErrors (what: string) libAdjust (source: ErrorInfo list) (expected: ErrorInfo list) : unit =
            let errors = source |> List.map (fun error -> { error with Range = adjustRange error.Range libAdjust })

            let inline checkEqual k a b =
             if a <> b then
                 Assert.AreEqual(a, b, sprintf "%s: Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A" what k a b errors)

            // TODO: Check all "categories", collect all results and print alltogether.
            checkEqual "Errors count"  expected.Length errors.Length

            List.zip errors expected
            |> List.iter (fun (actualError, expectedError) ->
                           let { Error = actualError; Range = actualRange; Message = actualMessage } = actualError
                           let { Error = expectedError; Range = expectedRange; Message = expectedMessage } = expectedError
                           checkEqual "Error" expectedError actualError
                           checkEqual "ErrorRange" expectedRange actualRange
                           checkEqual "Message" expectedMessage actualMessage)
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
            | Success _ -> failwith "Compilation was \"Success\" (expected: \"Failure\")."
            | Failure _ -> result

        let private assertResultsCategory (what: string) (selector: Output -> ErrorInfo list) (expected: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
             | Success r | Failure r ->
                assertErrors what r.Adjust (selector r) expected
            result

        let withResults (expectedResults: ErrorInfo list) result : CompilationResult =
            assertResultsCategory "Results" (fun r -> r.Warnings @ r.Errors) expectedResults result

        let withResult (expectedResult: ErrorInfo ) (result: CompilationResult) : CompilationResult =
            withResults [expectedResult] result

        let withDiagnostics (expected: (ErrorType * Line * Col * Line * Col * string) list) (result: CompilationResult) : CompilationResult =
            let (expectedResults: ErrorInfo list) =
                expected |>
                List.map(
                    fun e ->
                      let (error, (Line startLine), (Col startCol), (Line endLine), (Col endCol), message) = e
                      { Error = error
                        Range =
                            { StartLine   = startLine
                              StartColumn = startCol
                              EndLine     = endLine
                              EndColumn   = endCol }
                        Message     = message })
            withResults expectedResults result

        let withSingleDiagnostic (expected: (ErrorType * Line * Col * Line * Col * string)) (result: CompilationResult) : CompilationResult =
            withDiagnostics [expected] result

        let withErrors (expectedErrors: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            assertResultsCategory "Errors" (fun r -> r.Errors) expectedErrors result

        let withError (expectedError: ErrorInfo) (result: CompilationResult) : CompilationResult =
            withErrors [expectedError] result

        let checkCodes (expected: int list) (selector: Output -> ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertErrorNumbers (selector r) expected
            result

        let withErrorCodes (expectedCodes: int list) (result: CompilationResult) : CompilationResult =
            checkCodes expectedCodes (fun r -> r.Errors) result

        let withErrorCode (expectedCode: int) (result: CompilationResult) : CompilationResult =
            withErrorCodes [expectedCode] result

        let withWarnings (expectedWarnings: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            assertResultsCategory "Warnings" (fun r -> r.Warnings) expectedWarnings result

        let withWarning (expectedWarning: ErrorInfo) (result: CompilationResult) : CompilationResult =
            withWarnings [expectedWarning] result

        let withWarningCodes (expectedCodes: int list) (result: CompilationResult) : CompilationResult =
            checkCodes expectedCodes (fun r -> r.Warnings) result

        let withWarningCode (expectedCode: int) (result: CompilationResult) : CompilationResult =
            withWarningCodes [expectedCode] result

        let private checkErrorMessages (messages: string list) (selector: Output -> ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r -> assertErrorMessages (selector r) messages
            result

        let withMessages (messages: string list) (result: CompilationResult) : CompilationResult =
             checkErrorMessages messages (fun r -> r.Warnings @ r.Errors) result

        let withMessage (message: string) (result: CompilationResult) : CompilationResult =
            withMessages [message] result

        let withErrorMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            checkErrorMessages messages (fun r -> r.Errors) result

        let withErrorMessage (message: string) (result: CompilationResult) : CompilationResult =
            withErrorMessages [message] result

        let withWarningMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            checkErrorMessages messages (fun r -> r.Warnings) result

        let withWarningMessage (message: string) (result: CompilationResult) : CompilationResult =
            withWarningMessages [message] result
