module Module

let f (l: (int * int)[]) =
    for i1, i2 in l do
        ()
--------------------------------------------------------------------------------

Module::f
  (4,19-4,20)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0022

  (4,5-4,15)  for i1, i2
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

  (5,9-5,11)  ()
    IL_001d:  nop

  <hidden>
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1

  (4,16-4,18)  in
    IL_0022:  ldloc.1
    IL_0023:  ldloc.0
    IL_0024:  ldlen
    IL_0025:  conv.i4
    IL_0026:  blt.s IL_0006
    IL_0028:  ret
