[<NoDynamicInvocation>]
let inline f (x: ^T) = x

let inline g (x: ^T) (y: ^T) = f (x + y)

g 1 2 |> ignore
--------------------------------------------------------------------------------

Test::g
  (5,32-5,41)  f (x + y)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  tail.
    IL_0008:  call LanguagePrimitives::AdditionDynamic
    IL_000d:  ret

Test::g$W
  (5,32-5,41)  f (x + y)
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

Test::<g>__debug@7
  (5,32-5,41)  f (x + y)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
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
  (7,1-7,6)  g 1 2
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  call Test::<g>__debug@7
    IL_0007:  stloc.0

  (7,10-7,16)  ignore
    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  ret
