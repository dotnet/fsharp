open MyLib

[<EntryPoint>]
let main _ =
    let i = triple 3
    if i = 9 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,5-6,21)  let i = triple 3
    IL_0000:  ldc.i4.3
    IL_0001:  call MyLib::triple
    IL_0006:  stloc.0

  (7,5-7,18)  if i = 9 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 9
    IL_000a:  bne.un.s IL_000e

  (7,19-7,20)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (7,26-7,27)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
