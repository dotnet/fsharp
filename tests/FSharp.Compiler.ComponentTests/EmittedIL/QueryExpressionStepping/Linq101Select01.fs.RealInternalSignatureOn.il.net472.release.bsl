




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern Utils
{
  .ver 0:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:0:0:0
}
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
  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #1 input at line 11@12-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class assembly/'Pipe #1 input at line 11@12-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #1 input at line 11@12-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #1 input at line 11@12-1' assembly/'Pipe #1 input at line 11@12-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #1 input at line 11@13'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
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
                                 int32 pc,
                                 int32 current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<int32>& next) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0073

      IL_001e:  nop
      IL_001f:  br.s       IL_0066

      IL_0021:  nop
      IL_0022:  br.s       IL_0094

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class assembly/'Pipe #1 input at line 11@12-1' assembly/'Pipe #1 input at line 11@12-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_0030:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_0046:  br.s       IL_0066

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_004e:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_005b:  ldarg.0
      IL_005c:  ldloc.0
      IL_005d:  ldc.i4.1
      IL_005e:  add
      IL_005f:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::current
      IL_0064:  ldc.i4.1
      IL_0065:  ret

      IL_0066:  ldarg.0
      IL_0067:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_006c:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0071:  brtrue.s   IL_0048

      IL_0073:  ldarg.0
      IL_0074:  ldc.i4.3
      IL_0075:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_007a:  ldarg.0
      IL_007b:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_0080:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_0085:  nop
      IL_0086:  ldarg.0
      IL_0087:  ldnull
      IL_0088:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
      IL_008d:  ldarg.0
      IL_008e:  ldc.i4.3
      IL_008f:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_0094:  ldarg.0
      IL_0095:  ldc.i4.0
      IL_0096:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::current
      IL_009b:  ldc.i4.0
      IL_009c:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #1 input at line 11@13'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldc.i4.0
        IL_005f:  stfld      int32 assembly/'Pipe #1 input at line 11@13'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@13'::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance int32 get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #1 input at line 11@13'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.0
      IL_0003:  newobj     instance void assembly/'Pipe #1 input at line 11@13'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              int32)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'productNames@21-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class assembly/'productNames@21-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'productNames@21-1'::.ctor()
      IL_0005:  stsfld     class assembly/'productNames@21-1' assembly/'productNames@21-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname productNames@22
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/productNames@22::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/productNames@22::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/productNames@22::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_0076

      IL_001e:  nop
      IL_001f:  br.s       IL_0069

      IL_0021:  nop
      IL_0022:  br.s       IL_0097

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class assembly/'productNames@21-1' assembly/'productNames@21-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
      IL_0030:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003a:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 assembly/productNames@22::pc
      IL_0046:  br.s       IL_0069

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_004e:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 assembly/productNames@22::pc
      IL_005b:  ldarg.0
      IL_005c:  ldloc.0
      IL_005d:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0062:  stfld      string assembly/productNames@22::current
      IL_0067:  ldc.i4.1
      IL_0068:  ret

      IL_0069:  ldarg.0
      IL_006a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_006f:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0074:  brtrue.s   IL_0048

      IL_0076:  ldarg.0
      IL_0077:  ldc.i4.3
      IL_0078:  stfld      int32 assembly/productNames@22::pc
      IL_007d:  ldarg.0
      IL_007e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_0083:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_0088:  nop
      IL_0089:  ldarg.0
      IL_008a:  ldnull
      IL_008b:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.3
      IL_0092:  stfld      int32 assembly/productNames@22::pc
      IL_0097:  ldarg.0
      IL_0098:  ldnull
      IL_0099:  stfld      string assembly/productNames@22::current
      IL_009e:  ldc.i4.0
      IL_009f:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/productNames@22::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/productNames@22::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/productNames@22::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/productNames@22::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/productNames@22::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/productNames@22::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/productNames@22::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance string get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/productNames@22::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/productNames@22::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                int32,
                                                                                string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #2 input at line 28@29-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class assembly/'Pipe #2 input at line 28@29-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #2 input at line 28@29-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #2 input at line 28@29-1' assembly/'Pipe #2 input at line 28@29-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #2 input at line 28@30'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public string current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 string current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      string assembly/'Pipe #2 input at line 28@30'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<string>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<string>& next) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0024

      IL_001b:  nop
      IL_001c:  br.s       IL_007b

      IL_001e:  nop
      IL_001f:  br.s       IL_006e

      IL_0021:  nop
      IL_0022:  br.s       IL_009c

      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldsfld     class assembly/'Pipe #2 input at line 28@29-1' assembly/'Pipe #2 input at line 28@29-1'::@_instance
      IL_002b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_0030:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0035:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003a:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_003f:  ldarg.0
      IL_0040:  ldc.i4.1
      IL_0041:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_0046:  br.s       IL_006e

      IL_0048:  ldarg.0
      IL_0049:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_004e:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0053:  stloc.0
      IL_0054:  ldarg.0
      IL_0055:  ldc.i4.2
      IL_0056:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_005b:  ldarg.0
      IL_005c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_strings()
      IL_0061:  ldloc.0
      IL_0062:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_0067:  stfld      string assembly/'Pipe #2 input at line 28@30'::current
      IL_006c:  ldc.i4.1
      IL_006d:  ret

      IL_006e:  ldarg.0
      IL_006f:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_0074:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0079:  brtrue.s   IL_0048

      IL_007b:  ldarg.0
      IL_007c:  ldc.i4.3
      IL_007d:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_0082:  ldarg.0
      IL_0083:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_0088:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_008d:  nop
      IL_008e:  ldarg.0
      IL_008f:  ldnull
      IL_0090:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
      IL_0095:  ldarg.0
      IL_0096:  ldc.i4.3
      IL_0097:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_009c:  ldarg.0
      IL_009d:  ldnull
      IL_009e:  stfld      string assembly/'Pipe #2 input at line 28@30'::current
      IL_00a3:  ldc.i4.0
      IL_00a4:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #2 input at line 28@30'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      string assembly/'Pipe #2 input at line 28@30'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #2 input at line 28@30'::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance string get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/'Pipe #2 input at line 28@30'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<string> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/'Pipe #2 input at line 28@30'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              string)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #3 input at line 37@38-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<string>>
  {
    .field static assembly initonly class assembly/'Pipe #3 input at line 37@38-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<string,class [runtime]System.Collections.Generic.IEnumerable`1<string>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<string> Invoke(string _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (string V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<string>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #3 input at line 37@38-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #3 input at line 37@38-1' assembly/'Pipe #3 input at line 37@38-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #3 input at line 37@39'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<string,string>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Tuple`2<string,string> current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string> 'enum',
                                 int32 pc,
                                 class [runtime]System.Tuple`2<string,string> current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [runtime]System.Tuple`2<string,string> assembly/'Pipe #3 input at line 37@39'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<string,string>>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,string>>& next) cil managed
    {
      
      .maxstack  7
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0084

      IL_001e:  nop
      IL_001f:  br.s       IL_0077

      IL_0021:  nop
      IL_0022:  br         IL_00a5

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class assembly/'Pipe #3 input at line 37@38-1' assembly/'Pipe #3 input at line 37@38-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
      IL_0033:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<string,class [runtime]System.Collections.Generic.IEnumerable`1<string>,string>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                      class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<string>::GetEnumerator()
      IL_003d:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_0049:  br.s       IL_0077

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_0051:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<string>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_005e:  ldarg.0
      IL_005f:  ldloc.0
      IL_0060:  callvirt   instance string [runtime]System.String::ToUpper()
      IL_0065:  ldloc.0
      IL_0066:  callvirt   instance string [runtime]System.String::ToLower()
      IL_006b:  newobj     instance void class [runtime]System.Tuple`2<string,string>::.ctor(!0,
                                                                                              !1)
      IL_0070:  stfld      class [runtime]System.Tuple`2<string,string> assembly/'Pipe #3 input at line 37@39'::current
      IL_0075:  ldc.i4.1
      IL_0076:  ret

      IL_0077:  ldarg.0
      IL_0078:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_007d:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0082:  brtrue.s   IL_004b

      IL_0084:  ldarg.0
      IL_0085:  ldc.i4.3
      IL_0086:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_008b:  ldarg.0
      IL_008c:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_0091:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
      IL_0096:  nop
      IL_0097:  ldarg.0
      IL_0098:  ldnull
      IL_0099:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.3
      IL_00a0:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_00a5:  ldarg.0
      IL_00a6:  ldnull
      IL_00a7:  stfld      class [runtime]System.Tuple`2<string,string> assembly/'Pipe #3 input at line 37@39'::current
      IL_00ac:  ldc.i4.0
      IL_00ad:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<string> assembly/'Pipe #3 input at line 37@39'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<string>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [runtime]System.Tuple`2<string,string> assembly/'Pipe #3 input at line 37@39'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #3 input at line 37@39'::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,string> get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Tuple`2<string,string> assembly/'Pipe #3 input at line 37@39'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<string,string>> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/'Pipe #3 input at line 37@39'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                              int32,
                                                                                              class [runtime]System.Tuple`2<string,string>)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #4 input at line 44@45-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class assembly/'Pipe #4 input at line 44@45-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #4 input at line 44@45-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #4 input at line 44@45-1' assembly/'Pipe #4 input at line 44@45-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #4 input at line 44@46'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<string,bool>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Tuple`2<string,bool> current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32> 'enum',
                                 int32 pc,
                                 class [runtime]System.Tuple`2<string,bool> current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [runtime]System.Tuple`2<string,bool> assembly/'Pipe #4 input at line 44@46'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`2<string,bool>>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,bool>>& next) cil managed
    {
      
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_0089

      IL_001e:  nop
      IL_001f:  br.s       IL_007c

      IL_0021:  nop
      IL_0022:  br         IL_00aa

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class assembly/'Pipe #4 input at line 44@45-1' assembly/'Pipe #4 input at line 44@45-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
      IL_0033:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<int32,class [runtime]System.Collections.Generic.IEnumerable`1<int32>,int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                   class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
      IL_003d:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_0049:  br.s       IL_007c

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_0051:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_005e:  ldarg.0
      IL_005f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_strings()
      IL_0064:  ldloc.0
      IL_0065:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_006a:  ldloc.0
      IL_006b:  ldc.i4.2
      IL_006c:  rem
      IL_006d:  ldc.i4.0
      IL_006e:  ceq
      IL_0070:  newobj     instance void class [runtime]System.Tuple`2<string,bool>::.ctor(!0,
                                                                                            !1)
      IL_0075:  stfld      class [runtime]System.Tuple`2<string,bool> assembly/'Pipe #4 input at line 44@46'::current
      IL_007a:  ldc.i4.1
      IL_007b:  ret

      IL_007c:  ldarg.0
      IL_007d:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_0082:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0087:  brtrue.s   IL_004b

      IL_0089:  ldarg.0
      IL_008a:  ldc.i4.3
      IL_008b:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_0090:  ldarg.0
      IL_0091:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_0096:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
      IL_009b:  nop
      IL_009c:  ldarg.0
      IL_009d:  ldnull
      IL_009e:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
      IL_00a3:  ldarg.0
      IL_00a4:  ldc.i4.3
      IL_00a5:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_00aa:  ldarg.0
      IL_00ab:  ldnull
      IL_00ac:  stfld      class [runtime]System.Tuple`2<string,bool> assembly/'Pipe #4 input at line 44@46'::current
      IL_00b1:  ldc.i4.0
      IL_00b2:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<int32> assembly/'Pipe #4 input at line 44@46'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<int32>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [runtime]System.Tuple`2<string,bool> assembly/'Pipe #4 input at line 44@46'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #4 input at line 44@46'::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,bool> get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Tuple`2<string,bool> assembly/'Pipe #4 input at line 44@46'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`2<string,bool>> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/'Pipe #4 input at line 44@46'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                              int32,
                                                                                              class [runtime]System.Tuple`2<string,bool>)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #5 input at line 51@52-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>
  {
    .field static assembly initonly class assembly/'Pipe #5 input at line 51@52-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product> Invoke(class [Utils]Utils/Product _arg1) cil managed
    {
      
      .maxstack  5
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  tail.
      IL_0005:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<class [Utils]Utils/Product>(!!0)
      IL_000a:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #5 input at line 51@52-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #5 input at line 51@52-1' assembly/'Pipe #5 input at line 51@52-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto autochar serializable sealed nested assembly beforefieldinit specialname 'Pipe #5 input at line 51@53'
         extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>>
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 06 00 00 00 00 00 ) 
    .field public class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum'
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 pc
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> current
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> 'enum',
                                 int32 pc,
                                 class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> current) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_0007:  ldarg.0
      IL_0008:  ldarg.2
      IL_0009:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_000e:  ldarg.0
      IL_000f:  ldarg.3
      IL_0010:  stfld      class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> assembly/'Pipe #5 input at line 51@53'::current
      IL_0015:  ldarg.0
      IL_0016:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>>::.ctor()
      IL_001b:  ret
    } 

    .method public strict virtual instance int32 GenerateNext(class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>>& next) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Product V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_0006:  ldc.i4.1
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_001b,
                            IL_001e,
                            IL_0021)
      IL_0019:  br.s       IL_0027

      IL_001b:  nop
      IL_001c:  br.s       IL_008a

      IL_001e:  nop
      IL_001f:  br.s       IL_007d

      IL_0021:  nop
      IL_0022:  br         IL_00ab

      IL_0027:  nop
      IL_0028:  ldarg.0
      IL_0029:  ldsfld     class assembly/'Pipe #5 input at line 51@52-1' assembly/'Pipe #5 input at line 51@52-1'::@_instance
      IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
      IL_0033:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!2> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Collect<class [Utils]Utils/Product,class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>,class [Utils]Utils/Product>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0038:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerator`1<!0> class [runtime]System.Collections.Generic.IEnumerable`1<class [Utils]Utils/Product>::GetEnumerator()
      IL_003d:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_0042:  ldarg.0
      IL_0043:  ldc.i4.1
      IL_0044:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_0049:  br.s       IL_007d

      IL_004b:  ldarg.0
      IL_004c:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_0051:  callvirt   instance !0 class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>::get_Current()
      IL_0056:  stloc.0
      IL_0057:  ldarg.0
      IL_0058:  ldc.i4.2
      IL_0059:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_005e:  ldarg.0
      IL_005f:  ldloc.0
      IL_0060:  callvirt   instance string [Utils]Utils/Product::get_ProductName()
      IL_0065:  ldloc.0
      IL_0066:  callvirt   instance string [Utils]Utils/Product::get_Category()
      IL_006b:  ldloc.0
      IL_006c:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Product::get_UnitPrice()
      IL_0071:  newobj     instance void class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                 !1,
                                                                                                                                 !2)
      IL_0076:  stfld      class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> assembly/'Pipe #5 input at line 51@53'::current
      IL_007b:  ldc.i4.1
      IL_007c:  ret

      IL_007d:  ldarg.0
      IL_007e:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_0083:  callvirt   instance bool [runtime]System.Collections.IEnumerator::MoveNext()
      IL_0088:  brtrue.s   IL_004b

      IL_008a:  ldarg.0
      IL_008b:  ldc.i4.3
      IL_008c:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_0091:  ldarg.0
      IL_0092:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_0097:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
      IL_009c:  nop
      IL_009d:  ldarg.0
      IL_009e:  ldnull
      IL_009f:  stfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
      IL_00a4:  ldarg.0
      IL_00a5:  ldc.i4.3
      IL_00a6:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_00ab:  ldarg.0
      IL_00ac:  ldnull
      IL_00ad:  stfld      class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> assembly/'Pipe #5 input at line 51@53'::current
      IL_00b2:  ldc.i4.0
      IL_00b3:  ret
    } 

    .method public strict virtual instance void Close() cil managed
    {
      
      .maxstack  6
      .locals init (class [runtime]System.Exception V_0,
               class [runtime]System.Exception V_1)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
      IL_0006:  ldc.i4.3
      IL_0007:  sub
      IL_0008:  switch     ( 
                            IL_0013)
      IL_0011:  br.s       IL_0016

      IL_0013:  nop
      IL_0014:  br.s       IL_0076

      IL_0016:  nop
      .try
      {
        IL_0017:  ldarg.0
        IL_0018:  ldfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
        IL_001d:  switch     ( 
                              IL_0034,
                              IL_0037,
                              IL_003a,
                              IL_003d)
        IL_0032:  br.s       IL_0040

        IL_0034:  nop
        IL_0035:  br.s       IL_0056

        IL_0037:  nop
        IL_0038:  br.s       IL_0042

        IL_003a:  nop
        IL_003b:  br.s       IL_0041

        IL_003d:  nop
        IL_003e:  br.s       IL_0056

        IL_0040:  nop
        IL_0041:  nop
        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.3
        IL_0044:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
        IL_0049:  ldarg.0
        IL_004a:  ldfld      class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product> assembly/'Pipe #5 input at line 51@53'::'enum'
        IL_004f:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::Dispose<class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>>(!!0)
        IL_0054:  nop
        IL_0055:  nop
        IL_0056:  ldarg.0
        IL_0057:  ldc.i4.3
        IL_0058:  stfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
        IL_005d:  ldarg.0
        IL_005e:  ldnull
        IL_005f:  stfld      class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> assembly/'Pipe #5 input at line 51@53'::current
        IL_0064:  leave.s    IL_0070

      }  
      catch [runtime]System.Object 
      {
        IL_0066:  castclass  [runtime]System.Exception
        IL_006b:  stloc.1
        IL_006c:  ldloc.1
        IL_006d:  stloc.0
        IL_006e:  leave.s    IL_0070

      }  
      IL_0070:  nop
      IL_0071:  br         IL_0000

      IL_0076:  ldloc.0
      IL_0077:  brfalse.s  IL_007b

      IL_0079:  ldloc.0
      IL_007a:  throw

      IL_007b:  ret
    } 

    .method public strict virtual instance bool get_CheckClose() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 assembly/'Pipe #5 input at line 51@53'::pc
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
      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldc.i4.1
      IL_002d:  ret

      IL_002e:  ldc.i4.0
      IL_002f:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> get_LastGenerated() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal> assembly/'Pipe #5 input at line 51@53'::current
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerator`1<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>> GetFreshEnumerator() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ldc.i4.0
      IL_0002:  ldnull
      IL_0003:  newobj     instance void assembly/'Pipe #5 input at line 51@53'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                              int32,
                                                                                              class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>)
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@60'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 59@60'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<int32,object> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #6 input at line 59@60'::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<int32,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@61-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,bool>
  {
    .field static assembly initonly class assembly/'Pipe #6 input at line 59@61-1' @_instance
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
      IL_0001:  ldc.i4.5
      IL_0002:  clt
      IL_0004:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #6 input at line 59@61-1'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #6 input at line 59@61-1' assembly/'Pipe #6 input at line 59@61-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #6 input at line 59@62-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>
  {
    .field static assembly initonly class assembly/'Pipe #6 input at line 59@62-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,string>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance string Invoke(int32 n) cil managed
    {
      
      .maxstack  8
      IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_digits()
      IL_0005:  ldarg.1
      IL_0006:  tail.
      IL_0008:  callvirt   instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Item(int32)
      IL_000d:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #6 input at line 59@62-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #6 input at line 59@62-2' assembly/'Pipe #6 input at line 59@62-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@73-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 a
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 int32 a) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@73-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      int32 assembly/'Pipe #7 input at line 71@73-1'::a
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,object> Invoke(int32 _arg2) cil managed
    {
      
      .maxstack  7
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@73-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      int32 assembly/'Pipe #7 input at line 71@73-1'::a
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<int32,int32>,object>(!!0)
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@72'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@72'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable> Invoke(int32 _arg1) cil managed
    {
      
      .maxstack  8
      .locals init (int32 V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@72'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@72'::builder@
      IL_000e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbersB()
      IL_0013:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ldarg.0
      IL_0019:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #7 input at line 71@72'::builder@
      IL_001e:  ldloc.0
      IL_001f:  newobj     instance void assembly/'Pipe #7 input at line 71@73-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                int32)
      IL_0024:  tail.
      IL_0026:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,int32>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@74-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<int32,int32>,bool>
  {
    .field static assembly initonly class assembly/'Pipe #7 input at line 71@74-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<int32,int32>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [runtime]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  clt
      IL_0012:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #7 input at line 71@74-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #7 input at line 71@74-2' assembly/'Pipe #7 input at line 71@74-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #7 input at line 71@75-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Tuple`2<int32,int32>>
  {
    .field static assembly initonly class assembly/'Pipe #7 input at line 71@75-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Tuple`2<int32,int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<int32,int32> Invoke(class [runtime]System.Tuple`2<int32,int32> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<int32,int32>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<int32,int32>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  ldloc.1
      IL_0010:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0015:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #7 input at line 71@75-3'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #7 input at line 71@75-3' assembly/'Pipe #7 input at line 71@75-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@83-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@83-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer assembly/'Pipe #8 input at line 81@83-1'::c
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@83-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer assembly/'Pipe #8 input at line 81@83-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@82'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@82'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable> Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@82'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@82'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #8 input at line 81@82'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void assembly/'Pipe #8 input at line 81@83-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@84-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class assembly/'Pipe #8 input at line 81@84-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  10
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0xc350
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.2
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_LessThan(valuetype [netstandard]System.Decimal,
                                                                         valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #8 input at line 81@84-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #8 input at line 81@84-2' assembly/'Pipe #8 input at line 81@84-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #8 input at line 81@85-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>
  {
    .field static assembly initonly class assembly/'Pipe #8 input at line 81@85-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal> Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                !1,
                                                                                                                                !2)
      IL_0025:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #8 input at line 81@85-3'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #8 input at line 81@85-3' assembly/'Pipe #8 input at line 81@85-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@92-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@92-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer assembly/'Pipe #9 input at line 90@92-1'::c
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@92-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer assembly/'Pipe #9 input at line 90@92-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@91'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@91'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable> Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@91'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@91'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'Pipe #9 input at line 90@91'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void assembly/'Pipe #9 input at line 90@92-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@93-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class assembly/'Pipe #9 input at line 90@93-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1,
               valuetype [runtime]System.DateTime V_2,
               valuetype [runtime]System.DateTime V_3)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  ldc.i4     0x7ce
      IL_001a:  ldc.i4.1
      IL_001b:  ldc.i4.1
      IL_001c:  newobj     instance void [runtime]System.DateTime::.ctor(int32,
                                                                          int32,
                                                                          int32)
      IL_0021:  stloc.3
      IL_0022:  ldloc.2
      IL_0023:  ldloc.3
      IL_0024:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0029:  ldc.i4.0
      IL_002a:  clt
      IL_002c:  ldc.i4.0
      IL_002d:  ceq
      IL_002f:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #9 input at line 90@93-2'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #9 input at line 90@93-2' assembly/'Pipe #9 input at line 90@93-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'Pipe #9 input at line 90@94-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>>
  {
    .field static assembly initonly class assembly/'Pipe #9 input at line 90@94-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime> Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0020:  newobj     instance void class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>::.ctor(!0,
                                                                                                                                 !1,
                                                                                                                                 !2)
      IL_0025:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'Pipe #9 input at line 90@94-3'::.ctor()
      IL_0005:  stsfld     class assembly/'Pipe #9 input at line 90@94-3' assembly/'Pipe #9 input at line 90@94-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@101-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders3@101-1'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer assembly/'orders3@101-1'::c
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> Invoke(class [Utils]Utils/Order _arg2) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders3@101-1'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer assembly/'orders3@101-1'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit orders3@100
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders3@100::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable> Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders3@100::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders3@100::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders3@100::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void assembly/'orders3@101-1'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@102-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class assembly/'orders3@102-2' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  10
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0014:  ldc.i4     0x4e20
      IL_0019:  ldc.i4.0
      IL_001a:  ldc.i4.0
      IL_001b:  ldc.i4.0
      IL_001c:  ldc.i4.1
      IL_001d:  newobj     instance void [netstandard]System.Decimal::.ctor(int32,
                                                                            int32,
                                                                            int32,
                                                                            bool,
                                                                            uint8)
      IL_0022:  call       bool [netstandard]System.Decimal::op_GreaterThanOrEqual(valuetype [netstandard]System.Decimal,
                                                                                   valuetype [netstandard]System.Decimal)
      IL_0027:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'orders3@102-2'::.ctor()
      IL_0005:  stsfld     class assembly/'orders3@102-2' assembly/'orders3@102-2'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders3@103-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>
  {
    .field static assembly initonly class assembly/'orders3@103-3' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal> Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  ldloc.1
      IL_001b:  callvirt   instance valuetype [runtime]System.Decimal [Utils]Utils/Order::get_Total()
      IL_0020:  newobj     instance void class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>::.ctor(!0,
                                                                                                                                !1,
                                                                                                                                !2)
      IL_0025:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'orders3@103-3'::.ctor()
      IL_0005:  stsfld     class assembly/'orders3@103-3' assembly/'orders3@103-3'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit orders4@111
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders4@111::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [Utils]Utils/Customer,object> Invoke(class [Utils]Utils/Customer _arg1) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/orders4@111::builder@
      IL_0008:  ldloc.0
      IL_0009:  tail.
      IL_000b:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [Utils]Utils/Customer,object>(!!0)
      IL_0010:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@112-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>
  {
    .field static assembly initonly class assembly/'orders4@112-1' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [Utils]Utils/Customer c) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [Utils]Utils/Customer::get_Region()
      IL_0006:  ldstr      "WA"
      IL_000b:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0010:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'orders4@112-1'::.ctor()
      IL_0005:  stsfld     class assembly/'orders4@112-1' assembly/'orders4@112-1'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@113-3'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [Utils]Utils/Customer c
    .method assembly specialname rtspecialname 
            instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@,
                                 class [Utils]Utils/Customer c) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Order,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@113-3'::builder@
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      class [Utils]Utils/Customer assembly/'orders4@113-3'::c
      IL_0014:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object> Invoke(class [Utils]Utils/Order _arg3) cil managed
    {
      
      .maxstack  7
      .locals init (class [Utils]Utils/Order V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@113-3'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [Utils]Utils/Customer assembly/'orders4@113-3'::c
      IL_000e:  ldloc.0
      IL_000f:  newobj     instance void class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::.ctor(!0,
                                                                                                                                     !1)
      IL_0014:  tail.
      IL_0016:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Yield<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(!!0)
      IL_001b:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@111-2'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>
  {
    .field public class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@
    .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder builder@) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [Utils]Utils/Customer,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@111-2'::builder@
      IL_000d:  ret
    } 

    .method public strict virtual instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable> Invoke(class [Utils]Utils/Customer _arg2) cil managed
    {
      
      .maxstack  8
      .locals init (class [Utils]Utils/Customer V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@111-2'::builder@
      IL_0008:  ldarg.0
      IL_0009:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@111-2'::builder@
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance class [Utils]Utils/Order[] [Utils]Utils/Customer::get_Orders()
      IL_0014:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Order>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0019:  ldarg.0
      IL_001a:  ldfld      class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder assembly/'orders4@111-2'::builder@
      IL_001f:  ldloc.0
      IL_0020:  newobj     instance void assembly/'orders4@113-3'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder,
                                                                                class [Utils]Utils/Customer)
      IL_0025:  tail.
      IL_0027:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Order,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
      IL_002c:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@114-4'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>
  {
    .field static assembly initonly class assembly/'orders4@114-4' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,bool>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance bool Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1,
               valuetype [runtime]System.DateTime V_2,
               valuetype [runtime]System.DateTime V_3)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  callvirt   instance valuetype [runtime]System.DateTime [Utils]Utils/Order::get_OrderDate()
      IL_0014:  stloc.2
      IL_0015:  call       valuetype [runtime]System.DateTime assembly::get_cutOffDate()
      IL_001a:  stloc.3
      IL_001b:  ldloc.2
      IL_001c:  ldloc.3
      IL_001d:  call       int32 [netstandard]System.DateTime::Compare(valuetype [netstandard]System.DateTime,
                                                                       valuetype [netstandard]System.DateTime)
      IL_0022:  ldc.i4.0
      IL_0023:  clt
      IL_0025:  ldc.i4.0
      IL_0026:  ceq
      IL_0028:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'orders4@114-4'::.ctor()
      IL_0005:  stsfld     class assembly/'orders4@114-4' assembly/'orders4@114-4'::@_instance
      IL_000a:  ret
    } 

  } 

  .class auto ansi serializable sealed nested assembly beforefieldinit 'orders4@115-5'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`2<string,int32>>
  {
    .field static assembly initonly class assembly/'orders4@115-5' @_instance
    .method assembly specialname rtspecialname instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Tuple`2<string,int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Tuple`2<string,int32> Invoke(class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order> tupledArg) cil managed
    {
      
      .maxstack  6
      .locals init (class [Utils]Utils/Customer V_0,
               class [Utils]Utils/Order V_1)
      IL_0000:  ldarg.1
      IL_0001:  call       instance !0 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item1()
      IL_0006:  stloc.0
      IL_0007:  ldarg.1
      IL_0008:  call       instance !1 class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>::get_Item2()
      IL_000d:  stloc.1
      IL_000e:  ldloc.0
      IL_000f:  callvirt   instance string [Utils]Utils/Customer::get_CustomerID()
      IL_0014:  ldloc.1
      IL_0015:  callvirt   instance int32 [Utils]Utils/Order::get_OrderID()
      IL_001a:  newobj     instance void class [runtime]System.Tuple`2<string,int32>::.ctor(!0,
                                                                                             !1)
      IL_001f:  ret
    } 

    .method private specialname rtspecialname static void  .cctor() cil managed
    {
      
      .maxstack  10
      IL_0000:  newobj     instance void assembly/'orders4@115-5'::.ctor()
      IL_0005:  stsfld     class assembly/'orders4@115-5' assembly/'orders4@115-5'::@_instance
      IL_000a:  ret
    } 

  } 

  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbers@7
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numsPlusOne@10
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> products@17
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<string> productNames@19
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> strings@26
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> textNums@27
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> words@34
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<string,string>[] upperLowerWords@36
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>> digitOddEvens@43
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[] productInfos@50
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> digits@57
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> lowNums@58
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersA@67
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> numbersB@68
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`2<int32,int32>[] pairs@70
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> customers@79
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[] orders@80
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[] orders2@89
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> orders3@98
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly valuetype [runtime]System.DateTime cutOffDate@107
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field static assembly class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> orders4@109
  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numbers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@7
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numsPlusOne() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numsPlusOne@10
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> get_products() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@17
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<string> get_productNames() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<string> assembly::productNames@19
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_strings() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::strings@26
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_textNums() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::textNums@27
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_words() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@34
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<string,string>[] get_upperLowerWords() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<string,string>[] assembly::upperLowerWords@36
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>> get_digitOddEvens() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>> assembly::digitOddEvens@43
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[] get_productInfos() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[] assembly::productInfos@50
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_digits() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::digits@57
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> get_lowNums() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::lowNums@58
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numbersA() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbersA@67
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> get_numbersB() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbersB@68
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`2<int32,int32>[] get_pairs() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::pairs@70
    IL_0005:  ret
  } 

  .method public specialname static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> get_customers() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::customers@79
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[] get_orders() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[] assembly::orders@80
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[] get_orders2() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[] assembly::orders2@89
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> get_orders3() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> assembly::orders3@98
    IL_0005:  ret
  } 

  .method public specialname static valuetype [runtime]System.DateTime get_cutOffDate() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     valuetype [runtime]System.DateTime assembly::cutOffDate@107
    IL_0005:  ret
  } 

  .method public specialname static class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> get_orders4() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> assembly::orders4@109
    IL_0005:  ret
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
    
    .maxstack  13
    .locals init (class [runtime]System.Collections.Generic.IEnumerable`1<int32> V_0,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_1,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_2,
             class [runtime]System.Collections.Generic.IEnumerable`1<string> V_3,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_4,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,string>> V_5,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_6,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,bool>> V_7,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_8,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>> V_9,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_10,
             class [runtime]System.Collections.Generic.IEnumerable`1<string> V_11,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_12,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_13,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> V_14,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<int32,int32>> V_15,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_16,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> V_17,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_18,
             class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>> V_19,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_20,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_21,
             class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder V_22)
    IL_0000:  ldc.i4.5
    IL_0001:  ldc.i4.4
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.3
    IL_0004:  ldc.i4.s   9
    IL_0006:  ldc.i4.8
    IL_0007:  ldc.i4.6
    IL_0008:  ldc.i4.7
    IL_0009:  ldc.i4.2
    IL_000a:  ldc.i4.0
    IL_000b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0024:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0029:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_002e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0033:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0038:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_003d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0042:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbers@7
    IL_0047:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_004c:  stloc.1
    IL_004d:  ldnull
    IL_004e:  ldc.i4.0
    IL_004f:  ldc.i4.0
    IL_0050:  newobj     instance void assembly/'Pipe #1 input at line 11@13'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            int32)
    IL_0055:  stloc.0
    IL_0056:  ldloc.0
    IL_0057:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_005c:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numsPlusOne@10
    IL_0061:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> [Utils]Utils::getProductList()
    IL_0066:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::products@17
    IL_006b:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0070:  stloc.2
    IL_0071:  ldnull
    IL_0072:  ldc.i4.0
    IL_0073:  ldnull
    IL_0074:  newobj     instance void assembly/productNames@22::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                              int32,
                                                                              string)
    IL_0079:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<string> assembly::productNames@19
    IL_007e:  ldstr      "zero"
    IL_0083:  ldstr      "one"
    IL_0088:  ldstr      "two"
    IL_008d:  ldstr      "three"
    IL_0092:  ldstr      "four"
    IL_0097:  ldstr      "five"
    IL_009c:  ldstr      "six"
    IL_00a1:  ldstr      "seven"
    IL_00a6:  ldstr      "eight"
    IL_00ab:  ldstr      "nine"
    IL_00b0:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_00b5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ba:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00bf:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00c4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00c9:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00ce:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d3:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00d8:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_00e7:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::strings@26
    IL_00ec:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_00f1:  stloc.s    V_4
    IL_00f3:  ldnull
    IL_00f4:  ldc.i4.0
    IL_00f5:  ldnull
    IL_00f6:  newobj     instance void assembly/'Pipe #2 input at line 28@30'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            string)
    IL_00fb:  stloc.3
    IL_00fc:  ldloc.3
    IL_00fd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0102:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::textNums@27
    IL_0107:  ldstr      "aPPLE"
    IL_010c:  ldstr      "BlUeBeRrY"
    IL_0111:  ldstr      "cHeRry"
    IL_0116:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_011b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0120:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0125:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_012a:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::words@34
    IL_012f:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0134:  stloc.s    V_6
    IL_0136:  ldnull
    IL_0137:  ldc.i4.0
    IL_0138:  ldnull
    IL_0139:  newobj     instance void assembly/'Pipe #3 input at line 37@39'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<string>,
                                                                                            int32,
                                                                                            class [runtime]System.Tuple`2<string,string>)
    IL_013e:  stloc.s    V_5
    IL_0140:  ldloc.s    V_5
    IL_0142:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<string,string>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0147:  stsfld     class [runtime]System.Tuple`2<string,string>[] assembly::upperLowerWords@36
    IL_014c:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0151:  stloc.s    V_8
    IL_0153:  ldnull
    IL_0154:  ldc.i4.0
    IL_0155:  ldnull
    IL_0156:  newobj     instance void assembly/'Pipe #4 input at line 44@46'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<int32>,
                                                                                            int32,
                                                                                            class [runtime]System.Tuple`2<string,bool>)
    IL_015b:  stloc.s    V_7
    IL_015d:  ldloc.s    V_7
    IL_015f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<class [runtime]System.Tuple`2<string,bool>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0164:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>> assembly::digitOddEvens@43
    IL_0169:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_016e:  stloc.s    V_10
    IL_0170:  ldnull
    IL_0171:  ldc.i4.0
    IL_0172:  ldnull
    IL_0173:  newobj     instance void assembly/'Pipe #5 input at line 51@53'::.ctor(class [runtime]System.Collections.Generic.IEnumerator`1<class [Utils]Utils/Product>,
                                                                                            int32,
                                                                                            class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>)
    IL_0178:  stloc.s    V_9
    IL_017a:  ldloc.s    V_9
    IL_017c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0181:  stsfld     class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[] assembly::productInfos@50
    IL_0186:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_strings()
    IL_018b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::digits@57
    IL_0190:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0195:  stloc.s    V_12
    IL_0197:  ldloc.s    V_12
    IL_0199:  ldloc.s    V_12
    IL_019b:  ldloc.s    V_12
    IL_019d:  ldloc.s    V_12
    IL_019f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
    IL_01a4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01a9:  ldloc.s    V_12
    IL_01ab:  newobj     instance void assembly/'Pipe #6 input at line 59@60'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_01b0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [runtime]System.Collections.IEnumerable,int32,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                          class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_01b5:  ldsfld     class assembly/'Pipe #6 input at line 59@61-1' assembly/'Pipe #6 input at line 59@61-1'::@_instance
    IL_01ba:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<int32,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_01bf:  ldsfld     class assembly/'Pipe #6 input at line 59@62-2' assembly/'Pipe #6 input at line 59@62-2'::@_instance
    IL_01c4:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<int32,class [runtime]System.Collections.IEnumerable,string>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_01c9:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<string,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_01ce:  stloc.s    V_11
    IL_01d0:  ldloc.s    V_11
    IL_01d2:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToList<string>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_01d7:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::lowNums@58
    IL_01dc:  nop
    IL_01dd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_lowNums()
    IL_01e2:  stloc.s    V_13
    IL_01e4:  ldstr      "four"
    IL_01e9:  ldstr      "one"
    IL_01ee:  ldstr      "three"
    IL_01f3:  ldstr      "two"
    IL_01f8:  ldstr      "zero"
    IL_01fd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::get_Empty()
    IL_0202:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0207:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_020c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0211:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0216:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Cons(!0,
                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_021b:  stloc.s    V_14
    IL_021d:  ldloc.s    V_13
    IL_021f:  ldloc.s    V_14
    IL_0221:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0226:  callvirt   instance bool class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>::Equals(class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>,
                                                                                                                    class [runtime]System.Collections.IEqualityComparer)
    IL_022b:  ldc.i4.0
    IL_022c:  ceq
    IL_022e:  brfalse.s  IL_024a

    IL_0230:  ldstr      "lowNums failed"
    IL_0235:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_023a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_023f:  pop
    IL_0240:  ldc.i4.1
    IL_0241:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0246:  pop
    IL_0247:  nop
    IL_0248:  br.s       IL_024b

    IL_024a:  nop
    IL_024b:  ldc.i4.0
    IL_024c:  ldc.i4.2
    IL_024d:  ldc.i4.4
    IL_024e:  ldc.i4.5
    IL_024f:  ldc.i4.6
    IL_0250:  ldc.i4.8
    IL_0251:  ldc.i4.s   9
    IL_0253:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_0258:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_025d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0262:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0267:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_026c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0271:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0276:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_027b:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbersA@67
    IL_0280:  ldc.i4.1
    IL_0281:  ldc.i4.3
    IL_0282:  ldc.i4.5
    IL_0283:  ldc.i4.7
    IL_0284:  ldc.i4.8
    IL_0285:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
    IL_028a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_028f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0294:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_0299:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_029e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
    IL_02a3:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::numbersB@68
    IL_02a8:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_02ad:  stloc.s    V_16
    IL_02af:  ldloc.s    V_16
    IL_02b1:  ldloc.s    V_16
    IL_02b3:  ldloc.s    V_16
    IL_02b5:  ldloc.s    V_16
    IL_02b7:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbersA()
    IL_02bc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02c1:  ldloc.s    V_16
    IL_02c3:  newobj     instance void assembly/'Pipe #7 input at line 71@72'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_02c8:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<int32,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_02cd:  ldsfld     class assembly/'Pipe #7 input at line 71@74-2' assembly/'Pipe #7 input at line 71@74-2'::@_instance
    IL_02d2:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_02d7:  ldsfld     class assembly/'Pipe #7 input at line 71@75-3' assembly/'Pipe #7 input at line 71@75-3'::@_instance
    IL_02dc:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<int32,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                  class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_02e1:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<int32,int32>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_02e6:  stloc.s    V_15
    IL_02e8:  ldloc.s    V_15
    IL_02ea:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`2<int32,int32>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_02ef:  stsfld     class [runtime]System.Tuple`2<int32,int32>[] assembly::pairs@70
    IL_02f4:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> [Utils]Utils::getCustomerList()
    IL_02f9:  stsfld     class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::customers@79
    IL_02fe:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_0303:  stloc.s    V_18
    IL_0305:  ldloc.s    V_18
    IL_0307:  ldloc.s    V_18
    IL_0309:  ldloc.s    V_18
    IL_030b:  ldloc.s    V_18
    IL_030d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
    IL_0312:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0317:  ldloc.s    V_18
    IL_0319:  newobj     instance void assembly/'Pipe #8 input at line 81@82'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_031e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0323:  ldsfld     class assembly/'Pipe #8 input at line 81@84-2' assembly/'Pipe #8 input at line 81@84-2'::@_instance
    IL_0328:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_032d:  ldsfld     class assembly/'Pipe #8 input at line 81@85-3' assembly/'Pipe #8 input at line 81@85-3'::@_instance
    IL_0332:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0337:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_033c:  stloc.s    V_17
    IL_033e:  ldloc.s    V_17
    IL_0340:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0345:  stsfld     class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[] assembly::orders@80
    IL_034a:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_034f:  stloc.s    V_20
    IL_0351:  ldloc.s    V_20
    IL_0353:  ldloc.s    V_20
    IL_0355:  ldloc.s    V_20
    IL_0357:  ldloc.s    V_20
    IL_0359:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
    IL_035e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0363:  ldloc.s    V_20
    IL_0365:  newobj     instance void assembly/'Pipe #9 input at line 90@91'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_036a:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_036f:  ldsfld     class assembly/'Pipe #9 input at line 90@93-2' assembly/'Pipe #9 input at line 90@93-2'::@_instance
    IL_0374:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0379:  ldsfld     class assembly/'Pipe #9 input at line 90@94-3' assembly/'Pipe #9 input at line 90@94-3'::@_instance
    IL_037e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                                class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_0383:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0388:  stloc.s    V_19
    IL_038a:  ldloc.s    V_19
    IL_038c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0391:  stsfld     class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[] assembly::orders2@89
    IL_0396:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_039b:  stloc.s    V_21
    IL_039d:  ldloc.s    V_21
    IL_039f:  ldloc.s    V_21
    IL_03a1:  ldloc.s    V_21
    IL_03a3:  ldloc.s    V_21
    IL_03a5:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
    IL_03aa:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_03af:  ldloc.s    V_21
    IL_03b1:  newobj     instance void assembly/orders3@100::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_03b6:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_03bb:  ldsfld     class assembly/'orders3@102-2' assembly/'orders3@102-2'::@_instance
    IL_03c0:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_03c5:  ldsfld     class assembly/'orders3@103-3' assembly/'orders3@103-3'::@_instance
    IL_03ca:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                               class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_03cf:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_03d4:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> assembly::orders3@98
    IL_03d9:  ldc.i4     0x7cd
    IL_03de:  ldc.i4.1
    IL_03df:  ldc.i4.1
    IL_03e0:  newobj     instance void [runtime]System.DateTime::.ctor(int32,
                                                                        int32,
                                                                        int32)
    IL_03e5:  stsfld     valuetype [runtime]System.DateTime assembly::cutOffDate@107
    IL_03ea:  call       class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::get_query()
    IL_03ef:  stloc.s    V_22
    IL_03f1:  ldloc.s    V_22
    IL_03f3:  ldloc.s    V_22
    IL_03f5:  ldloc.s    V_22
    IL_03f7:  ldloc.s    V_22
    IL_03f9:  ldloc.s    V_22
    IL_03fb:  ldloc.s    V_22
    IL_03fd:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
    IL_0402:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,class [runtime]System.Collections.IEnumerable> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Source<class [Utils]Utils/Customer>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0407:  ldloc.s    V_22
    IL_0409:  newobj     instance void assembly/orders4@111::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_040e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [Utils]Utils/Customer,object>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                      class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0413:  ldsfld     class assembly/'orders4@112-1' assembly/'orders4@112-1'::@_instance
    IL_0418:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                     class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_041d:  ldloc.s    V_22
    IL_041f:  newobj     instance void assembly/'orders4@111-2'::.ctor(class [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder)
    IL_0424:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::For<class [Utils]Utils/Customer,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                                                       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!3>>)
    IL_0429:  ldsfld     class assembly/'orders4@114-4' assembly/'orders4@114-4'::@_instance
    IL_042e:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Where<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                              class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,bool>)
    IL_0433:  ldsfld     class assembly/'orders4@115-5' assembly/'orders4@115-5'::@_instance
    IL_0438:  callvirt   instance class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!2,!!1> [FSharp.Core]Microsoft.FSharp.Linq.QueryBuilder::Select<class [runtime]System.Tuple`2<class [Utils]Utils/Customer,class [Utils]Utils/Order>,class [runtime]System.Collections.IEnumerable,class [runtime]System.Tuple`2<string,int32>>(class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                                                                            class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,!!2>)
    IL_043d:  callvirt   instance class [runtime]System.Collections.Generic.IEnumerable`1<!0> class [FSharp.Core]Microsoft.FSharp.Linq.QuerySource`2<class [runtime]System.Tuple`2<string,int32>,class [runtime]System.Collections.IEnumerable>::get_Source()
    IL_0442:  stsfld     class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> assembly::orders4@109
    IL_0447:  ret
  } 

  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbers()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numsPlusOne()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numsPlusOne()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product>
          products()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Product> assembly::get_products()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerable`1<string>
          productNames()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<string> assembly::get_productNames()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          strings()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_strings()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          textNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_textNums()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          words()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_words()
  } 
  .property class [runtime]System.Tuple`2<string,string>[]
          upperLowerWords()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<string,string>[] assembly::get_upperLowerWords()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>>
          digitOddEvens()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [runtime]System.Tuple`2<string,bool>> assembly::get_digitOddEvens()
  } 
  .property class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[]
          productInfos()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`3<string,string,valuetype [runtime]System.Decimal>[] assembly::get_productInfos()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          digits()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_digits()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string>
          lowNums()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<string> assembly::get_lowNums()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersA()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbersA()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>
          numbersB()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> assembly::get_numbersB()
  } 
  .property class [runtime]System.Tuple`2<int32,int32>[]
          pairs()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`2<int32,int32>[] assembly::get_pairs()
  } 
  .property class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer>
          customers()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<class [Utils]Utils/Customer> assembly::get_customers()
  } 
  .property class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[]
          orders()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>[] assembly::get_orders()
  } 
  .property class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[]
          orders2()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.DateTime>[] assembly::get_orders2()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>>
          orders3()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`3<string,int32,valuetype [runtime]System.Decimal>> assembly::get_orders3()
  } 
  .property valuetype [runtime]System.DateTime
          cutOffDate()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get valuetype [runtime]System.DateTime assembly::get_cutOffDate()
  } 
  .property class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>>
          orders4()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 ) 
    .get class [runtime]System.Collections.Generic.IEnumerable`1<class [runtime]System.Tuple`2<string,int32>> assembly::get_orders4()
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






