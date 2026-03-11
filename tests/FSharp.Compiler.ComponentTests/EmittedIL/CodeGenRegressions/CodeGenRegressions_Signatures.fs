// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions_Signatures =

    let private getActualIL (result: CompilationResult) =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some p ->
                let (_, _, actualIL) = ILChecker.verifyILAndReturnActual [] p [ "// dummy" ]
                actualIL
            | None -> failwith "No output path"
        | _ -> failwith "Compilation failed"

    // https://github.com/dotnet/fsharp/issues/18135
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Issue_18135_StaticAbstractByrefParams`` () =
        let source = """
module Test

#nowarn "3535"

[<Interface>]
type I =
    static abstract Foo: int inref -> int

type T =
    interface I with
        static member Foo i = i

let f<'T when 'T :> I>() =
    let x = 123
    printfn "%d" ('T.Foo &x)

[<EntryPoint>]
let main _ =
    f<T>()
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13468
    [<Fact>]
    let ``Issue_13468_OutrefAsByref_IL`` () =
        let csCode = "namespace CSharpLib { public interface IOutTest { void TryGet(string k, out int v); } }"
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = "module Test\nopen CSharpLib\ntype MyImpl() =\n    interface IOutTest with\n        member this.TryGet(k, v) = v <- 42"
        let actualIL =
            FSharp fsCode
            |> withReferences [csLib]
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getActualIL
        Assert.Contains("[out]", actualIL)

    // https://github.com/dotnet/fsharp/issues/13468
    [<Fact>]
    let ``Issue_13468_OutrefAsByref_Runtime`` () =
        let csCode = """
namespace CSharpLib {
    public interface IOutTest { bool TryGet(string k, out int v); }
    public static class OutTestHelper {
        public static string Run(IOutTest impl) {
            int v;
            bool ok = impl.TryGet("key", out v);
            return ok ? v.ToString() : "fail";
        }
    }
}"""
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = """
module Test
open CSharpLib
type MyImpl() =
    interface IOutTest with
        member this.TryGet(k, v) = v <- 42; true

[<EntryPoint>]
let main _ =
    let result = OutTestHelper.Run(MyImpl())
    if result <> "42" then failwithf "Expected 42, got %s" result
    printfn "Success: %s" result
    0
"""
        FSharp fsCode
        |> withReferences [csLib]
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

