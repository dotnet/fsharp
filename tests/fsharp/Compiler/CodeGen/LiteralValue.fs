// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Compiler.UnitTests
open NUnit.Framework

[<TestFixture>]
module ``Literal Value`` =

    [<Test>]
    let ``Literal Value``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module LiteralValue

[<Literal>]
let x = 7

[<EntryPoint>]
let main _ =
    0
            """
            (fun verifier -> verifier.VerifyIL [
            """
.class public abstract auto ansi sealed LiteralValue
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
  .field public static literal int32 x = int32(0x00000007)
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.LiteralAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .method public static int32  main(string[] _arg1) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 )

    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  }

}
            """
            ])
