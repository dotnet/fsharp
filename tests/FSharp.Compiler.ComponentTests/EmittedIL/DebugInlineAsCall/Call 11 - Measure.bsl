[<Measure>] type cm

let inline scale (x: float<'u>) = x * 2.0

[<EntryPoint>]
let main _ =
    let v = scale 5.0<cm>
    if v = 10.0<cm> then 0 else 1
--------------------------------------------------------------------------------

Test::scale
  (4,35-4,42)  x * 2.0
    IL_0000:  ldarg.0
    IL_0001:  ldc.r8 2.000000
    IL_000a:  mul
    IL_000b:  ret

Test::main
  (8,5-8,26)  let v = scale 5.0<cm>
    IL_0000:  ldc.r8 5.000000
    IL_0009:  call Test::scale
    IL_000e:  stloc.0

  (9,5-9,25)  if v = 10.0<cm> then
    IL_000f:  nop

  <hidden>
    IL_0010:  ldloc.0
    IL_0011:  ldc.r8 10.000000
    IL_001a:  ceq
    IL_001c:  brfalse.s IL_0020

  (9,26-9,27)  0
    IL_001e:  ldc.i4.0
    IL_001f:  ret

  (9,33-9,34)  1
    IL_0020:  ldc.i4.1
    IL_0021:  ret

  <hidden>
