




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
.mresource public FSharpSignatureCompressedData.assembly
{
  
  
}
.mresource public FSharpOptimizationCompressedData.assembly
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
  .class abstract auto ansi sealed nested public M
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested assembly beforefieldinit f5@5
           extends [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc
    {
      .field static assembly initonly class assembly/M/f5@5 @_instance
      .method assembly specialname rtspecialname instance void  .ctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::.ctor()
        IL_0006:  ret
      } 

      .method public strict virtual instance object Specialize<a>() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  newobj     instance void class assembly/M/f5@5T<!!a>::.ctor(class assembly/M/f5@5)
        IL_0006:  box        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!a,!!a>>>>
        IL_000b:  ret
      } 

      .method private specialname rtspecialname static void  .cctor() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  10
        IL_0000:  newobj     instance void assembly/M/f5@5::.ctor()
        IL_0005:  stsfld     class assembly/M/f5@5 assembly/M/f5@5::@_instance
        IL_000a:  ret
      } 

    } 

    .class auto ansi serializable sealed nested assembly beforefieldinit f5@5T<a>
           extends class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,!a,!a,!a>
    {
      .field public class assembly/M/f5@5 self0@
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method assembly specialname rtspecialname instance void  .ctor(class assembly/M/f5@5 self0@) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.OptimizedClosures/FSharpFunc`5<int32,int32,!a,!a,!a>::.ctor()
        IL_0006:  ldarg.0
        IL_0007:  ldarg.1
        IL_0008:  stfld      class assembly/M/f5@5 class assembly/M/f5@5T<!a>::self0@
        IL_000d:  ret
      } 

      .method public strict virtual instance !a 
              Invoke(int32 x,
                     int32 y,
                     !a z,
                     !a w) cil managed
      {
        
        .maxstack  7
        .locals init (class assembly/M/f5@5 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      class assembly/M/f5@5 class assembly/M/f5@5T<!a>::self0@
        IL_0006:  stloc.0
        IL_0007:  nop
        IL_0008:  ldarg.1
        IL_0009:  ldarg.2
        IL_000a:  ble.s      IL_000e

        IL_000c:  ldarg.3
        IL_000d:  ret

        IL_000e:  ldarg.s    w
        IL_0010:  ret
      } 

    } 

    .method public static char  m() cil managed
    {
      
      .maxstack  7
      .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc V_0,
               char V_1,
               char V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldsfld     class assembly/M/f5@5 assembly/M/f5@5::@_instance
      IL_0005:  stloc.0
      IL_0006:  ldloc.0
      IL_0007:  ldc.i4.s   10
      IL_0009:  ldc.i4.s   10
      IL_000b:  ldc.i4.s   97
      IL_000d:  ldc.i4.s   98
      IL_000f:  stloc.1
      IL_0010:  stloc.2
      IL_0011:  stloc.3
      IL_0012:  stloc.s    V_4
      IL_0014:  callvirt   instance object [FSharp.Core]Microsoft.FSharp.Core.FSharpTypeFunc::Specialize<char>()
      IL_0019:  unbox.any  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<char,char>>>>
      IL_001e:  ldloc.s    V_4
      IL_0020:  ldloc.3
      IL_0021:  ldloc.2
      IL_0022:  ldloc.1
      IL_0023:  tail.
      IL_0025:  call       !!2 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::InvokeFast<char,char,char>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!1,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!1,!!2>>>>,
                                                                                                                              !0,
                                                                                                                              !1,
                                                                                                                              !!0,
                                                                                                                              !!1)
      IL_002a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly specialname static void staticInitialization@() cil managed
    {
      
      .maxstack  8
      IL_0000:  call       char assembly/M::m()
      IL_0005:  pop
      IL_0006:  ret
    } 

  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$assembly::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void assembly/M::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
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
    IL_0000:  call       void assembly::staticInitialization@()
    IL_0005:  ret
  } 

} 






