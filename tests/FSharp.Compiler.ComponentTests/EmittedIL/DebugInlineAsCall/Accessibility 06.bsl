open Lib

[<EntryPoint>]
let main _ =
    let i = publicFn 3
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,5-6,23)  let i = publicFn 3
    IL_0000:  ldc.i4.3
    IL_0001:  call Lib::publicFn
    IL_0006:  stloc.0

  (7,5-7,19)  if i = 10 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 10
    IL_000a:  bne.un.s IL_000e

  (7,20-7,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (7,27-7,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
