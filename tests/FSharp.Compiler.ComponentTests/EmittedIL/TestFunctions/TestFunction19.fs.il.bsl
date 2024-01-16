




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
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly int32 y
    .field assembly int32 x
    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 int32 y) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 assembly/C::x
      IL_000f:  ldarg.0
      IL_0010:  ldarg.2
      IL_0011:  stfld      int32 assembly/C::y
      IL_0016:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_X() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/C::x
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_Y() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/C::y
      IL_0006:  ret
    } 

    .property instance int32 X()
    {
      .get instance int32 assembly/C::get_X()
    } 
    .property instance int32 Y()
    {
      .get instance int32 assembly/C::get_Y()
    } 
  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'assembly@11-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo2
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> clo2) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/'assembly@11-1'::clo2
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit 
            Invoke(class assembly/C arg20) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/'assembly@11-1'::clo2
      IL_0006:  ldarg.1
      IL_0007:  tail.
      IL_0009:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit assembly@11
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo1
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> clo1) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> assembly/assembly@11::clo1
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
            Invoke(class assembly/C arg10) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> assembly/assembly@11::clo1
      IL_0006:  ldarg.1
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::Invoke(!0)
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  newobj     instance void assembly/'assembly@11-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
      IL_0013:  ret
    } 

  } 

  .method public static void  assembly(int32 inp) cil managed
  {
    
    .maxstack  5
    .locals init (class assembly/C V_0,
             class assembly/C V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>> V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  newobj     instance void assembly/C::.ctor(int32,
                                                               int32)
    IL_0007:  stloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldarg.0
    IL_000a:  newobj     instance void assembly/C::.ctor(int32,
                                                               int32)
    IL_000f:  stloc.1
    IL_0010:  ldstr      "c1 = %A, c2 = %A"
    IL_0015:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Tuple`2<class assembly/C,class assembly/C>>::.ctor(string)
    IL_001a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_001f:  stloc.2
    IL_0020:  ldloc.2
    IL_0021:  newobj     instance void assembly/assembly@11::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>)
    IL_0026:  ldloc.0
    IL_0027:  ldloc.1
    IL_0028:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class assembly/C,class assembly/C>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                             !0,
                                                                                                                                                                                             !1)
    IL_002d:  pop
    IL_002e:  ret
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






