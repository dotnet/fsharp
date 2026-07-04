type MyNum =
    { Value: float }
    static member FromFloat (_: MyNum) = fun (x: float) -> { Value = x }

type T =
    static member inline Invoke(x: float) : 'Num =
        let inline call (a: ^a) = (^a: (static member FromFloat : _ -> _) a)
        call Unchecked.defaultof<'Num> x

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(3.14)
    if result.Value = 3.14 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (13,5-13,39)  let result = T.Invoke<MyNum>(3.14)
    IL_0000:  ldc.r8 3.140000
    IL_0009:  call Test::<Invoke>__debug@13
    IL_000e:  stloc.0

  (14,5-14,32)  if result.Value = 3.14 then
    IL_000f:  ldloc.0
    IL_0010:  ldfld MyNum::Value@
    IL_0015:  ldc.r8 3.140000
    IL_001e:  ceq
    IL_0020:  brfalse.s IL_0024

  (14,33-14,34)  0
    IL_0022:  ldc.i4.0
    IL_0023:  ret

  (14,40-14,41)  1
    IL_0024:  ldc.i4.1
    IL_0025:  ret

Test::<Invoke>__debug@13
  <hidden>
    IL_0000:  ldsfld call@8-4::@_instance
    IL_0005:  stloc.0

  (9,9-9,41)  call Unchecked.defaultof<'Num> x
    IL_0006:  ldsfld result@8::@_instance
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  ldnull
    IL_000e:  ldarg.0
    IL_000f:  tail.
    IL_0011:  call InvokeFast
    IL_0016:  ret

MyNum::CompareTo
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_003f

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_003d

  <hidden>
    IL_0006:  call LanguagePrimitives::get_GenericComparer
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  ldfld MyNum::Value@
    IL_0012:  stloc.1
    IL_0013:  ldarg.1
    IL_0014:  ldfld MyNum::Value@
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldloc.2
    IL_001c:  clt
    IL_001e:  brfalse.s IL_0022

  <hidden>
    IL_0020:  ldc.i4.m1
    IL_0021:  ret

  <hidden>
    IL_0022:  ldloc.1
    IL_0023:  ldloc.2
    IL_0024:  cgt
    IL_0026:  brfalse.s IL_002a

  <hidden>
    IL_0028:  ldc.i4.1
    IL_0029:  ret

  <hidden>
    IL_002a:  ldloc.1
    IL_002b:  ldloc.2
    IL_002c:  ceq
    IL_002e:  brfalse.s IL_0032

  <hidden>
    IL_0030:  ldc.i4.0
    IL_0031:  ret

  <hidden>
    IL_0032:  ldloc.0
    IL_0033:  ldloc.1
    IL_0034:  ldloc.2
    IL_0035:  tail.
    IL_0037:  call HashCompare::GenericComparisonWithComparerIntrinsic
    IL_003c:  ret

  <hidden>
    IL_003d:  ldc.i4.1
    IL_003e:  ret

  <hidden>
    IL_003f:  ldarg.1
    IL_0040:  brfalse.s IL_0044

  <hidden>
    IL_0042:  ldc.i4.m1
    IL_0043:  ret

  <hidden>
    IL_0044:  ldc.i4.0
    IL_0045:  ret

MyNum::CompareTo
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  unbox.any MyNum
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s IL_004e

  <hidden>
    IL_000c:  ldarg.1
    IL_000d:  unbox.any MyNum
    IL_0012:  brfalse.s IL_004c

  <hidden>
    IL_0014:  ldarg.2
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldfld MyNum::Value@
    IL_001c:  stloc.3
    IL_001d:  ldloc.1
    IL_001e:  ldfld MyNum::Value@
    IL_0023:  stloc.s 4
    IL_0025:  ldloc.3
    IL_0026:  ldloc.s 4
    IL_0028:  clt
    IL_002a:  brfalse.s IL_002e

  <hidden>
    IL_002c:  ldc.i4.m1
    IL_002d:  ret

  <hidden>
    IL_002e:  ldloc.3
    IL_002f:  ldloc.s 4
    IL_0031:  cgt
    IL_0033:  brfalse.s IL_0037

  <hidden>
    IL_0035:  ldc.i4.1
    IL_0036:  ret

  <hidden>
    IL_0037:  ldloc.3
    IL_0038:  ldloc.s 4
    IL_003a:  ceq
    IL_003c:  brfalse.s IL_0040

  <hidden>
    IL_003e:  ldc.i4.0
    IL_003f:  ret

  <hidden>
    IL_0040:  ldloc.2
    IL_0041:  ldloc.3
    IL_0042:  ldloc.s 4
    IL_0044:  tail.
    IL_0046:  call HashCompare::GenericComparisonWithComparerIntrinsic
    IL_004b:  ret

  <hidden>
    IL_004c:  ldc.i4.1
    IL_004d:  ret

  <hidden>
    IL_004e:  ldarg.1
    IL_004f:  unbox.any MyNum
    IL_0054:  brfalse.s IL_0058

  <hidden>
    IL_0056:  ldc.i4.m1
    IL_0057:  ret

  <hidden>
    IL_0058:  ldc.i4.0
    IL_0059:  ret

