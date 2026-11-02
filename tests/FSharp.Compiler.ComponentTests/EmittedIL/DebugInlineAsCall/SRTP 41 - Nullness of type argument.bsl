type R =
    { V: int }
    static member Default = { V = 0 }

let inline isDefault (x: ^T) = x = (^T: (static member Default: ^T) ())

[<EntryPoint>]
let main _ =
    let r = { V = 1 }
    let n: R | null = null
    if not (isDefault r) && not (isDefault n) then 0 else 1
--------------------------------------------------------------------------------

Test::isDefault
  (6,32-6,72)  x = (^T: (static member Default: ^T) ())
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  brfalse.s IL_000d
    IL_0005:  ldnull
    IL_0006:  unbox.any 0x1b000003
    IL_000b:  br.s IL_0018
    IL_000d:  ldstr "Dynamic invocation of get_Default is not supported"
    IL_0012:  newobj NotSupportedException::.ctor
    IL_0017:  throw
    IL_0018:  stloc.1
    IL_0019:  ldloc.0
    IL_001a:  ldloc.1
    IL_001b:  tail.
    IL_001d:  call HashCompare::GenericEqualityIntrinsic
    IL_0022:  ret

Test::isDefault$W
  (6,32-6,72)  x = (^T: (static member Default: ^T) ())
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldnull
    IL_0004:  callvirt Invoke
    IL_0009:  stloc.1
    IL_000a:  ldloc.0
    IL_000b:  ldloc.1
    IL_000c:  tail.
    IL_000e:  call HashCompare::GenericEqualityIntrinsic
    IL_0013:  ret

Test::main
  (10,5-10,22)  let r = { V = 1 }
    IL_0000:  ldc.i4.1
    IL_0001:  newobj R::.ctor
    IL_0006:  stloc.0

  (11,5-11,27)  let n: R | null = null
    IL_0007:  nop

  (12,5-12,51)  if not (isDefault r) && not (isDefault n) then
    IL_0008:  nop

  (12,8-12,25)  not (isDefault r)
    IL_0009:  ldloc.0
    IL_000a:  call Test::<isDefault>__debug@12
    IL_000f:  ldc.i4.0
    IL_0010:  ceq

  <hidden>
    IL_0012:  nop
    IL_0013:  brfalse.s IL_0021

  (12,29-12,46)  not (isDefault n)
    IL_0015:  ldloc.1
    IL_0016:  call Test::<isDefault>__debug@12-1
    IL_001b:  ldc.i4.0
    IL_001c:  ceq

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_0023

  <hidden>
    IL_0021:  ldc.i4.0

  <hidden>
    IL_0022:  nop
    IL_0023:  brfalse.s IL_0027

  (12,52-12,53)  0
    IL_0025:  ldc.i4.0
    IL_0026:  ret

  (12,59-12,60)  1
    IL_0027:  ldc.i4.1
    IL_0028:  ret

Test::<isDefault>__debug@12
  (6,32-6,72)  x = (^T: (static member Default: ^T) ())
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  call R::get_Default
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  ldloc.1
    IL_000a:  call LanguagePrimitives::get_GenericEqualityComparer
    IL_000f:  callvirt R::Equals
    IL_0014:  ret

Test::<isDefault>__debug@12-1
  (6,32-6,72)  x = (^T: (static member Default: ^T) ())
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  call R::get_Default
    IL_0007:  stloc.1
    IL_0008:  ldloc.0
    IL_0009:  brfalse.s IL_0018

  <hidden>
    IL_000b:  ldloc.0
    IL_000c:  ldloc.1
    IL_000d:  call LanguagePrimitives::get_GenericEqualityComparer
    IL_0012:  callvirt R::Equals
    IL_0017:  ret

  <hidden>
    IL_0018:  ldloc.1
    IL_0019:  ldnull
    IL_001a:  cgt.un
    IL_001c:  ldc.i4.0
    IL_001d:  ceq
    IL_001f:  ret

