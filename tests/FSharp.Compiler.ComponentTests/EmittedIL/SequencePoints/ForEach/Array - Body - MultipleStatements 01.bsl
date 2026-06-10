Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_001c

  (5,5-5,10)  for i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_000a:  ldloc.2
    IL_000b:  call WriteLine

  (7,9-7,40)  System.Console.WriteLine(i + 1)
    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  call WriteLine

  <hidden>
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  stloc.1

  (5,11-5,13)  in
    IL_001c:  ldloc.1
    IL_001d:  ldloc.0
    IL_001e:  ldlen
    IL_001f:  conv.i4
    IL_0020:  blt.s IL_0006
    IL_0022:  ret
