open Microsoft.FSharp.Core.CompilerServices

[<Struct>]
type S = member this.Foo() = ()

type D = member _.Bar() = true

let inline f (x: ^A) =
    ResumableCode< ^B, _>(fun sm ->
        (^A: (member Foo: unit -> unit) x)
        (^B: (member Bar: unit -> bool) sm.Data)
    )

let inline g () = f (S())

[<EntryPoint>]
let main _ =
    let _ : ResumableCode<D, _> = g ()
    0
--------------------------------------------------------------------------------

Test::f
  (10,26-13,6)  (fun sm -> (^A: (member Foo: unit -> unit) x) (^B: (member Bar: unit -> bool) sm.Data) )
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

Test::f$W
  (10,26-13,6)  (fun sm -> (^A: (member Foo: unit -> unit) x) (^B: (member Bar: unit -> bool) sm.Data) )
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  newobj .ctor
    IL_0008:  ldftn Invoke
    IL_000e:  newobj .ctor
    IL_0013:  ret

Test::g
  (15,19-15,26)  f (S())
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  tail.
    IL_000a:  callvirt Invoke
    IL_000f:  ret

Test::g$W
  (15,19-15,26)  f (S())
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.1
    IL_0009:  tail.
    IL_000b:  callvirt Invoke
    IL_0010:  ret

Test::main
  (19,35-19,39)  g ()
    IL_0000:  call Test::<g>__debug@19
    IL_0005:  pop

  (20,5-20,6)  0
    IL_0006:  ldc.i4.0
    IL_0007:  ret

Test::<g>__debug@19
  (15,19-15,26)  f (S())
    IL_0000:  ldsfld main@9::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  tail.
    IL_000a:  callvirt Invoke
    IL_000f:  ret

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

S::Foo
  (5,30-5,32)  ()
    IL_0000:  ret

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

D::Bar
  (7,27-7,31)  true
    IL_0000:  ldc.i4.1
    IL_0001:  ret

g@9::Invoke
  (10,26-13,6)  (fun sm -> (^A: (member Foo: unit -> unit) x) (^B: (member Bar: unit -> bool) sm.Data) )
    IL_0000:  ldarg.1
    IL_0001:  newobj .ctor
    IL_0006:  ldftn Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

g@9-2::Invoke
  (10,26-13,6)  (fun sm -> (^A: (member Foo: unit -> unit) x) (^B: (member Bar: unit -> bool) sm.Data) )
    IL_0000:  ldarg.0
    IL_0001:  ldfld bar
    IL_0006:  ldarg.1
    IL_0007:  newobj .ctor
    IL_000c:  ldftn Invoke
    IL_0012:  newobj .ctor
    IL_0017:  ret

main@9::Invoke
  (10,26-13,6)  (fun sm -> (^A: (member Foo: unit -> unit) x) (^B: (member Bar: unit -> bool) sm.Data) )
    IL_0000:  ldarg.1
    IL_0001:  newobj main@10-1::.ctor
    IL_0006:  ldftn main@10-1::Invoke
    IL_000c:  newobj .ctor
    IL_0011:  ret

f@10::Invoke
  (11,9-11,43)  (^A: (member Foo: unit -> unit) x)
    IL_0000:  ldc.i4.0
    IL_0001:  brfalse.s IL_000b
    IL_0003:  ldnull
    IL_0004:  unbox.any Unit
    IL_0009:  br.s IL_0016
    IL_000b:  ldstr "Dynamic invocation of Foo is not supported"
    IL_0010:  newobj NotSupportedException::.ctor
    IL_0015:  throw
    IL_0016:  pop

  (12,9-12,49)  (^B: (member Bar: unit -> bool) sm.Data)
    IL_0017:  ldstr "Dynamic invocation of Bar is not supported"
    IL_001c:  newobj NotSupportedException::.ctor
    IL_0021:  throw

f@10-1::Invoke
  (11,9-11,43)  (^A: (member Foo: unit -> unit) x)
    IL_0000:  ldarg.0
    IL_0001:  ldfld foo
    IL_0006:  ldarg.0
    IL_0007:  ldfld x
    IL_000c:  callvirt Invoke
    IL_0011:  pop

  (12,9-12,49)  (^B: (member Bar: unit -> bool) sm.Data)
    IL_0012:  ldarg.0
    IL_0013:  ldfld bar
    IL_0018:  ldarg.1
    IL_0019:  ldfld Data
    IL_001e:  tail.
    IL_0020:  callvirt Invoke
    IL_0025:  ret

g@10-1::Invoke
  (11,9-11,43)  (^A: (member Foo: unit -> unit) x)
    IL_0000:  ldarg.0
    IL_0001:  ldflda x
    IL_0006:  call S::Foo
    IL_000b:  nop

  (12,9-12,49)  (^B: (member Bar: unit -> bool) sm.Data)
    IL_000c:  ldstr "Dynamic invocation of Bar is not supported"
    IL_0011:  newobj NotSupportedException::.ctor
    IL_0016:  throw

g@10-3::Invoke
  (11,9-11,43)  (^A: (member Foo: unit -> unit) x)
    IL_0000:  ldarg.0
    IL_0001:  ldflda x
    IL_0006:  call S::Foo
    IL_000b:  nop

  (12,9-12,49)  (^B: (member Bar: unit -> bool) sm.Data)
    IL_000c:  ldarg.0
    IL_000d:  ldfld bar
    IL_0012:  ldarg.1
    IL_0013:  ldfld Data
    IL_0018:  tail.
    IL_001a:  callvirt Invoke
    IL_001f:  ret

main@10-1::Invoke
  (11,9-11,43)  (^A: (member Foo: unit -> unit) x)
    IL_0000:  ldarg.0
    IL_0001:  ldflda main@10-1::x
    IL_0006:  call S::Foo
    IL_000b:  nop

  (12,9-12,49)  (^B: (member Bar: unit -> bool) sm.Data)
    IL_000c:  ldarg.1
    IL_000d:  ldfld Data
    IL_0012:  callvirt D::Bar
    IL_0017:  ret
