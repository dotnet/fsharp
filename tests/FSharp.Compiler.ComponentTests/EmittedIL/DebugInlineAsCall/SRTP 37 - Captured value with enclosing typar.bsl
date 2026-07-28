let f<'a> (v: 'a) =
    let xs = [ v; v ]
    let inline g y = List.length xs + int y
    g 1uy

[<EntryPoint>]
let main _ =
    if f "a" = 3 && f 1 = 3 && f 1.5 = 3 && f System.DateTime.Now = 3 then 0 else 1
--------------------------------------------------------------------------------

Test::f
  (3,5-3,22)  let xs = [ v; v ]
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call get_Empty
    IL_0007:  call Cons
    IL_000c:  call Cons
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  newobj .ctor
    IL_0018:  stloc.1

  (5,5-5,10)  g 1uy
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.1
    IL_001b:  tail.
    IL_001d:  call Test::<g>__debug@5
    IL_0022:  ret

Test::main
  (9,5-9,75)  if f "a" = 3 && f 1 = 3 && f 1.5 = 3 && f System.DateTime.Now = 3 then
    IL_0000:  nop

  (9,8-9,17)  f "a" = 3
    IL_0001:  ldstr "a"
    IL_0006:  call Test::f
    IL_000b:  ldc.i4.3
    IL_000c:  bne.un.s IL_001a

  (9,21-9,28)  f 1 = 3
    IL_000e:  ldc.i4.1
    IL_000f:  call Test::f
    IL_0014:  ldc.i4.3
    IL_0015:  ceq

  <hidden>
    IL_0017:  nop
    IL_0018:  br.s IL_001c

  <hidden>
    IL_001a:  ldc.i4.0

  <hidden>
    IL_001b:  nop
    IL_001c:  brfalse.s IL_0032

  (9,32-9,41)  f 1.5 = 3
    IL_001e:  ldc.r8 1.500000
    IL_0027:  call Test::f
    IL_002c:  ldc.i4.3
    IL_002d:  ceq

  <hidden>
    IL_002f:  nop
    IL_0030:  br.s IL_0034

  <hidden>
    IL_0032:  ldc.i4.0

  <hidden>
    IL_0033:  nop
    IL_0034:  brfalse.s IL_0046

  (9,45-9,70)  f System.DateTime.Now = 3
    IL_0036:  call DateTime::get_Now
    IL_003b:  call Test::f
    IL_0040:  ldc.i4.3
    IL_0041:  ceq

  <hidden>
    IL_0043:  nop
    IL_0044:  br.s IL_0048

  <hidden>
    IL_0046:  ldc.i4.0

  <hidden>
    IL_0047:  nop
    IL_0048:  brfalse.s IL_004c

  (9,76-9,77)  0
    IL_004a:  ldc.i4.0
    IL_004b:  ret

  (9,83-9,84)  1
    IL_004c:  ldc.i4.1
    IL_004d:  ret

Test::<g>__debug@5
  (4,22-4,44)  List.length xs + int y
    IL_0000:  ldarg.0
    IL_0001:  call ListModule::Length
    IL_0006:  ldarg.1
    IL_0007:  conv.i4
    IL_0008:  add
    IL_0009:  ret

g@4-1::Invoke
  (4,22-4,44)  List.length xs + int y
    IL_0000:  ldarg.0
    IL_0001:  ldfld xs
    IL_0006:  call ListModule::Length
    IL_000b:  ldarg.1
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  call LanguagePrimitives::ExplicitDynamic
    IL_0013:  add
    IL_0014:  ret
