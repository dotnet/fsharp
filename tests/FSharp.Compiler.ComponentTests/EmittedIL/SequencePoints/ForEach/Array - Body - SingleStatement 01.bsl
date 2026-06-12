Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0014

  (5,5-5,10)  for i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_000a:  ldloc.2
    IL_000b:  call WriteLine

  <hidden>
    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.1

  (5,11-5,13)  in
    IL_0014:  ldloc.1
    IL_0015:  ldloc.0
    IL_0016:  ldlen
    IL_0017:  conv.i4
    IL_0018:  blt.s IL_0006
    IL_001a:  ret
