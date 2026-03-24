module ErrorMessages.DiagnosticRegressionTests

open Xunit
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/13697
[<Fact>]
let ``Issue 13697 - typeof with out of scope type in attribute should report FS0039`` () =
    FSharp
        """
type FooAttribute(t:System.Type) = inherit System.Attribute()
[<Foo(typeof<OutOfScopeType>)>]
type Vehicle() = class end
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 39, Line 3, Col 14, Line 3, Col 28, "The type 'OutOfScopeType' is not defined.")
          (Error 267, Line 3, Col 7, Line 3, Col 29, "This is not a valid constant expression or custom attribute value") ]
