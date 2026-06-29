let inline apply (f: 'a -> 'b -> 'c) (x: 'a) (y: 'b) : 'c =
    f x y

[<EntryPoint>]
let main _ =
    let i = apply (+) 3 4
    if i = 7 then 0 else 1
--------------------------------------------------------------------------------

Test::apply
  (3,5-3,10)  f x y
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  tail.
    IL_0005:  call InvokeFast
    IL_000a:  ret

Test::main
  (7,5-7,26)  let i = apply (+) 3 4
    IL_0000:  ldsfld i@7::@_instance
    IL_0005:  ldc.i4.3
    IL_0006:  ldc.i4.4
    IL_0007:  call Test::apply
    IL_000c:  stloc.0

  (8,5-8,18)  if i = 7 then
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.7
    IL_000f:  bne.un.s IL_0013

  (8,19-8,20)  0
    IL_0011:  ldc.i4.0
    IL_0012:  ret

  (8,26-8,27)  1
    IL_0013:  ldc.i4.1
    IL_0014:  ret
