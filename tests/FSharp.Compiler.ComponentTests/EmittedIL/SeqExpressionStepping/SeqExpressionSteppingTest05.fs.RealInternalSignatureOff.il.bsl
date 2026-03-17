




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





.class public abstract auto ansi sealed SeqExpressionSteppingTest5
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest5
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f4@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x
      .field public class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y
      .field public int32 pc
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> x,
                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> y,
                                   int32 pc,
                                   int32 current) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } 

      .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        
        .maxstack  7
        .locals init (int32 V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_001f,
                              IL_0025,
                              IL_0028,
                              IL_002e)
        IL_001d:  br.s       IL_0034

        IL_001f:  nop
        IL_0020:  br         IL_00b7

        IL_0025:  nop
        IL_0026:  br.s       IL_0088

        IL_0028:  nop
        IL_0029:  br         IL_00b0

        IL_002e:  nop
        IL_002f:  br         IL_00f4

        IL_0034:  nop
        IL_0035:  br.s       IL_0037

        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.0
        IL_0039:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_003e:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_0043:  ldarg.0
        IL_0044:  ldc.i4.1
        IL_0045:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_004a:  ldarg.0
        IL_004b:  ldc.i4.0
        IL_004c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::.ctor(!0)
        IL_0051:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0056:  ldarg.0
        IL_0057:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_005c:  ldarg.0
        IL_005d:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0062:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0067:  ldc.i4.1
        IL_0068:  add
        IL_0069:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_006e:  ldarg.0
        IL_006f:  ldc.i4.2
        IL_0070:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0075:  ldarg.0
        IL_0076:  ldarg.0
        IL_0077:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_007c:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0081:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_0086:  ldc.i4.1
        IL_0087:  ret

        IL_0088:  ldarg.0
        IL_0089:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_008e:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_0093:  ldarg.0
        IL_0094:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_0099:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_009e:  add
        IL_009f:  stloc.0
        IL_00a0:  ldarg.0
        IL_00a1:  ldc.i4.3
        IL_00a2:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00a7:  ldarg.0
        IL_00a8:  ldloc.0
        IL_00a9:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_00ae:  ldc.i4.1
        IL_00af:  ret

        IL_00b0:  ldarg.0
        IL_00b1:  ldnull
        IL_00b2:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::y
        IL_00b7:  ldarg.0
        IL_00b8:  ldc.i4.4
        IL_00b9:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00be:  ldarg.0
        IL_00bf:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00c4:  ldarg.0
        IL_00c5:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00ca:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
        IL_00cf:  ldc.i4.1
        IL_00d0:  add
        IL_00d1:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
        IL_00d6:  ldstr      "done"
        IL_00db:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00e0:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00e5:  pop
        IL_00e6:  ldarg.0
        IL_00e7:  ldnull
        IL_00e8:  stfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
        IL_00ed:  ldarg.0
        IL_00ee:  ldc.i4.4
        IL_00ef:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_00f4:  ldarg.0
        IL_00f5:  ldc.i4.0
        IL_00f6:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_00fb:  ldc.i4.0
        IL_00fc:  ret
      } 

      .method public strict virtual instance void Close() cil managed
      {
        
        .maxstack  7
        .locals init (class [runtime]System.Exception V_0,
                 class [runtime]System.Exception V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  ldc.i4.4
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0019

        IL_0013:  nop
        IL_0014:  br         IL_00a1

        IL_0019:  nop
        .try
        {
          IL_001a:  ldarg.0
          IL_001b:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0020:  switch     ( 
                                IL_003b,
                                IL_003e,
                                IL_0041,
                                IL_0044,
                                IL_0047)
          IL_0039:  br.s       IL_004a

          IL_003b:  nop
          IL_003c:  br.s       IL_0081

          IL_003e:  nop
          IL_003f:  br.s       IL_0051

          IL_0041:  nop
          IL_0042:  br.s       IL_0050

          IL_0044:  nop
          IL_0045:  br.s       IL_004d

          IL_0047:  nop
          IL_0048:  br.s       IL_0081

          IL_004a:  nop
          IL_004b:  br.s       IL_004d

          IL_004d:  nop
          IL_004e:  br.s       IL_0051

          IL_0050:  nop
          IL_0051:  ldarg.0
          IL_0052:  ldc.i4.4
          IL_0053:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0058:  ldarg.0
          IL_0059:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
          IL_005e:  ldarg.0
          IL_005f:  ldfld      class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::x
          IL_0064:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::get_contents()
          IL_0069:  ldc.i4.1
          IL_006a:  add
          IL_006b:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>::set_contents(!0)
          IL_0070:  ldstr      "done"
          IL_0075:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
          IL_007a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
          IL_007f:  pop
          IL_0080:  nop
          IL_0081:  ldarg.0
          IL_0082:  ldc.i4.4
          IL_0083:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
          IL_0088:  ldarg.0
          IL_0089:  ldc.i4.0
          IL_008a:  stfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
          IL_008f:  leave.s    IL_009b

        }  
        catch [runtime]System.Object 
        {
          IL_0091:  castclass  [runtime]System.Exception
          IL_0096:  stloc.1
          IL_0097:  ldloc.1
          IL_0098:  stloc.0
          IL_0099:  leave.s    IL_009b

        }  
        IL_009b:  nop
        IL_009c:  br         IL_0000

        IL_00a1:  ldloc.0
        IL_00a2:  brfalse.s  IL_00a6

        IL_00a4:  ldloc.0
        IL_00a5:  throw

        IL_00a6:  ret
      } 

      .method public strict virtual instance bool get_CheckClose() cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::pc
        IL_0006:  switch     ( 
                              IL_0021,
                              IL_0024,
                              IL_0027,
                              IL_002a,
                              IL_002d)
        IL_001f:  br.s       IL_0030

        IL_0021:  nop
        IL_0022:  br.s       IL_0039

        IL_0024:  nop
        IL_0025:  br.s       IL_0037

        IL_0027:  nop
        IL_0028:  br.s       IL_0035

        IL_002a:  nop
        IL_002b:  br.s       IL_0033

        IL_002d:  nop
        IL_002e:  br.s       IL_0039

        IL_0030:  nop
        IL_0031:  br.s       IL_0033

        IL_0033:  ldc.i4.1
        IL_0034:  ret

        IL_0035:  ldc.i4.1
        IL_0036:  ret

        IL_0037:  ldc.i4.1
        IL_0038:  ret

        IL_0039:  ldc.i4.0
        IL_003a:  ret
      } 

      .method public strict virtual instance int32 get_LastGenerated() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::current
        IL_0006:  ret
      } 

      .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldnull
        IL_0001:  ldnull
        IL_0002:  ldc.i4.0
        IL_0003:  ldc.i4.0
        IL_0004:  newobj     instance void SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_0009:  ret
      } 

    } 

    .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> f4() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest5/SeqExpressionSteppingTest5/f4@6::.ctor(class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           class [FSharp.Core]Microsoft.FSharp.Core.FSharpRef`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_0009:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$SeqExpressionSteppingTest5
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
    IL_0000:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest5/SeqExpressionSteppingTest5::f4()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  pop
    IL_000d:  ret
  } 

} 





