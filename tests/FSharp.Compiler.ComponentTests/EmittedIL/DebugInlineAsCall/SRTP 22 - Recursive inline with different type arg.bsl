type T = T with
    static member ($) (T, _:int) = (+)
    static member ($) (T, _:decimal) = (+)

let inline sum (i:'a) (x:'a) :'r = (T $ Unchecked.defaultof<'r>) i x

type T with
    static member inline ($) (T, _:'t -> 'rest) = fun (a:'t) x -> sum (x + a)

[<EntryPoint>]
let main _ =
    let y:int = sum 2 3 4
    if y = 9 then 0 else 1
--------------------------------------------------------------------------------

Test::sum
  (6,36-6,69)  (T $ Unchecked.defaultof<'r>) i x
    IL_0000:  ldc.i4.0
    IL_0001:  brfalse.s IL_000b
    IL_0003:  ldnull
    IL_0004:  unbox.any 0x1b000008
    IL_0009:  br.s IL_0016
    IL_000b:  ldstr "Dynamic invocation of op_Dollar is not supported"
    IL_0010:  newobj NotSupportedException::.ctor
    IL_0015:  throw
    IL_0016:  ldarg.0
    IL_0017:  ldarg.1
    IL_0018:  tail.
    IL_001a:  call InvokeFast
    IL_001f:  ret

Test::sum$W
  (6,36-6,69)  (T $ Unchecked.defaultof<'r>) i x
    IL_0000:  ldarg.0
    IL_0001:  call T::get_T
    IL_0006:  ldloc.0
    IL_0007:  call InvokeFast
    IL_000c:  ldarg.1
    IL_000d:  ldarg.2
    IL_000e:  tail.
    IL_0010:  call InvokeFast
    IL_0015:  ret

Test::main
  (13,5-13,26)  let y:int = sum 2 3 4
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.3
    IL_0002:  call Test::<sum>__debug@13
    IL_0007:  ldc.i4.4
    IL_0008:  callvirt Invoke
    IL_000d:  stloc.0

  (14,5-14,18)  if y = 9 then
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s 9
    IL_0011:  bne.un.s IL_0015

  (14,19-14,20)  0
    IL_0013:  ldc.i4.0
    IL_0014:  ret

  (14,26-14,27)  1
    IL_0015:  ldc.i4.1
    IL_0016:  ret

Test::<sum>__debug@9-1
  (6,36-6,69)  (T $ Unchecked.defaultof<'r>) i x
    IL_0000:  call T::get_T
    IL_0005:  ldc.i4.0
    IL_0006:  call T::op_Dollar
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  tail.
    IL_000f:  call InvokeFast
    IL_0014:  ret

Test::<sum>__debug@13
  (6,36-6,69)  (T $ Unchecked.defaultof<'r>) i x
    IL_0000:  call T::get_T
    IL_0005:  ldnull
    IL_0006:  call Test::<op_Dollar>__debug@6
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  tail.
    IL_000f:  call InvokeFast
    IL_0014:  ret

T::CompareTo
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_000a

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_0008

  <hidden>
    IL_0006:  ldc.i4.0
    IL_0007:  ret

  <hidden>
    IL_0008:  ldc.i4.1
    IL_0009:  ret

  <hidden>
    IL_000a:  ldarg.1
    IL_000b:  brfalse.s IL_000f

  <hidden>
    IL_000d:  ldc.i4.m1
    IL_000e:  ret

  <hidden>
    IL_000f:  ldc.i4.0
    IL_0010:  ret

T::CompareTo
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  unbox.any T
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  brfalse.s IL_0016

  <hidden>
    IL_000a:  ldarg.1
    IL_000b:  unbox.any T
    IL_0010:  brfalse.s IL_0014

  <hidden>
    IL_0012:  ldc.i4.0
    IL_0013:  ret

  <hidden>
    IL_0014:  ldc.i4.1
    IL_0015:  ret

  <hidden>
    IL_0016:  ldarg.1
    IL_0017:  unbox.any T
    IL_001c:  brfalse.s IL_0020

  <hidden>
    IL_001e:  ldc.i4.m1
    IL_001f:  ret

  <hidden>
    IL_0020:  ldc.i4.0
    IL_0021:  ret

T::GetHashCode
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0009

  <hidden>
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldarg.0
    IL_0006:  pop
    IL_0007:  ldc.i4.0
    IL_0008:  ret

  <hidden>
    IL_0009:  ldc.i4.0
    IL_000a:  ret

T::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_000c

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_000a

  <hidden>
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ret

  <hidden>
    IL_000a:  ldc.i4.0
    IL_000b:  ret

  <hidden>
    IL_000c:  ldarg.1
    IL_000d:  ldnull
    IL_000e:  cgt.un
    IL_0010:  ldc.i4.0
    IL_0011:  ceq
    IL_0013:  ret

T::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst T
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0013

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  callvirt T::Equals
    IL_0012:  ret

  <hidden>
    IL_0013:  ldc.i4.0
    IL_0014:  ret

T::op_Dollar
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  stloc.0

  (3,37-3,38)  +
    IL_0002:  ldsfld op_Dollar@3::@_instance
    IL_0007:  ret

T::op_Dollar
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  stloc.0

  (4,41-4,42)  +
    IL_0002:  ldsfld op_Dollar@4-1::@_instance
    IL_0007:  ret

T::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0008

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  ldnull
    IL_0005:  cgt.un
    IL_0007:  ret

  <hidden>
    IL_0008:  ldarg.1
    IL_0009:  ldnull
    IL_000a:  cgt.un
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  ret

T::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst T
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0012

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt T::Equals
    IL_0011:  ret

  <hidden>
    IL_0012:  ldc.i4.0
    IL_0013:  ret

op_Dollar@9-2::Invoke
  (9,67-9,78)  sum (x + a)
    IL_0000:  ldarg.2
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.2
    IL_0004:  ldloc.1
    IL_0005:  ldloc.2
    IL_0006:  call LanguagePrimitives::AdditionDynamic
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  newobj .ctor
    IL_0012:  ret

op_Dollar@9-4::Invoke
  (9,67-9,78)  sum (x + a)
    IL_0000:  ldarg.2
    IL_0001:  stloc.1
    IL_0002:  ldarg.1
    IL_0003:  stloc.2
    IL_0004:  ldarg.0
    IL_0005:  ldfld op_Addition
    IL_000a:  ldloc.1
    IL_000b:  ldloc.2
    IL_000c:  call InvokeFast
    IL_0011:  stloc.0
    IL_0012:  ldarg.0
    IL_0013:  ldfld op_Addition
    IL_0018:  ldarg.0
    IL_0019:  ldfld op_Dollar
    IL_001e:  ldloc.0
    IL_001f:  newobj .ctor
    IL_0024:  ret

y@9::Invoke
  (9,67-9,78)  sum (x + a)
    IL_0000:  ldarg.2
    IL_0001:  ldarg.1
    IL_0002:  add
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  newobj y@9-1::.ctor
    IL_000a:  ret
