




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





.class public abstract auto ansi sealed SeqExpressionSteppingTest6
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public SeqExpressionSteppingTest6
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto autochar serializable sealed nested assembly beforefieldinit specialname f7@6
           extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
      .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> enum0
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 pc
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 current
      .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .method public specialname rtspecialname 
              instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                   class [runtime]System.Collections.Generic.IEnumerator`1<int32> enum0,
                                   int32 pc,
                                   int32 current) cil managed
      {
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_0007:  ldarg.0
        IL_0008:  ldarg.2
        IL_0009:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_000e:  ldarg.0
        IL_000f:  ldarg.3
        IL_0010:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0015:  ldarg.0
        IL_0016:  ldarg.s    current
        IL_0018:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_001d:  ldarg.0
        IL_001e:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
        IL_0023:  ret
      } 

      .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
      {
        
        .maxstack  6
        .locals init (int32 V_0,
                 int32 V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  ldc.i4.1
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0023,
                              IL_0026,
                              IL_0029,
                              IL_002f,
                              IL_0035)
        IL_0021:  br.s       IL_003b

        IL_0023:  nop
        IL_0024:  br.s       IL_0090

        IL_0026:  nop
        IL_0027:  br.s       IL_0083

        IL_0029:  nop
        IL_002a:  br         IL_00fc

        IL_002f:  nop
        IL_0030:  br         IL_00ef

        IL_0035:  nop
        IL_0036:  br         IL_011d

        IL_003b:  nop
        IL_003c:  br.s       IL_003e

        IL_003e:  ldarg.0
        IL_003f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
        IL_0044:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_0049:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_004e:  ldarg.0
        IL_004f:  ldc.i4.1
        IL_0050:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0055:  br.s       IL_0083

        IL_0057:  ldarg.0
        IL_0058:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_005d:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_0062:  stloc.0
        IL_0063:  ldstr      "hello"
        IL_0068:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_006d:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_0072:  pop
        IL_0073:  ldarg.0
        IL_0074:  ldc.i4.2
        IL_0075:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_007a:  ldarg.0
        IL_007b:  ldloc.0
        IL_007c:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_0081:  ldc.i4.1
        IL_0082:  ret

        IL_0083:  ldarg.0
        IL_0084:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_0089:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
        IL_008e:  brtrue.s   IL_0057

        IL_0090:  ldarg.0
        IL_0091:  ldc.i4.5
        IL_0092:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0097:  ldarg.0
        IL_0098:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_009d:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_00a2:  nop
        IL_00a3:  ldarg.0
        IL_00a4:  ldnull
        IL_00a5:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
        IL_00aa:  ldarg.0
        IL_00ab:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
        IL_00b0:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
        IL_00b5:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00ba:  ldarg.0
        IL_00bb:  ldc.i4.3
        IL_00bc:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_00c1:  br.s       IL_00ef

        IL_00c3:  ldarg.0
        IL_00c4:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00c9:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
        IL_00ce:  stloc.1
        IL_00cf:  ldstr      "goodbye"
        IL_00d4:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
        IL_00d9:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
        IL_00de:  pop
        IL_00df:  ldarg.0
        IL_00e0:  ldc.i4.4
        IL_00e1:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_00e6:  ldarg.0
        IL_00e7:  ldloc.1
        IL_00e8:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_00ed:  ldc.i4.1
        IL_00ee:  ret

        IL_00ef:  ldarg.0
        IL_00f0:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_00f5:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
        IL_00fa:  brtrue.s   IL_00c3

        IL_00fc:  ldarg.0
        IL_00fd:  ldc.i4.5
        IL_00fe:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0103:  ldarg.0
        IL_0104:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_0109:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_010e:  nop
        IL_010f:  ldarg.0
        IL_0110:  ldnull
        IL_0111:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
        IL_0116:  ldarg.0
        IL_0117:  ldc.i4.5
        IL_0118:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_011d:  ldarg.0
        IL_011e:  ldc.i4.0
        IL_011f:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
        IL_0124:  ldc.i4.0
        IL_0125:  ret
      } 

      .method public strict virtual instance void Close() cil managed
      {
        
        .maxstack  6
        .locals init (class [runtime]System.Exception V_0,
                 class [runtime]System.Exception V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  ldc.i4.5
        IL_0007:  sub
        IL_0008:  switch     ( 
                              IL_0013)
        IL_0011:  br.s       IL_0019

        IL_0013:  nop
        IL_0014:  br         IL_00a0

        IL_0019:  nop
        .try
        {
          IL_001a:  ldarg.0
          IL_001b:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0020:  switch     ( 
                                IL_003f,
                                IL_0042,
                                IL_0045,
                                IL_0048,
                                IL_004b,
                                IL_004e)
          IL_003d:  br.s       IL_0051

          IL_003f:  nop
          IL_0040:  br.s       IL_0080

          IL_0042:  nop
          IL_0043:  br.s       IL_006c

          IL_0045:  nop
          IL_0046:  br.s       IL_006b

          IL_0048:  nop
          IL_0049:  br.s       IL_0055

          IL_004b:  nop
          IL_004c:  br.s       IL_0054

          IL_004e:  nop
          IL_004f:  br.s       IL_0080

          IL_0051:  nop
          IL_0052:  br.s       IL_0054

          IL_0054:  nop
          IL_0055:  ldarg.0
          IL_0056:  ldc.i4.5
          IL_0057:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_005c:  ldarg.0
          IL_005d:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::enum0
          IL_0062:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_0067:  nop
          IL_0068:  nop
          IL_0069:  br.s       IL_0080

          IL_006b:  nop
          IL_006c:  ldarg.0
          IL_006d:  ldc.i4.5
          IL_006e:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0073:  ldarg.0
          IL_0074:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::'enum'
          IL_0079:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
          IL_007e:  nop
          IL_007f:  nop
          IL_0080:  ldarg.0
          IL_0081:  ldc.i4.5
          IL_0082:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
          IL_0087:  ldarg.0
          IL_0088:  ldc.i4.0
          IL_0089:  stfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
          IL_008e:  leave.s    IL_009a

        }  
        catch [runtime]System.Object 
        {
          IL_0090:  castclass  [runtime]System.Exception
          IL_0095:  stloc.1
          IL_0096:  ldloc.1
          IL_0097:  stloc.0
          IL_0098:  leave.s    IL_009a

        }  
        IL_009a:  nop
        IL_009b:  br         IL_0000

        IL_00a0:  ldloc.0
        IL_00a1:  brfalse.s  IL_00a5

        IL_00a3:  ldloc.0
        IL_00a4:  throw

        IL_00a5:  ret
      } 

      .method public strict virtual instance bool get_CheckClose() cil managed
      {
        
        .maxstack  5
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::pc
        IL_0006:  switch     ( 
                              IL_0025,
                              IL_0028,
                              IL_002b,
                              IL_002e,
                              IL_0031,
                              IL_0034)
        IL_0023:  br.s       IL_0037

        IL_0025:  nop
        IL_0026:  br.s       IL_0042

        IL_0028:  nop
        IL_0029:  br.s       IL_0040

        IL_002b:  nop
        IL_002c:  br.s       IL_003e

        IL_002e:  nop
        IL_002f:  br.s       IL_003c

        IL_0031:  nop
        IL_0032:  br.s       IL_003a

        IL_0034:  nop
        IL_0035:  br.s       IL_0042

        IL_0037:  nop
        IL_0038:  br.s       IL_003a

        IL_003a:  ldc.i4.1
        IL_003b:  ret

        IL_003c:  ldc.i4.1
        IL_003d:  ret

        IL_003e:  ldc.i4.1
        IL_003f:  ret

        IL_0040:  ldc.i4.1
        IL_0041:  ret

        IL_0042:  ldc.i4.0
        IL_0043:  ret
      } 

      .method public strict virtual instance int32 get_LastGenerated() cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::current
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
        IL_0004:  newobj     instance void SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                             int32,
                                                                                                             int32)
        IL_0009:  ret
      } 

    } 

    .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> es@4
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_es() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::es@4
      IL_0005:  ret
    } 

    .method public static class [runtime]System.Collections.Generic.IEnumerable`1<int32> f7() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldnull
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.0
      IL_0004:  newobj     instance void SeqExpressionSteppingTest6/SeqExpressionSteppingTest6/f7@6::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                           class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                                           int32,
                                                                                                           int32)
      IL_0009:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest6::init@
      IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest6::init@
      IL_000b:  pop
      IL_000c:  ret
    } 

    .method assembly static void  staticInitialization@() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0)
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
      IL_0017:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::es@4
      IL_001c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::f7()
      IL_0021:  stloc.0
      IL_0022:  ldloc.0
      IL_0023:  call       int32 [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Length<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0028:  pop
      IL_0029:  ret
    } 

    .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
            es()
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
      .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::get_es()
    } 
  } 

  .method private specialname rtspecialname static void  .cctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest6::init@
    IL_0006:  ldsfld     int32 '<StartupCode$assembly>'.$SeqExpressionSteppingTest6::init@
    IL_000b:  pop
    IL_000c:  ret
  } 

  .method assembly static void  staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       void SeqExpressionSteppingTest6/SeqExpressionSteppingTest6::staticInitialization@()
    IL_0005:  ret
  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$SeqExpressionSteppingTest6
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
    IL_0000:  call       void SeqExpressionSteppingTest6::staticInitialization@()
    IL_0005:  ret
  } 

} 