MyNum::GetHashCode
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0022

  <hidden>
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.0
    IL_0005:  ldc.i4 -1640531527
    IL_000a:  ldarg.1
    IL_000b:  ldarg.0
    IL_000c:  ldfld MyNum::Value@
    IL_0011:  call HashCompare::GenericHashWithComparerIntrinsic
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.6
    IL_0018:  shl
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  shr
    IL_001c:  add
    IL_001d:  add
    IL_001e:  add
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ret

  <hidden>
    IL_0022:  ldc.i4.0
    IL_0023:  ret

MyNum::Equals
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
    IL_000b:  ldfld MyNum::Value@
    IL_0010:  ldloc.0
    IL_0011:  ldfld MyNum::Value@
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

MyNum::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst MyNum
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0013

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.2
    IL_000d:  callvirt MyNum::Equals
    IL_0012:  ret

  <hidden>
    IL_0013:  ldc.i4.0
    IL_0014:  ret

MyNum::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_002c

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_002a

  <hidden>
    IL_0006:  ldarg.0
    IL_0007:  ldfld MyNum::Value@
    IL_000c:  stloc.0
    IL_000d:  ldarg.1
    IL_000e:  ldfld MyNum::Value@
    IL_0013:  stloc.1
    IL_0014:  ldloc.0
    IL_0015:  ldloc.1
    IL_0016:  ceq
    IL_0018:  brfalse.s IL_001c

  <hidden>
    IL_001a:  ldc.i4.1
    IL_001b:  ret

  <hidden>
    IL_001c:  ldloc.0
    IL_001d:  ldloc.0
    IL_001e:  beq.s IL_0028

  <hidden>
    IL_0020:  ldloc.1
    IL_0021:  ldloc.1
    IL_0022:  ceq
    IL_0024:  ldc.i4.0
    IL_0025:  ceq
    IL_0027:  ret

  <hidden>
    IL_0028:  ldc.i4.0
    IL_0029:  ret

  <hidden>
    IL_002a:  ldc.i4.0
    IL_002b:  ret

  <hidden>
    IL_002c:  ldarg.1
    IL_002d:  ldnull
    IL_002e:  cgt.un
    IL_0030:  ldc.i4.0
    IL_0031:  ceq
    IL_0033:  ret

MyNum::Equals
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  isinst MyNum
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s IL_0012

  <hidden>
    IL_000a:  ldarg.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt MyNum::Equals
    IL_0011:  ret

  <hidden>
    IL_0012:  ldc.i4.0
    IL_0013:  ret

FromFloat@4::Invoke
  (4,60-4,73)  { Value = x }
    IL_0000:  ldarg.1
    IL_0001:  newobj MyNum::.ctor
    IL_0006:  ret

T::Invoke
  <hidden>
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0

  (9,9-9,41)  call Unchecked.defaultof<'Num> x
    IL_0006:  ldsfld @_instance
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  ldloca.s 2
    IL_000f:  initobj 0x1b00000d
    IL_0015:  ldloc.2
    IL_0016:  ldarg.0
    IL_0017:  tail.
    IL_0019:  call InvokeFast
    IL_001e:  ret

T::Invoke$W
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0

  (9,9-9,41)  call Unchecked.defaultof<'Num> x
    IL_0007:  ldarg.0
    IL_0008:  newobj .ctor
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldloca.s 2
    IL_0011:  initobj 0x1b00000d
    IL_0017:  ldloc.2
    IL_0018:  ldarg.1
    IL_0019:  tail.
    IL_001b:  call InvokeFast
    IL_0020:  ret

call@8-1::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Invoke@8::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

call@8-3::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldarg.0
    IL_0001:  ldfld fromFloat0
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  callvirt Invoke
    IL_000e:  ret

Invoke@8-1::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldarg.0
    IL_0001:  ldfld fromFloat
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  callvirt Invoke
    IL_000e:  ret

call@8-5::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

result@8::Invoke
  (8,35-8,77)  (^a: (static member FromFloat : _ -> _) a)
    IL_0000:  ldarg.1
    IL_0001:  call MyNum::FromFloat
    IL_0006:  ret
