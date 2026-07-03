[<NoDynamicInvocation>]
let inline f x = x + 1

let inline g x = f x

g 1 |> ignore
--------------------------------------------------------------------------------

Test::g
  (5,18-5,21)  f x
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  ret

Test::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Test::init@
    IL_0006:  ldsfld $Test::init@
    IL_000b:  pop
    IL_000c:  ret

Test::staticInitialization@
  (7,1-7,4)  g 1
    IL_0000:  ldc.i4.1
    IL_0001:  call Test::g
    IL_0006:  stloc.0

  (7,8-7,14)  ignore
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ret
