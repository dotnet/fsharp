




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
             class [runtime]System.Exception V_4,
             class [runtime]System.Exception V_5,
             object V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> V_7,
             class [runtime]System.Exception V_8)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave      IL_007e

    }  
    filter
    {
      IL_0009:  stloc.2
      IL_000a:  ldloc.2
      IL_000b:  isinst     [runtime]System.Exception
      IL_0010:  dup
      IL_0011:  brtrue.s   IL_001a

      IL_0013:  pop
      IL_0014:  ldloc.2
      IL_0015:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> ActivePatternTestCase::'|RecoverableException|_|'(class [runtime]System.Exception)
      IL_0021:  stloc.3
      IL_0022:  ldloca.s   V_3
      IL_0024:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Tag()
      IL_0029:  ldc.i4.1
      IL_002a:  bne.un.s   IL_0038

      IL_002c:  ldloca.s   V_3
      IL_002e:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Item()
      IL_0033:  stloc.s    V_4
      IL_0035:  ldc.i4.1
      IL_0036:  br.s       IL_0039

      IL_0038:  ldc.i4.0
      IL_0039:  endfilter
    }  
    {  
      IL_003b:  stloc.s    V_6
      IL_003d:  ldloc.s    V_6
      IL_003f:  isinst     [runtime]System.Exception
      IL_0044:  dup
      IL_0045:  brtrue.s   IL_004f

      IL_0047:  pop
      IL_0048:  ldloc.s    V_6
      IL_004a:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_004f:  stloc.s    V_5
      IL_0051:  ldloc.s    V_5
      IL_0053:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception> ActivePatternTestCase::'|RecoverableException|_|'(class [runtime]System.Exception)
      IL_0058:  stloc.s    V_7
      IL_005a:  ldloca.s   V_7
      IL_005c:  call       instance int32 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Tag()
      IL_0061:  ldc.i4.1
      IL_0062:  bne.un.s   IL_0073

      IL_0064:  ldloca.s   V_7
      IL_0066:  call       instance !0 valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Exception>::get_Item()
      IL_006b:  stloc.s    V_8
      IL_006d:  ldarg.0
      IL_006e:  ldarg.1
      IL_006f:  add
      IL_0070:  stloc.0
      IL_0071:  leave.s    IL_007e

      IL_0073:  rethrow
      IL_0075:  ldnull
      IL_0076:  unbox.any  [runtime]System.Int32
      IL_007b:  stloc.0
      IL_007c:  leave.s    IL_007e

    }  
    IL_007e:  ldloc.0
    IL_007f:  ret
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





