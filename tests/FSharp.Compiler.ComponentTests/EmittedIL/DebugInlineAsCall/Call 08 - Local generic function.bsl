[<EntryPoint>]
let main _ =
    let inline apply (f: 'a -> 'b) (x: 'a) : 'b = f x
    let i = apply (fun x -> x + 1) 5
    if i = 6 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  <hidden>
    IL_0000:  ldsfld apply@4::@_instance
    IL_0005:  stloc.0

  (5,5-5,37)  let i = apply (fun x -> x + 1) 5
    IL_0006:  ldloc.0
    IL_0007:  ldsfld i@5::@_instance
    IL_000c:  ldc.i4.5
    IL_000d:  stloc.2
    IL_000e:  stloc.3
    IL_000f:  callvirt FSharpTypeFunc::Specialize
    IL_0014:  unbox.any FSharpTypeFunc
    IL_0019:  callvirt FSharpTypeFunc::Specialize
    IL_001e:  unbox.any 0x1b000003
    IL_0023:  ldloc.3
    IL_0024:  ldloc.2
    IL_0025:  call InvokeFast
    IL_002a:  stloc.1

  (6,5-6,18)  if i = 6 then
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.6
    IL_002d:  bne.un.s IL_0031

  (6,19-6,20)  0
    IL_002f:  ldc.i4.0
    IL_0030:  ret

  (6,26-6,27)  1
    IL_0031:  ldc.i4.1
    IL_0032:  ret

apply@4TT::Invoke
  (4,51-4,54)  f x
    IL_0000:  ldarg.0
    IL_0001:  ldfld self1@
    IL_0006:  stloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldfld self0@
    IL_000d:  stloc.0
    IL_000e:  ldarg.1
    IL_000f:  ldarg.2
    IL_0010:  tail.
    IL_0012:  callvirt Invoke
    IL_0017:  ret

i@5::Invoke
  (5,29-5,34)  x + 1
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  ret
