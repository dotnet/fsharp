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


// https://github.com/dotnet/fsharp/issues/16410
[<Fact>]
let ``Issue 16410 - no spurious FS3570 warning with KeyValue active pattern`` () =
    FSharp
        """
open System.Collections.Generic

module Map =
    let update empty k f m =
        match Map.tryFind k m with
        | Some v -> Map.add k (f v) m
        | None -> Map.add k (f empty) m

let invert (dict: IDictionary<'k,#seq<'k>>): Map<'k,Set<'k>> =
    (Map.empty, dict)
    ||> Seq.fold (fun res (KeyValue (k, ks)) ->
            (res, ks)
            ||> Seq.fold (fun res vk -> res |> Map.update Set.empty vk _.Add(k))
        )
        """
    |> typecheck
    |> shouldSucceed
