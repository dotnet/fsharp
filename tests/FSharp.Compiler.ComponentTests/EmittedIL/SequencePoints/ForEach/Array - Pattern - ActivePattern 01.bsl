module Module

let (|Id|) (x: int) = x

let f (l: int[]) =
    for Id i in l do
        ()
--------------------------------------------------------------------------------

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (6,17-6,18)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0019

  (6,5-6,13)  for Id i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2
    IL_000a:  ldloc.2
    IL_000b:  call |Id|
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  stloc.s 4

  (7,9-7,11)  ()
    IL_0014:  nop

  <hidden>
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stloc.1

  (6,14-6,16)  in
    IL_0019:  ldloc.1
    IL_001a:  ldloc.0
    IL_001b:  ldlen
    IL_001c:  conv.i4
    IL_001d:  blt.s IL_0006
    IL_001f:  ret
