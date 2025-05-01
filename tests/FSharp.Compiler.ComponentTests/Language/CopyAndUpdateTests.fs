module Language.CopyAndUpdateTests

open Xunit
open FSharp.Test.Compiler
open StructuredResultsAsserts

[<Fact>]
let ``Cannot update the same field twice in nested copy-and-update``() =
    FSharp """
type NestedRecTy = { B: string }

type RecTy = { D: NestedRecTy; E: string option }

let t2 x = { x with D.B = "a"; D.B = "b" }
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 668, Line 6, Col 23, Line 6, Col 24, "The field 'B' appears multiple times in this record expression or pattern")
    ]
    
[<Fact>]
let ``Cannot update the same field appears multiple times in nested copy-and-update``() =
    FSharp """
type NestedRecTy = { B: string }

type RecTy = { D: NestedRecTy; E: string option }

let t2 x = { x with D.B = "a"; D.B = "b"; D.B = "c" }
    """
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 668, Line 6, Col 23, Line 6, Col 24, "The field 'B' appears multiple times in this record expression or pattern")
        (Error 668, Line 6, Col 34, Line 6, Col 35, "The field 'B' appears multiple times in this record expression or pattern")
    ]
    
[<Fact>]
let ``Cannot update the same field appears multiple times in nested copy-and-update 2``() =
    FSharp """
type NestedRecTy = { B: string; C: string }

type RecTy = { D: NestedRecTy; E: string option }

let t2 x = { x with D.B = "a"; D.C = ""; D.B = "c" ; D.C = "d" }
    """
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 668, Line 6, Col 34, Line 6, Col 35, "The field 'C' appears multiple times in this record expression or pattern")
        (Error 668, Line 6, Col 23, Line 6, Col 24, "The field 'B' appears multiple times in this record expression or pattern")
    ]

[<Fact>]
let ``Cannot use nested copy-and-update in lang version70``() =
    FSharp """
type NestedRecTy = { B: string }

type RecTy = { D: NestedRecTy; E: string option }

let t2 x = { x with D.B = "a" }
    """
    |> withLangVersion70
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 6, Col 21, Line 6, Col 24, "Feature 'Nested record field copy-and-update' is not available in F# 7.0. Please use language version 8.0 or greater.")
    ]

[<Fact>]
let ``Nested copy-and-update merges same level updates``() =
    FSharp """
module CopyAndUpdateTests

type AnotherNestedRecTy = { A: int }

type NestedRecTy = { B: AnotherNestedRecTy; C: string }

type RecTy = { D: NestedRecTy; E: string option }

let t2 x = { x with D.B.A = 1; D.C = "ads" }
    """
    |> withLangVersion80
    |> withNoDebug
    |> withOptimize
    |> compile
    |> shouldSucceed
    |> verifyIL [
(*
        public static CopyAndUpdateTests.RecTy t2(CopyAndUpdateTests.RecTy x)
        {
            return new CopyAndUpdateTests.RecTy(new CopyAndUpdateTests.NestedRecTy(new CopyAndUpdateTests.AnotherNestedRecTy(1), "ads"), x.E@);
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
  IL_000b:  newobj     instance void CopyAndUpdateTests/NestedRecTy::.ctor(class CopyAndUpdateTests/AnotherNestedRecTy,
                                                                          string)
  IL_0010:  ldarg.0
  IL_0011:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> CopyAndUpdateTests/RecTy::E@
  IL_0016:  newobj     instance void CopyAndUpdateTests/RecTy::.ctor(class CopyAndUpdateTests/NestedRecTy,
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

type NestedRecTy = { B: string; C: AnotherNestedRecTy }

type RecTy = { D: NestedRecTy; E: string option; F: int }

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
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update correctly updates fields in nominal record with dotted field``() =
    FSharp """
module CopyAndUpdateTests

type A = { B: string }

type Foo = { ``A.B``: string; A: A; C: int }

let t1 = { ``A.B`` = "fooAB"; A = { B = "fooB" }; C = 42 }

let actual = { t1 with Foo.``A.B`` = "barAB"; Foo.A.B = "barB" }
let expected = { ``A.B`` = "barAB"; A = { B = "barB" }; C = 42 }

