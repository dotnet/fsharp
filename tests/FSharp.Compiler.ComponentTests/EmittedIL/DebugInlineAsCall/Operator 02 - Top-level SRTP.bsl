let inline (++) (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    let i = 3 ++ 4
    if i = 7 then 0 else 1
--------------------------------------------------------------------------------

Test::op_PlusPlus
  (2,35-2,40)  x + y
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  tail.
    IL_0008:  call LanguagePrimitives::AdditionDynamic
    IL_000d:  ret

Test::op_PlusPlus$W
  (2,35-2,40)  x + y
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
  (6,5-6,19)  let i = 3 ++ 4
    IL_0000:  ldc.i4.3
    IL_0001:  ldc.i4.4
    IL_0002:  call Test::<op_PlusPlus>__debug@6
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

Test::<op_PlusPlus>__debug@6
  (2,35-2,40)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
