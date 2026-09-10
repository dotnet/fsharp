module Module

let f (x: int) =
    x + x

let g () =
    if f 5 = 10 then 0 else 1
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,5-7,21)  if f 5 = 10 then
    IL_0000:  ldc.i4.5
    IL_0001:  call Module::f
    IL_0006:  ldc.i4.s 10
    IL_0008:  bne.un.s IL_000c

  (7,22-7,23)  0
    IL_000a:  ldc.i4.0
    IL_000b:  ret

  (7,29-7,30)  1
    IL_000c:  ldc.i4.1
    IL_000d:  ret
