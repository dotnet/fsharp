




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
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





.class public abstract auto ansi sealed ExplicitGuardCase
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  addWithExplicitGuardInWithClause(int32 a,
                                                                int32 b) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (int32 V_0,
             class [runtime]System.Exception V_1,
             object V_2,
             class [runtime]System.Exception V_3,
             class [runtime]System.Type V_4,
             class [runtime]System.Type V_5,
             class [runtime]System.Exception V_6)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave.s    IL_0050

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
      IL_0019:  stloc.3
      IL_001a:  ldloc.3
      IL_001b:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_0020:  stloc.s    V_4
      IL_0022:  ldtoken    [runtime]System.OperationCanceledException
      IL_0027:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_002c:  stloc.s    V_5
      IL_002e:  ldloc.s    V_4
      IL_0030:  ldloc.s    V_5
      IL_0032:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                               !!0)
      IL_0037:  ldc.i4.0
      IL_0038:  ceq
      IL_003a:  brfalse.s  IL_0045

      IL_003c:  ldloc.1
      IL_003d:  stloc.s    V_6
      IL_003f:  ldarg.0
      IL_0040:  ldarg.1
      IL_0041:  add
      IL_0042:  stloc.0
      IL_0043:  leave.s    IL_0050

      IL_0045:  rethrow
      IL_0047:  ldnull
      IL_0048:  unbox.any  [runtime]System.Int32
      IL_004d:  stloc.0
      IL_004e:  leave.s    IL_0050

    }  
    IL_0050:  ldloc.0
    IL_0051:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$ExplicitGuardCase
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 





