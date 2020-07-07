// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open FSharp.Compiler.SourceCodeServices

module Compiler =

    type SourceType =
        | Text of string
        | Path of string

    type OutputType = Library | Exe

    type CompilationUnit =
        | FS  of CompilationSource
        | FSX of CompilationSource
        | CS  of CompilationSource
        | CSx of CompilationSource
        | IL  of CompilationSource

    and CompilationSource =
        { Source:     SourceType
          Options:    string list
          OutputType: OutputType
          References: CompilationUnit list }

    type CompilationOutput = { Compilation: CompilationUnit
                               OutputPath:  string option
                               Errors:      FSharpErrorInfo list
                               Warnings:    FSharpErrorInfo list }

    type CompilationResult =
        | Success of CompilationOutput
        | Failure of CompilationOutput

    let private defaultOptions : string list = []

    let Fsx (source: string) : CompilationUnit =
        match source with
        | null -> failwith "Source cannot be null"
        | _ -> { Source     = Text source;
                 Options    = defaultOptions;
                 OutputType = Library
                 References = [] } |> CompilationUnit.FSX

    let FSharp (source: string) : CompilationUnit =
        match source with
        | null -> failwith "Source cannot be null"
        | _ -> { Source     = Text source;
                 Options    = defaultOptions;
                 OutputType = Library
                 References = [] } |> CompilationUnit.FS

    let CSharp (_: string) : CompilationUnit = failwith "TODO"
    
    let IL (_: string) : CompilationUnit = failwith "TODO"

    let withOptions (_: string list) (_: CompilationUnit) : CompilationUnit option = failwith "TODO"
    let asLibrary (_: CompilationUnit) : CompilationUnit = failwith "TODO"
    let asExe (_: CompilationUnit) : CompilationUnit = failwith "TODO"

    let compile (cUnit: CompilationUnit) : CompilationResult =
        let cSource = match cUnit with
                      | FS f | FSX f -> f
                      | _ -> failwith "TODO"

        let source = match cSource.Source with
                     | Text t -> t
                     | _ -> failwith "TODO"

        let sourceKind = match cUnit with
                         | FS _ -> SourceKind.Fs
                         | FSX _ -> SourceKind.Fsx
                         | _ -> failwith "TODO"

        let output = (if cSource.OutputType = Exe then CompileOutput.Exe else CompileOutput.Library)
        let options = cSource.Options |> Array.ofList
        let references = [] // cSource.References

        let compilation = Compilation.Create(source, sourceKind, output, options, references)

        let ((err: FSharpErrorInfo[], outputFilePath: string), _) = CompilerAssert.CompileRaw(compilation)

        let (errors, warnings) = err |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
                                     |> Array.partition  (fun e -> e.Severity = FSharpErrorSeverity.Error)
                                     |> fun (f, s) -> f |> List.ofArray, s |> List.ofArray

        let result = { Compilation = cUnit;
                       OutputPath  = None;
                       Warnings    = warnings;
                       Errors      = errors }

        if err.Length > 0 then
            Failure { result with Warnings = warnings;
                                  Errors   = errors }
        else
            Success { result with Warnings   = warnings;
                                  OutputPath = Some outputFilePath }

    // TODO: baseline helpers
    let typecheck (_: CompilationUnit option) = failwith "TODO"

    let execute (_: CompilationUnit option) = failwith "TODO"

    let getIL (_: CompilationUnit option) = failwith "TODO"

    [<AutoOpen>]
    // TODO: Reuse FluentAssertions' assertions here.
    module Assertions =
        
        let private getErrorInfo (info: FSharpErrorInfo) : string =
            sprintf "%A %A %A" info.Severity info.ErrorNumber info.Message

        let shouldSucceed (result: CompilationResult) : CompilationResult =
            match result with
            | Success _ -> result
            | Failure _ -> failwith "Compilation failed (expected: Success)."

        let shouldFail (result: CompilationResult) : CompilationResult =
            match result with
            | Failure _ -> result
            | Success _ -> failwith "Compilation succeded (expected: Failure)."

        // TODO: Better error messages.
        let private assertFSharpErrorInfo (source: FSharpErrorInfo list) (expected: int list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: FSharpErrorInfo) -> el.ErrorNumber = exp) source) then
                    failwith (sprintf "Expected issue '%A' was not found during compilation.\nAll messages:\n%A" exp (List.map getErrorInfo source))

            if (List.length source) <> (List.length expected) then
                failwith (sprintf "Expected list of issues differ from compilation result:\nExpected:\n %A\nActual:\n %A" expected (List.map getErrorInfo source))
                
        // TODO: withWarnings and withErrors + Ranges and mesages checking
        let withWarnings (expectedWarnings: int list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertFSharpErrorInfo r.Warnings expectedWarnings

            result

        let withErrors (expectedErrors: int list) (result: CompilationResult) : CompilationResult =
            match result with
            | Success r | Failure r ->
                assertFSharpErrorInfo r.Errors expectedErrors

            result
