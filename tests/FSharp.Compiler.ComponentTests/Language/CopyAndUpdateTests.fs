module FSharp.Compiler.ComponentTests.Language.CopyAndUpdateTests

open Xunit
open FSharp.Test.Compiler
open StructuredResultsAsserts

[<Fact>]
let ``Cannot update the same field twice in nested copy-and-update``() =
    FSharp """
type NestdRecTy = { B: string }

type RecTy = { D: NestdRecTy; E: string option }

let t2 x = { x with D.B = "a"; D.B = "b" }
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 668, Line 6, Col 21, Line 6, Col 22, "The field 'B' appears twice in this record expression or pattern")
    ]

[<Fact>]
let ``Cannot use nested copy-and-update in lang version70``() =
    FSharp """
type NestdRecTy = { B: string }

type RecTy = { D: NestdRecTy; E: string option }

let t2 x = { x with D.B = "a" }
    """
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 6, Col 21, Line 6, Col 24, "Feature 'Nested record field copy-and-update' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
    ]

[<Fact>]
let ``Nested copy-and-update merges same level updates``() =
    FSharp """
module CopyAndUpdateTests

type AnotherNestedRecTy = { A: int }

type NestdRecTy = { B: AnotherNestedRecTy; C: string }

type RecTy = { D: NestdRecTy; E: string option }

let t2 x = { x with D.B.A = 1; D.C = "ads" }
    """
    |> withLangVersionPreview
    |> withNoDebug
    |> withOptimize
    |> compile
    |> shouldSucceed
    |> verifyIL [
(*
        public static CopyAndUpdateTests.RecTy t2(CopyAndUpdateTests.RecTy x)
        {
            return new CopyAndUpdateTests.RecTy(new CopyAndUpdateTests.NestdRecTy(new CopyAndUpdateTests.AnotherNestedRecTy(1), "ads"), x.E@);
        }
*)
        """
.method public static class CopyAndUpdateTests/RecTy 
        t2(class CopyAndUpdateTests/RecTy x) cil managed
{
    
  .maxstack  8
  IL_0000:  ldc.i4.1
  IL_0001:  newobj     instance void CopyAndUpdateTests/AnotherNestedRecTy::.ctor(int32)
  IL_0006:  ldstr      "ads"
  IL_000b:  newobj     instance void CopyAndUpdateTests/NestdRecTy::.ctor(class CopyAndUpdateTests/AnotherNestedRecTy,
                                                                          string)
  IL_0010:  ldarg.0
  IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> CopyAndUpdateTests/RecTy::E@
  IL_0016:  newobj     instance void CopyAndUpdateTests/RecTy::.ctor(class CopyAndUpdateTests/NestdRecTy,
                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>)
  IL_001b:  ret
} 
        """
    ]

[<Fact>]
let ``Nested copy-and-update correctly updates fields in nominal record``() =
    FSharp """
module CopyAndUpdateTests

type AnotherNestedRecTy = { A: int }

type NestdRecTy = { B: string; C: AnotherNestedRecTy }

type RecTy = { D: NestdRecTy; E: string option; F: int }

let t1 = { D = { B = "t1"; C = { A = 1 } }; E = None; F = 42 }

let actual1 = { t1 with D.B = "t2" }
let expected1 = { D = { B = "t2"; C = { A = 1 } }; E = None; F = 42 }

let actual2 = { t1 with D.C.A = 3; E = Some "a" }
let expected2 = { D = { B = "t1"; C = { A = 3 } }; E = Some "a"; F = 42 }

if actual1 <> expected1 then
    failwith "actual1 does not equal expected1"

if actual2 <> expected2 then
    failwith "actual2 does not equal expected2"
    """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update correctly updates fields in anonymous record``() =
    FSharp """
module CopyAndUpdateTests

let t1 = {| D = {| B = "t1"; C = struct {| A = 1 |} |}; E = Option<string>.None |}

let actual1 = {| t1 with D.B = "t2" |}
let expected1 = {| D = {| B = "t2"; C = struct {| A = 1 |} |}; E = None |}

let actual2 = {| t1 with D.C.A = 3; E = Some "a" |}
let expected2 = {| D = {| B = "t1"; C = struct {| A = 3 |} |}; E = Some "a" |}

if actual1 <> expected1 then
    failwith "actual1 does not equal expected1"

