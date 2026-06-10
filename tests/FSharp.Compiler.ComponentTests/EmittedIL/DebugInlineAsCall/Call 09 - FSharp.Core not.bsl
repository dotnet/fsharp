[<EntryPoint>]
let main _ =
    let b = not true
    if b = false then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (4,5-4,21)  let b = not true
    IL_0000:  nop

  <hidden>
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.0
    IL_0003:  ceq

  <hidden>
    IL_0005:  nop

  <hidden>
    IL_0006:  stloc.0

  (5,5-5,22)  if b = false then
    IL_0007:  nop

  <hidden>
    IL_0008:  ldloc.0
    IL_0009:  brtrue.s IL_000d

  (5,23-5,24)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

  (5,30-5,31)  1
    IL_000d:  ldc.i4.1
    IL_000e:  ret

  <hidden>
