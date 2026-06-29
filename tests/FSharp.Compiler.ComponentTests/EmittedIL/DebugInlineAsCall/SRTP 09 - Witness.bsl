let check s (b1: 'a) (b2: 'a) = if b1 = b2 then () else failwith s

let inline add (x: ^T) (y: ^T) = x + y

[<EntryPoint>]
let main _ =
    check "int" (add 3 4) 7
    check "float" (add 1.0 2.0) 3.0
    0
--------------------------------------------------------------------------------

Test::check
  (2,33-2,48)  if b1 = b2 then
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.2
    IL_0003:  stloc.1
    IL_0004:  ldloc.0
    IL_0005:  ldloc.1
    IL_0006:  call HashCompare::GenericEqualityIntrinsic
    IL_000b:  brfalse.s IL_000e

  (2,49-2,51)  ()
    IL_000d:  ret

  (2,57-2,67)  failwith s
    IL_000e:  ldarg.0
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  call Operators::Failure
    IL_0016:  throw

Test::add
  (4,34-4,39)  x + y
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
  (4,34-4,39)  x + y
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

Test::main
  (8,5-8,28)  check "int" (add 3 4) 7
    IL_0000:  ldstr "int"
    IL_0005:  ldc.i4.3
    IL_0006:  ldc.i4.4
    IL_0007:  call Test::<add>__debug@8
    IL_000c:  ldc.i4.7
    IL_000d:  call Test::check
    IL_0012:  nop

  (9,5-9,36)  check "float" (add 1.0 2.0) 3.0
    IL_0013:  ldstr "float"
    IL_0018:  ldc.r8 1.000000
    IL_0021:  ldc.r8 2.000000
    IL_002a:  call Test::<add>__debug@9-1
    IL_002f:  ldc.r8 3.000000
    IL_0038:  call Test::check
    IL_003d:  nop

  (10,5-10,6)  0
    IL_003e:  ldc.i4.0
    IL_003f:  ret

Test::<add>__debug@8
  (4,34-4,39)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret

Test::<add>__debug@9-1
  (4,34-4,39)  x + y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  ret
