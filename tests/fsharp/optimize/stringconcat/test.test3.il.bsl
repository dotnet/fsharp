







.class public abstract auto ansi sealed Test.Test
       extends [mscorlib]System.Object
{
  .method public static string  test3() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newarr     [mscorlib]System.String
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.1
    IL_0009:  stloc.0
    IL_000a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_000f:  ldc.i4.1
    IL_0010:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0015:  ldstr      "_"
    IL_001a:  ldloca.s   V_0
    IL_001c:  constrained. [mscorlib]System.Int32
    IL_0022:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0027:  ldstr      "_"
    IL_002c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0031:  stelem     [mscorlib]System.String
    IL_0036:  dup
    IL_0037:  ldc.i4.1
    IL_0038:  ldc.i4.2
    IL_0039:  stloc.0
    IL_003a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_003f:  ldc.i4.2
    IL_0040:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0045:  ldstr      "_"
    IL_004a:  ldloca.s   V_0
    IL_004c:  constrained. [mscorlib]System.Int32
    IL_0052:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0057:  ldstr      "_"
    IL_005c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0061:  stelem     [mscorlib]System.String
    IL_0066:  dup
    IL_0067:  ldc.i4.2
    IL_0068:  ldc.i4.3
    IL_0069:  stloc.0
    IL_006a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_006f:  ldc.i4.3
    IL_0070:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0075:  ldstr      "_"
    IL_007a:  ldloca.s   V_0
    IL_007c:  constrained. [mscorlib]System.Int32
    IL_0082:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0087:  ldstr      "_"
    IL_008c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0091:  stelem     [mscorlib]System.String
    IL_0096:  dup
    IL_0097:  ldc.i4.3
    IL_0098:  ldc.i4.4
    IL_0099:  stloc.0
    IL_009a:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_009f:  ldc.i4.4
    IL_00a0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00a5:  ldstr      "_"
    IL_00aa:  ldloca.s   V_0
    IL_00ac:  constrained. [mscorlib]System.Int32
    IL_00b2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00b7:  ldstr      "_"
    IL_00bc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00c1:  stelem     [mscorlib]System.String
    IL_00c6:  dup
    IL_00c7:  ldc.i4.4
    IL_00c8:  ldc.i4.5
    IL_00c9:  stloc.0
    IL_00ca:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_00cf:  ldc.i4.5
    IL_00d0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00d5:  ldstr      "_"
    IL_00da:  ldloca.s   V_0
    IL_00dc:  constrained. [mscorlib]System.Int32
    IL_00e2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00e7:  ldstr      "_"
    IL_00ec:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00f1:  stelem     [mscorlib]System.String
    IL_00f6:  call       string [mscorlib]System.String::Concat(string[])
    IL_00fb:  ret
  } 

} 



