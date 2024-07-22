




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly assembly
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed MyTestModule
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .method public static string  nonNullableInputOutputFunc(string x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static string  nullableStringInputOutputFunc(string x) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static int32  nonNullableIntFunc(int32 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static valuetype [runtime]System.Nullable`1<int32> nullableIntFunc(valuetype [runtime]System.Nullable`1<int32> x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static valuetype [runtime]System.ValueTuple`6<string,string,int32,int32,int32,int32> genericValueTypeTest(valuetype [runtime]System.ValueTuple`6<string,string,int32,int32,int32,int32> x) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 00 01 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 00 01 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static class [runtime]System.Tuple`6<string,string,int32,int32,int32,int32> 
          genericRefTypeTest(string x_0,
                             string x_1,
                             int32 x_2,
                             int32 x_3,
                             int32 x_4,
                             int32 x_5) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 01 01 02 00 00 ) 
    .param [2]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  ldarg.s    x_4
    IL_0006:  ldarg.s    x_5
    IL_0008:  newobj     instance void class [runtime]System.Tuple`6<string,string,int32,int32,int32,int32>::.ctor(!0,
                                                                                                                          !1,
                                                                                                                          !2,
                                                                                                                          !3,
                                                                                                                          !4,
                                                                                                                          !5)
    IL_000d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>> nestedGenericsTest(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>> x) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } 

  .method public static int32  multiArgumentTest(string x,
                                                 string y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    .param [2]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.s   42
    IL_0002:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 





