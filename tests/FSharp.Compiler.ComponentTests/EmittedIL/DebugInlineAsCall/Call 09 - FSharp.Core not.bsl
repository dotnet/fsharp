[<EntryPoint>]
let main _ =
    let b = not true
    if b = false then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (4,5-4,21)  let b = not true
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.0
    IL_0002:  ceq

  <hidden>
    IL_0004:  nop
    IL_0005:  stloc.0

  (5,5-5,22)  if b = false then
    IL_0006:  ldloc.0
    IL_0007:  brtrue.s IL_000b

  (5,23-5,24)  0
    IL_0009:  ldc.i4.0
    IL_000a:  ret

  (5,30-5,31)  1
    IL_000b:  ldc.i4.1
    IL_000c:  ret
