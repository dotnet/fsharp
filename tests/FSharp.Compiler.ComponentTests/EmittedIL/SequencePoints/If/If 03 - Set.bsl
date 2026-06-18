module Module

let f (x: int) =
    x + x

let g (arr: int[]) =
    arr[0] <- if f 5 = 10 then 0 else 1
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,5-7,11)  arr[0]
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  stloc.1

  (7,15-7,31)  if f 5 = 10 then
    IL_0004:  ldloc.1
    IL_0005:  ldloc.0
    IL_0006:  ldc.i4.5
    IL_0007:  call Module::f
    IL_000c:  ldc.i4.s 10
    IL_000e:  bne.un.s IL_0018

  <hidden>
    IL_0010:  stloc.2
    IL_0011:  stloc.3

  (7,32-7,33)  0
    IL_0012:  ldloc.3
    IL_0013:  ldloc.2
    IL_0014:  ldc.i4.0

  <hidden>
    IL_0015:  nop
    IL_0016:  br.s IL_0022

  <hidden>
    IL_0018:  stloc.s 4
    IL_001a:  stloc.s 5

  (7,39-7,40)  1
    IL_001c:  ldloc.s 5
    IL_001e:  ldloc.s 4
    IL_0020:  ldc.i4.1

  <hidden>
    IL_0021:  nop
    IL_0022:  stelem.i4
    IL_0023:  ret
