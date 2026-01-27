




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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
  .method public static int32  TestFunction1() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldstr      "Hello"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  pop
    IL_0010:  ldstr      "World"
    IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001f:  pop
    IL_0020:  ldc.i4.3
    IL_0021:  ldc.i4.4
    IL_0022:  add
    IL_0023:  ret
  } 

  .method public static void  TestFunction3c() cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             string V_1,
             class [runtime]System.Exception V_2,
             object V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> V_4,
             string V_5,
             string V_6)
    .try
    {
      IL_0000:  nop
      IL_0001:  call       int32 assembly::TestFunction1()
      IL_0006:  stloc.0
      IL_0007:  ldstr      "hello"
      IL_000c:  stloc.1
      IL_000d:  ldloc.1
      IL_000e:  call       class [runtime]System.Exception [FSharp.Core]Microsoft.FSharp.Core.Operators::Failure(string)
      IL_0013:  throw

    }  
    catch [runtime]System.Object 
    {
      IL_0014:  stloc.3
      IL_0015:  ldloc.3
      IL_0016:  isinst     [runtime]System.Exception
      IL_001b:  dup
      IL_001c:  brtrue.s   IL_0025

      IL_001e:  pop
      IL_001f:  ldloc.3
      IL_0020:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_0025:  stloc.2
      IL_0026:  ldloc.2
      IL_0027:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> [FSharp.Core]Microsoft.FSharp.Core.Operators::FailurePattern(class [runtime]System.Exception)
      IL_002c:  stloc.s    V_4
      IL_002e:  ldloc.s    V_4
      IL_0030:  brfalse.s  IL_0064

      IL_0032:  ldloc.s    V_4
      IL_0034:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_0039:  stloc.s    V_5
      IL_003b:  ldloc.s    V_5
      IL_003d:  ldstr      "hello"
      IL_0042:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0047:  brfalse.s  IL_0064

      IL_0049:  ldloc.s    V_4
      IL_004b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>::get_Value()
      IL_0050:  stloc.s    V_6
      IL_0052:  ldstr      "World"
      IL_0057:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
      IL_005c:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0061:  pop
      IL_0062:  leave.s    IL_006f

      IL_0064:  rethrow
      IL_0066:  ldnull
      IL_0067:  unbox.any  [FSharp.Core]Microsoft.FSharp.Core.Unit
      IL_006c:  pop
      IL_006d:  leave.s    IL_006f

    }  
    IL_006f:  ret
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





