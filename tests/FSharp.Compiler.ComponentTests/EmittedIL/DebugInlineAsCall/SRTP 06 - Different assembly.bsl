open MyLib

let inline double (x: ^T) = add x x

[<EntryPoint>]
let main _ =
    let i = double 5
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::double
  (4,29-4,36)  add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  tail.
    IL_0004:  call MyLib::add
    IL_0009:  ret

Test::double$W
  (4,29-4,36)  add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.1
    IL_0003:  tail.
    IL_0005:  call MyLib::add$W
    IL_000a:  ret

Test::main
  (8,5-8,21)  let i = double 5
    IL_0000:  ldc.i4.5
    IL_0001:  call Test::<double>__debug@8
    IL_0006:  stloc.0

  (9,5-9,19)  if i = 10 then
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 10
    IL_000a:  bne.un.s IL_000e

  (9,20-9,21)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (9,27-9,28)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret

Test::<double>__debug@8
  (4,29-4,36)  add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  tail.
    IL_0004:  call Test::<add>__debug@4
    IL_0009:  ret
