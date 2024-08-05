




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
  .class auto ansi serializable nested public A
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } 

  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

    .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
            F(class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x1,
              class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> x2) cil managed
    {
      .param [1]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      .param [2]
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.OptionalArgumentAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_2,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_3,
               int32 V_4,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_6,
               class [runtime]System.Collections.Generic.List`1<class assembly/A> V_7,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_9,
               class assembly/A V_10,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
               class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
               class assembly/A V_13)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  stloc.2
      IL_0004:  ldloc.2
      IL_0005:  brfalse.s  IL_0009

      IL_0007:  br.s       IL_000d

      IL_0009:  ldloc.0
      IL_000a:  nop
      IL_000b:  br.s       IL_0013

      IL_000d:  ldloc.2
      IL_000e:  stloc.3
      IL_000f:  ldloc.0
      IL_0010:  ldc.i4.1
      IL_0011:  add
      IL_0012:  nop
      IL_0013:  stloc.1
      IL_0014:  ldarg.1
      IL_0015:  stloc.s    V_5
      IL_0017:  ldloc.s    V_5
      IL_0019:  brfalse.s  IL_001d

      IL_001b:  br.s       IL_0021

      IL_001d:  ldloc.1
      IL_001e:  nop
      IL_001f:  br.s       IL_0029

      IL_0021:  ldloc.s    V_5
      IL_0023:  stloc.s    V_6
      IL_0025:  ldloc.1
      IL_0026:  ldc.i4.1
      IL_0027:  add
      IL_0028:  nop
      IL_0029:  stloc.s    V_4
      IL_002b:  ldloc.s    V_4
      IL_002d:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
      IL_0032:  stloc.s    V_7
      IL_0034:  ldarg.0
      IL_0035:  stloc.s    V_8
      IL_0037:  ldloc.s    V_8
      IL_0039:  brfalse.s  IL_003d

      IL_003b:  br.s       IL_0041

      IL_003d:  nop
      IL_003e:  nop
      IL_003f:  br.s       IL_0058

      IL_0041:  ldloc.s    V_8
      IL_0043:  stloc.s    V_9
      IL_0045:  ldloc.s    V_9
      IL_0047:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_004c:  stloc.s    V_10
      IL_004e:  ldloc.s    V_7
      IL_0050:  ldloc.s    V_10
      IL_0052:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_0057:  nop
      IL_0058:  ldarg.1
      IL_0059:  stloc.s    V_11
      IL_005b:  ldloc.s    V_11
      IL_005d:  brfalse.s  IL_0061

      IL_005f:  br.s       IL_0065

      IL_0061:  nop
      IL_0062:  nop
      IL_0063:  br.s       IL_007c

      IL_0065:  ldloc.s    V_11
      IL_0067:  stloc.s    V_12
      IL_0069:  ldloc.s    V_12
      IL_006b:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
      IL_0070:  stloc.s    V_13
      IL_0072:  ldloc.s    V_7
      IL_0074:  ldloc.s    V_13
      IL_0076:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
      IL_007b:  nop
      IL_007c:  ldloc.s    V_7
      IL_007e:  ret
    } 

  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_1,
             int32 V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
             int32 V_6,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_7,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_9,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_10,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_13)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldnull
    IL_0003:  stloc.1
    IL_0004:  ldc.i4.0
    IL_0005:  stloc.2
    IL_0006:  ldloc.0
    IL_0007:  stloc.s    V_4
    IL_0009:  ldloc.s    V_4
    IL_000b:  brfalse.s  IL_000f

    IL_000d:  br.s       IL_0013

    IL_000f:  ldloc.2
    IL_0010:  nop
    IL_0011:  br.s       IL_001b

    IL_0013:  ldloc.s    V_4
    IL_0015:  stloc.s    V_5
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.1
    IL_0019:  add
    IL_001a:  nop
    IL_001b:  stloc.3
    IL_001c:  ldloc.1
    IL_001d:  stloc.s    V_7
    IL_001f:  ldloc.s    V_7
    IL_0021:  brfalse.s  IL_0025

    IL_0023:  br.s       IL_0029

    IL_0025:  ldloc.3
    IL_0026:  nop
    IL_0027:  br.s       IL_0031

    IL_0029:  ldloc.s    V_7
    IL_002b:  stloc.s    V_8
    IL_002d:  ldloc.3
    IL_002e:  ldc.i4.1
    IL_002f:  add
    IL_0030:  nop
    IL_0031:  stloc.s    V_6
    IL_0033:  ldloc.s    V_6
    IL_0035:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_003a:  stloc.s    V_9
    IL_003c:  ldloc.0
    IL_003d:  stloc.s    V_10
    IL_003f:  ldloc.s    V_10
    IL_0041:  brfalse.s  IL_0045

    IL_0043:  br.s       IL_0048

    IL_0045:  nop
    IL_0046:  br.s       IL_005b

    IL_0048:  ldloc.s    V_10
    IL_004a:  stloc.s    V_11
    IL_004c:  ldloc.s    V_9
    IL_004e:  ldloc.s    V_11
    IL_0050:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_0055:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_005a:  nop
    IL_005b:  ldloc.1
    IL_005c:  stloc.s    V_12
    IL_005e:  ldloc.s    V_12
    IL_0060:  brfalse.s  IL_0064

    IL_0062:  br.s       IL_0067

    IL_0064:  nop
    IL_0065:  br.s       IL_007a

    IL_0067:  ldloc.s    V_12
    IL_0069:  stloc.s    V_13
    IL_006b:  ldloc.s    V_9
    IL_006d:  ldloc.s    V_13
    IL_006f:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_0074:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0079:  nop
    IL_007a:  ldloc.s    V_9
    IL_007c:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test2() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_1,
             int32 V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
             int32 V_6,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_7,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_9,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_10,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_13)
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000a:  stloc.0
    IL_000b:  ldnull
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.2
    IL_000f:  ldloc.0
    IL_0010:  stloc.s    V_4
    IL_0012:  ldloc.s    V_4
    IL_0014:  brfalse.s  IL_0018

    IL_0016:  br.s       IL_001c

    IL_0018:  ldloc.2
    IL_0019:  nop
    IL_001a:  br.s       IL_0024

    IL_001c:  ldloc.s    V_4
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  nop
    IL_0024:  stloc.3
    IL_0025:  ldloc.1
    IL_0026:  stloc.s    V_7
    IL_0028:  ldloc.s    V_7
    IL_002a:  brfalse.s  IL_002e

    IL_002c:  br.s       IL_0032

    IL_002e:  ldloc.3
    IL_002f:  nop
    IL_0030:  br.s       IL_003a

    IL_0032:  ldloc.s    V_7
    IL_0034:  stloc.s    V_8
    IL_0036:  ldloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  nop
    IL_003a:  stloc.s    V_6
    IL_003c:  ldloc.s    V_6
    IL_003e:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_0043:  stloc.s    V_9
    IL_0045:  ldloc.0
    IL_0046:  stloc.s    V_10
    IL_0048:  ldloc.s    V_10
    IL_004a:  brfalse.s  IL_004e

    IL_004c:  br.s       IL_0051

    IL_004e:  nop
    IL_004f:  br.s       IL_0064

    IL_0051:  ldloc.s    V_10
    IL_0053:  stloc.s    V_11
    IL_0055:  ldloc.s    V_9
    IL_0057:  ldloc.s    V_11
    IL_0059:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_005e:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0063:  nop
    IL_0064:  ldloc.1
    IL_0065:  stloc.s    V_12
    IL_0067:  ldloc.s    V_12
    IL_0069:  brfalse.s  IL_006d

    IL_006b:  br.s       IL_0070

    IL_006d:  nop
    IL_006e:  br.s       IL_0083

    IL_0070:  ldloc.s    V_12
    IL_0072:  stloc.s    V_13
    IL_0074:  ldloc.s    V_9
    IL_0076:  ldloc.s    V_13
    IL_0078:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_007d:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0082:  nop
    IL_0083:  ldloc.s    V_9
    IL_0085:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test3() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_1,
             int32 V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
             int32 V_6,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_7,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_9,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_10,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_13)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  newobj     instance void assembly/A::.ctor()
    IL_0007:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.2
    IL_000f:  ldloc.0
    IL_0010:  stloc.s    V_4
    IL_0012:  ldloc.s    V_4
    IL_0014:  brfalse.s  IL_0018

    IL_0016:  br.s       IL_001c

    IL_0018:  ldloc.2
    IL_0019:  nop
    IL_001a:  br.s       IL_0024

    IL_001c:  ldloc.s    V_4
    IL_001e:  stloc.s    V_5
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  nop
    IL_0024:  stloc.3
    IL_0025:  ldloc.1
    IL_0026:  stloc.s    V_7
    IL_0028:  ldloc.s    V_7
    IL_002a:  brfalse.s  IL_002e

    IL_002c:  br.s       IL_0032

    IL_002e:  ldloc.3
    IL_002f:  nop
    IL_0030:  br.s       IL_003a

    IL_0032:  ldloc.s    V_7
    IL_0034:  stloc.s    V_8
    IL_0036:  ldloc.3
    IL_0037:  ldc.i4.1
    IL_0038:  add
    IL_0039:  nop
    IL_003a:  stloc.s    V_6
    IL_003c:  ldloc.s    V_6
    IL_003e:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_0043:  stloc.s    V_9
    IL_0045:  ldloc.0
    IL_0046:  stloc.s    V_10
    IL_0048:  ldloc.s    V_10
    IL_004a:  brfalse.s  IL_004e

    IL_004c:  br.s       IL_0051

    IL_004e:  nop
    IL_004f:  br.s       IL_0064

    IL_0051:  ldloc.s    V_10
    IL_0053:  stloc.s    V_11
    IL_0055:  ldloc.s    V_9
    IL_0057:  ldloc.s    V_11
    IL_0059:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_005e:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0063:  nop
    IL_0064:  ldloc.1
    IL_0065:  stloc.s    V_12
    IL_0067:  ldloc.s    V_12
    IL_0069:  brfalse.s  IL_006d

    IL_006b:  br.s       IL_0070

    IL_006d:  nop
    IL_006e:  br.s       IL_0083

    IL_0070:  ldloc.s    V_12
    IL_0072:  stloc.s    V_13
    IL_0074:  ldloc.s    V_9
    IL_0076:  ldloc.s    V_13
    IL_0078:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_007d:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_0082:  nop
    IL_0083:  ldloc.s    V_9
    IL_0085:  ret
  } 

  .method public static class [runtime]System.Collections.Generic.List`1<class assembly/A> 
          test4() cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_1,
             int32 V_2,
             int32 V_3,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_4,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_5,
             int32 V_6,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_7,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_8,
             class [runtime]System.Collections.Generic.List`1<class assembly/A> V_9,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_10,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_11,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_12,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A> V_13)
    IL_0000:  newobj     instance void assembly/A::.ctor()
    IL_0005:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_000a:  stloc.0
    IL_000b:  newobj     instance void assembly/A::.ctor()
    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<!0> class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::Some(!0)
    IL_0015:  stloc.1
    IL_0016:  ldc.i4.0
    IL_0017:  stloc.2
    IL_0018:  ldloc.0
    IL_0019:  stloc.s    V_4
    IL_001b:  ldloc.s    V_4
    IL_001d:  brfalse.s  IL_0021

    IL_001f:  br.s       IL_0025

    IL_0021:  ldloc.2
    IL_0022:  nop
    IL_0023:  br.s       IL_002d

    IL_0025:  ldloc.s    V_4
    IL_0027:  stloc.s    V_5
    IL_0029:  ldloc.2
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  nop
    IL_002d:  stloc.3
    IL_002e:  ldloc.1
    IL_002f:  stloc.s    V_7
    IL_0031:  ldloc.s    V_7
    IL_0033:  brfalse.s  IL_0037

    IL_0035:  br.s       IL_003b

    IL_0037:  ldloc.3
    IL_0038:  nop
    IL_0039:  br.s       IL_0043

    IL_003b:  ldloc.s    V_7
    IL_003d:  stloc.s    V_8
    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  nop
    IL_0043:  stloc.s    V_6
    IL_0045:  ldloc.s    V_6
    IL_0047:  newobj     instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::.ctor(int32)
    IL_004c:  stloc.s    V_9
    IL_004e:  ldloc.0
    IL_004f:  stloc.s    V_10
    IL_0051:  ldloc.s    V_10
    IL_0053:  brfalse.s  IL_0057

    IL_0055:  br.s       IL_005a

    IL_0057:  nop
    IL_0058:  br.s       IL_006d

    IL_005a:  ldloc.s    V_10
    IL_005c:  stloc.s    V_11
    IL_005e:  ldloc.s    V_9
    IL_0060:  ldloc.s    V_11
    IL_0062:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_0067:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_006c:  nop
    IL_006d:  ldloc.1
    IL_006e:  stloc.s    V_12
    IL_0070:  ldloc.s    V_12
    IL_0072:  brfalse.s  IL_0076

    IL_0074:  br.s       IL_0079

    IL_0076:  nop
    IL_0077:  br.s       IL_008c

    IL_0079:  ldloc.s    V_12
    IL_007b:  stloc.s    V_13
    IL_007d:  ldloc.s    V_9
    IL_007f:  ldloc.s    V_13
    IL_0081:  call       instance !0 class [FSharp.Core]Microsoft.FSharp.Core.FSharpOption`1<class assembly/A>::get_Value()
    IL_0086:  callvirt   instance void class [runtime]System.Collections.Generic.List`1<class assembly/A>::Add(!0)
    IL_008b:  nop
    IL_008c:  ldloc.s    V_9
    IL_008e:  ret
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






