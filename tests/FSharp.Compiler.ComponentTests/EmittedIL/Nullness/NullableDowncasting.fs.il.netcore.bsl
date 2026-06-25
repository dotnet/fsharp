




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





.class public abstract auto ansi sealed TestModule
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.NullableContextAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
  .method public static string  objToString(object o) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<string>(object)
    IL_0008:  ret
  } 

  .method public static string  objnullToNullableString(object o) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  [runtime]System.String
    IL_0006:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string> objnullToOption(object o) cil managed
  {
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8[]) = ( 01 00 02 00 00 00 02 01 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>
    IL_0006:  ret
  } 

  .method public static int32  objNullTOInt(object o) cil managed
  {
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  [runtime]System.Int32
    IL_0006:  ret
  } 

  .method public static !!a  castToA<a>(object o) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::UnboxGeneric<!!0>(object)
    IL_0008:  ret
  } 

  .method public static bool  isObjnullAString(object o) cil managed
  {
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  3
    .locals init (object V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst     [runtime]System.String
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  ret
  } 

  .method public static bool  isObjNullOption(object o) cil managed
  {
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<string>>(object)
    IL_0008:  ret
  } 

  .method public static bool  isOfType<a>(object o) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 00 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::TypeTestGeneric<!!0>(object)
    IL_0008:  ret
  } 

  .method public static !!b  castToNullableB<class b>(object a) cil managed
  {
    .param type b 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  !!b
    IL_0006:  ret
  } 

  .method public static !!a  downcastIplicitGeneric<class a>(object o) cil managed
  {
    .param type a 
      .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 01 00 00 ) 
    .param [0]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.NullableAttribute::.ctor(uint8) = ( 01 00 02 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  !!a
    IL_0006:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$TestModule
       extends [runtime]System.Object
{
} 






