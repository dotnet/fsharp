let inline double (x: int) =
    x + x

[<EntryPoint>]
let main _ =
    let i = double 1 + double 2
    if i = 6 then 0 else 1
--------------------------------------------------------------------------------

Test::double
  (3,5-3,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Test::main
  (7,5-7,32)  let i = double 1 + double 2
    IL_0000:  ldc.i4.1
    IL_0001:  call Test::double
    IL_0006:  ldc.i4.2
    IL_0007:  call Test::double
    IL_000c:  add
    IL_000d:  stloc.0

  (8,5-8,18)  if i = 6 then
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.6
    IL_0010:  bne.un.s IL_0014

  (8,19-8,20)  0
    IL_0012:  ldc.i4.0
    IL_0013:  ret

  (8,26-8,27)  1
    IL_0014:  ldc.i4.1
    IL_0015:  ret
