type MyBuilder<'T>() =
    member _.M(a1: byref<int>, a2: byref<int>, a3: byref<int>,
               a4: byref<int>, a5: byref<int>, a6: byref<int>,
               a7: byref<int>, a8: byref<int>) =
        a1 <- 1

let inline callMember<'Builder, 'A
    when 'Builder: (member M: byref<'A> * byref<'A> * byref<'A> * byref<'A>
                            * byref<'A> * byref<'A> * byref<'A> * byref<'A> -> unit)>
    (builder: 'Builder,
     a1: byref<'A>, a2: byref<'A>, a3: byref<'A>, a4: byref<'A>,
     a5: byref<'A>, a6: byref<'A>, a7: byref<'A>, a8: byref<'A>) =
    builder.M(&a1, &a2, &a3, &a4, &a5, &a6, &a7, &a8)

let runDynamic (builder: MyBuilder<'T>) =
    let mutable x = 0
    callMember (builder, &x, &x, &x, &x, &x, &x, &x, &x)

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
--------------------------------------------------------------------------------

Test::callMember
  (14,5-14,14)  builder.M
    IL_0000:  ldstr "Dynamic invocation of M is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::callMember$W
  (14,5-14,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  ldarg.s 4
    IL_0006:  ldarg.s 5
    IL_0008:  ldarg.s 6
    IL_000a:  ldarg.s 7
    IL_000c:  ldarg.s 8
    IL_000e:  ldarg.s 9
    IL_0010:  stloc.0
    IL_0011:  stloc.1
    IL_0012:  stloc.2
    IL_0013:  stloc.3
    IL_0014:  call InvokeFast
    IL_0019:  ldloc.3
    IL_001a:  ldloc.2
    IL_001b:  ldloc.1
    IL_001c:  ldloc.0
    IL_001d:  call InvokeFast
    IL_0022:  pop
    IL_0023:  ret

Test::runDynamic
  (17,5-17,22)  let mutable x = 0
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0

  (18,5-18,57)  callMember (builder, &x, &x, &x, &x, &x, &x, &x, &x)
    IL_0002:  ldarg.0
    IL_0003:  ldloca.s 0
    IL_0005:  ldloca.s 0
    IL_0007:  ldloca.s 0
    IL_0009:  ldloca.s 0
    IL_000b:  ldloca.s 0
    IL_000d:  ldloca.s 0
    IL_000f:  ldloca.s 0
    IL_0011:  ldloca.s 0
    IL_0013:  call Test::<callMember>__debug@18
    IL_0018:  nop
    IL_0019:  ret

Test::main
  (22,5-22,34)  runDynamic (MyBuilder<int>())
    IL_0000:  newobj .ctor
    IL_0005:  call Test::runDynamic
    IL_000a:  nop

  (23,5-23,6)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Test::<callMember>__debug@18
  (14,5-14,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  ldarg.s 4
    IL_0006:  ldarg.s 5
    IL_0008:  ldarg.s 6
    IL_000a:  ldarg.s 7
    IL_000c:  ldarg.s 8
    IL_000e:  callvirt M
    IL_0013:  nop
    IL_0014:  ret

MyBuilder`1::.ctor
  (2,6-2,15)  MyBuilder
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

MyBuilder`1::M
  (6,9-6,16)  a1 <- 1
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  stobj Int32
    IL_0007:  ret
