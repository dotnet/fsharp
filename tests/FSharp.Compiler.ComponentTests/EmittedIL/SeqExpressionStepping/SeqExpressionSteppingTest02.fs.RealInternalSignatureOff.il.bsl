




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





.class public abstract auto ansi sealed SeqExpressionSteppingTest2
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest2
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f1@5
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public int32 pc
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname instance void  .ctor(int32 pc, int32 current) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::current
        IL_000e:  ldarg.0
        IL_000f:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0014:  ret
      } 

      .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        
        .maxstack  6
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001b,
                              IL_001e,
                              IL_0021)
        IL_0019:  br.s       IL_0024

        IL_001b:  nop
        IL_001c:  br.s       IL_0047

        IL_001e:  nop
        IL_001f:  br.s       IL_0067

        IL_0021:  nop
        IL_0022:  br.s       IL_006e

        IL_0024:  nop
        IL_0025:  br.s       IL_0027

        IL_0027:  ldstr      "hello"
        IL_002c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0031:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0036:  pop
        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.1
        IL_0039:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_003e:  ldarg.0
        IL_003f:  ldc.i4.1
        IL_0040:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::current
        IL_0045:  ldc.i4.1
        IL_0046:  ret

        IL_0047:  ldstr      "goodbye"
        IL_004c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_0051:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0056:  pop
        IL_0057:  ldarg.0
        IL_0058:  ldc.i4.2
        IL_0059:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_005e:  ldarg.0
        IL_005f:  ldc.i4.2
        IL_0060:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::current
        IL_0065:  ldc.i4.1
        IL_0066:  ret

        IL_0067:  ldarg.0
        IL_0068:  ldc.i4.3
        IL_0069:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_006e:  ldarg.0
        IL_006f:  ldc.i4.0
        IL_0070:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::current
        IL_0075:  ldc.i4.0
        IL_0076:  ret
      } 

      .method public strict virtual instance void Close() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.3
        IL_0002:  stfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_0007:  ret
      } 

      .method public strict virtual instance bool get_CheckClose() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::pc
        IL_0006:  switch     ( 
                              IL_001d,
                              IL_0020,
                              IL_0023,
                              IL_0026)
        IL_001b:  br.s       IL_0029

        IL_001d:  nop
        IL_001e:  br.s       IL_0030

        IL_0020:  nop
        IL_0021:  br.s       IL_002e

        IL_0023:  nop
        IL_0024:  br.s       IL_002c

        IL_0026:  nop
        IL_0027:  br.s       IL_0030

        IL_0029:  nop
        IL_002a:  br.s       IL_002c

        IL_002c:  ldc.i4.0
        IL_002d:  ret

        IL_002e:  ldc.i4.0
        IL_002f:  ret

        IL_0030:  ldc.i4.0
        IL_0031:  ret
      } 

      .method public strict virtual instance int32 get_LastGenerated() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::current
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldc.i4.0
        IL_0001:  ldc.i4.0
        IL_0002:  newobj     instance void SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::.ctor(int32,
                                                                                                             int32)
        IL_0007:  ret
      } 

    } 

    .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> f1() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ldc.i4.0
      IL_0002:  newobj     instance void SeqExpressionSteppingTest2/SeqExpressionSteppingTest2/f1@5::.ctor(int32,
                                                                                                           int32)
      IL_0007:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$SeqExpressionSteppingTest2
       extends [runtime]System.Object
{
  .field static assembly int32 init@
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  3
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0)
    IL_0000:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest2/SeqExpressionSteppingTest2::f1()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } 

} 





