




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
             class [runtime]System.Exception V_6,
             class [runtime]System.Exception V_7,
             object V_8,
             class [runtime]System.Exception V_9,
             class [runtime]System.Type V_10,
             class [runtime]System.Type V_11,
             class [runtime]System.Exception V_12)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  div
      IL_0003:  stloc.0
      IL_0004:  leave      IL_009a

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
      IL_001c:  stloc.3
      IL_001d:  ldloc.3
      IL_001e:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_0023:  stloc.s    V_4
      IL_0025:  ldtoken    [runtime]System.OperationCanceledException
      IL_002a:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_002f:  stloc.s    V_5
      IL_0031:  ldloc.s    V_4
      IL_0033:  ldloc.s    V_5
      IL_0035:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                               !!0)
      IL_003a:  ldc.i4.0
      IL_003b:  ceq
      IL_003d:  brfalse.s  IL_0045

      IL_003f:  ldloc.1
      IL_0040:  stloc.s    V_6
      IL_0042:  ldc.i4.1
      IL_0043:  br.s       IL_0046

      IL_0045:  ldc.i4.0
      IL_0046:  endfilter
    }  
    {  
      IL_0048:  stloc.s    V_8
      IL_004a:  ldloc.s    V_8
      IL_004c:  isinst     [runtime]System.Exception
      IL_0051:  dup
      IL_0052:  brtrue.s   IL_005c

      IL_0054:  pop
      IL_0055:  ldloc.s    V_8
      IL_0057:  newobj     instance void [runtime]System.Runtime.CompilerServices.RuntimeWrappedException::.ctor(object)
      IL_005c:  stloc.s    V_7
      IL_005e:  ldloc.s    V_7
      IL_0060:  stloc.s    V_9
      IL_0062:  ldloc.s    V_9
      IL_0064:  callvirt   instance class [runtime]System.Type [runtime]System.Exception::GetType()
      IL_0069:  stloc.s    V_10
      IL_006b:  ldtoken    [runtime]System.OperationCanceledException
      IL_0070:  call       class [netstandard]System.Type [netstandard]System.Type::GetTypeFromHandle(valuetype [netstandard]System.RuntimeTypeHandle)
      IL_0075:  stloc.s    V_11
      IL_0077:  ldloc.s    V_10
      IL_0079:  ldloc.s    V_11
      IL_007b:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<class [runtime]System.Type>(!!0,
                                                                                                                                                               !!0)
      IL_0080:  ldc.i4.0
      IL_0081:  ceq
      IL_0083:  brfalse.s  IL_008f

      IL_0085:  ldloc.s    V_7
      IL_0087:  stloc.s    V_12
      IL_0089:  ldarg.0
      IL_008a:  ldarg.1
      IL_008b:  add
      IL_008c:  stloc.0
      IL_008d:  leave.s    IL_009a

      IL_008f:  rethrow
      IL_0091:  ldnull
      IL_0092:  unbox.any  [runtime]System.Int32
      IL_0097:  stloc.0
      IL_0098:  leave.s    IL_009a

    }  
    IL_009a:  ldloc.0
    IL_009b:  ret
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





