// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Diagnostics.Records

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``Warning emitted when record update syntax changes all fields in lang preview``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateOk r = { r with F1 = 1 }
let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion80
    |> withOptions ["--warnon:FS3560"]
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3560, Line 7, Col 20, Line 7, Col 46, "This copy-and-update record expression changes all fields of record type 'Records.R'. Consider using the record construction syntax instead.")
    ]

[<Fact>]
let ``Warning not emitted when record update syntax changes all fields in lang70``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion70
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Warning not emitted when record update syntax changes all fields when disabled manually in lang preview``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Warning emitted when record update syntax changes all fields when enabled manually in lang70``() =
    Fsx """
module Records

type R = { F1: int; F2: string }

let updateWarn r = { r with F1 = 1; F2 = "" }
    """
    |> withLangVersion70
    |> withOptions ["--warnon:FS3560"]
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3560, Line 6, Col 20, Line 6, Col 46, "This copy-and-update record expression changes all fields of record type 'Records.R'. Consider using the record construction syntax instead.")
    ]

[<Fact>]
let ``Warning not emitted for generated record updates within a nested copy-and-update expression in a lang preview``() =
    Fsx """
type AnotherNestedRecTy = { A: int; B: int }

type NestedRecTy = { C: {| c: AnotherNestedRecTy |} }

type RecTy = { D: NestedRecTy; I: int }

//                        vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
let t1 (x: NestedRecTy) = { x with C.c = Unchecked.defaultof<_> }

// Do not report for the nested NestedRecTy update
let t2 (x: RecTy) (a: AnotherNestedRecTy) = { x with D.C.c = { a with A = 3 } }

//                                                           vvvvvvvvvvvvvvvvvvvvvvv
let t3 (x: RecTy) (a: AnotherNestedRecTy) = { x with D.C.c = { a with A = 3; B = 4 } }
    """
    |> withLangVersion80
    |> withOptions ["--warnon:FS3560"]
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3560, Line 9, Col 27, Line 9, Col 66, "This copy-and-update record expression changes all fields of record type 'Test.NestedRecTy'. Consider using the record construction syntax instead.")
        (Warning 3560, Line 15, Col 62, Line 15, Col 85, "This copy-and-update record expression changes all fields of record type 'Test.AnotherNestedRecTy'. Consider using the record construction syntax instead.")
    ]
    
[<Fact>]
let ``Error when implementing interface with auto property in record type``() =
    FSharp """
type Foo =
  abstract member X : string with get, set
  abstract GetValue: unit -> string

type FooImpl = 
  { name: string }
  interface Foo with
    member val X = "" with get, set
    member x.GetValue() = x.name
    """
    |> withLangVersion80
    |> asExe
    |> compile
    |> shouldFail
    |> withSingleDiagnostic (Error 912, Line 9, Col 5, Line 9, Col 36, "This declaration element is not permitted in an augmentation")
    
[<Fact>]
let ``Error when declaring an abstract member in record type`` () =
    Fsx """
type R =
  { a : int; b : string }
  abstract M : unit -> unit
   """
    |> typecheck 
    |> shouldFail
    |>  withSingleDiagnostic (Error 912, Line 4, Col 3, Line 4, Col 28, "This declaration element is not permitted in an augmentation")
