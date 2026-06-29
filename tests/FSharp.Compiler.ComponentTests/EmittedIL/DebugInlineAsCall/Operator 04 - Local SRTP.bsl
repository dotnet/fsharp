[<EntryPoint>]
let main _ =
    let inline (++) (x: ^T) (y: ^T) = x + y
    let i = 3 ++ 4
    if i = 7 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  <hidden>
    IL_0000:  ldsfld op_PlusPlus@4::@_instance
    IL_0005:  stloc.0

  (5,5-5,19)  let i = 3 ++ 4
    IL_0006:  ldc.i4.3
    IL_0007:  ldc.i4.4
    IL_0008:  call Test::<op_PlusPlus>__debug@5
    IL_000d:  stloc.1

  (6,5-6,18)  if i = 7 then
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.7
    IL_0010:  bne.un.s IL_0014

  (6,19-6,20)  0
    IL_0012:  ldc.i4.0
    IL_0013:  ret

  (6,26-6,27)  1
    IL_0014:  ldc.i4.1
    IL_0015:  ret

Test::<op_PlusPlus>__debug@5
  (4,39-4,44)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret

op_PlusPlus@4-1::Invoke
  (4,39-4,44)  x + y
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.2
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  tail.
    IL_0008:  call LanguagePrimitives::AdditionDynamic
    IL_000d:  ret
