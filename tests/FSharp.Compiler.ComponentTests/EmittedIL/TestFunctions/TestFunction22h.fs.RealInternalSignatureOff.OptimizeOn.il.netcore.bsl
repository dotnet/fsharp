




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
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0021

    }  
    catch [runtime]System.Object 
    {
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  isinst     [runtime]System.Exception
      IL_000f:  dup
      IL_0010:  brtrue.s   IL_0019

      IL_0012:  pop
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0019:  stloc.0
      IL_001a:  call       void [runtime]System.Console::WriteLine()
      IL_001f:  leave.s    IL_0021

    }  
    IL_0021:  ret
  } 

  .method public static void  test2() cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             class [runtime]System.ArgumentException V_2,
             class [runtime]System.Exception V_3,
             object V_4)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_005d

    }  
    filter
    {
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  isinst     [runtime]System.Exception
      IL_000f:  dup
      IL_0010:  brtrue.s   IL_0019

      IL_0012:  pop
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  isinst     [runtime]System.ArgumentException
      IL_0020:  stloc.2
      IL_0021:  ldloc.2
      IL_0022:  brfalse.s  IL_0028

      IL_0024:  ldc.i4.1
      IL_0025:  nop
      IL_0026:  br.s       IL_002a

      IL_0028:  ldc.i4.0
      IL_0029:  nop
      IL_002a:  endfilter
    }  
    {  
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  isinst     [runtime]System.Exception
      IL_0035:  dup
      IL_0036:  brtrue.s   IL_0040

      IL_0038:  pop
      IL_0039:  ldloc.s    V_4
      IL_003b:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0040:  stloc.3
      IL_0041:  ldloc.3
      IL_0042:  isinst     [runtime]System.ArgumentException
      IL_0047:  stloc.2
      IL_0048:  ldloc.2
      IL_0049:  brfalse.s  IL_0052

      IL_004b:  call       void [runtime]System.Console::WriteLine()
      IL_0050:  leave.s    IL_005d

      IL_0052:  rethrow
      IL_0054:  ldnull
      IL_0055:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_005a:  pop
      IL_005b:  leave.s    IL_005d

    }  
    IL_005d:  ret
  } 

  .method public static void  test3() cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             class [runtime]System.ArgumentException V_2,
             class [runtime]System.ArgumentException V_3,
             class [runtime]System.Exception V_4,
             object V_5)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_0069

    }  
    filter
    {
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  isinst     [runtime]System.Exception
      IL_000f:  dup
      IL_0010:  brtrue.s   IL_0019

      IL_0012:  pop
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  isinst     [runtime]System.ArgumentException
      IL_0020:  stloc.2
      IL_0021:  ldloc.2
      IL_0022:  brfalse.s  IL_002a

      IL_0024:  ldloc.2
      IL_0025:  stloc.3
      IL_0026:  ldc.i4.1
      IL_0027:  nop
      IL_0028:  br.s       IL_002c

      IL_002a:  ldc.i4.0
      IL_002b:  nop
      IL_002c:  endfilter
    }  
    {  
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_5
      IL_0032:  isinst     [runtime]System.Exception
      IL_0037:  dup
      IL_0038:  brtrue.s   IL_0042

      IL_003a:  pop
      IL_003b:  ldloc.s    V_5
      IL_003d:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0042:  stloc.s    V_4
      IL_0044:  ldloc.s    V_4
      IL_0046:  isinst     [runtime]System.ArgumentException
      IL_004b:  stloc.2
      IL_004c:  ldloc.2
      IL_004d:  brfalse.s  IL_005e

      IL_004f:  ldloc.2
      IL_0050:  stloc.3
      IL_0051:  ldloc.3
      IL_0052:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_0057:  call       void [runtime]System.Console::WriteLine(string)
      IL_005c:  leave.s    IL_0069

      IL_005e:  rethrow
      IL_0060:  ldnull
      IL_0061:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_0066:  pop
      IL_0067:  leave.s    IL_0069

    }  
    IL_0069:  ret
  } 

  .method public static void  test4() cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException V_2,
             string V_3,
             class [runtime]System.Exception V_4,
             object V_5)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave      IL_007c

    }  
    filter
    {
      IL_000b:  stloc.1
      IL_000c:  ldloc.1
      IL_000d:  isinst     [runtime]System.Exception
      IL_0012:  dup
      IL_0013:  brtrue.s   IL_001c

      IL_0015:  pop
      IL_0016:  ldloc.1
      IL_0017:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_001c:  stloc.0
      IL_001d:  ldloc.0
      IL_001e:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0023:  stloc.2
      IL_0024:  ldloc.2
      IL_0025:  brfalse.s  IL_0037

      IL_0027:  ldloc.0
      IL_0028:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_002d:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0032:  stloc.3
      IL_0033:  ldc.i4.1
      IL_0034:  nop
      IL_0035:  br.s       IL_0039

      IL_0037:  ldc.i4.0
      IL_0038:  nop
      IL_0039:  endfilter
    }  
    {  
      IL_003b:  stloc.s    V_5
      IL_003d:  ldloc.s    V_5
      IL_003f:  isinst     [runtime]System.Exception
      IL_0044:  dup
      IL_0045:  brtrue.s   IL_004f

      IL_0047:  pop
      IL_0048:  ldloc.s    V_5
      IL_004a:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_004f:  stloc.s    V_4
      IL_0051:  ldloc.s    V_4
      IL_0053:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0058:  stloc.2
      IL_0059:  ldloc.2
      IL_005a:  brfalse.s  IL_0071

      IL_005c:  ldloc.s    V_4
      IL_005e:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0063:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
      IL_0068:  stloc.3
      IL_0069:  ldloc.3
      IL_006a:  call       void [runtime]System.Console::WriteLine(string)
      IL_006f:  leave.s    IL_007c

      IL_0071:  rethrow
      IL_0073:  ldnull
      IL_0074:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_0079:  pop
      IL_007a:  leave.s    IL_007c

    }  
    IL_007c:  ret
  } 

  .method public static void  test5() cil managed
  {
    
    .maxstack  4
    .locals init (class [runtime]System.Exception V_0,
             object V_1,
             class [runtime]System.ArgumentException V_2)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       void [runtime]System.Console::WriteLine()
      IL_0006:  leave.s    IL_005d

    }  
    catch [runtime]System.Object 
    {
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  isinst     [runtime]System.Exception
      IL_000f:  dup
      IL_0010:  brtrue.s   IL_0019

      IL_0012:  pop
      IL_0013:  ldloc.1
      IL_0014:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0019:  stloc.0
      IL_001a:  ldloc.0
      IL_001b:  isinst     [runtime]System.ArgumentException
      IL_0020:  brtrue.s   IL_002c

      IL_0022:  ldloc.0
      IL_0023:  isinst     [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0028:  brfalse.s  IL_0052

      IL_002a:  br.s       IL_0040

      IL_002c:  ldloc.0
      IL_002d:  unbox.any  [runtime]System.ArgumentException
      IL_0032:  stloc.2
      IL_0033:  ldloc.2
      IL_0034:  callvirt   instance string [runtime]System.Exception::get_Message()
      IL_0039:  call       void [runtime]System.Console::WriteLine(string)
      IL_003e:  leave.s    IL_005d

      IL_0040:  ldloc.0
      IL_0041:  castclass  [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException
      IL_0046:  call       instance string [FSharp.Core]Microsoft.FSharp.Core.MatchFailureException::get_Data0()
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





