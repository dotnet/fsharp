




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
             class [runtime]System.Exception V_2,
             class [runtime]System.Type V_3,
             class [runtime]System.Type V_4,
             class [runtime]System.Exception V_5)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave.s    IL_0042

    }  
    catch [runtime]System.Object 
    {
      IL_0006:  castclass  [runtime]System.Exception
      IL_000b:  stloc.1
      IL_000c:  ldloc.1
      IL_000d:  stloc.2
      IL_000e:  ldloc.2
      IL_000f:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_0014:  stloc.3
      IL_0015:  ldtoken    [runtime]System.OperationCanceledException
      IL_001a:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_001f:  stloc.s    V_4
      IL_0021:  ldloc.3
      IL_0022:  ldloc.s    V_4
      IL_0024:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                         !!0)
      IL_0029:  ldc.i4.0
      IL_002a:  ceq
      IL_002c:  brfalse.s  IL_0037

      IL_002e:  ldloc.1
      IL_002f:  stloc.s    V_5
      IL_0031:  ldarg.0
      IL_0032:  ldarg.1
      IL_0033:  add
      IL_0034:  stloc.0
      IL_0035:  leave.s    IL_0042

      IL_0037:  rethrow
      IL_0039:  ldnull
      IL_003a:  unbox.any  [runtime]System.Int32
      IL_003f:  stloc.0
      IL_0040:  leave.s    IL_0042

    }  
    IL_0042:  ldloc.0
    IL_0043:  ret
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






