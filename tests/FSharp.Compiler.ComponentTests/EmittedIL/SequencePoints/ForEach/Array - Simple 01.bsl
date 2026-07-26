module Module

let f (l: int[]) =
    for i in l do
        ()
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_000f

  (4,5-4,10)  for i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2

  (5,9-5,11)  ()
    IL_000a:  nop

  <hidden>
    IL_000b:  ldloc.1
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  stloc.1

  (4,11-4,13)  in
    IL_000f:  ldloc.1
    IL_0010:  ldloc.0
    IL_0011:  ldlen
    IL_0012:  conv.i4
    IL_0013:  blt.s IL_0006
    IL_0015:  ret
