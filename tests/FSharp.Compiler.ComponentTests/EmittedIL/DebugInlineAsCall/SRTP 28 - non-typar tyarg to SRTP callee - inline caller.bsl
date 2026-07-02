type MyBuilder<'T>() =
    member _.M() = ()

let inline callMember<'Builder when 'Builder: (member M: unit -> unit)> (builder: 'Builder) =
    builder.M()

let inline outerInline<'T> (builder: MyBuilder<'T>) =
    callMember builder

[<EntryPoint>]
let main _ =
    outerInline (MyBuilder<int>())
    0
--------------------------------------------------------------------------------

Test::callMember
  (6,5-6,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldstr "Dynamic invocation of M is not supported"
    IL_0007:  newobj NotSupportedException::.ctor
    IL_000c:  throw

Test::callMember$W
  (6,5-6,14)  builder.M
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloc.0
    IL_0004:  callvirt Invoke
    IL_0009:  pop
    IL_000a:  ret

Test::outerInline
  (9,5-9,23)  callMember builder
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  call Test::<callMember>__debug@9
    IL_0008:  ret

Test::main
  (13,5-13,35)  outerInline (MyBuilder<int>())
    IL_0000:  newobj .ctor
    IL_0005:  call Test::outerInline
    IL_000a:  nop

  (14,5-14,6)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Test::<callMember>__debug@9
  (6,5-6,14)  builder.M
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
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
  (3,20-3,22)  ()
    IL_0000:  ret