if actual2 <> expected2 then
    failwith "actual2 does not equal expected2"
    """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Qualified record field names are correctly recognized in nested copy-and-update``() =
    FSharp """
module CopyAndUpdateTests

module U =
    module U =
        type G = { U: {| a: G |}; I: int }

let moduleModulePrefix x = { x with U.U.U.a.U.a.U.a.I = 1 }

let moduleModuleTypePrefix x = { x with U.U.G.U.a.I = 1 }

open U

let modulePrefix x = { x with U.U.a.I = 1 }

let moduleTypePrefix x = { x with U.G.U.a.I = 1 }

open U

let typePrefix x = { x with G.U.a.I = 1 }

let modulePrefix2 x = { x with U.U.a.I = 1 }

let moduleTypePrefix2 x = { x with U.G.U.a.I = 1 }

let noPrefix x = { x with U.a.I = 1 }

let c3 = { U.G.U = Unchecked.defaultof<_>; I = 3 }

let c4 = { U.U = Unchecked.defaultof<_>; I = 3 }
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update works correctly on recursive records``() =
    FSharp """
module CopyAndUpdateTests

type G = { T: string; U: {| a: G |}; I: int }

let f x = { x with U.a.U.a.I = 0; I = -1 }

let start = { T = "a"; I = 1; U = {| a = { T = "a"; I = 2; U = {| a = { T = "a"; I = 3; U = Unchecked.defaultof<_> } |} } |} }

let actual = f start
let expected = { T = "a"; I = -1; U = {| a = { T = "a"; I = 2; U = {| a = { T = "a"; I = 0; U = Unchecked.defaultof<_> } |} } |} }

if actual <> expected then
    failwith "actual does not equal expected"
    """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Anonymous record with nested copy-and-update can change shape``() =
    FSharp """
module CopyAndUpdateTests

type RecTy = { D: int; E: string option }

let start = {| R = { D = 2; E = Some "e" }; S = 3 |}

let actual = {| start with R.E = None; S = "May I be a string now?"; T = 4 |}

let expected = {| R = { D = 2; E = None }; S = "May I be a string now?"; T = 4 |}

if actual <> expected then
    failwith "actual does not equal expected"
    """
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Anonymous record in a nominal record with nested copy-and-update cannot change shape``() =
    FSharp """
module CopyAndUpdateTests

type RecTy = { D: int; E: {| A: int |} }

let f x = { x with E.A = "May I be a string now?" }
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withResult {
        Error = Error 1
        Range = { StartLine = 6
                  StartColumn = 26
                  EndLine = 6
                  EndColumn = 50 }
        Message = "This expression was expected to have type
    'int'    
but here has type
    'string'    "
    }

[<Fact>]
let ``Nested copy-and-update does not compile when referencing invalid fields``() =
    FSharp """
module CopyAndUpdateTests

type NestdRecTy = { B: string; G: {| a: int |} }

type RecTy = { D: NestdRecTy; E: string option }

let t1 x = { x with D.B.A = "a" }
let t2 x = { x with D.C = "a" }
let t3 x = { x with D.G.b = "a" }
let t4 x = { x with C.D = "a" }
let t5 (x: {| a: int; b: NestdRecTy |}) = {| x with b.C = "a" |}
let t6 (x: {| a: int; b: NestdRecTy |}) = {| x with b.G.b = "a" |}
let t7 (x: {| a: int; b: NestdRecTy |}) = {| x with c.D = "a" |}
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 39, Line 8, Col 25, Line 8, Col 26, "The record label 'A' is not defined.")
        (Error 1129, Line 9, Col 23, Line 9, Col 24, "The record type 'NestdRecTy' does not contain a label 'C'.")
        (Error 1129, Line 10, Col 25, Line 10, Col 26, "The record type '{| a: int |}' does not contain a label 'b'.")
        (Error 39, Line 11, Col 21, Line 11, Col 22, "The namespace or module 'C' is not defined.")
        (Error 1129, Line 12, Col 55, Line 12, Col 56, "The record type 'NestdRecTy' does not contain a label 'C'.")
        (Error 1129, Line 13, Col 57, Line 13, Col 58, "The record type '{| a: int |}' does not contain a label 'b'.")
        (Error 1129, Line 14, Col 53, Line 14, Col 54, "The record type '{| a: int; b: NestdRecTy |}' does not contain a label 'c'.")
    ]