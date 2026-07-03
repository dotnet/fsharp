let inline f<'T, 'U when 'T: (member M: byref<'U> -> unit)> (x: byref<'T>, y: byref<'U>) =
    x.M(&y)

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

Test::f
  (3,5-3,8)  x.M
    IL_0000:  ldstr "Dynamic invocation of M is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::f$W
  (3,5-3,8)  x.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldobj 0x1b000003
    IL_0007:  ldarg.2
    IL_0008:  call InvokeFast
    IL_000d:  pop
    IL_000e:  ret

Test::main
  (11,5-11,24)  let mutable s = S()
    IL_0000:  nop

  (12,5-12,22)  let mutable v = 0
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.1

  (13,5-13,14)  f(&s, &v)
    IL_0003:  ldloca.s 0
    IL_0005:  ldloca.s 1
    IL_0007:  call Test::<f>__debug@13
    IL_000c:  nop

  (14,5-14,19)  if v = 42 then
    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.s 42
    IL_0010:  bne.un.s IL_0014

  (14,20-14,21)  0
    IL_0012:  ldc.i4.0
    IL_0013:  ret

  (14,27-14,28)  1
    IL_0014:  ldc.i4.1
    IL_0015:  ret

Test::<f>__debug@13
  (3,5-3,8)  x.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call S::M
    IL_0007:  nop
    IL_0008:  ret

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
  (7,33-7,40)  x <- 42
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
