Module::f
  (5,19-5,20)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0022

  (5,5-5,15)  for i1, i2
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem 0x1b000001
    IL_000d:  stloc.2
    IL_000e:  ldloc.2
    IL_000f:  call get_Item2
    IL_0014:  stloc.3
    IL_0015:  ldloc.2
    IL_0016:  call get_Item1
    IL_001b:  stloc.s 4

  (6,9-6,11)  ()
    IL_001d:  nop

  <hidden>
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1

  (5,16-5,18)  in
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  ldlen
    IL_0025:  conv.i4
    IL_0026:  blt.s IL_0006
    IL_0028:  ret