if actual <> expected then
    failwith "actual does not equal expected"
    """
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update correctly updates fields in nominal generic record``() =
    FSharp """
module CopyAndUpdateTests

type AnotherNestedRecTy = { A: int }

type NestedRecTy<'b> = { B: 'b; C: AnotherNestedRecTy }

type RecTy<'b, 'e> = { D: NestedRecTy<'b>; E: 'e option; F: int }

let t1 = { D = { B = "t1"; C = { A = 1 } }; E = Option<string>.None; F = 42 }

let actual1 = { t1 with D.B = "t2" }
let expected1 = { D = { B = "t2"; C = { A = 1 } }; E = None; F = 42 }

let actual2 = { t1 with D.C.A = 3; E = Some "a" }
let expected2 = { D = { B = "t1"; C = { A = 3 } }; E = Some "a"; F = 42 }

if actual1 <> expected1 then
    failwith "actual1 does not equal expected1"

if actual2 <> expected2 then
    failwith "actual2 does not equal expected2"
    """
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update correctly updates fields in nominal struct record``() =
    FSharp """
module CopyAndUpdateTests

[<Struct>]
type AnotherNestedRecTy = { A: int }

type NestedRecTy = { B: string; C: AnotherNestedRecTy; G: int; H: int }

[<Struct>]
type RecTy = { D: NestedRecTy; E: string option; F: int }

let t1 = { D = { B = "t1"; C = { A = 1 }; G = 0; H = 0 }; E = None; F = 42 }

let actual1 = { t1 with D.B = "t2"; D.G = 3; D.H = 1 }
let expected1 = { D = { B = "t2"; C = { A = 1 }; G = 3; H = 1 }; E = None; F = 42 }

let actual2 = { t1 with D.C.A = 3; E = Some "a"; D.G = 2; D.H = 3 }
let expected2 = { D = { B = "t1"; C = { A = 3 }; G = 2; H = 3 }; E = Some "a"; F = 42 }

if actual1 <> expected1 then
    failwith "actual1 does not equal expected1"

if actual2 <> expected2 then
    failwith "actual2 does not equal expected2"
    """
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update correctly updates fields in anonymous record``() =
    FSharp """
module CopyAndUpdateTests

let t1 = {| D = {| B = "t1"; C = struct {| A = 1 |}; G = 0 |}; E = Option<string>.None |}

let actual1 = {| t1 with D.B = "t2"; D.G = 3 |}
let expected1 = {| D = {| B = "t2"; C = struct {| A = 1 |}; G = 3 |}; E = None |}

let actual2 = {| t1 with D.C.A = 3; E = Some "a"; D.G = 2 |}
let expected2 = {| D = {| B = "t1"; C = struct {| A = 3 |}; G = 2 |}; E = Some "a" |}

if actual1 <> expected1 then
    failwith "actual1 does not equal expected1"

if actual2 <> expected2 then
    failwith "actual2 does not equal expected2"
    """
    |> withLangVersion80
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
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update works correctly on recursive records``() =
    FSharp """
module CopyAndUpdateTests

type G<'t> = { T: 't; U: {| a: G<'t> |}; I: int }

let f x = { x with U.a.U.a.I = 0; I = -1 }

let start = { T = "a"; I = 1; U = {| a = { T = "a"; I = 2; U = {| a = { T = "a"; I = 3; U = Unchecked.defaultof<_> } |} } |} }

let actual = f start
let expected = { T = "a"; I = -1; U = {| a = { T = "a"; I = 2; U = {| a = { T = "a"; I = 0; U = Unchecked.defaultof<_> } |} } |} }

if actual <> expected then
    failwith "actual does not equal expected"
    """
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update does not compile when assigning values of the wrong type``() =
    FSharp """
module CopyAndUpdateTests

type AnotherNestedRecTy = { A: int }

type NestedRecTy<'b> = { B: 'b; C: AnotherNestedRecTy }

type RecTy<'b, 'e> = { D: NestedRecTy<'b>; E: 'e option; F: int }

