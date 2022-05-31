// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.Diagnostics
open NUnit.Framework
open FSharp.Test
open FSharp.Test.Utilities

[<TestFixture>]
module InterfaceTests =

    [<Test>]
    let ShouldnWork() =
        CompilerAssert.Pass """
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

    let ``C# base with dim`` = """
using System;

namespace CSharpTest
{
    public interface ITestDim
    {
        int GetIt(int x) { return x;  }
    }
}
"""

    let ``Many Instantiations of the same interface - SetUp`` = """
module Program

open System
#if TEST_DIMS
open CSharpTest
#endif

type AnEnum =
    | One
    | Two

[<Struct>]
type AStructRecord = { Forename:string;  Surname:string }

type AClass =
    val value: int

    new(value) = { value = value }

type IInterface<'a> =
    abstract GetIt: 'a -> 'a

type implementation () =
// DIMs only available on netcoreapp 3
#if TEST_DIMS
    interface ITestDim
#endif
    interface IInterface<bool>                      with member _.GetIt(x) = x              // bool
    interface IInterface<byte>                      with member _.GetIt(x) = x              // byte
    interface IInterface<byte[]>                    with member _.GetIt(x) = x              // byte array
    interface IInterface<sbyte>                     with member _.GetIt(x) = x              // sbyte
    interface IInterface<int16>                     with member _.GetIt(x) = x              // int 16
    interface IInterface<uint16>                    with member _.GetIt(x) = x              // uint16
    interface IInterface<int>                       with member _.GetIt(x) = x              // int
    interface IInterface<uint32>                    with member _.GetIt(x) = x              // uint32
    interface IInterface<int64>                     with member _.GetIt(x) = x              // int64
    interface IInterface<uint64>                    with member _.GetIt(x) = x              // uint64
    interface IInterface<nativeint>                 with member _.GetIt(x) = x              // nativeint
    interface IInterface<unativeint>                with member _.GetIt(x) = x              // unativeint
    interface IInterface<char>                      with member _.GetIt(x) = x              // char
    interface IInterface<string>                    with member _.GetIt(x) = x              // string
    interface IInterface<single>                    with member _.GetIt(x) = x              // single
    interface IInterface<double>                    with member _.GetIt(x) = x              // double
    interface IInterface<decimal>                   with member _.GetIt(x) = x              // decimal
    interface IInterface<bigint>                    with member _.GetIt(x) = x              // bigint
    interface IInterface<struct (int * int)>        with member _.GetIt(x) = x              // struct tuple
    interface IInterface<AnEnum>                    with member _.GetIt(x) = x              // enum
    interface IInterface<ValueOption<string>>       with member _.GetIt(x) = x              // struct union
    interface IInterface<AStructRecord>             with member _.GetIt(x) = x              // struct record
    // Anonymous records are non-deterministic So don't include for il comparison
#if !NO_ANONYMOUS
    interface IInterface<struct {|First:int;  Second:int|}>   with member _.GetIt(x) = x    // Anonymous record
#endif
    interface IInterface<int -> int>                with member _.GetIt(x) = x              // func
    interface IInterface<float -> float>            with member _.GetIt(x) = x              // func
"""

    let ``Many Instantiations of the same interface - Asserts`` = """
let x = implementation ()
let assertion v assertIt =
    if not (assertIt(v)) then
        raise (new Exception (sprintf "Failed to retrieve %A from implementation" v))

let assertionRecord v assertIt =
    if not (assertIt(v)) then
        raise (new Exception (sprintf "Failed to retrieve %A from implementation" v))

// Ensure we can invoke the method and get the value back for each native F# type
#if TEST_DIMS
assertion 7 (fun v -> (x :> ITestDim).GetIt(v) = v)
#endif

