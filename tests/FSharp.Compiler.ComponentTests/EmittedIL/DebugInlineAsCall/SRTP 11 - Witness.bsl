type MyNum =
    { Value: float }
    static member FromFloat (_: MyNum, _: T) = fun (x: float) -> { Value = x }

and T =
    { Dummy: int }
    static member inline Invoke(x: float) : 'Num =
        let inline call2 (a: ^a, b: ^b) = ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
        let inline call (a: 'a) = fun (x: 'x) -> call2 (a, Unchecked.defaultof<'r>) x : 'r
        call Unchecked.defaultof<T> x

[<EntryPoint>]
let main _ =
    let result = T.Invoke<MyNum>(2.71)
    if result.Value = 2.71 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (15,5-15,39)  let result = T.Invoke<MyNum>(2.71)
    IL_0000:  ldc.r8 2.710000
    IL_0009:  call Test::<Invoke>__debug@15
    IL_000e:  stloc.0

  (16,5-16,32)  if result.Value = 2.71 then
    IL_000f:  ldloc.0
    IL_0010:  ldfld MyNum::Value@
    IL_0015:  ldc.r8 2.710000
    IL_001e:  ceq
    IL_0020:  brfalse.s IL_0024

  (16,33-16,34)  0
    IL_0022:  ldc.i4.0
    IL_0023:  ret

  (16,40-16,41)  1
    IL_0024:  ldc.i4.1
    IL_0025:  ret

Test::<Invoke>__debug@15
  <hidden>
    IL_0000:  ldsfld call2@9-4::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldsfld call@10-6::@_instance
    IL_000b:  stloc.1

  (11,9-11,38)  call Unchecked.defaultof<T> x
    IL_000c:  ldsfld result@10::@_instance
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  ldnull
    IL_0014:  ldarg.0
    IL_0015:  tail.
    IL_0017:  call InvokeFast
    IL_001c:  ret

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
  (4,66-4,79)  { Value = x }
    IL_0000:  ldarg.1
    IL_0001:  newobj MyNum::.ctor
    IL_0006:  ret

T::CompareTo
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
    IL_000d:  ldfld T::Dummy@
    IL_0012:  stloc.1
    IL_0013:  ldarg.1
    IL_0014:  ldfld T::Dummy@
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

T::CompareTo
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  unbox.any T
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    IL_0009:  ldarg.0
    IL_000a:  brfalse.s IL_0033

  <hidden>
    IL_000c:  ldarg.1
    IL_000d:  unbox.any T
    IL_0012:  brfalse.s IL_0031

  <hidden>
    IL_0014:  ldarg.2
    IL_0015:  stloc.2
    IL_0016:  ldarg.0
    IL_0017:  ldfld T::Dummy@
    IL_001c:  stloc.3
    IL_001d:  ldloc.1
    IL_001e:  ldfld T::Dummy@
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
    IL_0034:  unbox.any T
    IL_0039:  brfalse.s IL_003d

  <hidden>
    IL_003b:  ldc.i4.m1
    IL_003c:  ret

  <hidden>
    IL_003d:  ldc.i4.0
    IL_003e:  ret

T::GetHashCode
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
    IL_000d:  ldfld T::Dummy@
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

T::Equals
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
    IL_000b:  ldfld T::Dummy@
    IL_0010:  ldloc.0
    IL_0011:  ldfld T::Dummy@
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

T::Invoke
  <hidden>
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldsfld @_instance
    IL_000b:  stloc.1

  (11,9-11,38)  call Unchecked.defaultof<T> x
    IL_000c:  ldsfld @_instance
    IL_0011:  stloc.2
    IL_0012:  ldloc.2
    IL_0013:  ldnull
    IL_0014:  ldarg.0
    IL_0015:  tail.
    IL_0017:  call InvokeFast
    IL_001c:  ret

T::Invoke$W
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  newobj .ctor
    IL_000d:  stloc.1

  (11,9-11,38)  call Unchecked.defaultof<T> x
    IL_000e:  ldarg.0
    IL_000f:  newobj .ctor
    IL_0014:  stloc.2
    IL_0015:  ldloc.2
    IL_0016:  ldnull
    IL_0017:  ldarg.1
    IL_0018:  tail.
    IL_001a:  call InvokeFast
    IL_001f:  ret

T::Equals
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s IL_0017

  <hidden>
    IL_0003:  ldarg.1
    IL_0004:  brfalse.s IL_0015

  <hidden>
    IL_0006:  ldarg.0
    IL_0007:  ldfld T::Dummy@
    IL_000c:  ldarg.1
    IL_000d:  ldfld T::Dummy@
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

call2@9-1::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw

call@9-2::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw

Invoke@9-1::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw

call2@9-3::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldarg.0
    IL_000f:  ldfld fromFloat0
    IL_0014:  ldloc.1
    IL_0015:  ldloc.0
    IL_0016:  tail.
    IL_0018:  call InvokeFast
    IL_001d:  ret

call@9-5::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldarg.0
    IL_000f:  ldfld fromFloat0
    IL_0014:  ldloc.1
    IL_0015:  ldloc.0
    IL_0016:  tail.
    IL_0018:  call InvokeFast
    IL_001d:  ret

Invoke@9-3::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldarg.0
    IL_000f:  ldfld fromFloat
    IL_0014:  ldloc.1
    IL_0015:  ldloc.0
    IL_0016:  tail.
    IL_0018:  call InvokeFast
    IL_001d:  ret

call@10-1::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldloca.s 1
    IL_000a:  initobj 0x1b00002c
    IL_0010:  ldloc.1
    IL_0011:  newobj .ctor
    IL_0016:  ldarg.2
    IL_0017:  tail.
    IL_0019:  call InvokeFast
    IL_001e:  ret

Invoke@10::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldloca.s 1
    IL_000a:  initobj 0x1b000031
    IL_0010:  ldloc.1
    IL_0011:  newobj .ctor
    IL_0016:  ldarg.2
    IL_0017:  tail.
    IL_0019:  call InvokeFast
    IL_001e:  ret

call@10-4::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldarg.0
    IL_0001:  ldfld fromFloat
    IL_0006:  ldarg.0
    IL_0007:  ldfld fromFloat0
    IL_000c:  newobj .ctor
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  ldarg.1
    IL_0014:  ldloca.s 1
    IL_0016:  initobj 0x1b000035
    IL_001c:  ldloc.1
    IL_001d:  newobj .ctor
    IL_0022:  ldarg.2
    IL_0023:  tail.
    IL_0025:  call InvokeFast
    IL_002a:  ret

Invoke@10-2::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldarg.0
    IL_0001:  ldfld fromFloat
    IL_0006:  newobj .ctor
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldarg.1
    IL_000e:  ldloca.s 1
    IL_0010:  initobj 0x1b000031
    IL_0016:  ldloc.1
    IL_0017:  newobj .ctor
    IL_001c:  ldarg.2
    IL_001d:  tail.
    IL_001f:  call InvokeFast
    IL_0024:  ret

call2@9-5::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw

call@9-8::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldstr "Dynamic invocation of FromFloat is not supported"
    IL_0013:  newobj NotSupportedException::.ctor
    IL_0018:  throw

result@9-1::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (9,43-9,103)  ((^a or ^b) : (static member FromFloat : _ * _ -> _) (b, a))
    IL_000e:  ldloc.1
    IL_000f:  ldloc.0
    IL_0010:  tail.
    IL_0012:  call MyNum::FromFloat
    IL_0017:  ret

call@10-7::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldloc.1
    IL_0009:  newobj .ctor
    IL_000e:  ldarg.2
    IL_000f:  tail.
    IL_0011:  call InvokeFast
    IL_0016:  ret

result@10::Invoke
  (10,50-10,86)  call2 (a, Unchecked.defaultof<'r>) x
    IL_0000:  ldsfld result@9-1::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldnull
    IL_0009:  newobj .ctor
    IL_000e:  ldarg.2
    IL_000f:  tail.
    IL_0011:  call InvokeFast
    IL_0016:  ret
