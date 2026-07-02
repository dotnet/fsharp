type MyBuilder<'T>() =
    member _.M(a: byref<int>, b: byref<int>) = ()

let inline callMember<'Builder, 'A
    when 'Builder: (member M: byref<'A> * byref<'A> -> unit)>
    (builder: 'Builder, a: byref<'A>, b: byref<'A>) =
    builder.M(&a, &b)

let runDynamic (builder: MyBuilder<'T>) =
    let mutable x = 0
    callMember (builder, &x, &x)

[<EntryPoint>]
let main _ =
    runDynamic (MyBuilder<int>())
    0
--------------------------------------------------------------------------------

Test::callMember
  (8,5-8,14)  builder.M
    IL_0000:  ldstr "Dynamic invocation of M is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::callMember$W
  (8,5-8,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.3
    IL_0004:  call InvokeFast
    IL_0009:  pop
    IL_000a:  ret

Test::runDynamic
  (11,5-11,22)  let mutable x = 0
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0

  (12,5-12,33)  callMember (builder, &x, &x)
    IL_0002:  ldarg.0
    IL_0003:  ldloca.s 0
    IL_0005:  ldloca.s 0
    IL_0007:  call Test::<callMember>__debug@12
    IL_000c:  nop
    IL_000d:  ret

Test::main
  (16,5-16,34)  runDynamic (MyBuilder<int>())
    IL_0000:  newobj .ctor
    IL_0005:  call Test::runDynamic
    IL_000a:  nop

  (17,5-17,6)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Test::<callMember>__debug@12
  (8,5-8,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  callvirt M
    IL_0008:  nop
    IL_0009:  ret

MyBuilder`1::.ctor
  (2,6-2,15)  MyBuilder
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

MyBuilder`1::M
  (3,48-3,50)  ()
    IL_0000:  ret
