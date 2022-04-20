// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open NUnit.Framework

open System
open System.Threading.Tasks

[<TestFixture>]
module ByrefTailcalls =

    [<Test>]
    let ``check no tailcall to inref``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([| "/optimize";"/tailcalls" |],
            """
module Test

type Getter<'T, 'FT> = delegate of inref<'T> -> 'FT 

type GetterWrapper<'T, 'FT> (getter : Getter<'T, 'FT>) =
    member _.Get (instance : 'T) = getter.Invoke &instance

            """,
            (fun verifier -> verifier.VerifyIL [
            """
.method public hidebysig instance !FT 
        Get(!T 'instance') cil managed
{
  
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldfld      class Test/Getter`2<!0,!1> class Test/GetterWrapper`2<!T,!FT>::getter
  IL_0006:  ldarga.s   'instance'
  IL_0008:  callvirt   instance !1 class Test/Getter`2<!T,!FT>::Invoke(!0& modreq([runtime]System.Runtime.InteropServices.InAttribute))
  IL_000d:  ret
} 
                """
            ]))
