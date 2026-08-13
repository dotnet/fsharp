let inline getLength (x: ^T) =
    (^T : (member Length : int) x)

[<EntryPoint>]
let main _ =
    let i = getLength "hello"
    let j = getLength [1; 2; 3]
    if i = 5 && j = 3 then 0 else 1
--------------------------------------------------------------------------------

Test::getLength
  (3,5-3,35)  (^T : (member Length : int) x)
    IL_0000:  ldstr "Dynamic invocation of get_Length is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::getLength$W
  (3,5-3,35)  (^T : (member Length : int) x)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  tail.
    IL_0004:  callvirt Invoke
    IL_0009:  ret

Test::main
  (7,5-7,30)  let i = getLength "hello"
    IL_0000:  ldstr "hello"
    IL_0005:  call Test::<getLength>__debug@7
    IL_000a:  stloc.0

  (8,5-8,32)  let j = getLength [1; 2; 3]
    IL_000b:  ldc.i4.1
    IL_000c:  ldc.i4.2
    IL_000d:  ldc.i4.3
    IL_000e:  call get_Empty
    IL_0013:  call Cons
    IL_0018:  call Cons
    IL_001d:  call Cons
    IL_0022:  call Test::<getLength>__debug@8-1
    IL_0027:  stloc.1

  (9,5-9,27)  if i = 5 && j = 3 then
    IL_0028:  nop

  (9,8-9,13)  i = 5
    IL_0029:  ldloc.0
    IL_002a:  ldc.i4.5
    IL_002b:  bne.un.s IL_0034

  (9,17-9,22)  j = 3
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.3
    IL_002f:  ceq

  <hidden>
    IL_0031:  nop
    IL_0032:  br.s IL_0036

  <hidden>
    IL_0034:  ldc.i4.0

  <hidden>
    IL_0035:  nop
    IL_0036:  brfalse.s IL_003a

  (9,28-9,29)  0
    IL_0038:  ldc.i4.0
    IL_0039:  ret

  (9,35-9,36)  1
    IL_003a:  ldc.i4.1
    IL_003b:  ret

Test::<getLength>__debug@7
  (3,5-3,35)  (^T : (member Length : int) x)
    IL_0000:  ldarg.0
    IL_0001:  callvirt String::get_Length
    IL_0006:  ret

Test::<getLength>__debug@8-1
  (3,5-3,35)  (^T : (member Length : int) x)
    IL_0000:  ldarg.0
    IL_0001:  tail.
    IL_0003:  callvirt get_Length
    IL_0008:  ret
