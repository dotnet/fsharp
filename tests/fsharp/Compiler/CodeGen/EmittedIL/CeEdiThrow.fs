// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Compiler.UnitTests
open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module CeEdiThrow =

    [<Test>]
    let ``Emits EDI.Throw``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module CE

open System
type Try() =
    member _.Return i = i
    member _.Delay f = f
    member _.Run f = f()
    member _.TryWith(body : unit -> int, catch : exn -> int) =
        try body() with ex -> catch ex

let foo = Try(){
    try return invalidOp "Ex"
    with :? ArgumentException -> return 1
}
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public strict virtual instance int32
          Invoke(class [runtime]System.Exception _arg1) cil managed
  {

    .maxstack  5
    .locals init (class [runtime]System.ArgumentException V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     [runtime]System.ArgumentException
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  ldc.i4.1
    IL_000b:  ret

    IL_000c:  ldarg.1
    IL_000d:  call       class [runtime]System.Runtime.ExceptionServices.ExceptionDispatchInfo [runtime]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Capture(class [runtime]System.Exception)
    IL_0012:  callvirt   instance void [runtime]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Throw()
    IL_0017:  ldc.i4.0
    IL_0018:  ret
 }
            """
            ])
