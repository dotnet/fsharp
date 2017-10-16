// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.CodeFormatterExtTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``async workflows``() =
    formatSourceString false """
let fetchAsync(name, url:string) =
    async { 
        try 
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)
            printfn "Read %d characters for %s" html.Length name
        with
            | :? Exception -> ()
            | ex -> printfn "%s" (ex.Message);
    }
    """ config
    |> prepend newline
    |> should equal """
let fetchAsync (name, url : string) =
    async { 
        try 
            let uri = new System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri)
            printfn "Read %d characters for %s" html.Length name
        with
        | :? Exception -> ()
        | ex -> printfn "%s" (ex.Message)
    }
"""

[<Test>]
let ``computation expressions``() =
    formatSourceString false """
let comp =
    eventually { for x in 1 .. 2 do
                    printfn " x = %d" x
                 return 3 + 4 }""" config
    |> prepend newline
    |> should equal """
let comp =
    eventually { 
        for x in 1..2 do
            printfn " x = %d" x
        return 3 + 4
    }
"""

[<Test>]
let ``sequence expressions``() =
    formatSourceString false """
let s1 = seq { for i in 1 .. 10 -> i * i }
let s2 = seq { 0 .. 10 .. 100 }
let rec inorder tree =
    seq {
      match tree with
          | Tree(x, left, right) ->
               yield! inorder left
               yield x
               yield! inorder right
          | Leaf x -> yield x
    }   
    """ config
    |> prepend newline
    |> should equal """
let s1 =
    seq { 
        for i in 1..10 -> i * i
    }

let s2 = seq { 0..10..100 }

let rec inorder tree =
    seq { 
        match tree with
        | Tree(x, left, right) -> 
            yield! inorder left
            yield x
            yield! inorder right
        | Leaf x -> yield x
    }
"""

[<Test>]
let ``range expressions``() =
    formatSourceString false """
let factors number = 
    {2L .. number / 2L}
    |> Seq.filter (fun x -> number % x = 0L)""" config
    |> prepend newline
    |> should equal """
let factors number =
    { 2L..number / 2L } |> Seq.filter (fun x -> number % x = 0L)
"""
