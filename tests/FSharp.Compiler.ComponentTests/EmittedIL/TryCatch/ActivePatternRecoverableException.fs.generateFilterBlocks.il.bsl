




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
             valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> V_2,
             class [runtime]System.Exception V_3,
             class [runtime]System.Exception V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> V_5,
             class [runtime]System.Exception V_6)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave.s    IL_005f

    }  
    filter
    {
      IL_0006:  castclass  [runtime]System.Exception
      IL_000b:  stloc.1
      IL_000c:  ldloc.1
      IL_000d:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> ActivePatternTestCase::'|RecoverableException|_|'(class [runtime]System.Exception)
      IL_0012:  stloc.2
      IL_0013:  ldloca.s   V_2
      IL_0015:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Tag()
      IL_001a:  ldc.i4.1
      IL_001b:  bne.un.s   IL_0028

      IL_001d:  ldloca.s   V_2
      IL_001f:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Item()
      IL_0024:  stloc.3
      IL_0025:  ldc.i4.1
      IL_0026:  br.s       IL_0029

      IL_0028:  ldc.i4.0
      IL_0029:  endfilter
    }  
    {  
      IL_002b:  castclass  [runtime]System.Exception
      IL_0030:  stloc.s    V_4
      IL_0032:  ldloc.s    V_4
      IL_0034:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> ActivePatternTestCase::'|RecoverableException|_|'(class [runtime]System.Exception)
      IL_0039:  stloc.s    V_5
      IL_003b:  ldloca.s   V_5
      IL_003d:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Tag()
      IL_0042:  ldc.i4.1
      IL_0043:  bne.un.s   IL_0054

      IL_0045:  ldloca.s   V_5
      IL_0047:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Item()
      IL_004c:  stloc.s    V_6
      IL_004e:  ldarg.0
      IL_004f:  ldarg.1
      IL_0050:  add
      IL_0051:  stloc.0
      IL_0052:  leave.s    IL_005f

      IL_0054:  rethrow
      IL_0056:  ldnull
      IL_0057:  unbox.any  [runtime]System.Int32
      IL_005c:  stloc.0
      IL_005d:  leave.s    IL_005f

    }  
    IL_005f:  ldloc.0
    IL_0060:  ret
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






