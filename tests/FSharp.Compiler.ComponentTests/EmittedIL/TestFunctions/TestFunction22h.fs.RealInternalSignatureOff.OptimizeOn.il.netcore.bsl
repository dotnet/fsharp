




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
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
             class [runtime]System.Exception V_2)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0044

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
      IL_001a:  br.s       IL_0020

      IL_001c:  ldc.i4.0
      IL_001d:  nop
      IL_001e:  br.s       IL_0020

      IL_0020:  endfilter
    }  
    {  
      IL_0022:  castclass  [runtime]System.Exception
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  isinst     [runtime]System.ArgumentException
      IL_002e:  stloc.1
      IL_002f:  ldloc.1
      IL_0030:  brfalse.s  IL_0039

      IL_0032:  call       void [runtime]System.Console::WriteLine()
      IL_0037:  leave.s    IL_0044

      IL_0039:  rethrow
      IL_003b:  ldnull
      IL_003c:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_0041:  pop
      IL_0042:  leave.s    IL_0044

    }  
    IL_0044:  ret
  } 

  .method public static void  test3() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [runtime]System.ArgumentException V_1,
             class [runtime]System.ArgumentException V_2,
             class [runtime]System.Exception V_3)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_004e

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
      IL_001c:  br.s       IL_0022

      IL_001e:  ldc.i4.0
      IL_001f:  nop
      IL_0020:  br.s       IL_0022

      IL_0022:  endfilter
    }  
    {  
      IL_0024:  castclass  [runtime]System.Exception
      IL_0029:  stloc.3
      IL_002a:  ldloc.3
      IL_002b:  isinst     [runtime]System.ArgumentException
      IL_0030:  stloc.1
      IL_0031:  ldloc.1
      IL_0032:  brfalse.s  IL_0043

      IL_0034:  ldloc.1
      IL_0035:  stloc.2
      IL_0036:  ldloc.2
      IL_0037:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_003c:  call       void [runtime]System.Console::WriteLine(string)
      IL_0041:  leave.s    IL_004e

      IL_0043:  rethrow
      IL_0045:  ldnull
      IL_0046:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_004b:  pop
      IL_004c:  leave.s    IL_004e

    }  
    IL_004e:  ret
  } 

  .method public static void  test4() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException V_1,
             string V_2,
             class [runtime]System.Exception V_3)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_005d

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
      IL_0026:  br.s       IL_002c

      IL_0028:  ldc.i4.0
      IL_0029:  nop
      IL_002a:  br.s       IL_002c

      IL_002c:  endfilter
    }  
    {  
      IL_002e:  castclass  [runtime]System.Exception
      IL_0033:  stloc.3
      IL_0034:  ldloc.3
      IL_0035:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_003a:  stloc.1
      IL_003b:  ldloc.1
      IL_003c:  brfalse.s  IL_0052

      IL_003e:  ldloc.3
      IL_003f:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0044:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0049:  stloc.2
      IL_004a:  ldloc.2
      IL_004b:  call       void [runtime]System.Console::WriteLine(string)
      IL_0050:  leave.s    IL_005d

      IL_0052:  rethrow
      IL_0054:  ldnull
      IL_0055:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_005a:  pop
      IL_005b:  leave.s    IL_005d

    }  
    IL_005d:  ret
  } 

  .method public static void  test5() cil managed
  {
    
    .maxstack  3
    .locals init (class [runtime]System.Exception V_0,
             class [runtime]System.ArgumentException V_1)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0051

    }  
    catch [runtime]System.Object 
    {
      IL_0008:  castclass  [runtime]System.Exception
      IL_000d:  stloc.0
      IL_000e:  ldloc.0
      IL_000f:  isinst     [runtime]System.ArgumentException
      IL_0014:  brtrue.s   IL_0020

      IL_0016:  ldloc.0
      IL_0017:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_001c:  brfalse.s  IL_0046

      IL_001e:  br.s       IL_0034

      IL_0020:  ldloc.0
      IL_0021:  unbox.any  [runtime]System.ArgumentException
      IL_0026:  stloc.1
      IL_0027:  ldloc.1
      IL_0028:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_002d:  call       void [runtime]System.Console::WriteLine(string)
      IL_0032:  leave.s    IL_0051

      IL_0034:  ldloc.0
      IL_0035:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_003a:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
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