R::CompareTo
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0026

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_0024

  <hidden>
    IL_0006:  call LanguagePrimitives::get_GenericComparer
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  ldfld R::V@
    IL_0012:  stloc.1
    IL_0013:  ldarg.1
    IL_0014:  ldfld R::V@
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldloc.2
    IL_001c:  cgt
    IL_001e:  ldloc.1
    IL_001f:  ldloc.2
    IL_0020:  clt
    IL_0022:  sub
    IL_0023:  ret

  <hidden>
    IL_0024:  ldc.i4.1
    IL_0025:  ret

  <hidden>
    IL_0026:  ldarg.1
    IL_0027:  brfalse.s IL_002b

  <hidden>
    IL_0029:  ldc.i4.m1
    IL_002a:  ret

  <hidden>
    IL_002b:  ldc.i4.0
    IL_002c:  ret

R::CompareTo
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  unbox.any R
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s IL_0033

  <hidden>
    IL_000c:  ldarg.1
    IL_000d:  unbox.any R
    IL_0012:  brfalse.s IL_0031

  <hidden>
    IL_0014:  ldarg.2
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldfld R::V@
    IL_001c:  stloc.3
    IL_001d:  ldloc.1
    IL_001e:  ldfld R::V@
    IL_0023:  stloc.s 4
    IL_0025:  ldloc.3
    IL_0026:  ldloc.s 4
    IL_0028:  cgt
    IL_002a:  ldloc.3
    IL_002b:  ldloc.s 4
    IL_002d:  clt
    IL_002f:  sub
    IL_0030:  ret

  <hidden>
    IL_0031:  ldc.i4.1
    IL_0032:  ret

  <hidden>
    IL_0033:  ldarg.1
    IL_0034:  unbox.any R
    IL_0039:  brfalse.s IL_003d

  <hidden>
    IL_003b:  ldc.i4.m1
    IL_003c:  ret

  <hidden>
    IL_003d:  ldc.i4.0
    IL_003e:  ret

R::GetHashCode
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_001e

  <hidden>
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4 -1640531527
    IL_000a:  ldarg.1
    IL_000b:  stloc.1
    IL_000c:  ldarg.0
    IL_000d:  ldfld R::V@
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.6
    IL_0014:  shl
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.2
    IL_0017:  shr
    IL_0018:  add
    IL_0019:  add
    IL_001a:  add
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ret

  <hidden>
    IL_001e:  ldc.i4.0
    IL_001f:  ret

R::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_001b

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_0019

  <hidden>
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldarg.2
    IL_0009:  stloc.1
    IL_000a:  ldarg.0
    IL_000b:  ldfld R::V@
    IL_0010:  ldloc.0
    IL_0011:  ldfld R::V@
    IL_0016:  ceq
    IL_0018:  ret

  <hidden>
    IL_0019:  ldc.i4.0
    IL_001a:  ret

  <hidden>
    IL_001b:  ldarg.1
    IL_001c:  ldnull
    IL_001d:  cgt.un
    IL_001f:  ldc.i4.0
    IL_0020:  ceq
    IL_0022:  ret

R::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst R
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0013

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  callvirt R::Equals
    IL_0012:  ret

  <hidden>
    IL_0013:  ldc.i4.0
    IL_0014:  ret

R::get_Default
  (4,29-4,38)  { V = 0 }
    IL_0000:  ldc.i4.0
    IL_0001:  newobj R::.ctor
    IL_0006:  ret

R::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0017

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_0015

  <hidden>
    IL_0006:  ldarg.0
    IL_0007:  ldfld R::V@
    IL_000c:  ldarg.1
    IL_000d:  ldfld R::V@
    IL_0012:  ceq
    IL_0014:  ret

  <hidden>
    IL_0015:  ldc.i4.0
    IL_0016:  ret

  <hidden>
    IL_0017:  ldarg.1
    IL_0018:  ldnull
    IL_0019:  cgt.un
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  ret

R::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst R
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0012

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt R::Equals
    IL_0011:  ret

  <hidden>
    IL_0012:  ldc.i4.0
    IL_0013:  ret
