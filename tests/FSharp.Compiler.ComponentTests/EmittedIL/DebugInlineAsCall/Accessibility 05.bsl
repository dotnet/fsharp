open Module

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(3.14)
    if result.Value = 3.14 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,5-6,39)  let result = T.Invoke<MyNum>(3.14)
    IL_0000:  ldc.r8 3.140000
    IL_0009:  call Test::<Invoke>__debug@6
    IL_000e:  stloc.0

  (7,5-7,32)  if result.Value = 3.14 then
    IL_000f:  ldloc.0
    IL_0010:  call MyNum::get_Value
    IL_0015:  ldc.r8 3.140000
    IL_001e:  ceq
    IL_0020:  brfalse.s IL_0024

  (7,33-7,34)  0
    IL_0022:  ldc.i4.0
    IL_0023:  ret

  (7,40-7,41)  1
    IL_0024:  ldc.i4.1
    IL_0025:  ret
