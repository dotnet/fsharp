




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed ActivePatternTestCase
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public specialname static valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> '|RecoverableException|_|'(class [runtime]System.Exception exn) cil managed
  {
    .param [0]
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [runtime]System.OperationCanceledException V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst     [runtime]System.OperationCanceledException
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brfalse.s  IL_0012

    IL_000c:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_ValueNone()
    IL_0011:  ret

    IL_0012:  ldarg.0
    IL_0013:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::NewValueSome(!0)
    IL_0018:  ret
  } 

  .method public static int32  addWithActivePattern(int32 a,
                                                    int32 b) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (int32 V_0,
             class [runtime]System.Exception V_1,
             object V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> V_3,
             class [runtime]System.Exception V_4)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave.s    IL_0043

    }  
    catch [runtime]System.Object 
    {
      IL_0006:  stloc.2
      IL_0007:  ldloc.2
      IL_0008:  isinst     [runtime]System.Exception
      IL_000d:  dup
      IL_000e:  brtrue.s   IL_0017

      IL_0010:  pop
      IL_0011:  ldloc.2
      IL_0012:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0017:  stloc.1
      IL_0018:  ldloc.1
      IL_0019:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> ActivePatternTestCase::'|RecoverableException|_|'(class [runtime]System.Exception)
      IL_001e:  stloc.3
      IL_001f:  ldloca.s   V_3
      IL_0021:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Tag()
      IL_0026:  ldc.i4.1
      IL_0027:  bne.un.s   IL_0038

      IL_0029:  ldloca.s   V_3
      IL_002b:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Item()
      IL_0030:  stloc.s    V_4
      IL_0032:  ldarg.0
      IL_0033:  ldarg.1
      IL_0034:  add
      IL_0035:  stloc.0
      IL_0036:  leave.s    IL_0043

      IL_0038:  rethrow
      IL_003a:  ldnull
      IL_003b:  unbox.any  [runtime]System.Int32
      IL_0040:  stloc.0
      IL_0041:  leave.s    IL_0043

    }  
    IL_0043:  ldloc.0
    IL_0044:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ActivePatternTestCase
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 





