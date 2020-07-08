// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open FSharp.Test.Utilities.Assert
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open NUnit.Framework

module Compiler =

    type SourceType =
        | Text of string
        | Path of string

    type CompilationUnit =
        | FS  of FSharpCompilationSource
        | CS  of CSharpCompilationSource
        | IL  of ILCompilationSource

    and FSharpCompilationSource =
        { Source:         SourceType
          Options:        string list
          OutputType:     CompileOutput
          SourceKind:     SourceKind
          Name      :     string option
          IgnoreWarnings: bool
          References:     CompilationUnit list }

    and CSharpCompilationSource =
        { Source:          SourceType
          LangVersion:     CSharpLanguageVersion
          TargetFramework: TargetFramework
          Name:            string option
          References:      CompilationUnit list }

    and ILCompilationSource =
        { Source:     SourceType
          References: CompilationUnit list}

    type CompilationOutput = { OutputPath:  string option
                               Adjust:      int
                               Errors:      FSharpErrorInfo list
                               Warnings:    FSharpErrorInfo list }

    type CompilationResult =
        | Success of CompilationOutput
        | Failure of CompilationOutput

    let private defaultOptions : string list = []

    // Not very safe version of reading stuff from file, but we want to fail fast for now if anything goes wrong.
    let private getSource (src: SourceType) : string =
        match src with
        | Text t -> t
        | Path p -> System.IO.File.ReadAllText p

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

    let private fromFile (_: string) : CompilationUnit = failwith "TODO"

    let Fsx (source: string) : CompilationUnit =
        fsFromString source SourceKind.Fsx |> CompilationUnit.FS

    let FSharp (source: string) : CompilationUnit =
        fsFromString source SourceKind.Fs |> CompilationUnit.FS

    let CSharp (source: string) : CompilationUnit =
        csFromString source |> CompilationUnit.CS

    let withName (name: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> CompilationUnit.FS { src with Name = Some name }
        | CS src -> CompilationUnit.CS { src with Name = Some name }
        | IL _ -> failwith "TODO: Implement named IL"

    let withReferences (references: CompilationUnit list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with References = fs.References @ references }
        | CS cs -> CS { cs with References = cs.References @ references }
        | IL _ -> failwith "TODO: Support references for IL"

    // TODO: C# and IL versions where applicable
    let withOptions (options: string list) (src: FSharpCompilationSource) : CompilationUnit =
        CompilationUnit.FS { src with Options = options }

    let asLibrary (src: FSharpCompilationSource) : CompilationUnit =
        CompilationUnit.FS { src with OutputType = CompileOutput.Library }

    let asExe (src: FSharpCompilationSource) : CompilationUnit =
        CompilationUnit.FS { src with OutputType = CompileOutput.Exe }

    let ignoreWarnings (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> CompilationUnit.FS { fs with IgnoreWarnings = true }
        | _ -> failwith "TODO: Implement ignorewarnings for the rest."

    let private processReferences (references: CompilationUnit list) =
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
                    let source = getSource cs.Source
                    // TODO: reference support for C#, convert CompilationReference to MetadataReference
                    let name = if Option.isSome cs.Name then cs.Name.Value else null
                    let cmpl = CompilationUtil.CreateCSharpCompilation(source, cs.LangVersion, cs.TargetFramework, name = name) |> CompilationReference.Create
                    loop (cmpl::acc) xs
                | IL _ -> failwith "TODO: Process references for IL"
        loop [] references

    let private compileFSharp (fsSource: FSharpCompilationSource) : CompilationResult =

        let source = getSource fsSource.Source
        let sourceKind = fsSource.SourceKind
        let output = fsSource.OutputType
        let options = fsSource.Options |> Array.ofList

        let references = processReferences fsSource.References

        let compilation = Compilation.Create(source, sourceKind, output, options, references)

        let ((err: FSharpErrorInfo[], outputFilePath: string), _) = CompilerAssert.CompileRaw(compilation)

        let (errors, warnings) = err |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
                                     |> Array.partition  (fun e -> e.Severity = FSharpErrorSeverity.Error)
                                     |> fun (f, s) -> f |> List.ofArray, s |> List.ofArray

        let result = { OutputPath  = None;
                       Adjust      = 0;
                       Warnings    = warnings;
                       Errors      = errors }

        // Treat warnings as errors if "IgnoreWarnings" is false;
        if errors.Length > 0 || (warnings.Length > 0 && not fsSource.IgnoreWarnings) then
            Failure { result with Warnings = warnings;
                                  Errors   = errors }
        else
            Success { result with Warnings   = warnings;
                              OutputPath = Some outputFilePath }

    let compile (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> compileFSharp fs
        | _ -> failwith "TODO"

    // TODO: Typecheck with baseline
    let parse (_: CompilationUnit option) = failwith "TODO"

    let typecheck (_: CompilationUnit option) = failwith "TODO"

    let execute (_: CompilationUnit option) = failwith "TODO"

    let run (_: CompilationUnit option) = failwith "TODO"

    let getIL (_: CompilationUnit option) = failwith "TODO"


    [<AutoOpen>]
    // TODO: Reuse FluentAssertions' assertions here.
    module Assertions =

        let private getErrorInfo (info: FSharpErrorInfo) : string =
            sprintf "%A %A %A" info.Severity info.ErrorNumber info.Message

        // TODO: Better error messages.
        // TODO: Should probably generalize/dedupicate asserts
        let private assertErrorsLength (source: FSharpErrorInfo list) (expected: int list) : unit =
            if (List.length source) <> (List.length expected) then
                failwith (sprintf "Expected list of issues differ from compilation result:\nExpected:\n %A\nActual:\n %A" expected (List.map getErrorInfo source))
            ()

        let private assertErrorNumber (source: FSharpErrorInfo list) (expected: int list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: FSharpErrorInfo) -> el.ErrorNumber = exp) source) then
                    failwith (sprintf "Mismatch in ErrorNumber, expected '%A' was not found during compilation.\nAll messages:\n%A" exp (List.map getErrorInfo source))
            assertErrorsLength source expected

        let private assertErrors (what: string) libAdjust (source: FSharpErrorInfo list) (expected: (int * (int * int * int * int) * string) list) : unit =
            let errors =
                source
                |> List.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
                |> List.map (fun info -> (info.ErrorNumber, (info.StartLineAlternate - libAdjust, info.StartColumn + 1, info.EndLineAlternate - libAdjust, info.EndColumn + 1), info.Message))

            let inline checkEqual k a b =
                if a <> b then
                    Assert.AreEqual(a, b, sprintf "%s: Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A" what k a b errors)

            checkEqual "Errors"  expected.Length errors.Length

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
            | Failure _ -> failwith "Compilation failed (expected: Success)."

        let shouldFail (result: CompilationResult) : CompilationResult =
            match result with
            | Failure _ -> result
            | Success _ -> failwith "Compilation succeded (expected: Failure)."

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
                assertErrorNumber r.Warnings expectedWarnings

            result

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
                assertErrorNumber r.Errors expectedErrors

            result
