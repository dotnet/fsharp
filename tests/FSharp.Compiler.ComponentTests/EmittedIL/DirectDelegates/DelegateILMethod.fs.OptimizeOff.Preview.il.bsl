




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
  .class abstract auto autochar serializable sealed nested assembly beforefieldinit specialname ilStaticEta@12
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .method assembly static int32  Invoke(int32 a,
                                          int32 b) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  call       int32 [runtime]System.Math::Max(int32,
                                                            int32)
      IL_0007:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname ilInstanceEta@16
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Text.StringBuilder sb
    .method public specialname rtspecialname instance void  .ctor(class [runtime]System.Text.StringBuilder sb) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Text.StringBuilder assembly/ilInstanceEta@16::sb
      IL_0007:  ldarg.0
      IL_0008:  call       instance void [runtime]System.Object::.ctor()
      IL_000d:  ret
    } 

    .method assembly hidebysig instance class [runtime]System.Text.StringBuilder Invoke(string s) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Text.StringBuilder assembly/ilInstanceEta@16::sb
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance class [runtime]System.Text.StringBuilder [runtime]System.Text.StringBuilder::Append(string)
      IL_000c:  ret
    } 

  } 

  .method public static class [runtime]System.Func`3<int32,int32,int32> ilStaticEta() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      int32 assembly/ilStaticEta@12::Invoke(int32,
                                                                       int32)
    IL_0007:  newobj     instance void class [runtime]System.Func`3<int32,int32,int32>::.ctor(object,
                                                                                               native int)
    IL_000c:  ret
  } 

  .method public static class [runtime]System.Func`2<string,class [runtime]System.Text.StringBuilder> ilInstanceEta(class [runtime]System.Text.StringBuilder sb) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/ilInstanceEta@16::.ctor(class [runtime]System.Text.StringBuilder)
    IL_0006:  ldftn      instance class [runtime]System.Text.StringBuilder assembly/ilInstanceEta@16::Invoke(string)
    IL_000c:  newobj     instance void class [runtime]System.Func`2<string,class [runtime]System.Text.StringBuilder>::.ctor(object,
                                                                                                                              native int)
    IL_0011:  ret
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






