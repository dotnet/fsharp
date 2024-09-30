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

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

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

    [<InlineData(true, "public")>]          // RealSig
    [<InlineData(false, "assembly")>]       // Regular
    [<Theory>]
    let ``private DU in module`` (realSig, expected) =
        FSharp """
module RealInternalSignature
module Module =
    type private DU = ABC | YYZ

    let publicFunction () : bool =
        ABC = YYZ

Module.publicFunction () |> printfn "%b"
"""
        |> asExe
        |> withRealInternalSignature realSig
        |> compileAndRun
        |> withILContains [
            $$"""
      .method {{expected}} hidebysig instance bool Equals(class RealInternalSignature/Module/DU obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (int32 V_0,
                 int32 V_1)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_001b

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0019

        IL_0006:  ldarg.0
        IL_0007:  ldfld      int32 RealInternalSignature/Module/DU::_tag
        IL_000c:  stloc.0
        IL_000d:  ldarg.1
        IL_000e:  ldfld      int32 RealInternalSignature/Module/DU::_tag
        IL_0013:  stloc.1
        IL_0014:  ldloc.0
        IL_0015:  ldloc.1
        IL_0016:  ceq
        IL_0018:  ret

        IL_0019:  ldc.i4.0
        IL_001a:  ret

        IL_001b:  ldarg.1
        IL_001c:  ldnull
        IL_001d:  cgt.un
        IL_001f:  ldc.i4.0
        IL_0020:  ceq
        IL_0022:  ret
      } 
"""
            ]
        |> shouldSucceed
