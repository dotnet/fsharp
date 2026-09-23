[<NoDynamicInvocation>]
let inline f x = x + 1

let inline g (x: ^T) (y: ^T) = f (x + y)

g 1 2 |> ignore
--------------------------------------------------------------------------------

Test::g
  (5,32-5,41)  f (x + y)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.2
    IL_0004:  ldloc.1
    IL_0005:  ldloc.2
    IL_0006:  call LanguagePrimitives::AdditionDynamic
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  stloc.3
    IL_000e:  ldc.i4.1
    IL_000f:  stloc.s 4
    IL_0011:  ldstr "Dynamic invocation of op_Addition is not supported"
    IL_0016:  newobj NotSupportedException::.ctor
    IL_001b:  throw

Test::g$W
  (5,32-5,41)  f (x + y)
    IL_0000:  ldarg.2
    IL_0001:  stloc.1
    IL_0002:  ldarg.3
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  ldloc.1
    IL_0006:  ldloc.2
    IL_0007:  call InvokeFast
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  stloc.3
    IL_000f:  ldc.i4.1
    IL_0010:  stloc.s 4
    IL_0012:  ldarg.1
    IL_0013:  ldloc.3
    IL_0014:  ldloc.s 4
    IL_0016:  tail.
    IL_0018:  call InvokeFast
    IL_001d:  ret

Test::<g>__debug@7
  (5,32-5,41)  f (x + y)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ldc.i4.1
    IL_0004:  add
    IL_0005:  ret

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
