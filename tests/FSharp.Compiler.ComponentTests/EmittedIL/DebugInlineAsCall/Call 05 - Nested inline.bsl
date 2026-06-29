let inline double (x: int) =
    x + x

let inline quadruple (x: int) =
    double (double x)

[<EntryPoint>]
let main _ =
    let i = quadruple 3
    if i = 12 then 0 else 1
--------------------------------------------------------------------------------

Test::double
  (3,5-3,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Test::quadruple
  (6,5-6,22)  double (double x)
    IL_0000:  ldarg.0
    IL_0001:  call Test::double
    IL_0006:  call Test::double
    IL_000b:  ret

Test::main
  (10,5-10,24)  let i = quadruple 3
    IL_0000:  ldc.i4.3
    IL_0001:  call Test::quadruple
    IL_0006:  stloc.0

  (11,5-11,19)  if i = 12 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 12
    IL_000a:  bne.un.s IL_000e

  (11,20-11,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (11,27-11,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret
