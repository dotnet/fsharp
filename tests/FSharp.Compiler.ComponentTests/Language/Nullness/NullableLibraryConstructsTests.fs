module Language.NullableLibraryConstructs

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let typeCheckWithStrictNullness cu =
    cu
    |> withCheckNulls
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> typecheck

[<FactForNETCOREAPP>]
let ``Can unsafely forgive null using Unchecked nonNull function`` () = 
    FSharp """module MyLibrary

let readAllLines (reader:System.IO.StreamReader) : seq<string> =
    seq {
        while not reader.EndOfStream do
            reader.ReadLine() |> Unchecked.nonNull
    }
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<FactForNETCOREAPP>]
let ``Can unsafely forgive null using Unchecked NonNullQuick active pattern`` () = 
    FSharp """module MyLibrary

let readAllLines (reader:System.IO.StreamReader) : seq<string> =
    seq {
        while not reader.EndOfStream do
            match reader.ReadLine() with
            | Unchecked.NonNullQuick line -> yield line
    }
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed

[<Fact>]
let ``Can concat two maybe null strings`` () =
    FSharp """module MyLibrary

let addStrings (x:string | null) (y:string) : string  =

    let s2 = x + x
    let s3 = y + y
    let s4 = y + x
    let s4string : string = s4

    s4
"""
    |> asLibrary
    |> typeCheckWithStrictNullness
    |> shouldSucceed