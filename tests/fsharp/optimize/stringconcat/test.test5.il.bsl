







.class public abstract auto ansi sealed Test.Test
       extends [mscorlib]System.Object
{
  .method public static string  test5() cil managed
  {
    
    .maxstack  9
    .locals init (int32 V_0,
             string V_1)
    IL_0000:  ldc.i4.s   12
    IL_0002:  newarr     [mscorlib]System.String
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.5
    IL_000a:  stloc.0
    IL_000b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0010:  ldc.i4.5
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0016:  ldstr      "_"
    IL_001b:  ldloca.s   V_0
    IL_001d:  constrained. [mscorlib]System.Int32
    IL_0023:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0028:  ldstr      "_"
    IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0032:  stelem     [mscorlib]System.String
    IL_0037:  dup
    IL_0038:  ldc.i4.1
    IL_0039:  ldc.i4.6
    IL_003a:  stloc.0
    IL_003b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0040:  ldc.i4.6
    IL_0041:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0046:  ldstr      "_"
    IL_004b:  ldloca.s   V_0
    IL_004d:  constrained. [mscorlib]System.Int32
    IL_0053:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0058:  ldstr      "_"
    IL_005d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0062:  stelem     [mscorlib]System.String
    IL_0067:  dup
    IL_0068:  ldc.i4.2
    IL_0069:  ldc.i4.7
    IL_006a:  stloc.0
    IL_006b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0070:  ldc.i4.7
    IL_0071:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0076:  ldstr      "_"
    IL_007b:  ldloca.s   V_0
    IL_007d:  constrained. [mscorlib]System.Int32
    IL_0083:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0088:  ldstr      "_"
    IL_008d:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0092:  stelem     [mscorlib]System.String
    IL_0097:  dup
    IL_0098:  ldc.i4.3
    IL_0099:  ldc.i4.8
    IL_009a:  stloc.0
    IL_009b:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_00a0:  ldc.i4.8
    IL_00a1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00a6:  ldstr      "_"
    IL_00ab:  ldloca.s   V_0
    IL_00ad:  constrained. [mscorlib]System.Int32
    IL_00b3:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00b8:  ldstr      "_"
    IL_00bd:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00c2:  stelem     [mscorlib]System.String
    IL_00c7:  dup
    IL_00c8:  ldc.i4.4
    IL_00c9:  ldc.i4.s   9
    IL_00cb:  stloc.0
    IL_00cc:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_00d1:  ldc.i4.s   9
    IL_00d3:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00d8:  ldstr      "_"
    IL_00dd:  ldloca.s   V_0
    IL_00df:  constrained. [mscorlib]System.Int32
    IL_00e5:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_00ea:  ldstr      "_"
    IL_00ef:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_00f4:  stelem     [mscorlib]System.String
    IL_00f9:  dup
    IL_00fa:  ldc.i4.5
    IL_00fb:  ldc.i4.s   10
    IL_00fd:  stloc.0
    IL_00fe:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0103:  ldc.i4.s   10
    IL_0105:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_010a:  ldstr      "_"
    IL_010f:  ldloca.s   V_0
    IL_0111:  constrained. [mscorlib]System.Int32
    IL_0117:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_011c:  ldstr      "_"
    IL_0121:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0126:  stelem     [mscorlib]System.String
    IL_012b:  dup
    IL_012c:  ldc.i4.6
    IL_012d:  ldstr      "_50__60_"
    IL_0132:  stelem     [mscorlib]System.String
    IL_0137:  dup
    IL_0138:  ldc.i4.7
    IL_0139:  ldc.i4.s   100
    IL_013b:  stloc.0
    IL_013c:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0141:  ldc.i4.s   100
    IL_0143:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0148:  ldstr      "_"
    IL_014d:  ldloca.s   V_0
    IL_014f:  constrained. [mscorlib]System.Int32
    IL_0155:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_015a:  ldstr      "_"
    IL_015f:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0164:  stelem     [mscorlib]System.String
    IL_0169:  dup
    IL_016a:  ldc.i4.8
    IL_016b:  ldc.i4.s   101
    IL_016d:  stloc.0
    IL_016e:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_0173:  ldc.i4.s   101
    IL_0175:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_017a:  ldstr      "_"
    IL_017f:  ldloca.s   V_0
    IL_0181:  constrained. [mscorlib]System.Int32
    IL_0187:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_018c:  ldstr      "_"
    IL_0191:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0196:  ldc.i4.s   102
    IL_0198:  stloc.0
    IL_0199:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_019e:  ldc.i4.s   102
    IL_01a0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01a5:  ldstr      "_"
    IL_01aa:  ldloca.s   V_0
    IL_01ac:  constrained. [mscorlib]System.Int32
    IL_01b2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01b7:  ldstr      "_"
    IL_01bc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_01c1:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_01c6:  stloc.1
    IL_01c7:  ldloc.1
    IL_01c8:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01cd:  ldloc.1
    IL_01ce:  stelem     [mscorlib]System.String
    IL_01d3:  dup
    IL_01d4:  ldc.i4.s   9
    IL_01d6:  ldc.i4.s   103
    IL_01d8:  stloc.0
    IL_01d9:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_01de:  ldc.i4.s   103
    IL_01e0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_01e5:  ldstr      "_"
    IL_01ea:  ldloca.s   V_0
    IL_01ec:  constrained. [mscorlib]System.Int32
    IL_01f2:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_01f7:  ldstr      "_"
    IL_01fc:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0201:  stelem     [mscorlib]System.String
    IL_0206:  dup
    IL_0207:  ldc.i4.s   10
    IL_0209:  ldstr      "_104__105_"
    IL_020e:  stelem     [mscorlib]System.String
    IL_0213:  dup
    IL_0214:  ldc.i4.s   11
    IL_0216:  ldc.i4.s   106
    IL_0218:  stloc.0
    IL_0219:  call       class [mscorlib]System.Collections.Generic.List`1<int32> Test.Test::get_arr()
    IL_021e:  ldc.i4.s   106
    IL_0220:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0225:  ldstr      "_"
    IL_022a:  ldloca.s   V_0
    IL_022c:  constrained. [mscorlib]System.Int32
    IL_0232:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0237:  ldstr      "_"
    IL_023c:  call       string [mscorlib]System.String::Concat(string,
                                                                string,
                                                                string)
    IL_0241:  stelem     [mscorlib]System.String
    IL_0246:  call       string [mscorlib]System.String::Concat(string[])
    IL_024b:  ret
  } 

} 



