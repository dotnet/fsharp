﻿module TypeChecks.TestUtils

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.Text
open FSharp.Compiler.Syntax

let private checker = FSharpChecker.Create()

let parseSourceCode (name: string, code: string) =
    let sourceText = SourceText.ofString code

    let parsingOptions =
        { FSharpParsingOptions.Default with
            SourceFiles = [| name |]
        }

    let result =
        checker.ParseFile(name, sourceText, parsingOptions) |> Async.RunSynchronously

    result.ParseTree

type TestFileWithAST =
    {
        Idx: int
        File: string
        AST: ParsedInput
    }
    
    static member internal Map (x:TestFileWithAST) : FileInProject =
        {
            Idx = x.Idx
            FileName = x.File
            ParsedInput = x.AST
        }