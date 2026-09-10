module Module

let f (x: int) =
    x + x

let g () =
    (if f 5 = 10 then 0 else 1) + f 1
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,5-7,38)  (if f 5 = 10 then 0 else 1) + f 1
    IL_0000:  nop

  (7,6-7,22)  if f 5 = 10 then
    IL_0001:  ldc.i4.5
    IL_0002:  call Module::f
    IL_0007:  ldc.i4.s 10
    IL_0009:  bne.un.s IL_000f

  (7,23-7,24)  0
    IL_000b:  ldc.i4.0

  <hidden>
    IL_000c:  nop
    IL_000d:  br.s IL_0011

  (7,30-7,31)  1
    IL_000f:  ldc.i4.1

  <hidden>
    IL_0010:  nop
    IL_0011:  ldc.i4.1
    IL_0012:  call Module::f
    IL_0017:  add
    IL_0018:  ret
