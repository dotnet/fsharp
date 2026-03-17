




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
             class [runtime]System.Exception V_2,
             class [runtime]System.ArgumentException V_3)
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
      IL_002e:  stloc.3
      IL_002f:  ldloc.3
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
             class [runtime]System.Exception V_3,
             class [runtime]System.ArgumentException V_4,
             class [runtime]System.ArgumentException V_5)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0053

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
      IL_0030:  stloc.s    V_4
      IL_0032:  ldloc.s    V_4
      IL_0034:  brfalse.s  IL_0048

      IL_0036:  ldloc.s    V_4
      IL_0038:  stloc.s    V_5
      IL_003a:  ldloc.s    V_5
      IL_003c:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_0041:  call       void [runtime]System.Console::WriteLine(string)
      IL_0046:  leave.s    IL_0053

      IL_0048:  rethrow
      IL_004a:  ldnull
      IL_004b:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_0050:  pop
      IL_0051:  leave.s    IL_0053

    }  
    IL_0053:  ret
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
      IL_0006:  leave.s    IL_0061

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
      IL_003a:  stloc.s    V_4
      IL_003c:  ldloc.s    V_4
      IL_003e:  brfalse.s  IL_0056

      IL_0040:  ldloc.3
      IL_0041:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0046:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_004b:  stloc.s    V_5
      IL_004d:  ldloc.s    V_5
      IL_004f:  call       void [runtime]System.Console::WriteLine(string)
      IL_0054:  leave.s    IL_0061

      IL_0056:  rethrow
      IL_0058:  ldnull
      IL_0059:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_005e:  pop
      IL_005f:  leave.s    IL_0061

    }  
    IL_0061:  ret
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





