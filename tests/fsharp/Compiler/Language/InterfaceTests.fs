// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices
open NUnit.Framework
open FSharp.TestHelpers

[<TestFixture>]
module InterfaceTests =

    [<Test>]
    let ShouldWork() =
        CompilerAssert.Pass
            """
type IGet<'T> =
    abstract member Get : unit -> 'T
    
type GetTuple() =
    interface IGet<int * int> with
        member x.Get() = 1, 2
    
type GetFunction() =
    interface IGet<unit->int> with
        member x.Get() = fun () -> 1

type GetAnonymousRecord() =
    interface IGet<{| X : int |}> with
        member x.Get() = {| X = 1 |}

type GetNativePtr() =
    interface IGet<nativeptr<int>> with
        member x.Get() = failwith "not implemented"

type TUnion = A | B of int

type GetUnion() =
    interface IGet<TUnion> with
        member x.Get() = B 2

exit 0
            """