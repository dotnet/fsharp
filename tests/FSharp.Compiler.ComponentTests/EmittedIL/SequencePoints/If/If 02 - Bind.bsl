module Module

let f (x: int) =
    x + x

let g () =
    let y = if f 5 = 10 then 0 else 1
    y + y
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,13-7,29)  if f 5 = 10 then
    IL_0000:  ldc.i4.5
    IL_0001:  call Module::f
    IL_0006:  ldc.i4.s 10
    IL_0008:  bne.un.s IL_000e

  (7,30-7,31)  0
    IL_000a:  ldc.i4.0

  <hidden>
    IL_000b:  nop
    IL_000c:  br.s IL_0010

  (7,37-7,38)  1
    IL_000e:  ldc.i4.1

  <hidden>
    IL_000f:  nop
    IL_0010:  stloc.0

  (8,5-8,10)  y + y
    IL_0011:  ldloc.0
    IL_0012:  ldloc.0
    IL_0013:  add
    IL_0014:  ret
