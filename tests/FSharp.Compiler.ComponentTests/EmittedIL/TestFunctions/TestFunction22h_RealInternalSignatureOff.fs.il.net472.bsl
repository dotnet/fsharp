




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.assembly
{
  
  
}
.mresource public FSharpOptimizationData.assembly
{
  
  
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  test1() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0015

    }  
    catch [runtime]System.Object 
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  call       void [runtime]System.Console::WriteLine()
      IL_0013:  leave.s    IL_0015

    }  
    IL_0015:  ret
  } 

  .method public static void  test2() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [runtime]System.ArgumentException V_1,
             class [runtime]System.Exception V_2,
             class [runtime]System.ArgumentException V_3)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0042

    }  
    filter
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  isinst     [runtime]System.ArgumentException
      IL_0014:  stloc.1
      IL_0015:  ldloc.1
      IL_0016:  brfalse.s  IL_001c

      IL_0018:  ldc.i4.1
      IL_0019:  nop
      IL_001a:  br.s       IL_001e

      IL_001c:  ldc.i4.0
      IL_001d:  nop
      IL_001e:  endfilter
    }  
    {  
      IL_0020:  castclass  [runtime]System.Exception
      IL_0025:  stloc.2
      IL_0026:  ldloc.2
      IL_0027:  isinst     [runtime]System.ArgumentException
      IL_002c:  stloc.3
      IL_002d:  ldloc.3
      IL_002e:  brfalse.s  IL_0037

      IL_0030:  call       void [runtime]System.Console::WriteLine()
      IL_0035:  leave.s    IL_0042

      IL_0037:  rethrow
      IL_0039:  ldnull
      IL_003a:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_003f:  pop
      IL_0040:  leave.s    IL_0042

    }  
    IL_0042:  ret
  } 

  .method public static void  test3() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [runtime]System.ArgumentException V_1,
             class [runtime]System.ArgumentException V_2,
             class [runtime]System.Exception V_3,
             class [runtime]System.ArgumentException V_4,
             class [runtime]System.ArgumentException V_5)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0051

    }  
    filter
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  isinst     [runtime]System.ArgumentException
      IL_0014:  stloc.1
      IL_0015:  ldloc.1
      IL_0016:  brfalse.s  IL_001e

      IL_0018:  ldloc.1
      IL_0019:  stloc.2
      IL_001a:  ldc.i4.1
      IL_001b:  nop
      IL_001c:  br.s       IL_0020

      IL_001e:  ldc.i4.0
      IL_001f:  nop
      IL_0020:  endfilter
    }  
    {  
      IL_0022:  castclass  [runtime]System.Exception
      IL_0027:  stloc.3
      IL_0028:  ldloc.3
      IL_0029:  isinst     [runtime]System.ArgumentException
      IL_002e:  stloc.s    V_4
      IL_0030:  ldloc.s    V_4
      IL_0032:  brfalse.s  IL_0046

      IL_0034:  ldloc.s    V_4
      IL_0036:  stloc.s    V_5
      IL_0038:  ldloc.s    V_5
      IL_003a:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_003f:  call       void [runtime]System.Console::WriteLine(string)
      IL_0044:  leave.s    IL_0051

      IL_0046:  rethrow
      IL_0048:  ldnull
      IL_0049:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_004e:  pop
      IL_004f:  leave.s    IL_0051

    }  
    IL_0051:  ret
  } 

  .method public static void  test4() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException V_1,
             string V_2,
             class [runtime]System.Exception V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException V_4,
             string V_5)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_005f

    }  
    filter
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0014:  stloc.1
      IL_0015:  ldloc.1
      IL_0016:  brfalse.s  IL_0028

      IL_0018:  ldloc.0
      IL_0019:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_001e:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0023:  stloc.2
      IL_0024:  ldc.i4.1
      IL_0025:  nop
      IL_0026:  br.s       IL_002a

      IL_0028:  ldc.i4.0
      IL_0029:  nop
      IL_002a:  endfilter
    }  
    {  
      IL_002c:  castclass  [runtime]System.Exception
      IL_0031:  stloc.3
      IL_0032:  ldloc.3
      IL_0033:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0038:  stloc.s    V_4
      IL_003a:  ldloc.s    V_4
      IL_003c:  brfalse.s  IL_0054

      IL_003e:  ldloc.3
      IL_003f:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0044:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0049:  stloc.s    V_5
      IL_004b:  ldloc.s    V_5
      IL_004d:  call       void [runtime]System.Console::WriteLine(string)
      IL_0052:  leave.s    IL_005f

      IL_0054:  rethrow
      IL_0056:  ldnull
      IL_0057:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_005c:  pop
      IL_005d:  leave.s    IL_005f

    }  
    IL_005f:  ret
  } 

  .method public static void  test5() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             object V_2,
             class [runtime]System.ArgumentException V_3,
             string V_4)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_005f

    }  
    catch [runtime]System.Object 
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  stloc.1
      IL_0010:  ldloc.1
      IL_0011:  isinst     [runtime]System.ArgumentException
      IL_0016:  ldnull
      IL_0017:  cgt.un
      IL_0019:  brtrue.s   IL_002a

      IL_001b:  ldloc.0
      IL_001c:  stloc.2
      IL_001d:  ldloc.2
      IL_001e:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0023:  ldnull
      IL_0024:  cgt.un
      IL_0026:  brfalse.s  IL_0054

      IL_0028:  br.s       IL_003e

      IL_002a:  ldloc.0
      IL_002b:  unbox.any  [runtime]System.ArgumentException
      IL_0030:  stloc.3
      IL_0031:  ldloc.3
      IL_0032:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_0037:  call       void [runtime]System.Console::WriteLine(string)
      IL_003c:  leave.s    IL_005f

      IL_003e:  ldloc.0
      IL_003f:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0044:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0049:  stloc.s    V_4
      IL_004b:  ldloc.s    V_4
      IL_004d:  call       void [runtime]System.Console::WriteLine(string)
      IL_0052:  leave.s    IL_005f

      IL_0054:  rethrow
      IL_0056:  ldnull
      IL_0057:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_005c:  pop
      IL_005d:  leave.s    IL_005f

    }  
    IL_005f:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






