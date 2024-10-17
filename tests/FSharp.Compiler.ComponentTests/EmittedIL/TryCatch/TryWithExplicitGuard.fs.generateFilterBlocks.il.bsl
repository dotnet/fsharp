




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
             class [runtime]System.Exception V_5,
             class [runtime]System.Exception V_6,
             class [runtime]System.Exception V_7,
             class [runtime]System.Type V_8,
             class [runtime]System.Type V_9,
             class [runtime]System.Exception V_10)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave      IL_007d

    }  
    filter
    {
      IL_0009:  castclass  [runtime]System.Exception
      IL_000e:  stloc.1
      IL_000f:  ldloc.1
      IL_0010:  stloc.2
      IL_0011:  ldloc.2
      IL_0012:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_0017:  stloc.3
      IL_0018:  ldtoken    [runtime]System.OperationCanceledException
      IL_001d:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_0022:  stloc.s    V_4
      IL_0024:  ldloc.3
      IL_0025:  ldloc.s    V_4
      IL_0027:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                         !!0)
      IL_002c:  ldc.i4.0
      IL_002d:  ceq
      IL_002f:  brfalse.s  IL_0037

      IL_0031:  ldloc.1
      IL_0032:  stloc.s    V_5
      IL_0034:  ldc.i4.1
      IL_0035:  br.s       IL_0038

      IL_0037:  ldc.i4.0
      IL_0038:  endfilter
    }  
    {  
      IL_003a:  castclass  [runtime]System.Exception
      IL_003f:  stloc.s    V_6
      IL_0041:  ldloc.s    V_6
      IL_0043:  stloc.s    V_7
      IL_0045:  ldloc.s    V_7
      IL_0047:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_004c:  stloc.s    V_8
      IL_004e:  ldtoken    [runtime]System.OperationCanceledException
      IL_0053:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_0058:  stloc.s    V_9
      IL_005a:  ldloc.s    V_8
      IL_005c:  ldloc.s    V_9
      IL_005e:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                         !!0)
      IL_0063:  ldc.i4.0
      IL_0064:  ceq
      IL_0066:  brfalse.s  IL_0072

      IL_0068:  ldloc.s    V_6
      IL_006a:  stloc.s    V_10
      IL_006c:  ldarg.0
      IL_006d:  ldarg.1
      IL_006e:  add
      IL_006f:  stloc.0
      IL_0070:  leave.s    IL_007d

      IL_0072:  rethrow
      IL_0074:  ldnull
      IL_0075:  unbox.any  [runtime]System.Int32
      IL_007a:  stloc.0
      IL_007b:  leave.s    IL_007d

    }  
    IL_007d:  ldloc.0
    IL_007e:  ret
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