assertion true (fun v -> (x :> IInterface<bool>).GetIt(v) = v)
assertion 1uy  (fun v -> (x :> IInterface<byte>).GetIt(v) = v)
assertion 2y   (fun v -> (x :> IInterface<sbyte>).GetIt(v) = v)
assertion 3s   (fun v -> (x :> IInterface<int16>).GetIt(v) = v)
assertion 4us  (fun v -> (x :> IInterface<uint16>).GetIt(v) = v)
assertion 5l   (fun v -> (x :> IInterface<int>).GetIt(v) = v)
assertion 6ul  (fun v -> (x :> IInterface<uint32>).GetIt(v) = v)
assertion 7n   (fun v -> (x :> IInterface<nativeint>).GetIt(v) = v)
assertion 8un  (fun v -> (x :> IInterface<unativeint>).GetIt(v) = v)
assertion 9L   (fun v -> (x :> IInterface<int64>).GetIt(v) = v)
assertion 10UL  (fun v -> (x :> IInterface<uint64>).GetIt(v) = v)
assertion 12.12  (fun v -> (x :> IInterface<double>).GetIt(v) = v)
assertion 13I  (fun v -> (x :> IInterface<bigint>).GetIt(v) = v)
assertion 14M  (fun v -> (x :> IInterface<decimal>).GetIt(v) = v)
assertion 'A'  (fun v -> (x :> IInterface<char>).GetIt(v) = v)
assertion 'a'B (fun v -> (x :> IInterface<byte>).GetIt(v) = v)
assertion "16"B  (fun v -> (x :> IInterface<byte[]>).GetIt(v) = v)
assertion AnEnum.Two (fun v -> (x :> IInterface<AnEnum>).GetIt(v) = v)
assertion (ValueSome "7") (fun v -> (x :> IInterface<ValueOption<string>>).GetIt(v) = v)
assertion struct (1,2) (fun v -> (x :> IInterface<struct (int * int)>).GetIt(v) = v)
assertion { Forename="Forename";  Surname="Surname" } (fun v -> (x :> IInterface<AStructRecord>).GetIt(v) = v)
#if !NO_ANONYMOUS        // Anonymous records are non-deterministic So don't include for il comparison
assertion struct {|First=2;  Second=3 |} (fun (v:struct {|First:int;  Second:int|}) -> (x :> IInterface<struct {|First:int;  Second:int|}>).GetIt(v) = v)
#endif
assertion (fun x -> x * 2) (fun v ->
                        let f = (x :> IInterface<int -> int>).GetIt(v)
                        f(7) = 14)
assertion (fun (x:float) -> x * 3.0) (fun v ->
    let f = (x :> IInterface<float -> float>).GetIt(v)
    f(2.0) = 6.0)
"""

    let ``Many Instantiations of the same interface`` =
        ``Many Instantiations of the same interface - SetUp`` + ``Many Instantiations of the same interface - Asserts``


    [<Test>]
    let MultipleTypedInterfacesFSharp50() =

#if NETSTANDARD
        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(``C# base with dim``, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create
#endif

        let fsCmpl =
            Compilation.Create(
                "test.fs",
                ``Many Instantiations of the same interface - SetUp`` + ``Many Instantiations of the same interface - Asserts``,Library,
                options = [|
                    "--langversion:5.0";
#if !NETSTANDARD
                |])
#else
                    "--define:NETSTANDARD";
                    "--define:TEST_DIMS";
                |],
                cmplRefs = [csCmpl])
#endif

        CompilerAssert.Compile fsCmpl

    [<Test>]
    let MultipleTypedInterfacesFSharp47() =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [|
                "--langversion:4.7";
#if NETSTANDARD
                "--define:NETSTANDARD";
#endif
            |]
            ``Many Instantiations of the same interface``
            [|
                (FSharpDiagnosticSeverity.Error, 3350, (24, 6, 24, 20), "Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 5.0 or greater.")
            |]

    [<Test>]
    let MultipleTypedInterfacesFSharp50VerifyIl() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions(
            [|
                "--langversion:5.0";
                "--deterministic+";
                "--define:NO_ANONYMOUS";
#if NETSTANDARD
                "--define:NETSTANDARD";
#endif
            |],
            ``Many Instantiations of the same interface - SetUp``,
            (fun verifier -> verifier.VerifyIL ["""
.class auto ansi serializable nested public implementation
       extends [mscorlib]System.Object
       implements class Program/IInterface`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<float64,float64>>,
                  class Program/IInterface`1<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>>,
                  class Program/IInterface`1<valuetype Program/AStructRecord>,
                  class Program/IInterface`1<valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<string>>,
                  class Program/IInterface`1<class Program/AnEnum>,""" +
#if NETSTANDARD
             """
                  class Program/IInterface`1<valuetype [runtime]System.ValueTuple`2<int32,int32>>,
                  class Program/IInterface`1<valuetype [System.Runtime.Numerics]System.Numerics.BigInteger>,
                  class Program/IInterface`1<valuetype [runtime]System.Decimal>,""" +
#else
             """
                  class Program/IInterface`1<valuetype [mscorlib]System.ValueTuple`2<int32,int32>>,
                  class Program/IInterface`1<valuetype [System.Numerics]System.Numerics.BigInteger>,
                  class Program/IInterface`1<valuetype [mscorlib]System.Decimal>,""" +
#endif
             """
                  class Program/IInterface`1<float64>,
                  class Program/IInterface`1<float32>,
                  class Program/IInterface`1<string>,
                  class Program/IInterface`1<char>,
                  class Program/IInterface`1<native uint>,
                  class Program/IInterface`1<native int>,
                  class Program/IInterface`1<uint64>,
                  class Program/IInterface`1<int64>,
                  class Program/IInterface`1<uint32>,
                  class Program/IInterface`1<int32>,
                  class Program/IInterface`1<uint16>,
                  class Program/IInterface`1<int16>,
                  class Program/IInterface`1<int8>,
                  class Program/IInterface`1<uint8[]>,
                  class Program/IInterface`1<uint8>,
                  class Program/IInterface`1<bool>
{
"""] ))
