




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
  .class auto ansi serializable sealed nested assembly beforefieldinit f1@1
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/f1@1 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f1@1::.ctor()
      IL_0005:  stsfld     class assembly/f1@1 assembly/f1@1::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f2@2<a>
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!a>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!a> f
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!a> f) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!a>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!0> class assembly/f2@2<!a>::f
      IL_000d:  ret
    } 

    .method public strict virtual instance !a Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!0> class assembly/f2@2<!a>::f
      IL_0006:  ldarg.1
      IL_0007:  tail.
      IL_0009:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!a>::Invoke(!0)
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f3@3
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f3@3::f
      IL_000d:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f3@3::f
      IL_0006:  ldnull
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000c:  pop
      IL_000d:  ldarg.1
      IL_000e:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f4@4
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f4@4::f
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f4@4::g
      IL_0014:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f4@4::f
      IL_0006:  ldnull
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000c:  pop
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f4@4::g
      IL_0013:  ldnull
      IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0019:  pop
      IL_001a:  ldarg.1
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f5@5
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
  {
    .field static assembly initonly class assembly/f5@5 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance int32 Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f5@5::.ctor()
      IL_0005:  stsfld     class assembly/f5@5 assembly/f5@5::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f6@6
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f6@6::f
      IL_000d:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f6@6::f
      IL_0006:  ldnull
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000c:  pop
      IL_000d:  ldarg.1
      IL_000e:  tail.
      IL_0010:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_0015:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f7@7
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f7@7::f
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f7@7::g
      IL_0014:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> Invoke(int32 x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f7@7::f
      IL_0006:  ldnull
      IL_0007:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_000c:  pop
      IL_000d:  ldarg.0
      IL_000e:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> assembly/f7@7::g
      IL_0013:  ldnull
      IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0019:  pop
      IL_001a:  ldarg.1
      IL_001b:  tail.
      IL_001d:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_0022:  ret
    } 

  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/f1@1 assembly/f1@1::@_instance
    IL_0005:  ldarg.0
    IL_0006:  tail.
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!a> 
          f2<a>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!a> f,
                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class assembly/f2@2<!!a>::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!0>)
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,!!0>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f3(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/f3@3::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f4(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void assembly/f4@4::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0007:  ldarg.2
    IL_0008:  tail.
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000f:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class assembly/f5@5 assembly/f5@5::@_instance
    IL_0005:  ldarg.0
    IL_0006:  tail.
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f6(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void assembly/f6@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f7(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void assembly/f7@7::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0007:  ldarg.2
    IL_0008:  tail.
    IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!1> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Map<int32,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_000f:  ret
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






