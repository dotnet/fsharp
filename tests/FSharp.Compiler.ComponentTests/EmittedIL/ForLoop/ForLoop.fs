namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ForLoop =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=NoAllocationOfTuple01.fs SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoAllocationOfTuple01.dll" # NoAllocationOfTuple01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoAllocationOfTuple01.fs"|])>]
    let ``NoAllocationOfTuple01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnArray01.fs      SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnArray01.dll"      # ForEachOnArray01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnArray01.fs"|])>]
    let ``ForEachOnArray01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnList01.fs       SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnList01.dll"       # ForEachOnList01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnList01.fs"|])>]
    let ``ForEachOnList01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForEachOnString01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForEachOnString01.dll"     # ForEachOnString01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForEachOnString01.fs"|])>]
    let ``ForEachOnString01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength01.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength01.dll"     # ZeroToArrLength01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ZeroToArrLength01.fs"|])>]
    let ``ZeroToArrLength01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ZeroToArrLength02.fs     SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ZeroToArrLength02.dll"     # ZeroToArrLength02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ZeroToArrLength02.fs"|])>]
    let ``ZeroToArrLength02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable01.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable01.dll"            # NoIEnumerable01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable01.fsx"|])>]
    let ``NoIEnumerable01_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable02.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable02.dll"            # NoIEnumerable02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable02.fsx"|])>]
    let ``NoIEnumerable02_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoIEnumerable03.fsx SCFLAGS="-a -g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoIEnumerable03.dll"            # NoIEnumerable03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoIEnumerable03.fsx"|])>]
    let ``NoIEnumerable03_fsx`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd01.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd01.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd01.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd01.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd01_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd02.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd02.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd02.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd02.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd02_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd03.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd03.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd03.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd03.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd03_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd04.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd04.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd04.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd04.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd04_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize+"	# NonTrivialBranchingBindingInEnd05.fs --optimize+
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".opt", Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_opt`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonTrivialBranchingBindingInEnd05.fs SCFLAGS="--optimize-"	# NonTrivialBranchingBindingInEnd05.fs --optimize-
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonTrivialBranchingBindingInEnd05.fs"|])>]
    let ``NonTrivialBranchingBindingInEnd05_fs_nonopt`` compilation =
        compilation
        |> verifyCompilation

    [<Fact>]
    let ``Strided integer for loops should not allocate a RangeInt32 enumerator``() =
        FSharp """
module ForLoops

let loop1 () =
    let s = System.Random().Next(2, 2) // avoid constant step optimization
    for i in 0..s..10 do
        System.Console.WriteLine i
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
  .method public static void  loop1() cil managed
  {

    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  newobj     instance void [mscorlib]System.Random::.ctor()
    IL_0005:  ldc.i4.2
    IL_0006:  ldc.i4.2
    IL_0007:  callvirt   instance int32 [mscorlib]System.Random::Next(int32,
                                                                      int32)
    IL_000c:  stloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.3
    IL_000f:  ldloc.0
    IL_0010:  stloc.2
    IL_0011:  ldloc.2
    IL_0012:  brtrue.s   IL_0024

    IL_0014:  ldstr      "The step of a range cannot be zero."
    IL_0019:  ldstr      "step"
    IL_001e:  newobj     instance void [mscorlib]System.ArgumentException::.ctor(string,
                                                                                 string)
    IL_0023:  throw

    IL_0024:  ldc.i4.s   10
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  ldloc.3
    IL_0029:  blt.s      IL_002f

    IL_002b:  ldloc.2
    IL_002c:  ldc.i4.0
    IL_002d:  bgt.s      IL_0037

    IL_002f:  ldloc.1
    IL_0030:  ldloc.3
    IL_0031:  bgt.s      IL_0051

    IL_0033:  ldloc.2
    IL_0034:  ldc.i4.0
    IL_0035:  bge.s      IL_0051

    IL_0037:  ldloc.3
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003d:  ldloc.3
    IL_003e:  ldloc.2
    IL_003f:  add
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  ldloc.1
    IL_0043:  ble.s      IL_0049

    IL_0045:  ldloc.2
    IL_0046:  ldc.i4.0
    IL_0047:  bgt.s      IL_0051

    IL_0049:  ldloc.3
    IL_004a:  ldloc.1
    IL_004b:  bge.s      IL_0037

    IL_004d:  ldloc.2
    IL_004e:  ldc.i4.0
    IL_004f:  bge.s      IL_0037

    IL_0051:  ret
  }""" ]

    [<Fact>]
    let ``Strided integer for loops with constant step should be optimized``() =
        FSharp """
module ForLoops

let loop2 () =
    for i in 0..2..10 do
        System.Console.WriteLine i
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
  .method public static void  loop2() cil managed
  {

    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.1
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  blt.s      IL_0017

    IL_0009:  ldloc.1
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.2
    IL_0011:  add
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldloc.0
    IL_0015:  ble.s      IL_0009

    IL_0017:  ret
  }""" ]

    [<Fact>]
    let ``Strided integer for loops with constant negative step should be optimized``() =
        FSharp """
module ForLoops

let loop2 () =
    for i in 10 .. -2 .. 1 do
        System.Console.WriteLine i
        """
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
  .method public static void  loop2() cil managed
  {

    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.s   10
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  bgt.s      IL_0017

    IL_0009:  ldloc.1
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.2
    IL_0011:  sub
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldloc.0
    IL_0015:  bge.s      IL_0009

    IL_0017:  ret
  }""" ]