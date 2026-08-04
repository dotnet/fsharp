let inline apply ([<InlineIfLambda>] f: int -> int) (x: int) : int =
    f x

[<EntryPoint>]
let main _ =
    let result = apply (fun i -> i + 1) 5
    if result = 6 then 0 else 1
--------------------------------------------------------------------------------

Test::apply
  (3,5-3,8)  f x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  tail.
    IL_0004:  callvirt Invoke
    IL_0009:  ret

Test::main
  (7,5-7,42)  let result = apply (fun i -> i + 1) 5
    IL_0000:  ldsfld result@7::@_instance
    IL_0005:  ldc.i4.5
    IL_0006:  call Test::apply
    IL_000b:  stloc.0

  (8,5-8,23)  if result = 6 then
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.6
    IL_000e:  bne.un.s IL_0012

  (8,24-8,25)  0
    IL_0010:  ldc.i4.0
    IL_0011:  ret

  (8,31-8,32)  1
    IL_0012:  ldc.i4.1
    IL_0013:  ret

result@7::Invoke
  (7,34-7,39)  i + 1
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  ret
