namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Inlining =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=Match01.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Match01.dll"	# Match01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Match01.fs"|])>]
    let ``Match01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Match02.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Match02.dll"	# Match02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Match02.fs"|])>]
    let ``Match02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=StructUnion01.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructUnion01.dll"	# StructUnion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructUnion01.fs"|])>]
    let ``StructUnion01_fs`` compilation =
        compilation
        |> verifyCompilation


    [<Fact>]
    let ``List contains inlining`` () =
        Fsx """module Test
let data = [nanf |> float;5.0;infinity]
let found = data |> List.contains nan
        """
        |> asExe
        |> compile
        (* This is the essential aspect of the IL we are interested in - doing a direct specialized 'ceq' on primitive values, and not going via a GenericEqualityIntrinsic call*)
        |> verifyIL ["""
    .method assembly static bool  contains@1<a>(!!a e,
                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> xs1) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  4
        .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> V_0,
                 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64> V_1,
                 float64 V_2)
        IL_0000:  ldarg.1
        IL_0001:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_TailOrNull()
        IL_0006:  brfalse.s  IL_000a
    
        IL_0008:  br.s       IL_000c
    
        IL_000a:  ldc.i4.0
        IL_000b:  ret
    
        IL_000c:  ldarg.1
        IL_000d:  stloc.0
        IL_000e:  ldloc.0
        IL_000f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_TailOrNull()
        IL_0014:  stloc.1
        IL_0015:  ldloc.0
        IL_0016:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<float64>::get_HeadOrDefault()
        IL_001b:  stloc.2
        IL_001c:  ldc.r8     (00 00 00 00 00 00 F8 FF)
        IL_0025:  ldloc.2
        IL_0026:  ceq
        IL_0028:  brfalse.s  IL_002c
    
        IL_002a:  ldc.i4.1
        IL_002b:  ret
    
        IL_002c:  ldarg.0
        IL_002d:  ldloc.1
        IL_002e:  starg.s    xs1
        IL_0030:  starg.s    e
        IL_0032:  br.s       IL_0000
      }"""]

