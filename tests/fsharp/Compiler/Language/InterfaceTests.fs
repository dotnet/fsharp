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


    let multiTypedInterfaceSource = """
open System
type AnEnum =
    | One
    | Two

type AClass =
    val value: int
    
    new(value) = { value = value }

type IInterface<'a> =
    abstract GetIt: 'a -> 'a

type implementation () =
    interface IInterface<bool>       with member _.GetIt(x) = x
    interface IInterface<byte>       with member _.GetIt(x) = x
    interface IInterface<byte[]>     with member _.GetIt(x) = x
    interface IInterface<sbyte>      with member _.GetIt(x) = x
    interface IInterface<int16>      with member _.GetIt(x) = x
    interface IInterface<uint16>     with member _.GetIt(x) = x
    interface IInterface<int>        with member _.GetIt(x) = x
    interface IInterface<uint32>     with member _.GetIt(x) = x
    interface IInterface<int64>      with member _.GetIt(x) = x
    interface IInterface<uint64>     with member _.GetIt(x) = x
    interface IInterface<nativeint>  with member _.GetIt(x) = x
    interface IInterface<unativeint> with member _.GetIt(x) = x
    interface IInterface<char>       with member _.GetIt(x) = x
    interface IInterface<string>     with member _.GetIt(x) = x
    interface IInterface<single>     with member _.GetIt(x) = x
    interface IInterface<double>     with member _.GetIt(x) = x
    interface IInterface<decimal>    with member _.GetIt(x) = x
    interface IInterface<bigint>     with member _.GetIt(x) = x
    interface IInterface<AnEnum>     with member _.GetIt(x) = x

let x = implementation ()
let assertion v assertIt =
    if not (assertIt(v)) then
        raise (new Exception (sprintf "Failed to retrieve %A from implementation" v))

// Ensure we can invoke the method and get the value back for each native F# type

assertion true (fun v -> (x :> IInterface<bool>).GetIt(v) = v)
assertion 1uy  (fun v -> (x :> IInterface<byte>).GetIt(v) = v)
assertion 2y   (fun v -> (x :> IInterface<sbyte>).GetIt(v) = v)
assertion 3s   (fun v -> (x :> IInterface<int16>).GetIt(v) = v)
assertion 4us  (fun v -> (x :> IInterface<uint16>).GetIt(v) = v)
assertion 5l   (fun v -> (x :> IInterface<int>).GetIt(v) = v)
assertion 6ul  (fun v -> (x :> IInterface<uint32>).GetIt(v) = v)
assertion 7n   (fun v -> (x :> IInterface<nativeint>).GetIt(v) = v)
assertion 8un (fun v -> (x :> IInterface<unativeint>).GetIt(v) = v)
assertion 9L   (fun v -> (x :> IInterface<int64>).GetIt(v) = v)
assertion 10UL  (fun v -> (x :> IInterface<uint64>).GetIt(v) = v)
assertion 12.12  (fun v -> (x :> IInterface<double>).GetIt(v) = v)
assertion 13I  (fun v -> (x :> IInterface<bigint>).GetIt(v) = v)
assertion 14M  (fun v -> (x :> IInterface<decimal>).GetIt(v) = v)
assertion 'A'  (fun v -> (x :> IInterface<char>).GetIt(v) = v)
assertion 'a'B (fun v -> (x :> IInterface<byte>).GetIt(v) = v)
assertion "16"B  (fun v -> (x :> IInterface<byte[]>).GetIt(v) = v)
assertion AnEnum.Two (fun v -> (x :> IInterface<AnEnum>).GetIt(v) = v)
"""

    [<Test>]
    let MultipleTypedInterfacesFSharp50() =
        CompilerAssert.PassWithOptions
            [| "--langversion:preview" |]
            multiTypedInterfaceSource

    [<Test>]
    let MultipleTypedInterfacesFSharp47() =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.7" |]
            multiTypedInterfaceSource
            [|
                (FSharpErrorSeverity.Error, 443, (15,6,15,20), "This type implements the same interface at different generic instantiations 'IInterface<AnEnum>' and 'IInterface<bigint>'. This is not permitted in this version of F#.")
            |]

