module Module

let f (x: int) =
    x + x

let g () =
    let mutable x = 0
    if f 5 = 10 then x <- 1 else x <- 2
    x + x
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,5-7,22)  let mutable x = 0
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0

  (8,5-8,21)  if f 5 = 10 then
    IL_0002:  ldc.i4.5
    IL_0003:  call Module::f
    IL_0008:  ldc.i4.s 10
    IL_000a:  bne.un.s IL_0011

  (8,22-8,28)  x <- 1
    IL_000c:  ldc.i4.1
    IL_000d:  stloc.0

  <hidden>
    IL_000e:  nop
    IL_000f:  br.s IL_0014

  (8,34-8,40)  x <- 2
    IL_0011:  ldc.i4.2
    IL_0012:  stloc.0

  <hidden>
    IL_0013:  nop

  (9,5-9,10)  x + x
    IL_0014:  ldloc.0
    IL_0015:  ldloc.0
    IL_0016:  add
    IL_0017:  ret
