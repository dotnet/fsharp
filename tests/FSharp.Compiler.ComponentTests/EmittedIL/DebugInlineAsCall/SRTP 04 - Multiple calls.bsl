let inline add (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let i = add 1 2 + add 3 4
    if i = 10 then 0 else 1
--------------------------------------------------------------------------------

Test::add
  (2,34-2,39)  x + y
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
  (2,34-2,39)  x + y
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
  (6,5-6,30)  let i = add 1 2 + add 3 4
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  call Test::<add>__debug@6
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call Test::<add>__debug@6-1
    IL_000e:  add
    IL_000f:  stloc.0

  (7,5-7,19)  if i = 10 then
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.s 10
    IL_0013:  bne.un.s IL_0017

  (7,20-7,21)  0
    IL_0015:  ldc.i4.0
    IL_0016:  ret

  (7,27-7,28)  1
    IL_0017:  ldc.i4.1
    IL_0018:  ret

Test::<add>__debug@6
  (2,34-2,39)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret

Test::<add>__debug@6-1
  (2,34-2,39)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
