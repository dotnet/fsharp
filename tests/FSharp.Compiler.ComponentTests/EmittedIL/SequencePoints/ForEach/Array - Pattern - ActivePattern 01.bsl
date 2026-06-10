Module::|Id|
  (4,23-4,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (7,17-7,18)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0019

  (7,5-7,13)  for Id i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2
    IL_000a:  ldloc.2
    IL_000b:  call |Id|
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  stloc.s 4

  (8,9-8,11)  ()
    IL_0014:  nop

  <hidden>
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.1

  (7,14-7,16)  in
    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  ldlen
    IL_001c:  conv.i4
    IL_001d:  blt.s IL_0006
    IL_001f:  ret
