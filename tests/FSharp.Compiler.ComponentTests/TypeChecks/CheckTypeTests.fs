module TypeChecks.CheckTypeTests

open ErrorMessages.ExtendedDiagnosticData
open FSharp.Compiler.Diagnostics.ExtendedData
open FSharp.Test.Assert
open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Union case with function type`` () =
    FSharp """
namespace Foo

type Bar = | Bar of int -> int
type Other = int
"""
    |> typecheck
    |> withSingleDiagnostic (Error 3580, Line 4, Col 21, Line 4, Col 31, "Unexpected function type in union case field definition. If you intend the field to be a function, consider wrapping the function signature with parens, e.g. | Case of a -> b into | Case of (a -> b).")


[<Fact>]
let ``Class - Inherit 01`` () =
    FSharp """
type A = class end

type B() =
    inherit A()
"""
    |> typecheck
    |> withSingleDiagnostic (Error 1133, Line 5, Col 13, Line 5, Col 16, "No constructors are available for the type 'A'")

[<Fact>]
let ``Class - Inherit 02 - Data`` () =
    FSharp """
type A = class end

type B() =
    inherit A()
"""
    |> typecheckResults
    |> checkDiagnosticData
        (1133, "No constructors are available for the type 'A'")
        (fun (typeData: TypeExtendedData) -> typeData.Type.Format(typeData.DisplayContext) |> shouldEqual "A")
