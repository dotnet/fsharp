// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module TaskCETypeInference =

    // https://github.com/dotnet/fsharp/issues/14596
    [<Fact>]
    let ``Issue 14596 - Type inference in backgroundTask CE should not produce FS0073`` () =
        FSharp
            """
module UnresolvedTypeVarBug

type IMarker<'T> = interface end
type SomeAction<'T when 'T :> IMarker<'T>> = SomeAction of list<'T>
type Spec<'Action> = { Dummy: unit }

open System.Threading.Tasks

let dummyTask (_spec: Spec<'Action>) (_action: 'Action) : Task<Result<unit, string>> = failwith "not implemented"

let repro (spec: Spec<SomeAction<'T>>)
    : Task<Result<unit, string>> =

    backgroundTask {
        let action = SomeAction []
        let! res = dummyTask spec action

        match res with
        | Ok _ ->
            return Ok ()
        | Error err ->
            return Error err
    }
            """
        |> asLibrary
        |> typecheck
        |> shouldSucceed
