




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern runtime { }
.assembly extern System.Collections
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         
  .ver 8:0:0:0
}
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
.module assembly.dll

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed Spurious_tail
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto ansi serializable sealed nested public LoggerLevel
         extends [runtime]System.Enum
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype Spurious_tail/LoggerLevel A = int32(0x00000000)
    .field public static literal valuetype Spurious_tail/LoggerLevel B = int32(0x00000001)
  } 

  .class auto ansi serializable nested public Logger
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly class [System.Collections]System.Collections.Generic.List`1<string> messages
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4     0x2710
      IL_000e:  newobj     instance void class [System.Collections]System.Collections.Generic.List`1<string>::.ctor(int32)
      IL_0013:  stfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/Logger::messages
      IL_0018:  ret
    } 

    .method public hidebysig instance void 
            Log(valuetype Spurious_tail/LoggerLevel level,
                string msg) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/Logger::messages
      IL_0006:  ldarg.2
      IL_0007:  callvirt   instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
      IL_000c:  ldarg.0
      IL_000d:  callvirt   instance void Spurious_tail/Logger::'nop'()
      IL_0012:  ret
    } 

    .method public hidebysig instance void 
            Log<a>(valuetype Spurious_tail/LoggerLevel level,
                   !!a obj) cil managed
    {
      
      .maxstack  4
      .locals init (!!a V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/Logger::messages
      IL_0006:  ldarg.2
      IL_0007:  stloc.0
      IL_0008:  ldloca.s   V_0
      IL_000a:  constrained. !!a
      IL_0010:  callvirt   instance string [runtime]System.Object::ToString()
      IL_0015:  callvirt   instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
      IL_001a:  ldarg.0
      IL_001b:  callvirt   instance void Spurious_tail/Logger::'nop'()
      IL_0020:  ret
    } 

    .method assembly hidebysig instance void 'nop'() cil managed aggressiveinlining
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ret
    } 

  } 

  .class auto ansi serializable nested public TailLogger
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly class [System.Collections]System.Collections.Generic.List`1<string> messages
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4     0x2710
      IL_000e:  newobj     instance void class [System.Collections]System.Collections.Generic.List`1<string>::.ctor(int32)
      IL_0013:  stfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/TailLogger::messages
      IL_0018:  ret
    } 

    .method public hidebysig instance void 
            Log(valuetype Spurious_tail/LoggerLevel level,
                string msg) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/TailLogger::messages
      IL_0006:  ldarg.2
      IL_0007:  tail.
      IL_0009:  callvirt   instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
      IL_000e:  ret
    } 

    .method public hidebysig instance void 
            Log<a>(valuetype Spurious_tail/LoggerLevel level,
                   !!a obj) cil managed
    {
      
      .maxstack  4
      .locals init (!!a V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [System.Collections]System.Collections.Generic.List`1<string> Spurious_tail/TailLogger::messages
      IL_0006:  ldarg.2
      IL_0007:  stloc.0
      IL_0008:  ldloca.s   V_0
      IL_000a:  constrained. !!a
      IL_0010:  callvirt   instance string [runtime]System.Object::ToString()
      IL_0015:  tail.
      IL_0017:  callvirt   instance void class [System.Collections]System.Collections.Generic.List`1<string>::Add(!0)
      IL_001c:  ret
    } 

  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$Spurious_tail$fsx::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$Spurious_tail$fsx::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void [runtime]System.Console::WriteLine()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$Spurious_tail$fsx
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void Spurious_tail::staticInitialization@()
    IL_0005:  ret
  } 

} 






