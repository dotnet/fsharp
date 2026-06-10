[<EntryPoint>]
let main _ =
    let inline (++) (x: int) (y: int) = x + y + 1
    let i = 3 ++ 4
    if i = 8 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  <hidden>
    IL_0000:  ldsfld op_PlusPlus@4::@_instance
    IL_0005:  stloc.0

  (5,5-5,19)  let i = 3 ++ 4
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.3
    IL_0008:  ldc.i4.4
    IL_0009:  call InvokeFast
    IL_000e:  stloc.1

  (6,5-6,18)  if i = 8 then
    IL_000f:  nop

  <hidden>
    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.8
    IL_0012:  bne.un.s IL_0016

  (6,19-6,20)  0
    IL_0014:  ldc.i4.0
    IL_0015:  ret

  (6,26-6,27)  1
    IL_0016:  ldc.i4.1
    IL_0017:  ret

  <hidden>

op_PlusPlus@4::Invoke
  (4,41-4,50)  x + y + 1
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  add
    IL_0003:  ldc.i4.1
    IL_0004:  add
    IL_0005:  ret
