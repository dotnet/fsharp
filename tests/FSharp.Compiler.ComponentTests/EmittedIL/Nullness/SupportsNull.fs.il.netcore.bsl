




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
.assembly assembly
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed MyTestModule
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .method public static !!a  iCanProduceNullSometimes<class a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  4
    .locals init (!!a V_0,
             valuetype [runtime]System.DateTime V_1,
             valuetype [runtime]System.DateTime V_2,
             !!a V_3)
    IL_0000:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_1
    IL_0008:  call       instance int32 [runtime]System.DateTime::get_Hour()
    IL_000d:  ldc.i4.7
    IL_000e:  bne.un.s   IL_0014

    IL_0010:  ldarg.0
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  call       valuetype [runtime]System.DateTime [runtime]System.DateTime::get_Now()
    IL_0019:  stloc.2
    IL_001a:  ldloca.s   V_2
    IL_001c:  call       instance int32 [runtime]System.DateTime::get_Hour()
    IL_0021:  ldc.i4.7
    IL_0022:  bne.un.s   IL_0026

    IL_0024:  ldloc.3
    IL_0025:  ret

    IL_0026:  ldarg.0
    IL_0027:  ret
  } 

  .method public static string  iPatternMatchOnArg<class a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!a
    IL_0006:  brfalse.s  IL_000a

    IL_0008:  br.s       IL_0010

    IL_000a:  ldstr      "null"
    IL_000f:  ret

    IL_0010:  ldstr      "not null"
    IL_0015:  ret
  } 

  .method public static int32  iAcceptNullPartiallyInferredFromUnderscore<class a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } 

  .method public static int32  iAcceptNullPartiallyInferredFromNamedTypar<class a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } 

  .method public static int32  iAcceptNullWithNullAnnotation<class a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!a V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  box        !!a
    IL_0008:  brtrue.s   IL_000d

    IL_000a:  ldc.i4.1
    IL_000b:  br.s       IL_000e

    IL_000d:  ldc.i4.0
    IL_000e:  brfalse.s  IL_0012

    IL_0010:  ldc.i4.1
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } 

  .method public static int32  iAcceptNullExplicitAnnotation<class T>(!!T arg) cil managed
  {
    .param type T 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!T V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  box        !!T
    IL_0008:  brtrue.s   IL_000d

    IL_000a:  ldc.i4.1
    IL_000b:  br.s       IL_000e

    IL_000d:  ldc.i4.0
    IL_000e:  brfalse.s  IL_0012

    IL_0010:  ldc.i4.1
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } 

  .method public static !!b  fullyInferredTestCase<class a,class b>(!!a arg1,
                                                                   !!b arg2) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param type b 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (!!b V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       int32 MyTestModule::iAcceptNullPartiallyInferredFromUnderscore<!!0>(!!0)
    IL_0006:  call       void [runtime]System.Console::Write(int32)
    IL_000b:  ldarg.1
    IL_000c:  call       !!0 MyTestModule::iCanProduceNullSometimes<!!1>(!!0)
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ret
  } 

  .method public static int32  structShouldBeAllowedHere<a>(!!a arg) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    
    .maxstack  3
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  box        !!a
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       int32 MyTestModule::iAcceptNullPartiallyInferredFromUnderscore<object>(!!0)
    IL_000d:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 





