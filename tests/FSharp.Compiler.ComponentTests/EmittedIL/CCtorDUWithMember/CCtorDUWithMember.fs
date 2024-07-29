namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CCtorDUWithMember =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember01a.fs"|])>]
    let ``CCtorDUWithMember01a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> asFs
        |> withRealInternalSignatureOn
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember01.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember01a.fs"|])>]
    let ``CCtorDUWithMember01a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> asFs
        |> withRealInternalSignatureOff
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember01.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember02a.fs"|])>]
    let ``CCtorDUWithMember02a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember02.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember02a.fs"|])>]
    let ``CCtorDUWithMember02a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember02.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember03a.fs"|])>]
    let ``CCtorDUWithMember03a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember03.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember03a.fs"|])>]
    let ``CCtorDUWithMember03a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember03.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[| "CCtorDUWithMember04a.fs" |])>]
    let ``CCtorDUWithMember04a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember04.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[| "CCtorDUWithMember04a.fs" |])>]
    let ``CCtorDUWithMember04a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember04.fs"))
        |> verifyCompilation 



    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[| "CCtorDUWithInternalCase.fsi" |])>]
    let ``CCtorDUWithInternalCase_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithInternalCase.fs"))
        |> verifyCompilation 


    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[| "CCtorDUWithInternalCase.fsi" |])>]
    let ``CCtorDUWithInternalCase_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithInternalCase.fs"))
        |> verifyCompilation 


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public - DU internal case specified in fsi file`` (realSig) =
        Fsi """
namespace RealInternalSignature

[<StructuralEquality; StructuralComparison>]
type ILArrayShape =
    internal 
    | One
"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

[<StructuralEquality; StructuralComparison>]
type ILArrayShape =
    | One
        """))
        |> asLibrary
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            """
  .method public hidebysig instance bool 
          Equals(class RealInternalSignature.ILArrayShape obj,
                 class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
"""
            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public - DU internal case specified in fs file`` (realSig) =
        FSharp """
namespace RealInternalSignature

[<StructuralEquality; StructuralComparison>]
type ILArrayShape =
    internal 
    | One
"""
        |> asLibrary
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            """
      .method public hidebysig virtual final 
          instance bool  Equals(class RealInternalSignature.ILArrayShape obj,
                                class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0008

    IL_0003:  ldarg.1
    IL_0004:  ldnull
    IL_0005:  cgt.un
    IL_0007:  ret

    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  cgt.un
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  ret
  } 

  .method public hidebysig virtual final 
          instance bool  Equals(object obj,
                                class [runtime]System.Collections.IEqualityComparer comp) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  5
    .locals init (class RealInternalSignature.ILArrayShape V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     RealInternalSignature.ILArrayShape
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  callvirt   instance bool RealInternalSignature.ILArrayShape::Equals(class RealInternalSignature.ILArrayShape,
                                                                                  class [runtime]System.Collections.IEqualityComparer)
    IL_0012:  ret

    IL_0013:  ldc.i4.0
    IL_0014:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(class RealInternalSignature.ILArrayShape obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0008

    IL_0003:  ldarg.1
    IL_0004:  ldnull
    IL_0005:  cgt.un
    IL_0007:  ret

    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  cgt.un
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  ret
  } 

  .method public hidebysig virtual final instance bool  Equals(object obj) cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  4
    .locals init (class RealInternalSignature.ILArrayShape V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     RealInternalSignature.ILArrayShape
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0012

    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt   instance bool RealInternalSignature.ILArrayShape::Equals(class RealInternalSignature.ILArrayShape)
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } 
"""
            ]
        |> shouldSucceed

