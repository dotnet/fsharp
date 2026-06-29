open MyLib

[<EntryPoint>]
let main _ =
    let i = add 3 4
    if i = 7 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,5-6,20)  let i = add 3 4
    IL_0000:  ldc.i4.3
    IL_0001:  ldc.i4.4
    IL_0002:  call Test::<add>__debug@6
    IL_0007:  stloc.0

  (7,5-7,18)  if i = 7 then
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.7
    IL_000a:  bne.un.s IL_000e

  (7,19-7,20)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (7,26-7,27)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
