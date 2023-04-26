




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
  .class sequential ansi serializable sealed nested public S
         extends [runtime]System.ValueType
  {
    .pack 0
    .size 1
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname static int32 
            op_Addition<a>(!!a _arg1,
                           valuetype assembly/S _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public specialname static int32 
            op_Multiply<a>(!!a _arg3,
                           valuetype assembly/S _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.1
      IL_0001:  ret
    } 

    .method public specialname static int32 
            op_Addition<a>(valuetype assembly/S _arg5,
                           !!a _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.2
      IL_0001:  ret
    } 

    .method public specialname static int32 
            op_Multiply<a>(valuetype assembly/S _arg7,
                           !!a _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.3
      IL_0001:  ret
    } 

  } 

  .method public static int32  testmethod() cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  nop
    IL_0003:  nop
    IL_0004:  nop
    IL_0005:  nop
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ldc.i4.s   12
    IL_000a:  ret
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






