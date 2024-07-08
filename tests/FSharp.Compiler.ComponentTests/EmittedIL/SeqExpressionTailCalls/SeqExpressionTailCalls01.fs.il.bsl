




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
  .class auto autochar serializable sealed nested assembly beforefieldinit specialname rwalk@3
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public int32 x
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(int32 x,
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 assembly/rwalk@3::x
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/rwalk@3::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/rwalk@3::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 
            GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  7
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/rwalk@3::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_003a

      IL_001e:  nop
      IL_001f:  br.s       IL_0056

      IL_0021:  nop
      IL_0022:  br.s       IL_005d

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.1
      IL_0027:  stfld      int32 assembly/rwalk@3::pc
      IL_002c:  ldarg.0
      IL_002d:  ldarg.0
      IL_002e:  ldfld      int32 assembly/rwalk@3::x
      IL_0033:  stfld      int32 assembly/rwalk@3::current
      IL_0038:  ldc.i4.1
      IL_0039:  ret

      IL_003a:  ldarg.0
      IL_003b:  ldc.i4.2
      IL_003c:  stfld      int32 assembly/rwalk@3::pc
      IL_0041:  ldarg.1
      IL_0042:  ldarg.0
      IL_0043:  ldfld      int32 assembly/rwalk@3::x
      IL_0048:  ldc.i4.1
      IL_0049:  add
      IL_004a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> assembly::rwalk(int32)
      IL_004f:  stobj      class [runtime]System.Collections.Generic.IEnumerable`1<int32>
      IL_0054:  ldc.i4.2
      IL_0055:  ret

      IL_0056:  ldarg.0
      IL_0057:  ldc.i4.3
      IL_0058:  stfld      int32 assembly/rwalk@3::pc
      IL_005d:  ldarg.0
      IL_005e:  ldc.i4.0
      IL_005f:  stfld      int32 assembly/rwalk@3::current
      IL_0064:  ldc.i4.0
      IL_0065:  ret
    } 

    .method public strict virtual instance void 
            Close() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.3
      IL_0002:  stfld      int32 assembly/rwalk@3::pc
      IL_0007:  ret
    } 

    .method public strict virtual instance bool 
            get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/rwalk@3::pc
      IL_0006:  switch     ( 
                            IL_001d,
                            IL_0020,
                            IL_0023,
                            IL_0026)
      IL_001b:  br.s       IL_0029

      IL_001d:  nop
      IL_001e:  br.s       IL_002e

      IL_0020:  nop
      IL_0021:  br.s       IL_002c

      IL_0023:  nop
      IL_0024:  br.s       IL_002a

      IL_0026:  nop
      IL_0027:  br.s       IL_002e

      IL_0029:  nop
      IL_002a:  ldc.i4.0
      IL_002b:  ret

      IL_002c:  ldc.i4.0
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 
            get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/rwalk@3::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> 
            GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/rwalk@3::x
      IL_0006:  ldc.i4.0
      IL_0007:  ldc.i4.0
      IL_0008:  newobj     instance void assembly/rwalk@3::.ctor(int32,
                                                                                 int32,
                                                                                 int32)
      IL_000d:  ret
    } 

  } 

  .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> 
          rwalk(int32 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj     instance void assembly/rwalk@3::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_0008:  ret
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






