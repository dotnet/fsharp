




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
  .class auto ansi serializable sealed nested assembly beforefieldinit f33@47
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f33@47 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f33@47::.ctor()
      IL_0005:  stsfld     class assembly/f33@47 assembly/f33@47::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f33@47-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f33@47-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f33@47-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f33@47-1' assembly/'f33@47-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f34@48
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f34@48 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f34@48::.ctor()
      IL_0005:  stsfld     class assembly/f34@48 assembly/f34@48::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/'f34@48-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-2'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-2' assembly/'f34@48-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f34@48-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-1' assembly/'f34@48-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f34@48-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f34@48-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f34@48-3'::.ctor()
      IL_0005:  stsfld     class assembly/'f34@48-3' assembly/'f34@48-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit f35@49
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/f35@49 @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/f35@49::.ctor()
      IL_0005:  stsfld     class assembly/f35@49 assembly/f35@49::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>
  {
    .field static assembly initonly class assembly/'f35@49-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Invoke(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  tail.
      IL_0003:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Identity<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(!!0)
      IL_0008:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-2'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-2' assembly/'f35@49-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f35@49-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-1'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-1' assembly/'f35@49-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'f35@49-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>
  {
    .field static assembly initonly class assembly/'f35@49-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Core.Unit Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit x) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'f35@49-3'::.ctor()
      IL_0005:  stsfld     class assembly/'f35@49-3' assembly/'f35@49-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_002b

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldarg.0
    IL_0010:  ldnull
    IL_0011:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0016:  pop
    IL_0017:  stloc.s    V_5
    IL_0019:  ldloc.s    V_5
    IL_001b:  ldloc.3
    IL_001c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0021:  nop
    IL_0022:  ldloc.2
    IL_0023:  ldc.i4.1
    IL_0024:  add
    IL_0025:  stloc.2
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  conv.i8
    IL_0029:  add
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.s   10
    IL_002e:  conv.i8
    IL_002f:  blt.un.s   IL_0007

    IL_0031:  ldloca.s   V_0
    IL_0033:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0038:  ret
  } 

  .method public static int32[]  f00(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f,
                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  6
    .locals init (int32[] V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             native int V_4,
             int32[] V_5,
             native int V_6,
             int32[] V_7,
             native int V_8,
             int32[] V_9)
    IL_0000:  ldc.i4.s   10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr     [runtime]System.Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s       IL_0049

    IL_0011:  ldloc.2
    IL_0012:  stloc.3
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s    V_4
    IL_0018:  stloc.s    V_5
    IL_001a:  ldloc.s    V_5
    IL_001c:  ldloc.s    V_4
    IL_001e:  ldarg.0
    IL_001f:  ldnull
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  stloc.s    V_6
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldarg.1
    IL_002f:  ldnull
    IL_0030:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0035:  pop
    IL_0036:  stloc.s    V_8
    IL_0038:  stloc.s    V_9
    IL_003a:  ldloc.s    V_9
    IL_003c:  ldloc.s    V_8
    IL_003e:  ldloc.3
    IL_003f:  stelem.i4
    IL_0040:  ldloc.2
    IL_0041:  ldc.i4.1
    IL_0042:  add
    IL_0043:  stloc.2
    IL_0044:  ldloc.1
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.s   10
    IL_004c:  conv.i8
    IL_004d:  blt.un.s   IL_0011

    IL_004f:  ldloc.0
    IL_0050:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f) cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             class [runtime]System.IDisposable V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_0035

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldarg.0
      IL_001a:  ldnull
      IL_001b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
      IL_0020:  pop
      IL_0021:  ldloca.s   V_0
      IL_0023:  ldloc.3
      IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0029:  nop
      IL_002a:  ldloca.s   V_0
      IL_002c:  ldloc.3
      IL_002d:  ldc.i4.1
      IL_002e:  add
      IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_0034:  nop
      IL_0035:  ldloc.1
      IL_0036:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_003b:  brtrue.s   IL_0012

      IL_003d:  ldnull
      IL_003e:  stloc.2
      IL_003f:  leave.s    IL_0056

    }  
    finally
    {
      IL_0041:  ldloc.1
      IL_0042:  isinst     [runtime]System.IDisposable
      IL_0047:  stloc.s    V_4
      IL_0049:  ldloc.s    V_4
      IL_004b:  brfalse.s  IL_0055

      IL_004d:  ldloc.s    V_4
      IL_004f:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_0054:  endfinally
      IL_0055:  endfinally
    }  
    IL_0056:  ldloc.2
    IL_0057:  pop
    IL_0058:  ldloca.s   V_0
    IL_005a:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_005f:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0000() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001f

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldloc.3
    IL_0010:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0007

    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00000() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001f

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldloc.3
    IL_0010:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0007

    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f000000() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0023

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  stloc.s    V_5
    IL_0011:  ldloc.s    V_5
    IL_0013:  ldloc.3
    IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0019:  nop
    IL_001a:  ldloc.2
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.2
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  conv.i8
    IL_0021:  add
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.s   10
    IL_0026:  conv.i8
    IL_0027:  blt.un.s   IL_0007

    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0030:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0000000() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0023

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  stloc.s    V_5
    IL_0011:  ldloc.s    V_5
    IL_0013:  ldloc.3
    IL_0014:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0019:  nop
    IL_001a:  ldloc.2
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.2
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  conv.i8
    IL_0021:  add
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.s   10
    IL_0026:  conv.i8
    IL_0027:  blt.un.s   IL_0007

    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0030:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00000000() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0027

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  stloc.s    V_5
    IL_0011:  ldloc.s    V_5
    IL_0013:  stloc.s    V_6
    IL_0015:  ldloc.s    V_6
    IL_0017:  ldloc.3
    IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_001d:  nop
    IL_001e:  ldloc.2
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.2
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.s   10
    IL_002a:  conv.i8
    IL_002b:  blt.un.s   IL_0007

    IL_002d:  ldloca.s   V_0
    IL_002f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0034:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f000000000(int32 x, int32 y) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             int32 V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_7,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_8)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0037

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_5
    IL_000d:  ldloc.s    V_5
    IL_000f:  ldloc.3
    IL_0010:  ldarg.0
    IL_0011:  add
    IL_0012:  stloc.s    V_4
    IL_0014:  stloc.s    V_7
    IL_0016:  ldloc.s    V_7
    IL_0018:  ldloc.3
    IL_0019:  ldarg.1
    IL_001a:  add
    IL_001b:  stloc.s    V_6
    IL_001d:  stloc.s    V_8
    IL_001f:  ldloc.s    V_8
    IL_0021:  ldloc.3
    IL_0022:  ldloc.s    V_4
    IL_0024:  add
    IL_0025:  ldloc.s    V_6
    IL_0027:  add
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.2
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  conv.i8
    IL_003b:  blt.un.s   IL_0007

    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f0000000000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_0037

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldarg.0
    IL_0010:  ldnull
    IL_0011:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0016:  pop
    IL_0017:  stloc.s    V_5
    IL_0019:  ldloc.s    V_5
    IL_001b:  ldarg.1
    IL_001c:  ldnull
    IL_001d:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0022:  pop
    IL_0023:  stloc.s    V_6
    IL_0025:  ldloc.s    V_6
    IL_0027:  ldloc.3
    IL_0028:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.2
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.1
    IL_0034:  conv.i8
    IL_0035:  add
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  conv.i8
    IL_003b:  blt.un.s   IL_0007

    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0044:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f00000000000(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             class [runtime]System.Collections.Generic.IEnumerator`1<int32> V_1,
             class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6,
             class [runtime]System.IDisposable V_7)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000a:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br.s       IL_004c

      IL_0012:  ldloc.1
      IL_0013:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0018:  stloc.3
      IL_0019:  ldloca.s   V_0
      IL_001b:  stloc.s    V_4
      IL_001d:  ldloc.s    V_4
      IL_001f:  ldarg.0
      IL_0020:  ldnull
      IL_0021:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_002b:  nop
      IL_002c:  ldloca.s   V_0
      IL_002e:  stloc.s    V_5
      IL_0030:  ldloc.s    V_5
      IL_0032:  ldarg.1
      IL_0033:  ldnull
      IL_0034:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
      IL_0039:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_003e:  nop
      IL_003f:  ldloca.s   V_0
      IL_0041:  stloc.s    V_6
      IL_0043:  ldloc.s    V_6
      IL_0045:  ldloc.3
      IL_0046:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
      IL_004b:  nop
      IL_004c:  ldloc.1
      IL_004d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0052:  brtrue.s   IL_0012

      IL_0054:  ldnull
      IL_0055:  stloc.2
      IL_0056:  leave.s    IL_006d

    }  
    finally
    {
      IL_0058:  ldloc.1
      IL_0059:  isinst     [runtime]System.IDisposable
      IL_005e:  stloc.s    V_7
      IL_0060:  ldloc.s    V_7
      IL_0062:  brfalse.s  IL_006c

      IL_0064:  ldloc.s    V_7
      IL_0066:  callvirt   instance void [runtime]System.IDisposable::Dispose()
      IL_006b:  endfinally
      IL_006c:  endfinally
    }  
    IL_006d:  ldloc.2
    IL_006e:  pop
    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0076:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f1() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001f

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldloc.3
    IL_0010:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0007

    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f2() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f3() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001f

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldloc.3
    IL_0010:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s   10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s   IL_0007

    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f4() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s       IL_001f

    IL_0007:  ldloc.2
    IL_0008:  stloc.3
    IL_0009:  ldloca.s   V_0
    IL_000b:  stloc.s    V_4
    IL_000d:  ldloc.s    V_4
    IL_000f:  ldloc.3
    IL_0010:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.2
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.5
    IL_0021:  conv.i8
    IL_0022:  blt.un.s   IL_0007

    IL_0024:  ldloca.s   V_0
    IL_0026:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002b:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f5() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f6() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0005:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f7() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_0020

    IL_0008:  ldloc.2
    IL_0009:  stloc.3
    IL_000a:  ldloca.s   V_0
    IL_000c:  stloc.s    V_4
    IL_000e:  ldloc.s    V_4
    IL_0010:  ldloc.3
    IL_0011:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0016:  nop
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.m1
    IL_0019:  add
    IL_001a:  stloc.2
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  conv.i8
    IL_001e:  add
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.s   10
    IL_0023:  conv.i8
    IL_0024:  blt.un.s   IL_0008

    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f8() cil managed
  {
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.2
    IL_0006:  br.s       IL_0021

    IL_0008:  ldloc.2
    IL_0009:  stloc.3
    IL_000a:  ldloca.s   V_0
    IL_000c:  stloc.s    V_4
    IL_000e:  ldloc.s    V_4
    IL_0010:  ldloc.3
    IL_0011:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0016:  nop
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.s   -2
    IL_001a:  add
    IL_001b:  stloc.2
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.5
    IL_0023:  conv.i8
    IL_0024:  blt.un.s   IL_0008

    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_002d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f9(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.2
    IL_0017:  ldarg.0
    IL_0018:  stloc.3
    IL_0019:  br.s       IL_0035

    IL_001b:  ldloc.3
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloca.s   V_1
    IL_0020:  stloc.s    V_5
    IL_0022:  ldloc.s    V_5
    IL_0024:  ldloc.s    V_4
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.3
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  stloc.3
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  add
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  ldloc.0
    IL_0037:  blt.un.s   IL_001b

    IL_0039:  ldloca.s   V_1
    IL_003b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0040:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f10(int32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.2
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.3
    IL_0017:  br.s       IL_0033

    IL_0019:  ldloc.3
    IL_001a:  stloc.s    V_4
    IL_001c:  ldloca.s   V_1
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.s    V_5
    IL_0022:  ldloc.s    V_4
    IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0029:  nop
    IL_002a:  ldloc.3
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stloc.3
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.2
    IL_0034:  ldloc.0
    IL_0035:  blt.un.s   IL_0019

    IL_0037:  ldloca.s   V_1
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f11(int32 start, int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.2
    IL_0015:  ldarg.0
    IL_0016:  stloc.3
    IL_0017:  br.s       IL_0033

    IL_0019:  ldloc.3
    IL_001a:  stloc.s    V_4
    IL_001c:  ldloca.s   V_1
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.s    V_5
    IL_0022:  ldloc.s    V_4
    IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0029:  nop
    IL_002a:  ldloc.3
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stloc.3
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.2
    IL_0034:  ldloc.0
    IL_0035:  blt.un.s   IL_0019

    IL_0037:  ldloca.s   V_1
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f12(int32 start) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldc.i4.s   10
    IL_0002:  ldarg.0
    IL_0003:  bge.s      IL_000a

    IL_0005:  ldc.i4.0
    IL_0006:  conv.i8
    IL_0007:  nop
    IL_0008:  br.s       IL_0013

    IL_000a:  ldc.i4.s   10
    IL_000c:  ldarg.0
    IL_000d:  sub
    IL_000e:  conv.i8
    IL_000f:  ldc.i4.1
    IL_0010:  conv.i8
    IL_0011:  add
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  stloc.2
    IL_0017:  ldarg.0
    IL_0018:  stloc.3
    IL_0019:  br.s       IL_0035

    IL_001b:  ldloc.3
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloca.s   V_1
    IL_0020:  stloc.s    V_5
    IL_0022:  ldloc.s    V_5
    IL_0024:  ldloc.s    V_4
    IL_0026:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002b:  nop
    IL_002c:  ldloc.3
    IL_002d:  ldc.i4.1
    IL_002e:  add
    IL_002f:  stloc.3
    IL_0030:  ldloc.2
    IL_0031:  ldc.i4.1
    IL_0032:  conv.i8
    IL_0033:  add
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  ldloc.0
    IL_0037:  blt.un.s   IL_001b

    IL_0039:  ldloca.s   V_1
    IL_003b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0040:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f13(int32 step) cil managed
  {
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.0
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldc.i4.1
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldc.i4.1
    IL_0022:  sub
    IL_0023:  ldarg.0
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldc.i4.1
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.0
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldc.i4.0
    IL_0046:  conv.i8
    IL_0047:  stloc.2
    IL_0048:  ldc.i4.1
    IL_0049:  stloc.3
    IL_004a:  br.s       IL_0066

    IL_004c:  ldloc.3
    IL_004d:  stloc.s    V_4
    IL_004f:  ldloca.s   V_1
    IL_0051:  stloc.s    V_5
    IL_0053:  ldloc.s    V_5
    IL_0055:  ldloc.s    V_4
    IL_0057:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_005c:  nop
    IL_005d:  ldloc.3
    IL_005e:  ldarg.0
    IL_005f:  add
    IL_0060:  stloc.3
    IL_0061:  ldloc.2
    IL_0062:  ldc.i4.1
    IL_0063:  conv.i8
    IL_0064:  add
    IL_0065:  stloc.2
    IL_0066:  ldloc.2
    IL_0067:  ldloc.0
    IL_0068:  blt.un.s   IL_004c

    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0071:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f14(int32 finish) cil managed
  {
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.2
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.3
    IL_0017:  br.s       IL_0033

    IL_0019:  ldloc.3
    IL_001a:  stloc.s    V_4
    IL_001c:  ldloca.s   V_1
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.s    V_5
    IL_0022:  ldloc.s    V_4
    IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0029:  nop
    IL_002a:  ldloc.3
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stloc.3
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.2
    IL_0034:  ldloc.0
    IL_0035:  blt.un.s   IL_0019

    IL_0037:  ldloca.s   V_1
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f15(int32 start, int32 step) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_0010

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.s   10
    IL_0007:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  br.s       IL_0011

    IL_0010:  nop
    IL_0011:  ldc.i4.0
    IL_0012:  ldarg.1
    IL_0013:  bge.s      IL_002c

    IL_0015:  ldc.i4.s   10
    IL_0017:  ldarg.0
    IL_0018:  bge.s      IL_001f

    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  nop
    IL_001d:  br.s       IL_0044

    IL_001f:  ldc.i4.s   10
    IL_0021:  ldarg.0
    IL_0022:  sub
    IL_0023:  ldarg.1
    IL_0024:  div.un
    IL_0025:  conv.i8
    IL_0026:  ldc.i4.1
    IL_0027:  conv.i8
    IL_0028:  add
    IL_0029:  nop
    IL_002a:  br.s       IL_0044

    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.s   10
    IL_002f:  bge.s      IL_0036

    IL_0031:  ldc.i4.0
    IL_0032:  conv.i8
    IL_0033:  nop
    IL_0034:  br.s       IL_0044

    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.s   10
    IL_0039:  sub
    IL_003a:  ldarg.1
    IL_003b:  not
    IL_003c:  ldc.i4.1
    IL_003d:  add
    IL_003e:  div.un
    IL_003f:  conv.i8
    IL_0040:  ldc.i4.1
    IL_0041:  conv.i8
    IL_0042:  add
    IL_0043:  nop
    IL_0044:  stloc.0
    IL_0045:  ldc.i4.0
    IL_0046:  conv.i8
    IL_0047:  stloc.2
    IL_0048:  ldarg.0
    IL_0049:  stloc.3
    IL_004a:  br.s       IL_0066

    IL_004c:  ldloc.3
    IL_004d:  stloc.s    V_4
    IL_004f:  ldloca.s   V_1
    IL_0051:  stloc.s    V_5
    IL_0053:  ldloc.s    V_5
    IL_0055:  ldloc.s    V_4
    IL_0057:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_005c:  nop
    IL_005d:  ldloc.3
    IL_005e:  ldarg.1
    IL_005f:  add
    IL_0060:  stloc.3
    IL_0061:  ldloc.2
    IL_0062:  ldc.i4.1
    IL_0063:  conv.i8
    IL_0064:  add
    IL_0065:  stloc.2
    IL_0066:  ldloc.2
    IL_0067:  ldloc.0
    IL_0068:  blt.un.s   IL_004c

    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0071:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f16(int32 start, int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.2
    IL_0015:  ldarg.0
    IL_0016:  stloc.3
    IL_0017:  br.s       IL_0033

    IL_0019:  ldloc.3
    IL_001a:  stloc.s    V_4
    IL_001c:  ldloca.s   V_1
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.s    V_5
    IL_0022:  ldloc.s    V_4
    IL_0024:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0029:  nop
    IL_002a:  ldloc.3
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stloc.3
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  conv.i8
    IL_0031:  add
    IL_0032:  stloc.2
    IL_0033:  ldloc.2
    IL_0034:  ldloc.0
    IL_0035:  blt.un.s   IL_0019

    IL_0037:  ldloca.s   V_1
    IL_0039:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003e:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f17(int32 step, int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldc.i4.1
    IL_0004:  ldarg.0
    IL_0005:  ldarg.1
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.0
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.1
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.1
    IL_001e:  ldc.i4.1
    IL_001f:  sub
    IL_0020:  ldarg.0
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldc.i4.1
    IL_002a:  ldarg.1
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldc.i4.1
    IL_0033:  ldarg.1
    IL_0034:  sub
    IL_0035:  ldarg.0
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.2
    IL_0043:  ldc.i4.1
    IL_0044:  stloc.3
    IL_0045:  br.s       IL_0061

    IL_0047:  ldloc.3
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_1
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_5
    IL_0050:  ldloc.s    V_4
    IL_0052:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0057:  nop
    IL_0058:  ldloc.3
    IL_0059:  ldarg.0
    IL_005a:  add
    IL_005b:  stloc.3
    IL_005c:  ldloc.2
    IL_005d:  ldc.i4.1
    IL_005e:  conv.i8
    IL_005f:  add
    IL_0060:  stloc.2
    IL_0061:  ldloc.2
    IL_0062:  ldloc.0
    IL_0063:  blt.un.s   IL_0047

    IL_0065:  ldloca.s   V_1
    IL_0067:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_006c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f18(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.2
    IL_0043:  ldarg.0
    IL_0044:  stloc.3
    IL_0045:  br.s       IL_0061

    IL_0047:  ldloc.3
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_1
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_5
    IL_0050:  ldloc.s    V_4
    IL_0052:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0057:  nop
    IL_0058:  ldloc.3
    IL_0059:  ldarg.1
    IL_005a:  add
    IL_005b:  stloc.3
    IL_005c:  ldloc.2
    IL_005d:  ldc.i4.1
    IL_005e:  conv.i8
    IL_005f:  add
    IL_0060:  stloc.2
    IL_0061:  ldloc.2
    IL_0062:  ldloc.0
    IL_0063:  blt.un.s   IL_0047

    IL_0065:  ldloca.s   V_1
    IL_0067:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_006c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f19(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  conv.i8
    IL_001e:  stloc.3
    IL_001f:  ldloc.0
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_0041

    IL_0024:  ldloc.s    V_4
    IL_0026:  stloc.s    V_5
    IL_0028:  ldloca.s   V_2
    IL_002a:  stloc.s    V_6
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldloc.s    V_5
    IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0035:  nop
    IL_0036:  ldloc.s    V_4
    IL_0038:  ldc.i4.1
    IL_0039:  add
    IL_003a:  stloc.s    V_4
    IL_003c:  ldloc.3
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  ldloc.1
    IL_0043:  blt.un.s   IL_0024

    IL_0045:  ldloca.s   V_2
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f20(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.s    V_4
    IL_0020:  br.s       IL_003f

    IL_0022:  ldloc.s    V_4
    IL_0024:  stloc.s    V_5
    IL_0026:  ldloca.s   V_2
    IL_0028:  stloc.s    V_6
    IL_002a:  ldloc.s    V_6
    IL_002c:  ldloc.s    V_5
    IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0033:  nop
    IL_0034:  ldloc.s    V_4
    IL_0036:  ldc.i4.1
    IL_0037:  add
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.3
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  ldloc.1
    IL_0041:  blt.un.s   IL_0022

    IL_0043:  ldloca.s   V_2
    IL_0045:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f21(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             uint64 V_2,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
             uint64 V_4,
             int32 V_5,
             int32 V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_7)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  bge.s      IL_0019

    IL_0014:  ldc.i4.0
    IL_0015:  conv.i8
    IL_0016:  nop
    IL_0017:  br.s       IL_0021

    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  sub
    IL_001c:  conv.i8
    IL_001d:  ldc.i4.1
    IL_001e:  conv.i8
    IL_001f:  add
    IL_0020:  nop
    IL_0021:  stloc.2
    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  stloc.s    V_4
    IL_0026:  ldloc.0
    IL_0027:  stloc.s    V_5
    IL_0029:  br.s       IL_004a

    IL_002b:  ldloc.s    V_5
    IL_002d:  stloc.s    V_6
    IL_002f:  ldloca.s   V_3
    IL_0031:  stloc.s    V_7
    IL_0033:  ldloc.s    V_7
    IL_0035:  ldloc.s    V_6
    IL_0037:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_003c:  nop
    IL_003d:  ldloc.s    V_5
    IL_003f:  ldc.i4.1
    IL_0040:  add
    IL_0041:  stloc.s    V_5
    IL_0043:  ldloc.s    V_4
    IL_0045:  ldc.i4.1
    IL_0046:  conv.i8
    IL_0047:  add
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloc.s    V_4
    IL_004c:  ldloc.2
    IL_004d:  blt.un.s   IL_002b

    IL_004f:  ldloca.s   V_3
    IL_0051:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0056:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f22(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   10
    IL_000a:  ldloc.0
    IL_000b:  bge.s      IL_0012

    IL_000d:  ldc.i4.0
    IL_000e:  conv.i8
    IL_000f:  nop
    IL_0010:  br.s       IL_001b

    IL_0012:  ldc.i4.s   10
    IL_0014:  ldloc.0
    IL_0015:  sub
    IL_0016:  conv.i8
    IL_0017:  ldc.i4.1
    IL_0018:  conv.i8
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  conv.i8
    IL_001e:  stloc.3
    IL_001f:  ldloc.0
    IL_0020:  stloc.s    V_4
    IL_0022:  br.s       IL_0041

    IL_0024:  ldloc.s    V_4
    IL_0026:  stloc.s    V_5
    IL_0028:  ldloca.s   V_2
    IL_002a:  stloc.s    V_6
    IL_002c:  ldloc.s    V_6
    IL_002e:  ldloc.s    V_5
    IL_0030:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0035:  nop
    IL_0036:  ldloc.s    V_4
    IL_0038:  ldc.i4.1
    IL_0039:  add
    IL_003a:  stloc.s    V_4
    IL_003c:  ldloc.3
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  ldloc.1
    IL_0043:  blt.un.s   IL_0024

    IL_0045:  ldloca.s   V_2
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f23(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brtrue.s   IL_0018

    IL_000b:  ldc.i4.1
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0014:  pop
    IL_0015:  nop
    IL_0016:  br.s       IL_0019

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  ldloc.0
    IL_001b:  bge.s      IL_0034

    IL_001d:  ldc.i4.s   10
    IL_001f:  ldc.i4.1
    IL_0020:  bge.s      IL_0027

    IL_0022:  ldc.i4.0
    IL_0023:  conv.i8
    IL_0024:  nop
    IL_0025:  br.s       IL_004c

    IL_0027:  ldc.i4.s   10
    IL_0029:  ldc.i4.1
    IL_002a:  sub
    IL_002b:  ldloc.0
    IL_002c:  div.un
    IL_002d:  conv.i8
    IL_002e:  ldc.i4.1
    IL_002f:  conv.i8
    IL_0030:  add
    IL_0031:  nop
    IL_0032:  br.s       IL_004c

    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.s   10
    IL_0037:  bge.s      IL_003e

    IL_0039:  ldc.i4.0
    IL_003a:  conv.i8
    IL_003b:  nop
    IL_003c:  br.s       IL_004c

    IL_003e:  ldc.i4.1
    IL_003f:  ldc.i4.s   10
    IL_0041:  sub
    IL_0042:  ldloc.0
    IL_0043:  not
    IL_0044:  ldc.i4.1
    IL_0045:  add
    IL_0046:  div.un
    IL_0047:  conv.i8
    IL_0048:  ldc.i4.1
    IL_0049:  conv.i8
    IL_004a:  add
    IL_004b:  nop
    IL_004c:  stloc.1
    IL_004d:  ldc.i4.0
    IL_004e:  conv.i8
    IL_004f:  stloc.3
    IL_0050:  ldc.i4.1
    IL_0051:  stloc.s    V_4
    IL_0053:  br.s       IL_0072

    IL_0055:  ldloc.s    V_4
    IL_0057:  stloc.s    V_5
    IL_0059:  ldloca.s   V_2
    IL_005b:  stloc.s    V_6
    IL_005d:  ldloc.s    V_6
    IL_005f:  ldloc.s    V_5
    IL_0061:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0066:  nop
    IL_0067:  ldloc.s    V_4
    IL_0069:  ldloc.0
    IL_006a:  add
    IL_006b:  stloc.s    V_4
    IL_006d:  ldloc.3
    IL_006e:  ldc.i4.1
    IL_006f:  conv.i8
    IL_0070:  add
    IL_0071:  stloc.3
    IL_0072:  ldloc.3
    IL_0073:  ldloc.1
    IL_0074:  blt.un.s   IL_0055

    IL_0076:  ldloca.s   V_2
    IL_0078:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_007d:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f24(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f) cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0,
             uint64 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  bge.s      IL_0011

    IL_000c:  ldc.i4.0
    IL_000d:  conv.i8
    IL_000e:  nop
    IL_000f:  br.s       IL_0019

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  conv.i8
    IL_0015:  ldc.i4.1
    IL_0016:  conv.i8
    IL_0017:  add
    IL_0018:  nop
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  conv.i8
    IL_001c:  stloc.3
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.s    V_4
    IL_0020:  br.s       IL_003f

    IL_0022:  ldloc.s    V_4
    IL_0024:  stloc.s    V_5
    IL_0026:  ldloca.s   V_2
    IL_0028:  stloc.s    V_6
    IL_002a:  ldloc.s    V_6
    IL_002c:  ldloc.s    V_5
    IL_002e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0033:  nop
    IL_0034:  ldloc.s    V_4
    IL_0036:  ldc.i4.1
    IL_0037:  add
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.3
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  ldloc.1
    IL_0041:  blt.un.s   IL_0022

    IL_0043:  ldloca.s   V_2
    IL_0045:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_004a:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f25(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> h) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2,
             uint64 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_4,
             uint64 V_5,
             int32 V_6,
             int32 V_7,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_8)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0007:  stloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_000f:  stloc.1
    IL_0010:  ldarg.2
    IL_0011:  ldnull
    IL_0012:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0017:  stloc.2
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0027

    IL_001b:  ldloc.0
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_0023:  pop
    IL_0024:  nop
    IL_0025:  br.s       IL_0028

    IL_0027:  nop
    IL_0028:  ldc.i4.0
    IL_0029:  ldloc.1
    IL_002a:  bge.s      IL_0041

    IL_002c:  ldloc.2
    IL_002d:  ldloc.0
    IL_002e:  bge.s      IL_0035

    IL_0030:  ldc.i4.0
    IL_0031:  conv.i8
    IL_0032:  nop
    IL_0033:  br.s       IL_0057

    IL_0035:  ldloc.2
    IL_0036:  ldloc.0
    IL_0037:  sub
    IL_0038:  ldloc.1
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  br.s       IL_0057

    IL_0041:  ldloc.0
    IL_0042:  ldloc.2
    IL_0043:  bge.s      IL_004a

    IL_0045:  ldc.i4.0
    IL_0046:  conv.i8
    IL_0047:  nop
    IL_0048:  br.s       IL_0057

    IL_004a:  ldloc.0
    IL_004b:  ldloc.2
    IL_004c:  sub
    IL_004d:  ldloc.1
    IL_004e:  not
    IL_004f:  ldc.i4.1
    IL_0050:  add
    IL_0051:  div.un
    IL_0052:  conv.i8
    IL_0053:  ldc.i4.1
    IL_0054:  conv.i8
    IL_0055:  add
    IL_0056:  nop
    IL_0057:  stloc.3
    IL_0058:  ldc.i4.0
    IL_0059:  conv.i8
    IL_005a:  stloc.s    V_5
    IL_005c:  ldloc.0
    IL_005d:  stloc.s    V_6
    IL_005f:  br.s       IL_0080

    IL_0061:  ldloc.s    V_6
    IL_0063:  stloc.s    V_7
    IL_0065:  ldloca.s   V_4
    IL_0067:  stloc.s    V_8
    IL_0069:  ldloc.s    V_8
    IL_006b:  ldloc.s    V_7
    IL_006d:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0072:  nop
    IL_0073:  ldloc.s    V_6
    IL_0075:  ldloc.1
    IL_0076:  add
    IL_0077:  stloc.s    V_6
    IL_0079:  ldloc.s    V_5
    IL_007b:  ldc.i4.1
    IL_007c:  conv.i8
    IL_007d:  add
    IL_007e:  stloc.s    V_5
    IL_0080:  ldloc.s    V_5
    IL_0082:  ldloc.3
    IL_0083:  blt.un.s   IL_0061

    IL_0085:  ldloca.s   V_4
    IL_0087:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_008c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<int32,float64>> 
          f26(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [runtime]System.Tuple`2<int32,float64>> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [runtime]System.Tuple`2<int32,float64>>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.2
    IL_0043:  ldarg.0
    IL_0044:  stloc.3
    IL_0045:  br.s       IL_0069

    IL_0047:  ldloc.3
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_1
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_5
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldloc.s    V_4
    IL_0054:  conv.r8
    IL_0055:  newobj     instance void class [runtime]System.Tuple`2<int32,float64>::.ctor(!0,
                                                                                                  !1)
    IL_005a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [runtime]System.Tuple`2<int32,float64>>::Add(!0)
    IL_005f:  nop
    IL_0060:  ldloc.3
    IL_0061:  ldarg.1
    IL_0062:  add
    IL_0063:  stloc.3
    IL_0064:  ldloc.2
    IL_0065:  ldc.i4.1
    IL_0066:  conv.i8
    IL_0067:  add
    IL_0068:  stloc.2
    IL_0069:  ldloc.2
    IL_006a:  ldloc.0
    IL_006b:  blt.un.s   IL_0047

    IL_006d:  ldloca.s   V_1
    IL_006f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [runtime]System.Tuple`2<int32,float64>>::Close()
    IL_0074:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<valuetype [runtime]System.ValueTuple`2<int32,float64>> 
          f27(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<valuetype [runtime]System.ValueTuple`2<int32,float64>> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<valuetype [runtime]System.ValueTuple`2<int32,float64>>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.2
    IL_0043:  ldarg.0
    IL_0044:  stloc.3
    IL_0045:  br.s       IL_0069

    IL_0047:  ldloc.3
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_1
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_5
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldloc.s    V_4
    IL_0054:  conv.r8
    IL_0055:  newobj     instance void valuetype [runtime]System.ValueTuple`2<int32,float64>::.ctor(!0,
                                                                                                           !1)
    IL_005a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<valuetype [runtime]System.ValueTuple`2<int32,float64>>::Add(!0)
    IL_005f:  nop
    IL_0060:  ldloc.3
    IL_0061:  ldarg.1
    IL_0062:  add
    IL_0063:  stloc.3
    IL_0064:  ldloc.2
    IL_0065:  ldc.i4.1
    IL_0066:  conv.i8
    IL_0067:  add
    IL_0068:  stloc.2
    IL_0069:  ldloc.2
    IL_006a:  ldloc.0
    IL_006b:  blt.un.s   IL_0047

    IL_006d:  ldloca.s   V_1
    IL_006f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<valuetype [runtime]System.ValueTuple`2<int32,float64>>::Close()
    IL_0074:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
          f28(int32 start,
              int32 step,
              int32 finish) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  5
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldarg.1
    IL_0005:  ldarg.2
    IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                           int32,
                                                                                                                                                                           int32)
    IL_000b:  pop
    IL_000c:  nop
    IL_000d:  br.s       IL_0010

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  ldarg.1
    IL_0012:  bge.s      IL_0029

    IL_0014:  ldarg.2
    IL_0015:  ldarg.0
    IL_0016:  bge.s      IL_001d

    IL_0018:  ldc.i4.0
    IL_0019:  conv.i8
    IL_001a:  nop
    IL_001b:  br.s       IL_003f

    IL_001d:  ldarg.2
    IL_001e:  ldarg.0
    IL_001f:  sub
    IL_0020:  ldarg.1
    IL_0021:  div.un
    IL_0022:  conv.i8
    IL_0023:  ldc.i4.1
    IL_0024:  conv.i8
    IL_0025:  add
    IL_0026:  nop
    IL_0027:  br.s       IL_003f

    IL_0029:  ldarg.0
    IL_002a:  ldarg.2
    IL_002b:  bge.s      IL_0032

    IL_002d:  ldc.i4.0
    IL_002e:  conv.i8
    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  ldarg.0
    IL_0033:  ldarg.2
    IL_0034:  sub
    IL_0035:  ldarg.1
    IL_0036:  not
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  div.un
    IL_003a:  conv.i8
    IL_003b:  ldc.i4.1
    IL_003c:  conv.i8
    IL_003d:  add
    IL_003e:  nop
    IL_003f:  stloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  conv.i8
    IL_0042:  stloc.2
    IL_0043:  ldarg.0
    IL_0044:  stloc.3
    IL_0045:  br.s       IL_0068

    IL_0047:  ldloc.3
    IL_0048:  stloc.s    V_4
    IL_004a:  ldloca.s   V_1
    IL_004c:  stloc.s    V_5
    IL_004e:  ldloc.s    V_5
    IL_0050:  stloc.s    V_6
    IL_0052:  ldloc.s    V_6
    IL_0054:  ldloc.s    V_4
    IL_0056:  ldloc.s    V_4
    IL_0058:  mul
    IL_0059:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_005e:  nop
    IL_005f:  ldloc.3
    IL_0060:  ldarg.1
    IL_0061:  add
    IL_0062:  stloc.3
    IL_0063:  ldloc.2
    IL_0064:  ldc.i4.1
    IL_0065:  conv.i8
    IL_0066:  add
    IL_0067:  stloc.2
    IL_0068:  ldloc.2
    IL_0069:  ldloc.0
    IL_006a:  blt.un.s   IL_0047

    IL_006c:  ldloca.s   V_1
    IL_006e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0073:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f29(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             int32 V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
             uint64 V_3,
             int32 V_4,
             int32 V_5,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_6)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.1
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i8
    IL_0013:  stloc.3
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.s    V_4
    IL_0017:  br.s       IL_003a

    IL_0019:  ldloc.s    V_4
    IL_001b:  stloc.s    V_5
    IL_001d:  ldloca.s   V_2
    IL_001f:  stloc.s    V_6
    IL_0021:  ldloc.s    V_6
    IL_0023:  ldloc.s    V_5
    IL_0025:  ldloc.0
    IL_0026:  add
    IL_0027:  ldloc.1
    IL_0028:  add
    IL_0029:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002e:  nop
    IL_002f:  ldloc.s    V_4
    IL_0031:  ldc.i4.2
    IL_0032:  add
    IL_0033:  stloc.s    V_4
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.1
    IL_0037:  conv.i8
    IL_0038:  add
    IL_0039:  stloc.3
    IL_003a:  ldloc.3
    IL_003b:  ldc.i4.5
    IL_003c:  conv.i8
    IL_003d:  blt.un.s   IL_0019

    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0046:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f30(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0008:  stloc.0
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i8
    IL_0013:  stloc.2
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.3
    IL_0016:  br.s       IL_0034

    IL_0018:  ldloc.3
    IL_0019:  stloc.s    V_4
    IL_001b:  ldloca.s   V_1
    IL_001d:  stloc.s    V_5
    IL_001f:  ldloc.s    V_5
    IL_0021:  ldloc.s    V_4
    IL_0023:  ldloc.0
    IL_0024:  add
    IL_0025:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.3
    IL_002c:  ldc.i4.2
    IL_002d:  add
    IL_002e:  stloc.3
    IL_002f:  ldloc.2
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  ldc.i4.5
    IL_0036:  conv.i8
    IL_0037:  blt.un.s   IL_0018

    IL_0039:  ldloca.s   V_1
    IL_003b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0040:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f31(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  4
    .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
             uint64 V_1,
             int32 V_2,
             int32 V_3,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0010:  pop
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i8
    IL_0013:  stloc.1
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.2
    IL_0016:  br.s       IL_0030

    IL_0018:  ldloc.2
    IL_0019:  stloc.3
    IL_001a:  ldloca.s   V_0
    IL_001c:  stloc.s    V_4
    IL_001e:  ldloc.s    V_4
    IL_0020:  ldloc.3
    IL_0021:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_0026:  nop
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.2
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.1
    IL_002d:  conv.i8
    IL_002e:  add
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.5
    IL_0032:  conv.i8
    IL_0033:  blt.un.s   IL_0018

    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_003c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> f32(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f, class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32> g) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
    
    .maxstack  5
    .locals init (int32 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>& V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0008:  pop
    IL_0009:  ldarg.1
    IL_000a:  ldnull
    IL_000b:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::Invoke(!0)
    IL_0010:  stloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  conv.i8
    IL_0013:  stloc.2
    IL_0014:  ldc.i4.1
    IL_0015:  stloc.3
    IL_0016:  br.s       IL_0034

    IL_0018:  ldloc.3
    IL_0019:  stloc.s    V_4
    IL_001b:  ldloca.s   V_1
    IL_001d:  stloc.s    V_5
    IL_001f:  ldloc.s    V_5
    IL_0021:  ldloc.s    V_4
    IL_0023:  ldloc.0
    IL_0024:  add
    IL_0025:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.3
    IL_002c:  ldc.i4.2
    IL_002d:  add
    IL_002e:  stloc.3
    IL_002f:  ldloc.2
    IL_0030:  ldc.i4.1
    IL_0031:  conv.i8
    IL_0032:  add
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  ldc.i4.5
    IL_0036:  conv.i8
    IL_0037:  blt.un.s   IL_0018

    IL_0039:  ldloca.s   V_1
    IL_003b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
    IL_0040:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
          f33<a>(int32 start,
                 int32 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_1,
             uint64 V_2,
             int32 V_3,
             int32 V_4,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>& V_5)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  conv.i8
    IL_000d:  ldc.i4.1
    IL_000e:  conv.i8
    IL_000f:  add
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldc.i4.0
    IL_0013:  conv.i8
    IL_0014:  stloc.2
    IL_0015:  ldarg.0
    IL_0016:  stloc.3
    IL_0017:  br.s       IL_0041

    IL_0019:  ldloc.3
    IL_001a:  stloc.s    V_4
    IL_001c:  ldloca.s   V_1
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.s    V_5
    IL_0022:  ldsfld     class assembly/f33@47 assembly/f33@47::@_instance
    IL_0027:  ldsfld     class assembly/'f33@47-1' assembly/'f33@47-1'::@_instance
    IL_002c:  ldnull
    IL_002d:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_0032:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Add(!0)
    IL_0037:  nop
    IL_0038:  ldloc.3
    IL_0039:  ldc.i4.1
    IL_003a:  add
    IL_003b:  stloc.3
    IL_003c:  ldloc.2
    IL_003d:  ldc.i4.1
    IL_003e:  conv.i8
    IL_003f:  add
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  ldloc.0
    IL_0043:  blt.un.s   IL_0019

    IL_0045:  ldloca.s   V_1
    IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Close()
    IL_004c:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
          f34<a>(int64 start,
                 int64 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2,
             bool V_3,
             uint64 V_4,
             int64 V_5,
             int64 V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>& V_7,
             uint64 V_8,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>& V_9)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.s      IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_005d

    IL_0017:  ldc.i4.1
    IL_0018:  stloc.3
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  stloc.s    V_4
    IL_001d:  ldarg.0
    IL_001e:  stloc.s    V_5
    IL_0020:  br.s       IL_0057

    IL_0022:  ldloc.s    V_5
    IL_0024:  stloc.s    V_6
    IL_0026:  ldloca.s   V_2
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldsfld     class assembly/f34@48 assembly/f34@48::@_instance
    IL_0031:  ldsfld     class assembly/'f34@48-1' assembly/'f34@48-1'::@_instance
    IL_0036:  ldnull
    IL_0037:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_003c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Add(!0)
    IL_0041:  nop
    IL_0042:  ldloc.s    V_5
    IL_0044:  ldc.i4.1
    IL_0045:  conv.i8
    IL_0046:  add
    IL_0047:  stloc.s    V_5
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldc.i4.1
    IL_004c:  conv.i8
    IL_004d:  add
    IL_004e:  stloc.s    V_4
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldc.i4.0
    IL_0053:  conv.i8
    IL_0054:  cgt.un
    IL_0056:  stloc.3
    IL_0057:  ldloc.3
    IL_0058:  brtrue.s   IL_0022

    IL_005a:  nop
    IL_005b:  br.s       IL_00ad

    IL_005d:  ldarg.1
    IL_005e:  ldarg.0
    IL_005f:  bge.s      IL_0066

    IL_0061:  ldc.i4.0
    IL_0062:  conv.i8
    IL_0063:  nop
    IL_0064:  br.s       IL_006d

    IL_0066:  ldarg.1
    IL_0067:  ldarg.0
    IL_0068:  sub
    IL_0069:  ldc.i4.1
    IL_006a:  conv.i8
    IL_006b:  add.ovf.un
    IL_006c:  nop
    IL_006d:  stloc.s    V_4
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  stloc.s    V_8
    IL_0073:  ldarg.0
    IL_0074:  stloc.s    V_5
    IL_0076:  br.s       IL_00a6

    IL_0078:  ldloc.s    V_5
    IL_007a:  stloc.s    V_6
    IL_007c:  ldloca.s   V_2
    IL_007e:  stloc.s    V_9
    IL_0080:  ldloc.s    V_9
    IL_0082:  ldsfld     class assembly/'f34@48-2' assembly/'f34@48-2'::@_instance
    IL_0087:  ldsfld     class assembly/'f34@48-3' assembly/'f34@48-3'::@_instance
    IL_008c:  ldnull
    IL_008d:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_0092:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Add(!0)
    IL_0097:  nop
    IL_0098:  ldloc.s    V_5
    IL_009a:  ldc.i4.1
    IL_009b:  conv.i8
    IL_009c:  add
    IL_009d:  stloc.s    V_5
    IL_009f:  ldloc.s    V_8
    IL_00a1:  ldc.i4.1
    IL_00a2:  conv.i8
    IL_00a3:  add
    IL_00a4:  stloc.s    V_8
    IL_00a6:  ldloc.s    V_8
    IL_00a8:  ldloc.s    V_4
    IL_00aa:  blt.un.s   IL_0078

    IL_00ac:  nop
    IL_00ad:  ldloca.s   V_2
    IL_00af:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Close()
    IL_00b4:  ret
  } 

  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> 
          f35<a>(uint64 start,
                 uint64 finish,
                 !!a f) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 00 00 ) 
    
    .maxstack  6
    .locals init (uint64 V_0,
             bool V_1,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_2,
             bool V_3,
             uint64 V_4,
             uint64 V_5,
             uint64 V_6,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>& V_7,
             uint64 V_8,
             valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>& V_9)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  bge.un.s   IL_0009

    IL_0004:  ldc.i4.0
    IL_0005:  conv.i8
    IL_0006:  nop
    IL_0007:  br.s       IL_000d

    IL_0009:  ldarg.1
    IL_000a:  ldarg.0
    IL_000b:  sub
    IL_000c:  nop
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.m1
    IL_0010:  conv.i8
    IL_0011:  ceq
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_005d

    IL_0017:  ldc.i4.1
    IL_0018:  stloc.3
    IL_0019:  ldc.i4.0
    IL_001a:  conv.i8
    IL_001b:  stloc.s    V_4
    IL_001d:  ldarg.0
    IL_001e:  stloc.s    V_5
    IL_0020:  br.s       IL_0057

    IL_0022:  ldloc.s    V_5
    IL_0024:  stloc.s    V_6
    IL_0026:  ldloca.s   V_2
    IL_0028:  stloc.s    V_7
    IL_002a:  ldloc.s    V_7
    IL_002c:  ldsfld     class assembly/f35@49 assembly/f35@49::@_instance
    IL_0031:  ldsfld     class assembly/'f35@49-1' assembly/'f35@49-1'::@_instance
    IL_0036:  ldnull
    IL_0037:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_003c:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Add(!0)
    IL_0041:  nop
    IL_0042:  ldloc.s    V_5
    IL_0044:  ldc.i4.1
    IL_0045:  conv.i8
    IL_0046:  add
    IL_0047:  stloc.s    V_5
    IL_0049:  ldloc.s    V_4
    IL_004b:  ldc.i4.1
    IL_004c:  conv.i8
    IL_004d:  add
    IL_004e:  stloc.s    V_4
    IL_0050:  ldloc.s    V_4
    IL_0052:  ldc.i4.0
    IL_0053:  conv.i8
    IL_0054:  cgt.un
    IL_0056:  stloc.3
    IL_0057:  ldloc.3
    IL_0058:  brtrue.s   IL_0022

    IL_005a:  nop
    IL_005b:  br.s       IL_00ad

    IL_005d:  ldarg.1
    IL_005e:  ldarg.0
    IL_005f:  bge.un.s   IL_0066

    IL_0061:  ldc.i4.0
    IL_0062:  conv.i8
    IL_0063:  nop
    IL_0064:  br.s       IL_006d

    IL_0066:  ldarg.1
    IL_0067:  ldarg.0
    IL_0068:  sub
    IL_0069:  ldc.i4.1
    IL_006a:  conv.i8
    IL_006b:  add.ovf.un
    IL_006c:  nop
    IL_006d:  stloc.s    V_4
    IL_006f:  ldc.i4.0
    IL_0070:  conv.i8
    IL_0071:  stloc.s    V_5
    IL_0073:  ldarg.0
    IL_0074:  stloc.s    V_6
    IL_0076:  br.s       IL_00a6

    IL_0078:  ldloc.s    V_6
    IL_007a:  stloc.s    V_8
    IL_007c:  ldloca.s   V_2
    IL_007e:  stloc.s    V_9
    IL_0080:  ldloc.s    V_9
    IL_0082:  ldsfld     class assembly/'f35@49-2' assembly/'f35@49-2'::@_instance
    IL_0087:  ldsfld     class assembly/'f35@49-3' assembly/'f35@49-3'::@_instance
    IL_008c:  ldnull
    IL_008d:  call       !!0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::InvokeFast<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,!!0>>,
                                                                                                                                                                                                                                                                                                                                                !0,
                                                                                                                                                                                                                                                                                                                                                !1)
    IL_0092:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Add(!0)
    IL_0097:  nop
    IL_0098:  ldloc.s    V_6
    IL_009a:  ldc.i4.1
    IL_009b:  conv.i8
    IL_009c:  add
    IL_009d:  stloc.s    V_6
    IL_009f:  ldloc.s    V_5
    IL_00a1:  ldc.i4.1
    IL_00a2:  conv.i8
    IL_00a3:  add
    IL_00a4:  stloc.s    V_5
    IL_00a6:  ldloc.s    V_5
    IL_00a8:  ldloc.s    V_4
    IL_00aa:  blt.un.s   IL_0078

    IL_00ac:  nop
    IL_00ad:  ldloca.s   V_2
    IL_00af:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Close()
    IL_00b4:  ret
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





