module TypeChecks.FileOrderAutoTests

open FSharp.Test
open FSharp.Test.Compiler
open Xunit

let private fs name source =
    SourceCodeFileKind.Fs({ FileName = name; SourceText = Some source })

let private fsi name source =
    SourceCodeFileKind.Fsi({ FileName = name; SourceText = Some source })

let private compileMulti (files: SourceCodeFileKind list) =
    match files with
    | [] -> failwith "compileMulti: no files"
    | first :: rest ->
        fsFromString first
        |> FS
        |> asLibrary
        |> withAdditionalSourceFiles rest

[<Fact>]
let ``misordered files succeed under --file-order-auto+`` () =
    // A.fs is listed first but uses B.fs's binding. Without the flag this
    // is the canonical "you put files in the wrong order" failure.
    [ fs "A.fs" """module A
let useB x = B.b x
"""
      fs "B.fs" """module B
let b x = x + 1
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``mutual recursion across files succeeds via cycle group synthesis`` () =
    // Tree references Forest, Forest references Tree. Without auto-order this
    // requires `and` or `namespace rec` — neither possible across files.
    [ fs "Tree.fs" """module Tree
type Tree =
    | Leaf
    | Branch of Forest.Forest
"""
      fs "Forest.fs" """module Forest
type Forest = Tree.Tree list
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``signature file listed after impl is reordered correctly`` () =
    // The classic `.fsi`/`.fs` ordering pain: under auto-mode the pair is
    // collapsed and placed in dependency order regardless of input order.
    [ fs "B.fs" """module B
let b = A.a 42
"""
      fs "A.fs" """module A
let a x = x + 1
"""
      fsi "A.fsi" """module A
val a: int -> int
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``record disambiguation works regardless of file order`` () =
    // Two records with overlapping field names; usage site is listed before
    // the type definitions.
    [ fs "Usage.fs" """module Usage
open Types
let makePerson () : Person = { Name = "Ada"; Age = 36 }
let makeCity () : City = { Name = "London"; Population = 9_000_000 }
"""
      fs "Types.fs" """module Types
type Person = { Name: string; Age: int }
type City = { Name: string; Population: int }
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``SRTP inference resolves cross-file with operator declarations`` () =
    // Operations.fs uses (+) and Zero on Vector2D, defined in Types.fs.
    // Auto-order ensures Types is checked before Operations regardless
    // of source ordering.
    [ fs "Operations.fs" """module SrtpTest.Operations
open SrtpTest.Types
let inline sum (items: ^a list) : ^a =
    items |> List.fold (fun acc x -> acc + x) LanguagePrimitives.GenericZero
let inline dot (a: Vector2D) (b: Vector2D) =
    a.X * b.X + a.Y * b.Y
"""
      fs "Types.fs" """module SrtpTest.Types
type Vector2D = { X: float; Y: float }
with
    static member (+) (a: Vector2D, b: Vector2D) = { X = a.X + b.X; Y = a.Y + b.Y }
    static member Zero = { X = 0.0; Y = 0.0 }
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``union case disambiguation works with usage before types`` () =
    [ fs "Operations.fs" """module UnionTest.Operations
open UnionTest.Types
let area (s: Shape) =
    match s with
    | Circle r -> System.Math.PI * r * r
    | Rectangle(w, h) -> w * h
let describe (cmd: Command) =
    match cmd with
    | Start -> "starting"
    | Stop -> "stopping"
    | Reset -> "resetting"
"""
      fs "Types.fs" """module UnionTest.Types
type Shape =
    | Circle of radius: float
    | Rectangle of width: float * height: float
type Command = Start | Stop | Reset
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``operator overloads resolve across files in any order`` () =
    [ fs "Logic.fs" """module OperatorTest.Logic
open OperatorTest.Types
let totalPrice (items: Money list) = items |> List.reduce (+)
let applyDiscount (rate: decimal) (price: Money) = (1.0m - rate) * price
"""
      fs "Types.fs" """module OperatorTest.Types
type Money = { Amount: decimal; Currency: string }
with
    static member (+) (a: Money, b: Money) =
        { Amount = a.Amount + b.Amount; Currency = a.Currency }
    static member (*) (scalar: decimal, m: Money) =
        { Amount = scalar * m.Amount; Currency = m.Currency }
""" ]
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> shouldSucceed
    |> ignore

[<Fact>]
let ``manual mode preserves the existing 'wrong order' failure`` () =
    // Sanity check: without the flag, the misordered case still fails as
    // upstream F# does. Confirms we haven't changed default behaviour.
    [ fs "A.fs" """module A
let useB x = B.b x
"""
      fs "B.fs" """module B
let b x = x + 1
""" ]
    |> compileMulti
    |> compile
    |> shouldFail
    |> ignore

// Diagnostic-parity: the same error fires under manual and auto modes
// for the same broken source. This guards against auto-mode silently
// suppressing or recasting upstream's diagnostics.

let private assertSameFailureCode (code: int) (files: SourceCodeFileKind list) =
    files |> compileMulti |> compile |> shouldFail |> withErrorCode code |> ignore
    files |> compileMulti |> withFileOrderAuto |> compile |> shouldFail |> withErrorCode code |> ignore

[<Fact>]
let ``error parity: undefined name (FS0039) reports identically in both modes`` () =
    [ fs "Program.fs" """module Program
let x = nonexistentValue 42
""" ]
    |> assertSameFailureCode 39

[<Fact>]
let ``error parity: type mismatch (FS0001) reports identically in both modes`` () =
    [ fs "Program.fs" """module Program
let x : int = "not an int"
""" ]
    |> assertSameFailureCode 1

[<Fact>]
let ``error parity: wrong arity (FS0003) reports identically in both modes`` () =
    [ fs "Lib.fs" """module Lib
let add (x: int) (y: int) : int = x + y
"""
      fs "Program.fs" """module Program
open Lib
let r = add 1 2 3
""" ]
    |> assertSameFailureCode 3

[<Fact>]
let ``and-keyword deprecation FS3887 fires under auto-mode`` () =
    fs "AndUsage.fs" """module AndUsage
type Tree =
    | Leaf
    | Branch of Forest
and Forest = Tree list
"""
    |> List.singleton
    |> compileMulti
    |> withFileOrderAuto
    |> compile
    |> withWarningCode 3887
    |> ignore

[<Fact>]
let ``and-keyword is silent in manual mode`` () =
    fs "AndUsage.fs" """module AndUsage
type Tree =
    | Leaf
    | Branch of Forest
and Forest = Tree list
"""
    |> List.singleton
    |> compileMulti
    |> compile
    |> shouldSucceed
    |> withDiagnostics []
    |> ignore
