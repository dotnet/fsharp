let inline (++) (x: int) (y: int) = x + y + 1

[<EntryPoint>]
let main _ =
    let i = 3 ++ 4
    if i = 8 then 0 else 1
--------------------------------------------------------------------------------

Test::op_PlusPlus
  (2,37-2,46)  x + y + 1
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ldc.i4.1
    IL_0004:  add
    IL_0005:  ret

Test::main
  (6,5-6,19)  let i = 3 ++ 4
    IL_0000:  ldc.i4.3
    IL_0001:  ldc.i4.4
    IL_0002:  call Test::op_PlusPlus
    IL_0007:  stloc.0

  (7,5-7,18)  if i = 8 then
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.8
    IL_000a:  bne.un.s IL_000e

  (7,19-7,20)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (7,26-7,27)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
