open MyLib

[<Struct>]
type S =
    member _.M(x: byref<int>) = x <- 42

[<EntryPoint>]
let main _ =
    let mutable s = S()
    let mutable v = 0
    f(&s, &v)
    if v = 42 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (10,5-10,24)  let mutable s = S()
    IL_0000:  nop

  (11,5-11,22)  let mutable v = 0
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.1

  (12,5-12,14)  f(&s, &v)
    IL_0003:  ldloca.s 0
    IL_0005:  ldloca.s 1
    IL_0007:  call Test::<f>__debug@12
    IL_000c:  nop

  (13,5-13,19)  if v = 42 then
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.s 42
    IL_0010:  bne.un.s IL_0014

  (13,20-13,21)  0
    IL_0012:  ldc.i4.0
    IL_0013:  ret

  (13,27-13,28)  1
    IL_0014:  ldc.i4.1
    IL_0015:  ret

S::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst S
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001d

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any S
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  ldarg.2
    IL_0017:  call S::Equals
    IL_001c:  ret

  <hidden>
    IL_001d:  ldc.i4.0
    IL_001e:  ret

S::M
  (6,33-6,40)  x <- 42
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s 42
    IL_0003:  stobj Int32
    IL_0008:  ret

S::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst S
    IL_0008:  ldnull
    IL_0009:  cgt.un
    IL_000b:  brfalse.s IL_001c

  <hidden>
    IL_000d:  ldarg.1
    IL_000e:  unbox.any S
    IL_0013:  stloc.1
    IL_0014:  ldarg.0
    IL_0015:  ldloc.1
    IL_0016:  call S::Equals
    IL_001b:  ret

  <hidden>
    IL_001c:  ldc.i4.0
    IL_001d:  ret
