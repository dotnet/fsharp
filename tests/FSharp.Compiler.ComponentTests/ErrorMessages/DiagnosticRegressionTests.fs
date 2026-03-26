module ErrorMessages.DiagnosticRegressionTests

open Xunit
open FSharp.Test.Compiler

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