let t1 = { D = { B = "t1"; C = { A = 1 } }; E = Option<string>.None; F = 42 }

let actual1 = { t1 with D.B = 1 }

let actual2 = { t1 with D.C.A = 3; E = Some 1.0 }
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withResults [
        {
            Error = Error 1
            Range = { StartLine = 12
                      StartColumn = 31
                      EndLine = 12
                      EndColumn = 32 }
            Message = @"This expression was expected to have type
    'string'    
but here has type
    'int'    "
        }
        {
            Error = Error 1
            Range = { StartLine = 14
                      StartColumn = 45
                      EndLine = 14
                      EndColumn = 48 }
            Message = @"This expression was expected to have type
    'string'    
but here has type
    'float'    "
        }
    ]

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
    |> withLangVersion80
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``Anonymous record in a nominal record with nested copy-and-update cannot change shape``() =
    FSharp """
module CopyAndUpdateTests

type RecTy = { D: int; E: {| A: int |} }

let f x = { x with E.A = "May I be a string now?" }
    """
    |> withLangVersion80
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

type NestedRecTy = { B: string; G: {| a: int |} }

type RecTy = { D: NestedRecTy; E: string option }

let t1 x = { x with D.B.A = "a" }
let t2 x = { x with D.C = "a" }
let t3 x = { x with D.G.b = "a" }
let t4 x = { x with C.D = "a" }
let t5 (x: {| a: int; b: NestedRecTy |}) = {| x with b.C = "a" |}
let t6 (x: {| a: int; b: NestedRecTy |}) = {| x with b.G.b = "a" |}
let t7 (x: {| a: int; b: NestedRecTy |}) = {| x with c.D = "a" |}
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 39, Line 8, Col 25, Line 8, Col 26, "The record label 'A' is not defined.")
        (Error 1129, Line 9, Col 23, Line 9, Col 24, "The record type 'NestedRecTy' does not contain a label 'C'.")
        (Error 1129, Line 10, Col 25, Line 10, Col 26, "The record type '{| a: int |}' does not contain a label 'b'.")
        (Error 39, Line 11, Col 21, Line 11, Col 22, "The namespace or module 'C' is not defined.")
        (Error 1129, Line 12, Col 56, Line 12, Col 57, "The record type 'NestedRecTy' does not contain a label 'C'.")
        (Error 1129, Line 13, Col 58, Line 13, Col 59, "The record type '{| a: int |}' does not contain a label 'b'.")
        (Error 1129, Line 14, Col 54, Line 14, Col 55, "The record type '{| a: int; b: NestedRecTy |}' does not contain a label 'c'.")
    ]

[<Fact>]
let ``Nested copy-and-update works when the starting expression is not a simple identifier``() =
    FSharp """
module CopyAndUpdateTests

type Record1 = { Foo: int; Bar: int; }

[<AutoOpen>]
module Module =
    type Record2 = { Foo: Record1; G: string }
    let item: Record2 = Unchecked.defaultof<Record2>

ignore { Module.item with Foo.Foo = 3 }
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Nested, anonymous copy-and-update works when the starting expression is not a simple identifier``() =
    FSharp """
module CopyAndUpdateTests

type Record1 = { Foo: int; Bar: int; }

[<AutoOpen>]
module Module =
    let item = {| Foo = Unchecked.defaultof<Record1> |}

ignore {| Module.item with Foo.Foo = 3 |}
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Nested copy-and-update evaluates the original expression once``() =
    FSharp """
module CopyAndUpdateTests

type Record1 = { Foo: int; Bar: int; Baz: string }
type Record2 = { Foo: Record1; A: int; B: int }

let f () =
    printf "once"
    { A = 1; B = 2; Foo = { Foo = 99; Bar = 98; Baz = "a" } }

let actual = { f () with Foo.Foo = 3; Foo.Baz = "b"; A = -1 }

let expected = { A = -1; B = 2; Foo = { Foo = 3; Bar = 98; Baz = "b" } }

if actual <> expected then
    failwith "actual does not equal expected"
    """
    |> withLangVersion80
    |> compileExeAndRun
    |> verifyOutput "once"
