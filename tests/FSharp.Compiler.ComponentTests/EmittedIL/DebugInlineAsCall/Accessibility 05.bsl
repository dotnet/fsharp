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
    IL_000f:  nop

  <hidden>
    IL_0010:  ldloc.0
    IL_0011:  call MyNum::get_Value
    IL_0016:  ldc.r8 3.140000
    IL_001f:  ceq
    IL_0021:  brfalse.s IL_0025

  (7,33-7,34)  0
    IL_0023:  ldc.i4.0
    IL_0024:  ret

  (7,40-7,41)  1
    IL_0025:  ldc.i4.1
    IL_0026:  ret

  <hidden>
