let inline f (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = f 5
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Test::main
  (7,5-7,16)  let i = f 5
    IL_0000:  ldc.i4.5
    IL_0001:  call Test::f
    IL_0006:  stloc.0

  (8,5-8,19)  if i = 10 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 10
    IL_000a:  bne.un.s IL_000e

  (8,20-8,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (8,27-8,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
