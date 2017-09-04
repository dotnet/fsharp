// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.PipingTests

open NUnit.Framework
open FsUnit
open TestHelper

// the current behavior results in a compile error since the |> is merged to the last line 
[<Test>]
let ``should keep the pipe after infix operator``() =
    formatSourceString false """
let f x =
    someveryveryveryverylongexpression
    <|> if someveryveryveryverylongexpression then someveryveryveryverylongexpression else someveryveryveryverylongexpression
    <|> if someveryveryveryverylongexpression then someveryveryveryverylongexpression else someveryveryveryverylongexpression
    |> f
    """ config
    |> prepend newline
    |> should equal """
let f x =
    someveryveryveryverylongexpression 
    <|> if someveryveryveryverylongexpression then 
            someveryveryveryverylongexpression
        else someveryveryveryverylongexpression 
    <|> if someveryveryveryverylongexpression then 
            someveryveryveryverylongexpression
        else someveryveryveryverylongexpression
    |> f
"""

// the current behavior results in a compile error since the |> is merged to the last line 
[<Test>]
let ``should keep the pipe after pattern matching``() =
    formatSourceString false """let m = 
    match x with
    | y -> ErrorMessage msg
    | _ -> LogMessage(msg, true) 
    |> console.Write
    """ config
    |> prepend newline
    |> should equal """
let m =
    match x with
    | y -> ErrorMessage msg
    | _ -> LogMessage(msg, true)
    |> console.Write
"""

[<Test>]
let ``should break new lines on piping``() =
    formatSourceString false """
let runAll() =
    urlList
    |> Seq.map fetchAsync |> Async.Parallel 
    |> Async.RunSynchronously |> ignore""" config
    |> prepend newline
    |> should equal """
let runAll() =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
"""