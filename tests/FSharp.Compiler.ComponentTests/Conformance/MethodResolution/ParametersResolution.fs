// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.MethodResolution

open Xunit
open FSharp.Test.Compiler

module ParametersResolution =

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly`` () =
        FSharp """
open System.Runtime.InteropServices

type Thing =
    static member Do(o: outref<int>, [<Optional; DefaultParameterValue(7)>]i: int) =
        o <- i
        i = 7

// We expect return value to be false, and out value to be 42 here.
let returnvalue1, value1 = Thing.Do(i = 42)
// Have explicit boolean check for readability here:
if returnvalue1 <> false && value1 <> 42 then
    failwith "Mismatch: Return value should be false, and out value should be 42"

// Here, we expect return value to be true, and out value to be 7
let returnvalue2, value2 = Thing.Do()
// Have explicit boolean check for readability here:
if returnvalue2<> true && value2 <> 7 then
    failwith "Mismatch: Return value should be true, and out value should be 7"
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly (verify IL)`` () =
        FSharp """
module OutOptionalTests
open System.Runtime.InteropServices

type Thing =
    static member Do(o: outref<int>, [<Optional; DefaultParameterValue(1)>]i: int) = 
        o <- i
        i = 7
let (_:bool), (_:int) = Thing.Do(i = 42)
let (_:bool), (_:int) = Thing.Do()
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> verifyIL [
        """
.class public abstract auto ansi sealed OutOptionalTests
extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 )
  .class auto ansi serializable nested public Thing
  extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .method public static bool  Do([out] int32& o,
                            [opt] int32 i) cil managed
    {
      .param [2] = int32(0x00000001)

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stobj      [runtime]System.Int32
      IL_0007:  ldarg.1
      IL_0008:  ldc.i4.7
      IL_0009:  ceq
      IL_000b:  ret
    }

  }

  .method assembly specialname static class [runtime]System.Tuple`2<bool,int32>
   get_patternInput@9() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::patternInput@9
    IL_0005:  ret
  }

  .method assembly specialname static int32
   get_outArg@9() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@9
    IL_0005:  ret
  }

  .method assembly specialname static void
   set_outArg@9(int32 'value') cil managed
  {

    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@9
    IL_0006:  ret
  }

  .method assembly specialname static class [runtime]System.Tuple`2<bool,int32>
   'get_patternInput@10-1'() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::'patternInput@10-1'
    IL_0005:  ret
  }

  .method assembly specialname static int32
   'get_outArg@10-1'() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@10-1'
    IL_0005:  ret
  }

  .method assembly specialname static void
   'set_outArg@10-1'(int32 'value') cil managed
  {

    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@10-1'
    IL_0006:  ret
  }

  .property class [runtime]System.Tuple`2<bool,int32>
   patternInput@9()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .get class [runtime]System.Tuple`2<bool,int32> OutOptionalTests::get_patternInput@9()
  }
  .property int32 outArg@9()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .set void OutOptionalTests::set_outArg@9(int32)
    .get int32 OutOptionalTests::get_outArg@9()
  }
  .property class [runtime]System.Tuple`2<bool,int32>
   'patternInput@10-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .get class [runtime]System.Tuple`2<bool,int32> OutOptionalTests::'get_patternInput@10-1'()
  }
  .property int32 'outArg@10-1'()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .set void OutOptionalTests::'set_outArg@10-1'(int32)
    .get int32 OutOptionalTests::'get_outArg@10-1'()
  }
}
        """
        """
.class private abstract auto ansi sealed '<StartupCode$assembly>'.$OutOptionalTests
extends [runtime]System.Object
{
  .field static assembly initonly class [runtime]System.Tuple`2<bool,int32> patternInput@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .field static assembly int32 outArg@9
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .field static assembly initonly class [runtime]System.Tuple`2<bool,int32> 'patternInput@10-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .field static assembly int32 'outArg@10-1'
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 )
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
  .method private specialname rtspecialname static
   void  .cctor() cil managed
  {

    .maxstack  4
    .locals init (int32& V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@9
    IL_0006:  ldsflda    int32 '<StartupCode$assembly>'.$OutOptionalTests::outArg@9
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   42
    IL_000f:  stobj      [runtime]System.Int32
    IL_0014:  ldc.i4.0
    IL_0015:  call       int32 OutOptionalTests::get_outArg@9()
    IL_001a:  newobj     instance void class [runtime]System.Tuple`2<bool,int32>::.ctor(!0,
                                                                                  !1)
    IL_001f:  stsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::patternInput@9
    IL_0024:  ldc.i4.0
    IL_0025:  stsfld     int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@10-1'
    IL_002a:  ldsflda    int32 '<StartupCode$assembly>'.$OutOptionalTests::'outArg@10-1'
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.1
    IL_0032:  stobj      [runtime]System.Int32
    IL_0037:  ldc.i4.0
    IL_0038:  call       int32 OutOptionalTests::'get_outArg@10-1'()
    IL_003d:  newobj     instance void class [runtime]System.Tuple`2<bool,int32>::.ctor(!0,
                                                                                  !1)
    IL_0042:  stsfld     class [runtime]System.Tuple`2<bool,int32> '<StartupCode$assembly>'.$OutOptionalTests::'patternInput@10-1'
    IL_0047:  ret
  }

}
        """]

    [<Fact>]
    let ``Method with optional and out parameters resolves correctly (examples from original issue: https://github.com/dotnet/fsharp/issues/12515)`` () =
        Fsx """
open System.Runtime.InteropServices;

// Define a member with both outref and default parameters. The compiler's implicit outref handling can handle this
// if required and optional parameters are provided, but not if the default parameters are left out

type Thing =
    static member Do(x: int,
                     fast: outref<bool>,
                     think: outref<float>, 
                     [<Optional;
                       DefaultParameterValue(System.Threading.CancellationToken())>] 
                     token: System.Threading.CancellationToken
                    ) : bool = 
                     true
     static member Also(x: int,
                     [<Optional;
                       DefaultParameterValue(System.Threading.CancellationToken())>] 
                     token: System.Threading.CancellationToken,
                     fast: outref<bool>,
                     think: outref<float>                     
                    ) : bool = true

// Works, was error because we can't strip the default `token` parameter for some reason
let ok, fast, think = Thing.Do(1)

// works because the outrefs are detected and provided by the compiler
let ok2, fast2, think2 = Thing.Do(1, token = System.Threading.CancellationToken.None)

// Works, was error because we can't strip the default `token` parameter for some reason
let ok3, fast3, think3 = Thing.Also(1)

// works because the outrefs are detected and provided by the compiler
let ok4, fast4, think4 = Thing.Also(1, token = System.Threading.CancellationToken.None)

// works but requires a lot of work for the user
let mutable fast5 = Unchecked.defaultof<bool>
let mutable think5 = Unchecked.defaultof<float>

let ok5 = Thing.Do(1, &fast5, &think5)
        """
    [<Fact>]
    let ``Method with same optional and out parameter does not resolve`` () =
        Fsx """
open System.Runtime.InteropServices
        
type Thing =
    static member Do([<Optional>]i: outref<bool>) = true
let _, _ = Thing.Do()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 12, Line 6, Col 22, "The member or object constructor 'Do' takes 1 argument(s) but is here given 0. The required signature is 'static member Thing.Do: i: outref<bool> -> bool'.")
        ]

