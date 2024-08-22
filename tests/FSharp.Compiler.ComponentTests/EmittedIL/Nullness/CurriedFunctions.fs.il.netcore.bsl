




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
  .class auto ansi serializable sealed nested assembly beforefieldinit partiallyApplied@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,int32,class [runtime]System.Tuple`3<string,string,int32>>
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .field public string propperString
    .method assembly specialname rtspecialname instance void  .ctor(string propperString) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`3<string,int32,class [runtime]System.Tuple`3<string,string,int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      string MyTestModule/partiallyApplied@8::propperString
      IL_000d:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`3<string,string,int32> 
            Invoke(string b,
                   int32 c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string MyTestModule/partiallyApplied@8::propperString
      IL_0006:  ldarg.1
      IL_0007:  ldarg.2
      IL_0008:  newobj     instance void class [runtime]System.Tuple`3<string,string,int32>::.ctor(!0,
                                                                                                          !1,
                                                                                                          !2)
      IL_000d:  ret
    } 

  } 

  .method public static class [runtime]System.Tuple`3<string,string,int32> 
          curried3Func(string a,
                       string b,
                       int32 c) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 03 00 00 00 01 02 01 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  newobj     instance void class [runtime]System.Tuple`3<string,string,int32>::.ctor(!0,
                                                                                                        !1,
                                                                                                        !2)
    IL_0008:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Tuple`3<string,string,int32>>> partiallyApplied(string propperString) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 06 00 00 00 01 01 01 01 02 01 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void MyTestModule/partiallyApplied@8::.ctor(string)
    IL_0006:  ret
  } 

  .method public static !!b  secondOutOfTriple<a,class b,c,d,e>(!!a a,
                                                                !!b b,
                                                                !!c c,
                                                                !!d d,
                                                                !!e e) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 03 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param type b 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param type c 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param type d 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param type e 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [2]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 





