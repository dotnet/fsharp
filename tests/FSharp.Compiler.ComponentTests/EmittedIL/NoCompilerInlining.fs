// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module NoCompilerInlining =

    [<Fact>]
    let ``Inline nested binding using internal value not available for cross module inlining``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module internal OuterModule
    open System.Runtime.CompilerServices

    [<assembly:InternalsVisibleTo("middleModule")>]
    do ()

    let helloWorld = "Hello World"
    let sayOuterModuleHello (msg:string) = System.Console.WriteLine(msg) """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () =
        let msg = OuterModule.helloWorld
        OuterModule.sayOuterModuleHello(msg)"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 
""" ]

    [<Fact>]
    let ``Methods marked internal not available for cross module inlining``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module internal OuterModule
    open System.Runtime.CompilerServices

    [<assembly:InternalsVisibleTo("middleModule")>]
    do ()

    let sayOuterModuleHello () = System.Console.WriteLine("Hello World") """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () = OuterModule.sayOuterModuleHello()"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 
""" ]

    [<Fact>]
    let ``Methods marked internal not available for cross module inlining 2``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module public OuterModule
    open System.Runtime.CompilerServices

    [<assembly:InternalsVisibleTo("middleModule")>]
    do ()

    let sayOuterModuleHello () = System.Console.WriteLine("Hello World") """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () =
        let x = 1
        let y = 2
        System.Console.WriteLine("x + y: {0} + {1} = ", x, y)
        OuterModule.sayOuterModuleHello()"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "x + y: {0} + {1} = "
    IL_0005:  ldc.i4.1
    IL_0006:  box        [runtime]System.Int32
    IL_000b:  ldc.i4.2
    IL_000c:  box        [runtime]System.Int32
    IL_0011:  call       void [runtime]System.Console::WriteLine(string,
                                                                        object,
                                                                        object)
    IL_0016:  ldstr      "Hello World"
    IL_001b:  call       void [runtime]System.Console::WriteLine(string)
    IL_0020:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 
""" ]


    [<Fact>]
    let ``Methods marked public available for cross module inlining``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module  OuterModule
    open System.Runtime.CompilerServices

    let sayOuterModuleHello () = System.Console.WriteLine("Hello World") """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () = OuterModule.sayOuterModuleHello()"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "Hello World"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 
""" ]


    [<Fact>]
    let ``Nested Module marked internal not available for cross module inlining``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module OuterModule
    open System.Runtime.CompilerServices

    [<assembly:InternalsVisibleTo("middleModule")>]
    do ()

    module internal nestedModule =
        let sayNestedModuleHello () = System.Console.WriteLine("Hello World")

    let sayOuterModuleHello () = nestedModule.sayNestedModuleHello () """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () = OuterModule.sayOuterModuleHello()"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "Hello World"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 
""" ]

    [<Fact>]
    let ``Nested Module marked public available for cross module inlining``() =

        let outerModule =
            FSharpWithFileName
                "outerModule.fs"
                """
module OuterModule
    open System.Runtime.CompilerServices

    module nestedModule =
        let sayNestedModuleHello () = System.Console.WriteLine("Hello World")

    let sayOuterModuleHello () = nestedModule.sayNestedModuleHello () """
            |> withOptimize
            |> asLibrary
            |> withName "outerLibrary"

        let middleModule =
            FSharpWithFileName
                "middleModule.fs"
                """
module MiddleModule
    let sayMiddleModuleHello () = OuterModule.sayOuterModuleHello()"""
            |> withOptimize
            |> withReferences [outerModule]
            |> asLibrary
            |> withName "middleModule"

        FSharpWithFileName
            "program.fs"
            """MiddleModule.sayMiddleModuleHello()"""
        |> withOptimize
        |> withReferences [middleModule; outerModule]
        |> withName "Program"
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "Hello World"
    IL_0005:  call       void [runtime]System.Console::WriteLine(string)
    IL_000a:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 
""" ]



    [<Fact>]
    let ``Function marked with NoCompilerInlining is not inlined by the compiler``() =
        FSharp """
module NoCompilerInlining

let functionInlined () = 3

[<NoCompilerInliningAttribute>]
let functionNotInlined () = 3

let x () = functionInlined () + functionNotInlined ()
"""
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  functionInlined() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  functionNotInlined() cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  x() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  call       int32 NoCompilerInlining::functionNotInlined()
    IL_0006:  add
    IL_0007:  ret
  }"""]

    [<Fact>]
    let ``Value marked with NoCompilerInlining is not inlined by the compiler``() =
        FSharp """
module NoCompilerInlining

let valueInlined = 3

[<NoCompilerInliningAttribute>]
let valueNotInlined = 3

let x () = valueInlined + valueNotInlined
"""
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
          get_valueInlined() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
          get_valueNotInlined() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  x() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  call       int32 NoCompilerInlining::get_valueNotInlined()
    IL_0006:  add
    IL_0007:  ret
  }"""

                      """
  .property int32 valueInlined()
  {
    .get int32 NoCompilerInlining::get_valueInlined()
  }"""

                      """
  .property int32 valueNotInlined()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    .get int32 NoCompilerInlining::get_valueNotInlined()
  } 
"""]