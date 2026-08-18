let inline add (x: ^T) (y: ^T) = x + y

let inline addBoth (x: ^A) (y: ^B) =
    let a = add x x
    let b = add y y
    (a, b)

[<EntryPoint>]
let main _ =
    let (a, b) = addBoth 2 3.0
    if a = 4 && b = 6.0 then 0 else 1
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

Test::addBoth
  (5,5-5,20)  let a = add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call Test::add
    IL_0007:  stloc.0

  (6,5-6,20)  let b = add y y
    IL_0008:  ldarg.1
    IL_0009:  ldarg.1
    IL_000a:  call Test::add
    IL_000f:  stloc.1

  (7,6-7,10)  a, b
    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  newobj .ctor
    IL_0017:  ret

Test::addBoth$W
  (5,5-5,20)  let a = add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.2
    IL_0002:  ldarg.2
    IL_0003:  call Test::add$W
    IL_0008:  stloc.0

  (6,5-6,20)  let b = add y y
    IL_0009:  ldarg.1
    IL_000a:  ldarg.3
    IL_000b:  ldarg.3
    IL_000c:  call Test::add$W
    IL_0011:  stloc.1

  (7,6-7,10)  a, b
    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  newobj .ctor
    IL_0019:  ret

Test::main
  (11,5-11,31)  let (a, b) = addBoth 2 3.0
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.r8 3.000000
    IL_000a:  call Test::<addBoth>__debug@11
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  call get_Item2
    IL_0016:  stloc.1
    IL_0017:  ldloc.0
    IL_0018:  call get_Item1
    IL_001d:  stloc.2

  (12,5-12,29)  if a = 4 && b = 6.0 then
    IL_001e:  nop

  (12,8-12,13)  a = 4
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.4
    IL_0021:  bne.un.s IL_0032

  (12,17-12,24)  b = 6.0
    IL_0023:  ldloc.1
    IL_0024:  ldc.r8 6.000000
    IL_002d:  ceq

  <hidden>
    IL_002f:  nop
    IL_0030:  br.s IL_0034

  <hidden>
    IL_0032:  ldc.i4.0

  <hidden>
    IL_0033:  nop
    IL_0034:  brfalse.s IL_0038

  (12,30-12,31)  0
    IL_0036:  ldc.i4.0
    IL_0037:  ret

  (12,37-12,38)  1
    IL_0038:  ldc.i4.1
    IL_0039:  ret

Test::<add>__debug@5
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

Test::<addBoth>__debug@11
  (5,5-5,20)  let a = add x x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call Test::<add>__debug@5
    IL_0007:  stloc.0

  (6,5-6,20)  let b = add y y
    IL_0008:  ldarg.1
    IL_0009:  ldarg.1
    IL_000a:  call Test::<add>__debug@6-1
    IL_000f:  stloc.1

  (7,6-7,10)  a, b
    IL_0010:  ldloc.0
    IL_0011:  ldloc.1
    IL_0012:  newobj .ctor
    IL_0017:  ret
