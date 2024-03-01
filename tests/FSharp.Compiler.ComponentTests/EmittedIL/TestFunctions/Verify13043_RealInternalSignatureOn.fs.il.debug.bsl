




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





.class public abstract auto ansi sealed Verify13043
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested assembly beforefieldinit f@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/f@8::condition
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> l) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_0012

      IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0011:  ret

      IL_0012:  ldloc.0
      IL_0013:  stloc.1
      IL_0014:  ldloc.1
      IL_0015:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_001a:  stloc.2
      IL_001b:  ldloc.1
      IL_001c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
      IL_0021:  stloc.3
      IL_0022:  nop
      IL_0023:  ldarg.0
      IL_0024:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/f@8::condition
      IL_0029:  ldloc.3
      IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_002f:  brfalse.s  IL_0036

      IL_0031:  ldloc.2
      IL_0032:  starg.s    l
      IL_0034:  br.s       IL_0000

      IL_0036:  ldloc.3
      IL_0037:  ldloc.2
      IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_003d:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f@27-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/'f@27-1'::condition
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Invoke(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_0,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_1,
               class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> V_2,
               int32 V_3)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_0008:  brfalse.s  IL_000c

      IL_000a:  br.s       IL_0012

      IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
      IL_0011:  ret

      IL_0012:  ldloc.0
      IL_0013:  stloc.1
      IL_0014:  ldloc.1
      IL_0015:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
      IL_001a:  stloc.2
      IL_001b:  ldloc.1
      IL_001c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
      IL_0021:  stloc.3
      IL_0022:  nop
      IL_0023:  ldarg.0
      IL_0024:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> Verify13043/'f@27-1'::condition
      IL_0029:  ldloc.3
      IL_002a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::Invoke(!0)
      IL_002f:  brfalse.s  IL_0036

      IL_0031:  ldloc.2
      IL_0032:  starg.s    _arg1
      IL_0034:  br.s       IL_0000

      IL_0036:  ldloc.3
      IL_0037:  ldloc.2
      IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
      IL_003d:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit matchResult@38
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Verify13043/matchResult@38 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       bool Verify13043::condition(int32)
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Verify13043/matchResult@38::.ctor()
      IL_0005:  stsfld     class Verify13043/matchResult@38 Verify13043/matchResult@38::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit functionResult@43
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class Verify13043/functionResult@43 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       bool Verify13043::condition(int32)
      IL_0006:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void Verify13043/functionResult@43::.ctor()
      IL_0005:  stsfld     class Verify13043/functionResult@43 Verify13043/functionResult@43::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list@3
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> matchResult@38
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> functionResult@43
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_list() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::list@3
    IL_0005:  ret
  } 

  .method public static bool  condition(int32 n) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  clt
    IL_0004:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          dropWhileWithMatch(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition,
                             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>> V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Verify13043/f@8::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Invoke(!0)
    IL_0010:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          dropWhileWithFunction(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool> condition,
                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> list) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>> V_0)
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void Verify13043/'f@27-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  tail.
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::Invoke(!0)
    IL_0010:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_matchResult() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::matchResult@38
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_functionResult() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::functionResult@43
    IL_0005:  ret
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Verify13043::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Verify13043::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  6
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  ldc.i4.3
    IL_0003:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0008:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_000d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0012:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0017:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::list@3
    IL_001c:  ldsfld     class Verify13043/matchResult@38 Verify13043/matchResult@38::@_instance
    IL_0021:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
    IL_0026:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::dropWhileWithMatch(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,
                                                                                                                             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_002b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::matchResult@38
    IL_0030:  ldstr      "Match: %A"
    IL_0035:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_003a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_003f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_matchResult()
    IL_0044:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0049:  pop
    IL_004a:  ldsfld     class Verify13043/functionResult@43 Verify13043/functionResult@43::@_instance
    IL_004f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
    IL_0054:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::dropWhileWithFunction(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>,
                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>)
    IL_0059:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::functionResult@43
    IL_005e:  ldstr      "Function: %A"
    IL_0063:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>>::.ctor(string)
    IL_0068:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_006d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_functionResult()
    IL_0072:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0077:  pop
    IL_0078:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          list()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_list()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          matchResult()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_matchResult()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          functionResult()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> Verify13043::get_functionResult()
  } 
} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Verify13043
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  call       void Verify13043::staticInitialization@()
    IL_0005:  ret
  } 

} 






