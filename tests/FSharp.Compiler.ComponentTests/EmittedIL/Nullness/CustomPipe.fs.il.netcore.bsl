




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
  .method public static !!b  myassemblyFunc<a,b>(!!a arg,
                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b> func) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param type b 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  tail.
    IL_0004:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!b>::Invoke(!0)
    IL_0009:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$MyTestModule
       extends [runtime]System.Object
{
} 






