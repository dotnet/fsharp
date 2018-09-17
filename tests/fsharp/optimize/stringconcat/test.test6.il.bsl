







.class public abstract auto ansi sealed Test.Test
       extends [mscorlib]System.Object
{
  .method public static string  test6() cil managed
  {
    
    .maxstack  8
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldstr      "_"
    IL_0012:  ldloca.s   V_0
    IL_0014:  constrained. [mscorlib]System.Int32
    IL_001a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001f:  ldstr      "_"
    IL_0024:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0029:  ldc.i4.2
    IL_002a:  stloc.0
    IL_002b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0030:  ldc.i4.2
    IL_0031:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0036:  ldstr      "_"
    IL_003b:  ldloca.s   V_0
    IL_003d:  constrained. [mscorlib]System.Int32
    IL_0043:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0048:  ldstr      "_"
    IL_004d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0052:  ldc.i4.3
    IL_0053:  stloc.0
    IL_0054:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0059:  ldc.i4.3
    IL_005a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_005f:  ldstr      "_"
    IL_0064:  ldloca.s   V_0
    IL_0066:  constrained. [mscorlib]System.Int32
    IL_006c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0071:  ldstr      "_"
    IL_0076:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_007b:  ldc.i4.4
    IL_007c:  stloc.0
    IL_007d:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0082:  ldc.i4.4
    IL_0083:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0088:  ldstr      "_"
    IL_008d:  ldloca.s   V_0
    IL_008f:  constrained. [mscorlib]System.Int32
    IL_0095:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_009a:  ldstr      "_"
    IL_009f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00a4:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string,
                                                                string)
    IL_00a9:  ret
  } 

} 



