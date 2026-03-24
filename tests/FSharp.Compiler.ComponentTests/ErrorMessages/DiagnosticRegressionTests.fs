module ErrorMessages.DiagnosticRegressionTests

open Xunit
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/15655
[<Fact>]
let ``Issue 15655 - error codes 999 and 3217 are distinct`` () =
    // Verify that notAFunctionButMaybeIndexerErrorCode (3217) is distinct from packageManagerError (999)
    // by triggering a "not a function but maybe indexer" error
    FSharp
        """
let d = System.Collections.Generic.Dictionary<string,int>()
let v = d ["key"]
        """
    |> typecheck
    |> shouldFail
    |> withErrorCode 3217
