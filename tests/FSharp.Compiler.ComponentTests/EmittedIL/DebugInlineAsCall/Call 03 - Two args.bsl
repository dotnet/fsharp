let inline add a b =
    a + b

[<EntryPoint>]
let main _ =
    let i = add 1 2
    if i = 3 then 0 else 1
--------------------------------------------------------------------------------

Test::add
  (4,5-4,10)  a + b
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  tail.
    IL_0008:  call LanguagePrimitives::AdditionDynamic
    IL_000d:  ret

Test::add$W
  (4,5-4,10)  a + b
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.2
    IL_0003:  stloc.1
    IL_0004:  ldarg.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.1
    IL_0007:  tail.
    IL_0009:  call InvokeFast
    IL_000e:  ret

Test::main
  (8,5-8,20)  let i = add 1 2
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  call Test::<add>__debug@8
    IL_0007:  stloc.0

  (9,5-9,18)  if i = 3 then
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.3
    IL_000a:  bne.un.s IL_000e

  (9,19-9,20)  0
    IL_000c:  ldc.i4.0
    IL_000d:  ret

  (9,26-9,27)  1
    IL_000e:  ldc.i4.1
    IL_000f:  ret

Test::<add>__debug@8
  (4,5-4,10)  a + b
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
